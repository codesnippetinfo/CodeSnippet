using BlogSystem.BussinessLogic;
using BlogSystem.Entity;
using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for PublishPost
    /// </summary>
    public class PublishPost : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";
            string json = string.Empty;
            if (context.Session[ConstHelper.Session_USERID] == null)
            {
                var SessionTimeout = new
                {
                    success = ConstHelper.Fail,
                    message = "系统超时，请重新登陆",
                };
                json = JsonConvert.SerializeObject(SessionTimeout);
                context.Response.Write(json);
                return;
            }
            string strArticleID = context.Request.Unvalidated.Form["ArticleID"];
            string strMarkDown = context.Request.Unvalidated.Form["Content"];
            string strHTML = context.Request.Unvalidated.Form["HTML"];
            Article article = Article.GetArticleBySn(strArticleID);
            if (!article.OwnerId.Equals(context.Session[ConstHelper.Session_USERID].ToString()))
            {
                var NotTheOwner = new
                {
                    success = ConstHelper.Fail,
                    message = "你所编辑的文章的所有者和您的登陆用户不匹配",
                };
                json = JsonConvert.SerializeObject(NotTheOwner);
                context.Response.Write(json);
                return;
            }

            ArticleContent.SaveMarkDownVersion(strArticleID, strMarkDown, article.Sn, RevisionType.Draft);
            ArticleContent.SaveHTMLVersion(strArticleID, strHTML, article.Sn);
            if (article.IsFirstPage)
            {
                //首页发布处理
                Article.Publish(strArticleID, strMarkDown, strHTML);
            }
            else
            {
                //非首页处理
                ArticleContent.SaveMarkDownVersion(strArticleID, strMarkDown, article.Sn, RevisionType.Current);
            }
            if (!article.IsPrivate && !article.IsFirstPage)
            {
                //公开非首页,变成发布状态
                article.PublishStatus = ApproveStatus.Accept;
                article.PublishDateTime = System.DateTime.Now;
                if (article.IsPutToMyTopic)
                {
                    //发布后则加入到自己专题
                    var topic = Topic.GetTopicByAccountId(context.Session[ConstHelper.Session_USERID].ToString());
                    if (topic != null)
                    {
                        TopicArticle.InsertTopicArticle(new TopicArticle()
                        {
                            ArticleID = article.Sn,
                            TopicID = topic.Sn,
                            PublishStatus = ApproveStatus.NotNeed
                        });
                    }
                }
                Article.UpdateArticle(article);
                ArticleListManager.RemoveArticleItemBody(article.Sn);
            }
            var Success = new
            {
                success = ConstHelper.Success,
                message = "保存成功",
            };
            json = JsonConvert.SerializeObject(Success);
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}