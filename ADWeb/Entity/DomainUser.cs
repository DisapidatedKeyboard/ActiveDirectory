using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ADWeb
{
   
    public class DomainUser
    {
        /// <summary>
        /// 用户ID   User ID
        /// </summary>        
        public string UserId { get; set; }


        /// <summary>
        /// 用户名 User name
        /// </summary>    
        public string UserName { get; set; }


        /// <summary>
        /// 登录名：xxx@xxx.com  Login name(UserPrincipalName)
        /// </summary>     
        public string UserPrincipalName { get; set; }


        /// <summary>
        /// 登录名:(Windows  2000以前版本) Login name(sAMAccountname) (before Windows 2000)
        /// </summary>
        public string sAMAccountname { get; set; }


        /// <summary>
        /// 用户显示名称    User display name
        /// </summary>
        public string displayName { get; set; }



        /// <summary>
        /// 用户电话号码   User Telephone
        /// </summary>      
        public string Telephone { get; set; }




        /// <summary>
        /// 邮箱   Email
        /// </summary>           
        public string Email { get; set; }


        /// <summary>
        /// 描述    Description
        /// </summary>
        public string Description { get; set; }



        /// <summary>
        /// 部门      Department
        /// </summary>
        public string Department { get; set; }



        /// <summary>
        /// 办公室     Office
        /// </summary>
        public string PhysicalDeliveryOfficeName { get; set; }



        /// <summary>
        /// 名       Firstname
        /// </summary>
        public string firstName { get; set; }


        /// <summary>
        /// 姓       Lastname
        /// </summary>
        public string lastName { get; set; }

        /// <summary>
        /// 原用户密码     Old password
        /// </summary>
        public string UserPwd { get; set; }


        /// <summary>
        /// 新密码         New password
        /// </summary>
        public string NewPwd { get; set; }



        /// <summary>
        /// 信息集合        Information List
        /// </summary>
        public List<DomainUser> List { get; set; }


        /// <summary>
        /// 类型          Account type
        /// </summary>
        public string type { get; set; }


        /// <summary>
        /// 获取用户全名      Get user fullname
        /// </summary>
        /// <returns></returns>
        public string getName()
        {
            return lastName + firstName;
        }


        /// <summary>
        ///账户启用状态       Account enable status
        /// </summary>
        public bool state { get; set; }
        
        
        /// <summary>
        /// 返回信息        Result information
        /// </summary>
        public string Message { get; set; }
    }
}