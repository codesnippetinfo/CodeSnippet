using BlogSystem.BussinessLogic;
using BlogSystem.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for ReadMessage
    /// </summary>
    public class ReadMessage : IHttpHandler, IRequiresSessionState
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
            string strMessageId = context.Request.QueryString["MessageId"];
            if (SiteMessage.CloseMessage(strMessageId, context.Session[ConstHelper.Session_USERID].ToString())){
                var result = new
                {
                    success = ConstHelper.Success,
                    message = "已处理"
                };
                json = JsonConvert.SerializeObject(result);
                context.Response.Write(json);
            }
            else
            {
                var result = new
                {
                    success = ConstHelper.Fail,
                    message = "未处理"
                };
                json = JsonConvert.SerializeObject(result);
                context.Response.Write(json);
            }
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