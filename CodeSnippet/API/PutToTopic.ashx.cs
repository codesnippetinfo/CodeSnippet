using BlogSystem.Entity;
using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for Stock
    /// </summary>
    public class PutToTopic : IHttpHandler, IRequiresSessionState
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
            string strArticleID = context.Request.QueryString["ArticleID"];
            string strOwnerID = context.Session[ConstHelper.Session_USERID].ToString();
            bool isOK = TopicArticle.PutToTopic(strOwnerID, strArticleID);
            if (isOK)
            {
                var article = Article.GetArticleBySn(strArticleID);
                var Success = new
                {
                    success = ConstHelper.Success,
                    message = article.IsNeedTopicApproval ? "发送收录请求成功" : "收录成功",
                };
                json = JsonConvert.SerializeObject(Success);
            }
            else
            {
                var Success = new
                {
                    success = ConstHelper.Fail,
                    message = "收录失败",
                };
                json = JsonConvert.SerializeObject(Success);
            }
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