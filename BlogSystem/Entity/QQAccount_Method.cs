using InfraStructure.DataBase;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace BlogSystem.Entity
{
    public partial class QQAccount
    {
        public static string appID = string.Empty;
        public static string appKey = string.Empty;
        public static string callback = string.Empty;
        public static string authorizeURL = string.Empty;
        /// <summary>
        /// 注册方法为QQ
        /// </summary>
        public const string QQ = "QQ";
        /// <summary>
        /// 认证信息
        /// </summary>
        public struct QQOauthInfo
        {
            public string AccessToken { get; set; }
            public string ExpiresIn { get; set; }
            public string RefreshToken { get; set; }
        }
        /// <summary>
        /// 获取oauth信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static QQOauthInfo GetOauthInfo(string code)
        {
            string url = string.Format("https://graph.qq.com/oauth2.0/token?grant_type={0}&client_id={1}&client_secret={2}&code={3}&redirect_uri={4}", "authorization_code", appID, appKey, code, callback);
            string res = GetUrlResponse(url, Encoding.UTF8);
            InfraStructure.Log.InfoLog.Log("SYSTEM", "GetOauthInfo", "GetOauthInfo", res);
            QQOauthInfo qqOauthInfo = new QQOauthInfo();
            var resultparms = res.Split("&".ToCharArray());
            var accessToken = string.Empty;
            foreach (var parm in resultparms)
            {
                if (parm.StartsWith("access_token="))
                {
                    qqOauthInfo.AccessToken = parm.Split("=".ToCharArray())[1];
                }
                if (parm.StartsWith("expires_in="))
                {
                    qqOauthInfo.ExpiresIn = parm.Split("=".ToCharArray())[1];
                }
                if (parm.StartsWith("refresh_token="))
                {
                    qqOauthInfo.RefreshToken = parm.Split("=".ToCharArray())[1];
                }
            }
            return qqOauthInfo;
        }
        /// <summary>
        /// 通过GET方式获取页面的方法
        /// </summary>
        /// <param name="urlString">请求的URL</param>
        /// <param name="encoding">页面编码</param>
        /// <returns></returns>
        public static string GetUrlResponse(string urlString, Encoding encoding)
        {

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebRespones = null;
            Stream stream = null;
            string resResponse = string.Empty;
            try
            {
                httpWebRequest = WebRequest.Create(urlString) as HttpWebRequest;
            }
            catch (Exception ex)
            {
                InfraStructure.Log.ExceptionLog.Log("SYSTEM", "GetUrlResponse", "建立页面请求时发生错误", ex.ToString());
                return string.Empty;
            }
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; Maxthon 2.0)";

            try
            {
                httpWebRespones = (HttpWebResponse)httpWebRequest.GetResponse();
                stream = httpWebRespones.GetResponseStream();
            }
            catch (Exception ex)
            {
                InfraStructure.Log.ExceptionLog.Log("SYSTEM", "GetUrlResponse", "接受服务器返回页面时发生错误", ex.ToString());
                return string.Empty;
            }

            StreamReader streamReader = new StreamReader(stream, encoding);
            try
            {
                resResponse = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                InfraStructure.Log.ExceptionLog.Log("SYSTEM", "GetUrlResponse", "读取页面数据时发生错误", ex.ToString());
                return string.Empty;
            }
            finally
            {
                streamReader.Close();
                stream.Close();
            }
            return resResponse;
        }
        /// <summary>
        /// 获取QQ账号的OpenID
        /// </summary>
        /// <param name="qqOauthInfo"></param>
        /// <returns></returns>
        public static string GetOpenID(QQOauthInfo qqOauthInfo)
        {
            //PC网站
            string res = GetUrlResponse("https://graph.qq.com/oauth2.0/me?access_token=" + qqOauthInfo.AccessToken, Encoding.UTF8);
            InfraStructure.Log.InfoLog.Log("SYSTEM", "GetOpenID", "GetOpenID", res);
            //callback( { "client_id":"YOUR_APPID","openid":"YOUR_OPENID"} );
            string json = res.Substring(res.IndexOf("(") + 1);
            json = json.Substring(0, json.IndexOf(")"));
            dynamic obj = JsonConvert.DeserializeObject(json);
            return obj.openid;
        }
        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="qqOauthInfo"></param>
        /// <param name="openID"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(QQOauthInfo qqOauthInfo, string openID)
        {
            string urlGetInfo = string.Format(@"https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}", qqOauthInfo.AccessToken, appID, openID);
            string resUserInfo = GetUrlResponse(urlGetInfo, Encoding.UTF8);
            InfraStructure.Log.InfoLog.Log("SYSTEM", "GetUserInfo", "GetUserInfo", resUserInfo);
            dynamic obj = JsonConvert.DeserializeObject(resUserInfo);
            QQAccount qqlogin = new QQAccount()
            {
                nickname = obj.nickname,
                figureurl = obj.figureurl,
                gender = obj.gender,
                OpenID = openID,
                LastAccess = DateTime.Now
            };
            var qqAccount = GetQQAccountByOpenId(qqlogin.OpenID);
            if (qqAccount == null)
            {
                //没有该用户，添加
                var qqId = InsertQQAccount(qqlogin);
                UserInfo userinfo = new UserInfo()
                {
                    RegisterAccountID = qqId,
                    TopicList = new List<string>(),
                    TagList = new List<string>(),
                    RegisterMethod = QQ,
                    NickName = qqlogin.nickname,
                    Avatar_url = qqlogin.figureurl,
                    Privilege = UserType.Normal
                };
                var userinfoId = UserInfo.InsertUserInfo(userinfo);
                if (userinfoId == 1.ToString(SnFormat))
                {
                    //第一个用户是管理员(该代码仅仅被执行一次)
                    userinfo.Privilege = UserType.Admin;
                    UserInfo.UpdateUserInfo(userinfo);
                }
                //更新UserInfoID
                qqlogin.UserInfoID = userinfoId;
                UpdateQQAccount(qqlogin);

                //Welcome
                var articleurl = "<a href = '/Article/Index?ArticleId=00000006'>网站使用方法</a>";
                SiteMessage.CreateNotify(userinfoId, "欢迎加入CodeSnippet.info,请阅读[" + articleurl + "]");
                return userinfo;
            }
            else
            {
                //用新的信息替换旧的信息
                qqAccount.figureurl = qqlogin.figureurl;
                qqAccount.nickname = qqlogin.nickname;
                UpdateQQAccount(qqAccount);

                var userinfo = GetUserInfoByRegMethodInnerId(qqAccount.Sn);
                userinfo.Avatar_url = qqlogin.figureurl;
                userinfo.NickName = qqlogin.nickname;
                //防御性代码
                if (userinfo.TagList == null) userinfo.TagList = new List<string>();
                if (userinfo.TopicList == null) userinfo.TopicList = new List<string>();
                UserInfo.UpdateUserInfo(userinfo);
                return userinfo;
            }
        }

        /// <summary>
        /// 根据登陆名称获得帐号
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static QQAccount GetQQAccountByOpenId(string OpenId)
        {
            IMongoQuery AccountQuery = Query.EQ(nameof(OpenID), OpenId);
            return MongoDbRepository.GetFirstRec<QQAccount>(AccountQuery);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfoByRegMethodInnerId(string accountId)
        {
            IMongoQuery RegisterAccountIDQuery = Query.EQ(nameof(UserInfo.RegisterAccountID), accountId);
            IMongoQuery qqQuery = Query.EQ(nameof(UserInfo.RegisterMethod), QQ);
            return MongoDbRepository.GetFirstRec<UserInfo>(Query.And(RegisterAccountIDQuery, qqQuery));
        }
    }
}