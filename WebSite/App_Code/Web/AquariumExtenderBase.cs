using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MyCompany.Data;
using MyCompany.Services;

namespace MyCompany.Web
{
	public class AquariumFieldEditorAttribute : Attribute
    {
    }
    
    public class AquariumExtenderBase : ExtenderControl
    {
        
        private string _clientComponentName;
        
        public static string DefaultServicePath = "~/Services/DataControllerService.asmx";
        
        public static string AppServicePath = "~/appservices";
        
        private string _servicePath;
        
        private SortedDictionary<string, object> _properties;
        
        private static bool _enableCombinedScript;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _ignoreCombinedScript;
        
        private static bool _enableMinifiedScript = true;
        
        public AquariumExtenderBase(string clientComponentName)
        {
            this._clientComponentName = clientComponentName;
        }
        
        [System.ComponentModel.Description("A path to a data controller web service.")]
        [System.ComponentModel.DefaultValue("~/Services/DataControllerService.asmx")]
        public virtual string ServicePath
        {
            get
            {
                if (String.IsNullOrEmpty(_servicePath))
                	return AquariumExtenderBase.DefaultServicePath;
                return _servicePath;
            }
            set
            {
                _servicePath = value;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public SortedDictionary<string, object> Properties
        {
            get
            {
                if (_properties == null)
                	_properties = new SortedDictionary<string, object>();
                return _properties;
            }
        }
        
        public static bool EnableCombinedScript
        {
            get
            {
                return _enableCombinedScript;
            }
            set
            {
                _enableCombinedScript = value;
            }
        }
        
        public bool IgnoreCombinedScript
        {
            get
            {
                return this._ignoreCombinedScript;
            }
            set
            {
                this._ignoreCombinedScript = value;
            }
        }
        
        public static bool EnableMinifiedScript
        {
            get
            {
                return _enableMinifiedScript;
            }
            set
            {
                _enableMinifiedScript = value;
            }
        }
        
        public static string CombinedScriptName
        {
            get
            {
                string lang = CultureInfo.CurrentUICulture.IetfLanguageTag.ToLower();
                string scriptMode = String.Empty;
                scriptMode = "?_touch";
                return String.Format("~/appservices/combined-{0}.{1}.js{2}", ApplicationServices.Version, lang, scriptMode);
            }
        }
        
        protected virtual bool RequiresMembershipScripts
        {
            get
            {
                return false;
            }
        }
        
        protected override System.Collections.Generic.IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
        {
            if (Site != null)
            	return null;
            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            {
                bool requireRegistration = false;
                Control c = this;
                while (!(requireRegistration) && ((c != null) && !((c is HtmlForm))))
                {
                    if (c is UpdatePanel)
                    	requireRegistration = true;
                    c = c.Parent;
                }
                if (!(requireRegistration))
                	return null;
            }
            ScriptBehaviorDescriptor descriptor = new ScriptBehaviorDescriptor(_clientComponentName, targetControl.ClientID);
            descriptor.AddProperty("id", this.ClientID);
            string baseUrl = ResolveClientUrl("~");
            if (baseUrl == "~")
            	baseUrl = String.Empty;
            bool isTouchUI = ApplicationServices.IsTouchClient;
            if (!(isTouchUI))
            {
                descriptor.AddProperty("baseUrl", baseUrl);
                descriptor.AddProperty("servicePath", ResolveClientUrl(ServicePath));
            }
            ConfigureDescriptor(descriptor);
            return new ScriptBehaviorDescriptor[] {
                    descriptor};
        }
        
        protected virtual void ConfigureDescriptor(ScriptBehaviorDescriptor descriptor)
        {
        }
        
        public static ScriptReference CreateScriptReference(string p)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            List<string> scripts = ((List<string>)(HttpRuntime.Cache["AllApplicationScripts"]));
            if (scripts == null)
            {
                scripts = new List<string>();
                string[] files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/js"), "*.js", SearchOption.AllDirectories);
                foreach (string script in files)
                {
                    Match m = Regex.Match(Path.GetFileName(script), "^(.+?)\\.(\\w\\w(\\-\\w+)*)\\.js$");
                    if (m.Success)
                    	scripts.Add(m.Value);
                }
                HttpRuntime.Cache["AllApplicationScripts"] = scripts;
            }
            if (scripts.Count > 0)
            {
                Match name = Regex.Match(p, "^(?\'Path\'.+\\/)(?\'Name\'.+?)\\.js$");
                if (name.Success)
                {
                    string test = String.Format("{0}.{1}.js", name.Groups["Name"].Value, culture.Name);
                    bool success = scripts.Contains(test);
                    if (!(success))
                    {
                        test = String.Format("{0}.{1}.js", name.Groups["Name"].Value, culture.Name.Substring(0, 2));
                        success = scripts.Contains(test);
                    }
                    if (success)
                    	p = (name.Groups["Path"].Value + test);
                }
            }
            p = (p + String.Format("?{0}", ApplicationServices.Version));
            return new ScriptReference(p);
        }
        
        protected override System.Collections.Generic.IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (Site != null)
            	return null;
            if ((Page != null) && ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            	return null;
            List<ScriptReference> scripts = new List<ScriptReference>();
            if (EnableCombinedScript && !(IgnoreCombinedScript))
            {
                ScriptReference combinedScript = new ScriptReference(CombinedScriptName);
                combinedScript.ResourceUICultures = null;
                scripts.Add(combinedScript);
                return scripts;
            }
            string fileType = ".min.js";
            if (!(EnableMinifiedScript))
            	fileType = ".js";
            CultureInfo ci = CultureInfo.CurrentUICulture;
            if (!((ci.Name == "en-US")))
            	scripts.Add(CreateScriptReference(String.Format("~/js/sys/culture/{0}.js", ci.Name)));
            scripts.Add(CreateScriptReference(("~/js/sys/jquery-2.2.4" + fileType)));
            scripts.Add(CreateScriptReference("~/js/sys/bootstrap.min.js"));
            scripts.Add(CreateScriptReference(String.Format("~/js/sys/jquery.mobile-{0}{1}", ApplicationServices.JqmVersion, fileType)));
            scripts.Add(CreateScriptReference("~/js/sys/MicrosoftAjax.min.js"));
            scripts.Add(CreateScriptReference("~/js/sys/MicrosoftAjaxWebForms.min.js"));
            scripts.Add(CreateScriptReference(("~/js/daf/daf-resources" + fileType)));
            scripts.Add(CreateScriptReference(("~/js/daf/daf" + fileType)));
            scripts.Add(CreateScriptReference(("~/js/daf/daf-odp" + fileType)));
            if (EnableCombinedScript)
            	scripts.Add(CreateScriptReference(("~/js/daf/daf-membership" + fileType)));
            ConfigureScripts(scripts);
            scripts.Add(CreateScriptReference(("~/js/daf/touch" + fileType)));
            scripts.Add(CreateScriptReference(("~/js/daf/touch-edit" + fileType)));
            scripts.Add(CreateScriptReference(("~/js/daf/touch-charts" + fileType)));
            scripts.Add(CreateScriptReference(("~/js/sys/unicode" + fileType)));
            if (!(String.IsNullOrEmpty(ApplicationServices.Current.AddScripts())))
            	scripts.Add(CreateScriptReference("~/js/daf/add.min.js"));
            if (Context.Request.Url.Host.Equals("localhost") && File.Exists(Context.Server.MapPath("~/js/codeontime.designer.js")))
            	scripts.Add(CreateScriptReference("~/js/codeontime.designer.js"));
            ApplicationServices.Current.ConfigureScripts(scripts);
            return scripts;
        }
        
        protected virtual void ConfigureScripts(List<ScriptReference> scripts)
        {
            if (RequiresMembershipScripts && !(EnableCombinedScript))
            	if (EnableMinifiedScript)
                {
                    scripts.Add(CreateScriptReference("~/js/daf/daf-resources.min.js"));
                    scripts.Add(CreateScriptReference("~/js/daf/daf-membership.min.js"));
                }
                else
                {
                    scripts.Add(CreateScriptReference("~/js/daf/daf-resources.js"));
                    scripts.Add(CreateScriptReference("~/js/daf/daf-membership.js"));
                }
        }
        
        protected override void OnLoad(EventArgs e)
        {
            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            	return;
            base.OnLoad(e);
            if (Site != null)
            	return;
            RegisterFrameworkSettings(Page);
        }
        
        public static void RegisterFrameworkSettings(Page p)
        {
            if (!(p.ClientScript.IsStartupScriptRegistered(typeof(AquariumExtenderBase), "TargetFramework")))
            {
                string designerPort = ApplicationServices.DesignerPort;
                if (!(String.IsNullOrEmpty(designerPort)) && !(Controller.UserIsInRole(ApplicationServices.SiteContentDevelopers)))
                	designerPort = String.Empty;
                p.ClientScript.RegisterStartupScript(typeof(AquariumExtenderBase), "TargetFramework", String.Format("var __targetFramework=\"4.5\",__tf=4.0,__servicePath=\"{0}\",__baseUrl=\"{1}\",__design" +
                            "erPort=\"{2}\";", p.ResolveClientUrl(AquariumExtenderBase.DefaultServicePath), p.ResolveClientUrl("~"), designerPort), true);
                p.ClientScript.RegisterStartupScript(typeof(AquariumExtenderBase), "TouchUI", (("var __settings=" + ApplicationServices.Create().UserSettings(p).ToString(Newtonsoft.Json.Formatting.None)) 
                                + ";"), true);
            }
        }
        
        public static List<ScriptReference> StandardScripts()
        {
            return StandardScripts(false);
        }
        
        public static List<ScriptReference> StandardScripts(bool ignoreCombinedScriptFlag)
        {
            AquariumExtenderBase extender = new AquariumExtenderBase(null);
            extender.IgnoreCombinedScript = ignoreCombinedScriptFlag;
            return new List<ScriptReference>(extender.GetScriptReferences());
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
    }
}
