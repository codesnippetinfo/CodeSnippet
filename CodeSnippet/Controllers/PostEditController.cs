using BlogSystem.BussinessLogic;
using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using BlogSystem.TagSystem;
using InfraStructure.DataBase;
using InfraStructure.Table;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CodeSnippet.Controllers
{
    public class PostEditController : Controller
    {
        [HttpGet]
        public ActionResult ArticleEdit(string ArticleId = "")
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            string ownerId = Session[ConstHelper.Session_USERID].ToString();
            if (string.IsNullOrEmpty(ArticleId))
            {
                ViewData.Model = new Article()
                {
                    IsFirstPage = true,
                    IsOriginal = true,
                    IsTopicable = true,
                };
                ViewBag.Title = "新建";
            }
            else
            {
                ViewData.Model = Article.GetArticleBySn(ArticleId);
                ViewBag.Title = "修改";
            }
            return View();
        }

        [HttpPost]
        public ActionResult ArticleEdit(FormCollection collection)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            string ownerId = Session[ConstHelper.Session_USERID].ToString();
            string ArticleId = Request.QueryString["ArticleId"];
            //Article
            Article NewArticle = new Article()
            {
                OwnerId = ownerId,
                CollectionID = Request.QueryString[nameof(Article.CollectionID)],
                Title = collection[nameof(Article.Title)],
                CustomTagList = collection[nameof(Article.CustomTagList)],
                IsFirstPage = collection[nameof(Article.IsFirstPage)] != null,
                IsCloseComment = collection[nameof(Article.IsCloseComment)] != null,
                IsOriginal = collection[nameof(Article.IsOriginal)] != null,
                IsPrivate = collection[nameof(Article.IsPrivate)] != null,
                AdvLink = collection[nameof(Article.AdvLink)],
                AdvText = collection[nameof(Article.AdvText)],
                AdvImageUrl = collection[nameof(Article.AdvImageUrl)],
                Catalog = collection[nameof(Article.Catalog)],
                IsNeedTopicApproval = collection[nameof(Article.IsNeedTopicApproval)] != null,
                IsPutToMyTopic = collection[nameof(Article.IsPutToMyTopic)] != null,
                IsTopicable = collection[nameof(Article.IsTopicable)] != null,
            };
            NewArticle.Title = NewArticle.Title.Trim();
            NewArticle.CustomTagList = NewArticle.CustomTagList.Trim();
            NewArticle.TagName = TagUtility.getTagsFromTitle(NewArticle.Title);
            NewArticle.Level = (ArticleLevel)System.Enum.Parse(typeof(ArticleLevel), collection["Level"]);
            ViewData.Model = NewArticle;
            if (NewArticle.Title.Length < 5)
            {
                ViewData.ModelState.AddModelError("Title", "标题字数过少");
                return View();
            }
            if (NewArticle.IsFirstPage)
            {
                if (NewArticle.IsCloseComment)
                {
                    ViewData.ModelState.AddModelError("IsCloseComment", "关闭评论的文章无法发布到首页");
                    return View();
                }
                if (!NewArticle.IsOriginal)
                {
                    ViewData.ModelState.AddModelError("IsOriginal", "转载的文章无法发布到首页");
                    return View();
                }
                if (NewArticle.IsPrivate)
                {
                    ViewData.ModelState.AddModelError("IsPrivate", "私有的文章无法发布到首页");
                    return View();
                }
            }
            if (NewArticle.IsPrivate)
            {
                if (NewArticle.IsTopicable || NewArticle.IsPutToMyTopic)
                {
                    ViewData.ModelState.AddModelError("IsTopicable", "私有文章无法被专题收录");
                    return View();
                }
            }
            if (NewArticle.IsNeedTopicApproval && !NewArticle.IsTopicable)
            {
                ViewData.ModelState.AddModelError("IsNeedTopicApproval", "不能被收录到专题的文章无法设定收录审核");
                return View();
            }
            if (string.IsNullOrEmpty(ArticleId))
            {
                ViewBag.Title = "新建";
                if (NewArticle.IsPrivate)
                {
                    NewArticle.PublishStatus = ApproveStatus.NotNeed;
                }
                else
                {
                    NewArticle.PublishStatus = ApproveStatus.None;
                }
                NewArticle.TagName = TagUtility.GetTagNameList(NewArticle.Title, NewArticle.CustomTagList);
                var SerialNumber = Article.InsertArticle(NewArticle, ownerId);
                //加入草稿，保证可以编辑
                ArticleContent.InsertArticleContent(
                    new ArticleContent()
                    {
                        Revision = RevisionType.Draft,
                        ArticleID = SerialNumber,
                        ContentType = ArticleContent.MarkDown
                    }, ownerId);
                if (NewArticle.IsPrivate)
                {
                    //私人的，没有发布的概念，当前版本也建立好
                    ArticleContent.InsertArticleContent(
                        new ArticleContent()
                        {
                            Revision = RevisionType.Current,
                            ArticleID = SerialNumber,
                            ContentType = ArticleContent.MarkDown
                        }, ownerId);
                    //私人的，没有发布的概念，当前版本也建立好
                    ArticleContent.InsertArticleContent(
                        new ArticleContent()
                        {
                            Revision = RevisionType.Current,
                            ArticleID = SerialNumber,
                            ContentType = ArticleContent.HTML
                        }, ownerId);
                }
                //转移到EditPost
                return Redirect("MarkDownEditor?ArticleID=" + SerialNumber);
            }
            else
            {
                var OldArticle = Article.GetArticleBySn(ArticleId);
                OldArticle.Title = NewArticle.Title;
                OldArticle.CustomTagList = NewArticle.CustomTagList;
                OldArticle.TagName = TagUtility.GetTagNameList(OldArticle.Title, OldArticle.CustomTagList);
                //考虑到如果是非首页文章，没有发布审核环节
                //如果已经发布的文章，不允许更改IsFirstPage
                if (OldArticle.IsPrivate && !NewArticle.IsPrivate)
                {
                    //旧的是私有，新的不是私有
                    //必须将状态改为初始化，不然会饶过审核系统
                    OldArticle.PublishStatus = ApproveStatus.None;
                }
                if (OldArticle.PublishStatus != ApproveStatus.Accept)
                {
                    //已经发布的公开的文章不允许
                    //1. 修改为首页或者私有
                    OldArticle.IsFirstPage = NewArticle.IsFirstPage;
                    OldArticle.IsPrivate = NewArticle.IsPrivate;
                    //2. 修改专题
                    OldArticle.IsTopicable = NewArticle.IsTopicable;
                    OldArticle.IsPutToMyTopic = NewArticle.IsPutToMyTopic;
                    OldArticle.IsNeedTopicApproval = NewArticle.IsNeedTopicApproval;
                    //3 .推广
                    OldArticle.AdvLink = NewArticle.AdvLink;
                    OldArticle.AdvText = NewArticle.AdvText;
                    OldArticle.AdvImageUrl = NewArticle.AdvImageUrl;
                }
                OldArticle.IsOriginal = NewArticle.IsOriginal;
                OldArticle.IsCloseComment = NewArticle.IsCloseComment;
                OldArticle.Level = NewArticle.Level;
                OldArticle.Catalog = NewArticle.Catalog;

                Article.UpdateArticle(OldArticle);
                ArticleListManager.RemoveArticleItemBody(ArticleId);
                ViewBag.Title = "修改";
                return Redirect("/PostEdit/PostList?CollectionId=" + NewArticle.CollectionID);
            }
        }

        /// <summary>
        /// 文集编辑器
        /// </summary>
        /// <param name="CollectionId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CollectionEdit(string CollectionId = "")
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            if (string.IsNullOrEmpty(CollectionId))
            {
                ViewBag.Title = "新建文集";
                ViewData.Model = new Collection();
            }
            else
            {
                ViewBag.Title = "修改文集";
                ViewData.Model = Collection.GetCollectionBySn(CollectionId);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CollectionEdit(FormCollection collection)
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            string ownerId = Session[ConstHelper.Session_USERID].ToString();
            string CollectionId = Request.QueryString["CollectionId"];
            Collection EditCollection = null;
            EditCollection = new Collection()
            {
                OwnerId = ownerId,
                Title = collection[nameof(Collection.Title)].Trim(),
                CustomTagList = collection[nameof(Collection.CustomTagList)].Trim(),
                Description = collection[nameof(Collection.Description)].Trim(),
                IsSerie = collection[nameof(Collection.IsSerie)] != null,
            };
            ViewData.Model = EditCollection;
            if (string.IsNullOrEmpty(CollectionId))
            {
                ViewBag.Title = "新建文集";
            }
            else
            {
                ViewBag.Title = "修改文集";
            }
            if (string.IsNullOrEmpty(EditCollection.Description))
            {
                ViewData.ModelState.AddModelError(nameof(Collection.Description), "请填写简介");
                return View();
            }
            if (EditCollection.Description.Length < 15)
            {
                ViewData.ModelState.AddModelError(nameof(Collection.Description), "简介字数过少");
                return View();
            }
            EditCollection.TagName = TagUtility.GetTagNameList(EditCollection.Title, EditCollection.CustomTagList);
            if (string.IsNullOrEmpty(CollectionId))
            {
                //新建文集时，不允许设置同名文集
                if (Collection.ExistCollectionByTitle(EditCollection.Title, EditCollection.OwnerId))
                {
                    ViewData.ModelState.AddModelError(nameof(Collection.Title), "文集名称已经存在");
                    return View();
                }
                ViewBag.Title = "新建文集";
                CollectionId = OwnerTableOperator.InsertRec(EditCollection, ownerId);
            }
            else
            {
                var OldCollection = Collection.GetCollectionBySn(CollectionId);
                OldCollection.Title = EditCollection.Title;
                OldCollection.Description = EditCollection.Description;
                OldCollection.IsSerie = EditCollection.IsSerie;
                OldCollection.CustomTagList = EditCollection.CustomTagList;

                if (OldCollection.CreateDateTime == System.DateTime.MinValue)
                {
                    //修复以前BUG的问题
                    OldCollection.CreateDateTime = System.DateTime.Now;
                    OldCollection.CreateUser = MongoDbRepository.UserSystem;
                }
                ViewBag.Title = "修改文集";
                MongoDbRepository.UpdateRec(OldCollection);
            }
            return Redirect("/PostEdit/PostList?CollectionId=" + CollectionId);
        }
        /// <summary>
        /// 编辑投稿
        /// </summary>
        /// <param name="ArticleID"></param>
        /// <returns></returns>
        public ActionResult MarkDownEditor(string ArticleID)
        {
            Article article = Article.GetArticleBySn(ArticleID);
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            if (Session[ConstHelper.Session_USERID].ToString() != article.OwnerId) return Redirect("/Home/Index");
            ViewBag.IsPrivate = article.IsPrivate;
            ViewBag.FreeVolumn = UploadFile.GetFreeVolumnByAccountId(article.OwnerId);
            ViewData.Model = ArticleContent.GetMarkDown(ArticleID, RevisionType.Draft);
            ViewBag.Title = "编辑-" + article.Title;
            return View();
        }

        /// <summary>
        /// 删除投稿
        /// </summary>
        /// <param name="ArticleID"></param>
        /// <returns></returns>
        public ActionResult DeletePost(string ArticleID)
        {
            Article article = Article.GetArticleBySn(ArticleID);
            if (article == null) return Redirect("/Home/Index");
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            if (Session[ConstHelper.Session_USERID].ToString() != article.OwnerId) return Redirect("/Home/Index");
            //已经发布的首页文章不允许删除
            if (article.PublishStatus == ApproveStatus.Accept && article.IsFirstPage) return Redirect("/Home/Index");
            Article.DropArticle(article);
            //收录到专题中的文章也要处理掉
            TopicArticle.RemoveArticle(article.Sn);
            return Redirect("/PostEdit/PostList");
        }
        /// <summary>
        /// 编辑列表
        /// </summary>
        /// <param name="CollectionId"></param>
        /// <returns></returns>
        public ActionResult PostList(string CollectionId = "")
        {
            if (Session[ConstHelper.Session_USERID] == null) return Redirect("/Home/Index");
            string OwnerId = Session[ConstHelper.Session_USERID].ToString();
            var collectionlist = Collection.GetCollectionListByOwnerId(OwnerId);
            ViewBag.CollectionId = CollectionId;
            ViewBag.CollectionList = collectionlist;
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
    }
}