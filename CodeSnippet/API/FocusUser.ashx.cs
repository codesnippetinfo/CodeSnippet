using BlogSystem.Entity;
using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for FocusUser
    /// </summary>
    public class FocusUser : IHttpHandler, IRequiresSessionState
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
            string strAccountID = context.Request.QueryString["AccountID"];
            string strOwnerID = context.Session[ConstHelper.Session_USERID].ToString();
            bool isOK = Focus.FocusAccount(strOwnerID, strAccountID);
            if (isOK)
            {
                var Success = new
                {
                    success = ConstHelper.Success,
                    message = "关注成功",
                };
                json = JsonConvert.SerializeObject(Success);
            }
            else
            {
                var Success = new
                {
                    success = ConstHelper.Fail,
                    message = "关注失败（已经关注）",
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