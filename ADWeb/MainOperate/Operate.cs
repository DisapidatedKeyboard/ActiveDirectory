using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace ADWeb
{
    public class Operate
    {
        /// <summary>
        /// 域名
        /// Domain name
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        ///主机域 IP
        ///Domain Host IP
        /// </summary>
        public string domainIp { get; set; }
        /// <summary>
        /// 管理员账号
        /// Administrator account
        /// </summary>
        public string adminUser { get; set; }
        /// <summary>
        /// 管理员密码
        /// Administrator password
        /// </summary>
        public string adminPwd { get; set; }
        /// <summary>
        /// 路径的最前端
        /// Path head
        /// </summary>
        public string pathHead { get; set; }
        /// <summary>
        /// 路径的最后端
        /// Path foot
        /// </summary>
        public string pathFoot { get; set; }



        #region 构造函数    Constructor
        /// <summary>
        /// 构造函数    Constructor
        ///string domain,域名     Domain name
        ///string adUser,管理员用户名     Administrator name
        ///string adPwd,管理员密码       Administrator password
        ///string domainIp,域主机地址    Domain host address
        /// </summary>
        public Operate(string domainname, string domainip, string adminname, string adminpassword,string dc)
        {
            domain = domainname;
            domainIp = domainip;
            adminUser = adminname;
            adminPwd = adminpassword;
            pathHead = "LDAP://" + domainIp + "/";
            pathFoot = "DC=" + domain + ",DC="+dc;
        }
        #endregion


        #region 登录验证    Login Validation
        /// <summary>
        /// 验证域登录       Validate domain logon
        /// </summary>
        /// <returns></returns>
        public bool Verification()
        {
            DirectoryEntry entry = new DirectoryEntry(pathHead + pathFoot, adminUser, adminPwd);
            try
            {                
                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + adminUser + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(" Authenticating user fail!" + ex.Message);                
            } 
        }
        #endregion


        #region  获取指定域内所有用户账号信息     Get all user account information in the specified domain
        /// <summary>
        /// 获取指定域内所有用户账号信息      Gets all user account information in the specified domain
        /// </summary>
        /// <param name="url">域路径 Domain path</param>
        /// <returns></returns>
        public List<DomainUser> getAllUser(string url)
        {
            List<DomainUser> user = new List<DomainUser>();
            DirectoryEntry ent = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd);
            //循环子节点取值    Value of cyclic sub node        
            foreach (DirectoryEntry child in ent.Children)
            {
                DomainUser domainuser = new DomainUser();
                string[] sArray = child.Name.Split('=');
                domainuser.UserName = sArray[1];
                domainuser.type = child.SchemaClassName;
                if (child.Properties["userAccountControl"].Value != null)
                {
                    //判断账号是否是正常启用状态     Determine whether the account is normally enabled
                    if (child.Properties["userAccountControl"].Value.ToString() == "66048" || child.Properties["userAccountControl"].Value.ToString() == "512")
                    {
                        domainuser.state = true;
                    }
                    else
                    {
                        domainuser.state = false;
                    }
                }
                else
                {
                    domainuser.state = false;
                }
                if (child.Properties["description"].Value != null)
                {
                    //获取账号描述信息      Get account description information
                    domainuser.Description = child.Properties["description"].Value.ToString();
                }
                else
                {
                    domainuser.Description = "";
                }
                user.Add(domainuser);
            }
            ent.Close();
            return user;
        }
        #endregion


        #region  新建用户账户     Add user account
        /// <summary>
        /// 新建账户   Add user account
        /// </summary>
        /// <param name="url">新建账户所在路径 The path to the new account</param>
        /// <param name="user">用户对象   DomainUser object</param>
        /// <returns></returns>

        public string adduser(string url, DomainUser user)
        {
            string result = string.Empty;
            try
            {
                user.UserName = user.lastName + user.firstName;
                DirectoryEntry ent = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd);
                DirectoryEntry newUser = ent.Children.Add("CN=" + user.UserName, "user");
                if (user.lastName != "")
                {
                    //设置姓       Set lastname
                    newUser.Properties["sn"].Value = user.lastName;
                }
                if (user.firstName != "")
                {
                    //设置名       Set firstname
                    newUser.Properties["givenName"].Value = user.firstName;
                }
                //设置登录账号名   Set login name
                newUser.Properties["sAMAccountname"].Value = user.sAMAccountname;
                //设置登录账号名   Set login name
                newUser.Properties["userPrincipalName"].Value = user.UserPrincipalName;
                if (user.displayName != "" && user.displayName != null)
                {
                    //设置显示名    Set display name
                    newUser.Properties["displayName"].Value = user.displayName;
                }
                if (user.Department != "" && user.Department != null)
                {
                    //设置部门      Set department
                    newUser.Properties["department"].Value = user.Department;
                }
                if (user.Description != "" && user.Description != null)
                {
                    //设置描述      Set description
                    newUser.Properties["description"].Value = user.Description;
                }
                if (user.Telephone != "" && user.Telephone != null)
                {
                    //设置电话号码    Set telephone number
                    newUser.Properties["telephoneNumber"].Value = user.Telephone;
                }
                newUser.CommitChanges();
                //设置密码          Set password 
                newUser.Invoke("SetPassword", new object[] { user.UserPwd });
                //启用账号          Enable account
                newUser.Properties["userAccountControl"].Value = 66048;
                newUser.CommitChanges();

                ent.Close();
                newUser.Close();
                result = "Add new user success！";
                return result;
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {                
                result = "Add new user error！" + E.Message.ToString();
                return result;
            }
        }
        #endregion


        #region  删除用户账号       Delete user account
        /// <summary>
        /// 删除用户账号
        /// </summary>
        /// <param name="url">用户路径  User path</param>
        /// <param name="name">用户名 User's name</param>
        /// <returns></returns>

        public string deleteuser(string url, string name)
        {
            try
            {
                if (name != null || name != "")
                {
                    DirectoryEntry ent = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd);
                    DirectoryEntry user = ent.Children.Find("CN=" + name);
                    ent.Children.Remove(user);
                    ent.CommitChanges();

                    ent.Close();
                    return "Delete success!";
                }
                else
                {
                    return "User name is empty！";
                }
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {                                
                return "Delete fail!" + E.Message.ToString();
            }
        }
        #endregion


        #region  修改更新用户信息  Update user's information
        /// <summary>
        /// 修改更新用户信息    Update user's information
        /// </summary>
        /// <param name="url">用户路径 user path</param>
        /// <param name="olduser">原用户名 old user name</param>
        /// <param name="user">用户对象 DomainUser object</param>
        /// <returns></returns>

        public string update(string url, string olduser, DomainUser user)
        {
            try
            {
                DirectoryEntry ent = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd);
                DirectoryEntry newUser = ent.Children.Find("CN=" + olduser);
                //AD域只允许把空改为非空，不能将非空改为空
                //Can't change the empty information to non empty
                if (user.lastName != "" && user.lastName != null)
                {
                    newUser.Properties["sn"].Value = user.lastName;
                }
                if (user.firstName != "" && user.firstName != null)
                {
                    newUser.Properties["givenName"].Value = user.firstName;
                }
                if (user.sAMAccountname != "" && user.sAMAccountname != null)
                {
                    newUser.Properties["sAMAccountname"].Value = user.sAMAccountname;
                }
                if (user.UserPrincipalName != "" && user.UserPrincipalName != null)
                {
                    newUser.Properties["userPrincipalName"].Value = user.UserPrincipalName;
                }
                if (user.Description != "" && user.Description != null)
                {
                    newUser.Properties["description"].Value = user.Description;
                }
                if (user.displayName != "" && user.displayName != null)
                {
                    newUser.Properties["displayName"].Value = user.displayName;
                }
                if (user.Department != "" && user.Department != null)
                {
                    newUser.Properties["department"].Value = user.Department;
                }
                if (user.Telephone != "" && user.Telephone != null)
                {
                    newUser.Properties["telephoneNumber"].Value = user.Telephone;
                }
                newUser.CommitChanges();

                //修改用户名     Update user name(use it if you want)
                //if (user.UserName!=""&&user.UserName!=olduser)
                //{
                //    newUser.Rename("CN=" + user.UserName);
                //    newUser.CommitChanges();
                //}


                //修改密码      Change password
                if (user.UserPwd != null && user.NewPwd != null & user.UserPwd != "" && user.NewPwd != "")
                {
                    try
                    {
                        newUser.Invoke("ChangePassword", new object[] { user.UserPwd, user.NewPwd });
                        newUser.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        string message = "";
                        Exception baseException = ex.GetBaseException();
                        if (baseException is COMException)
                        {
                            COMException comException = baseException as COMException;
                            switch (comException.ErrorCode)
                            {
                                case -2147024810:
                                    message = "The original password error!";
                                    break;
                                case -2147022651:
                                    message = "Indicates that the password does not conform to the domain security requirements!";
                                    break;
                                case -2147023570:
                                    message = "Invalid password!";
                                    break;
                                case -2147016657:
                                    message = "User password is invalid!";
                                    break;
                                default:
                                    message = "Unknown error!";
                                    break;
                            }
                            return message;
                        }
                    }

                }

                ent.Close();
                newUser.Close();
                return "Update information success！";

            }
            catch (DirectoryServicesCOMException E)
            {                
                return "Update information！ fail!" + E.Message.ToString();
            }
        }
        #endregion


        #region  查询用户详细信息   Query user details
        /// <summary>
        /// 查询详细用户信息    Query user details
        /// </summary>
        /// <param name="url">账户路径 Account path </param>
        /// <param name="name">账户名 Account name</param>
        /// <returns></returns>
        public DomainUser getuserinfo(string url, string name)
        {
            DomainUser user = new DomainUser();
            try
            {
                DirectoryEntry ent = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd);
                DirectoryEntry UserInfo = ent.Children.Find("CN=" + name);
                if (UserInfo.Properties["givenName"].Value != null)
                {
                    user.firstName = UserInfo.Properties["givenName"].Value.ToString();
                }
                else
                {
                    user.firstName = "";
                }

                if (UserInfo.Properties["sn"].Value != null)
                {
                    user.lastName = UserInfo.Properties["sn"].Value.ToString();
                }
                else
                {
                    user.lastName = "";
                }
                user.sAMAccountname = UserInfo.Properties["sAMAccountname"].Value.ToString();
                user.UserPrincipalName = UserInfo.Properties["userPrincipalName"].Value.ToString();
                if (UserInfo.Properties["displayName"].Value != null)
                {
                    user.displayName = UserInfo.Properties["displayName"].Value.ToString();
                }
                else
                {
                    user.displayName = "";
                }
                if (UserInfo.Properties["description"].Value != null)
                {
                    user.Description = UserInfo.Properties["description"].Value.ToString();
                }
                else
                {
                    user.Description = "";
                }
                if (UserInfo.Properties["department"].Value != null)
                {
                    user.Department = UserInfo.Properties["department"].Value.ToString();
                }
                else
                {
                    user.Department = "";
                }
                if (UserInfo.Properties["telephoneNumber"].Value != null)
                {
                    user.Telephone = UserInfo.Properties["telephoneNumber"].Value.ToString();
                }
                else
                {
                    user.Telephone = "";
                }

                return user;
            }
            catch (DirectoryServicesCOMException E)
            {                
                user.Message = E.Message.ToString();
                return user;
            }
        }
        #endregion


        #region  账户启用与禁用    Forbidden account and enable account
        /// <summary>
        /// 账户的启用与禁用      Forbidden account and enable account
        /// </summary>
        /// <param name="url">账户路径  Account path</param>
        /// <param name="name">账户名 Account name</param>
        /// <returns></returns>
        public string softdelete(string url, string name)
        {
            DirectoryEntry ent = null;
            DirectoryEntry newUser = null;
            try
            {
                ent = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd);
                newUser = ent.Children.Find("CN=" + name);
                //判断账户的状态    Determine the status of an account               
                if (newUser.Properties["userAccountControl"].Value.ToString() == "66048" || newUser.Properties["userAccountControl"].Value.ToString() == "512")
                {
                    newUser.Properties["userAccountControl"].Value = 514;
                    newUser.CommitChanges();
                }
                else
                {
                    newUser.Properties["userAccountControl"].Value = 66048;
                    newUser.CommitChanges();
                }
                newUser.Close();
                return "Operate success！";
            }
            catch (DirectoryServicesCOMException E)
            {
                return "Operate fail！" + E.Message.ToString();
            }
            finally
            {
                if (newUser != null || ent != null)
                {
                    newUser.Dispose();
                    ent.Dispose();
                }
            }
        }
        #endregion


        #region 创建组     Create group
        /// <summary>
        /// 创建组         Create group
        /// </summary>
        /// <param name="url">组路径 Group path</param>
        /// <param name="name">组名称 Group name</param>
        /// <returns>是否创建成功  Create success or fail</returns>
        public string CreateGroup(string url, string name)
        {
            DirectoryEntry parentEntry = null;
            try
            {
                parentEntry = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                DirectoryEntry groupEntry = parentEntry.Children.Add("CN=" + name, "group");
                groupEntry.CommitChanges();                
                return "Create group success!";
            }
            catch (DirectoryServicesCOMException ex)
            {                
                return "Create group fail!" + ex.Message.ToString();
            }
            finally
            {
                if (parentEntry != null)
                {
                    parentEntry.Dispose();
                }
            }
        }
        #endregion


        #region 删除组     Delete group
        /// <summary>
        /// 删除组     Delete group
        /// </summary>
        /// <param name="url">组路径 Group path</param>
        /// <param name="name">组名称 Group name</param>
        /// <returns>是否删除成功 Delete success or fail</returns>
        public string DeleteGroup(string url, string name)
        {
            DirectoryEntry parentEntry = null;
            try
            {
                parentEntry = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                DirectoryEntry groupEntry = parentEntry.Children.Find("CN=" + name, "group");
                parentEntry.Children.Remove(groupEntry);
                groupEntry.CommitChanges();                
                return "Delete group success！";
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {                
                return "Delete group fail！" + ex.Message.ToString();
            }
            finally
            {
                if (parentEntry != null)
                {
                    parentEntry.Dispose();
                }
            }
        }
        #endregion


        #region 创建OU    Create OrganizeUnit
        /// <summary>
        /// 创建OU        Create OrganizeUnit
        /// </summary>        
        /// <param name="name">创建的OU名称 OrganizeUnit name</param>
        /// <param name="url">父组织单位 OrganizeUnit's parent path</param>
        /// <returns></returns>
        public string CreateOrganizeUnit(string url, string name)
        {
            DirectoryEntry parentEntry = null;
            try
            {
                parentEntry = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                DirectoryEntry organizeEntry = parentEntry.Children.Add("OU=" + name, "organizationalUnit");
                organizeEntry.CommitChanges();                
                return "Create OrganizeUnit success！";
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {                
                return "Create OrganizeUnit fail！" + ex.Message.ToString();
            }
            finally
            {
                if (parentEntry != null)
                {
                    parentEntry.Dispose();
                }
            }
        }
        #endregion


        #region 删除OU    Delete OrganizeUnit
        /// <summary>
        /// 删除OU        Delete OrganizeUnit
        /// </summary>
        /// <param name="name">OU名称 OrganizeUnit name</param>
        /// <param name="url">父组织单位 OrganizeUnit's parent path</param>
        /// <returns></returns>
        public string DeleteOrganizeUnit(string url, string name)
        {
            DirectoryEntry parentEntry = null;
            try
            {                
                parentEntry = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                DirectoryEntry organizeEntry = parentEntry.Children.Find("OU=" + name, "organizationalUnit");
                DirectoryEntry ouParnet = organizeEntry.Parent;
                //先删除组织单元下的用户或者组
                //First delete the user or group under the organizational unit
                ouParnet.Children.Remove(organizeEntry);
                ouParnet.CommitChanges();                
                return "Delete OrganizeUnit success！";
            }
            catch (DirectoryServicesCOMException ex)
            {                
                return "Delete OrganizeUnit fail！" + ex.Message.ToString();
            }
            finally
            {
                if (parentEntry != null)
                {
                    parentEntry.Dispose();
                }
            }
        }
        #endregion


        #region 重命名组    Rename group
        /// <summary>
        /// 重命名组        Rename group
        /// </summary>
        /// <param name="url">组所在域中的路径  Group path</param>
        /// <param name="oldname">原组名  Old group name</param>
        /// <param name="newname">新组名  New group name</param>
        /// <returns></returns>
        public string RenameGroup(string url, string oldname, string newname)
        {
            DirectoryEntry OUEntry = null;
            DirectoryEntry group = null;
            try
            {                
                OUEntry = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                group = OUEntry.Children.Find("CN=" + oldname, "group");
                group.Rename("CN=" + newname);
                group.CommitChanges();
                return "Rename success！";
            }
            catch (DirectoryServicesCOMException E)
            {
                return "Rename fail！" + E.Message.ToString();
            }
            finally
            {
                if (OUEntry != null || group != null)
                {
                    OUEntry.Dispose();
                    group.Dispose();
                }
            }

        }
        #endregion


        #region 重命名OU   Rename OrganizeUnit
        /// <summary>
        /// 重命名OU Rename OrganizeUnit
        /// </summary>
        /// <param name="url">OU路径 OrganizeUnit path</param>
        /// <param name="oldname">原OU名称 Old OrganizeUnit name</param>
        /// <param name="newname">新OU名称 New OrganizeUnit name</param>
        /// <returns></returns>

        public string RenameOU(string url, string oldname, string newname)
        {
            DirectoryEntry OUEntry = null;
            DirectoryEntry ou = null;
            try
            {                
                OUEntry = new DirectoryEntry(pathHead + url + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                ou = OUEntry.Children.Find("OU=" + oldname, "organizationalUnit");
                ou.Rename("OU=" + newname);
                ou.CommitChanges();
                return "Rename success！";
            }
            catch (DirectoryServicesCOMException E)
            {
                return "Rename fail！" + E.Message.ToString();
            }
            finally
            {
                if (OUEntry != null || ou != null)
                {
                    OUEntry.Dispose();
                    ou.Dispose();
                }
            }

        }
        #endregion


        #region 移动对象到容器     Moving objects to containers
        /// <summary>
        /// 移动对象到容器         Moving objects to containers
        /// </summary>
        /// <param name="oldurl">原对象路径  Original object path</param>
        /// <param name="newurl">将要移动到的容器路径    New object path</param>
        /// <param name="name">对象名 Object name</param>
        /// <returns></returns>
        public string MoveTo(string oldurl, string newurl, string name)
        {
            DirectoryEntry NewUser = null;
            try
            {
                NewUser = new DirectoryEntry(pathHead + "CN=" + name + "," + oldurl + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                NewUser.MoveTo(new DirectoryEntry(pathHead + newurl + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure));
                return "Move success！";
            }
            catch (DirectoryServicesCOMException ex)
            {
                return "Move fail！" + ex.Message.ToString();
            }
            finally
            {
                if (NewUser != null)
                {
                    NewUser.Dispose();
                }
            }
        }
        #endregion


        #region 移动OU到容器     Moving OrganizeUnit to containers
        /// <summary>
        /// 移动OU到容器     Moving OrganizeUnit to containers
        /// </summary>
        /// <param name="oldurl">原OU路径  Original OrganizeUnit path</param>
        /// <param name="newurl">将要移动到的容器路径   New OrganizeUnit path</param>
        /// <param name="name">OU名称 OrganizeUnit name</param>
        /// <returns></returns>
        public string MoveOUTo(string oldurl, string newurl, string name)
        {
            DirectoryEntry NewUser = null;
            try
            {
                NewUser = new DirectoryEntry(pathHead + "OU=" + name + "," + oldurl + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure);
                NewUser.MoveTo(new DirectoryEntry(pathHead + newurl + pathFoot, adminUser, adminPwd, AuthenticationTypes.Secure));
                return "Move success！";
            }
            catch (DirectoryServicesCOMException ex)
            {
                return "Move fail！" + ex.Message.ToString();
            }
            finally
            {
                if (NewUser != null)
                {
                    NewUser.Dispose();
                }
            }
        }
        #endregion


    }
}



