using BlogSystem.BussinessLogic;
using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using BlogSystem.TagSystem;
using InfraStructure.DataBase;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace CodeSnippet.Controllers
{
    public class AuthorController : Controller
    {
        /// <summary>
        /// 作者页
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public ActionResult Index(string CollectionId, string accountid)
        {
            if (accountid == null) return Redirect("/");
            var user = UserInfo.GetUserInfoBySn(accountid);
            if (user == null) return Redirect("/");

            var collectionlist = Collection.GetCollectionListByOwnerId(accountid);
            if (CollectionId == "0")
            {
                if (collectionlist.Count > 0)
                {
                    CollectionId = collectionlist[0].Sn;
                }
            }

            ViewBag.CollectionId = CollectionId;
            ViewBag.CollectionList = collectionlist;
            if (collectionlist.Count == 0)
            {
                //如果没有文集，则什么都不显示
                ViewBag.CollectionArticles = new List<Article>();
            }
            else
            {
                //显示第一个或者指定文集
                var CurrentCollectionId = string.IsNullOrEmpty(CollectionId) ? collectionlist[0].Sn : CollectionId;
                ArticleListManager.ArticleQueryFilter aqf = new ArticleListManager.ArticleQueryFilter()
                {
                    collectionid = CurrentCollectionId,
                    isPrivate = false,
                    statuslist = new ApproveStatus[] { ApproveStatus.Accept, ApproveStatus.NotNeed }
                };
                if (Session[ConstHelper.Session_USERID] != null && Session[ConstHelper.Session_USERID].ToString().Equals(accountid))
                {
                    //自己看自己
                    aqf.isPrivate = null;
                    aqf.statuslist = null;
                }
                var ArticleList = ArticleListManager.GetArticleItemBodyListByArticleList(ArticleListManager.GetArticles(aqf));
                ViewBag.CollectionArticles = ArticleList;
            }
            var userbody = UserManager.GetUserBody(accountid);
            ViewData.Model = userbody;
            ViewBag.Title = user.NickName;
            ArticleListManager.ArticleQueryFilter publicAQF = new ArticleListManager.ArticleQueryFilter()
            {
                ownid = accountid,
                isPrivate = false,
                statuslist = new ApproveStatus[] { ApproveStatus.Accept, ApproveStatus.NotNeed }
            };
            var articles = ArticleListManager.GetArticles(publicAQF);
            var rankContain = TagUtility.Statistics(articles);
            ViewBag.PersonalTagAside = ASideColumnManager.CreateTagRankAside(rankContain, 10);
            ViewBag.PersonalHobbyAside = ASideColumnManager.TagHobby(accountid, 10);
            ViewBag.RankTagDic = rankContain.RankKeyDic;
            return View();
        }

        /// <summary>
        /// 关注者列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult FocusList(string UserId = "00000001")
        {
            var userbody = UserManager.GetUserBody(UserId);
            ViewData.Model = userbody.FocusList;
            ViewBag.Title = userbody.UserInfo.NickName + " 关注的人";
            return View("UserList");
        }
        /// <summary>
        /// 跟随着列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult FollowList(string UserId = "00000001")
        {
            var userbody = UserManager.GetUserBody(UserId);
            ViewData.Model = userbody.FollowList;
            ViewBag.Title = "关注" + userbody.UserInfo.NickName + "的人";
            return View("UserList");
        }

        /// <summary>
        /// 个人设定 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Setting()
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            ViewData.Model = UserInfo.GetUserInfoBySn(Session[ConstHelper.Session_USERID].ToString());
            return View();
        }
        [HttpPost]
        public ActionResult Setting(FormCollection colllection)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            var u = UserInfo.GetUserInfoBySn(Session[ConstHelper.Session_USERID].ToString());
            if (colllection.AllKeys.Contains("Catalog"))
            {
                u.Catalog = colllection["Catalog"].Split(",".ToArray()).ToList();
            }
            if (colllection.AllKeys.Contains("Level"))
            {
                u.Level.Clear();
                foreach (var level in colllection["Level"].Split(",".ToArray()))
                {
                    u.Level.Add((ArticleLevel)Enum.Parse(typeof(ArticleLevel), level));
                }
            }
            u.AntiTag = colllection["AntiTag"];
            u.ContainTag = colllection["ContainTag"];
            UserInfo.UpdateUserInfo(u);
            ViewData.Model = u;
            return Redirect("/Home/PersonIndex");
        }
        /// <summary>
        /// 站内消息
        /// </summary>
        /// <returns></returns>
        public ActionResult SiteMessageList(int PageNo = 1)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            var msglist = SiteMessage.GetMessage(Session[ConstHelper.Session_USERID].ToString());
            Pages p = new Pages(msglist.Count, 50);
            p.CurrentPageNo = PageNo;
            ViewBag.Pages = p;
            ViewData.Model = p.GetList(msglist);
            ViewBag.Title = "消息";
            return View();
        }

        /// <summary>
        /// 收藏页
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public ActionResult StockPage(int PageNo = 1)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            var accountid = Session[ConstHelper.Session_USERID].ToString();
            var StockList = Stock.GetStockByAccount(accountid);
            Pages p = new Pages(StockList.Count, 50);
            p.CurrentPageNo = PageNo;
            ViewBag.Pages = p;
            ViewData.Model = p.GetList(StockList);
            ViewBag.Title = "收藏页";
            var rankContain = TagUtility.Statistics((StockList.Select((x) => { return x.ArticleInfo; }).ToList()));
            ViewBag.PersonalStockAside = ASideColumnManager.CreateTagRankAside(rankContain, 10);
            return View();
        }

        /// <summary>
        /// 从收藏中移除文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public ActionResult RemoveFromStock(string articleId)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            Stock.RemoveArticle(Session[ConstHelper.Session_USERID].ToString(), articleId);
            return Redirect("/Author/StockPage?PageNo=1");
        }

        #region 专题
        /// <summary>
        /// 新建专题
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateTopic()
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            Topic topic = null;
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            if (Topic.IsExistTopic(accountid))
            {
                topic = Topic.GetTopicByAccountId(accountid);
                ViewBag.Title = "修改专题";
            }
            else
            {
                topic = new Topic();
                ViewBag.Title = "新建专题";
            }
            ViewData.Model = topic;
            return View();
        }
        [HttpPost]
        public ActionResult CreateTopic(FormCollection collection)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            Topic topic = null;
            string accountid = Session[ConstHelper.Session_USERID].ToString();

            if (Topic.IsExistTopic(accountid))
            {
                ViewBag.Title = "修改专题";
                topic = Topic.GetTopicByAccountId(accountid);
            }
            else
            {
                ViewBag.Title = "新建专题";
                topic = new Topic();
                //无法修改名字信息
                topic.Title = collection[nameof(Topic.Title)].Trim();
                if (string.IsNullOrEmpty(topic.Title))
                {
                    ViewData.ModelState.AddModelError(nameof(Topic.Title), "请输入名称");
                    return View();
                }
            }
            topic.Description = collection[nameof(Topic.Description)].Trim();
            topic.CustomTagList = collection[nameof(Topic.CustomTagList)].Trim();
            topic.TagName = TagUtility.GetTagNameList(topic.Title, topic.CustomTagList);
            topic.IsNeedApproval = collection[nameof(Topic.IsNeedApproval)] != null;
            topic.IsPostable = collection[nameof(Topic.IsPostable)] != null;
            ViewData.Model = topic;

            if (string.IsNullOrEmpty(topic.Description))
            {
                ViewData.ModelState.AddModelError(nameof(Topic.Description), "请输入简介");
                return View();
            }
            if (topic.Description.Length < 15)
            {
                ViewData.ModelState.AddModelError(nameof(Topic.Description), "简介字数过少");
                return View();
            }
            if (string.IsNullOrEmpty(topic.CustomTagList))
            {
                ViewData.ModelState.AddModelError(nameof(Topic.CustomTagList), "请输入标签");
                return View();
            }

            if (topic.IsNeedApproval && !topic.IsPostable)
            {
                ViewData.ModelState.AddModelError(nameof(Topic.IsNeedApproval), "不允许投稿，则不能设定需要审核");
                return View();
            }

            if (Topic.IsExistTopic(accountid))
            {
                Topic.UpdateTopic(topic);
            }
            else
            {
                Topic.InsertTopic(topic, accountid);
            }
            return Redirect("/Author/ManagerTopic");
        }

        /// <summary>
        /// 接受文章被收录到某主题（操作人是文章的拥有者）
        /// </summary>
        /// <param name="TopicOwnerId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult AcceptTopic(string TopicOwnerId, string ArticleId, string MessageId)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            var article = Article.GetArticleBySn(ArticleId);
            if (article == null)
            {
                SiteMessage.CloseMessage(MessageId, accountid, "没有发现文章");
            }
            else
            {
                if (accountid != article.OwnerId) return Redirect("/");
                TopicArticle.ChangeTopicStatus(TopicOwnerId, ArticleId, true);
                SiteMessage.CreateNotify(TopicOwnerId, "您的收录请求被接受，[" + article.Title + "]被收录到您的文集中", accountid);
                SiteMessage.CloseMessage(MessageId, accountid, "接受");
            }
            return Redirect("/Author/SiteMessageList");
        }
        /// <summary>
        /// 拒绝文章被收录到某主题（操作人是文章的拥有者）
        /// </summary>
        /// <param name="TopicOwnerId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult RefuseTopic(string TopicOwnerId, string ArticleId, string MessageId)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            var article = Article.GetArticleBySn(ArticleId);
            if (article == null)
            {
                SiteMessage.CloseMessage(MessageId, accountid, "没有发现文章");
            }
            else
            {
                if (accountid != article.OwnerId) return Redirect("/");
                TopicArticle.ChangeTopicStatus(TopicOwnerId, ArticleId, false);
                SiteMessage.CreateNotify(TopicOwnerId, "您的收录请求被拒绝，[" + article.Title + "]没有被收录到您的文集中", accountid);
                SiteMessage.CloseMessage(MessageId, accountid, "拒绝");
            }
            return Redirect("/Author/SiteMessageList");
        }
        /// <summary>
        /// 从专题中移除文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public ActionResult RemoveFromTopic(string articleId)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            Topic topic = null;
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            if (Topic.IsExistTopic(accountid))
            {
                topic = Topic.GetTopicByAccountId(accountid);
            }
            else
            {
                return Redirect("/");
            }
            TopicArticle.Remove(topic.Sn, articleId);
            return Redirect("/Author/ManagerTopic?PageNo=1");
        }

        /// <summary>
        /// 管理专题
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public ActionResult ManagerTopic(int PageNo = 1)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            var topicArticles = ArticleListManager.GetTopicArticleList(accountid, false);
            Pages p = new Pages(topicArticles.Count, 50);
            p.CurrentPageNo = PageNo;
            ViewBag.Pages = p;
            ViewBag.Title = "管理专题";
            ViewData.Model = p.GetList(topicArticles);
            return View();
        }

        /// <summary>
        /// 作者专题页
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public ActionResult TopicPage(string accountid, int PageNo = 1)
        {
            if (accountid == null) return Redirect("/");
            var user = UserInfo.GetUserInfoBySn(accountid);
            if (user == null) return Redirect("/");
            var topic = Topic.GetTopicByAccountId(accountid);
            if (topic == null) return Redirect("/");

            var topicArticles = ArticleListManager.GetTopicArticleList(accountid, true);
            Pages p = new Pages(topicArticles.Count, 50);
            p.CurrentPageNo = PageNo;
            ViewBag.Pages = p;
            ViewData.Model = p.GetList(topicArticles);
            ViewBag.Title = topic.Title;
            return View();
        }


        /// <summary>
        /// 欲投稿文章列表
        /// </summary>
        /// <param name="CollectionId"></param>
        /// <returns></returns>
        public ActionResult PostToTopic(string TopicId, string CollectionId = "")
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            string OwnerId = Session[ConstHelper.Session_USERID].ToString();
            var collectionlist = Collection.GetCollectionListByOwnerId(OwnerId);
            ViewBag.CollectionId = CollectionId;
            ViewBag.CollectionList = collectionlist;
            ViewBag.TopicId = TopicId;
            if (collectionlist.Count == 0)
            {
                //如果没有文集，则什么都不显示
                ViewData.Model = new List<ArticleItemBody>();
            }
            else
            {
                //显示第一个或者指定文集
                var CurrentCollectionId = string.IsNullOrEmpty(CollectionId) ? collectionlist[0].Sn : CollectionId;
                //自己的文章，则并非只是发布到首页的
                var ArticleList = Article.GetArticleByColIdAndPublish(CurrentCollectionId, false);
                ViewData.Model = ArticleListManager.GetArticleItemBodyListByArticleList(ArticleList);
                ViewBag.CollectionId = CurrentCollectionId;
            }
            ViewBag.Title = "列表";
            return View();
        }
        /// <summary>
        /// 投稿文章到专题
        /// </summary>
        /// <param name="TopicId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult PostArticle(string TopicId, string ArticleId)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            var article = Article.GetArticleBySn(ArticleId);
            if (article.OwnerId != accountid) return Redirect("/");
            var topic = Topic.GetTopicBySn(TopicId);
            if (topic == null || (!topic.IsPostable)) return Redirect("/");
            if (topic.IsNeedApproval)
            {
                TopicArticle.InsertTopicArticle(new TopicArticle()
                {
                    ArticleID = ArticleId,
                    TopicID = TopicId,
                    PublishStatus = ApproveStatus.Pending
                });
                var parm = "TopicId=" + topic.Sn + "&ArticleId=" + ArticleId;
                var articleurl = "<a href = '/Article/Index?ArticleId=" + article.Sn + "'>" + article.Title + "</a>";
                var topicurl = "<a href = '/Author/TopicPage?accountid=" + topic.OwnerId + "'>" + topic.Title + "</a>";
                SiteMessage.CreateYesNo(topic.OwnerId, articleurl + "请求投稿到专题" + topicurl, "/Author/AcceptActicle?" + parm, "/Author/RefuseActicle?" + parm, accountid);
            }
            else
            {
                TopicArticle.InsertTopicArticle(new TopicArticle()
                {
                    ArticleID = ArticleId,
                    TopicID = TopicId,
                    PublishStatus = ApproveStatus.Accept
                });
            }
            return Redirect("/Author/PostToTopic?TopicId=" + TopicId);
        }
        /// <summary>
        /// 接受文章被收录到某主题（操作人是专题的拥有者）
        /// </summary>
        /// <param name="TopicOwnerId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult AcceptActicle(string TopicId, string ArticleId, string MessageId)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            var article = Article.GetArticleBySn(ArticleId);
            var topic = Topic.GetTopicBySn(TopicId);
            if (article == null || topic == null)
            {
                SiteMessage.CloseMessage(MessageId, accountid, "没有发现文章或者主题");
            }
            else
            {
                if (accountid != topic.OwnerId) return Redirect("/");
                TopicArticle.ChangeTopicStatus(topic.OwnerId, ArticleId, true);
                SiteMessage.CreateNotify(article.OwnerId, "您的收录请求被接受，[" + article.Title + "]被收录到文集中", accountid);
                SiteMessage.CloseMessage(MessageId, accountid, "接受");
            }
            return Redirect("/Author/SiteMessageList");
        }
        /// <summary>
        /// 拒绝文章被收录到某主题（操作人是专题的拥有者）
        /// </summary>
        /// <param name="TopicOwnerId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult RefuseActicle(string TopicId, string ArticleId, string MessageId)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
            string accountid = Session[ConstHelper.Session_USERID].ToString();
            var article = Article.GetArticleBySn(ArticleId);
            var topic = Topic.GetTopicBySn(TopicId);
            if (article == null || topic == null)
            {
                SiteMessage.CloseMessage(MessageId, accountid, "没有发现文章或者主题");
            }
            else
            {
                if (accountid != topic.OwnerId) return Redirect("/");
                TopicArticle.ChangeTopicStatus(topic.OwnerId, ArticleId, false);
                SiteMessage.CreateNotify(article.OwnerId, "您的收录请求被拒绝，[" + article.Title + "]没有被收录到文集中", accountid);
                SiteMessage.CloseMessage(MessageId, accountid, "拒绝");
            }
            return Redirect("/Author/SiteMessageList");
        }
        #endregion
    }
}