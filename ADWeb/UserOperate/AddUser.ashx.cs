using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// AddUser 的摘要说明
    /// </summary>
    public class AddUser : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string url=context.Request.Form["url"];
            DomainUser user = new DomainUser();
            if (context.Request.Form["lastname_add"] != "" && context.Request.Form["firstname_add"] != "" && context.Request.Form["loginname_add"] != "")
            {
                user.lastName = context.Request.Form["lastname_add"];
                user.firstName = context.Request.Form["firstname_add"];
                user.sAMAccountname = context.Request.Form["loginname_add"];
                user.UserPrincipalName = context.Request.Form["loginname_add"];
                if (context.Request.Form["displayname_add"] != "")
                {
                    user.displayName = context.Request.Form["displayname_add"];
                }
                if (context.Request.Form["description_add"] != "")
                {
                    user.Description = context.Request.Form["description_add"];
                }
                if (context.Request.Form["department_add"] != "")
                {
                    user.Department = context.Request.Form["department_add"];
                }
                if (context.Request.Form["telphone_add"] != "")
                {
                    user.Telephone = context.Request.Form["telphone_add"];
                }
                if (context.Request.Form["password_add"] != "" && context.Request.Form["confirmpwd_add"] != "")
                {
                    user.UserPwd = context.Request.Form["password_add"];
                    string confim = context.Request.Form["confirmpwd_add"];
                    if (user.UserPwd == confim)
                    {
                        string domainip = context.Session["domainip"].ToString();
                        string domainname = context.Session["domainname"].ToString();
                        string username = context.Session["username"].ToString();
                        string password = context.Session["password"].ToString();
                        string dc = context.Session["dc"].ToString();

                        Operate op = new Operate(domainname, domainip, username, password, dc);

                        string result = op.adduser(url,user);
                        context.Response.Write(result);
                    }
                    else
                    {
                        context.Response.Write("The password entered for the two time is inconsistent!");
                    }
                }
                else
                {
                    context.Response.Write("Password cannot be empty!");
                }
            }
            else
            {
                context.Response.Write("Firstname、Lastname and Loginname is required!");
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