using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// GetAllUser 的摘要说明
    /// </summary>
    public class GetAllUser : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string url = context.Request.Form["url"];
            string table = "";
            string status = "";         //账户类型，控制是否显示操作按钮（visibility:visible）这里采用disabled属性            
            string image = "";        //是否禁用，不同状态的按钮显示不用文字;
            string hint = "";           //鼠标悬停提示文字

            string domainip = context.Session["domainip"].ToString();
            string domainname = context.Session["domainname"].ToString();
            string username = context.Session["username"].ToString();
            string password = context.Session["password"].ToString();
            string dc = context.Session["dc"].ToString();

            Operate op = new Operate(domainname, domainip, username, password, dc);

            List<DomainUser> domainuser = op.getAllUser(url);
            int count = domainuser.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (domainuser[i].type == "user")
                    {
                        status = "";
                    }
                    else
                    {
                        status = "disabled='true'";
                    }
                    if (domainuser[i].state)
                    {
                        image = "../images/disable.png";
                        hint = "Disable";
                    }
                    else
                    {
                        image = "../images/enable.png";
                        hint = "Enable";
                    }
                    
                    table += "<tr>";
                    table += "<td>" + domainuser[i].UserName + "</td>";
                    table += "<td>" + domainuser[i].type + "</td>";
                    table += "<td>" + domainuser[i].Description + "</td>";
                    table += "<td>" + "<button id = \"update\" title=\"Update\"  class=\"btnchange\" onclick=\"Update('" + domainuser[i].UserName + "','" + domainuser[i].type + "')\"></button>" + "<button id = \"delete\" title=\"Delete\"  class=\"btndelete\" onclick=\"Delete('" + domainuser[i].UserName + "','" + domainuser[i].type + "')\"></button>" + "<button id = \"check\" title=\""+hint+ "\" style=\"background-image: url('" + image + "')\" " + status + "\" class=\"btncheck\" onclick=\"Check('" + domainuser[i].UserName + "')\"></button>" + "<button id = \"move\" title=\"Move\"  class=\"btnmove\" onclick=\"Moveto('" + domainuser[i].UserName + "','" + domainuser[i].type + "')\"></button>" + "</td>";
                    table += "</tr>";
                }

                context.Response.Write(table);
            }
            else
            {
                context.Response.Write("");
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