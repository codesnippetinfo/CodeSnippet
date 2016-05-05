using BlogSystem.Entity;
using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for Stock
    /// </summary>
    public class StockArticle : IHttpHandler, IRequiresSessionState
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
            bool isOK = Stock.StockArticle(strOwnerID, strArticleID);
            if (isOK)
            {
                var Success = new
                {
                    success = ConstHelper.Success,
                    message = "收藏成功",
                };
                json = JsonConvert.SerializeObject(Success);
            }
            else
            {
                var Success = new
                {
                    success = ConstHelper.Fail,
                    message = "收藏失败（已经收藏）",
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