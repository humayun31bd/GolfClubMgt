using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.Routing;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyCompany.Data;
using MyCompany.Handlers;
using MyCompany.Security;
using MyCompany.Web;

namespace MyCompany.Services
{
	public class UriRestConfig
    {
        
        private Regex _uri;
        
        private SortedDictionary<string, string> _properties;
        
        public static string[] SupportedJSONContentTypes = new string[] {
                "application/json",
                "text/javascript",
                "application/javascript",
                "application/ecmascript",
                "application/x-ecmascript"};
        
        public UriRestConfig(string uri)
        {
            _uri = new Regex(uri, RegexOptions.IgnoreCase);
            _properties = new SortedDictionary<string, string>();
        }
        
        public string this[string propertyName]
        {
            get
            {
                string result = null;
                _properties.TryGetValue(propertyName.ToLower(), out result);
                return result;
            }
            set
            {
                if (!(String.IsNullOrEmpty(value)))
                	value = value.Trim();
                _properties[propertyName.ToLower()] = value;
            }
        }
        
        public static List<UriRestConfig> Enumerate(ControllerConfiguration config)
        {
            List<UriRestConfig> list = new List<UriRestConfig>();
            XPathNavigator restConfigNode = config.SelectSingleNode("/c:dataController/c:restConfig");
            if (restConfigNode != null)
            {
                UriRestConfig urc = null;
                // configuration regex: ^\s*(?'Property'\w+)\s*(:|=)\s*(?'Value'.+?)\s*$
                Match m = Regex.Match(restConfigNode.Value, "^\\s*(?\'Property\'\\w+)\\s*(:|=)\\s*(?\'Value\'.+?)\\s*$", (RegexOptions.IgnoreCase | RegexOptions.Multiline));
                while (m.Success)
                {
                    string propertyName = m.Groups["Property"].Value;
                    string propertyValue = m.Groups["Value"].Value;
                    if (propertyName.Equals("Uri", StringComparison.CurrentCultureIgnoreCase))
                    	try
                        {
                            urc = new UriRestConfig(propertyValue);
                            list.Add(urc);
                        }
                        catch (Exception )
                        {
                        }
                    else
                    	if (urc != null)
                        	urc[propertyName] = propertyValue;
                    m = m.NextMatch();
                }
            }
            return list;
        }
        
        public virtual bool IsMatch(HttpRequest request)
        {
            return _uri.IsMatch(request.Path);
        }
        
        public static bool RequiresAuthentication(HttpRequest request, ControllerConfiguration config)
        {
            foreach (UriRestConfig urc in Enumerate(config))
            	if (urc.IsMatch(request) && (urc["Users"] == "?"))
                	return false;
            return true;
        }
        
        public static bool IsAuthorized(HttpRequest request, ControllerConfiguration config)
        {
            if (request.AcceptTypes == null)
            	return false;
            foreach (UriRestConfig urc in Enumerate(config))
            	if (urc.IsMatch(request))
                {
                    // verify HTTP method
                    string httpMethod = urc["Method"];
                    if (!(String.IsNullOrEmpty(httpMethod)))
                    {
                        string[] methodList = Regex.Split(httpMethod, "(\\s*,\\s*)");
                        if (!(methodList.Contains(request.HttpMethod)))
                        	return false;
                    }
                    // verify user identity
                    string users = urc["Users"];
                    if (!(String.IsNullOrEmpty(users)) && !((users == "?")))
                    {
                        if (!(HttpContext.Current.User.Identity.IsAuthenticated))
                        	return false;
                        if (!((users == "*")))
                        {
                            string[] userList = Regex.Split(users, "(\\s*,\\s*)");
                            if (!(userList.Contains(HttpContext.Current.User.Identity.Name)))
                            	return false;
                        }
                    }
                    // verify user roles
                    string roles = urc["Roles"];
                    if (!(String.IsNullOrEmpty(roles)) && !(DataControllerBase.UserIsInRole(roles)))
                    	return false;
                    // verify SSL, Xml, and JSON constrains
                    if (true.ToString().Equals(urc["Ssl"], StringComparison.OrdinalIgnoreCase) && !(request.IsSecureConnection))
                    	return false;
                    if (false.ToString().Equals(urc["Xml"], StringComparison.OrdinalIgnoreCase) && !(IsJSONRequest(request)))
                    	return false;
                    if (false.ToString().Equals(urc["Json"], StringComparison.OrdinalIgnoreCase) && IsJSONRequest(request))
                    	return false;
                    return true;
                }
            return false;
        }
        
        public static string TypeOfJSONRequest(HttpRequest request)
        {
            if (((request.QueryString["_dataType"] == "json") || !(String.IsNullOrEmpty(request.QueryString["_instance"]))) || !(String.IsNullOrEmpty(request.QueryString["callback"])))
            	return "application/javascript";
            if (request.AcceptTypes != null)
            	foreach (string t in request.AcceptTypes)
                {
                    int typeIndex = Array.IndexOf(UriRestConfig.SupportedJSONContentTypes, t);
                    if (!((typeIndex == -1)))
                    	return t;
                }
            return null;
        }
        
        public static bool IsJSONRequest(HttpRequest request)
        {
            return !(String.IsNullOrEmpty(TypeOfJSONRequest(request)));
        }
        
        public static bool IsJSONPRequest(HttpRequest request)
        {
            string t = TypeOfJSONRequest(request);
            return (!(String.IsNullOrEmpty(t)) && !((t == SupportedJSONContentTypes[0])));
        }
    }
    
    public partial class RepresentationalStateTransfer : RepresentationalStateTransferBase
    {
    }
    
    public class RepresentationalStateTransferBase : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        
        public static Regex JsonDateRegex = new Regex("\"\\\\/Date\\((\\-?\\d+)\\)\\\\/\"");
        
        public static Regex ScriptResourceRegex = new Regex("^(?\'ScriptName\'[\\w\\-]+?)(\\-(?\'Version\'[\\.\\d]+))?(\\.(?\'Culture\'[\\w\\-]+?))?(\\.(?\'Ac" +
                "cent\'\\w+))?\\.(?\'Extension\'js|css)", RegexOptions.IgnoreCase);
        
        public static Regex CultureJavaScriptRegex = new Regex("//<\\!\\[CDATA\\[\\s+(?\'JavaScript\'var __cultureInfo[\\s\\S]*?)//\\]\\]>");
        
        public static string[] NumericTypes = new string[] {
                "SByte",
                "Byte",
                "Int16",
                "Int32",
                "UInt32",
                "Int64",
                "Single",
                "Double",
                "Decimal",
                "Currency"};
        
        bool IHttpHandler.IsReusable
        {
            get
            {
                return true;
            }
        }
        
        protected virtual string HttpMethod
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                string requestType = request.HttpMethod;
                if ((requestType == "GET") && !(String.IsNullOrEmpty(request["callback"])))
                {
                    string t = request.QueryString["_type"];
                    if (!(String.IsNullOrEmpty(t)))
                    	requestType = t;
                }
                return requestType;
            }
        }
        
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            CultureManager.Initialize();
            RouteValueDictionary routeValues = context.Request.RequestContext.RouteData.Values;
            string controllerName = ((string)(routeValues["Controller"]));
            if (String.IsNullOrEmpty(controllerName))
            	controllerName = context.Request.QueryString["_controller"];
            Stream output = context.Response.OutputStream;
            string contentType = "text/xml";
            bool json = UriRestConfig.IsJSONRequest(context.Request);
            if (json)
            	contentType = (UriRestConfig.TypeOfJSONRequest(context.Request) + "; charset=utf-8");
            context.Response.ContentType = contentType;
            try
            {
                if (controllerName == "saas")
                {
                    string service = ((string)(routeValues["Segment1"]));
                    OAuthHandler handler = OAuthHandlerFactory.Create(service);
                    if (handler != null)
                    	handler.ProcessRequest(context);
                }
                else
                	if (controllerName == "_authenticate")
                    	AuthenticateSaaS(context);
                    else
                    {
                        Match script = ScriptResourceRegex.Match(controllerName);
                        string scriptName = script.Groups["ScriptName"].Value;
                        bool isSaaS = (scriptName == "factory");
                        bool isCombinedScript = (scriptName == "combined");
                        bool isStylesheet = (scriptName == "stylesheet");
                        if ((isStylesheet || (scriptName == "touch-theme")) && (script.Groups["Extension"].Value == "css"))
                        {
                            context.Response.ContentType = "text/css";
                            string css = String.Empty;
                            if (isStylesheet)
                            	css = ApplicationServices.CombineTouchUIStylesheets(context);
                            else
                            	css = StylesheetGenerator.Compile(controllerName);
                            ApplicationServices.CompressOutput(context, css);
                        }
                        else
                        	if ((isSaaS || isCombinedScript) && (HttpMethod == "GET"))
                            	CombineScripts(context, isSaaS, scriptName, script.Groups["Culture"].Value, script.Groups["Version"].Value);
                            else
                            	if (Regex.IsMatch(HttpMethod, "^(GET|POST|DELETE|PUT)$"))
                                	PerformRequest(context, output, json, controllerName);
                                else
                                	context.Response.StatusCode = 400;
                    }
            }
            catch (Exception error)
            {
                if (context.Response.StatusCode != 302)
                {
                    context.Response.ContentType = "text/xml";
                    context.Response.Clear();
                    XmlWriter writer = CreateXmlWriter(output);
                    RenderException(context, error, writer);
                    writer.Close();
                    context.Response.StatusCode = 400;
                }
            }
        }
        
        protected virtual void CombineScripts(HttpContext context, bool isSaaS, string scriptName, string culture, string version)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            if (!(isSaaS))
            {
                HttpCachePolicy cache = response.Cache;
                cache.SetCacheability(HttpCacheability.Public);
                cache.VaryByParams["_touch"] = true;
                cache.VaryByHeaders["User-Agent"] = true;
                cache.SetOmitVaryStar(true);
                cache.SetExpires(DateTime.Now.AddDays(365));
                cache.SetValidUntilExpires(true);
                cache.SetLastModifiedFromFileDependencies();
            }
            if (isSaaS)
            {
                if (!(String.IsNullOrEmpty(culture)))
                	try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                    }
                    catch (Exception )
                    {
                    }
            }
            StringBuilder sb = new StringBuilder();
            string baseUrl = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, request.ApplicationPath);
            List<ScriptReference> scripts = AquariumExtenderBase.StandardScripts(true);
            foreach (ScriptReference sr in scripts)
            {
                bool add = true;
                string path = sr.Path;
                int index = path.IndexOf("?");
                if (index > 0)
                {
                    path = path.Substring(0, index);
                    if (path.EndsWith("_System.js"))
                    	add = !((request.QueryString["jquery"] == "false"));
                    else
                    	if (path.Contains("Web.Membership") || path.Contains("Web.Mobile"))
                        	add = !(isSaaS);
                }
                if (add)
                {
                    string script;
                    if (String.IsNullOrEmpty(path))
                    	script = new StreamReader(GetType().Assembly.GetManifestResourceStream(sr.Name)).ReadToEnd();
                    else
                    	script = File.ReadAllText(context.Server.MapPath(path));
                    script = script.Replace("sourceMappingURL=", " sourceMappingURL=../js/");
                    sb.AppendLine(script);
                    if (!(script.EndsWith(";")))
                    	sb.Append(";");
                }
            }
            if (isSaaS)
            {
                if (ApplicationServices.IsTouchClient)
                	sb.AppendFormat(String.Format("$(\'<link></link>\').appendTo($(\'head\')).attr({{ href: \'{0}/css//jquery.mobile-{1}." +
                                "min.css\', type: \'text/css\', rel: \'stylesheet\' }});", ApplicationServices.JqmVersion), baseUrl, ApplicationServices.JqmVersion);
                else
                	sb.AppendFormat(String.Format("$(\'<link></link>\').appendTo($(\'head\')).attr({{ href: \'{0}/App_Themes/MyCompany/_T" +
                                "heme_Azure.css?{0}\', type: \'text/css\', rel: \'stylesheet\' }});", ApplicationServices.Version), baseUrl);
                try
                {
                    StringBuilder blankPage = new StringBuilder();
                    StringWriter sw = new StringWriter(blankPage);
                    context.Server.Execute("~/default.aspx?_page=_blank", sw);
                    sw.Flush();
                    sw.Close();
                    Match cultureJS = CultureJavaScriptRegex.Match(blankPage.ToString());
                    if (cultureJS.Success)
                    {
                        sb.AppendLine(cultureJS.Groups["JavaScript"].Value);
                        sb.AppendLine("Sys.CultureInfo.CurrentCulture=__cultureInfo;");
                    }
                }
                catch (Exception )
                {
                }
                sb.AppendFormat("var __targetFramework=\'4.5\';__tf=4.0;__cothost=\'appfactory\';__appInfo=\'GolfClubNe" +
                        "w|{0}\';", BusinessRules.JavaScriptString(context.User.Identity.Name));
                sb.AppendFormat("Sys.Application.add_init(function() {{ Web.DataView._run(\'{0}\',\'{0}/Services/Data" +
                        "ControllerService.asmx\', {1}) }});", baseUrl, context.User.Identity.IsAuthenticated.ToString().ToLower());
            }
            context.Response.ContentType = "application/javascript";
            ApplicationServices.CompressOutput(context, sb.ToString());
        }
        
        protected virtual void AuthenticateSaaS(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string args = request.Params["args"];
            StringBuilder result = new StringBuilder(String.Format("{0}(", request.QueryString["callback"]));
            object resultObject = false;
            string[] login = JsonConvert.DeserializeObject<string[]>(args);
            resultObject = ApplicationServices.Login(((string)(login[0])), ((string)(login[1])), false);
            result.Append(JsonConvert.SerializeObject(resultObject));
            result.Append(")");
            string jsonp = result.ToString();
            response.Write(jsonp);
        }
        
        private string DoReplaceDateTicks(Match m)
        {
            return String.Format("new Date({0})", m.Groups[1].Value);
        }
        
        internal virtual XmlWriter CreateXmlWriter(Stream output)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = false;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(output, settings);
            return writer;
        }
        
        internal virtual void RenderException(HttpContext context, Exception error, XmlWriter writer)
        {
            if (error != null)
            {
                writer.WriteStartElement("error");
                writer.WriteElementString("message", error.Message);
                writer.WriteElementString("type", error.GetType().ToString());
                if (context.Request.UserHostName == "::1")
                {
                    writer.WriteStartElement("stackTrace");
                    writer.WriteCData(error.StackTrace);
                    writer.WriteEndElement();
                    RenderException(context, error.InnerException, writer);
                }
                writer.WriteEndElement();
            }
        }
        
        protected XPathNavigator SelectView(ControllerConfiguration config, string viewId)
        {
            return config.SelectSingleNode("/c:dataController/c:views/c:view[@id=\'{0}\']", viewId);
        }
        
        protected XPathNavigator SelectDataField(ControllerConfiguration config, string viewId, string fieldName)
        {
            return config.SelectSingleNode("/c:dataController/c:views/c:view[@id=\'{0}\']/.//c:dataField[@fieldName=\'{1}\' or @a" +
                    "liasFieldName=\'{1}\']", viewId, fieldName);
        }
        
        protected XPathNavigator SelectField(ControllerConfiguration config, string name)
        {
            return config.SelectSingleNode("/c:dataController/c:fields/c:field[@name=\'{0}\']", name);
        }
        
        protected XPathNavigator SelectActionGroup(ControllerConfiguration config, string actionGroupId)
        {
            return config.SelectSingleNode("/c:dataController/c:actions/c:actionGroup[@id=\'{0}\']", actionGroupId);
        }
        
        protected XPathNavigator SelectAction(ControllerConfiguration config, string actionGroupId, string actionId)
        {
            return config.SelectSingleNode("/c:dataController/c:actions/c:actionGroup[@id=\'{0}\']/c:action[@id=\'{1}\']", actionGroupId, actionId);
        }
        
        private bool VerifyActionSegments(ControllerConfiguration config, string actionGroupId, string actionId, bool keyIsAvailable)
        {
            bool result = true;
            if (SelectActionGroup(config, actionGroupId) != null)
            {
                XPathNavigator actionNode = SelectAction(config, actionGroupId, actionId);
                if (actionNode == null)
                	result = false;
                else
                	if (!(keyIsAvailable) && ((actionNode.GetAttribute("whenKeySelected", String.Empty) == "true") || Regex.IsMatch(actionNode.GetAttribute("commandName", String.Empty), "^(Update|Delete)$")))
                    	result = false;
            }
            else
            	result = false;
            return result;
        }
        
        private void AnalyzeRouteValues(HttpRequest request, HttpResponse response, bool isHttpGetMethod, ControllerConfiguration config, out string view, out string key, out string fieldName, out string actionGroupId, out string actionId, out string commandName)
        {
            RouteValueDictionary routeValues = request.RequestContext.RouteData.Values;
            string segment1 = ((string)(routeValues["Segment1"]));
            string segment2 = ((string)(routeValues["Segment2"]));
            string segment3 = ((string)(routeValues["Segment3"]));
            string segment4 = ((string)(routeValues["Segment4"]));
            view = null;
            key = null;
            fieldName = null;
            actionGroupId = null;
            actionId = null;
            commandName = null;
            if (!(String.IsNullOrEmpty(segment1)))
            	if (SelectView(config, segment1) != null)
                {
                    view = segment1;
                    if (isHttpGetMethod)
                    {
                        key = segment2;
                        fieldName = segment3;
                    }
                    else
                    	if (VerifyActionSegments(config, segment2, segment3, false))
                        {
                            actionGroupId = segment2;
                            actionId = segment3;
                        }
                        else
                        	if (String.IsNullOrEmpty(segment2))
                            {
                                if (HttpMethod != "POST")
                                	response.StatusCode = 404;
                            }
                            else
                            {
                                key = segment2;
                                if (VerifyActionSegments(config, segment3, segment4, true))
                                {
                                    actionGroupId = segment3;
                                    actionId = segment4;
                                }
                                else
                                	if (!(((HttpMethod == "PUT") || (HttpMethod == "DELETE"))))
                                    	response.StatusCode = 404;
                            }
                }
                else
                	if (isHttpGetMethod)
                    {
                        key = segment1;
                        fieldName = segment2;
                    }
                    else
                    	if (VerifyActionSegments(config, segment1, segment2, false))
                        {
                            actionGroupId = segment1;
                            actionId = segment2;
                        }
                        else
                        	if (String.IsNullOrEmpty(segment1))
                            	response.StatusCode = 404;
                            else
                            {
                                key = segment1;
                                if (VerifyActionSegments(config, segment2, segment3, true))
                                {
                                    actionGroupId = segment2;
                                    actionId = segment3;
                                }
                                else
                                	if (!(((HttpMethod == "PUT") || (HttpMethod == "DELETE"))))
                                    	response.StatusCode = 404;
                            }
            else
            {
                view = request.QueryString["_view"];
                key = request.QueryString["_key"];
                fieldName = request.QueryString["_fieldName"];
                if (!(isHttpGetMethod))
                	actionGroupId = request.QueryString["_actionId"];
            }
            if (!(isHttpGetMethod))
            {
                XPathNavigator actionNode = SelectAction(config, actionGroupId, actionId);
                if (actionNode != null)
                	commandName = actionNode.GetAttribute("commandName", String.Empty);
                else
                	commandName = HttpMethodToCommandName(request);
            }
        }
        
        private string HttpMethodToCommandName(HttpRequest request)
        {
            if (HttpMethod == "POST")
            	return "Insert";
            if (HttpMethod == "PUT")
            	return "Update";
            if (HttpMethod == "DELETE")
            	return "Delete";
            return null;
        }
        
        protected virtual bool AuthorizeRequest(HttpRequest request, ControllerConfiguration config)
        {
            return UriRestConfig.IsAuthorized(request, config);
        }
        
        private void PerformRequest(HttpContext context, Stream output, bool json, string controllerName)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            ControllerConfiguration config = null;
            try
            {
                config = DataControllerBase.CreateConfigurationInstance(GetType(), controllerName);
            }
            catch (Exception )
            {
                response.StatusCode = 404;
                return;
            }
            if (!(AuthorizeRequest(request, config)))
            {
                response.StatusCode = 404;
                return;
            }
            // analyze route segments
            bool isHttpGetMethod = (HttpMethod == "GET");
            string view = null;
            string key = null;
            string fieldName = null;
            string actionGroupId = null;
            string actionId = null;
            string commandName = null;
            AnalyzeRouteValues(request, response, isHttpGetMethod, config, out view, out key, out fieldName, out actionGroupId, out actionId, out commandName);
            if (response.StatusCode == 404)
            	return;
            bool keyIsAvailable = !(String.IsNullOrEmpty(key));
            if (String.IsNullOrEmpty(view))
            	if (isHttpGetMethod)
                	view = Controller.GetSelectView(controllerName);
                else
                	if (commandName == "Insert")
                    	view = Controller.GetInsertView(controllerName);
                    else
                    	if (commandName == "Update")
                        	view = Controller.GetUpdateView(controllerName);
                        else
                        	if (commandName == "Delete")
                            	view = Controller.GetDeleteView(controllerName);
            if (SelectView(config, view) == null)
            {
                response.StatusCode = 404;
                return;
            }
            XPathNavigator dataFieldNode = null;
            XPathNavigator fieldNode = null;
            if (!(String.IsNullOrEmpty(fieldName)))
            {
                dataFieldNode = SelectDataField(config, view, fieldName);
                fieldNode = SelectField(config, fieldName);
                if ((dataFieldNode == null) || (fieldNode == null))
                {
                    response.StatusCode = 404;
                    return;
                }
            }
            // create a filter
            List<string> filter = new List<string>();
            // process key fields
            if (keyIsAvailable)
            {
                string[] values = key.Split(new char[] {
                            ','}, StringSplitOptions.RemoveEmptyEntries);
                XPathNodeIterator keyIterator = config.Select("/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']");
                int index = 0;
                while (keyIterator.MoveNext())
                {
                    filter.Add(String.Format("{0}:={1}", keyIterator.Current.GetAttribute("name", String.Empty), values[index]));
                    index++;
                }
            }
            // process quick find
            string quickFind = request.Params["_q"];
            if (!(String.IsNullOrEmpty(quickFind)))
            	filter.Add(String.Format("{0}:~{1}", config.SelectSingleNode("/c:dataController/c:views/c:view[@id=\'{0}\']/.//c:dataField[1]/@fieldName", view).Value, quickFind));
            // process filter parameters
            if (!(keyIsAvailable))
            	foreach (string filterName in request.Params.Keys)
                	if (SelectDataField(config, view, filterName) != null)
                    	filter.Add(String.Format("{0}:={1}", filterName, request.Params[filterName]));
                    else
                    {
                        Match m = BusinessRules.SqlFieldFilterOperationRegex.Match(filterName);
                        string filterFieldName = m.Groups["Name"].Value;
                        if (m.Success && (SelectDataField(config, view, filterFieldName) != null))
                        {
                            string operation = m.Groups["Operation"].Value;
                            RowFilterOperation filterOperation = ((RowFilterOperation)(TypeDescriptor.GetConverter(typeof(RowFilterOperation)).ConvertFromString(operation)));
                            string filterValue = request.Params[filterName];
                            if ((filterOperation == RowFilterOperation.Includes) || (filterOperation == RowFilterOperation.DoesNotInclude))
                            	filterValue = Regex.Replace(filterValue, ",", "$or$");
                            else
                            	if (filterOperation == RowFilterOperation.Between)
                                	filterValue = Regex.Replace(filterValue, ",", "$and$");
                            filter.Add(String.Format("{0}:{1}{2}", filterFieldName, RowFilterAttribute.ComparisonOperations[Convert.ToInt32(filterOperation)], filterValue));
                        }
                    }
            // execute request
            if (isHttpGetMethod)
            	if (fieldNode != null)
                {
                    string style = "o";
                    if (request.QueryString["_style"] == "Thumbnail")
                    	style = "t";
                    string blobPath = String.Format("~/Blob.ashx?{0}={1}|{2}", fieldNode.GetAttribute("onDemandHandler", String.Empty), style, key);
                    context.RewritePath(blobPath);
                    Blob blobHandler = new Blob();
                    ((IHttpHandler)(blobHandler)).ProcessRequest(context);
                }
                else
                	ExecuteHttpGetRequest(request, response, output, json, controllerName, view, filter, keyIsAvailable);
            else
            	ExecuteActionRequest(request, response, output, json, config, controllerName, view, key, filter, actionGroupId, actionId);
        }
        
        private void ExecuteActionRequest(HttpRequest request, HttpResponse response, Stream output, bool json, ControllerConfiguration config, string controllerName, string view, string key, List<string> filter, string actionGroupId, string actionId)
        {
            XPathNavigator actionNode = SelectAction(config, actionGroupId, actionId);
            string commandName = HttpMethodToCommandName(request);
            string commandArgument = String.Empty;
            string lastCommandName = String.Empty;
            if (actionNode == null)
            {
                if (String.IsNullOrEmpty(commandName))
                {
                    response.StatusCode = 404;
                    return;
                }
            }
            else
            {
                commandName = actionNode.GetAttribute("commandName", String.Empty);
                commandArgument = actionNode.GetAttribute("commandArgument", String.Empty);
                lastCommandName = actionNode.GetAttribute("whenLastCommandName", String.Empty);
            }
            // prepare action arguments
            ActionArgs args = new ActionArgs();
            args.Controller = controllerName;
            args.View = view;
            args.CommandName = commandName;
            args.CommandArgument = commandArgument;
            args.LastCommandName = lastCommandName;
            args.Filter = filter.ToArray();
            args.SortExpression = request.QueryString["_sortExpression"];
            string selectedValues = request.Params["_selectedValues"];
            if (!(String.IsNullOrEmpty(selectedValues)))
            	args.SelectedValues = selectedValues.Split(new char[] {
                            ','}, StringSplitOptions.RemoveEmptyEntries);
            args.Trigger = request.Params["_trigger"];
            args.Path = String.Format("{0}/{1}", actionGroupId, actionId);
            NameValueCollection form = request.Form;
            if (request.HttpMethod == "GET")
            	form = request.QueryString;
            List<FieldValue> values = new List<FieldValue>();
            foreach (string fieldName in form.Keys)
            {
                XPathNavigator field = SelectField(config, fieldName);
                XPathNavigator dataField = SelectDataField(config, view, fieldName);
                if (field != null)
                {
                    object oldValue = form[(fieldName + "_OldValue")];
                    object value = form[fieldName];
                    // try parsing the values
                    string dataFormatString = null;
                    if (dataField != null)
                    	dataFormatString = dataField.GetAttribute("dataFormatString", String.Empty);
                    if (String.IsNullOrEmpty(dataFormatString))
                    	dataFormatString = field.GetAttribute("dataFormatString", String.Empty);
                    if (!(String.IsNullOrEmpty(dataFormatString)) && !(dataFormatString.StartsWith("{")))
                    	dataFormatString = String.Format("{{0:{0}}}", dataFormatString);
                    string fieldType = field.GetAttribute("type", String.Empty);
                    if (NumericTypes.Contains(fieldType))
                    {
                        double d;
                        if (Double.TryParse(((string)(value)), NumberStyles.Any, CultureInfo.CurrentUICulture, out d))
                        	value = d;
                        if (Double.TryParse(((string)(oldValue)), NumberStyles.Any, CultureInfo.CurrentUICulture, out d))
                        	oldValue = d;
                    }
                    else
                    	if (fieldType == "DateTime")
                        {
                            DateTime dt;
                            if (!(String.IsNullOrEmpty(dataFormatString)))
                            {
                                if (DateTime.TryParseExact(((string)(value)), dataFormatString, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt))
                                	value = dt;
                                if (DateTime.TryParseExact(((string)(oldValue)), dataFormatString, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt))
                                	oldValue = dt;
                            }
                            else
                            {
                                if (DateTime.TryParse(((string)(value)), out dt))
                                	value = dt;
                                if (DateTime.TryParse(((string)(oldValue)), out dt))
                                	oldValue = dt;
                            }
                        }
                    // create a field value
                    FieldValue fv = null;
                    if (oldValue != null)
                    	fv = new FieldValue(fieldName, oldValue, value);
                    else
                    	fv = new FieldValue(fieldName, value);
                    // figure if the field is read-only
                    bool readOnly = (field.GetAttribute("readOnly", String.Empty) == "true");
                    string writeRoles = field.GetAttribute("writeRoles", String.Empty);
                    if (!(String.IsNullOrEmpty(writeRoles)) && !(DataControllerBase.UserIsInRole(writeRoles)))
                    	readOnly = true;
                    if (dataField == null)
                    	readOnly = true;
                    fv.ReadOnly = readOnly;
                    // add field value to the list
                    values.Add(fv);
                }
            }
            int keyIndex = 0;
            XPathNodeIterator keyIterator = config.Select("/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']");
            while (keyIterator.MoveNext())
            {
                string fieldName = keyIterator.Current.GetAttribute("name", String.Empty);
                foreach (FieldValue fv in values)
                	if (fv.Name == fieldName)
                    {
                        fieldName = null;
                        if ((fv.OldValue == null) && ((commandName == "Update") || (commandName == "Delete")))
                        {
                            fv.OldValue = fv.NewValue;
                            fv.Modified = false;
                        }
                        break;
                    }
                if (!(String.IsNullOrEmpty(fieldName)))
                {
                    string oldValue = null;
                    if (!(String.IsNullOrEmpty(key)))
                    {
                        string[] keyValues = key.Split(new char[] {
                                    ','}, StringSplitOptions.RemoveEmptyEntries);
                        if (keyIndex < keyValues.Length)
                        	oldValue = keyValues[keyIndex];
                    }
                    values.Add(new FieldValue(fieldName, oldValue, oldValue));
                }
                keyIndex++;
            }
            args.Values = values.ToArray();
            // execute action
            IDataController controllerInstance = ControllerFactory.CreateDataController();
            ActionResult result = controllerInstance.Execute(controllerName, view, args);
            // redirect response location if success or error url has been specified
            string successUrl = request.Params["_successUrl"];
            string errorUrl = request.Params["_errorUrl"];
            if ((result.Errors.Count == 0) && !(String.IsNullOrEmpty(successUrl)))
            {
                response.RedirectLocation = successUrl;
                response.StatusCode = 301;
                return;
            }
            if ((result.Errors.Count > 0) && !(String.IsNullOrEmpty(errorUrl)))
            {
                if (errorUrl.Contains("?"))
                	errorUrl = (errorUrl + "&");
                else
                	errorUrl = (errorUrl + "?");
                errorUrl = String.Format("{0}_error={1}", errorUrl, HttpUtility.UrlEncode(result.Errors[0]));
                response.RedirectLocation = errorUrl;
                response.StatusCode = 301;
                return;
            }
            if (json)
            {
                StreamWriter sw = CreateStreamWriter(request, response, output);
                BeginResponsePadding(request, sw);
                sw.Write("{{\"rowsAffected\":{0}", result.RowsAffected);
                if ((result.Errors != null) && (result.Errors.Count > 0))
                {
                    sw.Write(",\"errors\":[");
                    bool first = true;
                    foreach (string error in result.Errors)
                    {
                        if (first)
                        	first = false;
                        else
                        	sw.Write(",");
                        sw.Write("{{\"message\":\"{0}\"}}", BusinessRules.JavaScriptString(error));
                    }
                    sw.Write("]");
                }
                if (!(String.IsNullOrEmpty(result.ClientScript)))
                	sw.Write(",\"clientScript\":\"{0}\"", BusinessRules.JavaScriptString(result.ClientScript));
                if (!(String.IsNullOrEmpty(result.NavigateUrl)))
                	sw.Write(",\"navigateUrl\":\"{0}\"", BusinessRules.JavaScriptString(result.NavigateUrl));
                if (result.Values != null)
                	foreach (FieldValue fv in result.Values)
                    {
                        sw.Write(",\"{0}\":", fv.Name);
                        WriteJSONValue(sw, fv.Value, null);
                    }
                sw.Write("}");
                EndResponsePadding(request, sw);
                sw.Close();
            }
            else
            {
                XmlWriter writer = CreateXmlWriter(output);
                writer.WriteStartDocument();
                writer.WriteStartElement("result");
                writer.WriteAttributeString("rowsAffected", result.RowsAffected.ToString());
                if ((result.Errors != null) && (result.Errors.Count > 0))
                {
                    writer.WriteStartElement("errors");
                    foreach (string error in result.Errors)
                    {
                        writer.WriteStartElement("error");
                        writer.WriteAttributeString("message", error);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                if (!(String.IsNullOrEmpty(result.ClientScript)))
                	writer.WriteAttributeString("clientScript", result.ClientScript);
                if (!(String.IsNullOrEmpty(result.NavigateUrl)))
                	writer.WriteAttributeString("navigateUrl", result.NavigateUrl);
                if (result.Values != null)
                	foreach (FieldValue fv in result.Values)
                    	writer.WriteElementString(fv.Name, Convert.ToString(fv.Value));
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }
        
        protected virtual void WriteJSONValue(StreamWriter writer, object v, DataField field)
        {
            string dataFormatString = null;
            if (field != null)
            	dataFormatString = field.DataFormatString;
            if (v == null)
            	writer.Write("null");
            else
            	if (v is string)
                	writer.Write("\"{0}\"", BusinessRules.JavaScriptString(((string)(v))));
                else
                	if (v is DateTime)
                    	writer.Write("\"{0}\"", ConvertDateToJSON(((DateTime)(v)), dataFormatString));
                    else
                    	if (v is Guid)
                        	writer.Write("\"{0}\"", BusinessRules.JavaScriptString(v.ToString()));
                        else
                        	if (v is bool)
                            	writer.Write(v.ToString().ToLower());
                            else
                            	if (!(String.IsNullOrEmpty(dataFormatString)))
                                	writer.Write("\"{0}\"", ConvertValueToJSON(v, dataFormatString));
                                else
                                	writer.Write(ConvertValueToJSON(v, null));
        }
        
        protected virtual void ExecuteHttpGetRequest(HttpRequest request, HttpResponse response, Stream output, bool json, string controllerName, string view, List<string> filter, bool keyIsAvailable)
        {
            // prepare a page request
            int pageSize;
            int.TryParse(request.QueryString["_pageSize"], out pageSize);
            if (pageSize == 0)
            	pageSize = 100;
            int pageIndex;
            int.TryParse(request.QueryString["_pageIndex"], out pageIndex);
            PageRequest r = new PageRequest();
            r.Controller = controllerName;
            r.View = view;
            r.RequiresMetaData = true;
            r.PageSize = pageSize;
            r.PageIndex = pageIndex;
            r.Filter = filter.ToArray();
            r.RequiresRowCount = ((pageIndex == 0) && !(keyIsAvailable));
            r.SortExpression = request.QueryString["_sortExpression"];
            // request the data
            IDataController controllerInstance = ControllerFactory.CreateDataController();
            ViewPage page = controllerInstance.GetPage(r.Controller, r.View, r);
            if (keyIsAvailable && (page.Rows.Count == 0))
            {
                response.StatusCode = 404;
                return;
            }
            // stream out the data
            XmlWriter writer = null;
            StreamWriter sw = null;
            if (json)
            {
                sw = CreateStreamWriter(request, response, output);
                BeginResponsePadding(request, sw);
                if (!(keyIsAvailable))
                {
                    sw.Write("{");
                    if (r.RequiresRowCount)
                    	sw.Write("\"totalRowCount\":{0},", page.TotalRowCount);
                    sw.Write("\"pageSize\":{0},\"pageIndex\":{1},\"rowCount\":{2},", page.PageSize, page.PageIndex, page.Rows.Count);
                    sw.Write("\"{0}\":[", controllerName);
                }
            }
            else
            {
                writer = CreateXmlWriter(output);
                writer.WriteStartDocument();
                writer.WriteStartElement(controllerName);
                if (r.RequiresRowCount)
                	writer.WriteAttributeString("totalRowCount", page.TotalRowCount.ToString());
                if (!(keyIsAvailable))
                {
                    writer.WriteAttributeString("pageSize", page.PageSize.ToString());
                    writer.WriteAttributeString("pageIndex", page.PageIndex.ToString());
                    writer.WriteAttributeString("rowCount", page.Rows.Count.ToString());
                    writer.WriteStartElement("items");
                }
            }
            bool firstRow = true;
            foreach (DataField field in page.Fields)
            	if (!(String.IsNullOrEmpty(field.DataFormatString)) && !(field.DataFormatString.StartsWith("{")))
                	field.DataFormatString = String.Format("{{0:{0}}}", field.DataFormatString);
            foreach (object[] row in page.Rows)
            {
                int index = 0;
                if (json)
                {
                    if (firstRow)
                    	firstRow = false;
                    else
                    	sw.Write(",");
                    sw.Write("{");
                }
                else
                	if (!(keyIsAvailable))
                    	writer.WriteStartElement("item");
                bool firstField = true;
                foreach (DataField field in page.Fields)
                {
                    if (json)
                    {
                        if (firstField)
                        	firstField = false;
                        else
                        	sw.Write(",");
                        sw.Write("\"{0}\":", field.Name);
                        WriteJSONValue(sw, row[index], field);
                    }
                    else
                    {
                        object v = row[index];
                        if (v != null)
                        {
                            string s = null;
                            if (!(String.IsNullOrEmpty(field.DataFormatString)))
                            	s = String.Format(field.DataFormatString, v);
                            else
                            	s = Convert.ToString(v);
                            writer.WriteAttributeString(field.Name, s);
                        }
                    }
                    index++;
                }
                if (json)
                	sw.Write("}");
                else
                	if (!(keyIsAvailable))
                    	writer.WriteEndElement();
                if (keyIsAvailable)
                	break;
            }
            if (json)
            {
                if (!(keyIsAvailable))
                	sw.Write("]}");
                EndResponsePadding(request, sw);
                sw.Close();
            }
            else
            {
                if (!(keyIsAvailable))
                	writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }
        
        protected virtual string ConvertValueToJSON(object v, string dataFormatString)
        {
            if (String.IsNullOrEmpty(dataFormatString))
            	return v.ToString();
            else
            	return String.Format(dataFormatString, v);
        }
        
        protected virtual string ConvertDateToJSON(DateTime dt, string dataFormatString)
        {
            dt = dt.ToUniversalTime();
            if (String.IsNullOrEmpty(dataFormatString))
            	return dt.ToString("F");
            else
            	return String.Format(dataFormatString, dt);
        }
        
        protected virtual void BeginResponsePadding(HttpRequest request, StreamWriter sw)
        {
            string callback = request.QueryString["callback"];
            if (!(String.IsNullOrEmpty(callback)))
            	sw.Write("{0}(", callback);
            else
            	if ((request.HttpMethod == "GET") && UriRestConfig.IsJSONPRequest(request))
                {
                    string instance = request.QueryString["_instance"];
                    if (String.IsNullOrEmpty(instance))
                    	instance = ((string)(request.RequestContext.RouteData.Values["Controller"]));
                    sw.Write("MyCompany=typeof MyCompany==\'undefined\'?{{}}:MyCompany;MyCompany.{0}=", instance);
                }
        }
        
        protected virtual void EndResponsePadding(HttpRequest request, StreamWriter sw)
        {
            string callback = request.QueryString["callback"];
            if (!(String.IsNullOrEmpty(callback)))
            	sw.Write(")");
            else
            	if ((request.HttpMethod == "GET") && UriRestConfig.IsJSONPRequest(request))
                	sw.Write(";");
        }
        
        protected virtual StreamWriter CreateStreamWriter(HttpRequest request, HttpResponse response, Stream output)
        {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!(String.IsNullOrEmpty(acceptEncoding)))
            {
                string[] encodings = acceptEncoding.Split(',');
                if (encodings.Contains("gzip"))
                {
                    output = new GZipStream(output, CompressionMode.Compress);
                    response.AppendHeader("Content-Encoding", "gzip");
                }
                else
                	if (encodings.Contains("deflate"))
                    {
                        output = new DeflateStream(output, CompressionMode.Compress);
                        response.AppendHeader("Content-Encoding", "deflate");
                    }
            }
            return new StreamWriter(output);
        }
    }
    
    public partial class FacebookOAuthHandler : FacebookOAuthHandlerBase
    {
    }
    
    public partial class FacebookOAuthHandlerBase : OAuthHandler
    {
        
        private JObject _userObj;
        
        protected override string Scope
        {
            get
            {
                return "email";
            }
        }
        
        protected virtual string GetVersion()
        {
            string version = Config["Version"];
            if (String.IsNullOrEmpty(version))
            	version = "v2.8";
            return version;
        }
        
        public override string GetHandlerName()
        {
            return "Facebook";
        }
        
        public override void RequestAuthorizationCode()
        {
            string authUrl = String.Format("https://www.facebook.com/{0}/dialog/oauth?response_type=code&client_id={1}&redire" +
                    "ct_uri={2}&scope={3}&state={4}", GetVersion(), Config.ClientId, Config.RedirectUri, Uri.EscapeDataString(Scope), Uri.EscapeDataString(GetState()));
            HttpContext.Current.Response.Redirect(authUrl);
        }
        
        protected override WebRequest GetAccessTokenRequest(string code, bool refresh)
        {
            return WebRequest.Create(String.Format("https://graph.facebook.com/{0}/oauth/access_token?client_id={1}&redirect_uri={2}&" +
                        "client_secret={3}&code={4}", GetVersion(), Config.ClientId, Config.RedirectUri, Config.ClientSecret, code));
        }
        
        protected override WebRequest GetQueryRequest(string method, string token)
        {
            WebRequest request = WebRequest.Create((("https://graph.facebook.com/" + GetVersion()) 
                            + ("/" + method)));
            request.Headers[HttpRequestHeader.Authorization] = ("Bearer " + token);
            return request;
        }
        
        public override string GetUserName()
        {
            _userObj = Query("me?fields=email", false);
            return ((string)(_userObj["email"]));
        }
        
        public override string GetUserImageUrl(MembershipUser user)
        {
            return ("https://graph.facebook.com/" 
                        + (GetVersion() 
                        + ("/" 
                        + (((string)(_userObj["id"])) + "/picture"))));
        }
    }
    
    public partial class GoogleOAuthHandler : GoogleOAuthHandlerBase
    {
    }
    
    public partial class GoogleOAuthHandlerBase : OAuthHandler
    {
        
        private JObject _userObj;
        
        protected override string Scope
        {
            get
            {
                string defaultScope = "email";
                if (StoreToken)
                	defaultScope = (defaultScope + " https://www.googleapis.com/auth/admin.directory.group.readonly");
                return defaultScope;
            }
        }
        
        public override string GetHandlerName()
        {
            return "Google";
        }
        
        public override void RequestAuthorizationCode()
        {
            string accessType = "online";
            if (StoreToken)
            	accessType = "offline";
            string authUrl = String.Format("https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={0}&red" +
                    "irect_uri={1}&scope={2}&state={3}&access_type={4}&prompt=select_account", Config.ClientId, Uri.EscapeDataString(Config.RedirectUri), Uri.EscapeDataString(Scope), Uri.EscapeDataString(GetState()), accessType);
            HttpContext.Current.Response.Redirect(authUrl);
        }
        
        protected override WebRequest GetAccessTokenRequest(string code, bool refresh)
        {
            WebRequest request = WebRequest.Create("https://www.googleapis.com/oauth2/v4/token");
            request.Method = "POST";
            string codeType = "code";
            if (refresh)
            	codeType = "access_token";
            string body = String.Format("{0}={1}&client_id={2}&client_secret={3}&redirect_uri={4}&grant_type=authorization" +
                    "_code", codeType, code, Config.ClientId, Config.ClientSecret, Config.RedirectUri);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            using (Stream stream = request.GetRequestStream())
            	stream.Write(bodyBytes, 0, bodyBytes.Length);
            return request;
        }
        
        protected override WebRequest GetQueryRequest(string method, string token)
        {
            WebRequest request = WebRequest.Create(("https://www.googleapis.com/" + method));
            request.Headers[HttpRequestHeader.Authorization] = ("Bearer " + token);
            return request;
        }
        
        public override string GetUserName()
        {
            _userObj = Query("userinfo/v2/me", false);
            return ((string)(_userObj["email"]));
        }
        
        public override List<string> GetUserRoles(MembershipUser user)
        {
            List<string> roles = base.GetUserRoles(user);
            JObject result = Query(("admin/directory/v1/groups?userKey=" + ((string)(_userObj["id"]))), true);
            if (result != null)
            	foreach (JObject group in ((JArray)(result["groups"])))
                	roles.Add(((string)(group["name"])));
            else
            	throw new Exception("Unable to get roles.");
            return roles;
        }
        
        public override string GetUserImageUrl(MembershipUser user)
        {
            return ((string)(_userObj["picture"]));
        }
    }
    
    public partial class SharePointOAuthHandler : SharePointOAuthHandlerBase
    {
    }
    
    public class SharePointOAuthHandlerBase : OAuthHandler
    {
        
        private string _realm;
        
        private string _showNavigation;
        
        private JObject _userObj;
        
        protected override string Scope
        {
            get
            {
                if (StoreToken)
                	return "Web.Read AllProfiles.Read";
                else
                	return String.Empty;
            }
        }
        
        public override string GetHandlerName()
        {
            return "SharePoint";
        }
        
        public override void RequestAuthorizationCode()
        {
            string version = Config["Version"];
            if (String.IsNullOrEmpty(version))
            	version = "15";
            string authUrl = String.Format("{0}/_layouts/{1}/OAuthAuthorize.aspx?client_id={2}&scope={3}&response_type=code&r" +
                    "edirect_uri={4}&state={5}", ClientUri, version, Config.ClientId, Uri.EscapeDataString(Scope), Uri.EscapeDataString(Config.RedirectUri), Uri.EscapeDataString(GetState()));
            HttpContext.Current.Response.Redirect(authUrl);
        }
        
        protected override WebRequest GetAccessTokenRequest(string code, bool refresh)
        {
            if (String.IsNullOrEmpty(_realm))
            {
                WebRequest getRealm = WebRequest.Create((ClientUri + "/_vti_bin/client.svc"));
                getRealm.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
                WebResponse response;
                try
                {
                    response = getRealm.GetResponse();
                }
                catch (WebException ex)
                {
                    response = ex.Response;
                }
                string wwwAuthentication = response.Headers["WWW-Authenticate"];
                Match realmMatch = Regex.Match(wwwAuthentication, "Bearer realm=\"(.+?)\"");
                _realm = realmMatch.Groups[1].Value;
            }
            WebRequest request = WebRequest.Create((("https://accounts.accesscontrol.windows.net/" + _realm) 
                            + "/tokens/OAuth/2"));
            request.Method = "POST";
            string grantType = "authorization_code";
            string codeName = "code";
            if (refresh)
            {
                grantType = "refresh_token";
                codeName = grantType;
            }
            string body = String.Format("grant_type={0}&client_id={1}%40{2}&client_secret={3}&{4}={5}&redirect_uri={6}&res" +
                    "ource=00000003-0000-0ff1-ce00-000000000000%2F{7}%40{2}", grantType, Config.ClientId, _realm, Uri.EscapeDataString(Config.ClientSecret), codeName, code, Config.RedirectUri, ClientUri.Substring(8));
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            using (Stream stream = request.GetRequestStream())
            	stream.Write(bodyBytes, 0, bodyBytes.Length);
            return request;
        }
        
        protected override WebRequest GetQueryRequest(string method, string token)
        {
            WebRequest request = WebRequest.Create(String.Format("{0}/_api/{1}", ClientUri, method));
            ((HttpWebRequest)(request)).Accept = "application/json;odata=verbose";
            request.Headers[HttpRequestHeader.Authorization] = ("Bearer " + token);
            return request;
        }
        
        public override JObject Query(string method, bool useSystemToken)
        {
            JObject result = base.Query(method, useSystemToken);
            if ((result == null) || (result["d"] == null))
            	return result;
            return ((JObject)(result["d"]));
        }
        
        public override string GetState()
        {
            return ((base.GetState() + "|showNavigation=") 
                        + _showNavigation);
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
        
        public override string GetUserName()
        {
            _userObj = Query("Web/CurrentUser", false);
            return ((string)(_userObj["LoginName"])).Split('|').Last();
        }
        
        public override List<string> GetUserRoles(MembershipUser user)
        {
            List<string> roles = base.GetUserRoles(user);
            JObject groups = Query(("Web/GetUserById(" 
                            + (((string)(_userObj["Id"])) + ")/Groups")), !(StoreToken));
            if (groups == null)
            	throw new Exception("Unable to get roles.");
            foreach (JToken group in ((JArray)(groups["results"])).Children())
            {
                string name = ((string)(group["LoginName"]));
                if (!(name.StartsWith("SharingLinks.")))
                	roles.Add(name);
            }
            return roles;
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
            StartPage = ((StartPage 
                        + (connector + "_showNavigation=")) 
                        + _showNavigation);
            base.RedirectToStartPage(context);
        }
        
        public override string GetUserImageUrl(MembershipUser user)
        {
            JObject info = Query(String.Format("SP.UserProfiles.PeopleManager/GetPropertiesFor(accountName=@v)?@v=\'{0}\'&$select=P" +
                        "ictureUrl", HttpUtility.UrlEncode(((string)(_userObj["LoginName"])))), true);
            return ((string)(info["PictureUrl"]));
        }
    }
    
    public partial class WindowsLiveOAuthHandler : WindowsLiveOAuthHandlerBase
    {
    }
    
    public partial class WindowsLiveOAuthHandlerBase : OAuthHandler
    {
        
        protected override string Scope
        {
            get
            {
                return "wl.emails";
            }
        }
        
        public override string GetHandlerName()
        {
            return "WindowsLive";
        }
        
        public override void RequestAuthorizationCode()
        {
            string authUrl = String.Format("https://login.live.com/oauth20_authorize.srf?client_id={0}&scope={1}&response_typ" +
                    "e=code&redirect_uri={2}&state={3}", Config.ClientId, Uri.EscapeDataString(Scope), Uri.EscapeDataString(Config.RedirectUri), Uri.EscapeDataString(GetState()));
            HttpContext.Current.Response.Redirect(authUrl);
        }
        
        protected override WebRequest GetAccessTokenRequest(string code, bool refresh)
        {
            WebRequest request = WebRequest.Create("https://login.live.com/oauth20_token.srf");
            request.Method = "POST";
            string codeName = "code";
            string grantType = "authorization_code";
            if (refresh)
            {
                codeName = "refresh_token";
                grantType = codeName;
            }
            string body = String.Format("client_id={0}&redirect_uri={1}&client_secret={2}&{3}={4}&grant_type={5}", Config.ClientId, Config.RedirectUri, Config.ClientSecret, codeName, code, grantType);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            using (Stream stream = request.GetRequestStream())
            	stream.Write(bodyBytes, 0, bodyBytes.Length);
            return request;
        }
        
        protected override WebRequest GetQueryRequest(string method, string token)
        {
            string version = Config["Version"];
            if (String.IsNullOrEmpty(version))
            	version = "v5.0";
            return WebRequest.Create(String.Format("https://apis.live.net/{0}/{1}?access_token={2}", version, method, token));
        }
        
        public override string GetUserName()
        {
            JObject userObj = Query("me", false);
            return ((string)(userObj["emails"]["account"]));
        }
    }
    
    public partial class IdentityServerOAuthHandler : IdentityServerOAuthHandlerBase
    {
    }
    
    public partial class IdentityServerOAuthHandlerBase : OAuthHandler
    {
        
        protected override string Scope
        {
            get
            {
                string configScope = Config["Scope"];
                if (!(String.IsNullOrEmpty(configScope)))
                	return configScope;
                return "openid email";
            }
        }
        
        public override string GetHandlerName()
        {
            return "IdentityServer";
        }
        
        public override void RequestAuthorizationCode()
        {
            string authUrl = String.Format("{0}/connect/authorize?response_type=code&client_id={1}&redirect_uri={2}&scope={3}" +
                    "&state={4}", ClientUri, Config.ClientId, Config.RedirectUri, Uri.EscapeDataString(Scope), Uri.EscapeDataString(GetState()));
            HttpContext.Current.Response.Redirect(authUrl);
        }
        
        protected override WebRequest GetAccessTokenRequest(string code, bool refresh)
        {
            WebRequest request = WebRequest.Create((ClientUri + "/connect/token"));
            request.Method = "POST";
            string codeName = "code";
            string grantType = "authorization_code";
            if (refresh)
            {
                codeName = "refresh_token";
                grantType = codeName;
            }
            string body = String.Format("client_id={0}&redirect_uri={1}&client_secret={2}&{3}={4}&grant_type={5}", Config.ClientId, Config.RedirectUri, Config.ClientSecret, codeName, code, grantType);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            using (Stream stream = request.GetRequestStream())
            	stream.Write(bodyBytes, 0, bodyBytes.Length);
            return request;
        }
        
        protected override WebRequest GetQueryRequest(string method, string token)
        {
            WebRequest request = WebRequest.Create(String.Format("{0}/connect/{1}", ClientUri, method));
            request.Headers[HttpRequestHeader.Authorization] = ("Bearer " + token);
            return request;
        }
        
        public override string GetUserName()
        {
            JObject userObj = Query("userinfo", false);
            return ((string)(userObj["email"]));
        }
    }
    
    public partial class MSGraphOAuthHandler : MSGraphOAuthHandlerBase
    {
    }
    
    public partial class MSGraphOAuthHandlerBase : OAuthHandler
    {
        
        private string _userID = null;
        
        private string _tenantID;
        
        protected override string Scope
        {
            get
            {
                if (StoreToken)
                	return "https://graph.microsoft.com/.default";
                else
                	return "User.Read";
            }
        }
        
        public virtual string TenantID
        {
            get
            {
                if (String.IsNullOrEmpty(_tenantID))
                {
                    _tenantID = Config["Tenant ID"];
                    if (String.IsNullOrEmpty(_tenantID))
                    	_tenantID = "common";
                }
                return _tenantID;
            }
        }
        
        public override string GetHandlerName()
        {
            return "MSGraph";
        }
        
        public override void RequestAuthorizationCode()
        {
            StringBuilder sb = new StringBuilder("https://login.microsoftonline.com/");
            sb.Append(TenantID);
            if (!(StoreToken))
            	sb.Append("/oauth2/v2.0/authorize");
            else
            	sb.Append("/adminconsent");
            sb.AppendFormat("?client_id={0}&redirect_uri={1}&state={2}", Config.ClientId, Uri.EscapeDataString(Config.RedirectUri), Uri.EscapeDataString(GetState()));
            if (!(StoreToken))
            {
                sb.Append("&response_type=code&response_mode=query&scope=");
                sb.Append(Uri.EscapeDataString(Scope));
            }
            HttpContext.Current.Response.Redirect(sb.ToString());
        }
        
        protected override WebRequest GetAccessTokenRequest(string code, bool refresh)
        {
            WebRequest request = WebRequest.Create(String.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", TenantID));
            request.Method = "POST";
            string body = String.Format("client_id={0}&redirect_uri={1}&client_secret={2}&scope={3}", Config.ClientId, Uri.EscapeDataString(Config.RedirectUri), Config.ClientSecret, Uri.EscapeDataString(Scope));
            if (refresh)
            	body = (body 
                            + ("&grant_type=refresh_token&refresh_token=" + code));
            else
            	if (!(StoreToken))
                	body = (body 
                                + ("&grant_type=authorization_code&code=" + code));
                else
                	body = (body + "&grant_type=client_credentials");
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            using (Stream stream = request.GetRequestStream())
            	stream.Write(bodyBytes, 0, bodyBytes.Length);
            return request;
        }
        
        protected override WebRequest GetQueryRequest(string method, string token)
        {
            WebRequest request = WebRequest.Create(("https://graph.microsoft.com/v1.0/" + method));
            request.Headers[HttpRequestHeader.Authorization] = ("Bearer " + token);
            return request;
        }
        
        protected override bool RefreshTokens(bool useSystemToken)
        {
            if (!(useSystemToken))
            	return base.RefreshTokens(useSystemToken);
            StoreToken = true;
            Tokens = GetAccessTokens("True", false);
            StoreTokens(Tokens);
            StoreToken = false;
            return !(String.IsNullOrEmpty(Tokens[0]));
        }
        
        public override string GetUserName()
        {
            JObject userObj = Query("me", false);
            _userID = ((string)(userObj["id"]));
            return ((string)(userObj["userPrincipalName"]));
        }
        
        public override List<string> GetUserRoles(MembershipUser user)
        {
            List<string> roles = base.GetUserRoles(user);
            JObject roleObj = Query((("users/" + _userID) 
                            + "/memberOf"), true);
            if (roleObj != null)
            	foreach (JToken role in ((JArray)(roleObj["value"])))
                	roles.Add(((string)(role["displayName"])));
            return roles;
        }
        
        protected override string GetAuthCode(HttpRequest request)
        {
            if (StoreToken)
            	return request.QueryString["admin_consent"];
            return base.GetAuthCode(request);
        }
    }
}
