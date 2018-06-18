using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using MyCompany.Data;
using MyCompany.Services;
using MyCompany.Web;

namespace MyCompany.Handlers
{
	public partial class Site : SiteBase
    {
        
        static Site()
        {
            CompilationSection compilation = ((CompilationSection)(WebConfigurationManager.GetSection("system.web/compilation")));
            bool releaseMode = !(compilation.Debug);
            AquariumExtenderBase.EnableMinifiedScript = releaseMode;
            AquariumExtenderBase.EnableCombinedScript = releaseMode;
            ApplicationServices.EnableMinifiedCss = releaseMode;
            ApplicationServices.EnableCombinedCss = releaseMode;
        }
    }
    
    public class SiteBase : MyCompany.Web.PageBase
    {
        
        private bool _isTouchUI;
        
        private AttributeDictionary _bodyAttributes;
        
        private LiteralControl _bodyTag;
        
        private LiteralContainer _pageHeaderContent;
        
        private LiteralContainer _pageTitleContent;
        
        private LiteralContainer _headContent;
        
        private LiteralContainer _pageContent;
        
        private LiteralContainer _pageFooterContent;
        
        private LiteralContainer _pageSideBarContent;
        
        public static string Copyright
        {
            get
            {
                return "&copy; 2017 MyCompany. ^Copyright^All rights reserved.^Copyright^";
            }
        }
        
        public override string Device
        {
            get
            {
                return _bodyAttributes["data-device"];
            }
        }
        
        public string ResolveAppUrl(string html)
        {
            string appPath = Request.ApplicationPath;
            if (!(appPath.EndsWith("/")))
            	appPath = (appPath + "/");
            return html.Replace("=\"~/", ("=\"" + appPath));
        }
        
        protected override void OnInit(EventArgs e)
        {
            if (Request.Path.StartsWith((ResolveUrl(AquariumExtenderBase.DefaultServicePath) + "/"), StringComparison.CurrentCultureIgnoreCase) || Request.Path.StartsWith((ResolveUrl(AquariumExtenderBase.AppServicePath) + "/"), StringComparison.CurrentCultureIgnoreCase))
            	ApplicationServices.HandleServiceRequest(Context);
            if (Request.Params["_page"] == "_blank")
            	return;
            string link = Request.Params["_link"];
            if (!(String.IsNullOrEmpty(link)))
            {
                StringEncryptor enc = new StringEncryptor();
                string[] permalink = enc.Decrypt(link.Split(',')[0]).Split('?');
                if (permalink.Length == 2)
                	Page.ClientScript.RegisterStartupScript(GetType(), "Redirect", String.Format("window.location.replace(\'{0}?_link={1}\');", permalink[0], HttpUtility.UrlEncode(link)), true);
            }
            else
            {
                string requestUrl = Request.RawUrl;
                if ((requestUrl.Length > 1) && requestUrl.EndsWith("/"))
                	requestUrl = requestUrl.Substring(0, (requestUrl.Length - 1));
                if (Request.ApplicationPath.Equals(requestUrl, StringComparison.CurrentCultureIgnoreCase))
                {
                    string homePageUrl = ApplicationServices.HomePageUrl;
                    if (!(Request.ApplicationPath.Equals(homePageUrl)))
                    	Response.Redirect(homePageUrl);
                }
            }
            SortedDictionary<string, string> contentInfo = ApplicationServices.LoadContent();
            InitializeSiteMaster();
            string s = null;
            if (!(contentInfo.TryGetValue("PageTitle", out s)))
            	s = ApplicationServices.Current.Name;
            this.Title = s;
            if (_pageTitleContent != null)
            	if (_isTouchUI)
                	_pageTitleContent.Text = String.Empty;
                else
                	_pageTitleContent.Text = s;
            HtmlMeta appName = new HtmlMeta();
            appName.Name = "application-name";
            appName.Content = ApplicationServices.Current.Name;
            Header.Controls.Add(appName);
            if (contentInfo.TryGetValue("Head", out s) && (_headContent != null))
            	_headContent.Text = s;
            if (contentInfo.TryGetValue("PageContent", out s) && (_pageContent != null))
            {
                if (_isTouchUI)
                	s = String.Format("<div id=\"PageContent\" style=\"display:none\">{0}</div>", s);
                Match userControl = Regex.Match(s, "<div\\s+data-user-control\\s*=s*\"([\\s\\S]+?)\"\\s*>\\s*</div>");
                if (userControl.Success)
                {
                    int startPos = 0;
                    while (userControl.Success)
                    {
                        _pageContent.Controls.Add(new LiteralControl(s.Substring(startPos, (userControl.Index - startPos))));
                        startPos = (userControl.Index + userControl.Length);
                        string controlFileName = userControl.Groups[1].Value;
                        string controlExtension = Path.GetExtension(controlFileName);
                        string siteControlText = null;
                        if (!(controlFileName.StartsWith("~")))
                        	controlFileName = (controlFileName + "~");
                        if (String.IsNullOrEmpty(controlExtension))
                        {
                            string testFileName = (controlFileName + ".ascx");
                            if (File.Exists(Server.MapPath(testFileName)))
                            {
                                controlFileName = testFileName;
                                controlExtension = ".ascx";
                            }
                            else
                            {
                                if (ApplicationServices.IsSiteContentEnabled)
                                {
                                    string relativeControlPath = controlFileName.Substring(1);
                                    if (relativeControlPath.StartsWith("/"))
                                    	relativeControlPath = relativeControlPath.Substring(1);
                                    siteControlText = ApplicationServices.Current.ReadSiteContentString(("sys/" + relativeControlPath));
                                }
                                if (siteControlText == null)
                                {
                                    testFileName = (controlFileName + ".html");
                                    if (File.Exists(Server.MapPath(testFileName)))
                                    {
                                        controlFileName = testFileName;
                                        controlExtension = ".html";
                                    }
                                }
                            }
                        }
                        try
                        {
                            if (controlExtension == ".ascx")
                            	_pageContent.Controls.Add(LoadControl(controlFileName));
                            else
                            {
                                string controlText = siteControlText;
                                if (controlText == null)
                                	controlText = File.ReadAllText(Server.MapPath(controlFileName));
                                Match bodyMatch = Regex.Match(controlText, "<body[\\s\\S]*?>([\\s\\S]+?)</body>");
                                if (bodyMatch.Success)
                                	controlText = bodyMatch.Groups[1].Value;
                                controlText = Localizer.Replace("Controls", Path.GetFileName(Server.MapPath(controlFileName)), controlText);
                                _pageContent.Controls.Add(new LiteralControl(controlText));
                            }
                        }
                        catch (Exception ex)
                        {
                            _pageContent.Controls.Add(new LiteralControl(String.Format("Error loading \'{0}\': {1}", controlFileName, ex.Message)));
                        }
                        userControl = userControl.NextMatch();
                    }
                    if (startPos < s.Length)
                    	_pageContent.Controls.Add(new LiteralControl(s.Substring(startPos)));
                }
                else
                	_pageContent.Text = s;
            }
            else
            	if (_isTouchUI)
                {
                    _pageContent.Text = "<div id=\"PageContent\" style=\"display:none\"><div data-app-role=\"page\">404 Not Foun" +
                        "d</div></div>";
                    this.Title = "Golf Club New";
                }
                else
                	_pageContent.Text = "404 Not Found";
            if (_isTouchUI)
            {
                if (_pageFooterContent != null)
                	_pageFooterContent.Text = (("<footer style=\"display:none\"><small>" + Copyright) 
                                + "</small></footer>");
            }
            else
            	if (contentInfo.TryGetValue("About", out s))
                {
                    if (_pageSideBarContent != null)
                    	_pageSideBarContent.Text = String.Format("<div class=\"TaskBox About\"><div class=\"Inner\"><div class=\"Header\">About</div><div" +
                                " class=\"Value\">{0}</div></div></div>", s);
                }
            string bodyAttributes = null;
            if (contentInfo.TryGetValue("BodyAttributes", out bodyAttributes))
            	_bodyAttributes.Parse(bodyAttributes);
            if (!(ApplicationServices.UserIsAuthorizedToAccessResource(HttpContext.Current.Request.Path, _bodyAttributes["data-authorize-roles"])))
            {
                string requestPath = Request.Path.Substring(1);
                if (!((WorkflowRegister.IsEnabled || WorkflowRegister.Allows(requestPath))))
                	ApplicationServices.Current.RedirectToLoginPage();
            }
            _bodyAttributes.Remove("data-authorize-roles");
            if (!(_isTouchUI))
            {
                string classAttr = _bodyAttributes["class"];
                if (String.IsNullOrEmpty(classAttr))
                	classAttr = String.Empty;
                if (!(classAttr.Contains("Wide")))
                	classAttr = (classAttr + " Standard");
                classAttr = ((classAttr + " ") 
                            + (Regex.Replace(Request.Path.ToLower(), "\\W", "_").Substring(1) + "_html"));
                _bodyAttributes["class"] = classAttr.Trim();
            }
            _bodyTag.Text = String.Format("\r\n<body{0}>\r\n", _bodyAttributes.ToString());
            base.OnInit(e);
        }
        
        protected virtual void InitializeSiteMaster()
        {
            _isTouchUI = ApplicationServices.IsTouchClient;
            string html = String.Empty;
            string siteMasterPath = "~/site.desktop.html";
            if (_isTouchUI)
            	siteMasterPath = "~/site.touch.html";
            siteMasterPath = Server.MapPath(siteMasterPath);
            if (!(File.Exists(siteMasterPath)))
            	siteMasterPath = Server.MapPath("~/site.html");
            if (File.Exists(siteMasterPath))
            	html = File.ReadAllText(siteMasterPath);
            else
            	throw new Exception("File site.html has not been found.");
            Match htmlMatch = Regex.Match(html, "<html(?\'HtmlAttr\'[\\S\\s]*?)>\\s*<head(?\'HeadAttr\'[\\S\\s]*?)>\\s*(?\'Head\'[\\S\\s]*?)\\s*<" +
                    "/head>\\s*<body(?\'BodyAttr\'[\\S\\s]*?)>\\s*(?\'Body\'[\\S\\s]*?)\\s*</body>\\s*</html>\\s*");
            if (!(htmlMatch.Success))
            	throw new Exception("File site.html must contain \'head\' and \'body\' elements.");
            // instructions
            Controls.Add(new LiteralControl(html.Substring(0, htmlMatch.Index)));
            // html
            Controls.Add(new LiteralControl(String.Format("<html{0} xml:lang={1} lang=\"{1}\">\r\n", htmlMatch.Groups["HtmlAttr"].Value, CultureInfo.CurrentUICulture.IetfLanguageTag)));
            // head
            Controls.Add(new HtmlHead());
            if (_isTouchUI)
            	Header.Controls.Add(new LiteralControl("<meta charset=\"utf-8\">\r\n"));
            else
            	Header.Controls.Add(new LiteralControl("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n"));
            string headHtml = Regex.Replace(htmlMatch.Groups["Head"].Value, "\\s*<title([\\s\\S+]*?title>)\\s*", String.Empty);
            Header.Controls.Add(new LiteralControl(headHtml));
            _headContent = new LiteralContainer();
            Header.Controls.Add(_headContent);
            // body
            _bodyTag = new LiteralControl();
            _bodyAttributes = new AttributeDictionary(htmlMatch.Groups["BodyAttr"].Value);
            Controls.Add(_bodyTag);
            string themePath = Server.MapPath("~/App_Themes/MyCompany");
            if (Directory.Exists(themePath))
            	foreach (string stylesheetFileName in Directory.GetFiles(themePath, "*.css"))
                {
                    string fileName = Path.GetFileName(stylesheetFileName);
                    if (!(fileName.Equals("_Theme_Aquarium.css")))
                    {
                        HtmlLink link = new HtmlLink();
                        link.Href = ("~/App_Themes/MyCompany/" + fileName);
                        link.Attributes["type"] = "text/css";
                        link.Attributes["rel"] = "stylesheet";
                        Header.Controls.Add(link);
                    }
                }
            // form
            Controls.Add(new HtmlForm());
            Form.ID = "aspnetForm";
            // ScriptManager
            ScriptManager sm = new ScriptManager();
            sm.ID = "sm";
            sm.AjaxFrameworkMode = AjaxFrameworkMode.Disabled;
            if (AquariumExtenderBase.EnableCombinedScript)
            	sm.EnableScriptLocalization = false;
            sm.ScriptMode = ScriptMode.Release;
            Form.Controls.Add(sm);
            // SiteMapDataSource
            SiteMapDataSource siteMapDataSource1 = new SiteMapDataSource();
            siteMapDataSource1.ID = "SiteMapDataSource1";
            siteMapDataSource1.ShowStartingNode = false;
            Form.Controls.Add(siteMapDataSource1);
            // parse and initialize placeholders
            string body = htmlMatch.Groups["Body"].Value;
            Match placeholderMatch = Regex.Match(body, "<div\\s+data-role\\s*=\\s*\"placeholder\"(?\'Attributes\'[\\s\\S]+?)>\\s*(?\'DefaultContent\'" +
                    "[\\s\\S]*?)\\s*</div>");
            int startPos = 0;
            while (placeholderMatch.Success)
            {
                AttributeDictionary attributes = new AttributeDictionary(placeholderMatch.Groups["Attributes"].Value);
                // create placeholder content
                Form.Controls.Add(new LiteralControl(body.Substring(startPos, (placeholderMatch.Index - startPos))));
                string placeholder = attributes["data-placeholder"];
                string defaultContent = placeholderMatch.Groups["DefaultContent"].Value;
                if (!(CreatePlaceholder(Form.Controls, placeholder, defaultContent, attributes)))
                {
                    LiteralContainer placeholderControl = new LiteralContainer();
                    placeholderControl.Text = defaultContent;
                    Form.Controls.Add(placeholderControl);
                    if (placeholder == "page-header")
                    	_pageHeaderContent = placeholderControl;
                    if (placeholder == "page-title")
                    	_pageTitleContent = placeholderControl;
                    if (placeholder == "page-side-bar")
                    	_pageSideBarContent = placeholderControl;
                    if (placeholder == "page-content")
                    	_pageContent = placeholderControl;
                    if (placeholder == "page-footer")
                    	_pageFooterContent = placeholderControl;
                }
                startPos = (placeholderMatch.Index + placeholderMatch.Length);
                placeholderMatch = placeholderMatch.NextMatch();
            }
            if (startPos < body.Length)
            	Form.Controls.Add(new LiteralControl(body.Substring(startPos)));
            // end body
            Controls.Add(new LiteralControl("\r\n</body>\r\n"));
            // end html
            Controls.Add(new LiteralControl("\r\n</html>\r\n"));
        }
        
        protected virtual bool CreatePlaceholder(ControlCollection container, string placeholder, string defaultContent, AttributeDictionary attributes)
        {
            if (placeholder == "membership-bar")
            {
                MembershipBar mb = new MembershipBar();
                mb.ID = "mb";
                if (attributes["data-display-remember-me"] == "false")
                	mb.DisplayRememberMe = false;
                if (attributes["data-remember-me-set"] == "true")
                	mb.RememberMeSet = true;
                if (attributes["data-display-password-recovery"] == "false")
                	mb.DisplayPasswordRecovery = false;
                if (attributes["data-display-sign-up"] == "false")
                	mb.DisplaySignUp = false;
                if (attributes["data-display-my-account"] == "false")
                	mb.DisplayMyAccount = false;
                if (attributes["data-display-help"] == "false")
                	mb.DisplayHelp = false;
                if (attributes["data-display-login"] == "false")
                	mb.DisplayLogin = false;
                if (!(String.IsNullOrEmpty(attributes["data-idle-user-timeout"])))
                	mb.IdleUserTimeout = Convert.ToInt32(attributes["data-idle-user-timeout"]);
                if (attributes["data-enable-history"] == "true")
                	mb.EnableHistory = true;
                if (attributes["data-enable-permalinks"] == "true")
                	mb.EnablePermalinks = true;
                container.Add(mb);
                return true;
            }
            if (placeholder == "menu-bar")
            {
                HtmlGenericControl menuDiv = new HtmlGenericControl();
                menuDiv.TagName = "div";
                menuDiv.ID = "PageMenuBar";
                menuDiv.Attributes["class"] = "PageMenuBar";
                container.Add(menuDiv);
                MenuExtender menu = new MenuExtender();
                menu.ID = "Menu1";
                menu.DataSourceID = "SiteMapDataSource1";
                menu.TargetControlID = menuDiv.ID;
                menu.HoverStyle = ((MenuHoverStyle)(TypeDescriptor.GetConverter(typeof(MenuHoverStyle)).ConvertFromString(attributes.ValueOf("data-hover-style", "Auto"))));
                menu.PopupPosition = ((MenuPopupPosition)(TypeDescriptor.GetConverter(typeof(MenuPopupPosition)).ConvertFromString(attributes.ValueOf("data-popup-position", "Left"))));
                menu.ShowSiteActions = (attributes["data-show-site-actions"] == "true");
                menu.PresentationStyle = ((MenuPresentationStyle)(TypeDescriptor.GetConverter(typeof(MenuPresentationStyle)).ConvertFromString(attributes.ValueOf("data-presentation-style", "MultiLevel"))));
                container.Add(menu);
                return true;
            }
            if (placeholder == "site-map-path")
            {
                SiteMapPath siteMapPath1 = new SiteMapPath();
                siteMapPath1.ID = "SiteMapPath1";
                siteMapPath1.CssClass = "SiteMapPath";
                siteMapPath1.PathSeparatorStyle.CssClass = "PathSeparator";
                siteMapPath1.CurrentNodeStyle.CssClass = "CurrentNode";
                siteMapPath1.NodeStyle.CssClass = "Node";
                siteMapPath1.RootNodeStyle.CssClass = "RootNode";
                container.Add(siteMapPath1);
                return true;
            }
            return false;
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            ApplicationServices.RegisterCssLinks(this);
            if (_isTouchUI)
            {
                // hide top-level literals
                foreach (Control c in Form.Controls)
                	if (c is LiteralControl)
                    	c.Visible = false;
                // look deep in children for ASP.NET controls
                HideAspNetControls(Form.Controls);
            }
            base.OnPreRender(e);
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            // create page content
            StringBuilder sb = new StringBuilder();
            HtmlTextWriter w = new HtmlTextWriter(new StringWriter(sb));
            base.Render(w);
            w.Flush();
            w.Close();
            string content = sb.ToString();
            if (_isTouchUI)
            {
                // perform cleanup for super lightweight output
                content = Regex.Replace(content, "(<body([\\s\\S]*?)>\\s*)<form\\s+([\\s\\S]*?)</div>\\s*", "$1");
                content = Regex.Replace(content, "\\s*</form>\\s*(</body>)", "\r\n$1");
                content = Regex.Replace(content, "<script(?\'Attributes\'[\\s\\S]*?)>(?\'Script\'[\\s\\S]*?)</script>\\s*", DoValidateScript);
                content = Regex.Replace(content, "<title>\\s*([\\s\\S]+?)\\s*</title>", "<title>$1</title>");
                content = Regex.Replace(content, "<div>\\s*<input([\\s\\S]+?)VIEWSTATEGENERATOR([\\s\\S]+?)</div>", String.Empty);
                content = Regex.Replace(content, "<div.+?></div>.+?(<div.+?class=\"PageMenuBar\"></div>)\\s*", String.Empty);
                content = Regex.Replace(content, "\\$get\\(\".*?mb_d\"\\)", "null");
                content = Regex.Replace(content, "\\s*(<footer[\\s\\S]+?</small></footer>)\\s*", "$1");
                content = Regex.Replace(content, "\\s*type=\"text/javascript\"\\s*", " ");
            }
            content = Regex.Replace(content, "(>\\s+)//<\\!\\[CDATA\\[\\s*", "$1");
            content = Regex.Replace(content, "\\s*//\\]\\]>\\s*</script>", "\r\n</script>");
            content = Regex.Replace(content, "<div\\s+data-role\\s*=\"placeholder\"\\s+(?\'Attributes\'[\\s\\S]+?)>(?\'DefaultContent\'[\\s" +
                    "\\S]*?)</div>", DoReplacePlaceholder);
            content = ResolveAppUrl(content);
            ApplicationServices.CompressOutput(Context, content);
        }
        
        private string DoReplacePlaceholder(Match m)
        {
            AttributeDictionary attributes = new AttributeDictionary(m.Groups["Attributes"].Value);
            string defaultContent = m.Groups["DefaultContent"].Value;
            string replacement = ReplaceStaticPlaceholder(attributes["data-placeholder"], attributes, defaultContent);
            if (replacement == null)
            	return m.Value;
            else
            	return replacement;
        }
        
        public virtual string ReplaceStaticPlaceholder(string name, AttributeDictionary attributes, string defaultContent)
        {
            return null;
        }
        
        private void HideAspNetControls(ControlCollection controls)
        {
            int i = 0;
            while (i < controls.Count)
            {
                Control c = controls[i];
                if ((c is SiteMapPath) || ((c is Image) || (c is TreeView)))
                	controls.Remove(c);
                else
                {
                    HideAspNetControls(c.Controls);
                    i++;
                }
            }
        }
        
        private string DoValidateScript(Match m)
        {
            string script = m.Groups["Script"].Value;
            if (script.Contains("aspnetForm"))
            	return String.Empty;
            Match srcMatch = Regex.Match(m.Groups["Attributes"].Value, "src=\"(.+?)\"");
            if (srcMatch.Success)
            {
                string src = srcMatch.Groups[1].Value;
                if (src.Contains(".axd?"))
                {
                    try
                    {
                        WebClient client = new WebClient();
                        script = client.DownloadString(String.Format("http://{0}/{1}", Request.Url.Authority, src));
                    }
                    catch (Exception )
                    {
                        return script;
                    }
                    if (script.Contains("WebForm_PostBack"))
                    	return String.Empty;
                }
            }
            script = m.Value.Replace("WebForm_InitCallback();", String.Empty);
            return script;
        }
    }
    
    public class LiteralContainer : Panel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _text;
        
        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
            }
        }
        
        protected override void Render(HtmlTextWriter output)
        {
            if (Controls.Count > 0)
            	foreach (Control c in Controls)
                	c.RenderControl(output);
            else
            	output.Write(Text);
        }
    }
    
    public class AttributeDictionary : SortedDictionary<string, string>
    {
        
        public AttributeDictionary(string attributes)
        {
            Parse(attributes);
        }
        
        public new string this[string name]
        {
            get
            {
                return this.ValueOf(name, null);
            }
            set
            {
                if (value == null)
                	Remove(name);
                else
                	base[name] = value;
            }
        }
        
        public string ValueOf(string name, string defaultValue)
        {
            string v = null;
            if (!(TryGetValue(name, out v)))
            	v = defaultValue;
            return v;
        }
        
        public void Parse(string attributes)
        {
            Match attributeMatch = Regex.Match(attributes, "\\s*(?\'Name\'[\\w\\-]+?)\\s*=\\s*\"(?\'Value\'.+?)\"");
            while (attributeMatch.Success)
            {
                this[attributeMatch.Groups["Name"].Value] = attributeMatch.Groups["Value"].Value;
                attributeMatch = attributeMatch.NextMatch();
            }
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string name in Keys)
            	sb.AppendFormat(" {0}=\"{1}\"", name, this[name]);
            return sb.ToString();
        }
    }
}
