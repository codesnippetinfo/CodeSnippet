using System.Web;
using BlogSystem.Entity;
using Newtonsoft.Json;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for GetCommentMarkDown
    /// </summary>
    public class GetCommentMarkDown : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strCommentId = context.Request.Unvalidated.QueryString["CommentId"];
            var strContent = Comment.GetCommentBySn(strCommentId).ContentMD;
            context.Response.ContentType = "text/json";
            var result = new
            {
                success = ConstHelper.Success,
                MDContent = strContent,
            };
            string json = JsonConvert.SerializeObject(result);
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