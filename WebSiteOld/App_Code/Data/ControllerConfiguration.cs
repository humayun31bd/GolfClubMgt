using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.XPath;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Reflection;
using MyCompany.Services;
using Newtonsoft.Json.Linq;

namespace MyCompany.Data
{
	public class ControllerConfiguration
    {
        
        private XPathNavigator _navigator;
        
        private XmlNamespaceManager _namespaceManager;
        
        private IXmlNamespaceResolver _resolver;
        
        private string _actionHandlerType;
        
        private string _dataFilterType;
        
        private string _handlerType;
        
        public static Regex VariableDetectionRegex = new Regex("\\$\\w+\\$");
        
        public static Regex VariableReplacementRegex = new Regex("\\$(\\w+)\\$([\\s\\S]*?)\\$(\\w+)\\$");
        
        public static Regex LocalizationDetectionRegex = new Regex("\\^\\w+\\^");
        
        public const string Namespace = "urn:schemas-codeontime-com:data-aquarium";
        
        private string _connectionStringName;
        
        private string _controllerName;
        
        private bool _conflictDetectionEnabled;
        
        private DynamicExpression[] _expressions;
        
        private IPlugIn _plugIn;
        
        private string _rawConfiguration;
        
        private bool _usesVariables;
        
        private bool _requiresLocalization;
        
        public ControllerConfiguration(string path) : 
                this(File.OpenRead(path))
        {
        }
        
        public ControllerConfiguration(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            this._rawConfiguration = sr.ReadToEnd();
            sr.Close();
            this._usesVariables = VariableDetectionRegex.IsMatch(this._rawConfiguration);
            this._requiresLocalization = LocalizationDetectionRegex.IsMatch(this._rawConfiguration);
            Initialize(new XPathDocument(new StringReader(this._rawConfiguration)).CreateNavigator());
        }
        
        public ControllerConfiguration(XPathDocument document) : 
                this(document.CreateNavigator())
        {
        }
        
        public ControllerConfiguration(XPathNavigator navigator)
        {
            Initialize(navigator);
        }
        
        public string ConnectionStringName
        {
            get
            {
                return _connectionStringName;
            }
        }
        
        public string ControllerName
        {
            get
            {
                return _controllerName;
            }
        }
        
        public bool ConflictDetectionEnabled
        {
            get
            {
                return _conflictDetectionEnabled;
            }
        }
        
        public IXmlNamespaceResolver Resolver
        {
            get
            {
                return _resolver;
            }
        }
        
        public XPathNavigator Navigator
        {
            get
            {
                return _navigator;
            }
        }
        
        public DynamicExpression[] Expressions
        {
            get
            {
                return _expressions;
            }
            set
            {
                _expressions = value;
            }
        }
        
        public IPlugIn PlugIn
        {
            get
            {
                return _plugIn;
            }
        }
        
        public string RawConfiguration
        {
            get
            {
                return _rawConfiguration;
            }
        }
        
        public bool UsesVariables
        {
            get
            {
                return _usesVariables;
            }
        }
        
        public bool RequiresLocalization
        {
            get
            {
                return _requiresLocalization;
            }
        }
        
        public XPathNavigator TrimmedNavigator
        {
            get
            {
                List<string> hiddenFields = new List<string>();
                XPathNodeIterator fieldIterator = Select("/c:dataController/c:fields/c:field[@roles!=\'\']");
                while (fieldIterator.MoveNext())
                {
                    string roles = fieldIterator.Current.GetAttribute("roles", String.Empty);
                    if (!(DataControllerBase.UserIsInRole(roles)))
                    	hiddenFields.Add(fieldIterator.Current.GetAttribute("name", String.Empty));
                }
                if (hiddenFields.Count == 0)
                	return Navigator;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Navigator.OuterXml);
                XPathNavigator nav = doc.CreateNavigator();
                XPathNodeIterator dataFieldIterator = nav.Select("//c:dataField", Resolver);
                while (dataFieldIterator.MoveNext())
                	if (hiddenFields.Contains(dataFieldIterator.Current.GetAttribute("fieldName", String.Empty)))
                    {
                        XPathNavigator hiddenAttr = dataFieldIterator.Current.SelectSingleNode("@hidden");
                        if (hiddenAttr == null)
                        	dataFieldIterator.Current.CreateAttribute(String.Empty, "hidden", String.Empty, "true");
                        else
                        	hiddenAttr.SetValue("true");
                    }
                return nav;
            }
        }
        
        public ControllerConfiguration Virtualize(string controllerName)
        {
            ControllerConfiguration config = this;
            if (!(_navigator.CanEdit))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_navigator.OuterXml);
                config = new ControllerConfiguration(doc.CreateNavigator());
            }
            return config;
        }
        
        protected virtual void Initialize(XPathNavigator navigator)
        {
            _navigator = navigator;
            _namespaceManager = new XmlNamespaceManager(_navigator.NameTable);
            _namespaceManager.AddNamespace("c", ControllerConfiguration.Namespace);
            _resolver = ((IXmlNamespaceResolver)(_namespaceManager));
            ResolveBaseViews();
            _controllerName = ((string)(Evaluate("string(/c:dataController/@name)")));
            _handlerType = ((string)(Evaluate("string(/c:dataController/@handler)")));
            if (String.IsNullOrEmpty(_handlerType))
            {
                Type t = Type.GetType("MyCompany.Rules.SharedBusinessRules");
                if (t != null)
                	_handlerType = t.FullName;
            }
            _actionHandlerType = _handlerType;
            _dataFilterType = _handlerType;
            string s = ((string)(Evaluate("string(/c:dataController/@actionHandlerType)")));
            if (!(String.IsNullOrEmpty(s)))
            	_actionHandlerType = s;
            s = ((string)(Evaluate("string(/c:dataController/@dataFilterType)")));
            if (!(String.IsNullOrEmpty(s)))
            	_dataFilterType = s;
            string plugInType = ((string)(Evaluate("string(/c:dataController/@plugIn)")));
            if (!(String.IsNullOrEmpty(plugInType)) && ApplicationServices.IsTouchClient)
            	plugInType = String.Empty;
            if (!(String.IsNullOrEmpty(plugInType)))
            {
                Type t = Type.GetType(plugInType);
                _plugIn = ((IPlugIn)(t.Assembly.CreateInstance(t.FullName)));
                _plugIn.Config = this;
            }
        }
        
        public virtual void Complete()
        {
            _connectionStringName = ((string)(Evaluate("string(/c:dataController/@connectionStringName)")));
            if (String.IsNullOrEmpty(_connectionStringName))
            	_connectionStringName = "MyCompany";
            _conflictDetectionEnabled = ((bool)(Evaluate("/c:dataController/@conflictDetection=\'compareAllValues\'")));
            List<DynamicExpression> expressions = new List<DynamicExpression>();
            XPathNodeIterator expressionIterator = Select("//c:expression[@test!=\'\' or @result!=\'\']");
            while (expressionIterator.MoveNext())
            	expressions.Add(new DynamicExpression(expressionIterator.Current, _namespaceManager));
            XPathNodeIterator ruleIterator = Select("/c:dataController/c:businessRules/c:rule[@type=\'JavaScript\']");
            while (ruleIterator.MoveNext())
            {
                DynamicExpression rule = new DynamicExpression();
                rule.Type = DynamicExpressionType.ClientScript;
                rule.Scope = DynamicExpressionScope.Rule;
                XPathNavigator ruleNav = ruleIterator.Current;
                rule.Result = String.Format("<id>{0}</id><command>{1}</command><argument>{2}</argument><view>{3}</view><phase>" +
                        "{4}</phase><js>{5}</js>", ruleNav.GetAttribute("id", String.Empty), ruleNav.GetAttribute("commandName", String.Empty), ruleNav.GetAttribute("commandArgument", String.Empty), ruleNav.GetAttribute("view", String.Empty), ruleNav.GetAttribute("phase", String.Empty), ruleNav.Value);
                expressions.Add(rule);
            }
            _expressions = expressions.ToArray();
        }
        
        private void EnsureChildNode(XPathNavigator parent, string nodeName)
        {
            XPathNavigator child = parent.SelectSingleNode(String.Format("c:{0}", nodeName), _resolver);
            if (child == null)
            	parent.AppendChild(String.Format("<{0}/>", nodeName));
        }
        
        public virtual ControllerConfiguration EnsureVitalElements()
        {
            // verify that the data controller has views and actions
            XPathNavigator root = SelectSingleNode("/c:dataController[c:views/c:view and c:actions/c:actionGroup]");
            if (root != null)
            	return this;
            // add missing configuration elements
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_navigator.OuterXml);
            ControllerConfiguration config = new ControllerConfiguration(doc.CreateNavigator());
            XPathNavigator fieldsNode = config.SelectSingleNode("/c:dataController/c:fields[not(c:field[@isPrimaryKey=\'true\'])]");
            if (fieldsNode != null)
            	fieldsNode.AppendChild("<field name=\"PrimaryKey\" type=\"Int32\" isPrimaryKey=\"true\" readOnly=\"true\"/>");
            root = config.SelectSingleNode("/c:dataController");
            EnsureChildNode(root, "views");
            XPathNavigator viewsNode = config.SelectSingleNode("/c:dataController/c:views[not(c:view)]");
            if (viewsNode != null)
            {
                StringBuilder sb = new StringBuilder("<view id=\"view1\" type=\"Form\" label=\"Form\"><categories><category id=\"c1\" flow=\"New" +
                        "Column\"><dataFields>");
                XPathNodeIterator fieldIterator = config.Select("/c:dataController/c:fields/c:field");
                while (fieldIterator.MoveNext())
                {
                    string fieldName = fieldIterator.Current.GetAttribute("name", String.Empty);
                    bool hidden = (fieldName == "PrimaryKey");
                    string length = fieldIterator.Current.GetAttribute("length", String.Empty);
                    if (String.IsNullOrEmpty(length) && (((bool)(fieldIterator.Current.Evaluate("not(c:items/@style!=\'\')", _resolver))) == true))
                    	if (fieldIterator.Current.GetAttribute("type", String.Empty) == "String")
                        	length = "50";
                        else
                        	length = "20";
                    sb.AppendFormat("<dataField fieldName=\"{0}\" hidden=\"{1}\"", fieldName, hidden.ToString().ToLower());
                    if (!(String.IsNullOrEmpty(length)))
                    	sb.AppendFormat(" columns=\"{0}\"", length);
                    sb.Append(" />");
                }
                sb.Append("</dataFields></category></categories></view>");
                viewsNode.AppendChild(sb.ToString());
            }
            EnsureChildNode(root, "actions");
            XPathNavigator actionsNode = config.SelectSingleNode("/c:dataController/c:actions[not(c:actionGroup)]");
            if (actionsNode != null)
            	actionsNode.AppendChild(@"<actionGroup id=""ag1"" scope=""Form"">
<action id=""a1"" commandName=""Confirm"" causesValidation=""true"" whenLastCommandName=""New"" />
<action id=""a2"" commandName=""Cancel"" whenLastCommandName=""New"" />
<action id=""a3"" commandName=""Confirm"" causesValidation=""true"" whenLastCommandName=""Edit"" />
<action id=""a4"" commandName=""Cancel"" whenLastCommandName=""Edit"" />
<action id=""a5"" commandName=""Edit"" causesValidation=""true"" />
</actionGroup>");
            XPathNavigator plugIn = config.SelectSingleNode("/c:dataController/@plugIn");
            if (plugIn != null)
            {
                plugIn.DeleteSelf();
                config._plugIn = null;
            }
            return config;
        }
        
        protected virtual void ResolveBaseViews()
        {
            XPathNavigator firstUnresolvedView = SelectSingleNode("/c:dataController/c:views/c:view[@baseViewId!=\'\' and not (.//c:dataField)]");
            if (firstUnresolvedView != null)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(_navigator.OuterXml);
                _navigator = document.CreateNavigator();
                XPathNodeIterator unresolvedViewIterator = Select("/c:dataController/c:views/c:view[@baseViewId!=\'\']");
                while (unresolvedViewIterator.MoveNext())
                {
                    string baseViewId = unresolvedViewIterator.Current.GetAttribute("baseViewId", String.Empty);
                    unresolvedViewIterator.Current.SelectSingleNode("@baseViewId").DeleteSelf();
                    XPathNavigator baseView = SelectSingleNode(String.Format("/c:dataController/c:views/c:view[@id=\'{0}\']", baseViewId));
                    if (baseView != null)
                    {
                        List<XPathNavigator> nodesToDelete = new List<XPathNavigator>();
                        XPathNodeIterator emptyNodeIterator = unresolvedViewIterator.Current.Select("c:*[not(child::*) and .=\'\']", _resolver);
                        while (emptyNodeIterator.MoveNext())
                        	nodesToDelete.Add(emptyNodeIterator.Current.Clone());
                        foreach (XPathNavigator n in nodesToDelete)
                        	n.DeleteSelf();
                        XPathNodeIterator copyNodeIterator = baseView.Select("c:*", _resolver);
                        while (copyNodeIterator.MoveNext())
                        	if (unresolvedViewIterator.Current.SelectSingleNode(("c:" + copyNodeIterator.Current.LocalName), _resolver) == null)
                            	unresolvedViewIterator.Current.AppendChild(copyNodeIterator.Current.OuterXml);
                    }
                }
                _navigator = new XPathDocument(new StringReader(_navigator.OuterXml)).CreateNavigator();
            }
        }
        
        private void InitializeHandler(object handler)
        {
            if ((handler != null) && (handler is BusinessRules))
            	((BusinessRules)(handler)).ControllerName = ControllerName;
        }
        
        public BusinessRules CreateBusinessRules()
        {
            IActionHandler handler = CreateActionHandler();
            if (handler == null)
            	return null;
            else
            {
                BusinessRules rules = ((BusinessRules)(handler));
                rules.Config = this;
                return rules;
            }
        }
        
        public IActionHandler CreateActionHandler()
        {
            if (String.IsNullOrEmpty(_actionHandlerType))
            	return null;
            else
            {
                Type t = Type.GetType(_actionHandlerType);
                object handler = t.Assembly.CreateInstance(t.FullName);
                InitializeHandler(handler);
                if (handler is BusinessRules)
                	((BusinessRules)(handler)).Config = this;
                return ((IActionHandler)(handler));
            }
        }
        
        public IDataFilter CreateDataFilter()
        {
            if (String.IsNullOrEmpty(_dataFilterType))
            	return null;
            else
            {
                Type t = Type.GetType(_dataFilterType);
                object dataFilter = t.Assembly.CreateInstance(t.FullName);
                InitializeHandler(dataFilter);
                if (typeof(IDataFilter).IsInstanceOfType(dataFilter))
                	return ((IDataFilter)(dataFilter));
                else
                	return null;
            }
        }
        
        public IRowHandler CreateRowHandler()
        {
            if (String.IsNullOrEmpty(_actionHandlerType))
            	return null;
            else
            {
                Type t = Type.GetType(_actionHandlerType);
                object handler = t.Assembly.CreateInstance(t.FullName);
                InitializeHandler(handler);
                if (typeof(IRowHandler).IsInstanceOfType(handler))
                	return ((IRowHandler)(handler));
                else
                	return null;
            }
        }
        
        public void AssignDynamicExpressions(ViewPage page)
        {
            List<DynamicExpression> list = new List<DynamicExpression>();
            if (page.IncludeMetadata("expressions"))
            	foreach (DynamicExpression de in _expressions)
                	if (de.AllowedInView(page.View))
                    	list.Add(de);
            page.Expressions = list.ToArray();
        }
        
        public ControllerConfiguration Clone()
        {
            string variablesPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Controllers\\_variables.xml");
            SortedDictionary<string, string> variables = ((SortedDictionary<string, string>)(HttpRuntime.Cache[variablesPath]));
            if (variables == null)
            {
                variables = new SortedDictionary<string, string>();
                if (File.Exists(variablesPath))
                {
                    XPathDocument varDoc = new XPathDocument(variablesPath);
                    XPathNavigator varNav = varDoc.CreateNavigator();
                    XPathNodeIterator varIterator = varNav.Select("/variables/variable");
                    while (varIterator.MoveNext())
                    {
                        string varName = varIterator.Current.GetAttribute("name", String.Empty);
                        string varValue = varIterator.Current.Value;
                        if (!(variables.ContainsKey(varName)))
                        	variables.Add(varName, varValue);
                        else
                        	variables[varName] = varValue;
                    }
                }
                HttpRuntime.Cache.Insert(variablesPath, variables, new CacheDependency(variablesPath));
            }
            return new ControllerConfiguration(new XPathDocument(new StringReader(new ControllerConfigurationUtility(_rawConfiguration, variables).ReplaceVariables())));
        }
        
        public ControllerConfiguration Localize(string controller)
        {
            string localizedContent = Localizer.Replace("Controllers", (controller + ".xml"), _navigator.OuterXml);
            if (PlugIn != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(localizedContent);
                return new ControllerConfiguration(doc.CreateNavigator());
            }
            else
            	return new ControllerConfiguration(new XPathDocument(new StringReader(localizedContent)));
        }
        
        public XPathNavigator SelectSingleNode(string selector, params System.Object[] args)
        {
            return _navigator.SelectSingleNode(String.Format(selector, args), _resolver);
        }
        
        public XPathNodeIterator Select(string selector, params System.Object[] args)
        {
            return _navigator.Select(String.Format(selector, args), _resolver);
        }
        
        public object Evaluate(string selector, params System.Object[] args)
        {
            return _navigator.Evaluate(String.Format(selector, args), _resolver);
        }
        
        public string ReadActionData(string path)
        {
            if (!(String.IsNullOrEmpty(path)))
            {
                string[] p = path.Split('/');
                if (p.Length == 2)
                {
                    XPathNavigator dataNav = SelectSingleNode("/c:dataController/c:actions/c:actionGroup[@id=\'{0}\']/c:action[@id=\'{1}\']/c:data", p[0], p[1]);
                    if (dataNav != null)
                    	return dataNav.Value;
                }
            }
            return null;
        }
        
        public void ParseActionData(string path, SortedDictionary<string, string> variables)
        {
            string data = ReadActionData(path);
            if (!(String.IsNullOrEmpty(data)))
            {
                Match m = Regex.Match(data, "^\\s*(\\w+)\\s*=\\s*(.+?)\\s*$", RegexOptions.Multiline);
                while (m.Success)
                {
                    variables[m.Groups[1].Value] = m.Groups[2].Value;
                    m = m.NextMatch();
                }
            }
        }
        
        public string LoadLayout(string view)
        {
            string viewLayout = null;
            // load the view layout
            string fileName = String.Format("{0}.{1}.html", this.ControllerName, view);
            bool tryLoad = true;
            while (tryLoad)
            {
                fileName = Path.Combine(Path.Combine(HttpRuntime.AppDomainAppPath, "Views"), fileName);
                if (File.Exists(fileName))
                	viewLayout = File.ReadAllText(fileName);
                else
                {
                    Stream stream = GetType().Assembly.GetManifestResourceStream(String.Format("MyCompany.Views.{0}.{1}.html", this.ControllerName, view));
                    if (stream != null)
                    	using (StreamReader sr = new StreamReader(stream))
                        	viewLayout = sr.ReadToEnd();
                }
                if ((viewLayout != null) && Regex.IsMatch(viewLayout, "^\\s*\\w+\\.\\w+\\.html\\s*$", RegexOptions.IgnoreCase))
                	fileName = viewLayout;
                else
                	tryLoad = false;
            }
            return viewLayout;
        }
        
        public string ToJson()
        {
            ControllerConfiguration config = this.Virtualize(this.ControllerName);
            Complete();
            XPathNodeIterator ruleIterator = config.Select("/c:dataController/c:businessRules/c:rule");
            bool newOnServer = false;
            bool calculateOnServer = false;
            while (ruleIterator.MoveNext())
            {
                string type = ruleIterator.Current.GetAttribute("type", String.Empty);
                string commandName = ruleIterator.Current.GetAttribute("commandName", String.Empty);
                if (type != "JavaScript")
                	if ((commandName == "New") && !(newOnServer))
                    {
                        newOnServer = true;
                        config.SelectSingleNode("/c:dataController").CreateAttribute(String.Empty, "newOnServer", null, "true");
                    }
                    else
                    	if ((commandName == "Calculate") && !(calculateOnServer))
                        {
                            calculateOnServer = true;
                            config.SelectSingleNode("/c:dataController").CreateAttribute(String.Empty, "calculateOnServer", null, "true");
                        }
            }
            string expressions = JArray.FromObject(this.Expressions).ToString();
            string[] exceptions = new string[] {
                    "//comment()",
                    "c:dataController/c:commands",
                    "c:dataController/@handler",
                    "//c:field/c:formula",
                    "//c:businessRules/c:rule[@type=\"Code\" or @type=\"Sql\" or @type=\"Email\"]",
                    "//c:businessRules/c:rule/text()",
                    "//c:validate",
                    "//c:styles",
                    "//c:visibility",
                    "//c:readOnly",
                    "//c:expression"};
            foreach (string ex in exceptions)
            {
                List<XPathNavigator> toDelete = new List<XPathNavigator>();
                XPathNodeIterator iterator = config.Select(ex);
                while (iterator.MoveNext())
                	toDelete.Add(iterator.Current.Clone());
                foreach (XPathNavigator node in toDelete)
                	node.DeleteSelf();
            }
            // special case of items/item serialization
            XPathNodeIterator itemsIterator = config.Select("//c:items[c:item]");
            while (itemsIterator.MoveNext())
            {
                StringBuilder lovBuilder = new StringBuilder("<list>");
                XPathNodeIterator itemIterator = itemsIterator.Current.SelectChildren(XPathNodeType.Element);
                while (itemIterator.MoveNext())
                	lovBuilder.Append(itemIterator.Current.OuterXml);
                lovBuilder.Append("</list>");
                itemsIterator.Current.InnerXml = lovBuilder.ToString();
            }
            // load custom view layouts
            XPathNodeIterator viewIterator = config.Select("//c:views/c:view");
            while (viewIterator.MoveNext())
            {
                string layout = LoadLayout(viewIterator.Current.GetAttribute("id", String.Empty));
                if (!(String.IsNullOrEmpty(layout)))
                	viewIterator.Current.AppendChild(String.Format("<layout><![CDATA[{0}]]></layout>", layout));
            }
            // extend JSON with "expressions"
            string json = XmlConverter.ToJson(config.Navigator, "dataController", true, "commands", "output", "fields", "views", "categories", "dataFields", "actions", "actionGroup", "businessRules", "list");
            Match eof = Regex.Match(json, "\\}\\s*\\}\\s*$");
            json = (json.Substring(0, eof.Index) 
                        + (",\"expressions\":" 
                        + (expressions + eof.Value)));
            return json;
        }
    }
    
    public class ControllerConfigurationUtility
    {
        
        private static SortedDictionary<string, string> _assemblyResources;
        
        private string _rawConfiguration;
        
        private SortedDictionary<string, string> _variables;
        
        static ControllerConfigurationUtility()
        {
            _assemblyResources = new SortedDictionary<string, string>();
            Assembly a = typeof(ControllerConfigurationUtility).Assembly;
            foreach (string resource in a.GetManifestResourceNames())
            	_assemblyResources[resource.ToLowerInvariant()] = resource;
        }
        
        public ControllerConfigurationUtility(string rawConfiguration, SortedDictionary<string, string> variables)
        {
            _rawConfiguration = rawConfiguration;
            _variables = variables;
        }
        
        public string ReplaceVariables()
        {
            return ControllerConfiguration.VariableReplacementRegex.Replace(_rawConfiguration, DoReplace);
        }
        
        private string DoReplace(Match m)
        {
            if (m.Groups[1].Value == m.Groups[3].Value)
            {
                string s = null;
                if (_variables.TryGetValue(m.Groups[1].Value, out s))
                	return s;
                else
                	return m.Groups[2].Value;
            }
            return m.Value;
        }
        
        public static Stream GetResourceStream(params string[] resourceNames)
        {
            Assembly a = typeof(ControllerConfigurationUtility).Assembly;
            string resourceName = null;
            foreach (string resource in resourceNames)
            	if (_assemblyResources.TryGetValue(resource.ToLowerInvariant(), out resourceName))
                	return a.GetManifestResourceStream(resourceName);
            return null;
        }
        
        public static string GetResourceText(params string[] resourceNames)
        {
            Stream res = GetResourceStream(resourceNames);
            if (res == null)
            	return null;
            using (StreamReader sr = new StreamReader(res))
            	return sr.ReadToEnd();
        }
        
        public static string GetFilePath(params string[] paths)
        {
            foreach (string path in paths)
            	if (File.Exists(path))
                	return path;
            return null;
        }
        
        public static string GetFileText(params string[] paths)
        {
            string path = GetFilePath(paths);
            if (!(String.IsNullOrEmpty(path)))
            	return File.ReadAllText(path);
            return null;
        }
    }
    
    public class XmlConverter
    {
        
        private XPathNavigator _navigator;
        
        private string[] _arrays = null;
        
        private bool _renderMetadata = false;
        
        private string _root;
        
        private StringBuilder _sb;
        
        public XmlConverter(XPathNavigator navigator, string root, bool metadata, string[] arrays)
        {
            _navigator = navigator;
            _root = root;
            _renderMetadata = metadata;
            _arrays = arrays;
        }
        
        public static string ToJson(XPathNavigator navigator, string root, bool metadata, params System.String[] arrays)
        {
            XmlConverter xmlc = new XmlConverter(navigator, root, metadata, arrays);
            return xmlc.ToJson();
        }
        
        public string ToJson()
        {
            XPathNavigator nav = _navigator;
            _sb = new StringBuilder("{\n");
            while (nav.Name != _root && nav.MoveToFirstChild())
            {
            }
            XmlToJson(nav, false, 1);
            _sb.AppendLine("\n}");
            return _sb.ToString();
        }
        
        private void WriteJsonValue(XPathNavigator nav)
        {
            string v = nav.ToString();
            int tempInt32;
            if (int.TryParse(v, out tempInt32))
            	_sb.Append(tempInt32);
            else
            {
                bool tempBool;
                if (bool.TryParse(v, out tempBool))
                	_sb.Append(tempBool.ToString().ToLower());
                else
                	_sb.Append(HttpUtility.JavaScriptStringEncode(v, true));
            }
        }
        
        private void WriteMultilineValue(XPathNavigator nav)
        {
            string type = null;
            XPathNavigator props = nav.CreateNavigator();
            bool keepGoing = true;
            while (keepGoing)
            {
                props.MoveToParent();
                if (props.MoveToFirstAttribute())
                	keepGoing = false;
            }
            keepGoing = true;
            while (keepGoing)
            {
                if (props.Name == "type")
                	type = props.Value;
                if (!(props.MoveToNextAttribute()))
                	keepGoing = false;
            }
            if (String.IsNullOrEmpty(type))
            	WriteJsonValue(nav);
            else
            {
                props.MoveToRoot();
                props.MoveToFirstChild();
                WriteJsonValue(nav);
            }
        }
        
        private void XmlToJson(XPathNavigator nav, bool isArrayMember, int depth)
        {
            string padding = new string(' ', (depth * 2));
            bool isArray = _arrays.Contains(nav.Name);
            bool isComplexArray = (isArray && nav.HasAttributes);
            bool closingBracket = true;
            if (!(isComplexArray))
            {
                if (!(isArrayMember))
                {
                    _sb.AppendFormat((padding + "\"{0}\": "), nav.Name);
                    if (nav.MoveToFirstChild())
                    {
                        if (nav.NodeType == XPathNodeType.Text)
                        	closingBracket = false;
                        nav.MoveToParent();
                    }
                }
                if (closingBracket)
                	if (isArray)
                    	_sb.AppendLine("[");
                    else
                    	if (!(isArrayMember))
                        	_sb.AppendLine("{");
                        else
                        	_sb.AppendLine((padding + "{"));
            }
            bool firstProp = true;
            string childPadding = (padding + "  ");
            bool keepGoing;
            if (isComplexArray && isArrayMember)
            	_sb.AppendLine((padding + "{"));
            if (nav.MoveToFirstAttribute())
            {
                keepGoing = true;
                while (keepGoing)
                {
                    if (firstProp)
                    	firstProp = false;
                    else
                    	_sb.AppendLine(",");
                    _sb.AppendFormat((childPadding + "\"{0}\": "), nav.Name);
                    WriteJsonValue(nav);
                    if (!(nav.MoveToNextAttribute()))
                    	keepGoing = false;
                }
                nav.MoveToParent();
                if (isComplexArray)
                {
                    _sb.AppendLine(",");
                    _sb.AppendFormat((childPadding + "\"{0}\": [\n"), nav.Name);
                    firstProp = true;
                }
            }
            if (nav.MoveToFirstChild())
            {
                if (nav.NodeType == XPathNodeType.Text)
                {
                    if (isArrayMember)
                    {
                        _sb.AppendLine(",");
                        _sb.Append((childPadding + "\"@text\": "));
                    }
                    else
                    {
                        _sb.AppendLine(" {");
                        _sb.Append((childPadding + "\"@value\": "));
                    }
                    if (nav.Value.Contains("\n"))
                    	WriteMultilineValue(nav);
                    else
                    	WriteJsonValue(nav);
                    if (!(isArrayMember))
                    	_sb.Append(("\n" 
                                        + (padding + "}")));
                }
                else
                {
                    keepGoing = true;
                    while (keepGoing)
                    {
                        if (firstProp)
                        	firstProp = false;
                        else
                        	_sb.AppendLine(",");
                        XmlToJson(nav, isArray, (depth + 1));
                        if (!(nav.MoveToNext()))
                        	keepGoing = false;
                    }
                }
                nav.MoveToParent();
            }
            if (closingBracket)
            {
                _sb.AppendLine();
                if (isComplexArray)
                	_sb.Append((padding + "  ]"));
                else
                	if (isArray)
                    	_sb.Append((padding + "]"));
                    else
                    	_sb.Append((padding + "}"));
            }
            if (isComplexArray && isArrayMember)
            	_sb.Append(("\n" 
                                + (padding + "}")));
            if (nav.MoveToNext())
            {
                _sb.AppendLine(",");
                XmlToJson(nav, isArrayMember, depth);
            }
        }
    }
}
