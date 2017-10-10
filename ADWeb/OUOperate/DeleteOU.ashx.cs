using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// DeleteOU 的摘要说明
    /// </summary>
    public class DeleteOU : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string name = context.Request.Form["name"];
            string url = context.Request.Form["url"];

            string domainip = context.Session["domainip"].ToString();
            string domainname = context.Session["domainname"].ToString();
            string username = context.Session["username"].ToString();
            string password = context.Session["password"].ToString();
            string dc = context.Session["dc"].ToString();

            Operate op = new Operate(domainname, domainip, username, password, dc);

            string result = op.DeleteOrganizeUnit(url,name);
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