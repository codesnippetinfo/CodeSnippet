using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;
using BlogSystem.Entity;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for JoinCircle
    /// </summary>
    public class JoinCircle : IHttpHandler, IRequiresSessionState
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
            string strTagName = context.Request.QueryString["TagName"];
            string strOwnerID = context.Session[ConstHelper.Session_USERID].ToString();
            bool isOK = UserInfo.JoinTag(strTagName,strOwnerID);
            if (isOK)
            {
                var Success = new
                {
                    success = ConstHelper.Success,
                    message = "加入成功",
                };
                json = JsonConvert.SerializeObject(Success);
            }
            else
            {
                var Success = new
                {
                    success = ConstHelper.Fail,
                    message = "加入失败（已经加入）",
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