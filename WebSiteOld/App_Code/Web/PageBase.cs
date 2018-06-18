using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MyCompany.Data;
using MyCompany.Services;

namespace MyCompany.Web
{
	public partial class PageBase : PageBaseCore
    {
    }
    
    public class PageBaseCore : System.Web.UI.Page
    {
        
        public virtual string Device
        {
            get
            {
                return null;
            }
        }
        
        protected override void InitializeCulture()
        {
            CultureManager.Initialize();
            base.InitializeCulture();
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ValidateUrlParameters();
            if (Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft)
            	foreach (Control c in Controls)
                	ChangeCurrentCultureTextFlowDirection(c);
            string mobileSwitch = Request.Params["_mobile"];
            if (String.IsNullOrEmpty(mobileSwitch))
            	mobileSwitch = Request.Params["_touch"];
            if (mobileSwitch != null)
            {
                HttpCookie cookie = new HttpCookie("appfactorytouchui", ((mobileSwitch == "true")).ToString().ToLower());
                if (String.IsNullOrEmpty(mobileSwitch))
                	cookie.Expires = DateTime.Today.AddDays(-1);
                else
                	cookie.Expires = DateTime.Now.AddDays(30);
                Response.AppendCookie(cookie);
                Response.Redirect(Request.CurrentExecutionFilePath);
            }
            bool isTouchUI = ApplicationServices.IsTouchClient;
            if (((Device == "touch") && !(isTouchUI)) || ((Device == "desktop") && isTouchUI))
            	Response.Redirect("~/");
            ApplicationServices.VerifyUrl();
        }
        
        private bool ChangeCurrentCultureTextFlowDirection(Control c)
        {
            if (c is HtmlGenericControl)
            {
                HtmlGenericControl gc = ((HtmlGenericControl)(c));
                if (gc.TagName == "body")
                {
                    gc.Attributes["dir"] = "rtl";
                    gc.Attributes["class"] = "RTL";
                    return true;
                }
            }
            else
            	foreach (Control child in c.Controls)
                {
                    bool result = ChangeCurrentCultureTextFlowDirection(child);
                    if (result)
                    	return true;
                }
            return false;
        }
        
        protected virtual string HideUnauthorizedDataViews(string content)
        {
            bool tryRoles = true;
            while (tryRoles)
            {
                Match m = Regex.Match(content, "\\s*\\bdata-roles\\s*=\\s*\"([\\S\\s]*?)\"");
                tryRoles = m.Success;
                if (tryRoles)
                {
                    string stringAfter = content.Substring((m.Index + m.Length));
                    if (DataControllerBase.UserIsInRole(m.Groups[1].Value))
                    	content = (content.Substring(0, m.Index) + stringAfter);
                    else
                    {
                        int startPos = content.Substring(0, m.Index).LastIndexOf("<div");
                        Match closingDiv = Regex.Match(stringAfter, "</div>");
                        content = (content.Substring(0, startPos) + stringAfter.Substring((closingDiv.Index + closingDiv.Length)));
                    }
                }
            }
            return content;
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();
            HtmlTextWriter tempWriter = new HtmlTextWriter(new StringWriter(sb));
            base.Render(tempWriter);
            tempWriter.Flush();
            tempWriter.Close();
            string page = MyCompany.Data.Localizer.Replace("Pages", Path.GetFileName(Request.PhysicalPath), sb.ToString());
            if (page.Contains("data-content-framework=\"bootstrap\""))
            	if (ApplicationServices.EnableCombinedCss)
                	page = Regex.Replace(page, "_cf=\"", "_cf=bootstrap\"");
                else
                	if (ApplicationServices.IsTouchClient)
                    	page = Regex.Replace(page, "(<link\\s+href=\"[.\\w\\/]+?touch\\-theme\\..+?\".+?/>)", (("<link href=\"" + ResolveClientUrl(("~/css/sys/bootstrap.css?" + ApplicationServices.Version))) 
                                        + "\" type=\"text/css\" rel=\"stylesheet\" />$1"));
                    else
                    	page = Regex.Replace(page, "\\/>\\s*<title>", (("/><link href=\"" + ResolveClientUrl(("~/css/sys/bootstrap.css?" + ApplicationServices.Version))) 
                                        + "\" type=\"text/css\" rel=\"stylesheet\" /><title>"));
            ApplicationServices.CompressOutput(Context, HideUnauthorizedDataViews(page));
        }
        
        protected virtual void ValidateUrlParameters()
        {
            bool success = true;
            string link = Page.Request["_link"];
            if (!(String.IsNullOrEmpty(link)))
            	try
                {
                    StringEncryptor enc = new StringEncryptor();
                    link = enc.Decrypt(link.Replace(" ", "+").Split(',')[0]);
                    if (!(link.Contains('?')))
                    	link = ('?' + link);
                    string[] permalink = link.Split('?');
                    ClientScript.RegisterClientScriptBlock(GetType(), "CommandLine", String.Format("var __dacl=\'{0}?{1}\';", permalink[0], BusinessRules.JavaScriptString(permalink[1])), true);
                }
                catch (Exception )
                {
                    success = false;
                }
            if (!(success))
            {
                Response.StatusCode = 403;
                Response.End();
            }
        }
    }
    
    public partial class ControlBase : ControlBaseCore
    {
    }
    
    public class ControlBaseCore : System.Web.UI.UserControl
    {
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();
            HtmlTextWriter tempWriter = new HtmlTextWriter(new StringWriter(sb));
            base.Render(tempWriter);
            tempWriter.Flush();
            tempWriter.Close();
            writer.Write(MyCompany.Data.Localizer.Replace("Pages", Path.GetFileName(Request.PhysicalPath), sb.ToString()));
        }
        
        public static System.Web.UI.Control LoadPageControl(System.Web.UI.Control placeholder, string pageName, bool developmentMode)
        {
            try
            {
                System.Web.UI.Page page = placeholder.Page;
                string basePath = "~";
                if (!(developmentMode))
                	basePath = "~/DesktopModules/MyCompany";
                string controlPath = String.Format("{0}/Pages/{1}.ascx", basePath, pageName);
                System.Web.UI.Control c = page.LoadControl(controlPath);
                if (c != null)
                {
                    placeholder.Controls.Clear();
                    placeholder.Controls.Add(new LiteralControl("<table style=\"width:100%\" id=\"PageBody\" class=\"Hosted\"><tr><td valign=\"top\" id=\"P" +
                                "ageContent\">"));
                    placeholder.Controls.Add(c);
                    placeholder.Controls.Add(new LiteralControl("</td></tr></table>"));
                    return c;
                }
            }
            catch (Exception )
            {
            }
            return null;
        }
    }
}
