using BlogSystem.BussinessLogic;
using BlogSystem.Entity;
using BlogSystem.TagSystem;
using InfraStructure.DataBase;
using InfraStructure.Misc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace CodeSnippet.Controllers
{
    public class HomeController : Controller
    {

        /// <summary>
        /// 首页文章加载
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public ActionResult Index(int PageNo = 1)
        {
            Pages p = new Pages(ArticleListManager.GetArticleCnt(ArticleListManager.FirstPageArticleQueryFileter), 20);
            p.CurrentPageNo = PageNo;
            var currentpageList = ArticleListManager.GetPublicListForArticleByPage(p, ArticleListManager.FirstPageArticleQueryFileter);
            ViewData.Model = currentpageList;
            ViewBag.TopArticle = ArticleListManager.GetTopArticle();
            ViewBag.Pages = p;
            ViewBag.AsideFirst = ASideColumnManager.MostArticleAuthor(10);
            ViewBag.AsideSecond = ASideColumnManager.MostHotTag(10);
            ViewBag.AsideThird = ASideColumnManager.HotArticle(10, 72);
            return View();
        }

        /// <summary>
        /// 统计
        /// </summary>
        /// <returns></returns>
        public ActionResult Statistics()
        {
            //标签
            ViewBag.TagChartName = "/Temp/" + DateTime.Now.ToString("yyyyMMdd") + "_Tag.jpeg";
            string tagFilename = Server.MapPath(ViewBag.TagChartName);
            if (!System.IO.File.Exists(tagFilename))
            {
                var tagDictionary = new Dictionary<string, int>();
                for (int i = 0; i < Math.Min(10, TagUtility.TagRankContain.RankList.Count); i++)
                {
                    tagDictionary.Add(TagUtility.TagRankContain.RankList[i].Key, TagUtility.TagRankContain.RankList[i].Count);
                }
                InfraStructure.Chart.ChartHelper.GetColumnChart(tagFilename, "标签数量TOP10", tagDictionary,InfraStructure.Chart.ChartType.Column,800,600);
            }

            //作者
            ViewBag.AuthorChartName = "/Temp/" + DateTime.Now.ToString("yyyyMMdd") + "_Author.jpeg";
            string authorFilename = Server.MapPath(ViewBag.AuthorChartName);
            var userRank = new RankContain(UserManager.UserGroupCntResult());
            ViewBag.UserRank = userRank;
            if (!System.IO.File.Exists(authorFilename))
            {
                var userDictionary = new Dictionary<string, int>();
                for (int i = 0; i < Math.Min(10, userRank.RankList.Count); i++)
                {
                    userDictionary.Add(UserInfo.GetUserNickNameByAccountId(userRank.RankList[i].Key), userRank.RankList[i].Count);
                }
                InfraStructure.Chart.ChartHelper.GetColumnChart(authorFilename, "作者TOP10", userDictionary, InfraStructure.Chart.ChartType.Column, 800, 600);
            }


            ViewBag.AsideFirst = ASideColumnManager.MostArticleAuthor(10);
            ViewBag.AsideSecond = ASideColumnManager.MostHotTag(10);
            ViewBag.AsideThird = ASideColumnManager.HotArticle(10, 72);
            return View();
        }

        /// <summary>
        /// 专题一览
        /// </summary>
        /// <returns></returns>
        public ActionResult TopicList()
        {
            ViewData.Model = ViewData.Model = Topic.getAllTopic();
            ViewBag.TopArticle = ArticleListManager.GetTopArticle();
            ViewBag.AsideFirst = ASideColumnManager.MostArticleAuthor(10);
            ViewBag.AsideSecond = ASideColumnManager.MostHotTag(10);
            ViewBag.AsideThird = ASideColumnManager.HotArticle(10, 72);
            return View();
        }

        /// <summary>
        /// 系列教程一览
        /// </summary>
        /// <returns></returns>
        public ActionResult SerialList()
        {
            ViewData.Model = Collection.getAllSerial();
            ViewBag.TopArticle = ArticleListManager.GetTopArticle();
            ViewBag.AsideFirst = ASideColumnManager.MostArticleAuthor(10);
            ViewBag.AsideSecond = ASideColumnManager.MostHotTag(10);
            ViewBag.AsideThird = ASideColumnManager.HotArticle(10, 72);
            return View();
        }

        /// <summary>
        /// 个人首页
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonIndex(int PageNo = 1)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            var u = UserInfo.GetUserInfoBySn(Session[ConstHelper.Session_USERID].ToString());
            ArticleListManager.ArticleQueryFilter filter = ArticleListManager.FirstPageArticleQueryFileter;
            //难度和分类
            filter.Levelist = u.Level.Count == 0 ? null : u.Level.ToArray();
            filter.Cataloglist = u.Catalog.Count == 0 ? null : u.Catalog.ToArray();
            filter.ContainTag = string.IsNullOrEmpty(u.ContainTag) ? null : u.ContainTag.Split(";".ToArray());
            filter.AntiTag = string.IsNullOrEmpty(u.AntiTag) ? null : u.AntiTag.Split(";".ToArray());
            //TODO:标签
            Pages p = new Pages(ArticleListManager.GetArticleCnt(filter), 20);
            p.CurrentPageNo = PageNo;
            var currentpageList = ArticleListManager.GetPublicListForArticleByPage(p, filter);
            ViewData.Model = currentpageList;
            ViewBag.TopArticle = ArticleListManager.GetTopArticle();
            ViewBag.Pages = p;
            ViewBag.TopArticle = ArticleListManager.GetTopArticle();
            ViewBag.AsideFirst = ASideColumnManager.MostArticleAuthor(10);
            ViewBag.AsideSecond = ASideColumnManager.MostHotTag(10);
            ViewBag.AsideThird = ASideColumnManager.HotArticle(10, 72);
            return View();
        }

        /// <summary>
        /// 检索
        /// </summary>
        /// <returns></returns>
        public ActionResult Search(int PageNo = 1)
        {
            //先从Post里面获得
            string KeyWord = Request.Form["txtKeyword"];

            if (string.IsNullOrEmpty(KeyWord))
            {
                //再从URL中取得
                KeyWord = Request.QueryString["Keyword"];
            }

            if (string.IsNullOrEmpty(KeyWord))
            {
                //两次取得都为空的话，退出
                return Redirect("/");
            }
            ViewBag.KeyWord = KeyWord;
            ViewBag.Title = KeyWord + "的检索结果";
            string SearchMethod = ConfigurationManager.AppSettings["SearchMethod"];
            var resultArticles = new List<Article>();
            switch (SearchMethod)
            {
                case ConstHelper.MongoTextSearch:
                    //设置Text索引用以检索(ElasticSearch)
                    resultArticles = Article.Search(KeyWord);
                    break;
                case ConstHelper.ElasticSearch:
                    resultArticles = SearchManager.Search(KeyWord, nameof(Article.Title));
                    break;
            }
            //公开发布的文章
            resultArticles = resultArticles.Where((x) => { return !x.IsPrivate && x.PublishStatus == ApproveStatus.Accept; }).ToList();
            var resultArticleItems = ArticleListManager.GetArticleItemBodyListByArticleList(resultArticles);
            //按照评分排序
            resultArticleItems.Sort((x, y) => { return y.Score.CompareTo(x.Score); });
            Pages p = new Pages(resultArticles.Count, 20);
            p.CurrentPageNo = PageNo;
            ViewBag.Pages = p;
            ViewData.Model = p.GetList(resultArticleItems);
            return View();
        }

        /// <summary>
        /// 标签列表
        /// </summary>
        /// <param name="TagName"></param>
        /// <returns></returns>
        public ActionResult TagList(string TagName, int PageNo = 1)
        {
            var articles = Article.GetArticleListByTag(TagName);
            Pages p = new Pages(articles.Count, 50);
            p.CurrentPageNo = PageNo;
            ViewBag.TagArticleCnt = articles.Count;
            ViewBag.Pages = p;
            var articlebodys = ArticleListManager.GetArticleItemBodyListByArticleList(p.GetList(articles));
            ViewBag.MostAuthorByTag = ASideColumnManager.GetMostAuthorByTag(TagName, 10);
            ViewData.Model = articlebodys;
            ViewBag.TagStockCnt = articlebodys.Sum((x) => { return x.StockCnt; });
            ViewBag.Title = TagName;
            return View();
        }

        /// <summary>
        /// 标签圈子用户
        /// </summary>
        /// <param name="TagName"></param>
        /// <returns></returns>
        public ActionResult CircleUserList(string TagName)
        {
            var circleuserlist = UserInfo.GetJoins(TagName);
            ViewData.Model = circleuserlist;
            ViewBag.Title = TagName + "圈子成员";
            return View("UserList");
        }

        /// <summary>
        /// Github第三方认证回调地址
        /// </summary>
        /// <returns></returns>
        public ActionResult GitHubOAuth()
        {
            //为了方便测试，这里不添加POST或者GET指示了
            //从Github返回的参数中获得code的值
            //这个code代表了授权用户的ID
            //由于是https，这里必须要转换为HttpWebRequest
            var userInfo = GithubAccount.GetUserInfo(Request.Params["code"]);
            if (userInfo != null)
            {
                //SN和UserInfoId在多种登陆方式的时候，不是同步的
                Session[ConstHelper.Session_USERID] = userInfo.Sn;
                Session[ConstHelper.Session_NAME] = userInfo.NickName;
                Session[ConstHelper.Session_AVATAR] = userInfo.Avatar_url;
                Session[ConstHelper.Session_PRIVILEGE] = userInfo.Privilege;
                if (userInfo.Privilege == UserType.Admin)
                {
                    return Redirect("/Admin/Index");
                }
                else
                {
                    return Redirect("/Home/PersonIndex");
                }
            }
            return Redirect("/");
        }

        /// <summary>
        /// Login QQ
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginQQ()
        {
            //随机数
            string state = new Random(100000).Next(99, 99999).ToString();
            Session["QQState"] = state;
            string authenticationUrl = string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&state={3}", QQAccount.authorizeURL, QQAccount.appID, QQAccount.callback, state);//互联地址
            return new RedirectResult(authenticationUrl);
        }

        /// <summary>
        /// QQ认证
        /// </summary>
        /// <returns></returns>
        public ActionResult QQOAuth()
        {
            if (string.IsNullOrEmpty(Request.Params["code"]) || string.IsNullOrEmpty(Request.Params["state"])) return Redirect("/");
            var code = Request.Params["code"];
            var state = Request.Params["state"];
            //随机数验证
            string requestState = Session["QQState"] == null ? "" : Session["QQState"].ToString();
            if (state != requestState) return Redirect("/");
            try
            {
                var qqOauthInfo = QQAccount.GetOauthInfo(code);
                //获取用的OpenID,这个ID是QQ给我们的用户的唯一ID，可以作为我们系统用户唯一性的判断存在我们自己的库中
                string openID = QQAccount.GetOpenID(qqOauthInfo);
                var userInfo = QQAccount.GetUserInfo(qqOauthInfo, openID);
                if (userInfo != null)
                {
                    //SN和UserInfoId在多种登陆方式的时候，不是同步的
                    Session[ConstHelper.Session_USERID] = userInfo.Sn;
                    Session[ConstHelper.Session_NAME] = userInfo.NickName;
                    Session[ConstHelper.Session_AVATAR] = userInfo.Avatar_url;
                    Session[ConstHelper.Session_PRIVILEGE] = userInfo.Privilege;
                    if (userInfo.Privilege == UserType.Admin)
                    {
                        return Redirect("/Admin/Index");
                    }
                    else
                    {
                        return Redirect("/Home/PersonIndex");
                    }
                }
                else
                {
                    return Redirect("/");
                }
            }
            catch (Exception ex)
            {
                InfraStructure.Log.ExceptionLog.Log("SYSTEM", "QQOAuth", "GET USER INFO", ex.ToString());
                return Redirect("/");
            }
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ActionResult Logoff()
        {
            Session.Clear();
            return Redirect("/");
        }

        #region 测试用
        /// <summary>
        /// 是否为Debug
        /// </summary>
        bool Debug = ConfigurationManager.AppSettings["DEBUGMODE"] == "true";

        /// <summary>
        /// 作为Admin登陆
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginAsUserId(int uid = 1)
        {
            if (!Debug) return Redirect("/");
            var u = UserInfo.GetUserInfoBySn(uid.ToString(EntityBase.SnFormat));
            if (u != null)
            {
                Session[ConstHelper.Session_USERID] = u.Sn;
                Session[ConstHelper.Session_NAME] = u.NickName;
                Session[ConstHelper.Session_AVATAR] = u.Avatar_url;
                Session[ConstHelper.Session_PRIVILEGE] = u.Privilege;
            }
            return Redirect("/");
        }
        #endregion

    }
}