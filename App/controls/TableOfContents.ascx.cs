using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class Controls_TableOfContents : System.Web.UI.UserControl
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!(IsPostBack))
        {
            TreeView1.DataBind();
            ConfigureNodeTargets(TreeView1.Nodes);
        }
    }
    
    private void ConfigureNodeTargets(TreeNodeCollection nodes)
    {
        foreach (TreeNode n in nodes)
        {
            Match m = Regex.Match(n.NavigateUrl, "^(_\\w+):(.+)$");
            if (m.Success)
            {
                n.Target = m.Groups[1].Value;
                n.NavigateUrl = m.Groups[2].Value;
            }
            ConfigureNodeTargets(n.ChildNodes);
        }
    }
}
