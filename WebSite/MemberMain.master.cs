using MyCompany.Services;
using MyCompany.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;



public partial class MemberMain : System.Web.UI.MasterPage
{
    
    public static string[] MicrosoftJavaScript = new string[] {
            "MicrosoftAjax.js",
            "MicrosoftAjaxWebForms.js"};
    
    static Main()
    {
        bool releaseMode = true;
        AquariumExtenderBase.EnableMinifiedScript = releaseMode;
        ApplicationServices.EnableMinifiedCss = releaseMode;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (AquariumExtenderBase.EnableCombinedScript)
        	sm.EnableScriptLocalization = false;
        string pageCssClass = (Page.GetType().Name + " Loading");
        PropertyInfo p = Page.GetType().GetProperty("CssClass");
        if (null != p)
        {
            string cssClassName = ((string)(p.GetValue(Page, null)));
            if (!(String.IsNullOrEmpty(pageCssClass)))
            	pageCssClass = (pageCssClass + " ");
            pageCssClass = (pageCssClass + cssClassName);
        }
        if (!(pageCssClass.Contains("Wide")))
        	pageCssClass = (pageCssClass + " Standard");
        LiteralControl c = ((LiteralControl)(Page.Form.Controls[0]));
        if ((null != c) && !(String.IsNullOrEmpty(pageCssClass)))
        	c.Text = Regex.Replace(c.Text, "<div>", String.Format("<div class=\"{0}\">", pageCssClass));
        c = ((LiteralControl)(PageContentPlaceHolder.Controls[0]));
    }
    
    protected void Page_PreRender(object sender, EventArgs e)
    {
        ApplicationServices.RegisterCssLinks(Page);
    }
}
