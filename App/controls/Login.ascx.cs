using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class Controls_Login : System.Web.UI.UserControl
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated && !(String.IsNullOrEmpty(Request.Params["ReturnUrl"])))
        	Response.Redirect("~/Pages/Home.aspx");
    }
}
