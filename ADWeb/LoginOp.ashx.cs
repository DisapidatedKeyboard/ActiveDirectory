using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// LoginOp 的摘要说明
    /// </summary>
    public class LoginOp : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string domainname = context.Request.Form["domainname"];
            string domainip = context.Request.Form["domainip"];
            string username=context.Request.Form["user"];
            string password = context.Request.Form["pwd"];

            if (domainname != "" && domainip != "" && username != "" && password != "")
            {
                string[] array = domainname.Split('.');
                string name = array[0];     //域名前缀  Domain prefix
                string dc = array[1];       //域名后缀  Domain suffix

                Operate op = new Operate(name, domainip, username, password, dc);
                if (op.Verification())
                {
                    //在这里验证连接并将正确的连接参数记录到Session中 
                    //Verify the connection here and record the correct connection parameters into Session  
                    context.Session["domainip"] = domainip;
                    context.Session["domainname"] = name;
                    context.Session["username"] = username;
                    context.Session["password"] = password;
                    context.Session["dc"] = dc;
                    context.Response.Redirect("ListIndex.html", false);
                }
                else
                {
                    context.Response.Write("<script language=javascript>alert('Login error!Check the input information!');window.location='Login.html'</script>");
                }
            }
            else
            {
                context.Response.Write("<script language=javascript>alert('Information can not be empty!');window.location='Login.html'</script>");
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