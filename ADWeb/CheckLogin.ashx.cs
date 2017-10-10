using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// CheckLogin 的摘要说明
    /// </summary>
    public class CheckLogin : IHttpHandler, IRequiresSessionState
    {
        //登录状态验证    Login status verification
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Session["domainip"] == null)
            {
                context.Response.Write("error");
            }
            else
            {
                string domainname = context.Session["domainname"].ToString();
                string page = context.Request.Form["Check"];
                if (page == "ListIndex")
                {
                    context.Response.Write(domainname);
                }
                else
                {
                    context.Response.Write("error");
                }
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