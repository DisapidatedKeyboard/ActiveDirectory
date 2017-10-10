using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// RenameGP 的摘要说明
    /// </summary>
    public class RenameGP : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string url = context.Request.Form["url"];
            string name = context.Request.Form["gpname"];
            string oldname = context.Request.Form["oldgroup"];

            string domainip = context.Session["domainip"].ToString();
            string domainname = context.Session["domainname"].ToString();
            string username = context.Session["username"].ToString();
            string password = context.Session["password"].ToString();
            string dc = context.Session["dc"].ToString();

            Operate op = new Operate(domainname, domainip, username, password, dc);

            string result = op.RenameGroup(url,oldname,name);
            context.Response.Write(result);
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