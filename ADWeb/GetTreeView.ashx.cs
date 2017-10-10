using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ADWeb
{
    /// <summary>
    /// GetTreeView 的摘要说明
    /// </summary>
    public class GetTreeView : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //添加GetOuTree的内部Operate初始化参数
            //Add the internal Operate initialization parameter of GetOuTree
            string domainip = context.Session["domainip"].ToString();
            string domainname = context.Session["domainname"].ToString();
            string username = context.Session["username"].ToString();
            string password = context.Session["password"].ToString();
            string dc = context.Session["dc"].ToString();
            

            JObject jobject = JObject.FromObject( OuTreeNode.GetOuTree(domainname, username, password, domainip, dc));
            JArray jarray = new JArray();
            jarray.Add(jobject);
            string json = jarray.ToString();
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