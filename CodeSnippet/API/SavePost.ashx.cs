using BlogSystem.Entity;
using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for SavePost
    /// </summary>
    public class SavePost : IHttpHandler, IRequiresSessionState
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
                    message = "[保存失败]系统超时，请重新登陆",
                };
                json = JsonConvert.SerializeObject(SessionTimeout);
                context.Response.Write(json);
                return;
            }
            string strArticleID = context.Request.Unvalidated.Form["ArticleID"];
            string strMarkDown = context.Request.Unvalidated.Form["Content"];
            Article article = Article.GetArticleBySn(strArticleID);
            if (!article.OwnerId.Equals(context.Session[ConstHelper.Session_USERID].ToString()))
            {
                var NotTheOwner = new
                {
                    success = ConstHelper.Fail,
                    message = "[保存失败]你所编辑的文章的所有者和您的登陆用户不匹配",
                };
                json = JsonConvert.SerializeObject(NotTheOwner);
                context.Response.Write(json);
                return;
            }
            ArticleContent.SaveMarkDownVersion(strArticleID, strMarkDown, article.Sn, RevisionType.Draft);
            context.Response.ContentType = "text/json";
            string strfree = UploadFile.GetFreeVolumnByAccountId(context.Session[ConstHelper.Session_USERID].ToString());
            var result = new
            {
                success = ConstHelper.Success,
                message = "[保存成功]上次保存成功时间:" + System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + ",剩余图片空间：" + strfree,
            };
            json = JsonConvert.SerializeObject(result);
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