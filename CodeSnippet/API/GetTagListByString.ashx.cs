using System.Web;
using Newtonsoft.Json;
using BlogSystem.TagSystem;
namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for GetTagListByString
    /// </summary>
    public class GetTagListByString : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string strTitle = context.Request.Form["strTitle"];
            context.Response.ContentType = "text/json";
            string json = string.Empty;
            var Success = new
            {
                success = ConstHelper.Success,
                TagList = TagUtility.getTagsFromTitle(strTitle),
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