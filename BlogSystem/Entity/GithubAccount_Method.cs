using InfraStructure.DataBase;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace BlogSystem.Entity
{
    public partial class GithubAccount : EntityBase
    {
        /// <summary>
        /// GitHub
        /// </summary>
        public const string Github = nameof(Github);
        /// <summary>
        /// 根据登陆名称获得帐号
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static GithubAccount GetGitAccountByLogin(string login)
        {
            IMongoQuery AccountQuery = Query.EQ(nameof(Login), login);
            return MongoDbRepository.GetFirstRec<GithubAccount>(AccountQuery);
        }
        /// <summary>
        /// 根据ID获得名称
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetNameByAccountId(string accountId)
        {
            IMongoQuery AccountQuery = Query.EQ(MongoDbRepository.MongoKeyField, accountId);
            var acc = MongoDbRepository.GetFirstRec<GithubAccount>(AccountQuery);
            return acc != null ? acc.Name : string.Empty;
        }

        public static UserInfo GetUserInfoByRegMethodInnerId(string accountId)
        {
            IMongoQuery RegisterAccountIDQuery = Query.EQ(nameof(UserInfo.RegisterAccountID), accountId);
            IMongoQuery GithubQuery = Query.EQ(nameof(UserInfo.RegisterMethod), Github);
            return MongoDbRepository.GetFirstRec<UserInfo>(Query.And(RegisterAccountIDQuery, GithubQuery));
        }

        public static string ClientID = string.Empty;
        public static string ClientSecret = string.Empty;
        public static string AppName = string.Empty;
        public const string githubTokenUrl = "https://github.com/login/oauth/access_token";

        /// <summary>
        /// 获得login
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        private static GithubAccount GetGithubAccount(string Code)
        {
            try
            {
                var resonseJson = "";
                var webRequest = WebRequest.Create(githubTokenUrl) as HttpWebRequest;
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                //Github API V3必须加上下面这句话！
                webRequest.UserAgent = AppName;
                var postData = string.Format("code={0}&client_id={1}&client_secret={2}", Code, ClientID, ClientSecret);

                //在HTTP POST请求中传递参数
                using (var sw = new StreamWriter(webRequest.GetRequestStream()))
                {
                    sw.Write(postData);
                    sw.Close();
                }

                //发送请求，并获取服务器响应
                using (var response = webRequest.GetResponse())
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        resonseJson = sr.ReadToEnd();
                    }
                }

                var resultparms = resonseJson.Split("&".ToCharArray());
                var accessToken = string.Empty;
                foreach (var parm in resultparms)
                {
                    if (parm.StartsWith("access_token="))
                    {
                        accessToken = parm.Split("=".ToCharArray())[1];
                    }
                }
                if (string.IsNullOrEmpty(accessToken)) return null;
                webRequest = WebRequest.Create("https://api.github.com/user") as HttpWebRequest;
                webRequest.Method = "GET";
                webRequest.Headers.Add("Authorization", "token " + accessToken);
                //Github API V3必须加上下面这句话！
                webRequest.UserAgent = AppName;

                using (var response = webRequest.GetResponse())
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        dynamic obj = JsonConvert.DeserializeObject(sr.ReadToEnd());
                        GithubAccount gitlogin = new GithubAccount()
                        {
                            Login = obj.login,
                            Avatar_url = obj.avatar_url,
                            Email = obj.email == null ? "<未知>" : obj.email,
                            Name = obj.name == null ? obj.login : obj.name,
                            Html_url = obj.html_url,
                            Company = obj.company == null ? "<未知>" : obj.company,
                            Blog = obj.blog == null ? "<未知>" : obj.blog,
                            Location = obj.location == null ? "<未知>" : obj.location,
                            Followers = obj.followers,
                            Following = obj.following,
                            LastAccess = DateTime.Now
                        };
                        return gitlogin;
                    }
                }
            }
            catch (Exception ex)
            {
                InfraStructure.Log.ExceptionLog.Log("SYSTEM", "GitHubOAuth", "GET USER INFO", ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 获得用户
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(string Code)
        {
            var gitlogin = GetGithubAccount(Code);
            //获得GitAccount
            GithubAccount gitAccount = GetGitAccountByLogin(gitlogin.Login);
            if (gitAccount == null)
            {
                //没有该用户，添加
                var GitId = InsertGithubAccount(gitlogin);
                UserInfo userinfo = new UserInfo()
                {
                    RegisterAccountID = GitId,
                    TopicList = new List<string>(),
                    TagList = new List<string>(),
                    //具有50个追随者的普通是特约作者
                    Privilege = gitlogin.Followers >= 50 ? UserType.Author : UserType.Normal,
                    RegisterMethod = Github,
                    NickName = gitlogin.Name,
                    Avatar_url = gitlogin.Avatar_url,
                    ContainTag = string.Empty,
                    AntiTag = string.Empty,
                    Catalog = new List<string>(),
                    Level = new List<ArticleLevel>()
                };
                var userinfoId = UserInfo.InsertUserInfo(userinfo);
                if (userinfoId == 1.ToString(SnFormat))
                {
                    //第一个用户是管理员(该代码仅仅被执行一次)
                    userinfo.Privilege = UserType.Admin;
                    UserInfo.UpdateUserInfo(userinfo);
                }
                //更新UserInfoID
                gitlogin.UserInfoID = userinfoId;
                UpdateGithubAccount(gitlogin);

                //Welcome
                var articleurl = "<a href = '/Article/Index?ArticleId=00000006'>网站使用方法</a>";
                SiteMessage.CreateNotify(userinfoId, "欢迎加入CodeSnippet.info,请阅读[" + articleurl + "]");
                return userinfo;
            }
            else
            {
                //用新的信息替换旧的信息
                gitAccount.Avatar_url = gitlogin.Avatar_url;
                gitAccount.Blog = gitlogin.Blog;
                gitAccount.Company = gitlogin.Company;
                gitAccount.Email = gitlogin.Email;
                gitAccount.Followers = gitlogin.Followers;
                gitAccount.Following = gitlogin.Following;
                gitAccount.Html_url = gitlogin.Html_url;
                gitAccount.Location = gitlogin.Location;
                gitAccount.Name = gitlogin.Name;
                UpdateGithubAccount(gitAccount);
                var userinfo = GetUserInfoByRegMethodInnerId(gitAccount.Sn);
                //这里必须返回gitAccount，因为gitlogin没有UserInfoID
                userinfo.Avatar_url = gitlogin.Avatar_url;
                userinfo.NickName = gitlogin.Name;

                //防御性代码
                if (userinfo.TagList == null) userinfo.TagList = new List<string>();
                if (userinfo.TopicList == null) userinfo.TopicList = new List<string>();
                if (userinfo.Catalog == null) userinfo.Catalog = new List<string>();
                if (userinfo.Level == null) userinfo.Level = new List<ArticleLevel>();
                if (userinfo.ContainTag == null) userinfo.ContainTag = string.Empty;
                if (userinfo.AntiTag == null) userinfo.AntiTag = string.Empty;

                //具有50个追随者的普通人是特约作者
                if (gitlogin.Followers >= 50)
                {
                    if (userinfo.Privilege == UserType.Normal)
                    {
                        userinfo.Privilege = UserType.Author;
                    }
                }
                UserInfo.UpdateUserInfo(userinfo);
                return userinfo;
            }
        }
    }
}
