using MyCompany.Data;
using MyCompany.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;



public partial class _Default : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Params["_page"] == "_blank")
        	return;
        string link = Request.Params["_link"];
        if (!(String.IsNullOrEmpty(link)))
        {
            StringEncryptor enc = new StringEncryptor();
            string[] permalink = enc.Decrypt(link.Split(',')[0]).Split('?');
            Page.ClientScript.RegisterStartupScript(GetType(), "Redirect", String.Format("location.replace(\'{0}?_link={1}\');\r\n", permalink[0], HttpUtility.UrlEncode(link)), true);
        }
        else
        	Response.Redirect(ApplicationServices.HomePageUrl);
    }
}
