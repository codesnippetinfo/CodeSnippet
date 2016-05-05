using BlogSystem.BussinessLogic;
using BlogSystem.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CodeSnippet.Controllers
{
    public class ArticleController : Controller
    {
        /// <summary>
        /// 进入单篇文章
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string ArticleId)
        {
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            var article = Article.GetArticleBySn(ArticleId);
            if (article == null) return Redirect("/"); 
            if (!article.IsPrivate && (article.PublishStatus != ApproveStatus.Accept && article.PublishStatus != ApproveStatus.NotNeed))
            {
                //公开文章，但是没有发布
                return Redirect("/");
            }
            if (article.IsPrivate)
            {
                if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
                if (!Session[ConstHelper.Session_USERID].ToString().Equals(article.OwnerId)) return Redirect("/");
            }
            //获得IP地址
            var visit = new Visitor()
            {
                UserAgent = HttpContext.Request.UserAgent,
                UserHostAddress = HttpContext.Request.UserHostAddress,
                UserHostName = HttpContext.Request.UserHostName,
                UserLanguages = HttpContext.Request.UserLanguages.ToList(),
                ArticleID = ArticleId,
                UserInfoId = Session[ConstHelper.Session_USERID] != null ? Session[ConstHelper.Session_USERID].ToString() : null,
            };
            if (visit.IsSpider)
            {
                Visitor.InsertSpider(visit);
            }
            else
            {
                Visitor.InsertVisitor(visit);
            }
            var articlebody = ArticleListManager.GetArticleBodyById(ArticleId);
            ViewData.Model = articlebody;
            ViewBag.CommentList = ArticleListManager.GetCommentBodyList(ArticleId);
            ViewBag.Title = articlebody.ArticleInfo.Title;
            ViewBag.AuthorId = articlebody.AuthorInfo.Sn;
            return View();
        }
        /// <summary>
        /// 收藏者
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult StockUserList(string ArticleId)
        {
            var articlebody = ArticleListManager.GetArticleBodyById(ArticleId);
            ViewData.Model = articlebody.StockAccountList;
            ViewBag.Title = articlebody.ArticleInfo.Title + " 收藏者";
            return View("UserList");
        }


        /// <summary>
        /// PDF用
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult SimplePDF(string ArticleId)
        {
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            var articlebody = ArticleListManager.GetArticleBodyById(ArticleId);
            if (articlebody == null) return Redirect("/");
            if (!articlebody.ArticleInfo.IsPrivate && articlebody.ArticleInfo.PublishStatus != ApproveStatus.Accept)
            {
                //公开文章，但是没有发布
                return Redirect("/");
            }
            if (articlebody.ArticleInfo.IsPrivate)
            {
                if (Session[ConstHelper.Session_USERID] == null) return Redirect("/");
                if (!Session[ConstHelper.Session_USERID].ToString().Equals(articlebody.ArticleInfo.OwnerId)) return Redirect("/");
            }
            ViewBag.Html = ArticleContent.GetHtmlString(ArticleId);
            ViewData.Model = articlebody;
            return View();
        }
        /// <summary>
        /// 修改评论 或者 回复评论
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult ModifyOrReplyComment(string ArticleId)
        {
            if (string.IsNullOrEmpty(ArticleId)) return Redirect("/");
            var article = Article.GetArticleBySn(ArticleId);
            if (article == null) return Redirect("/");
            if (article.PublishStatus != ApproveStatus.Accept) return Redirect("/");

            if (Session[ConstHelper.Session_USERID] == null)
            {
                return Redirect("/Article/Index?ArticleId=" + ArticleId);
            }
            if (string.IsNullOrEmpty(Request.Unvalidated["ModifyReplyContent"]))
            {
                return Redirect("/Article/Index?ArticleId=" + ArticleId);
            }
            var newComment = new Comment();
            if (!string.IsNullOrEmpty(Request["ReplyCommentId"].ToString()))
            {
                newComment.OwnerId = Session[ConstHelper.Session_USERID].ToString();
                newComment.ContentMD = Request.Unvalidated["ModifyReplyContent"];
                newComment.ContentHTML = Request.Unvalidated["ModifyReplyHTML"];
                newComment.ArticleID = ArticleId;
                newComment.ReplyCommentID = Request["ReplyCommentId"].ToString();
                Comment.InsertComment(newComment, newComment.OwnerId);
            }
            else
            {
                newComment = Comment.GetCommentBySn(Request["ModifyCommentId"].ToString());
                newComment.ContentMD = Request.Unvalidated["ModifyReplyContent"];
                newComment.ContentHTML = Request.Unvalidated["ModifyReplyHTML"];
                Comment.UpdateComment(newComment);
            }
            return Redirect("/Article/Index?ArticleId=" + ArticleId);
        }

        /// <summary>
        /// 添加针对文章的评论
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public ActionResult CreateComment(string ArticleId)
        {
            var article = Article.GetArticleBySn(ArticleId);
            if (article == null) return Redirect("/");
            if (article.PublishStatus != ApproveStatus.Accept) return Redirect("/");
            if (Session[ConstHelper.Session_USERID] == null)
            {
                return Redirect("/Article/Index?ArticleId=" + ArticleId);
            }
            if (string.IsNullOrEmpty(Request.Unvalidated["CommentContent"]))
            {
                return Redirect("/Article/Index?ArticleId=" + ArticleId);
            }
            var newComment = new Comment();
            newComment.OwnerId = Session[ConstHelper.Session_USERID].ToString();
            newComment.ContentMD = Request.Unvalidated["CommentContent"];
            newComment.ContentHTML = Request.Unvalidated["CommentHTML"];
            newComment.ArticleID = ArticleId;
            Comment.InsertComment(newComment, newComment.OwnerId);
            return Redirect("/Article/Index?ArticleId=" + ArticleId);
        }

        /// <summary>
        /// 删除指定序列号的评论
        /// </summary>
        /// <param name="CommentId"></param>
        /// <returns></returns>
        public ActionResult DeleteComment(string CommentId)
        {
            if (string.IsNullOrEmpty(CommentId)) return Redirect("/");
            var comment = Comment.GetCommentBySn(CommentId);
            //如果文章具有子评论，则不能删除
            if (!string.IsNullOrEmpty(comment.ReplyCommentID) || !Comment.HasSubComment(CommentId)) {
                //回复评论ID不为空 或者 没有子评论
                Comment.DropComment(comment);
            }
            return Redirect("/Article/Index?ArticleId=" + comment.ArticleID);
        }

        /// <summary>
        /// 文集内文章列表
        /// </summary>
        /// <param name="CollectionId"></param>
        /// <returns></returns>
        public ActionResult CollectionList(string CollectionId)
        {
            if (string.IsNullOrEmpty(CollectionId)) return Redirect("/");
            //文章展示页面，链接到该文章所属文集，所以是对于大众的，只要是 发布的 ，不论是否在首页
            var ArticleList = Article.GetArticleByColIdAndPublish(CollectionId, true);
            ViewData.Model =  ArticleListManager.GetArticleItemBodyListByArticleList(ArticleList);
            var Col = Collection.GetCollectionBySn(CollectionId);
            ViewBag.Title = Col.Title;
            return View();
        }
    }
}