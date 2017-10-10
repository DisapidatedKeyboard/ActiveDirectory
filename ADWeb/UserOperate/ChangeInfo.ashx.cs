using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// ChangeInfo 的摘要说明
    /// </summary>
    public class ChangeInfo : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            DomainUser user = new DomainUser();
            //user.UserName = context.Request.Form["newname"];  use this when you want to rename 
            user.lastName = context.Request.Form["lastname"];
            user.firstName = context.Request.Form["firstname"];
            user.sAMAccountname = context.Request.Form["loginname"];
            user.UserPrincipalName = context.Request.Form["loginname"];
            user.displayName = context.Request.Form["displayname"];
            user.Description = context.Request.Form["description"];
            user.Department = context.Request.Form["department"];
            user.Telephone = context.Request.Form["telphone"];
            string olduser = context.Request.Form["olduser"];
            string url = context.Request.Form["url"];

            string domainip = context.Session["domainip"].ToString();
            string domainname = context.Session["domainname"].ToString();
            string username = context.Session["username"].ToString();
            string password = context.Session["password"].ToString();
            string dc = context.Session["dc"].ToString();

            Operate op = new Operate(domainname, domainip, username, password, dc);

            string result =op.update(url,olduser,user);

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