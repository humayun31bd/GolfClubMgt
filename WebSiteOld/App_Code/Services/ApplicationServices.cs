using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Configuration;
using System.IO.Compression;
using System.Xml.XPath;
using System.Web.Routing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyCompany.Data;
using MyCompany.Handlers;
using MyCompany.Web;

namespace MyCompany.Services
{
	public abstract class ServiceRequestHandler
    {
        
        public virtual string[] AllowedMethods
        {
            get
            {
                return new string[] {
                        "POST"};
            }
        }
        
        public virtual bool RequiresAuthentication
        {
            get
            {
                return false;
            }
        }
        
        public abstract object HandleRequest(DataControllerService service, JObject args);
        
        public virtual object HandleException(JObject args, Exception ex)
        {
            return ApplicationServices.Current.HandleException(args, ex);
        }
        
        public static void Redirect(string redirectUrl)
        {
            throw new ServiceRequestRedirectException(redirectUrl);
        }
    }
    
    public class GetPageServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            PageRequest r = args["request"].ToObject<PageRequest>();
            return service.GetPage(ControllerUtilities.ValidateName(((string)(args["controller"]))), ControllerUtilities.ValidateName(((string)(args["view"]))), r);
        }
    }
    
    public class GetControllerListServiceRequestHandler : ServiceRequestHandler
    {
        
        public override bool RequiresAuthentication
        {
            get
            {
                return true;
            }
        }
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            StringBuilder jsonArray = new StringBuilder("[");
            string[] list = args["controllers"].ToObject<string[]>();
            bool first = true;
            foreach (string name in list)
            {
                if (first)
                	first = false;
                else
                	jsonArray.Append(",");
                ControllerConfiguration config = DataControllerBase.CreateConfigurationInstance(GetType(), name);
                string json = config.ToJson();
                jsonArray.Append(json);
            }
            jsonArray.Append("]");
            return jsonArray.ToString();
        }
    }
    
    public class CommitServiceRequestHandler : ServiceRequestHandler
    {
        
        public override bool RequiresAuthentication
        {
            get
            {
                return true;
            }
        }
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            TransactionManager tm = new TransactionManager();
            return tm.Commit(((JArray)(args["log"])));
        }
    }
    
    public class GetPageListServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.GetPageList(args["requests"].ToObject<PageRequest[]>());
        }
    }
    
    public class GetListOfValuesServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            DistinctValueRequest r = args["request"].ToObject<DistinctValueRequest>();
            return service.GetListOfValues(ControllerUtilities.ValidateName(((string)(args["controller"]))), ControllerUtilities.ValidateName(((string)(args["view"]))), r);
        }
    }
    
    public class ExecuteServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            ActionArgs a = args["args"].ToObject<ActionArgs>();
            return service.Execute(ControllerUtilities.ValidateName(((string)(args["controller"]))), ControllerUtilities.ValidateName(((string)(args["view"]))), a);
        }
    }
    
    public class ExecuteAndGetPageServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            ExecuteViewPageArgs arg = args.ToObject<ExecuteViewPageArgs>();
            ActionArgs a = arg.Args;
            ActionResult result = service.Execute(a.Controller, a.View, a);
            if (result.Errors.Count > 0)
            {
                ViewPage vp = new ViewPage();
                vp.Errors = result.Errors;
                return vp;
            }
            else
            {
                PageRequest request = new PageRequest(0, arg.PageSize, String.Empty, null);
                request.Controller = a.Controller;
                request.View = a.View;
                request.LastCommandName = a.CommandName;
                request.LastCommandArgument = a.CommandArgument;
                request.RequiresMetaData = arg.Metadata;
                request.DoesNotRequireAggregates = !(arg.Aggregates);
                request.RequiresRowCount = arg.RowCount;
                request.SyncKey = GetPrimaryKey(result, a);
                return service.GetPage(a.Controller, a.View, request);
            }
        }
        
        private object[] GetPrimaryKey(ActionResult result, ActionArgs args)
        {
            ControllerConfiguration config = Controller.CreateConfigurationInstance(GetType(), args.Controller);
            SortedDictionary<string, FieldValue> pKeys = new SortedDictionary<string, FieldValue>();
            foreach (XPathNavigator nav in config.Select("/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']"))
            {
                foreach (FieldValue fv in result.Values)
                	if (fv.Name == nav.GetAttribute("name", String.Empty))
                    {
                        pKeys[fv.Name] = fv;
                        break;
                    }
                foreach (FieldValue fv in args.Values)
                	if (fv.Name == nav.GetAttribute("name", String.Empty))
                    {
                        pKeys[fv.Name] = fv;
                        break;
                    }
            }
            List<object> key = new List<object>();
            foreach (FieldValue fv in pKeys.Values)
            	key.Add(fv.Value);
            return key.ToArray();
        }
    }
    
    public class ExecuteListServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.ExecuteList(args["requests"].ToObject<ActionArgs[]>());
        }
    }
    
    public class GetCompletionListServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.GetCompletionList(((string)(args["prefixText"])), ((int)(args["count"])), ((string)(args["contextKey"])));
        }
    }
    
    public class LoginServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.Login(((string)(args["username"])), ((string)(args["password"])), ((bool)(args["createPersistentCookie"])));
        }
    }
    
    public class LogoutServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            service.Logout();
            return null;
        }
    }
    
    public class RolesServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.Roles();
        }
    }
    
    public class ThemesServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.Themes();
        }
    }
    
    public class GetSurveyServiceRequestHandler : ServiceRequestHandler
    {
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            return service.GetSurvey(((string)(args["name"])));
        }
    }
    
    public class DnnOAuthServiceRequestHandler : ServiceRequestHandler
    {
        
        public override string[] AllowedMethods
        {
            get
            {
                return new string[] {
                        "GET",
                        "POST"};
            }
        }
        
        public override object HandleRequest(DataControllerService service, JObject args)
        {
            DnnOAuthHandler handler = new DnnOAuthHandler();
            handler.ProcessRequest(HttpContext.Current);
            return null;
        }
        
        public override object HandleException(JObject args, Exception ex)
        {
            if (ex is ThreadAbortException)
            	throw ex;
            return base.HandleException(args, ex);
        }
    }
    
    public class ServiceRequestError
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _exceptionType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _message;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _stackTrace;
        
        public string ExceptionType
        {
            get
            {
                return this._exceptionType;
            }
            set
            {
                this._exceptionType = value;
            }
        }
        
        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }
        
        public string StackTrace
        {
            get
            {
                return this._stackTrace;
            }
            set
            {
                this._stackTrace = value;
            }
        }
    }
    
    public class ServiceRequestRedirectException : Exception
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _redirectUrl;
        
        public ServiceRequestRedirectException(string redirectUrl)
        {
            this.RedirectUrl = redirectUrl;
        }
        
        public virtual string RedirectUrl
        {
            get
            {
                return this._redirectUrl;
            }
            set
            {
                this._redirectUrl = value;
            }
        }
    }
    
    public partial class RequestValidationService : RequestValidationServiceBase
    {
    }
    
    public class RequestValidationServiceBase
    {
        
        public static Regex ValidRequestRegex = new Regex("<[^\\w<>]*(?:[^<>\"\'\\s]*:)?[^\\w<>]*(?:\\W*s\\W*c\\W*r\\W*i\\W*p\\W*t|\\W*f\\W*o\\W*r\\W*m|\\W*" +
                "s\\W*t\\W*y\\W*l\\W*e|\\W*s\\W*v\\W*g|\\W*m\\W*a\\W*r\\W*q\\W*u\\W*e\\W*e|(?:\\W*l\\W*i\\W*n\\W*k|" +
                "\\W*o\\W*b\\W*j\\W*e\\W*c\\W*t|\\W*e\\W*m\\W*b\\W*e\\W*d|\\W*a\\W*p\\W*p\\W*l\\W*e\\W*t|\\W*p\\W*a\\" +
                "W*r\\W*a\\W*m|\\W*i?\\W*f\\W*r\\W*a\\W*m\\W*e|\\W*b\\W*a\\W*s\\W*e|\\W*b\\W*o\\W*d\\W*y|\\W*m\\W*e" +
                "\\W*t\\W*a|\\W*i\\W*m\\W*a?\\W*g\\W*e?|\\W*v\\W*i\\W*d\\W*e\\W*o|\\W*a\\W*u\\W*d\\W*i\\W*o|\\W*b\\W" +
                "*i\\W*n\\W*d\\W*i\\W*n\\W*g\\W*s|\\W*s\\W*e\\W*t|\\W*i\\W*s\\W*i\\W*n\\W*d\\W*e\\W*x|\\W*a\\W*n\\W*" +
                "i\\W*m\\W*a\\W*t\\W*e)[^>\\w])|(?:<\\w[\\s\\S]*[\\s\\0\\/]|[\'\"])(?:formaction|style|backgro" +
                "und|src|lowsrc|ping|on(?:d(?:e(?:vice(?:(?:orienta|mo)tion|proximity|found|light" +
                ")|livery(?:success|error)|activate)|r(?:ag(?:e(?:n(?:ter|d)|xit)|(?:gestur|leav)" +
                "e|start|drop|over)?|op)|i(?:s(?:c(?:hargingtimechange|onnect(?:ing|ed))|abled)|a" +
                "ling)|ata(?:setc(?:omplete|hanged)|(?:availabl|chang)e|error)|urationchange|ownl" +
                "oading|blclick)|Moz(?:M(?:agnifyGesture(?:Update|Start)?|ouse(?:PixelScroll|Hitt" +
                "est))|S(?:wipeGesture(?:Update|Start|End)?|crolledAreaChanged)|(?:(?:Press)?TapG" +
                "estur|BeforeResiz)e|EdgeUI(?:C(?:omplet|ancel)|Start)ed|RotateGesture(?:Update|S" +
                "tart)?|A(?:udioAvailable|fterPaint))|c(?:o(?:m(?:p(?:osition(?:update|start|end)" +
                "|lete)|mand(?:update)?)|n(?:t(?:rolselect|extmenu)|nect(?:ing|ed))|py)|a(?:(?:ll" +
                "schang|ch)ed|nplay(?:through)?|rdstatechange)|h(?:(?:arging(?:time)?ch)?ange|eck" +
                "ing)|(?:fstate|ell)change|u(?:echange|t)|l(?:ick|ose))|m(?:o(?:z(?:pointerlock(?" +
                ":change|error)|(?:orientation|time)change|fullscreen(?:change|error)|network(?:d" +
                "own|up)load)|use(?:(?:lea|mo)ve|o(?:ver|ut)|enter|wheel|down|up)|ve(?:start|end)" +
                "?)|essage|ark)|s(?:t(?:a(?:t(?:uschanged|echange)|lled|rt)|k(?:sessione|comma)nd" +
                "|op)|e(?:ek(?:complete|ing|ed)|(?:lec(?:tstar)?)?t|n(?:ding|t))|u(?:ccess|spend|" +
                "bmit)|peech(?:start|end)|ound(?:start|end)|croll|how)|b(?:e(?:for(?:e(?:(?:scrip" +
                "texecu|activa)te|u(?:nload|pdate)|p(?:aste|rint)|c(?:opy|ut)|editfocus)|deactiva" +
                "te)|gin(?:Event)?)|oun(?:dary|ce)|l(?:ocked|ur)|roadcast|usy)|a(?:n(?:imation(?:" +
                "iteration|start|end)|tennastatechange)|fter(?:(?:scriptexecu|upda)te|print)|udio" +
                "(?:process|start|end)|d(?:apteradded|dtrack)|ctivate|lerting|bort)|DOM(?:Node(?:" +
                "Inserted(?:IntoDocument)?|Removed(?:FromDocument)?)|(?:CharacterData|Subtree)Mod" +
                "ified|A(?:ttrModified|ctivate)|Focus(?:Out|In)|MouseScroll)|r(?:e(?:s(?:u(?:m(?:" +
                "ing|e)|lt)|ize|et)|adystatechange|pea(?:tEven)?t|movetrack|trieving|ceived)|ow(?" +
                ":s(?:inserted|delete)|e(?:nter|xit))|atechange)|p(?:op(?:up(?:hid(?:den|ing)|sho" +
                "w(?:ing|n))|state)|a(?:ge(?:hide|show)|(?:st|us)e|int)|ro(?:pertychange|gress)|l" +
                "ay(?:ing)?)|t(?:ouch(?:(?:lea|mo)ve|en(?:ter|d)|cancel|start)|ime(?:update|out)|" +
                "ransitionend|ext)|u(?:s(?:erproximity|sdreceived)|p(?:gradeneeded|dateready)|n(?" +
                ":derflow|load))|f(?:o(?:rm(?:change|input)|cus(?:out|in)?)|i(?:lterchange|nish)|" +
                "ailed)|l(?:o(?:ad(?:e(?:d(?:meta)?data|nd)|start)?|secapture)|evelchange|y)|g(?:" +
                "amepad(?:(?:dis)?connected|button(?:down|up)|axismove)|et)|e(?:n(?:d(?:Event|ed)" +
                "?|abled|ter)|rror(?:update)?|mptied|xit)|i(?:cc(?:cardlockerror|infochange)|n(?:" +
                "coming|valid|put))|o(?:(?:(?:ff|n)lin|bsolet)e|verflow(?:changed)?|pen)|SVG(?:(?" +
                ":Unl|L)oad|Resize|Scroll|Abort|Error|Zoom)|h(?:e(?:adphoneschange|l[dp])|ashchan" +
                "ge|olding)|v(?:o(?:lum|ic)e|ersion)change|w(?:a(?:it|rn)ing|heel)|key(?:press|do" +
                "wn|up)|(?:AppComman|Loa)d|no(?:update|match)|Request|zoom))[\\s\\0]*=");
        
        public static JObject ToJson(HttpContext context)
        {
            RequestValidationService service = new RequestValidationService();
            byte[] data = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(data, 0, data.Length);
            string args = service.ValidateJson(Encoding.UTF8.GetString(data), context);
            JObject json = null;
            if (!(String.IsNullOrEmpty(args)))
            	json = service.ValidateJson(JObject.Parse(args), context);
            return json;
        }
        
        public virtual string ValidateJson(string json, HttpContext context)
        {
            if (ValidRequestRegex.IsMatch(json))
            	throw new HttpException(400, "Bad Request");
            return HttpUtility.HtmlDecode(json);
        }
        
        public virtual JObject ValidateJson(JObject json, HttpContext context)
        {
            bool isBad = false;
            if (json["IgnoreBusinessRules"] != null)
            	isBad = true;
            if (json["requests"] != null)
            {
                JArray list = ((JArray)(json["requests"]));
                foreach (JObject args in list.Values<JObject>())
                	if (args["IgnoreBusinessRules"] != null)
                    {
                        isBad = true;
                        break;
                    }
            }
            if (isBad)
            	throw new HttpException(400, "Bad Request");
            return json;
        }
    }
    
    public class WorkflowResources
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SortedDictionary<string, string> _staticResources;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private List<Regex> _dynamicResources;
        
        public WorkflowResources()
        {
            _staticResources = new SortedDictionary<string, string>();
            _dynamicResources = new List<Regex>();
        }
        
        public SortedDictionary<string, string> StaticResources
        {
            get
            {
                return this._staticResources;
            }
            set
            {
                this._staticResources = value;
            }
        }
        
        public List<Regex> DynamicResources
        {
            get
            {
                return this._dynamicResources;
            }
            set
            {
                this._dynamicResources = value;
            }
        }
    }
    
    public partial class WorkflowRegister : WorkflowRegisterBase
    {
    }
    
    public class WorkflowRegisterBase
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SortedDictionary<string, WorkflowResources> _resources;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SortedDictionary<string, List<string>> _roleRegister;
        
        public WorkflowRegisterBase()
        {
            // initialize system workflows
            _resources = new SortedDictionary<string, WorkflowResources>();
            RegisterBuiltinWorkflowResources();
            foreach (SiteContentFile w in ApplicationServices.Current.ReadSiteContent("sys/workflows%", "%"))
            {
                string text = w.Text;
                if (!(String.IsNullOrEmpty(text)))
                {
                    WorkflowResources wr = null;
                    if (!(Resources.TryGetValue(w.PhysicalName, out wr)))
                    {
                        wr = new WorkflowResources();
                        Resources[w.PhysicalName] = wr;
                    }
                    foreach (string s in text.Split(new char[] {
                                '\n'}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string query = s.Trim();
                        if (!(String.IsNullOrEmpty(query)))
                        	if (s.StartsWith("regex "))
                            {
                                string regexQuery = s.Substring(6).Trim();
                                if (!(String.IsNullOrEmpty(regexQuery)))
                                	try
                                    {
                                        wr.DynamicResources.Add(new Regex(regexQuery, RegexOptions.IgnoreCase));
                                    }
                                    catch (Exception )
                                    {
                                    }
                            }
                            else
                            	wr.StaticResources[query.ToLower()] = query;
                    }
                }
            }
            // read "role" workflows from the register
            _roleRegister = new SortedDictionary<string, List<string>>();
            foreach (SiteContentFile rr in ApplicationServices.Current.ReadSiteContent("sys/register/roles%", "%"))
            {
                string text = rr.Text;
                if (!(String.IsNullOrEmpty(text)))
                {
                    List<string> workflows = null;
                    if (!(RoleRegister.TryGetValue(rr.PhysicalName, out workflows)))
                    {
                        workflows = new List<string>();
                        RoleRegister[rr.PhysicalName] = workflows;
                    }
                    foreach (string s in text.Split(new char[] {
                                '\n',
                                ','}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string name = s.Trim();
                        if (!(String.IsNullOrEmpty(name)))
                        	workflows.Add(name);
                    }
                }
            }
        }
        
        public SortedDictionary<string, WorkflowResources> Resources
        {
            get
            {
                return this._resources;
            }
            set
            {
                this._resources = value;
            }
        }
        
        public SortedDictionary<string, List<string>> RoleRegister
        {
            get
            {
                return this._roleRegister;
            }
            set
            {
                this._roleRegister = value;
            }
        }
        
        public List<string> UserWorkflows
        {
            get
            {
                List<string> workflows = ((List<string>)(HttpContext.Current.Items["WorkflowRegister_UserWorkflows"]));
                if (workflows == null)
                {
                    workflows = new List<string>();
                    IIdentity identity = HttpContext.Current.User.Identity;
                    if (identity.IsAuthenticated)
                    	foreach (SiteContentFile urf in ApplicationServices.Current.ReadSiteContent("sys/register/users%", identity.Name))
                        {
                            string text = urf.Text;
                            if (!(String.IsNullOrEmpty(text)))
                            	foreach (string s in text.Split(new char[] {
                                            '\n',
                                            ','}, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string name = s.Trim();
                                    if (!(String.IsNullOrEmpty(name)) && !(workflows.Contains(name)))
                                    	workflows.Add(name);
                                }
                        }
                    // enumerate role workflows
                    bool isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
                    foreach (string role in RoleRegister.Keys)
                    	if ((((role == "?") && !(isAuthenticated)) || ((role == "*") && isAuthenticated)) || DataControllerBase.UserIsInRole(role))
                        	foreach (string name in RoleRegister[role])
                            	if (!(workflows.Contains(name)))
                                	workflows.Add(name);
                    HttpContext.Current.Items["WorkflowRegister_UserWorkflows"] = workflows;
                }
                return workflows;
            }
        }
        
        public bool Enabled
        {
            get
            {
                return (_resources.Count > 0);
            }
        }
        
        public static bool IsEnabled
        {
            get
            {
                if (!(ApplicationServices.IsSiteContentEnabled))
                	return false;
                WorkflowRegister wr = WorkflowRegister.GetCurrent();
                return ((wr != null) && wr.Enabled);
            }
        }
        
        public virtual int CacheDuration
        {
            get
            {
                return 30;
            }
        }
        
        protected virtual void RegisterBuiltinWorkflowResources()
        {
        }
        
        public static bool Allows(string fileName)
        {
            if (!(ApplicationServices.IsSiteContentEnabled))
            	return false;
            WorkflowRegister wr = WorkflowRegister.GetCurrent(fileName);
            if ((wr == null) || !(wr.Enabled))
            	return false;
            return wr.IsMatch(fileName);
        }
        
        public bool IsMatch(string physicalPath, string physicalName)
        {
            string fileName = physicalPath;
            if (String.IsNullOrEmpty(fileName))
            	fileName = physicalName;
            else
            	fileName = ((fileName + "/") 
                            + physicalName);
            return IsMatch(fileName);
        }
        
        public bool IsMatch(string fileName)
        {
            fileName = fileName.ToLower();
            List<string> activeWorkflows = UserWorkflows;
            foreach (string workflow in activeWorkflows)
            {
                WorkflowResources resourceList = null;
                if (Resources.TryGetValue(workflow, out resourceList))
                {
                    if (resourceList.StaticResources.ContainsKey(fileName))
                    	return true;
                    foreach (Regex re in resourceList.DynamicResources)
                    	if (re.IsMatch(fileName))
                        	return true;
                }
            }
            return false;
        }
        
        public static WorkflowRegister GetCurrent()
        {
            return GetCurrent(null);
        }
        
        public static WorkflowRegister GetCurrent(string relativePath)
        {
            if ((relativePath != null) && (relativePath.StartsWith("sys/workflows") || relativePath.StartsWith("sys/register")))
            	return null;
            string key = "WorkflowRegister_Current";
            HttpContext context = HttpContext.Current;
            WorkflowRegister instance = ((WorkflowRegister)(context.Items[key]));
            if (instance == null)
            {
                instance = ((WorkflowRegister)(context.Cache[key]));
                if (instance == null)
                {
                    instance = new WorkflowRegister();
                    context.Cache.Add(key, instance, null, DateTime.Now.AddSeconds(instance.CacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
                }
                context.Items[key] = instance;
            }
            return instance;
        }
    }
    
    public enum SiteContentFields
    {
        
        SiteContentId,
        
        DataFileName,
        
        DataContentType,
        
        Length,
        
        Path,
        
        Data,
        
        Roles,
        
        Users,
        
        Text,
        
        CacheProfile,
        
        RoleExceptions,
        
        UserExceptions,
        
        Schedule,
        
        ScheduleExceptions,
    }
    
    public class SiteContentFile
    {
        
        private object _id;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _name;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _path;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _contentType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _length;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private byte[] _data;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _physicalName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _error;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _schedule;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _scheduleExceptions;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cacheProfile;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _cacheDuration;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private HttpCacheability _cacheLocation;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _cacheVaryByParams;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _cacheVaryByHeaders;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _cacheNoStore;
        
        public SiteContentFile()
        {
            this.CacheLocation = HttpCacheability.NoCache;
        }
        
        public object Id
        {
            get
            {
                return _id;
            }
            set
            {
                if ((value != null) && (value.GetType() == typeof(byte[])))
                	value = new Guid(((byte[])(value)));
                _id = value;
            }
        }
        
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
        
        public string Path
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }
        
        public string ContentType
        {
            get
            {
                return this._contentType;
            }
            set
            {
                this._contentType = value;
            }
        }
        
        public int Length
        {
            get
            {
                return this._length;
            }
            set
            {
                this._length = value;
            }
        }
        
        public byte[] Data
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
            }
        }
        
        public string PhysicalName
        {
            get
            {
                return this._physicalName;
            }
            set
            {
                this._physicalName = value;
            }
        }
        
        public string Error
        {
            get
            {
                return this._error;
            }
            set
            {
                this._error = value;
            }
        }
        
        public string Schedule
        {
            get
            {
                return this._schedule;
            }
            set
            {
                this._schedule = value;
            }
        }
        
        public string ScheduleExceptions
        {
            get
            {
                return this._scheduleExceptions;
            }
            set
            {
                this._scheduleExceptions = value;
            }
        }
        
        public string CacheProfile
        {
            get
            {
                return this._cacheProfile;
            }
            set
            {
                this._cacheProfile = value;
            }
        }
        
        public int CacheDuration
        {
            get
            {
                return this._cacheDuration;
            }
            set
            {
                this._cacheDuration = value;
            }
        }
        
        public HttpCacheability CacheLocation
        {
            get
            {
                return this._cacheLocation;
            }
            set
            {
                this._cacheLocation = value;
            }
        }
        
        public string[] CacheVaryByParams
        {
            get
            {
                return this._cacheVaryByParams;
            }
            set
            {
                this._cacheVaryByParams = value;
            }
        }
        
        public string[] CacheVaryByHeaders
        {
            get
            {
                return this._cacheVaryByHeaders;
            }
            set
            {
                this._cacheVaryByHeaders = value;
            }
        }
        
        public bool CacheNoStore
        {
            get
            {
                return this._cacheNoStore;
            }
            set
            {
                this._cacheNoStore = value;
            }
        }
        
        public string Text
        {
            get
            {
                if ((this.Data != null) && (!(String.IsNullOrEmpty(this.ContentType)) && this.ContentType.StartsWith("text/")))
                	return Encoding.UTF8.GetString(this.Data);
                return null;
            }
            set
            {
                if (value == null)
                	_data = null;
                else
                {
                    _data = Encoding.UTF8.GetBytes(value);
                    _contentType = "text/plain";
                }
            }
        }
        
        public bool IsText
        {
            get
            {
                return ((_contentType != null) && Regex.IsMatch(_contentType, "^((text/\\w+)|(application/(javascript|json)))$"));
            }
        }
        
        public static byte[] ReadAllBytes(string relativePath)
        {
            return ApplicationServices.Current.ReadSiteContentBytes(relativePath);
        }
        
        public static int WriteAllBytes(string relativePath, byte[] data)
        {
            return WriteAllBytes(relativePath, MimeMapping.GetMimeMapping(System.IO.Path.GetFileName(relativePath)), data);
        }
        
        public static int WriteAllBytes(string relativePath, string contentType, byte[] data)
        {
            ApplicationServices services = ApplicationServices.Current;
            List<FieldValue> values = ToValues(relativePath, contentType, true);
            values.Add(new FieldValue(services.SiteContentFieldName(SiteContentFields.Data), data));
            values.Add(new FieldValue(services.SiteContentFieldName(SiteContentFields.Length), null));
            if (data != null)
            {
                values.Last().NewValue = data.Length;
                values.Last().Modified = true;
            }
            return Write(values).RowsAffected;
        }
        
        public static string ReadAllText(string relativePath)
        {
            return ApplicationServices.Current.ReadSiteContentString(relativePath);
        }
        
        public static JObject ReadJson(string relativePath)
        {
            string result = ReadAllText(relativePath);
            if (!(String.IsNullOrEmpty(result)) && (result[0] == '{'))
            	return JObject.Parse(result);
            return new JObject();
        }
        
        public static int WriteAllText(string relativePath, string text)
        {
            return WriteAllText(relativePath, "text/plain", text);
        }
        
        public static int WriteAllText(string relativePath, string contentType, string text)
        {
            List<FieldValue> values = ToValues(relativePath, contentType, true);
            values.Add(new FieldValue(ApplicationServices.Current.SiteContentFieldName(SiteContentFields.Text), text));
            return Write(values).RowsAffected;
        }
        
        public static int WriteJson(string relativePath, JObject json)
        {
            return WriteAllText(relativePath, "application/json", json.ToString());
        }
        
        public static ActionResult Write(List<FieldValue> values)
        {
            ActionArgs args = new ActionArgs();
            args.Controller = ApplicationServices.Current.GetSiteContentControllerName();
            args.View = "createForm1";
            args.Values = values.ToArray();
            args.LastCommandName = "New";
            args.CommandName = "Insert";
            args.IgnoreBusinessRules = true;
            if (values[0].OldValue != null)
            {
                args.View = "editForm1";
                args.LastCommandName = null;
                args.CommandName = "Update";
            }
            IDataController c = ControllerFactory.CreateDataController();
            ((Controller)(c)).AllowPublicAccess = true;
            ActionResult result = c.Execute(args.Controller, args.View, args);
            result.RaiseExceptionIfErrors();
            return result;
        }
        
        public static int Delete(string relativePath)
        {
            ApplicationServices services = ApplicationServices.Current;
            List<FieldValue> values = ToValues(relativePath, null, false);
            List<string> keys = new List<string>();
            foreach (SiteContentFile file in services.ReadSiteContent(((string)(values[2].Value)), ((string)(values[1].Value))))
            	keys.Add(file.Id.ToString());
            if (keys.Count > 0)
            {
                ActionArgs args = new ActionArgs();
                args.Controller = services.GetSiteContentControllerName();
                args.View = "grid1";
                args.Values = new FieldValue[] {
                        new FieldValue(values[0].Name, keys[0], keys[0])};
                args.SelectedValues = keys.ToArray();
                args.CommandName = "Delete";
                args.IgnoreBusinessRules = true;
                IDataController c = ControllerFactory.CreateDataController();
                ((Controller)(c)).AllowPublicAccess = true;
                ActionResult result = c.Execute(args.Controller, args.View, args);
                result.RaiseExceptionIfErrors();
                return result.RowsAffected;
            }
            return 0;
        }
        
        public static bool Exists(string relativePath)
        {
            return (ApplicationServices.Current.ReadSiteContent(relativePath).Length > 0);
        }
        
        private static List<FieldValue> ToValues(string relativePath, string contentType, bool checkForExisting)
        {
            ApplicationServices services = ApplicationServices.Current;
            string name = relativePath;
            string path = null;
            int index = relativePath.LastIndexOf("/");
            if (index >= 0)
            {
                name = relativePath.Substring((index + 1));
                path = relativePath.Substring(0, index);
            }
            List<FieldValue> list = new List<FieldValue>();
            list.Add(new FieldValue(services.SiteContentFieldName(SiteContentFields.SiteContentId)));
            list.Add(new FieldValue(services.SiteContentFieldName(SiteContentFields.DataFileName), name));
            list.Add(new FieldValue(services.SiteContentFieldName(SiteContentFields.Path), path));
            list.Add(new FieldValue(services.SiteContentFieldName(SiteContentFields.DataContentType)));
            if (checkForExisting)
            {
                SiteContentFile file = services.ReadSiteContent(relativePath);
                if (file != null)
                {
                    list[0].OldValue = file.Id;
                    list[0].Modified = false;
                    list[1].OldValue = file.Name;
                    list[2].OldValue = file.Path;
                    list[3].OldValue = file.ContentType;
                }
            }
            if (!(String.IsNullOrEmpty(contentType)))
            {
                list[3].NewValue = contentType;
                list[3].Modified = true;
            }
            return list;
        }
        
        public override string ToString()
        {
            return String.Format("{0}/{1}", Path, Name);
        }
    }
    
    public class SiteContentFileList : List<SiteContentFile>
    {
    }
    
    public partial class ApplicationServices : ApplicationServicesBase
    {
        
        public static String HomePageUrl
        {
            get
            {
                return Create().UserHomePageUrl();
            }
        }
        
        public static void Initialize()
        {
            Create().RegisterServices();
        }
        
        public static object Login(string username, string password, bool createPersistentCookie)
        {
            return Create().AuthenticateUser(username, password, createPersistentCookie);
        }
        
        public static void Logout()
        {
            Create().UserLogout();
        }
        
        public static string[] Roles()
        {
            return Create().UserRoles();
        }
        
        public static JObject Themes()
        {
            return Create().UserThemes();
        }
    }
    
    public class ApplicationServicesBase
    {
        
        public static bool EnableMobileClient = true;
        
        public static string DesignerPort = String.Empty;
        
        private JObject _defaultSettings;
        
        private static bool _enableCombinedCss;
        
        private static bool _enableMinifiedCss = true;
        
        public static Regex NameValueListRegex = new Regex("^\\s*(?\'Name\'\\w+)\\s*=\\s*(?\'Value\'[\\S\\s]+?)\\s*$", RegexOptions.Multiline);
        
        public static Regex SystemResourceRegex = new Regex("~/((sys/)|(views/)|(controllers/)|(site\\b))", RegexOptions.IgnoreCase);
        
        private string _userTheme;
        
        private string _userAccent;
        
        public static Regex CssUrlRegex = new Regex("(?\'Header\'\\burl\\s*\\(\\s*(\\\"|\\\')?)(?\'Name\'[\\w/\\.]+)(?\'Symbol\'\\S)");
        
        public static Regex DefaultExcludeScriptRegex = new Regex("^(daf\\\\|sys\\\\|lib\\\\|surveys\\\\|_references\\.js)|((.+?)\\.(\\w\\w(\\-\\w+)*)\\.js$)");
        
        public static SortedDictionary<string, ServiceRequestHandler> RequestHandlers = new SortedDictionary<string, ServiceRequestHandler>();
        
        public static Regex ViewPageCompressRegex = new Regex("((\"(DefaultValue)\"\\:(\"[\\s\\S]*?\"))|(\"(Items|Pivots|Fields|Views|ActionGroups|Categ" +
                "ories|Filter|Expressions|Errors)\"\\:(\\[\\]))|(\"(Len|CategoryIndex|Rows|Columns|Sea" +
                "rch|ItemsPageSize|Aggregate|OnDemandStyle|TextMode|MaskType|AutoCompletePrefixLe" +
                "ngth|DataViewPageSize|PageOffset)\"\\:(0))|(\"(CausesValidation|AllowQBE|AllowSorti" +
                "ng|FormatOnClient|HtmlEncode|RequiresMetaData|RequiresRowCount|ShowInSelector|Da" +
                "taViewShow(ActionBar|Description|ViewSelector|PageSize|SearchBar|QuickFind))\"\\:(" +
                "true))|(\"(IsPrimaryKey|ReadOnly|HasDefaultValue|Hidden|AllowLEV|AllowNulls|OnDem" +
                "and|IsMirror|Calculated|CausesCalculate|IsVirtual|AutoSelect|SearchOnStart|ShowI" +
                "nSummary|ItemsLetters|WhenKeySelected|RequiresSiteContentText|RequiresPivot|Requ" +
                "iresAggregates|Floating|Collapsed|Label|SupportsCaching|AllowDistinctFieldInFilt" +
                "er|Flat|RequiresMetaData|RequiresRowCount|Distinct|(DataView(ShowInSummary|Multi" +
                "Select|ShowModalForms|SearchByFirstLetter|SearchOnStart|ShowRowNumber|AutoHighli" +
                "ghtFirstRow|AutoSelectFirstRow)))\"\\:(false))|(\"(AliasName|Tag|FooterText|ToolTip" +
                "|Watermark|DataFormatString|Copy|HyperlinkFormatString|SourceFields|SearchOption" +
                "s|ItemsDataController|ItemsTargetController|ItemsDataView|ItemsDataValueField|It" +
                "emsDataTextField|ItemsStyle|ItemsNewDataView|OnDemandHandler|Mask|ContextFields|" +
                "Formula|Flow|Label|Configuration|Editor|ItemsDescription|Group|CommandName|Comma" +
                "ndArgument|HeaderText|Description|CssClass|Confirmation|Notify|Key|WhenLastComma" +
                "ndName|WhenLastCommandArgument|WhenClientScript|WhenTag|WhenHRef|WhenView|PivotD" +
                "efinitions|Aggregates|PivotDefinitions|Aggregates|ViewType|LastView|StatusBar|Ic" +
                "ons|LEVs|QuickFindHint|InnerJoinPrimaryKey|SystemFilter|DistinctValueFieldName|C" +
                "lientScript|FirstLetters|SortExpression|Template|Tab|Wizard|InnerJoinForeignKey|" +
                "Expressions|ViewHeaderText|ViewLayout|GroupExpression|FieldFilter|Wrap|Tags|Tag|" +
                "Id|Filter|(DataView(Id|FilterSource|Controller|FilterFields|ShowActionButtons|Sh" +
                "owPager)))\"\\:(\"\\s*\"|null))|(\"Type\":\"String\")),?");
        
        public static Regex ViewPageCompress2Regex = new Regex(",\\}(,|])");
        
        public virtual JObject DefaultSettings
        {
            get
            {
                if (_defaultSettings == null)
                {
                    string json = "{}";
                    string filePath = HttpContext.Current.Server.MapPath("~/touch-settings.json");
                    if (File.Exists(filePath))
                    	json = File.ReadAllText(filePath);
                    _defaultSettings = JObject.Parse(json);
                    EnsureJsonProperty(_defaultSettings, "appName", "Golf Club");
                    EnsureJsonProperty(_defaultSettings, "map.apiKey", MapsApiIdentifier);
                    EnsureJsonProperty(_defaultSettings, "charts.maxPivotRowCount", MaxPivotRowCount);
                    EnsureJsonProperty(_defaultSettings, "ui.theme.name", "Light");
                    JObject ui = ((JObject)(_defaultSettings["ui"]));
                    EnsureJsonProperty(ui, "theme.accent", "Aquarium");
                    EnsureJsonProperty(ui, "displayDensity.mobile", "Auto");
                    EnsureJsonProperty(ui, "displayDensity.desktop", "Condensed");
                    EnsureJsonProperty(ui, "list.labels.display", "DisplayedBelow");
                    EnsureJsonProperty(ui, "list.initialMode", "SeeAll");
                    EnsureJsonProperty(ui, "menu.location", "toolbar");
                    EnsureJsonProperty(ui, "actions.promote", true);
                    EnsureJsonProperty(ui, "smartDates", true);
                    EnsureJsonProperty(ui, "transitions.style", "");
                    EnsureJsonProperty(ui, "sidebar.when", "Landscape");
                }
                return _defaultSettings;
            }
        }
        
        public static bool EnableCombinedCss
        {
            get
            {
                return _enableCombinedCss;
            }
            set
            {
                _enableCombinedCss = value;
            }
        }
        
        public static bool EnableMinifiedCss
        {
            get
            {
                return _enableMinifiedCss;
            }
            set
            {
                _enableMinifiedCss = value;
            }
        }
        
        public static bool IsSiteContentEnabled
        {
            get
            {
                return !(String.IsNullOrEmpty(SiteContentControllerName));
            }
        }
        
        public static string SiteContentControllerName
        {
            get
            {
                return Create().GetSiteContentControllerName();
            }
        }
        
        public static string[] SiteContentEditors
        {
            get
            {
                return Create().GetSiteContentEditors();
            }
        }
        
        public static string[] SiteContentDevelopers
        {
            get
            {
                return Create().GetSiteContentDevelopers();
            }
        }
        
        public static bool IsContentEditor
        {
            get
            {
                IPrincipal principal = HttpContext.Current.User;
                foreach (string r in Create().GetSiteContentEditors())
                	if (principal.IsInRole(r))
                    	return true;
                return false;
            }
        }
        
        public static bool IsDeveloper
        {
            get
            {
                IPrincipal principal = HttpContext.Current.User;
                foreach (string r in Create().GetSiteContentDevelopers())
                	if (principal.IsInRole(r))
                    	return true;
                return false;
            }
        }
        
        public static bool IsSafeMode
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                Uri test = request.UrlReferrer;
                if (test == null)
                	test = request.Url;
                return ((test == null) && (test.ToString().Contains("_safemode=true") && DataControllerBase.UserIsInRole(SiteContentDevelopers)));
            }
        }
        
        public virtual int ScheduleCacheDuration
        {
            get
            {
                return 20;
            }
        }
        
        public virtual string Realm
        {
            get
            {
                return Name;
            }
        }
        
        public virtual string Name
        {
            get
            {
                return "Golf Club New";
            }
        }
        
        public static string MapsApiIdentifier
        {
            get
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Request.Headers["X-Cot-Manifest-Request"] == "true"))
                	return WebConfigurationManager.AppSettings["MapsApiIdentifierMobile"];
                return WebConfigurationManager.AppSettings["MapsApiIdentifier"];
            }
        }
        
        public virtual int MaxPivotRowCount
        {
            get
            {
                return 250000;
            }
        }
        
        public static ApplicationServices Current
        {
            get
            {
                return Create();
            }
        }
        
        public static bool IsTouchClient
        {
            get
            {
                return true;
            }
        }
        
        public virtual string UserTheme
        {
            get
            {
                if (String.IsNullOrEmpty(_userTheme))
                	LoadTheme();
                return _userTheme;
            }
        }
        
        public virtual string UserAccent
        {
            get
            {
                if (String.IsNullOrEmpty(_userAccent))
                	LoadTheme();
                return _userAccent;
            }
        }
        
        public virtual bool EnableCors
        {
            get
            {
                return false;
            }
        }
        
        public static JToken Settings(string selector)
        {
            return SelectFrom(Current.DefaultSettings, selector);
        }
        
        public static JToken SelectFrom(JToken json, string selector)
        {
            string[] path = Regex.Split(selector, "\\.");
            for (int i = 0; (i < path.Length); i++)
            {
                json = json[path[i]];
                if (json == null)
                	break;
            }
            return json;
        }
        
        public virtual string GetNavigateUrl()
        {
            return null;
        }
        
        public static void VerifyUrl()
        {
            string navigateUrl = Create().GetNavigateUrl();
            if (!(String.IsNullOrEmpty(navigateUrl)))
            {
                HttpContext current = HttpContext.Current;
                if (!(VirtualPathUtility.ToAbsolute(navigateUrl).Equals(current.Request.RawUrl, StringComparison.CurrentCultureIgnoreCase)))
                	current.Response.Redirect(navigateUrl);
            }
        }
        
        public virtual void RegisterServices()
        {
            CreateStandardMembershipAccounts();
            RouteCollection routes = RouteTable.Routes;
            RegisterIgnoredRoutes(routes);
            RegisterContentServices(RouteTable.Routes);
            // Register service request handlers
            RequestHandlers.Add("getpage", new GetPageServiceRequestHandler());
            RequestHandlers.Add("getpagelist", new GetPageListServiceRequestHandler());
            RequestHandlers.Add("getlistofvalues", new GetListOfValuesServiceRequestHandler());
            RequestHandlers.Add("execute", new ExecuteServiceRequestHandler());
            RequestHandlers.Add("executeandgetpage", new ExecuteAndGetPageServiceRequestHandler());
            RequestHandlers.Add("executelist", new ExecuteListServiceRequestHandler());
            RequestHandlers.Add("getcompletionlist", new GetCompletionListServiceRequestHandler());
            RequestHandlers.Add("login", new LoginServiceRequestHandler());
            RequestHandlers.Add("logout", new LogoutServiceRequestHandler());
            RequestHandlers.Add("roles", new RolesServiceRequestHandler());
            RequestHandlers.Add("themes", new ThemesServiceRequestHandler());
            RequestHandlers.Add("getsurvey", new GetSurveyServiceRequestHandler());
            RequestHandlers.Add("saas/dnn", new DnnOAuthServiceRequestHandler());
            OAuthHandlerFactory.Handlers.Add("dnn", typeof(DnnOAuthHandler));
            RequestHandlers.Add("getcontrollerlist", new GetControllerListServiceRequestHandler());
            RequestHandlers.Add("commit", new CommitServiceRequestHandler());
            // Find designer port
            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IISExpress\\config\\applicationhost.config");
                if (File.Exists(configPath))
                {
                    string content = File.ReadAllText(configPath);
                    Match m = Regex.Match(content, "<site name=\"CodeOnTime\".*?bindingInformation=\"\\*:(?\'Port\'\\d+):localhost\"", RegexOptions.Singleline);
                    if (m.Success)
                    	DesignerPort = m.Groups["Port"].Value;
                }
            }
            finally
            {
                // release resources here
            }
        }
        
        public static void Start()
        {
            Current.InstanceStart();
        }
        
        protected virtual void InstanceStart()
        {
            MyCompany.Services.ApplicationServices.Initialize();
        }
        
        public static void Stop()
        {
            Current.InstanceStop();
        }
        
        protected virtual void InstanceStop()
        {
        }
        
        public static void SessionStart()
        {
            // The line below will prevent intermittent error “Session state has created a session id,
            // but cannot save it because the response was already flushed by the application.”
            string sessionId = HttpContext.Current.Session.SessionID;
            Current.UserSessionStart();
        }
        
        protected virtual void UserSessionStart()
        {
        }
        
        public static void SessionStop()
        {
            Current.UserSessionStop();
        }
        
        protected virtual void UserSessionStop()
        {
        }
        
        public static void Error()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            	Current.HandleError(context, context.Server.GetLastError());
        }
        
        protected virtual void HandleError(HttpContext context, Exception error)
        {
        }
        
        public virtual object HandleException(JObject result, Exception ex)
        {
            while (ex.InnerException != null)
            	ex = ex.InnerException;
            ServiceRequestError error = new ServiceRequestError();
            error.Message = ex.Message;
            error.ExceptionType = ex.GetType().ToString();
            HttpContext current = HttpContext.Current;
            if (current.Request.Url.Host.Equals("localhost") || !(HttpContext.Current.IsCustomErrorEnabled))
            	error.StackTrace = ex.StackTrace;
            return error;
        }
        
        public virtual void RegisterContentServices(RouteCollection routes)
        {
            GenericRoute.Map(RouteTable.Routes, new PlaceholderHandler(), "placeholder/{FileName}");
        }
        
        public virtual void RegisterIgnoredRoutes(RouteCollection routes)
        {
        }
        
        public static SortedDictionary<string, string> LoadContent()
        {
            SortedDictionary<string, string> content = new SortedDictionary<string, string>();
            Create().LoadContent(HttpContext.Current.Request, HttpContext.Current.Response, content);
            string rawContent = null;
            if (content.TryGetValue("File", out rawContent))
            {
                // find the head
                Match headMatch = Regex.Match(rawContent, "<head>([\\s\\S]+?)</head>");
                if (headMatch.Success)
                {
                    string head = headMatch.Groups[1].Value;
                    head = Regex.Replace(head, "\\s*<meta charset=\".+\"\\s*/?>\\s*", String.Empty);
                    content["Head"] = Regex.Replace(head, "\\s*<title>([\\S\\s]*?)</title>\\s*", String.Empty);
                    // find the title
                    Match titleMatch = Regex.Match(head, "<title>(?\'Title\'[\\S\\s]+?)</title>");
                    if (titleMatch.Success)
                    {
                        string title = titleMatch.Groups["Title"].Value;
                        content["PageTitle"] = title;
                        content["PageTitleContent"] = title;
                    }
                    // find "about"
                    Match aboutMatch = Regex.Match(head, "<meta\\s+name\\s*=\\s*\"description\"\\s+content\\s*=\\s*\"([\\s\\S]+?)\"\\s*/>");
                    if (aboutMatch.Success)
                    	content["About"] = HttpUtility.HtmlDecode(aboutMatch.Groups[1].Value);
                }
                // find the body
                Match bodyMatch = Regex.Match(rawContent, "<body(?\'Attr\'[\\s\\S]*?)>(?\'Body\'[\\s\\S]+?)</body>");
                if (bodyMatch.Success)
                {
                    content["PageContent"] = EnrichData(bodyMatch.Groups["Body"].Value);
                    content["BodyAttributes"] = bodyMatch.Groups["Attr"].Value;
                }
                else
                	content["PageContent"] = EnrichData(rawContent);
            }
            return content;
        }
        
        static string EnrichData(string body)
        {
            return Regex.Replace(body, "(<script[^>]*data-type=\"\\$app\\.execute\"[^>]*>(?<Script>(.|\\n)*?)<\\/script>)", DoEnrichData);
        }
        
        static string DoEnrichData(Match m)
        {
            try
            {
                string json = m.Groups["Script"].Value.Trim().Trim(')', '(', ';');
                JObject obj = JObject.Parse(json);
                PageRequest request = new PageRequest();
                request.Controller = ((string)(obj["controller"]));
                request.View = ((string)(obj["view"]));
                request.PageIndex = Convert.ToInt32(obj["pageIndex"]);
                request.PageSize = Convert.ToInt32(obj["pageSize"]);
                if (request.PageSize == 0)
                	request.PageSize = 100;
                request.SortExpression = ((string)(obj["sortExpression"]));
                JArray metadataFilter = ((JArray)(obj["metadataFilter"]));
                if (metadataFilter != null)
                	request.MetadataFilter = metadataFilter.ToObject<string[]>();
                else
                	request.MetadataFilter = new string[] {
                            "fields"};
                request.RequiresMetaData = true;
                ViewPage page = ControllerFactory.CreateDataController().GetPage(request.Controller, request.View, request);
                string output = ApplicationServices.CompressViewPageJsonOutput(JsonConvert.SerializeObject(page));
                object doFormat = obj["format"];
                if (doFormat == null)
                	doFormat = "true";
                object id = obj["id"];
                if (id == null)
                	id = request.Controller;
                return String.Format("<script>$app.data({{\"id\":\"{0}\",\"format\":{1},\"d\":{2}}});</script>", id, Convert.ToBoolean(doFormat).ToString().ToLower(), output);
            }
            catch (Exception ex)
            {
                return (("<div class=\"well text-danger\">" + ex.Message) 
                            + "</div>");
            }
        }
        
        public virtual string GetSiteContentControllerName()
        {
            return null;
        }
        
        public virtual string GetSiteContentViewId()
        {
            return "editForm1";
        }
        
        public virtual string[] GetSiteContentEditors()
        {
            return new string[] {
                    "Administrators",
                    "Content Editors",
                    "Developers"};
        }
        
        public virtual string[] GetSiteContentDevelopers()
        {
            return new string[] {
                    "Administrators",
                    "Developers"};
        }
        
        public virtual void AfterAction(ActionArgs args, ActionResult result)
        {
        }
        
        public virtual void BeforeAction(ActionArgs args, ActionResult result)
        {
            if ((args.Controller == SiteContentControllerName) && !(args.IgnoreBusinessRules))
            {
                bool userIsDeveloper = IsDeveloper;
                if ((!(IsContentEditor) || !(userIsDeveloper)) || (args.Values == null))
                	throw new HttpException(403, "Forbidden");
                FieldValue id = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.SiteContentId));
                FieldValue path = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.Path));
                FieldValue fileName = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.DataFileName));
                FieldValue text = args.SelectFieldValueObject(SiteContentFieldName(SiteContentFields.Text));
                // verify "Path" access
                if ((path == null) || (fileName == null))
                	throw new HttpException(403, "Forbidden");
                if (((path.Value != null) && path.Value.ToString().StartsWith("sys/", StringComparison.CurrentCultureIgnoreCase)) && !(userIsDeveloper))
                	throw new HttpException(403, "Forbidden");
                if (((path.OldValue != null) && path.OldValue.ToString().StartsWith("sys/", StringComparison.CurrentCultureIgnoreCase)) && !(userIsDeveloper))
                	throw new HttpException(403, "Forbidden");
                // convert and parse "Text" as needed
                if ((text != null) && args.CommandName != "Delete")
                {
                    string s = Convert.ToString(text.Value);
                    if (s == "$Text")
                    {
                        string fullPath = Convert.ToString(path.Value);
                        if (!(String.IsNullOrEmpty(fullPath)))
                        	fullPath = (fullPath + "/");
                        fullPath = (fullPath + Convert.ToString(fileName.Value));
                        if (!(fullPath.StartsWith("/")))
                        	fullPath = ("/" + fullPath);
                        if (!(fullPath.EndsWith(".html", StringComparison.CurrentCultureIgnoreCase)))
                        	fullPath = (fullPath + ".html");
                        string physicalPath = HttpContext.Current.Server.MapPath(("~" + fullPath));
                        if (!(File.Exists(physicalPath)))
                        {
                            physicalPath = HttpContext.Current.Server.MapPath(("~" + fullPath.Replace("-", String.Empty)));
                            if (!(File.Exists(physicalPath)))
                            	physicalPath = null;
                        }
                        if (!(String.IsNullOrEmpty(physicalPath)))
                        	text.NewValue = File.ReadAllText(physicalPath);
                    }
                }
            }
        }
        
        public virtual string SiteContentFieldName(SiteContentFields field)
        {
            return field.ToString();
        }
        
        public virtual string ReadSiteContentString(string relativePath)
        {
            byte[] data = ReadSiteContentBytes(relativePath);
            if (data == null)
            	return null;
            return Encoding.UTF8.GetString(data);
        }
        
        public virtual byte[] ReadSiteContentBytes(string relativePath)
        {
            SiteContentFile f = ReadSiteContent(relativePath);
            if (f == null)
            	return null;
            return f.Data;
        }
        
        public virtual SiteContentFile ReadSiteContent(string relativePath)
        {
            HttpContext context = HttpContext.Current;
            SiteContentFile f = ((SiteContentFile)(context.Items[relativePath]));
            if (f == null)
            	f = ((SiteContentFile)(context.Cache[relativePath]));
            if (f == null)
            {
                string path = relativePath;
                string fileName = relativePath;
                int index = relativePath.LastIndexOf("/");
                if (index >= 0)
                {
                    fileName = path.Substring((index + 1));
                    path = relativePath.Substring(0, index);
                }
                else
                	path = null;
                SiteContentFileList files = ReadSiteContent(path, fileName, 1);
                if (files.Count == 1)
                {
                    f = files[0];
                    context.Items[relativePath] = f;
                    if (f.CacheDuration > 0)
                    	context.Cache.Add(relativePath, f, null, DateTime.Now.AddSeconds(f.CacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
            }
            return f;
        }
        
        public virtual SiteContentFileList ReadSiteContent(string relativePath, string fileName)
        {
            return ReadSiteContent(relativePath, fileName, Int32.MaxValue);
        }
        
        public virtual SiteContentFileList ReadSiteContent(string relativePath, string fileName, int maxCount)
        {
            SiteContentFileList result = new SiteContentFileList();
            if (IsSafeMode)
            	return result;
            // prepare a filter
            string dataFileNameField = SiteContentFieldName(SiteContentFields.DataFileName);
            string pathField = SiteContentFieldName(SiteContentFields.Path);
            List<string> filter = new List<string>();
            string pathFilter = null;
            if (!(String.IsNullOrEmpty(relativePath)))
            {
                pathFilter = "{0}:={1}";
                int firstWildcardIndex = relativePath.IndexOf("%");
                if (firstWildcardIndex >= 0)
                {
                    int lastWildcardIndex = relativePath.LastIndexOf("%");
                    pathFilter = "{0}:$contains${1}";
                    if (firstWildcardIndex == lastWildcardIndex)
                    	if (firstWildcardIndex == 0)
                        {
                            pathFilter = "{0}:$endswith${1}";
                            relativePath = relativePath.Substring(1);
                        }
                        else
                        	if (lastWildcardIndex == (relativePath.Length - 1))
                            {
                                pathFilter = "{0}:$beginswith${1}";
                                relativePath = relativePath.Substring(0, lastWildcardIndex);
                            }
                }
            }
            else
            	pathFilter = "{0}:=null";
            string fileNameFilter = null;
            if (!(String.IsNullOrEmpty(fileName)) && !((fileName == "%")))
            {
                fileNameFilter = "{0}:={1}";
                int firstWildcardIndex = fileName.IndexOf("%");
                if (firstWildcardIndex >= 0)
                {
                    int lastWildcardIndex = fileName.LastIndexOf("%");
                    fileNameFilter = "{0}:$contains${1}";
                    if (firstWildcardIndex == lastWildcardIndex)
                    	if (firstWildcardIndex == 0)
                        {
                            fileNameFilter = "{0}:$endswith${1}";
                            fileName = fileName.Substring(1);
                        }
                        else
                        	if (lastWildcardIndex == (fileName.Length - 1))
                            {
                                fileNameFilter = "{0}:$beginswith${1}";
                                fileName = fileName.Substring(0, lastWildcardIndex);
                            }
                }
            }
            if (!(String.IsNullOrEmpty(pathFilter)) || !(String.IsNullOrEmpty(fileNameFilter)))
            {
                filter.Add("_match_:$all$");
                if (!(String.IsNullOrEmpty(pathFilter)))
                	filter.Add(String.Format(pathFilter, pathField, DataControllerBase.ValueToString(relativePath)));
                if (fileName != null && !((fileName == "%")))
                {
                    filter.Add(String.Format(fileNameFilter, dataFileNameField, DataControllerBase.ValueToString(fileName)));
                    if (String.IsNullOrEmpty(Path.GetExtension(fileName)) && (String.IsNullOrEmpty(relativePath) || (!(relativePath.StartsWith("sys/", StringComparison.OrdinalIgnoreCase)) || relativePath.StartsWith("sys/controls", StringComparison.OrdinalIgnoreCase))))
                    {
                        filter.Add("_match_:$all$");
                        if (!(String.IsNullOrEmpty(pathFilter)))
                        	filter.Add(String.Format(pathFilter, pathField, DataControllerBase.ValueToString(relativePath)));
                        filter.Add(String.Format(fileNameFilter, dataFileNameField, DataControllerBase.ValueToString((Path.GetFileNameWithoutExtension(fileName).Replace("-", String.Empty) + ".html"))));
                    }
                }
            }
            //  determine user identity
            HttpContext context = HttpContext.Current;
            string userName = String.Empty;
            bool isAuthenticated = false;
            IPrincipal user = context.User;
            if (user != null)
            {
                userName = user.Identity.Name.ToLower();
                isAuthenticated = user.Identity.IsAuthenticated;
            }
            // enumerate site content files
            PageRequest r = new PageRequest();
            r.Controller = GetSiteContentControllerName();
            r.View = GetSiteContentViewId();
            r.RequiresSiteContentText = true;
            r.PageSize = Int32.MaxValue;
            r.Filter = filter.ToArray();
            IDataEngine engine = ControllerFactory.CreateDataEngine();
            DataControllerBase controller = ((DataControllerBase)(engine));
            controller.AllowPublicAccess = true;
            IDataReader reader = engine.ExecuteReader(r);
            SortedDictionary<string, SiteContentFile> blobsToResolve = new SortedDictionary<string, SiteContentFile>();
            // verify optional SiteContent fields
            SortedDictionary<string, string> fieldDictionary = new SortedDictionary<string, string>();
            for (int i = 0; (i < reader.FieldCount); i++)
            {
                string fieldName = reader.GetName(i);
                fieldDictionary[fieldName] = fieldName;
            }
            string rolesField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.Roles), out rolesField);
            string usersField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.Users), out usersField);
            string roleExceptionsField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.RoleExceptions), out roleExceptionsField);
            string userExceptionsField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.UserExceptions), out userExceptionsField);
            string cacheProfileField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.CacheProfile), out cacheProfileField);
            string scheduleField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.Schedule), out scheduleField);
            string scheduleExceptionsField = null;
            fieldDictionary.TryGetValue(SiteContentFieldName(SiteContentFields.ScheduleExceptions), out scheduleExceptionsField);
            DataField dataField = controller.CreateViewPage().FindField(SiteContentFieldName(SiteContentFields.Data));
            string blobHandler = dataField.OnDemandHandler;
            WorkflowRegister wr = WorkflowRegister.GetCurrent(relativePath);
            // read SiteContent files
            while (reader.Read())
            {
                // verify user access rights
                bool include = true;
                if (!(String.IsNullOrEmpty(rolesField)))
                {
                    string roles = Convert.ToString(reader[rolesField]);
                    if (!(String.IsNullOrEmpty(roles)) && !((roles == "?")))
                    	if ((roles == "*") && !(isAuthenticated))
                        	include = false;
                        else
                        	if (!(isAuthenticated) || (!((roles == "*")) && !(DataControllerBase.UserIsInRole(roles))))
                            	include = false;
                }
                if (include && !(String.IsNullOrEmpty(usersField)))
                {
                    string users = Convert.ToString(reader[usersField]);
                    if (!(String.IsNullOrEmpty(users)) && (Array.IndexOf(users.ToLower().Split(new char[] {
                                                ','}, StringSplitOptions.RemoveEmptyEntries), userName) == -1))
                    	include = false;
                }
                if (include && !(String.IsNullOrEmpty(roleExceptionsField)))
                {
                    string roleExceptions = Convert.ToString(reader[roleExceptionsField]);
                    if (!(String.IsNullOrEmpty(roleExceptions)) && (isAuthenticated && ((roleExceptions == "*") || DataControllerBase.UserIsInRole(roleExceptions))))
                    	include = false;
                }
                if (include && !(String.IsNullOrEmpty(userExceptionsField)))
                {
                    string userExceptions = Convert.ToString(reader[userExceptionsField]);
                    if (!(String.IsNullOrEmpty(userExceptions)) && !((Array.IndexOf(userExceptions.ToLower().Split(new char[] {
                                                ','}, StringSplitOptions.RemoveEmptyEntries), userName) == -1)))
                    	include = false;
                }
                string physicalName = Convert.ToString(reader[dataFileNameField]);
                string physicalPath = Convert.ToString(reader[SiteContentFieldName(SiteContentFields.Path)]);
                // check if the content object is a part of a workflow
                if (((wr != null) && wr.Enabled) && !(wr.IsMatch(physicalPath, physicalName)))
                	include = false;
                string schedule = null;
                string scheduleExceptions = null;
                // check if the content object is on schedule
                if (include && (String.IsNullOrEmpty(physicalPath) || !(physicalPath.StartsWith("sys/schedules/"))))
                {
                    if (!(String.IsNullOrEmpty(scheduleField)))
                    	schedule = Convert.ToString(reader[scheduleField]);
                    if (!(String.IsNullOrEmpty(scheduleExceptionsField)))
                    	scheduleExceptions = Convert.ToString(reader[scheduleExceptionsField]);
                }
                // create a file instance
                if (include)
                {
                    string siteContentIdField = SiteContentFieldName(SiteContentFields.SiteContentId);
                    SiteContentFile f = new SiteContentFile();
                    f.Id = reader[siteContentIdField];
                    f.Name = fileName;
                    f.PhysicalName = physicalName;
                    if (String.IsNullOrEmpty(f.Name) || f.Name.Contains("%"))
                    	f.Name = f.PhysicalName;
                    f.Path = physicalPath;
                    f.ContentType = Convert.ToString(reader[SiteContentFieldName(SiteContentFields.DataContentType)]);
                    f.Schedule = schedule;
                    f.ScheduleExceptions = scheduleExceptions;
                    if (!(String.IsNullOrEmpty(cacheProfileField)))
                    {
                        string cacheProfile = Convert.ToString(reader[cacheProfileField]);
                        if (!(String.IsNullOrEmpty(cacheProfile)))
                        {
                            f.CacheProfile = cacheProfile;
                            cacheProfile = ReadSiteContentString(("sys/cache-profiles/" + cacheProfile));
                            if (!(String.IsNullOrEmpty(cacheProfile)))
                            {
                                Match m = NameValueListRegex.Match(cacheProfile);
                                while (m.Success)
                                {
                                    string n = m.Groups["Name"].Value.ToLower();
                                    string v = m.Groups["Value"].Value;
                                    if (n == "duration")
                                    {
                                        int duration = 0;
                                        if (Int32.TryParse(v, out duration))
                                        {
                                            f.CacheDuration = duration;
                                            f.CacheLocation = HttpCacheability.ServerAndPrivate;
                                        }
                                    }
                                    else
                                    	if (n == "location")
                                        	try
                                            {
                                                f.CacheLocation = ((HttpCacheability)(TypeDescriptor.GetConverter(typeof(HttpCacheability)).ConvertFromString(v)));
                                            }
                                            catch (Exception )
                                            {
                                            }
                                        else
                                        	if (n == "varybyheaders")
                                            	f.CacheVaryByHeaders = v.Split(new char[] {
                                                            ',',
                                                            ';'}, StringSplitOptions.RemoveEmptyEntries);
                                            else
                                            	if (n == "varybyparams")
                                                	f.CacheVaryByParams = v.Split(new char[] {
                                                                ',',
                                                                ';'}, StringSplitOptions.RemoveEmptyEntries);
                                                else
                                                	if (n == "nostore")
                                                    	f.CacheNoStore = (v.ToLower() == "true");
                                    m = m.NextMatch();
                                }
                            }
                        }
                    }
                    object textString = reader[SiteContentFieldName(SiteContentFields.Text)];
                    if (DBNull.Value.Equals(textString) || !(f.IsText))
                    {
                        string blobKey = String.Format("{0}=o|{1}", blobHandler, f.Id);
                        if (f.CacheDuration > 0)
                        	f.Data = ((byte[])(HttpContext.Current.Cache[blobKey]));
                        if (f.Data == null)
                        	blobsToResolve[blobKey] = f;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(f.ContentType))
                        	if (Regex.IsMatch(((string)(textString)), "</\\w+\\s*>"))
                            	f.ContentType = "text/xml";
                            else
                            	f.ContentType = "text/plain";
                        f.Data = Encoding.UTF8.GetBytes(((string)(textString)));
                    }
                    result.Add(f);
                    if (result.Count == maxCount)
                    	break;
                }
            }
            reader.Close();
            foreach (string blobKey in blobsToResolve.Keys)
            {
                SiteContentFile f = blobsToResolve[blobKey];
                // download blob content
                try
                {
                    f.Data = Blob.Read(blobKey);
                    if (f.CacheDuration > 0)
                    	HttpContext.Current.Cache.Add(blobKey, f.Data, null, DateTime.Now.AddSeconds(f.CacheDuration), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
                catch (Exception ex)
                {
                    f.Error = ex.Message;
                }
            }
            return result;
        }
        
        public virtual bool IsSystemResource(HttpRequest request)
        {
            return SystemResourceRegex.IsMatch(request.AppRelativeCurrentExecutionFilePath);
        }
        
        public virtual void LoadContent(HttpRequest request, HttpResponse response, SortedDictionary<string, string> content)
        {
            if (IsSystemResource(request))
            	return;
            string text = null;
            bool tryFileSystem = true;
            if (IsSiteContentEnabled)
            {
                string fileName = HttpUtility.UrlDecode(request.Url.Segments[(request.Url.Segments.Length - 1)]);
                string path = request.CurrentExecutionFilePath.Substring(request.ApplicationPath.Length);
                if ((fileName == "/") && String.IsNullOrEmpty(path))
                	fileName = "index";
                else
                {
                    path = path.Substring(0, (path.Length - fileName.Length));
                    if (path.EndsWith("/"))
                    	path = path.Substring(0, (path.Length - 1));
                }
                if (String.IsNullOrEmpty(path))
                	path = null;
                SiteContentFileList files = ReadSiteContent(path, fileName, 1);
                if (files.Count > 0)
                {
                    SiteContentFile f = files[0];
                    if (f.ContentType == "text/html")
                    {
                        text = f.Text;
                        tryFileSystem = false;
                    }
                    else
                    {
                        if (f.CacheDuration > 0)
                        {
                            DateTime expires = DateTime.Now.AddSeconds(f.CacheDuration);
                            response.Cache.SetExpires(expires);
                            response.Cache.SetCacheability(f.CacheLocation);
                            if (f.CacheVaryByParams != null)
                            	foreach (string header in f.CacheVaryByParams)
                                	response.Cache.VaryByParams[header] = true;
                            if (f.CacheVaryByHeaders != null)
                            	foreach (string header in f.CacheVaryByHeaders)
                                	response.Cache.VaryByHeaders[header] = true;
                            if (f.CacheNoStore)
                            	response.Cache.SetNoStore();
                        }
                        response.ContentType = f.ContentType;
                        response.AddHeader("Content-Disposition", ("filename=" + HttpUtility.UrlEncode(f.PhysicalName)));
                        response.OutputStream.Write(f.Data, 0, f.Data.Length);
                        try
                        {
                            response.Flush();
                        }
                        catch (Exception )
                        {
                        }
                        response.End();
                    }
                }
            }
            if (tryFileSystem)
            {
                string filePath = request.PhysicalPath;
                string fileExtension = Path.GetExtension(filePath);
                if (!((fileExtension.ToLower() == ".html")) && File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    response.AddHeader("Content-Disposition", ("filename=" + HttpUtility.UrlEncode(fileName)));
                    response.ContentType = MimeMapping.GetMimeMapping(fileName);
                    System.DateTime expires = DateTime.Now.AddSeconds(((60 * 60) 
                                    * 24));
                    response.Cache.SetExpires(expires);
                    response.Cache.SetCacheability(HttpCacheability.Public);
                    byte[] data = File.ReadAllBytes(filePath);
                    response.OutputStream.Write(data, 0, data.Length);
                    try
                    {
                        response.Flush();
                    }
                    catch (Exception )
                    {
                    }
                    response.End();
                }
                if (!(String.IsNullOrEmpty(fileExtension)))
                	filePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                filePath = (filePath + ".html");
                if (File.Exists(filePath))
                	text = File.ReadAllText(filePath);
                else
                	if (Path.GetFileNameWithoutExtension(filePath).Contains("-"))
                    {
                        filePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileName(filePath).Replace("-", String.Empty));
                        if (File.Exists(filePath))
                        	text = File.ReadAllText(filePath);
                    }
                if (text != null)
                	text = Localizer.Replace("Pages", filePath, text);
            }
            if (text != null)
            {
                text = Regex.Replace(text, "<!--[\\s\\S]+?-->\\s*", String.Empty);
                content["File"] = text;
            }
        }
        
        public virtual void CreateStandardMembershipAccounts()
        {
            // Create a separate code file with a definition of the partial class ApplicationServices overriding
            // this method to prevent automatic registration of 'admin' and 'user'. Do not change this file directly.
            RegisterStandardMembershipAccounts();
        }
        
        public virtual bool RequiresAuthentication(HttpRequest request)
        {
            return request.Path.EndsWith("Export.ashx", StringComparison.CurrentCultureIgnoreCase);
        }
        
        public virtual bool AuthenticateRequest(HttpContext context)
        {
            return false;
        }
        
        public virtual void RedirectToLoginPage()
        {
            OAuthHandler handler = OAuthHandlerFactory.GetActiveHandler();
            if (handler != null)
            {
                handler.StartPage = HttpContext.Current.Request.Url.AbsolutePath;
                handler.RedirectToLoginPage();
                return;
            }
            FormsAuthentication.RedirectToLoginPage();
        }
        
        public virtual object AuthenticateUser(string username, string password, bool createPersistentCookie)
        {
            HttpResponse response = HttpContext.Current.Response;
            if (password.StartsWith("token:"))
            {
                // validate token login
                try
                {
                    string key = password.Substring(6);
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(key);
                    if (ValidateTicket(ticket))
                    {
                        MembershipUser user = Membership.GetUser(ticket.Name);
                        if (user != null)
                        {
                            InvalidateTicket(ticket);
                            HttpCookie cookie = new HttpCookie(".PROVIDER", String.Empty);
                            if (!(String.IsNullOrEmpty(ticket.UserData)) && ticket.UserData.StartsWith("OAUTH:"))
                            {
                                OAuthHandler handler = OAuthHandlerFactory.Create(ticket.UserData.Substring(6));
                                if (handler != null)
                                {
                                    cookie.Value = handler.GetHandlerName();
                                    if (!(handler.AuthenticateTicket(user)))
                                    	return false;
                                }
                            }
                            HttpContext.Current.Response.SetCookie(cookie);
                            FormsAuthentication.SetAuthCookie(user.UserName, createPersistentCookie);
                            return CreateTicket(user);
                        }
                    }
                }
                catch (Exception )
                {
                }
            }
            else
            {
                // login user
                if (UserLogin(username, password, createPersistentCookie))
                {
                    MembershipUser user = Membership.GetUser(username);
                    if (user != null)
                    	return CreateTicket(user);
                }
            }
            return false;
        }
        
        public virtual UserTicket CreateTicket(MembershipUser user)
        {
            int timeout = (60 
                        * (24 * 7));
            JToken jTimeout = DefaultSettings["TokenExpiration"];
            if (jTimeout != null)
            	timeout = (((int)(jTimeout)) * 60);
            string userData = String.Empty;
            OAuthHandler handler = OAuthHandlerFactory.GetActiveHandler();
            if (handler != null)
            	userData = ("OAUTH:" + handler.GetHandlerName());
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(timeout), false, userData);
            return new UserTicket(user, FormsAuthentication.Encrypt(ticket));
        }
        
        public virtual bool ValidateTicket(FormsAuthenticationTicket ticket)
        {
            return !(((ticket == null) || (ticket.Expired || String.IsNullOrEmpty(ticket.Name))));
        }
        
        public virtual void InvalidateTicket(FormsAuthenticationTicket ticket)
        {
        }
        
        public virtual bool UserLogin(string username, string password, bool createPersistentCookie)
        {
            if (Membership.ValidateUser(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, createPersistentCookie);
                return true;
            }
            else
            	return false;
        }
        
        public virtual void UserLogout()
        {
            FormsAuthentication.SignOut();
            if (ApplicationServices.IsSiteContentEnabled)
            {
                OAuthHandler handler = OAuthHandlerFactory.GetActiveHandler();
                if (handler != null)
                	handler.SignOut();
            }
        }
        
        public virtual string[] UserRoles()
        {
            return Roles.GetRolesForUser();
        }
        
        public virtual JObject UserThemes()
        {
            JObject lists = new JObject();
            JArray themes = new JArray();
            JArray accents = new JArray();
            lists["themes"] = themes;
            lists["accents"] = accents;
            string themesPath = HttpContext.Current.Server.MapPath("~/css/themes");
            foreach (string f in Directory.GetFiles(themesPath, "touch-theme.*.json"))
            {
                JObject theme = JObject.Parse(File.ReadAllText(f));
                JObject t = new JObject();
                t["name"] = theme["name"];
                t["color"] = theme["color"];
                themes.Add(t);
            }
            foreach (string f in Directory.GetFiles(themesPath, "touch-accent.*.json"))
            {
                JObject accent = JObject.Parse(File.ReadAllText(f));
                JObject a = new JObject();
                a["name"] = accent["name"];
                a["color"] = accent["color"];
                accents.Add(a);
            }
            return lists;
        }
        
        public virtual JObject UserSettings(Page p)
        {
            JObject settings = new JObject(DefaultSettings);
            if (settings["membership"] == null)
            	settings["membership"] = new JObject();
            string userKey = String.Empty;
            MembershipUser user = Membership.GetUser();
            if (user != null)
            {
                userKey = Convert.ToString(user.ProviderUserKey);
                if (!(String.IsNullOrEmpty(user.Comment)))
                {
                    Match m = Regex.Match(user.Comment, "\\bSource:\\s*\\b(?\'Value\'\\w+)\\b", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        OAuthHandler handler = OAuthHandlerFactory.Create(m.Groups["Value"].Value);
                        if (handler != null)
                        	settings["membership"]["profile"] = handler.GetUserProfile();
                    }
                }
            }
            settings["appInfo"] = string.Join("|", new string[] {
                        Name,
                        HttpContext.Current.User.Identity.Name,
                        userKey});
            settings["ui"]["theme"]["name"] = UserTheme;
            settings["ui"]["theme"]["accent"] = UserAccent;
            return settings;
        }
        
        public virtual string UserHomePageUrl()
        {
            return "~/Pages/Home.aspx";
        }
        
        public virtual string UserPictureString(MembershipUser user)
        {
            try
            {
                Image img = UserPictureImage(user);
                if (img == null)
                	img = UserPictureFromCMS(user);
                if (img != null)
                {
                    if ((img.Width > 80) || (img.Height > 80))
                    {
                        float scale = (((float)(img.Width)) / 80);
                        int height = ((int)((img.Height / scale)));
                        int width = 80;
                        if (img.Height < img.Width)
                        {
                            scale = (((float)(img.Height)) / 80);
                            height = 80;
                            width = ((int)((img.Width / scale)));
                        }
                        img = Blob.ResizeImage(img, width, height);
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        img.Save(stream, ImageFormat.Bmp);
                        byte[] bytes = stream.ToArray();
                        img.Dispose();
                        return ("data:image/raw;base64," + Convert.ToBase64String(bytes));
                    }
                }
            }
            catch (Exception )
            {
            }
            return String.Empty;
        }
        
        public virtual Image UserPictureImage(MembershipUser user)
        {
            string url = UserPictureUrl(user);
            if (!(String.IsNullOrEmpty(url)))
            {
                WebRequest request = WebRequest.Create(url);
                using (Stream stream = request.GetResponse().GetResponseStream())
                	using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        return ((Image)(new ImageConverter().ConvertFrom(ms.ToArray())));
                    }
            }
            else
            {
                url = UserPictureFilePath(user);
                if (!(String.IsNullOrEmpty(url)))
                	return Image.FromFile(url);
            }
            return null;
        }
        
        public virtual Image UserPictureFromCMS(MembershipUser user)
        {
            return null;
        }
        
        public virtual string UserPictureFilePath(MembershipUser user)
        {
            return null;
        }
        
        public virtual string UserPictureUrl(MembershipUser user)
        {
            return null;
        }
        
        public static ApplicationServices Create()
        {
            return new ApplicationServices();
        }
        
        public static bool UserIsAuthorizedToAccessResource(string path, string roles)
        {
            return !(Create().ResourceAuthorizationIsRequired(path, roles));
        }
        
        public virtual bool ResourceAuthorizationIsRequired(string path, string roles)
        {
            if (roles == null)
            	roles = String.Empty;
            else
            	roles = roles.Trim();
            bool requiresAuthorization = false;
            bool isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (String.IsNullOrEmpty(roles) && !(isAuthenticated))
            	requiresAuthorization = true;
            if (!(String.IsNullOrEmpty(roles)) && !((roles == "?")))
            	if (roles == "*")
                {
                    if (!(isAuthenticated))
                    	requiresAuthorization = true;
                }
                else
                	if (!(isAuthenticated) || !(DataControllerBase.UserIsInRole(roles)))
                    	requiresAuthorization = true;
            if (path == FormsAuthentication.LoginUrl)
            {
                requiresAuthorization = false;
                if (!(isAuthenticated) && (!((HttpContext.Current.Request.QueryString["_autoLogin"] == "false")) && (HttpContext.Current.Request.Cookies[".TOKEN"] == null)))
                {
                    OAuthHandler handler = OAuthHandlerFactory.CreateAutoLogin();
                    if (handler != null)
                    {
                        HttpContext.Current.Response.Cookies.Set(new HttpCookie(".PROVIDER", handler.GetHandlerName()));
                        requiresAuthorization = true;
                    }
                }
            }
            return requiresAuthorization;
        }
        
        public static void RegisterStandardMembershipAccounts()
        {
            // Confirm existence of schema version
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["LocalSqlServer"];
            if (css == null)
            	css = ConfigurationManager.ConnectionStrings["MyCompany"];
            if ((css != null) && (css.ProviderName == "System.Data.SqlClient"))
            {
                int count = 0;
                using (SqlText sql = new SqlText("select count(*) from dbo.aspnet_SchemaVersions where Feature in (\'common\', \'membe" +
                        "rship\', \'role manager\')", css.Name))
                	count = ((int)(sql.ExecuteScalar()));
                if (count == 0)
                {
                    using (SqlText sql = new SqlText("insert into dbo.aspnet_SchemaVersions values (\'common\', 1, 1)", css.Name))
                    	sql.ExecuteNonQuery();
                    using (SqlText sql = new SqlText("insert into dbo.aspnet_SchemaVersions values (\'membership\', 1, 1)", css.Name))
                    	sql.ExecuteNonQuery();
                    using (SqlText sql = new SqlText("insert into dbo.aspnet_SchemaVersions values (\'role manager\', 1, 1)", css.Name))
                    	sql.ExecuteNonQuery();
                }
            }
            MembershipUser admin = Membership.GetUser("admin");
            if ((admin != null) && admin.IsLockedOut)
            	admin.UnlockUser();
            MembershipUser user = Membership.GetUser("user");
            if ((user != null) && user.IsLockedOut)
            	user.UnlockUser();
            if (Membership.GetUser("admin") == null)
            {
                MembershipCreateStatus status;
                admin = Membership.CreateUser("admin", "admin123%", "admin@MyCompany.com", "ASP.NET", "Code OnTime", true, out status);
                user = Membership.CreateUser("user", "user123%", "user@MyCompany.com", "ASP.NET", "Code OnTime", true, out status);
                Roles.CreateRole("Administrators");
                Roles.CreateRole("Users");
                Roles.AddUserToRole(admin.UserName, "Users");
                Roles.AddUserToRole(user.UserName, "Users");
                Roles.AddUserToRole(admin.UserName, "Administrators");
            }
        }
        
        public static void RegisterCssLinks(Page p)
        {
            HtmlLink l = new HtmlLink();
            l.ID = "MyCompanyTheme";
            l.Attributes.Add("type", "text/css");
            l.Attributes.Add("rel", "stylesheet");
            p.Header.Controls.Add(((Control)(l)));
            ApplicationServices services = ApplicationServices.Current;
            string jqmCss = String.Format("jquery.mobile-{0}.min.css", ApplicationServices.JqmVersion);
            l.Href = ("~/css/sys/" + jqmCss);
            HtmlMeta meta = new HtmlMeta();
            meta.Attributes["name"] = "viewport";
            meta.Attributes["content"] = "width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no";
            p.Header.Controls.AddAt(0, meta);
            foreach (string stylesheet in services.EnumerateTouchUIStylesheets())
            {
                string cssName = Path.GetFileName(stylesheet);
                if (!(cssName.StartsWith("jquery.mobile")) && !(cssName.StartsWith("bootstrap")))
                {
                    HtmlLink cssLink = new HtmlLink();
                    cssLink.Href = String.Format("{0}?{1}", stylesheet.Replace('\\', '/'), ApplicationServices.Version);
                    if (cssName.StartsWith("touch-theme."))
                    {
                        Match themeVars = StylesheetGenerator.ThemeStylesheetRegex.Match(cssName);
                        cssLink.Href = String.Format("~/Theme.ashx?theme={0}&accent={1}&v={2}", themeVars.Groups["Theme"].Value, themeVars.Groups["Accent"].Value, ApplicationServices.Version);
                        cssLink.Attributes["class"] = "app-theme";
                    }
                    cssLink.Attributes["type"] = "text/css";
                    cssLink.Attributes["rel"] = "stylesheet";
                    p.Header.Controls.Add(cssLink);
                }
            }
            List<Control> removeList = new List<Control>();
            foreach (Control c2 in p.Header.Controls)
            	if (c2 is HtmlLink)
                {
                    l = ((HtmlLink)(c2));
                    if (l.Href.Contains("App_Themes/"))
                    	removeList.Add(l);
                }
            foreach (Control c2 in removeList)
            	p.Header.Controls.Remove(c2);
        }
        
        private void LoadTheme()
        {
            string theme = String.Empty;
            if (HttpContext.Current != null)
            {
                HttpCookie themeCookie = HttpContext.Current.Request.Cookies[(".COTTHEME" + BusinessRules.UserName)];
                if (themeCookie != null)
                	theme = themeCookie.Value;
            }
            if (!(String.IsNullOrEmpty(theme)) && theme.Contains('.'))
            {
                theme = theme.Replace(" ", String.Empty);
                string[] parts = theme.Split('.');
                _userTheme = parts[0];
                _userAccent = parts[1];
            }
            else
            {
                _userTheme = ((string)(DefaultSettings["ui"]["theme"]["name"]));
                _userAccent = ((string)(DefaultSettings["ui"]["theme"]["accent"]));
            }
        }
        
        protected virtual bool AllowTouchUIStylesheet(string name)
        {
            return !(Regex.IsMatch(name, "^(touch|bootstrap|jquery\\.mobile)"));
        }
        
        public virtual List<string> EnumerateTouchUIStylesheets()
        {
            List<string> stylesheets = new List<string>();
            string ext = ".min.css";
            if (!(EnableMinifiedCss))
            	ext = ".css";
            stylesheets.Add(string.Format("~\\css\\sys\\jquery.mobile-{0}{1}", ApplicationServices.JqmVersion, ext));
            stylesheets.Add(("~\\css\\daf\\touch" + ext));
            stylesheets.Add(("~\\css\\daf\\touch-charts" + ext));
            stylesheets.Add(("~\\css\\sys\\bootstrap" + ext));
            stylesheets.Add(String.Format("~\\appservices\\touch-theme.{0}.{1}.css", UserTheme, UserAccent));
            // enumerate custom css files
            List<string> customCss = ((List<string>)(HttpRuntime.Cache["IncludedCss"]));
            if (customCss == null)
            {
                customCss = new List<string>();
                string cssPath = Path.Combine(HttpRuntime.AppDomainAppPath, "css");
                CacheDependency dep = null;
                if (Directory.Exists(cssPath))
                {
                    dep = new FolderCacheDependency(cssPath, "*.css");
                    string ignorePath = Path.Combine(cssPath, "_ignore.txt");
                    Regex ignoreRegex = null;
                    if (File.Exists(ignorePath))
                    	ignoreRegex = BuildSearchPathRegex(File.ReadAllLines(ignorePath));
                    foreach (string filePath in Directory.EnumerateFiles(cssPath, "*.css", SearchOption.AllDirectories))
                    {
                        string css = Path.GetFileName(filePath);
                        string relativePath = ("~\\" + filePath.Substring(HttpRuntime.AppDomainAppPath.Length));
                        if (AllowTouchUIStylesheet(css) && ((ignoreRegex == null) || !(ignoreRegex.IsMatch(relativePath.Substring(2)))))
                        	if (!(css.EndsWith(".min.css")))
                            	customCss.Add(relativePath);
                            else
                            {
                                int index = customCss.IndexOf((css.Substring(0, (css.Length - 7)) + "css"));
                                if (index > -1)
                                	customCss[index] = relativePath;
                                else
                                	customCss.Add(relativePath);
                            }
                    }
                }
                HttpRuntime.Cache.Add("IncludedCss", customCss, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            stylesheets.AddRange(customCss);
            return stylesheets;
        }
        
        private static string DoReplaceCssUrl(Match m)
        {
            string header = m.Groups["Header"].Value;
            string name = m.Groups["Name"].Value;
            string symbol = m.Groups["Symbol"].Value;
            if (((name == "data") || name.StartsWith("http")) && (symbol == ":"))
            	return m.Value;
            if (name.StartsWith("../"))
            	name = name.Substring(3);
            return (header 
                        + ("../css/" 
                        + (name + symbol)));
        }
        
        public static string CombineTouchUIStylesheets(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpCachePolicy cache = response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByHeaders["User-Agent"] = true;
            cache.SetOmitVaryStar(true);
            cache.SetExpires(DateTime.Now.AddDays(365));
            cache.SetValidUntilExpires(true);
            cache.SetLastModifiedFromFileDependencies();
            // combine scripts
            string contentFramework = context.Request.QueryString["_cf"];
            bool includeBootstrap = (contentFramework == "bootstrap");
            StringBuilder sb = new StringBuilder();
            ApplicationServices services = Create();
            foreach (string stylesheet in services.EnumerateTouchUIStylesheets())
            {
                string cssName = Path.GetFileName(stylesheet);
                if (includeBootstrap || !(cssName.StartsWith("bootstrap")))
                	if (cssName.StartsWith("touch-theme."))
                    	sb.AppendLine(StylesheetGenerator.Compile(cssName));
                    else
                    {
                        string data = File.ReadAllText(HttpContext.Current.Server.MapPath(stylesheet));
                        data = CssUrlRegex.Replace(data, DoReplaceCssUrl);
                        if (!(data.Contains("@import url")))
                        	sb.AppendLine(data);
                        else
                        	sb.Insert(0, data);
                    }
            }
            return sb.ToString();
        }
        
        public virtual void ConfigureScripts(List<ScriptReference> scripts)
        {
            string jsPath = Path.Combine(HttpRuntime.AppDomainAppPath, "js");
            List<string> includedScripts = ((List<string>)(HttpRuntime.Cache["IncludedScripts"]));
            if (includedScripts == null)
            {
                includedScripts = new List<string>();
                CacheDependency dep = null;
                if (Directory.Exists(jsPath))
                {
                    dep = new FolderCacheDependency(jsPath, "*.js");
                    string ignorePath = Path.Combine(jsPath, "_ignore.txt");
                    Regex ignoreRegex = null;
                    if (File.Exists(ignorePath))
                    	ignoreRegex = BuildSearchPathRegex(File.ReadAllLines(ignorePath));
                    foreach (string file in Directory.EnumerateFiles(jsPath, "*.js", SearchOption.AllDirectories))
                    {
                        string relativeFile = file.Substring((jsPath.Length + 1));
                        if (((ignoreRegex == null) || !(ignoreRegex.IsMatch(relativeFile))) && !(DefaultExcludeScriptRegex.IsMatch(relativeFile)))
                        	includedScripts.Add(("~/" + file.Substring(HttpRuntime.AppDomainAppPath.Length).Replace("\\", "/")));
                    }
                    int i = 0;
                    while (i < includedScripts.Count)
                    {
                        string scriptName = includedScripts[i];
                        if (scriptName.EndsWith(".min.js"))
                        {
                            if (AquariumExtenderBase.EnableMinifiedScript)
                            	scriptName = (scriptName.Substring(0, (scriptName.Length - 7)) + ".js");
                            includedScripts.Remove(scriptName);
                        }
                        else
                        	i++;
                    }
                }
                HttpRuntime.Cache.Add("IncludedScripts", includedScripts, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            foreach (string file in includedScripts)
            	scripts.Add(AquariumExtenderBase.CreateScriptReference(file));
        }
        
        Regex BuildSearchPathRegex(string[] paths)
        {
            if (paths.Length == 0)
            	return null;
            StringBuilder sb = new StringBuilder();
            foreach (string path in paths)
            {
                if (sb.Length != 0)
                	sb.Append("|");
                sb.AppendFormat("({0})", Regex.Escape(path.Trim().Replace("/", "\\")).Replace("\\*", ".*"));
            }
            return new Regex(sb.ToString());
        }
        
        public static void CompressOutput(HttpContext context, string data)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!(String.IsNullOrEmpty(acceptEncoding)))
            	if (acceptEncoding.Contains("gzip"))
                {
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                    response.AppendHeader("Content-Encoding", "gzip");
                }
                else
                	if (acceptEncoding.Contains("deflate"))
                    {
                        response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                        response.AppendHeader("Content-Encoding", "deflate");
                    }
            byte[] output = Encoding.UTF8.GetBytes(data);
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("Content-Length", output.Length.ToString());
            response.OutputStream.Write(output, 0, output.Length);
            try
            {
                response.Flush();
            }
            catch (Exception )
            {
            }
        }
        
        public static void HandleServiceRequest(HttpContext context)
        {
            string methodName = context.Request.AppRelativeCurrentExecutionFilePath.ToLowerInvariant();
            if (methodName.StartsWith(AquariumExtenderBase.DefaultServicePath))
            	methodName = methodName.Substring((AquariumExtenderBase.DefaultServicePath.Length + 1));
            else
            	if (methodName.StartsWith(AquariumExtenderBase.AppServicePath))
                	methodName = methodName.Substring((AquariumExtenderBase.AppServicePath.Length + 1));
            if (String.IsNullOrEmpty(methodName))
            	throw new HttpException(400, "Method not specified.");
            ServiceRequestHandler handler = null;
            if (RequestHandlers.TryGetValue(methodName.ToLower(), out handler))
            {
                JObject args = RequestValidationService.ToJson(context);
                object result = null;
                if ((handler.AllowedMethods != null) && !(handler.AllowedMethods.Contains(context.Request.HttpMethod)))
                	throw new HttpException(405, "This HTTP Method is not allowed.");
                if (handler.RequiresAuthentication && !(context.Request.IsAuthenticated))
                	throw new HttpException(403, "Requires authentication.");
                try
                {
                    result = handler.HandleRequest(new DataControllerService(), args);
                }
                catch (ServiceRequestRedirectException rex)
                {
                    result = new JObject();
                    ((JObject)(result))["RedirectUrl"] = rex.RedirectUrl;
                }
                catch (Exception ex)
                {
                    result = handler.HandleException(args, ex);
                }
                if (result != null)
                {
                    context.Response.ContentType = "application/json; charset=utf-8";
                    string output = String.Format("{{\"d\":{0}}}", JsonConvert.SerializeObject(result));
                    ApplicationServices.CompressOutput(context, CompressViewPageJsonOutput(output));
                }
            }
            else
            	throw new HttpException(404, "Method not found.");
            context.Response.End();
        }
        
        public static string CompressViewPageJsonOutput(string output)
        {
            int startIndex = 0;
            int dataIndex = 0;
            int lastIndex = 0;
            int lastLength = output.Length;
            while (true)
            {
                startIndex = output.IndexOf("{\"Controller\":", lastIndex, StringComparison.Ordinal);
                dataIndex = output.IndexOf(",\"NewRow\":", lastIndex, StringComparison.Ordinal);
                if ((startIndex < 0) || (dataIndex < 0))
                	break;
                string metadata = (output.Substring(0, startIndex) + ViewPageCompressRegex.Replace(output.Substring(startIndex, (dataIndex - startIndex)), String.Empty));
                if (metadata.EndsWith(","))
                	metadata = metadata.Substring(0, (metadata.Length - 1));
                output = (ViewPageCompress2Regex.Replace(metadata, "}$1") + output.Substring(dataIndex));
                lastIndex = ((dataIndex + 10) 
                            - (lastLength - output.Length));
                lastLength = output.Length;
            }
            return output;
        }
        
        public static string ResolveClientUrl(string relativeUrl)
        {
            HttpRequest request = HttpContext.Current.Request;
            string root = (request.Url.Scheme 
                        + (Uri.SchemeDelimiter + request.Url.Host));
            if (!(request.Url.IsDefaultPort))
            	root = (root 
                            + (":" + Convert.ToString(request.Url.Port)));
            if (relativeUrl.StartsWith("~/"))
            	relativeUrl = relativeUrl.Substring(2);
            else
            	if (relativeUrl.StartsWith("/"))
                	relativeUrl = relativeUrl.Substring(1);
                else
                	relativeUrl = (request.Url.AbsolutePath 
                                + ("/" + relativeUrl));
            string appPath = request.ApplicationPath;
            if (!(appPath.EndsWith("/")))
            	appPath = (appPath + "/");
            string result = ((root + appPath) 
                        + relativeUrl);
            if (String.IsNullOrEmpty(relativeUrl))
            	result = result.Substring(0, (result.Length - 1));
            return result;
        }
        
        public virtual SortedDictionary<string, string> CorsConfiguration(HttpRequest request)
        {
            if (EnableCors)
            {
                SortedDictionary<string, string> list = new SortedDictionary<string, string>();
                string origin = request.Headers["Origin"];
                if (String.IsNullOrEmpty(origin))
                	origin = "*";
                list["Access-Control-Allow-Origin"] = origin;
                list["Access-Control-Allow-Methods"] = "GET,POST";
                list["Access-Control-Allow-Credentials"] = "true";
                list["Access-Control-Allow-Headers"] = "content-type,authorization";
                return list;
            }
            return null;
        }
        
        private static void EnsureJsonProperty(JObject ptr, string path, object defaultValue)
        {
            if (defaultValue == null)
            	defaultValue = String.Empty;
            string[] parts = path.Split('.');
            int counter = parts.Length;
            foreach (string part in parts)
            {
                counter--;
                if (ptr[part] == null)
                	if (counter != 0)
                    	ptr[part] = new JObject();
                    else
                    	ptr[part] = JToken.FromObject(defaultValue);
                if (counter != 0)
                	ptr = ((JObject)(ptr[part]));
            }
        }
        
        public static JToken TryGetJsonProperty(JObject ptr, string path)
        {
            string[] parts = path.Split('.');
            JToken temp = null;
            for (int i = 0; (i < (parts.Length - 1)); i++)
            {
                temp = ptr[parts[i]];
                if (temp != null)
                	ptr = ((JObject)(temp));
                else
                	return null;
            }
            return ptr[parts[(parts.Length - 1)]];
        }
    }
    
    public class AnonymousUserIdentity : IIdentity
    {
        
        string IIdentity.AuthenticationType
        {
            get
            {
                return "None";
            }
        }
        
        bool IIdentity.IsAuthenticated
        {
            get
            {
                return false;
            }
        }
        
        string IIdentity.Name
        {
            get
            {
                return String.Empty;
            }
        }
    }
    
    public partial class ApplicationSiteMapProvider : ApplicationSiteMapProviderBase
    {
    }
    
    public class ApplicationSiteMapProviderBase : System.Web.XmlSiteMapProvider
    {
        
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            string device = node["Device"];
            bool isTouchUI = ApplicationServices.IsTouchClient;
            if ((device == "touch") && !(isTouchUI))
            	return false;
            if ((device == "desktop") && isTouchUI)
            	return false;
            return base.IsAccessibleToUser(context, node);
        }
    }
    
    public partial class ApplicationSessionState : ApplicationSessionStateBase
    {
    }
    
    public class ApplicationSessionStateBase : SessionStateStoreProviderBase
    {
        
        public static ApplicationSessionStateBase Current;
        
        private SessionStateSection _config = null;
        
        private string _connectionStringName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _applicationName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _writeExceptionsToEventLog;
        
        public virtual string ApplicationName
        {
            get
            {
                return this._applicationName;
            }
            set
            {
                this._applicationName = value;
            }
        }
        
        public virtual bool WriteExceptionsToEventLog
        {
            get
            {
                return this._writeExceptionsToEventLog;
            }
            set
            {
                this._writeExceptionsToEventLog = value;
            }
        }
        
        public virtual void DeleteExpiredSessions()
        {
            using (SqlText cmd = CreateSqlText("DELETE FROM aspnet_Sessions WHERE Expires < @Expires"))
            {
                cmd.AddParameter("Expires", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }
        
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            	throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(name))
            	name = "ApplicationSessionState";
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Custom Session State Store Provider");
            }
            base.Initialize(name, config);
            _applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(ApplicationName);
            _config = ((SessionStateSection)(cfg.GetSection("system.web/sessionState")));
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (settings != null)
            	_connectionStringName = settings.Name;
            _writeExceptionsToEventLog = false;
            if (config["writeExceptionsToEventLog"] != null)
            {
                if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
                	_writeExceptionsToEventLog = true;
            }
            Current = this;
        }
        
        public override void Dispose()
        {
        }
        
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }
        
        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            byte[] sessItems = Serialize(((SessionStateItemCollection)(item.Items)));
            if (newItem)
            {
                DeleteExpiredSessions();
                using (SqlText cmd = CreateSqlText("INSERT INTO aspnet_Sessions (SessionId, ApplicationName, Created, Expires, LockDa" +
                        "te, LockId, Timeout, Locked, SessionItems, Flags) Values(@SessionId, @Applicatio" +
                        "nName, @Created, @Expires, @LockDate, @LockId , @Timeout, @Locked, @SessionItems" +
                        ", @Flags)"))
                {
                    cmd.AddParameter("SessionId", id);
                    cmd.AddParameter("Expires", DateTime.Now.AddMinutes(((double)(item.Timeout))));
                    cmd.AddParameter("Created", DateTime.Now);
                    cmd.AddParameter("LockDate", DateTime.Now);
                    cmd.AddParameter("LockId", 0);
                    cmd.AddParameter("Timeout", item.Timeout);
                    cmd.AddParameter("Locked", 0);
                    cmd.AddParameter("SessionItems", sessItems);
                    cmd.AddParameter("Flags", 0);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            	using (SqlText cmd = CreateSqlText("UPDATE aspnet_Sessions SET Expires = @Expires, SessionItems = @SessionItems, Lock" +
                        "ed = @Locked WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName" +
                        " AND LockId = @LockId"))
                {
                    cmd.AddParameter("SessionId", id);
                    cmd.AddParameter("Expires", DateTime.Now.AddMinutes(((double)(item.Timeout))));
                    cmd.AddParameter("SessionItems", sessItems);
                    cmd.AddParameter("Locked", 0);
                    cmd.AddParameter("LockId", lockId);
                    cmd.ExecuteNonQuery();
                }
        }
        
        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
        {
            return GetSessionStoreItem(false, context, id, out locked, out lockAge, out lockId, out actionFlags);
        }
        
        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
        {
            return GetSessionStoreItem(true, context, id, out locked, out lockAge, out lockId, out actionFlags);
        }
        
        private SessionStateStoreData GetSessionStoreItem(bool lockRecord, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
        {
            SessionStateStoreData item = null;
            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actionFlags = 0;
            DbDataReader reader = null;
            DateTime expires;
            byte[] serializedItems = null;
            bool foundRecord = false;
            bool deleteData = false;
            int timeout = 0;
            try
            {
                // Obtain a lock if possible. Ignore the record if it is expired.
                if (lockRecord)
                	using (SqlText cmd = CreateSqlText("UPDATE aspnet_Sessions SET Locked = @LockedTrue, LockDate = @LockDate WHERE Sessi" +
                            "onId = @SessionId AND ApplicationName = @ApplicationName AND Locked = @LockedFal" +
                            "se AND Expires > @Expires"))
                    {
                        cmd.AddParameter("LockedTrue", 1);
                        cmd.AddParameter("LockDate", DateTime.Now);
                        cmd.AddParameter("SessionId", id);
                        cmd.AddParameter("LockedFalse", 0);
                        cmd.AddParameter("Expires", DateTime.Now);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            // No record was updated because the record was locked or not found.
                            locked = true;
                        }
                        else
                        {
                            // The record was updated.
                            locked = false;
                        }
                    }
                // Retrieve the current session item information.
                using (SqlText cmd = CreateSqlText("SELECT Expires, SessionItems, LockId, LockDate, Flags, Timeout FROM aspnet_Sessio" +
                        "ns WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName"))
                {
                    cmd.AddParameter("SessionId", id);
                    // Retrieve session item data from the data source.
                    reader = cmd.ExecuteReader();
                    while (cmd.Read())
                    {
                        expires = reader.GetDateTime(0);
                        if (expires < DateTime.Now)
                        {
                            // The record was expired. Mark it as not locked.
                            locked = false;
                            // The session was expired. Mark the data for deletion.
                            deleteData = true;
                        }
                        else
                        	foundRecord = true;
                        if (reader[1] == DBNull.Value)
                        	serializedItems = new byte[0];
                        else
                        	serializedItems = ((byte[])(reader[1]));
                        lockId = reader.GetInt32(2);
                        lockAge = DateTime.Now.Subtract(reader.GetDateTime(3));
                        actionFlags = ((SessionStateActions)(reader.GetInt32(4)));
                        timeout = reader.GetInt32(5);
                    }
                    reader.Close();
                }
                // If the return session item is expired, delete the record from the data source.
                if (deleteData)
                	using (SqlText cmd = CreateSqlText("DELETE FROM aspnet_Sessions WHERE SessionId = @SessionId AND ApplicationName = @A" +
                            "pplicationName"))
                    {
                        cmd.AddParameter("SessionId", id);
                        cmd.ExecuteNonQuery();
                    }
                // The record was not found. Ensure that locked is false.
                if (!(foundRecord))
                	locked = false;
                // 
                //                        If the record was found and you obtain a lock, then set
                //                        the lockId, clear the actionFlags, and create the SessionStateSToreItem to return.
                //                      
                if (foundRecord && !(locked))
                {
                    lockId = (((int)(lockId)) + 1);
                    using (SqlText cmd = CreateSqlText("UPDATE aspnet_Sessions SET LockId = @LockId, Flags = 0 WHERE SessionId = @Session" +
                            "Id AND ApplicationName = @ApplicationName"))
                    {
                        cmd.AddParameter("lockId", lockId);
                        cmd.AddParameter("SessionId", id);
                        cmd.ExecuteNonQuery();
                    }
                    // 
                    //                            If the actionFlags parameter is not InitializeItem
                    //                            deserialize the stored SessionStateItemCollection.
                    //                          
                    if (actionFlags == SessionStateActions.InitializeItem)
                    	item = CreateNewStoreData(context, Convert.ToInt32(_config.Timeout.TotalMinutes));
                    else
                    	item = Deserialize(context, serializedItems, timeout);
                }
            }
            finally
            {
                if (reader != null)
                	reader.Close();
            }
            return item;
        }
        
        private byte[] Serialize(SessionStateItemCollection items)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            if (items != null)
            	items.Serialize(writer);
            writer.Close();
            return ms.ToArray();
        }
        
        private SessionStateStoreData Deserialize(HttpContext context, byte[] serializedItems, int timeout)
        {
            MemoryStream ms = new MemoryStream(serializedItems);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            if (ms.Length > 0)
            {
                BinaryReader reader = new BinaryReader(ms);
                sessionItems = SessionStateItemCollection.Deserialize(reader);
            }
            return new SessionStateStoreData(sessionItems, SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }
        
        public SqlText CreateSqlText(string sql)
        {
            SqlText cmd = new SqlText(sql, _connectionStringName);
            cmd.Command.CommandText = cmd.Command.CommandText.Replace("@", cmd.ParameterMarker);
            if (cmd.Command.CommandText.Contains((cmd.ParameterMarker + "ApplicationName")))
            	cmd.AssignParameter("ApplicationName", ApplicationName);
            cmd.Name = "MyCompany Session State Provider";
            cmd.WriteExceptionsToEventLog = _writeExceptionsToEventLog;
            return cmd;
        }
        
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            using (SqlText cmd = CreateSqlText("UPDATE aspnet_Sessions SET Locked = 0, Expires = @Expires WHERE SessionId = @Sess" +
                    "ionId AND ApplicationName = @ApplicationName AND LockId = @LockId"))
            {
                cmd.AddParameter("Expires", DateTime.Now.AddMinutes(_config.Timeout.TotalMinutes));
                cmd.AddParameter("SessionId", id);
                cmd.AddParameter("LockId", lockId);
                cmd.ExecuteNonQuery();
            }
        }
        
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            using (SqlText cmd = CreateSqlText("DELETE FROM aspnet_Sessions WHERE SessionId = @SessionId AND ApplicationName = @A" +
                    "pplicationName AND LockId = @LockId"))
            {
                cmd.AddParameter("SessionId", id);
                cmd.AddParameter("LockId", lockId);
                cmd.ExecuteNonQuery();
            }
        }
        
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            using (SqlText cmd = CreateSqlText("INSERT INTO aspnet_Sessions (SessionId, ApplicationName, Created, Expires, LockDa" +
                    "te, LockId, Timeout, Locked, Flags) Values(@SessionId, @ApplicationName, @Create" +
                    "d, @Expires, @LockDate, @LockId, @Timeout, @Locked, @Flags)"))
            {
                cmd.AddParameter("SessionId", id);
                cmd.AddParameter("Created", DateTime.Now);
                cmd.AddParameter("Expires", DateTime.Now.AddMinutes(((double)(timeout))));
                cmd.AddParameter("LockDate", DateTime.Now);
                cmd.AddParameter("LockId", 0);
                cmd.AddParameter("Timeout", timeout);
                cmd.AddParameter("Locked", 0);
                cmd.AddParameter("Flags", 1);
                cmd.ExecuteNonQuery();
            }
        }
        
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }
        
        public override void ResetItemTimeout(HttpContext context, string id)
        {
            using (SqlText cmd = CreateSqlText("UPDATE aspnet_Sessions SET Expires = @Expires WHERE SessionId = @SessionId AND Ap" +
                    "plicationName = @ApplicationName"))
            {
                cmd.AddParameter("Expires", DateTime.Now.AddMinutes(_config.Timeout.TotalMinutes));
                cmd.AddParameter("SessionId", id);
                cmd.ExecuteNonQuery();
            }
        }
        
        public override void InitializeRequest(HttpContext context)
        {
        }
        
        public override void EndRequest(HttpContext context)
        {
        }
    }
    
    public partial class PlaceholderHandler : PlaceholderHandlerBase
    {
    }
    
    public class PlaceholderHandlerBase : IHttpHandler
    {
        
        private static Regex _imageSizeRegex = new Regex("((?\'background\'[a-zA-Z0-9]+?)-((?\'textcolor\'[a-zA-Z0-9]+?)-)?)?(?\'width\'[0-9]+?)(" +
                "x(?\'height\'[0-9]*))?\\.[a-zA-Z][a-zA-Z][a-zA-Z]");
        
        bool IHttpHandler.IsReusable
        {
            get
            {
                return true;
            }
        }
        
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            // Get file name
            RouteValueDictionary routeValues = context.Request.RequestContext.RouteData.Values;
            string fileName = ((string)(routeValues["FileName"]));
            // Get extension
            string ext = Path.GetExtension(fileName);
            ImageFormat format = ImageFormat.Png;
            string contentType = "image/png";
            if (ext == ".jpg")
            {
                format = ImageFormat.Jpeg;
                contentType = "image/jpg";
            }
            else
            	if (ext == ".gif")
                {
                    format = ImageFormat.Gif;
                    contentType = "image/jpg";
                }
            // get width and height
            Match regexMatch = _imageSizeRegex.Matches(fileName)[0];
            Capture widthCapture = regexMatch.Groups["width"];
            int width = 500;
            if (widthCapture.Length != 0)
            	width = Convert.ToInt32(widthCapture.Value);
            if (width == 0)
            	width = 500;
            if (width > 4096)
            	width = 4096;
            Capture heightCapture = regexMatch.Groups["height"];
            int height = width;
            if (heightCapture.Length != 0)
            	height = Convert.ToInt32(heightCapture.Value);
            if (height == 0)
            	height = 500;
            if (height > 4096)
            	height = 4096;
            // Get background and text colors
            Color background = GetColor(regexMatch.Groups["background"], Color.LightGray);
            Color textColor = GetColor(regexMatch.Groups["textcolor"], Color.Black);
            int fontSize = ((width + height) 
                        / 50);
            if (fontSize < 10)
            	fontSize = 10;
            Font font = new Font(FontFamily.GenericSansSerif, fontSize);
            // Get text
            string text = context.Request.QueryString["text"];
            if (String.IsNullOrEmpty(text))
            	text = string.Format("{0} x {1}", width, height);
            // Get position for text
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                Graphics textDrawing = Graphics.FromImage(img);
                textSize = textDrawing.MeasureString(text, font);
            }
            // Draw the image
            using (Image image = new Bitmap(width, height))
            {
                Graphics drawing = Graphics.FromImage(image);
                drawing.Clear(background);
                using (Brush textBrush = new SolidBrush(textColor))
                	drawing.DrawString(text, font, textBrush, ((width - textSize.Width) 
                                    / 2), ((height - textSize.Height) 
                                    / 2));
                drawing.Save();
                drawing.Dispose();
                // Return image
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, format);
                    HttpCachePolicy cache = context.Response.Cache;
                    cache.SetCacheability(HttpCacheability.Public);
                    cache.SetOmitVaryStar(true);
                    cache.SetExpires(DateTime.Now.AddDays(365));
                    cache.SetValidUntilExpires(true);
                    cache.SetLastModifiedFromFileDependencies();
                    context.Response.ContentType = contentType;
                    context.Response.AddHeader("Content-Length", Convert.ToString(stream.Length));
                    context.Response.AddHeader("File-Name", fileName);
                    context.Response.BinaryWrite(stream.ToArray());
                    context.Response.OutputStream.Flush();
                }
            }
        }
        
        private static Color GetColor(Capture colorName, Color defaultColor)
        {
            try
            {
                if (colorName.Length > 0)
                {
                    string s = colorName.Value;
                    if (Regex.IsMatch(s, "^[0-9abcdef]{3,6}$"))
                    	s = ("#" + s);
                    return ColorTranslator.FromHtml(s);
                }
            }
            catch (Exception )
            {
            }
            return defaultColor;
        }
    }
    
    public class GenericRoute : IRouteHandler
    {
        
        private IHttpHandler _handler;
        
        public GenericRoute(IHttpHandler handler)
        {
            _handler = handler;
        }
        
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext context)
        {
            return _handler;
        }
        
        public static void Map(RouteCollection routes, IHttpHandler handler, string url)
        {
            Route r = new Route(url, new GenericRoute(handler));
            r.Defaults = new RouteValueDictionary();
            r.Constraints = new RouteValueDictionary();
            routes.Add(r);
        }
    }
    
    public class SaasConfiguration
    {
        
        private string _config;
        
        private string _clientId;
        
        private string _clientSecret;
        
        private string _redirectUri;
        
        private string _accessToken;
        
        private string _refreshToken;
        
        public SaasConfiguration(string config)
        {
            _config = (("\n" + config) 
                        + "\n");
        }
        
        public virtual string ClientId
        {
            get
            {
                if (String.IsNullOrEmpty(_clientId))
                	_clientId = this["Client Id"];
                return _clientId;
            }
        }
        
        public virtual string ClientSecret
        {
            get
            {
                if (String.IsNullOrEmpty(_clientSecret))
                	_clientSecret = this["Client Secret"];
                return _clientSecret;
            }
        }
        
        public virtual string RedirectUri
        {
            get
            {
                if (HttpContext.Current.Request.IsLocal && String.IsNullOrEmpty(_redirectUri))
                	_redirectUri = this["Local Redirect Uri"];
                if (String.IsNullOrEmpty(_redirectUri))
                	_redirectUri = this["Redirect Uri"];
                return _redirectUri;
            }
        }
        
        public virtual string AccessToken
        {
            get
            {
                if (String.IsNullOrEmpty(_accessToken))
                	_accessToken = this["Access Token"];
                return _accessToken;
            }
            set
            {
                _accessToken = value;
                this["Access Token"] = value;
            }
        }
        
        public virtual string RefreshToken
        {
            get
            {
                if (String.IsNullOrEmpty(_refreshToken))
                	_refreshToken = this["Refresh Token"];
                return _refreshToken;
            }
            set
            {
                _refreshToken = value;
                this["Refresh Token"] = value;
            }
        }
        
        public virtual string this[string property]
        {
            get
            {
                if (String.IsNullOrEmpty(_config))
                	return String.Empty;
                Match m = Regex.Match(_config, (("\\n(" + property) 
                                + ")\\:\\s*?\\n?(?\'Value\'[^\\s\\n].+?)\\n"), RegexOptions.IgnoreCase);
                if (m.Success)
                	return m.Groups["Value"].Value.Trim();
                return String.Empty;
            }
            set
            {
                if (!(String.IsNullOrEmpty(_config)))
                {
                    string oldValue = this[property];
                    if (!(String.IsNullOrEmpty(oldValue)))
                    	_config = _config.Replace(oldValue, value);
                    else
                    	_config = (_config 
                                    + ((Environment.NewLine + property) 
                                    + (": " + value)));
                }
            }
        }
        
        public override string ToString()
        {
            return _config;
        }
    }
    
    public abstract class OAuthHandler
    {
        
        public string StartPage;
        
        private bool _refreshedToken = false;
        
        private string _clientUri;
        
        private SaasConfiguration _config = null;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _tokens;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _storeToken;
        
        public virtual string ClientUri
        {
            get
            {
                if (String.IsNullOrEmpty(_clientUri) && ApplicationServices.IsSiteContentEnabled)
                {
                    _clientUri = Config["Client Uri"];
                    if (!(_clientUri.StartsWith("http")))
                    	_clientUri = ("https://" + _clientUri);
                }
                return _clientUri;
            }
        }
        
        protected virtual SaasConfiguration Config
        {
            get
            {
                return _config;
            }
        }
        
        protected virtual string[] Tokens
        {
            get
            {
                return this._tokens;
            }
            set
            {
                this._tokens = value;
            }
        }
        
        protected virtual bool StoreToken
        {
            get
            {
                return this._storeToken;
            }
            set
            {
                this._storeToken = value;
            }
        }
        
        protected virtual string Scope
        {
            get
            {
                return String.Empty;
            }
        }
        
        public virtual void ProcessRequest(HttpContext context)
        {
            try
            {
                ApplicationServices services = ApplicationServices.Create();
                StartPage = context.Request.QueryString["start"];
                if (String.IsNullOrEmpty(StartPage))
                	StartPage = services.UserHomePageUrl();
                string state = context.Request.QueryString["state"];
                if (!(String.IsNullOrEmpty(state)))
                	SetState(state);
                RestoreSession(context);
                if (Config == null)
                	throw new Exception("Provider not found.");
                else
                {
                    string code = GetAuthCode(context.Request);
                    if (String.IsNullOrEmpty(code))
                    {
                        string error = context.Request.QueryString["error"];
                        if (!(String.IsNullOrEmpty(error)))
                        	throw new Exception(error);
                        else
                        	RequestAuthorizationCode();
                    }
                    else
                    {
                        Tokens = GetAccessTokens(code, false);
                        if (Tokens == null)
                        	context.Response.StatusCode = 401;
                        else
                        {
                            if (StoreToken)
                            	StoreTokens(Tokens);
                            else
                            {
                                MembershipUser user = SyncUser();
                                if (user == null)
                                	throw new Exception("No user found.");
                                SetSession(context, user);
                            }
                            RedirectToStartPage(context);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(context, ex);
            }
        }
        
        public virtual void SetSession(HttpContext context, MembershipUser user)
        {
            ApplicationServices services = ApplicationServices.Current;
            // logout current user
            HttpCookie auth = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (auth != null)
            {
                FormsAuthenticationTicket oldTicket = FormsAuthentication.Decrypt(auth.Value);
                if (oldTicket.Name != user.UserName)
                	services.UserLogout();
            }
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, user.UserName, DateTime.Now, DateTime.Now.AddHours(12), false, ("OAUTH:" + GetHandlerName()));
            string encrypted = FormsAuthentication.Encrypt(ticket);
            JToken accountManagerEnabled = ApplicationServices.TryGetJsonProperty(services.DefaultSettings, "membership.accountManager.enabled");
            if ((accountManagerEnabled == null) || accountManagerEnabled.Value<bool>())
            {
                // client token login
                HttpCookie cookie = new HttpCookie(".TOKEN", encrypted);
                cookie.Expires = System.DateTime.Now.AddMinutes(5);
                context.Response.SetCookie(cookie);
            }
            else
            {
                // server login
                services.AuthenticateUser(user.UserName, ("token:" + encrypted), false);
            }
            context.Response.Cookies.Set(new HttpCookie(".PROVIDER", GetHandlerName()));
        }
        
        public virtual void RestoreSession(HttpContext context)
        {
            if (context.Request.QueryString["storeToken"] == "true")
            	StoreToken = true;
        }
        
        protected virtual string[] GetAccessTokens(string code, bool refresh)
        {
            WebRequest request = GetAccessTokenRequest(code, refresh);
            WebResponse response = request.GetResponse();
            string json = String.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            	json = sr.ReadToEnd();
            if (!(HttpContext.Current.IsCustomErrorEnabled) && (String.IsNullOrEmpty(json) || !((json[0] == '{'))))
            	throw new Exception(("Error fetching access tokens. Response: " + json));
            JObject responseObj = JObject.Parse(json);
            string error = ((string)(responseObj["error"]));
            if (!(String.IsNullOrEmpty(error)))
            	throw new Exception(error);
            return new string[] {
                    ((string)(responseObj["access_token"])),
                    ((string)(responseObj["refresh_token"]))};
        }
        
        protected virtual void StoreTokens(string[] tokens)
        {
        }
        
        protected virtual string GetAuthCode(HttpRequest request)
        {
            return request.QueryString["code"];
        }
        
        public virtual JObject Query(string method, bool useSystemToken)
        {
            JObject result = null;
            try
            {
                string token = Tokens[0];
                if (useSystemToken)
                	token = Config.AccessToken;
                if (String.IsNullOrEmpty(token))
                	throw new Exception("No token for request.");
                WebRequest request = GetQueryRequest(method, token);
                WebResponse response = request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                	result = JObject.Parse(sr.ReadToEnd());
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = ((HttpWebResponse)(ex.Response));
                    if ((response.StatusCode == HttpStatusCode.Unauthorized) && !(_refreshedToken))
                    {
                        _refreshedToken = true;
                        if (!(RefreshTokens(useSystemToken)))
                        	throw new Exception("Token expired.");
                        else
                        	result = Query(method, useSystemToken);
                    }
                    else
                    	if (response.StatusCode == HttpStatusCode.Forbidden)
                        	throw new Exception("Insufficient permissions.");
                }
            }
            return result;
        }
        
        protected virtual bool RefreshTokens(bool useSystemToken)
        {
            string refresh = Tokens[1];
            if (useSystemToken)
            	refresh = Config.RefreshToken;
            if (!(String.IsNullOrEmpty(refresh)))
            {
                Tokens = GetAccessTokens(refresh, true);
                if (Tokens != null)
                {
                    if (useSystemToken)
                    	StoreTokens(Tokens);
                    return true;
                }
            }
            return false;
        }
        
        public virtual MembershipUser SyncUser()
        {
            string username = GetUserName();
            MembershipUser user = Membership.GetUser(username);
            if ((user == null) && (Config["Sync User"] == "true"))
            {
                // create user
                string comment = ("Source: " + GetHandlerName());
                MembershipCreateStatus status;
                Guid pw = Guid.NewGuid();
                user = Membership.CreateUser(username, pw.ToString(), username, comment, pw.ToString(), true, out status);
                if (status != MembershipCreateStatus.Success)
                	throw new Exception(status.ToString());
                user.Comment = comment;
                Membership.UpdateUser(user);
                Roles.AddUserToRoles(user.UserName, GetDefaultUserRoles(user));
            }
            if (user != null)
            {
                string newEmail = GetUserEmail(user);
                if (!(String.IsNullOrEmpty(newEmail)) && newEmail != user.Email)
                {
                    user.Email = newEmail;
                    Membership.UpdateUser(user);
                }
                SetUserAvatar(user);
                if (Config["Sync Roles"] == "true")
                {
                    // verify roles
                    List<string> roleList = GetUserRoles(user);
                    foreach (string role in roleList)
                    	if (!(Roles.IsUserInRole(user.UserName, role)))
                        {
                            if (!(Roles.RoleExists(role)))
                            	Roles.CreateRole(role);
                            Roles.AddUserToRole(user.UserName, role);
                        }
                    List<string> existingRoles = new List<string>(Roles.GetRolesForUser(user.UserName));
                    foreach (string oldRole in existingRoles)
                    	if (!(roleList.Contains(oldRole)))
                        	Roles.RemoveUserFromRole(user.UserName, oldRole);
                }
            }
            return user;
        }
        
        public abstract string GetUserName();
        
        public virtual string GetUserEmail(MembershipUser user)
        {
            return user.Email;
        }
        
        public virtual void SetUserAvatar(MembershipUser user)
        {
        }
        
        public virtual string GetUserImageUrl(MembershipUser user)
        {
            return null;
        }
        
        public virtual string[] GetDefaultUserRoles(MembershipUser user)
        {
            return new string[] {
                    "Users"};
        }
        
        public virtual List<string> GetUserRoles(MembershipUser user)
        {
            List<string> roleList = new List<string>();
            roleList.Add("Users");
            return roleList;
        }
        
        public virtual string GetUserProfile()
        {
            return "logout";
        }
        
        public virtual string GetState()
        {
            string state = ("start=" + StartPage);
            if (StoreToken)
            	state = (state + "|storeToken=true");
            return state;
        }
        
        public virtual void SetState(string state)
        {
            foreach (string part in state.Split('|'))
            {
                string[] ps = part.Split('=');
                if (ps[0] == "start")
                	StartPage = ps[1];
                else
                	if (ps[0] == "storeToken")
                    	StoreToken = ((ps[1] == "true") && Roles.IsUserInRole("Administrators"));
            }
        }
        
        public virtual void RedirectToLoginPage()
        {
            RequestAuthorizationCode();
        }
        
        public virtual void RedirectToStartPage(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            	context.Response.Redirect(StartPage);
            else
            	context.Response.Redirect(((ApplicationServices.Current.UserHomePageUrl() + "?ReturnUrl=") 
                                + HttpUtility.UrlEncode(ApplicationServices.ResolveClientUrl(StartPage))));
        }
        
        public virtual bool AuthenticateTicket(MembershipUser user)
        {
            return true;
        }
        
        public virtual void SignOut()
        {
        }
        
        protected virtual void HandleException(HttpContext context, Exception ex)
        {
            while (ex.InnerException != null)
            	ex = ex.InnerException;
            ServiceRequestError error = new ServiceRequestError();
            error.Message = ex.Message;
            error.ExceptionType = ex.GetType().ToString();
            if (!(context.IsCustomErrorEnabled))
            	error.StackTrace = ex.StackTrace;
            context.Server.ClearError();
            context.Response.TrySkipIisCustomErrors = true;
            context.Response.ContentType = "application/json";
            context.Response.Clear();
            context.Response.Write(JsonConvert.SerializeObject(error));
        }
        
        public abstract string GetHandlerName();
        
        public abstract void RequestAuthorizationCode();
        
        protected abstract WebRequest GetAccessTokenRequest(string code, bool refresh);
        
        protected abstract WebRequest GetQueryRequest(string method, string token);
    }
    
    public partial class OAuthHandlerFactory : OAuthHandlerFactoryBase
    {
    }
    
    public class OAuthHandlerFactoryBase
    {
        
        public static SortedDictionary<string, Type> Handlers = new SortedDictionary<string, Type>();
        
        public static OAuthHandler Create(string service)
        {
            return new OAuthHandlerFactory().GetHandler(service);
        }
        
        public static OAuthHandler GetActiveHandler()
        {
            HttpCookie saas = HttpContext.Current.Request.Cookies[".PROVIDER"];
            if ((saas != null) && (saas.Value != null))
            	return OAuthHandlerFactory.Create(saas.Value);
            return null;
        }
        
        public virtual OAuthHandler GetHandler(string service)
        {
            Type t = null;
            if (Handlers.TryGetValue(service.ToLower(), out t))
            	return ((OAuthHandler)(Activator.CreateInstance(t)));
            return null;
        }
        
        public static OAuthHandler CreateAutoLogin()
        {
            return new OAuthHandlerFactory().GetAutoLoginHandler();
        }
        
        public virtual OAuthHandler GetAutoLoginHandler()
        {
            return null;
        }
    }
    
    public partial class DnnOAuthHandler : DnnOAuthHandlerBase
    {
    }
    
    public partial class DnnOAuthHandlerBase : OAuthHandler
    {
        
        private string _showNavigation;
        
        private JObject _userInfo;
        
        protected override string Scope
        {
            get
            {
                string sc = Config["Scope"];
                string tokens = Config["Tokens"];
                if (!(String.IsNullOrEmpty(tokens)))
                	sc = (sc 
                                + (" token:" + string.Join(" token:", tokens.Split(' '))));
                return sc;
            }
        }
        
        public override string GetHandlerName()
        {
            return "DNN";
        }
        
        public override void RequestAuthorizationCode()
        {
            string authUrl = String.Format("{0}?response_type=code&client_id={1}&redirect_uri={2}&state={3}", ClientUri, Config.ClientId, Config.RedirectUri, Uri.EscapeDataString(GetState()));
            if (!(String.IsNullOrEmpty(Scope)))
            	authUrl = (authUrl 
                            + ("&scope=" + Uri.EscapeDataString(Scope)));
            string username = HttpContext.Current.Request.QueryString["username"];
            if (!(String.IsNullOrEmpty(username)))
            	authUrl = (authUrl 
                            + ("&username=" + username));
            HttpContext.Current.Response.Redirect(authUrl);
        }
        
        protected override WebRequest GetAccessTokenRequest(string code, bool refresh)
        {
            WebRequest request = WebRequest.Create(ClientUri);
            request.Method = "POST";
            string codeType = "code";
            if (refresh)
            	codeType = "access_token";
            string body = String.Format("{0}={1}&client_id={2}&client_secret={3}&redirect_uri={4}&grant_type=authorization" +
                    "_code", codeType, code, Config.ClientId, Config.ClientSecret, Uri.EscapeDataString(Config.RedirectUri));
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            using (Stream stream = request.GetRequestStream())
            	stream.Write(bodyBytes, 0, bodyBytes.Length);
            return request;
        }
        
        protected override WebRequest GetQueryRequest(string method, string token)
        {
            WebRequest request = WebRequest.Create((ClientUri 
                            + ("?method=" + method)));
            request.Headers[HttpRequestHeader.Authorization] = ("Bearer " + token);
            return request;
        }
        
        public override string GetState()
        {
            return (base.GetState() 
                        + ("|showNavigation=" + HttpContext.Current.Request.QueryString["showNavigation"]));
        }
        
        public override void SetState(string state)
        {
            base.SetState(state);
            foreach (string part in state.Split('|'))
            {
                string[] ps = part.Split('=');
                if (ps[0] == "showNavigation")
                	_showNavigation = ps[1];
            }
        }
        
        public override void RestoreSession(HttpContext context)
        {
            if (String.IsNullOrEmpty(_showNavigation))
            	_showNavigation = context.Request.QueryString["showNavigation"];
            string session = context.Request.QueryString["session"];
            if (!(String.IsNullOrEmpty(session)) && (session == "new"))
            	ApplicationServices.Current.UserLogout();
            else
            {
                base.RestoreSession(context);
                if (!(StoreToken) && context.User.Identity.IsAuthenticated)
                	RedirectToStartPage(context);
            }
        }
        
        public override void RedirectToStartPage(HttpContext context)
        {
            string connector = "?";
            if (StartPage.Contains("?"))
            	connector = "&";
            StartPage = (StartPage 
                        + (connector 
                        + ("_showNavigation=" + _showNavigation)));
            base.RedirectToStartPage(context);
        }
        
        public override string GetUserName()
        {
            return ((string)(_userInfo["UserName"]));
        }
        
        public override string GetUserEmail(MembershipUser user)
        {
            return ((string)(_userInfo["UserEmail"]));
        }
        
        public override List<string> GetUserRoles(MembershipUser user)
        {
            List<string> roles = base.GetUserRoles(user);
            foreach (JToken r in _userInfo.Value<JArray>("Roles"))
            	roles.Add(r.ToString());
            return roles;
        }
        
        public override MembershipUser SyncUser()
        {
            _userInfo = Query("me", false);
            MembershipUser user = base.SyncUser();
            SiteContentFile.WriteJson(String.Format("sys/users/{0}.json", user.UserName), ((JObject)(_userInfo["Tokens"])));
            return user;
        }
        
        public override string GetUserImageUrl(MembershipUser user)
        {
            return String.Format("{0}/DnnImageHandler.ashx?mode=profilepic&userId={1}&h=80&w=80", ClientUri, Convert.ToInt32(_userInfo["UserID"]));
        }
        
        public override void SignOut()
        {
            string url = ApplicationServices.ResolveClientUrl(ApplicationServices.Current.UserHomePageUrl());
            ServiceRequestHandler.Redirect(String.Format("{0}?_logout=true&client_id={1}&redirect_uri={2}", ClientUri, Config.ClientId, url));
        }
    }
    
    public class UserTicket
    {
        
        public string UserName;
        
        public string Email;
        
        public string Token;
        
        public string Picture;
        
        public Dictionary<string, string> Claims = new Dictionary<string, string>();
        
        public UserTicket()
        {
        }
        
        public UserTicket(MembershipUser user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Picture = ApplicationServices.Create().UserPictureString(user);
        }
        
        public UserTicket(MembershipUser user, string token) : 
                this(user)
        {
            this.Token = token;
        }
    }
    
    public class ManifestFile
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _name;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _path;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _mD5;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _length;
        
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
        
        public string Path
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }
        
        public string MD5
        {
            get
            {
                return this._mD5;
            }
            set
            {
                this._mD5 = value;
            }
        }
        
        public int Length
        {
            get
            {
                return this._length;
            }
            set
            {
                this._length = value;
            }
        }
        
        public static ManifestFile FromPath(string relativePath)
        {
            ManifestFile f = new ManifestFile();
            if (relativePath.Contains("?"))
            	relativePath = relativePath.Substring(0, relativePath.IndexOf("?"));
            f.Path = relativePath.Replace('\\', '/').Replace("~/", String.Empty);
            f.Name = System.IO.Path.GetFileName(f.Path);
            byte[] fileBytes = File.ReadAllBytes(HttpContext.Current.Server.MapPath(("~/" + f.Path)));
            f.MD5 = ComputeHash(fileBytes);
            f.Length = fileBytes.Length;
            return f;
        }
        
        public static ManifestFile FromResource(string resourceName)
        {
            ManifestFile file = new ManifestFile();
            file.Name = resourceName;
            file.Path = ("_resources/" + resourceName);
            using (Stream s = ControllerConfigurationUtility.GetResourceStream(resourceName))
            	using (MemoryStream ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    file.MD5 = ComputeHash(ms.ToArray());
                    file.Length = Convert.ToInt32(ms.Length);
                }
            return file;
        }
        
        public static ManifestFile GetConfig(string config)
        {
            byte[] configBytes = Encoding.UTF8.GetBytes(config);
            ManifestFile configFile = new ManifestFile();
            configFile.Path = "js/host/config.js";
            configFile.Name = "config.js";
            configFile.Length = configBytes.Length;
            configFile.MD5 = ComputeHash(configBytes);
            return configFile;
        }
        
        public static string ComputeHash(byte[] data)
        {
            MD5 prov = new MD5CryptoServiceProvider();
            byte[] hashData = prov.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; (i < hashData.Length); i++)
            	sb.Append(hashData[i].ToString("x2"));
            return sb.ToString();
        }
    }
    
    public class StylesheetGenerator
    {
        
        private string _template;
        
        private JObject _theme;
        
        private JObject _accent;
        
        private SortedDictionary<string, string> _themeVariables = new SortedDictionary<string, string>();
        
        public static Regex ThemeStylesheetRegex = new Regex("^touch-theme\\.(?\'Theme\'\\w+)\\.((?\'Accent\'\\w+)\\.)?css$");
        
        public static Regex ThemeVariableRegex = new Regex("(?\'Item\'(?\'Before\'\\w+:\\s*)\\/\\*\\s*(?\'Name\'(@[\\w\\.]+(,\\s*)?)+)\\s*\\*\\/(?\'Value\'.+?))" +
                "(?\'After\'(!important)?;\\s*)$", RegexOptions.Multiline);
        
        public StylesheetGenerator(string theme, string accent)
        {
            string touchPath = HttpContext.Current.Server.MapPath("~/css");
            string css = Path.Combine(touchPath, "daf", "touch-theme.css");
            if (File.Exists(css))
            {
                _template = File.ReadAllText(css);
                string themeFile = Path.Combine(touchPath, "themes", ("touch-theme." 
                                + (theme + ".json")));
                string accentFile = Path.Combine(touchPath, "themes", ("touch-accent." 
                                + (accent + ".json")));
                if (File.Exists(themeFile) && File.Exists(accentFile))
                {
                    _accent = JObject.Parse(File.ReadAllText(accentFile));
                    _theme = JObject.Parse(File.ReadAllText(themeFile));
                }
            }
        }
        
        public static string Compile(string fileName)
        {
            Match m = ThemeStylesheetRegex.Match(fileName);
            if (m.Success)
            	return new StylesheetGenerator(m.Groups["Theme"].Value, m.Groups["Accent"].Value).ToString();
            return String.Empty;
        }
        
        public static string Minify(string css)
        {
            css = Regex.Replace(css, "[a-zA-Z]+#", "#");
            css = Regex.Replace(css, "[\\n\\r]+\\s*", String.Empty);
            css = Regex.Replace(css, "\\s\\s+", " ");
            css = Regex.Replace(css, "\\s?([:,;{}])\\s?", "$1");
            css = css.Replace(";}", "}");
            css = Regex.Replace(css, "([\\s:]0)(px|pt|%|em)", "$1");
            css = Regex.Replace(css, "/\\*[\\d\\D]*?\\*/", String.Empty);
            return css;
        }
        
        public override string ToString()
        {
            string result = _template;
            if (!(String.IsNullOrEmpty(_template)) && ((_theme != null) && (_accent != null)))
            	result = ThemeVariableRegex.Replace(result, DoReplaceThemeVariables);
            if (ApplicationServices.EnableMinifiedCss)
            	result = Minify(result);
            return result;
        }
        
        protected string DoReplaceThemeVariables(Match m)
        {
            string variable = m.Groups["Name"].Value;
            string before = m.Groups["Before"].Value;
            string after = m.Groups["After"].Value;
            string[] parts = variable.Split(',');
            string value = null;
            foreach (string part in parts)
            	if (TryGetThemeVariable(part.Trim().Substring(1), out value))
                	break;
            if (String.IsNullOrEmpty(value))
            	value = m.Groups["Value"].Value;
            if (ApplicationServices.EnableMinifiedCss)
            	return ((before + value) 
                            + after);
            else
            	return ((before 
                            + (" /*" + variable)) 
                            + (("*/ " + value) 
                            + after));
        }
        
        protected bool TryGetThemeVariable(string name, out string value)
        {
            if (!(_themeVariables.TryGetValue(name, out value)))
            {
                JToken token = null;
                if (name.StartsWith("theme."))
                {
                    token = ApplicationServices.TryGetJsonProperty(_accent, String.Join(".", "theme", _theme["name"], name.Substring(6)));
                    if ((token == null) || (token.Type == JTokenType.Null))
                    	token = ApplicationServices.TryGetJsonProperty(_theme, name.Substring(6));
                }
                else
                {
                    token = ApplicationServices.TryGetJsonProperty(_accent, String.Join(".", "theme", _theme["name"], name));
                    if ((token == null) || (token.Type == JTokenType.Null))
                    	token = ApplicationServices.TryGetJsonProperty(_accent, name);
                }
                if ((token != null) && token.Type != JTokenType.Null)
                	value = ((string)(token));
                _themeVariables[name] = value;
            }
            return !(String.IsNullOrEmpty(value));
        }
    }
    
    public class FolderCacheDependency : CacheDependency
    {
        
        private FileSystemWatcher _watcher;
        
        public FolderCacheDependency(string dirName, string filter)
        {
            _watcher = new FileSystemWatcher(dirName, filter);
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += new FileSystemEventHandler(this.watcher_Changed);
            _watcher.Deleted += new FileSystemEventHandler(this.watcher_Changed);
            _watcher.Created += new FileSystemEventHandler(this.watcher_Changed);
            _watcher.Renamed += new RenamedEventHandler(this.watcher_Renamed);
        }
        
        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            NotifyDependencyChanged(this, e);
        }
        
        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            NotifyDependencyChanged(this, e);
        }
    }
}
