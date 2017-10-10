using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace ADWeb
{
    public class OuTreeNode
    {
        public string text { get; set; }
        public string id { get; set; }
 
        public List<OuTreeNode> nodes = new List<OuTreeNode>();
  

        public static OuTreeNode GetOuTree(string domainname,string username,string password,string domainip,string dc)
        {
            Operate op = new Operate(domainname,domainip,username,password,dc);
            string domainName = string.Empty;
            OuTreeNode rootNode = null;
            DirectoryContextType type = new DirectoryContextType();
            DirectoryContext text = new DirectoryContext(type, op.adminUser, op.adminPwd);            
            using (Domain domain = Domain.GetDomain(text)) 
            {
                domainName = domain.Name;
                //使用域节点来构造OU树的根节点  Use the domain node to construct the root node of the OU tree
                rootNode = new OuTreeNode() { text = domainName };
                //递归的查找子节点，构造OU树    Recursively finds the child nodes and constructs the OU tree
                GetOuTreeRecursivly(rootNode, domain.GetDirectoryEntry());   
            }
            return rootNode;
        }

        //递归构造树节点  Recursive construction tree node
        private static void GetOuTreeRecursivly(OuTreeNode parentNode, DirectoryEntry parentDirectoryEntry)
        {
            using (DirectorySearcher ds = new DirectorySearcher(parentDirectoryEntry))
            {
                ds.Filter = "(objectClass=organizationalunit)";
                ds.SearchScope = SearchScope.OneLevel; //仅查找当前OU下的子OU  Just search the child OU under the current OU

                try
                {
                    using (SearchResultCollection result = ds.FindAll())
                    {
                        foreach (SearchResult entry in result)
                        {
                            //获取Name属性 Get Name attribute
                            string name = entry.GetDirectoryEntry().Properties["Name"].Value.ToString();  
                            byte[] bytes = entry.GetDirectoryEntry().Properties["ObjectGuid"].Value as byte[];
                            Guid id = new Guid(bytes);
                            //获取id属性  Get id attribute
                            OuTreeNode node = new OuTreeNode() { text = name, id = id.ToString() };
                            parentNode.nodes.Add(node);
                            using (DirectoryEntry child = entry.GetDirectoryEntry())
                            {
                                GetOuTreeRecursivly(node, child);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }        
    }
}