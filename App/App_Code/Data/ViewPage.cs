﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Security;
using System.Globalization;
using System.IO;
using MyCompany.Services;

namespace MyCompany.Data
{
	public class ViewPage
    {
        
        private int _skipCount;
        
        private int _readCount;
        
        private string[] _originalFilter;
        
        private string _controller;
        
        private string _view;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _viewType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _timeStamp;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _pageIndex;
        
        private int _pageSize;
        
        private int _pageOffset;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _tag;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresMetaData;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _fieldFilter;
        
        private string[] _metadataFilter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresSiteContentText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresPivot;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Dictionary<string, string> _pivotDefinitions;
        
        private SortedDictionary<int, PivotTable> _pivots = new SortedDictionary<int, PivotTable>();
        
        private static int _targetDataPoints = 25;
        
        private bool _requiresRowCount;
        
        private bool _disableJSONCompatibility;
        
        private object[] _aggregates;
        
        private List<DataField> _fields;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _sortExpression;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _groupExpression;
        
        private int _totalRowCount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _clientScript;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _firstLetters;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _filter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _systemFilter;
        
        private string _distinctValueFieldName;
        
        private List<View> _views;
        
        private DynamicExpression[] _expressions;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _supportsCaching;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _lastView;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _statusBar;
        
        private bool _allowDistinctFieldInFilter;
        
        private string[] _icons;
        
        private FieldValue[] _levs;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _quickFindHint;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _innerJoinPrimaryKey;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _innerJoinForeignKey;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _viewHeaderText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _viewLayout;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _distinct;
        
        private List<ActionGroup> _actionGroups;
        
        private List<Category> _categories;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private List<string> _errors;
        
        private object[] _newRow;
        
        private List<object[]> _rows;
        
        private string _staticLookupViewTypeFilter;
        
        [ThreadStatic]
        public static bool PopulatingStaticItems;
        
        private SortedDictionary<string, object> _customFilter;
        
        private static Regex _stringNumSplitter = new Regex("(?\'PropertyName\'.+?)(?\'PropertyValue\'\\d+)?$");
        
        private static string[] _supportedChartTypes = new string[] {
                "annotation",
                "area",
                "areastacked",
                "bar",
                "barstacked",
                "bubble",
                "candlestick",
                "combo",
                "column",
                "columnstacked",
                "diff",
                "gauge",
                "geo",
                "histogram",
                "interval",
                "line",
                "map",
                "org",
                "pie",
                "pie3d",
                "donut",
                "sankey",
                "scatter",
                "steppedarea",
                "table",
                "timeline",
                "treemap",
                "trendline",
                "wordtree"};
        
        private static string[] _supportedBuckets = new string[] {
                "timeofday",
                "second",
                "minute",
                "halfhour",
                "hour",
                "day",
                "dayofweek",
                "dayofyear",
                "weekofmonth",
                "week",
                "weekofyear",
                "twoweek",
                "twoweeks",
                "month",
                "quarter",
                "year"};
        
        private static string[] _supportedModes = new string[] {
                "sum",
                "min",
                "max",
                "avg",
                "count"};
        
        private Regex _pivotTagRegex = new Regex("(?\'tag\'pivot[\\w-]+(:(\".+?\"|\'.+?\'|\\S))?)");
        
        private string[] _requestedFieldFilter;
        
        public ViewPage() : 
                this(new PageRequest(0, 0, null, null))
        {
        }
        
        public ViewPage(DistinctValueRequest request) : 
                this(new PageRequest(0, 0, null, request.Filter))
        {
            _tag = request.Tag;
            _distinctValueFieldName = request.FieldName;
            _pageSize = request.MaximumValueCount;
            _allowDistinctFieldInFilter = request.AllowFieldInFilter;
            _controller = request.Controller;
            _view = request.View;
            _filter = request.Filter;
            _quickFindHint = request.QuickFindHint;
        }
        
        public ViewPage(PageRequest request)
        {
            _tag = request.Tag;
            this.PageOffset = request.PageOffset;
            _requiresMetaData = ((request.PageIndex == -1) || request.RequiresMetaData);
            _requiresRowCount = ((request.PageIndex < 0) || request.RequiresRowCount);
            if (request.PageIndex == -2)
            	request.PageIndex = 0;
            _pageSize = request.PageSize;
            if (request.RequiresPivot)
            {
                RequiresPivot = true;
                _requiresMetaData = false;
                _requiresRowCount = false;
                _pageSize = Int32.MaxValue;
                PivotDefinitions = new Dictionary<string, string>();
                if (!(String.IsNullOrEmpty(request.PivotDefinitions)))
                	foreach (string definition in request.PivotDefinitions.Split(';'))
                    {
                        string[] def = definition.Split('=');
                        if (def.Length == 2)
                        	PivotDefinitions.Add(def[0], def[1].Replace(',', ' '));
                    }
            }
            if (request.PageIndex > 0)
            	_pageIndex = request.PageIndex;
            _rows = new List<object[]>();
            _errors = new List<string>();
            _fields = new List<DataField>();
            ResetSkipCount(false);
            _readCount = _pageSize;
            _sortExpression = request.SortExpression;
            _groupExpression = request.GroupExpression;
            _filter = request.Filter;
            _systemFilter = request.SystemFilter;
            _totalRowCount = -1;
            _views = new List<View>();
            _actionGroups = new List<ActionGroup>();
            _categories = new List<Category>();
            _controller = request.Controller;
            _view = request.View;
            _lastView = request.LastView;
            _viewType = request.ViewType;
            _supportsCaching = request.SupportsCaching;
            _quickFindHint = request.QuickFindHint;
            _innerJoinPrimaryKey = request.InnerJoinPrimaryKey;
            _innerJoinForeignKey = request.InnerJoinForeignKey;
            _requiresSiteContentText = request.RequiresSiteContentText;
            _disableJSONCompatibility = request.DisableJSONCompatibility;
            _fieldFilter = request.FieldFilter;
            _requestedFieldFilter = request.FieldFilter;
            _metadataFilter = request.MetadataFilter;
            _distinct = request.Distinct;
            _staticLookupViewTypeFilter = "Form";
            if (!(String.IsNullOrEmpty(request.Tag)) && Regex.IsMatch(request.Tag, "\\bview\\-type\\-inline\\-editor\\b"))
            	_staticLookupViewTypeFilter = "Grid";
            _timeStamp = ("ts" + DateTime.Now.ToString("s"));
        }
        
        public string Controller
        {
            get
            {
                return _controller;
            }
        }
        
        public string View
        {
            get
            {
                return _view;
            }
        }
        
        public string ViewType
        {
            get
            {
                return this._viewType;
            }
            set
            {
                this._viewType = value;
            }
        }
        
        public string TimeStamp
        {
            get
            {
                return this._timeStamp;
            }
            set
            {
                this._timeStamp = value;
            }
        }
        
        public int PageIndex
        {
            get
            {
                return this._pageIndex;
            }
            set
            {
                this._pageIndex = value;
            }
        }
        
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
        }
        
        public int PageOffset
        {
            get
            {
                return _pageOffset;
            }
            set
            {
                _pageOffset = value;
            }
        }
        
        public string Tag
        {
            get
            {
                return this._tag;
            }
            set
            {
                this._tag = value;
            }
        }
        
        public bool RequiresMetaData
        {
            get
            {
                return this._requiresMetaData;
            }
            set
            {
                this._requiresMetaData = value;
            }
        }
        
        public string[] FieldFilter
        {
            get
            {
                return this._fieldFilter;
            }
            set
            {
                this._fieldFilter = value;
            }
        }
        
        public virtual bool RequiresSiteContentText
        {
            get
            {
                return this._requiresSiteContentText;
            }
            set
            {
                this._requiresSiteContentText = value;
            }
        }
        
        public virtual bool RequiresPivot
        {
            get
            {
                return this._requiresPivot;
            }
            set
            {
                this._requiresPivot = value;
            }
        }
        
        public virtual Dictionary<string, string> PivotDefinitions
        {
            get
            {
                return this._pivotDefinitions;
            }
            set
            {
                this._pivotDefinitions = value;
            }
        }
        
        public virtual PivotTable[] Pivots
        {
            get
            {
                // Eliminate auto pivots
                Dictionary<int, List<PivotTable>> groups = new Dictionary<int, List<PivotTable>>();
                foreach (PivotTable pivot in _pivots.Values)
                	if (pivot.Group != 0)
                    {
                        if (!(groups.ContainsKey(pivot.Group)))
                        	groups[pivot.Group] = new List<PivotTable>();
                        groups[pivot.Group].Add(pivot);
                    }
                foreach (KeyValuePair<int, List<PivotTable>> groupKvp in groups)
                {
                    PivotTable candidate = null;
                    foreach (PivotTable p in groupKvp.Value)
                    {
                        if (candidate == null)
                        	candidate = p;
                        else
                        {
                            int delta = Math.Abs((_targetDataPoints - p.Rows.Count));
                            int candidateDelta = Math.Abs((_targetDataPoints - candidate.Rows.Count));
                            if (delta < candidateDelta)
                            	candidate = p;
                        }
                        _pivots.Remove(p.Id);
                    }
                    PivotTable original = _pivots[groupKvp.Key];
                    candidate.Id = original.Id;
                    candidate.Name = original.Name;
                    candidate.ChartType = original.ChartType;
                    _pivots[groupKvp.Key] = candidate;
                }
                return _pivots.Values.ToArray();
            }
        }
        
        public bool RequiresRowCount
        {
            get
            {
                return _requiresRowCount;
            }
        }
        
        public bool RequiresAggregates
        {
            get
            {
                foreach (DataField field in Fields)
                	if (field.Aggregate != DataFieldAggregate.None)
                    	return true;
                return false;
            }
        }
        
        public object[] Aggregates
        {
            get
            {
                return _aggregates;
            }
            set
            {
                _aggregates = value;
            }
        }
        
        public List<DataField> Fields
        {
            get
            {
                return _fields;
            }
        }
        
        public string SortExpression
        {
            get
            {
                return this._sortExpression;
            }
            set
            {
                this._sortExpression = value;
            }
        }
        
        public string GroupExpression
        {
            get
            {
                return this._groupExpression;
            }
            set
            {
                this._groupExpression = value;
            }
        }
        
        public int TotalRowCount
        {
            get
            {
                return _totalRowCount;
            }
            set
            {
                _totalRowCount = value;
                int pageCount = (value / this.PageSize);
                if ((value % this.PageSize) > 0)
                	pageCount++;
                if (pageCount <= PageIndex)
                	this._pageIndex = 0;
            }
        }
        
        public string ClientScript
        {
            get
            {
                return this._clientScript;
            }
            set
            {
                this._clientScript = value;
            }
        }
        
        public string FirstLetters
        {
            get
            {
                return this._firstLetters;
            }
            set
            {
                this._firstLetters = value;
            }
        }
        
        public string[] Filter
        {
            get
            {
                return this._filter;
            }
            set
            {
                this._filter = value;
            }
        }
        
        public string[] SystemFilter
        {
            get
            {
                return this._systemFilter;
            }
            set
            {
                this._systemFilter = value;
            }
        }
        
        public string DistinctValueFieldName
        {
            get
            {
                return _distinctValueFieldName;
            }
        }
        
        public List<View> Views
        {
            get
            {
                return _views;
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
        
        public bool SupportsCaching
        {
            get
            {
                return this._supportsCaching;
            }
            set
            {
                this._supportsCaching = value;
            }
        }
        
        public string LastView
        {
            get
            {
                return this._lastView;
            }
            set
            {
                this._lastView = value;
            }
        }
        
        public string StatusBar
        {
            get
            {
                return this._statusBar;
            }
            set
            {
                this._statusBar = value;
            }
        }
        
        public bool AllowDistinctFieldInFilter
        {
            get
            {
                return _allowDistinctFieldInFilter;
            }
        }
        
        public string[] Icons
        {
            get
            {
                return _icons;
            }
            set
            {
                _icons = value;
            }
        }
        
        public bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }
        
        public FieldValue[] LEVs
        {
            get
            {
                return _levs;
            }
            set
            {
                _levs = value;
            }
        }
        
        public string QuickFindHint
        {
            get
            {
                return this._quickFindHint;
            }
            set
            {
                this._quickFindHint = value;
            }
        }
        
        public string InnerJoinPrimaryKey
        {
            get
            {
                return this._innerJoinPrimaryKey;
            }
            set
            {
                this._innerJoinPrimaryKey = value;
            }
        }
        
        public string InnerJoinForeignKey
        {
            get
            {
                return this._innerJoinForeignKey;
            }
            set
            {
                this._innerJoinForeignKey = value;
            }
        }
        
        public string ViewHeaderText
        {
            get
            {
                return this._viewHeaderText;
            }
            set
            {
                this._viewHeaderText = value;
            }
        }
        
        public string ViewLayout
        {
            get
            {
                return this._viewLayout;
            }
            set
            {
                this._viewLayout = value;
            }
        }
        
        public bool Distinct
        {
            get
            {
                return this._distinct;
            }
            set
            {
                this._distinct = value;
            }
        }
        
        public List<ActionGroup> ActionGroups
        {
            get
            {
                return _actionGroups;
            }
        }
        
        public List<Category> Categories
        {
            get
            {
                return _categories;
            }
        }
        
        public List<string> Errors
        {
            get
            {
                return this._errors;
            }
            set
            {
                this._errors = value;
            }
        }
        
        public object[] NewRow
        {
            get
            {
                return _newRow;
            }
            set
            {
                _newRow = value;
            }
        }
        
        public List<object[]> Rows
        {
            get
            {
                return _rows;
            }
        }
        
        public bool IncludeField(string name)
        {
            return ((_fieldFilter == null) || _fieldFilter.Contains(name));
        }
        
        public bool IncludeMetadata(string name)
        {
            return ((_metadataFilter == null) || _metadataFilter.Contains(name));
        }
        
        public void ChangeFilter(string[] filter)
        {
            _filter = filter;
            _originalFilter = null;
        }
        
        public bool SkipNext()
        {
            if (_skipCount == 0)
            	return false;
            _skipCount--;
            return true;
        }
        
        public void ResetSkipCount(bool preFetch)
        {
            if (preFetch)
            {
                _skipCount = ((_pageIndex - 1) 
                            * _pageSize);
                _readCount = (_readCount * 3);
                if (_skipCount < 0)
                {
                    _skipCount = 0;
                    _readCount = (_readCount - _pageSize);
                }
            }
            else
            	_skipCount = (_pageIndex * _pageSize);
        }
        
        public bool ReadNext()
        {
            if (_readCount == 0)
            	return false;
            _readCount--;
            return true;
        }
        
        public void AcceptAllRows()
        {
            _readCount = Int32.MaxValue;
            _skipCount = 0;
        }
        
        public bool ContainsField(string name)
        {
            return (FindField(name) != null);
        }
        
        public DataField FindField(string name)
        {
            foreach (DataField field in Fields)
            	if (field.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                	return field;
            return null;
        }
        
        public int IndexOfField(string name)
        {
            for (int i = 0; (i < Fields.Count); i++)
            {
                DataField field = Fields[i];
                if (field.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                	return i;
            }
            return -1;
        }
        
        public bool PopulateStaticItems(DataField field, FieldValue[] contextValues)
        {
            if (!(IncludeMetadata("items")))
            	return false;
            if (field.SupportsStaticItems())
            	InitializeManyToManyProperties(field);
            if (field.SupportsStaticItems() && (String.IsNullOrEmpty(field.ContextFields) || (contextValues != null)))
            {
                if (PopulatingStaticItems)
                	return true;
                PopulatingStaticItems = true;
                try
                {
                    string[] filter = null;
                    if (!(String.IsNullOrEmpty(field.ContextFields)))
                    {
                        List<string> contextFilter = new List<string>();
                        Match m = Regex.Match(field.ContextFields, "(\\w+)\\s*=\\s*(.+?)($|,)");
                        SortedDictionary<string, List<string>> staticContextValues = new SortedDictionary<string, List<string>>();
                        while (m.Success)
                        {
                            Match vm = Regex.Match(m.Groups[2].Value, "^(\\\'(?\'Value\'.+?)\\\'|(?\'Value\'\\d+))$");
                            if (vm.Success)
                            {
                                List<string> lov = null;
                                if (!(staticContextValues.TryGetValue(m.Groups[1].Value, out lov)))
                                {
                                    lov = new List<string>();
                                    staticContextValues[m.Groups[1].Value] = lov;
                                }
                                lov.Add(vm.Groups["Value"].Value);
                            }
                            else
                            	if (contextValues != null)
                                	foreach (FieldValue cv in contextValues)
                                    	if (cv.Name == m.Groups[2].Value)
                                        {
                                            if (cv.NewValue == null)
                                            	return true;
                                            contextFilter.Add(String.Format("{0}:={1}", m.Groups[1].Value, cv.NewValue));
                                            break;
                                        }
                            m = m.NextMatch();
                        }
                        foreach (string fieldName in staticContextValues.Keys)
                        {
                            List<string> lov = staticContextValues[fieldName];
                            if (lov.Count == 1)
                            	contextFilter.Add(String.Format("{0}:={1}", fieldName, lov[0]));
                            else
                            	contextFilter.Add(String.Format("{0}:$in${1}", fieldName, String.Join("$or$", lov.ToArray())));
                        }
                        filter = contextFilter.ToArray();
                    }
                    string sortExpression = null;
                    if (String.IsNullOrEmpty(field.ItemsTargetController) && String.IsNullOrEmpty(field.ItemsDataView))
                    	sortExpression = field.ItemsDataTextField;
                    PageRequest request = new PageRequest(0, 1000, sortExpression, filter);
                    request.RequiresMetaData = true;
                    if (ActionArgs.Current != null)
                    	request.ExternalFilter = ActionArgs.Current.ExternalFilter;
                    request.MetadataFilter = new string[] {
                            "fields"};
                    ViewPage page = ControllerFactory.CreateDataController().GetPage(field.ItemsDataController, field.ItemsDataView, request);
                    int dataValueFieldIndex = page.Fields.IndexOf(page.FindField(field.ItemsDataValueField));
                    if (dataValueFieldIndex == -1)
                    	foreach (DataField aField in page.Fields)
                        	if (aField.IsPrimaryKey)
                            {
                                dataValueFieldIndex = page.Fields.IndexOf(aField);
                                break;
                            }
                    int dataTextFieldIndex = page.Fields.IndexOf(page.FindField(field.ItemsDataTextField));
                    if (dataTextFieldIndex == -1)
                    {
                        int i = 0;
                        while ((dataTextFieldIndex == -1) && (i < page.Fields.Count))
                        {
                            DataField f = page.Fields[i];
                            if (!(f.Hidden) && (f.Type == "String"))
                            	dataTextFieldIndex = i;
                            i++;
                        }
                        if (dataTextFieldIndex == -1)
                        	dataTextFieldIndex = 0;
                    }
                    List<int> fieldIndexes = new List<int>();
                    fieldIndexes.Add(dataValueFieldIndex);
                    fieldIndexes.Add(dataTextFieldIndex);
                    if (!(String.IsNullOrEmpty(field.Copy)))
                    {
                        Match m = Regex.Match(field.Copy, "(\\w+)\\s*=\\s*(\\w+)");
                        while (m.Success)
                        {
                            int copyFieldIndex = page.Fields.IndexOf(page.FindField(m.Groups[2].Value));
                            fieldIndexes.Add(copyFieldIndex);
                            m = m.NextMatch();
                        }
                    }
                    foreach (object[] row in page.Rows)
                    {
                        object[] values = new object[fieldIndexes.Count];
                        for (int i = 0; (i < fieldIndexes.Count); i++)
                        {
                            int copyFieldIndex = fieldIndexes[i];
                            if (copyFieldIndex >= 0)
                            	values[i] = row[copyFieldIndex];
                            else
                            	values[i] = null;
                        }
                        field.Items.Add(values);
                    }
                    return true;
                }
                finally
                {
                    PopulatingStaticItems = false;
                }
            }
            return false;
        }
        
        public ViewPage ToResult(ControllerConfiguration configuration, XPathNavigator mainView)
        {
            if (!(_requiresMetaData))
            {
                Fields.Clear();
                Expressions = null;
            }
            else
            {
                if (IncludeMetadata("views"))
                {
                    XPathNodeIterator viewIterator = configuration.Select("/c:dataController/c:views/c:view[not(@virtualViewId!=\'\')]");
                    while (viewIterator.MoveNext())
                    {
                        View v = new View(viewIterator.Current, mainView, configuration.Resolver);
                        if (v.Id == this.View)
                        	ViewHeaderText = v.HeaderText();
                        Views.Add(v);
                    }
                }
                if (IncludeMetadata("layouts"))
                	ViewLayout = configuration.LoadLayout(this.View);
                if (IncludeMetadata("actions"))
                {
                    XPathNodeIterator actionGroupIterator = configuration.Select("/c:dataController/c:actions//c:actionGroup");
                    while (actionGroupIterator.MoveNext())
                    	ActionGroups.Add(new ActionGroup(actionGroupIterator.Current, configuration.Resolver));
                }
                if (IncludeMetadata("items"))
                {
                    FieldValue[] contextValues = null;
                    object[] row = NewRow;
                    if ((row == null) && (Rows.Count >= 1))
                    	row = Rows[0];
                    if (row != null)
                    {
                        List<FieldValue> valueList = new List<FieldValue>();
                        int i = 0;
                        foreach (DataField field in Fields)
                        {
                            valueList.Add(new FieldValue(field.Name, row[i]));
                            i++;
                        }
                        contextValues = valueList.ToArray();
                    }
                    foreach (DataField field in Fields)
                    	if (!(ApplicationServices.IsTouchClient) || ((field.ItemsStyle == "CheckBoxList") || (!(String.IsNullOrEmpty(field.ItemsTargetController)) || (configuration.SelectSingleNode("/c:dataController/c:views/c:view[@id=\'{0}\']/@type", this.View).Value == _staticLookupViewTypeFilter))))
                        	PopulateStaticItems(field, contextValues);
                }
            }
            if (_originalFilter != null)
            	_filter = _originalFilter;
            if (_filter != null)
            	for (int i = 0; (i < _filter.Length); i++)
                {
                    string f = _filter[i];
                    if (f.StartsWith("_match_") || f.StartsWith("_donotmatch_"))
                    {
                        string[] oldFilter = _filter;
                        _filter = new string[i];
                        Array.Copy(oldFilter, _filter, i);
                        break;
                    }
                }
            if (new ControllerUtilities().SupportsLastEnteredValues(this.Controller))
            {
                if (RequiresMetaData && ((HttpContext.Current != null) && (HttpContext.Current.Session != null)))
                	LEVs = ((FieldValue[])(HttpContext.Current.Session[String.Format("{0}$LEVs", _controller)]));
            }
            if (!(_disableJSONCompatibility))
            {
                DataControllerBase.EnsureJsonCompatibility(NewRow);
                DataControllerBase.EnsureJsonCompatibility(Rows);
            }
            if (!(IncludeMetadata("fields")))
            	Fields.Clear();
            else
            	foreach (DataField f in Fields)
                {
                    if (!(String.IsNullOrEmpty(f.Formula)))
                    	f.Formula = null;
                    InitializeManyToManyProperties(f);
                }
            return this;
        }
        
        public DataTable ToDataTable()
        {
            return ToDataTable("table");
        }
        
        public DataTable ToDataTable(string tableName)
        {
            DataTable table = new DataTable(tableName);
            List<Type> columnTypes = new List<Type>();
            foreach (DataField field in Fields)
            {
                System.Type t = typeof(string);
                if (!(((field.Type == "Byte[]") || (field.Type == "DataView"))))
                	t = DataControllerBase.TypeMap[field.Type];
                table.Columns.Add(field.Name, t);
                columnTypes.Add(t);
            }
            foreach (object[] row in Rows)
            {
                DataRow newRow = table.NewRow();
                for (int i = 0; (i < Fields.Count); i++)
                {
                    object v = row[i];
                    if (v == null)
                    	v = DBNull.Value;
                    else
                    {
                        Type t = columnTypes[i];
                        if ((t == typeof(DateTime)) && (v is string))
                        	v = DateTime.Parse(((string)(v)));
                        else
                        	if ((t == typeof(DateTimeOffset)) && (v is string))
                            {
                                DateTimeOffset dto;
                                if (DateTimeOffset.TryParse(((string)(v)), out dto))
                                	v = dto;
                                else
                                	v = DBNull.Value;
                            }
                    }
                    newRow[i] = v;
                }
                table.Rows.Add(newRow);
            }
            table.AcceptChanges();
            return table;
        }
        
        public List<T> ToList<T>()
        
        {
            Type objectType = typeof(T);
            List<T> list = new List<T>();
            object[] args = new object[1];
            Type[] types = new Type[Fields.Count];
            for (int j = 0; (j < Fields.Count); j++)
            {
                System.Reflection.PropertyInfo propInfo = objectType.GetProperty(Fields[j].Name);
                if (propInfo != null)
                	types[j] = propInfo.PropertyType;
            }
            foreach (object[] row in Rows)
            {
                T instance = ((T)(objectType.Assembly.CreateInstance(objectType.FullName)));
                int i = 0;
                foreach (DataField field in Fields)
                {
                    try
                    {
                        Type fieldType = types[i];
                        if (fieldType != null)
                        {
                            args[0] = DataControllerBase.ConvertToType(fieldType, row[i]);
                            objectType.InvokeMember(field.Name, System.Reflection.BindingFlags.SetProperty, null, instance, args);
                        }
                    }
                    catch (Exception )
                    {
                    }
                    i++;
                }
                list.Add(instance);
            }
            return list;
        }
        
        public bool CustomFilteredBy(string fieldName)
        {
            return ((_customFilter != null) && _customFilter.ContainsKey(fieldName));
        }
        
        public void ApplyDataFilter(IDataFilter dataFilter, string controller, string view, string lookupContextController, string lookupContextView, string lookupContextFieldName)
        {
            if (dataFilter == null)
            	return;
            if (_filter == null)
            	_filter = new string[0];
            IDataFilter2 dataFilter2 = null;
            if (typeof(IDataFilter2).IsInstanceOfType(dataFilter))
            {
                dataFilter2 = ((IDataFilter2)(dataFilter));
                dataFilter2.AssignContext(controller, view, lookupContextController, lookupContextView, lookupContextFieldName);
            }
            List<string> newFilter = new List<string>(_filter);
            _customFilter = new SortedDictionary<string, object>();
            if (dataFilter2 != null)
            	dataFilter2.Filter(controller, view, _customFilter);
            else
            	dataFilter.Filter(_customFilter);
            foreach (string key in _customFilter.Keys)
            {
                object v = _customFilter[key];
                if ((v == null) || !(v.GetType().IsArray))
                	v = new object[] {
                            v};
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}:", key);
                foreach (object item in ((Array)(v)))
                {
                    if (dataFilter2 != null)
                    	sb.Append(item);
                    else
                    	sb.AppendFormat("={0}", item);
                    sb.Append(Convert.ToChar(0));
                }
                newFilter.Add(sb.ToString());
            }
            _originalFilter = _filter;
            _filter = newFilter.ToArray();
        }
        
        public void UpdateFieldValue(string fieldName, object[] row, object value)
        {
            for (int i = 0; (i < Fields.Count); i++)
            	if (Fields[i].Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                	row[i] = value;
        }
        
        public object SelectFieldValue(string fieldName, object[] row)
        {
            for (int i = 0; (i < Fields.Count); i++)
            	if (Fields[i].Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                	return row[i];
            return null;
        }
        
        public FieldValue SelectFieldValueObject(string fieldName, object[] row)
        {
            for (int i = 0; (i < Fields.Count); i++)
            	if (Fields[i].Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                	return new FieldValue(fieldName, row[i]);
            return null;
        }
        
        public void RemoveFromFilter(string fieldName)
        {
            if (_filter == null)
            	return;
            List<string> newFilter = new List<string>(_filter);
            string prefix = (fieldName + ":");
            foreach (string s in newFilter)
            	if (s.StartsWith(prefix))
                {
                    newFilter.Remove(s);
                    break;
                }
            _filter = newFilter.ToArray();
        }
        
        public bool RequiresResultSet(CommandConfigurationType configuration)
        {
            if (!(String.IsNullOrEmpty(QuickFindHint)))
            	return true;
            if (_filter != null)
            	foreach (string s in _filter)
                {
                    Match m = DataControllerBase.FilterExpressionRegex.Match(s);
                    if (m.Success == m.Groups["Alias"].Value.Contains(","))
                    	return true;
                }
            return false;
        }
        
        public virtual void InitializeManyToManyProperties(DataField field)
        {
            string key1 = null;
            string key2 = null;
            if (!(String.IsNullOrEmpty(field.ItemsTargetController)) && String.IsNullOrEmpty(field.ItemsDataValueField))
            	ViewPage.InitializeManyToManyProperties(field, _controller, out key1, out key2);
        }
        
        public static void InitializeManyToManyProperties(DataField field, string controller, out string targetForeignKey1, out string targetForeignKey2)
        {
            Controller target = new Controller();
            target.SelectView(field.ItemsTargetController, null);
            XPathNodeIterator field1 = target.Config.Select(String.Format("/c:dataController/c:fields/c:field[c:items/@dataController=\'{0}\']", controller));
            XPathNodeIterator field2 = target.Config.Select(String.Format("/c:dataController/c:fields/c:field[c:items/@dataController=\'{0}\']", field.ItemsDataController));
            if (!(field1.MoveNext()))
            	throw new Exception(String.Format("Field with lookup controller \'{0}\' not found in target controller \'{1}\'.", controller, field.ItemsTargetController));
            if (!(field2.MoveNext()))
            	throw new Exception(String.Format("Field with lookup controller \'{0}\' not found in target controller \'{1}\'.", field.ItemsDataController, field.ItemsTargetController));
            targetForeignKey1 = field1.Current.GetAttribute("name", String.Empty);
            targetForeignKey2 = field2.Current.GetAttribute("name", String.Empty);
            XPathNodeIterator field2items = target.Config.Select(String.Format("/c:dataController/c:fields/c:field[@name=\'{0}\']/c:items", targetForeignKey2));
            if (field2items.MoveNext())
            {
                field.ItemsDataValueField = field2items.Current.GetAttribute("dataValueField", String.Empty);
                field.ItemsDataTextField = field2items.Current.GetAttribute("dataTextField", String.Empty);
                field.ItemsDataView = field2items.Current.GetAttribute("dataView", String.Empty);
            }
        }
        
        public virtual void AddPivotField(DataField field)
        {
            // process tags
            MatchCollection tags = _pivotTagRegex.Matches(field.Tag);
            foreach (Capture tagCapture in tags)
            {
                string tag = tagCapture.Value;
                if (tag.StartsWith("pivot"))
                {
                    string[] properties = tag.Split('-');
                    if (properties.Length >= 2)
                    {
                        // populate properties
                        int pivotID = 0;
                        string chartType = String.Empty;
                        string fieldType = String.Empty;
                        bool subtotalsEnabled = false;
                        bool grandTotalsEnabled = false;
                        int fieldTypeIndex = 0;
                        Dictionary<string, object> additionalProperties = new Dictionary<string, object>();
                        foreach (string property in properties)
                        {
                            Match match = _stringNumSplitter.Match(property);
                            if (match.Success)
                            {
                                string propertyName = ((string)(match.Groups["PropertyName"].Value.ToLower().Trim()));
                                int propertyValue;
                                string propertyValueString = String.Empty;
                                if (!(int.TryParse(match.Groups["PropertyValue"].Value, out propertyValue)))
                                {
                                    propertyValue = 0;
                                    if (property.Contains(':'))
                                    {
                                        propertyName = property.Split(':')[0];
                                        propertyValueString = property.Split(':')[1];
                                        propertyValueString = propertyValueString.Substring(1, (propertyValueString.Length - 2));
                                    }
                                }
                                if (propertyName == "pivot")
                                	pivotID = propertyValue;
                                else
                                	if (((propertyName == "row") || (propertyName == "col")) || ((propertyName == "val") || (propertyName == "value")))
                                    {
                                        fieldType = propertyName;
                                        fieldTypeIndex = propertyValue;
                                    }
                                    else
                                    	if (propertyName == "column")
                                        	if (propertyValue == 0)
                                            	chartType = propertyName;
                                            else
                                            {
                                                fieldType = propertyName;
                                                fieldTypeIndex = propertyValue;
                                            }
                                        else
                                        	if ((propertyName == "subtotal") || (propertyName == "subtotals"))
                                            	subtotalsEnabled = true;
                                            else
                                            	if ((propertyName == "grandtotal") || (propertyName == "grandtotals"))
                                                	grandTotalsEnabled = true;
                                                else
                                                	if (_supportedChartTypes.Contains(propertyName))
                                                    	chartType = propertyName;
                                                    else
                                                    	if (String.IsNullOrEmpty(propertyValueString))
                                                        	additionalProperties.Add(propertyName.ToLower(), propertyValue);
                                                        else
                                                        	additionalProperties.Add(propertyName.ToLower(), propertyValueString);
                            }
                        }
                        // get or add pivot
                        PivotTable pivot;
                        if (!(_pivots.ContainsKey(pivotID)))
                        {
                            pivot = new PivotTable(pivotID, this);
                            _pivots.Add(pivotID, pivot);
                        }
                        else
                        	pivot = _pivots[pivotID];
                        if (!(String.IsNullOrEmpty(chartType)))
                        	pivot.ChartType = chartType;
                        if (subtotalsEnabled)
                        	pivot.SubtotalsEnabled = true;
                        if (grandTotalsEnabled)
                        	pivot.GrandTotalsEnabled = true;
                        // add field to pivot
                        if (!(String.IsNullOrEmpty(fieldType)))
                        {
                            FieldInfo fi = null;
                            if (fieldType == "row")
                            	fi = pivot.GetRowField(fieldTypeIndex, field);
                            else
                            	if (fieldType.StartsWith("col"))
                                	fi = pivot.GetColumnField(fieldTypeIndex, field);
                                else
                                	if (fieldType.StartsWith("val"))
                                    	fi = pivot.GetValueField(fieldTypeIndex, field);
                                    else
                                    	return;
                            // check properties
                            for (int i = 0; (i < additionalProperties.Count); i++)
                            {
                                KeyValuePair<string, object> kvp = additionalProperties.ElementAt(i);
                                bool remove = true;
                                string property = kvp.Key;
                                if (_supportedModes.Contains(property))
                                	fi.Mode = property;
                                else
                                	if (_supportedBuckets.Contains(property))
                                    	fi.Bucket = property;
                                    else
                                    	if (property == "date")
                                        {
                                            // expand auto date
                                            int newPivotID = (int.MaxValue 
                                                        - (pivotID * 6));
                                            string newTag = (String.Format("pivotgroup{0}-", pivotID) 
                                                        + (Regex.Replace(tag, "pivot(\\d+?)-|row(\\d+?)|date|all", String.Empty) + "-pivot"));
                                            field.Tag = ((newTag + newPivotID.ToString()) 
                                                        + "-row1-year ");
                                            AddPivotField(field);
                                            newPivotID++;
                                            // quarter
                                            field.Tag = ((newTag + newPivotID.ToString()) 
                                                        + "-row1-year ");
                                            field.Tag = (field.Tag 
                                                        + ((newTag + newPivotID.ToString()) 
                                                        + "-row2-quarter"));
                                            AddPivotField(field);
                                            newPivotID++;
                                            // month
                                            field.Tag = ((newTag + newPivotID.ToString()) 
                                                        + "-row1-year ");
                                            field.Tag = (field.Tag 
                                                        + ((newTag + newPivotID.ToString()) 
                                                        + "-row2-month"));
                                            AddPivotField(field);
                                            newPivotID++;
                                            // week of year
                                            field.Tag = ((newTag + newPivotID.ToString()) 
                                                        + "-row1-year ");
                                            field.Tag = (field.Tag 
                                                        + ((newTag + newPivotID.ToString()) 
                                                        + "-row2-month "));
                                            field.Tag = (field.Tag 
                                                        + ((newTag + newPivotID.ToString()) 
                                                        + "-row3-weekofyear"));
                                            AddPivotField(field);
                                            newPivotID++;
                                            // day
                                            field.Tag = ((newTag + newPivotID.ToString()) 
                                                        + "-row1-year ");
                                            field.Tag = (field.Tag 
                                                        + ((newTag + newPivotID.ToString()) 
                                                        + "-row2-month "));
                                            field.Tag = (field.Tag 
                                                        + ((newTag + newPivotID.ToString()) 
                                                        + "-row3-day"));
                                            AddPivotField(field);
                                            newPivotID++;
                                            fi.Bucket = "year";
                                        }
                                        else
                                        	if (property == "pivotgroup")
                                            {
                                                // duplicate properties from original pivot
                                                int pivotGroupID = Convert.ToInt32(kvp.Value);
                                                pivot.Group = pivotGroupID;
                                                PivotTable original = _pivots[pivotGroupID];
                                                pivot.ColumnFields = original.ColumnFields;
                                                pivot.ValueFields = original.ValueFields;
                                                pivot.Properties = original.Properties;
                                            }
                                            else
                                            	if (property == "all")
                                                {
                                                    fi.ExpandBuckets = true;
                                                    if (fieldType == "row")
                                                    	pivot.ExpandBucketsInRowCount++;
                                                    else
                                                    	if (fieldType.StartsWith("col"))
                                                        	pivot.ExpandBucketsInRowCount++;
                                                }
                                                else
                                                	if (property == "hideblank")
                                                    	fi.HideBlank = true;
                                                    else
                                                    	if (property == "top")
                                                        	fi.ShowTop = Convert.ToInt32(kvp.Value);
                                                        else
                                                        	if (property == "first")
                                                            	fi.ShowFirst = Convert.ToInt32(kvp.Value);
                                                            else
                                                            	if (property == "calendar")
                                                                	fi.Mode = "calendar";
                                                                else
                                                                	if (property.StartsWith("sort"))
                                                                    {
                                                                        if (property.Contains("asc"))
                                                                        	fi.SortDirection = SortDirection.Ascending;
                                                                        else
                                                                        	fi.SortDirection = SortDirection.Descending;
                                                                        if (property.Contains("byval"))
                                                                        	fi.SortByValue = true;
                                                                    }
                                                                    else
                                                                    	if (property == "other")
                                                                        	fi.CollapseOther = true;
                                                                        else
                                                                        	if (property == "format")
                                                                            	pivot.Formats.Add(String.Format("{0}{1}", fi.Field.Name, pivot.ValueFields.Values.ToList().IndexOf(fi)), ((string)(kvp.Value)));
                                                                            else
                                                                            	if (property == "raw")
                                                                                	fi.Raw = true;
                                                                                else
                                                                                	remove = false;
                                if (remove)
                                {
                                    // remove the property
                                    i--;
                                    additionalProperties.Remove(kvp.Key);
                                }
                            }
                        }
                        // add special properties
                        foreach (KeyValuePair<string, object> kvp in additionalProperties)
                        	pivot.AddProperty(kvp.Key, kvp.Value);
                    }
                }
            }
        }
        
        public virtual void AddPivotValues(object[] values)
        {
            if ((_pivots == null) || (_pivots.Count == 0))
            	return;
            foreach (PivotTable pivot in _pivots.Values)
            	pivot.Insert(values);
        }
        
        public List<DataField> EnumerateKeyFields()
        {
            List<DataField> list = new List<DataField>();
            foreach (DataField field in Fields)
            	if (field.IsPrimaryKey)
                	list.Add(field);
            return list;
        }
        
        public List<DataField> EnumerateSyncFields()
        {
            if (Distinct)
            	return new List<DataField>();
            return EnumerateKeyFields();
        }
        
        public string[] RequestedFieldFilter()
        {
            return _requestedFieldFilter;
        }
    }
    
    public class ExecuteViewPageArgs
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ActionArgs _args;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _pageSize;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _metadata;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _aggregates;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _rowCount;
        
        public ExecuteViewPageArgs()
        {
            PageSize = 1;
        }
        
        public virtual ActionArgs Args
        {
            get
            {
                return this._args;
            }
            set
            {
                this._args = value;
            }
        }
        
        public virtual int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                this._pageSize = value;
            }
        }
        
        public virtual bool Metadata
        {
            get
            {
                return this._metadata;
            }
            set
            {
                this._metadata = value;
            }
        }
        
        public virtual bool Aggregates
        {
            get
            {
                return this._aggregates;
            }
            set
            {
                this._aggregates = value;
            }
        }
        
        public virtual bool RowCount
        {
            get
            {
                return this._rowCount;
            }
            set
            {
                this._rowCount = value;
            }
        }
    }
    
    public class PivotTable
    {
        
        public int Id;
        
        public string Name;
        
        internal int Group = 0;
        
        private string _title;
        
        private string _valuesName;
        
        public string ChartType;
        
        internal int ExpandBucketsInRowCount = 0;
        
        internal int ExpandBucketsInColumnCount = 0;
        
        internal bool SubtotalsEnabled = false;
        
        internal bool GrandTotalsEnabled = false;
        
        public int RecordCount = 0;
        
        private ViewPage _page;
        
        private bool _structureValidated = false;
        
        internal SortedDictionary<int, FieldInfo> RowFields = new SortedDictionary<int, FieldInfo>();
        
        internal SortedDictionary<int, FieldInfo> ColumnFields = new SortedDictionary<int, FieldInfo>();
        
        internal SortedDictionary<int, FieldInfo> ValueFields = new SortedDictionary<int, FieldInfo>();
        
        internal SortedDictionary<string, DimensionInfo> Rows = new SortedDictionary<string, DimensionInfo>();
        
        internal SortedDictionary<string, DimensionInfo> Columns = new SortedDictionary<string, DimensionInfo>();
        
        private SortedDictionary<string, ValueInfo> _values = new SortedDictionary<string, ValueInfo>();
        
        private SortedDictionary<string, object> _properties = new SortedDictionary<string, object>();
        
        private SortedDictionary<string, object> _formats = new SortedDictionary<string, object>();
        
        private bool _dataValidated = false;
        
        public PivotTable(int id, ViewPage page)
        {
            this.Id = id;
            this.Name = ("pivot" + id.ToString());
            this._page = page;
        }
        
        public virtual string Title
        {
            get
            {
                if (String.IsNullOrEmpty(_title))
                {
                    if (!(_structureValidated))
                    	return String.Empty;
                    ValidateData();
                    StringBuilder sb = new StringBuilder();
                    bool first = true;
                    string lastFieldLabel = String.Empty;
                    foreach (FieldInfo info in ColumnFields.Values)
                    	if (lastFieldLabel != info.Field.Label)
                        {
                            lastFieldLabel = info.Field.Label;
                            if (first)
                            	first = false;
                            else
                            	sb.Append(", ");
                            if (info.ShowTop > 0)
                            	sb.Append(String.Format("$Top {0} ", info.ShowTop));
                            sb.Append(String.Format("\"{0}\"", lastFieldLabel));
                        }
                    if (!(first))
                    	sb.Append(": ");
                    sb.Append(ValuesName);
                    sb.Append(" $By ");
                    first = true;
                    foreach (FieldInfo info in RowFields.Values)
                    	if (lastFieldLabel != info.Field.Label)
                        {
                            lastFieldLabel = info.Field.Label;
                            if (first)
                            	first = false;
                            else
                            	sb.Append(", ");
                            if (info.ShowTop > 0)
                            	sb.Append(String.Format("$Top {0} ", info.ShowTop));
                            sb.Append(String.Format("\"{0}\"", lastFieldLabel));
                        }
                    _title = sb.ToString();
                }
                return _title;
            }
        }
        
        public virtual string ValuesName
        {
            get
            {
                if (String.IsNullOrEmpty(_valuesName))
                {
                    StringBuilder sb = new StringBuilder();
                    bool first = true;
                    foreach (FieldInfo info in ValueFields.Values)
                    {
                        if (first)
                        	first = false;
                        else
                        	sb.Append(", ");
                        sb.Append((("$" + info.Mode.Substring(0, 1).ToUpper()) 
                                        + info.Mode.Substring(1)));
                        sb.Append("Of ");
                        string label = info.Field.Label;
                        if (!((label == "$CurrentViewLabel")))
                        	label = String.Format("\"{0}\"", label);
                        sb.Append(label);
                    }
                    _valuesName = sb.ToString();
                }
                return _valuesName;
            }
        }
        
        public virtual object[] Data
        {
            get
            {
                return Serialize();
            }
        }
        
        public virtual string[] RowFieldNames
        {
            get
            {
                List<string> list = new List<string>();
                foreach (FieldInfo fi in RowFields.Values)
                	list.Add(fi.Field.Name);
                return list.ToArray();
            }
        }
        
        public virtual string[] ColumnFieldNames
        {
            get
            {
                List<string> list = new List<string>();
                foreach (FieldInfo fi in ColumnFields.Values)
                	list.Add(fi.Field.Name);
                return list.ToArray();
            }
        }
        
        public virtual string[] ValueFieldNames
        {
            get
            {
                List<string> list = new List<string>();
                foreach (FieldInfo fi in ValueFields.Values)
                	list.Add(fi.Field.Name);
                return list.ToArray();
            }
        }
        
        public virtual SortedDictionary<string, object> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }
        
        public virtual SortedDictionary<string, object> Formats
        {
            get
            {
                return _formats;
            }
            set
            {
                _formats = value;
            }
        }
        
        private void ValidateStructure()
        {
            if (_structureValidated)
            	return;
            else
            	_structureValidated = true;
            // Assign value field
            if (ValueFields.Count == 0)
            {
                DataField primaryKeyField = null;
                foreach (DataField field in _page.Fields)
                	if (field.IsPrimaryKey)
                    {
                        primaryKeyField = field;
                        break;
                    }
                if (primaryKeyField == null)
                	primaryKeyField = _page.Fields.First();
                if (primaryKeyField != null)
                	GetValueField(0, primaryKeyField);
                primaryKeyField.Label = "$CurrentViewLabel";
            }
            // Validate aliases
            ValidateAliases(RowFields);
            ValidateAliases(ColumnFields);
            ValidateAliases(ValueFields);
            // Validate calendar fields
            foreach (KeyValuePair<int, FieldInfo> kvp in ValueFields)
            {
                FieldInfo fi = kvp.Value;
                if (fi.Mode == "calendar")
                {
                    DataField[] calendarFields = new DataField[5];
                    foreach (DataField df in _page.Fields)
                    	if (df.IsPrimaryKey)
                        	calendarFields[0] = df;
                        else
                        	if (!(String.IsNullOrEmpty(df.Tag)))
                            	foreach (string prop in df.Tag.Split(' '))
                                	if (prop.StartsWith("calendar-"))
                                    	if (prop == "calendar-date")
                                        	calendarFields[1] = df;
                                        else
                                        	if (prop == "calendar-end")
                                            	calendarFields[2] = df;
                                            else
                                            	if (prop == "calendar-color")
                                                	calendarFields[3] = df;
                                                else
                                                	if (prop == "calendar-text")
                                                    	calendarFields[4] = df;
                    // Resolve lookups
                    for (int i = 0; (i < 5); i++)
                    {
                        DataField df = calendarFields[i];
                        if ((df != null) && !(String.IsNullOrEmpty(df.AliasName)))
                        	foreach (DataField alias in _page.Fields)
                            	if (alias.Name == df.AliasName)
                                	calendarFields.SetValue(alias, i);
                    }
                    fi.CalendarFields = calendarFields;
                }
            }
        }
        
        private void ValidateData()
        {
            if (_dataValidated)
            	return;
            else
            	_dataValidated = true;
            // Expand Buckets
            if ((ExpandBucketsInRowCount > 0) && (Rows.Count > 0))
            {
                Stack<FieldInfo> rowStack = new Stack<FieldInfo>(RowFields.Values.Reverse());
                ExpandBuckets(rowStack, Rows, ExpandBucketsInRowCount);
            }
            if ((ExpandBucketsInColumnCount > 0) && (Columns.Count > 0))
            {
                Stack<FieldInfo> columnStack = new Stack<FieldInfo>(ColumnFields.Values.Reverse());
                ExpandBuckets(columnStack, Columns, ExpandBucketsInColumnCount);
            }
            // Ensure rows and columns
            if (Rows.Count == 0)
            	Rows.Add(String.Empty, new DimensionInfo(String.Empty, new object[] {
                                string.Empty}, RowFields.Count, ValueFields));
            if (Columns.Count == 0)
            	Columns.Add(String.Empty, new DimensionInfo(String.Empty, new object[] {
                                string.Empty}, ColumnFields.Count, ValueFields));
            // Verify ShowTop
            foreach (FieldInfo fi in RowFields.Values)
            	if (fi.ShowTop >= Rows.Count)
                	fi.ShowTop = 0;
            foreach (FieldInfo fi in ColumnFields.Values)
            	if (fi.ShowTop >= Columns.Count)
                	fi.ShowTop = 0;
        }
        
        private void ExpandBuckets(Stack<FieldInfo> fieldStack, SortedDictionary<string, DimensionInfo> dimension, int expandBucketsCount)
        {
            ExpandBuckets(string.Empty, new List<object>(), 1, expandBucketsCount, fieldStack, dimension);
        }
        
        private void ExpandBuckets(string key, List<object> keyValues, int depth, int expandBucketsCount, Stack<FieldInfo> fieldStack, SortedDictionary<string, DimensionInfo> dimension)
        {
            if (depth != 1)
            	key = (key + "|");
            if (fieldStack.Count == 0)
            	return;
            FieldInfo fieldInfo = fieldStack.Pop();
            List<object> bucketKeys = new List<object>();
            if (fieldInfo.ExpandBuckets)
            {
                // find all possible buckets in range
                expandBucketsCount--;
                if (!(String.IsNullOrEmpty(fieldInfo.Bucket)))
                {
                    // Create row for each missing bucket
                    object iterator = fieldInfo.Min;
                    object max = fieldInfo.Max;
                    while (!(fieldInfo.EqualToMax(iterator)))
                    {
                        bucketKeys.Add(iterator);
                        FindNextBucket(ref iterator, fieldInfo.Bucket);
                    }
                    bucketKeys.Add(iterator);
                }
                else
                {
                    // Expand lookup fields with distinct values
                    if (fieldInfo.ValueField == null)
                    	return;
                    DataField originalField = fieldInfo.ValueField;
                    string view = "grid1";
                    if (!(String.IsNullOrEmpty(originalField.ItemsDataView)))
                    	view = originalField.ItemsDataView;
                    string field = originalField.ItemsDataTextField;
                    if (String.IsNullOrEmpty(field))
                    {
                        ControllerConfiguration config = DataControllerBase.CreateConfigurationInstance(GetType(), originalField.ItemsDataController);
                        XPathNavigator fieldNav = config.SelectSingleNode("/c:dataController/c:views/c:view[@type!=\'Form\'][1]/c:dataFields/c:dataField[@fiel" +
                                "dName!=/c:dataController/c:fields/c:field[@isPrimaryKey]/@name][1]/@fieldName");
                        if (fieldNav != null)
                        	field = fieldNav.Value;
                    }
                    IDataEngine engine = ControllerFactory.CreateDataEngine();
                    PageRequest lookupRequest = new PageRequest();
                    lookupRequest.Controller = originalField.ItemsDataController;
                    lookupRequest.PageSize = DataControllerBase.MaximumDistinctValues;
                    using (DbDataReader reader = engine.ExecuteReader(lookupRequest))
                    	while (reader.Read())
                        {
                            // Get first string field
                            if (String.IsNullOrEmpty(((string)(field))))
                            {
                                object[] values = new object[reader.FieldCount];
                                int length = reader.GetValues(values);
                                for (int i = 0; (i < length); i++)
                                	if (values[i] is string)
                                    {
                                        field = reader.GetName(i);
                                        break;
                                    }
                            }
                            bucketKeys.Add(Convert.ToString(reader[field]));
                        }
                }
            }
            else
            {
                // Find all columns in this level
                foreach (DimensionInfo dim in dimension.Values)
                	if (dim.Depth == depth)
                    	bucketKeys.Add(dim.Labels.Last());
            }
            foreach (object bucketKey in bucketKeys)
            {
                List<object> newKeyValues = new List<object>(keyValues);
                object unformattedKey = bucketKey;
                string newKey = (key + FormatPivotValue(ref unformattedKey, newKeyValues, fieldInfo));
                DimensionInfo val = null;
                // expand buckets
                if (!(dimension.TryGetValue(newKey, out val)))
                	dimension.Add(newKey, new DimensionInfo(newKey, newKeyValues.ToArray(), depth, ValueFields));
                // expand lower levels
                if (fieldStack.Count != 0 && expandBucketsCount != 0)
                	ExpandBuckets(newKey, newKeyValues, (depth + 1), expandBucketsCount, new Stack<FieldInfo>(fieldStack), dimension);
            }
        }
        
        private void FindNextBucket(ref object iterator, string bucket)
        {
            if (bucket == "timeofday")
            {
            }
            else
            	if (bucket == "second")
                	((TimeSpan)(iterator)).Add(TimeSpan.FromSeconds(1));
                else
                	if (bucket == "minute")
                    	((TimeSpan)(iterator)).Add(TimeSpan.FromMinutes(1));
                    else
                    	if (bucket == "halfhour")
                        	((TimeSpan)(iterator)).Add(TimeSpan.FromMinutes(30));
                        else
                        	if (bucket == "hour")
                            	((TimeSpan)(iterator)).Add(TimeSpan.FromHours(1));
                            else
                            	if (iterator is int)
                                	iterator = (((int)(iterator)) + 1);
                                else
                                	if (iterator is long)
                                    	iterator = (((long)(iterator)) + 1);
        }
        
        private void ValidateAliases(SortedDictionary<int, FieldInfo> fieldList)
        {
            // detect alias fields
            SortedDictionary<int, DataField> fieldsToReplace = new SortedDictionary<int, DataField>();
            foreach (KeyValuePair<int, FieldInfo> kvp in fieldList)
            {
                DataField field = kvp.Value.Field;
                if (!(String.IsNullOrEmpty(field.AliasName)))
                {
                    DataField aliasField = null;
                    foreach (DataField f in _page.Fields)
                    	if (f.Name == field.AliasName)
                        	aliasField = f;
                    if (aliasField != null)
                    	fieldsToReplace[kvp.Key] = aliasField;
                }
            }
            // Replace alias fields
            foreach (KeyValuePair<int, DataField> kvp in fieldsToReplace)
            {
                FieldInfo fi = fieldList[kvp.Key];
                fi.ValueField = fi.Field;
                fi.Field = kvp.Value;
            }
        }
        
        public virtual void Insert(object[] values)
        {
            if (!(_structureValidated))
            	ValidateStructure();
            // calculate row and column
            List<DimensionInfo> keyList = new List<DimensionInfo>();
            string rowKey = GetPivotKey(values, RowFields, Rows, keyList);
            string columnKey = GetPivotKey(values, ColumnFields, Columns, keyList);
            if (String.IsNullOrEmpty(rowKey) && ((RowFields.Count > 1) || ((RowFields.Count == 1) && RowFields.First().Value.HideBlank)))
            	return;
            if (String.IsNullOrEmpty(columnKey) && ((ColumnFields.Count > 1) || ((ColumnFields.Count == 1) && ColumnFields.First().Value.HideBlank)))
            	return;
            int i = 0;
            foreach (FieldInfo fi in ValueFields.Values)
            {
                // calculate key
                string dataKey = String.Format("{0},{1}", rowKey, columnKey);
                if (ValueFields.Count > 1)
                {
                    dataKey = String.Format("{0},{1}", dataKey, fi.Field.Name);
                    if (!(String.IsNullOrEmpty(fi.Mode)))
                    	dataKey = String.Format("{0},{1}", dataKey, fi.Mode);
                }
                // get the value
                string mode = fi.Mode;
                object val = null;
                if (mode == "calendar")
                {
                    object[] valList = new object[5];
                    for (int f = 0; (f < 5); f++)
                    {
                        DataField valueField = fi.CalendarFields[f];
                        if (valueField != null)
                        {
                            int valIndex = _page.Fields.IndexOf(valueField);
                            object thisVal = values[valIndex];
                            if (thisVal is DateTime)
                            	thisVal = DataControllerBase.EnsureJsonCompatibility(thisVal);
                            valList[f] = thisVal;
                        }
                    }
                    val = valList;
                }
                else
                {
                    DataField valueField = fi.Field;
                    int valIndex = _page.Fields.IndexOf(valueField);
                    val = values[valIndex];
                }
                ValueInfo data = null;
                // find the data in Values
                if (!(_values.TryGetValue(dataKey, out data)))
                {
                    data = new ValueInfo(fi);
                    _values.Add(dataKey, data);
                }
                if ((fi.ShowFirst == 0) || (fi.ShowFirst > data.Values.Count))
                {
                    data.Add(val);
                    foreach (DimensionInfo dimension in keyList)
                    	dimension.Values[i].Add(val, mode);
                }
                i++;
            }
            RecordCount++;
        }
        
        public virtual FieldInfo GetRowField(int index, DataField field)
        {
            FieldInfo info = null;
            if (RowFields.ContainsKey(index))
            {
                info = RowFields[index];
                if (info.Field.Name != field.Name && ((info.ValueField == null) || info.ValueField.Name != field.Name))
                	throw new Exception("Duplicate row field declared in pivot.");
            }
            else
            {
                info = new FieldInfo(field);
                RowFields.Add(index, info);
            }
            return info;
        }
        
        public virtual FieldInfo GetColumnField(int index, DataField field)
        {
            FieldInfo info = null;
            if (ColumnFields.ContainsKey(index))
            {
                info = ColumnFields[index];
                if (info.Field.Name != field.Name && ((info.ValueField == null) || info.ValueField.Name != field.Name))
                	throw new Exception("Duplicate column field declared in pivot.");
            }
            else
            {
                info = new FieldInfo(field);
                ColumnFields.Add(index, info);
            }
            return info;
        }
        
        public virtual FieldInfo GetValueField(int index, DataField field)
        {
            FieldInfo info = null;
            if (ValueFields.ContainsKey(index))
            {
                info = ValueFields[index];
                if (info.Field.Name != field.Name && ((info.ValueField == null) || info.ValueField.Name != field.Name))
                	throw new Exception("Duplicate value field declared in pivot.");
            }
            else
            {
                info = new FieldInfo(field);
                ValueFields.Add(index, info);
            }
            return info;
        }
        
        public virtual void AddProperty(string key, object value)
        {
            if (!(_properties.ContainsKey(key)))
            	_properties.Add(key, value);
        }
        
        private string GetPivotKey(object[] values, SortedDictionary<int, FieldInfo> fieldList, SortedDictionary<string, DimensionInfo> dimensionList, List<DimensionInfo> keyList)
        {
            string pivotKey = String.Empty;
            List<object> pivotValuesList = new List<object>();
            int depth = 1;
            foreach (KeyValuePair<int, FieldInfo> fieldPair in fieldList)
            {
                FieldInfo fi = fieldPair.Value;
                DataField field = fi.Field;
                if (depth != 1)
                	pivotKey = (pivotKey + "|");
                // append value
                int index = _page.Fields.IndexOf(field);
                object value = values[index];
                object dimensionKey = FormatPivotValue(ref value, pivotValuesList, fi);
                pivotKey = (pivotKey + dimensionKey);
                if (fi.HideBlank && String.IsNullOrEmpty(Convert.ToString(dimensionKey)))
                	return String.Empty;
                // initialize row or column
                DimensionInfo dimKey = null;
                if (!(dimensionList.TryGetValue(pivotKey, out dimKey)))
                {
                    dimKey = new DimensionInfo(pivotKey, pivotValuesList.ToArray(), depth, ValueFields);
                    dimensionList.Add(pivotKey, dimKey);
                }
                // update field info
                fi.Add(value);
                keyList.Add(dimKey);
                depth++;
            }
            return pivotKey;
        }
        
        private object FormatPivotValue(ref object value, List<object> pivotValuesList, FieldInfo fi)
        {
            if (value == null)
            	return value;
            bool addValueToArray = true;
            CultureInfo ci = CultureInfo.CurrentCulture;
            // form buckets based on bucket mode
            if (!(String.IsNullOrEmpty(fi.Bucket)))
            	if (value is DateTime)
                {
                    DateTime date = ((DateTime)(value));
                    if (fi.Bucket == "timeofday")
                    {
                        value = date.TimeOfDay;
                        addValueToArray = false;
                        pivotValuesList.Add(((TimeSpan)(value)).ToString());
                    }
                    else
                    	if (fi.Bucket == "minute")
                        {
                            value = new TimeSpan(0, date.Minute, 0);
                            addValueToArray = false;
                            pivotValuesList.Add(String.Format("{0:d2}", date.Minute));
                        }
                        else
                        	if (fi.Bucket == "halfhour")
                            {
                                int minute = 0;
                                if (date.Minute >= 30)
                                	minute = 30;
                                value = new System.TimeSpan(date.Hour, minute, 0);
                                addValueToArray = false;
                                pivotValuesList.Add(String.Format("{0:d2}:{1:d2}", date.Hour, minute));
                            }
                            else
                            	if (fi.Bucket == "hour")
                                {
                                    value = new TimeSpan(date.Hour, 0, 0);
                                    addValueToArray = false;
                                    pivotValuesList.Add(String.Format("{0:d2}:00", date.Hour));
                                }
                                else
                                	if ((fi.Bucket == "day") || (fi.Bucket == "dayofmonth"))
                                    	value = date.Day;
                                    else
                                    	if (fi.Bucket == "dayofweek")
                                        {
                                            value = ((int)(date.DayOfWeek));
                                            addValueToArray = false;
                                            pivotValuesList.Add(date.DayOfWeek);
                                        }
                                        else
                                        	if (fi.Bucket == "dayofyear")
                                            	value = date.DayOfYear;
                                            else
                                            	if (fi.Bucket == "weekofmonth")
                                                {
                                                    value = GetWeekNumberOfMonth(date);
                                                    addValueToArray = false;
                                                    pivotValuesList.Add(String.Format("$Week{0}", value));
                                                }
                                                else
                                                	if ((fi.Bucket == "week") || (fi.Bucket == "weekofyear"))
                                                    {
                                                        value = ci.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                                                        addValueToArray = false;
                                                        pivotValuesList.Add(String.Format("$Week{0}", value));
                                                    }
                                                    else
                                                    	if ((fi.Bucket == "twoweek") || (fi.Bucket == "twoweeks"))
                                                        {
                                                        }
                                                        else
                                                        	if (fi.Bucket == "month")
                                                            {
                                                                value = date.Month;
                                                                addValueToArray = false;
                                                                if (RowFields.Count > 2)
                                                                	pivotValuesList.Add(ci.DateTimeFormat.AbbreviatedMonthNames[(date.Month - 1)]);
                                                                else
                                                                	pivotValuesList.Add(ci.DateTimeFormat.MonthNames[(date.Month - 1)]);
                                                            }
                                                            else
                                                            	if (fi.Bucket == "quarter")
                                                                {
                                                                    int month = date.Month;
                                                                    addValueToArray = false;
                                                                    if (month <= 3)
                                                                    	value = 1;
                                                                    else
                                                                    	if (month <= 6)
                                                                        	value = 2;
                                                                        else
                                                                        	if (month <= 9)
                                                                            	value = 3;
                                                                            else
                                                                            	value = 4;
                                                                    pivotValuesList.Add(("$Quarter" + value.ToString()));
                                                                }
                                                                else
                                                                	if (fi.Bucket == "year")
                                                                    	value = date.Year;
                }
                else
                	if (value is long)
                    {
                        long val = ((long)(value));
                        if (fi.Bucket == "dayofweek")
                        {
                            addValueToArray = false;
                            pivotValuesList.Add(ci.DateTimeFormat.DayNames[val]);
                        }
                        else
                        	if (fi.Bucket == "month")
                            {
                                addValueToArray = false;
                                pivotValuesList.Add(ci.DateTimeFormat.MonthNames[(val - 1)]);
                            }
                            else
                            	if (fi.Bucket == "quarter")
                                {
                                    addValueToArray = false;
                                    pivotValuesList.Add(("$Quarter" + value.ToString()));
                                }
                    }
                    else
                    	if (value is TimeSpan)
                        {
                            TimeSpan time = ((TimeSpan)(value));
                            if (fi.Bucket == "second")
                            	value = time.Seconds;
                            else
                            	if (fi.Bucket == "minute")
                                	value = time.Minutes;
                                else
                                	if (fi.Bucket == "hour")
                                    	value = time.Hours;
                                    else
                                    	if (fi.Bucket == "day")
                                        	value = time.Days;
                        }
            // allow unformatted value printing
            if (fi.Raw && !(addValueToArray))
            {
                pivotValuesList.RemoveAt((pivotValuesList.Count - 1));
                pivotValuesList.Add(value);
            }
            if (addValueToArray)
            	pivotValuesList.Add(value);
            object formattedValue = value;
            // Format field value in sortable form
            if (value is DateTime)
            	formattedValue = ((DateTime)(value)).ToString("s");
            else
            	if (value is TimeSpan)
                	formattedValue = ((TimeSpan)(value)).ToString("hh\\:mm\\:ss");
                else
                	if (value is int)
                    	formattedValue = ((int)(value)).ToString("D10");
                    else
                    	if (value is short)
                        	formattedValue = ((short)(value)).ToString("D5");
                        else
                        	if (value is long)
                            	formattedValue = ((long)(value)).ToString("D10");
                            else
                            	if (value is decimal)
                                	formattedValue = ((decimal)(value)).ToString("########.####");
                                else
                                	if (value != null)
                                    	formattedValue = value.ToString();
            return formattedValue;
        }
        
        static int GetWeekNumberOfMonth(System.DateTime date)
        {
            System.DateTime firstDayOfMonth = new System.DateTime(date.Year, date.Month, 1);
            int firstDay = ((int)(firstDayOfMonth.DayOfWeek));
            if (firstDay == 0)
            	firstDay = 7;
            double d = ((firstDay 
                        + (date.Day - 1)) 
                        / Convert.ToDouble(7));
            return ((int)(Math.Ceiling(d)));
        }
        
        object[] Serialize()
        {
            ValidateData();
            int columnDepth = ColumnFields.Count;
            int rowDepth = RowFields.Count;
            // sort the rows and columns
            List<DimensionInfo> sortedColumns = new List<DimensionInfo>(Columns.Values);
            DimensionInfoComparer columnComparer = new DimensionInfoComparer(ColumnFields);
            sortedColumns.Sort(columnComparer);
            List<DimensionInfo> sortedRows = new List<DimensionInfo>(Rows.Values);
            DimensionInfoComparer rowComparer = new DimensionInfoComparer(RowFields);
            rowComparer.ValueFields.AddRange(ValueFields.Values);
            sortedRows.Sort(rowComparer);
            // rows of the pivot
            List<object> rowList = new List<object>();
            // add header row
            List<object> columnHeaderList = new List<object>();
            // add row label label
            if (rowDepth != 0)
            	columnHeaderList.Add(GetLabel(RowFields.Values.ToArray()));
            else
            	if (columnDepth != 0)
                	columnHeaderList.Add(GetLabel(ColumnFields.Values.ToArray()));
                else
                	columnHeaderList.Add(ValuesName);
            // add column header labels
            foreach (DimensionInfo dim in sortedColumns)
            {
                // skip group headers
                if ((dim.Depth == columnDepth) || SubtotalsEnabled)
                {
                    string columnLabel = GetLabel(dim.Labels);
                    if (ValueFields.Count == 1)
                    	if (!(String.IsNullOrEmpty(columnLabel)))
                        	columnHeaderList.Add(columnLabel);
                        else
                        	if (Columns.Count == 1)
                            	columnHeaderList.Add(ValuesName);
                            else
                            	columnHeaderList.Add("$Blank");
                    else
                    	foreach (FieldInfo fi in ValueFields.Values)
                        	if (!(String.IsNullOrEmpty(columnLabel)))
                            	columnHeaderList.Add(String.Format("{0}, {1}", columnLabel, fi.Field.Name));
                            else
                            	columnHeaderList.Add(fi.Field.Name);
                }
            }
            if (GrandTotalsEnabled)
            	columnHeaderList.Add("$GrandTotals");
            rowList.Add(columnHeaderList.ToArray());
            // add rows
            bool useRawLabel = false;
            foreach (FieldInfo rowLabelInfo in RowFields.Values)
            	if (rowLabelInfo.Raw)
                {
                    useRawLabel = true;
                    break;
                }
            foreach (DimensionInfo rowInfo in sortedRows)
            	if ((rowInfo.Depth == rowDepth) || SubtotalsEnabled)
                {
                    string row = rowInfo.Key;
                    List<object> columnList = new List<object>();
                    // row label
                    string rowLabel = String.Empty;
                    if (useRawLabel)
                    {
                        bool first = true;
                        StringBuilder sb = new StringBuilder();
                        foreach (object obj in rowInfo.Labels)
                        {
                            if (first)
                            	first = false;
                            else
                            	sb.Append(", ");
                            sb.Append(obj.ToString());
                        }
                        rowLabel = sb.ToString();
                    }
                    else
                    	rowLabel = GetLabel(rowInfo.Labels);
                    if (!(String.IsNullOrEmpty(rowLabel)))
                    	columnList.Add(rowLabel);
                    else
                    	if (Rows.Count == 1)
                        	columnList.Add(ValuesName);
                        else
                        	columnList.Add("$Blank");
                    // columns
                    foreach (DimensionInfo columnInfo in sortedColumns)
                    	if ((columnInfo.Depth == columnDepth) || SubtotalsEnabled)
                        {
                            string column = columnInfo.Key;
                            // add values
                            foreach (FieldInfo fi in ValueFields.Values)
                            {
                                // form value key
                                string valueKey = String.Format("{0},{1}", row, column);
                                if (ValueFields.Count > 1)
                                {
                                    valueKey = String.Format("{0},{1}", valueKey, fi.Field.Name);
                                    if (!(String.IsNullOrEmpty(fi.Mode)))
                                    	valueKey = String.Format("{0},{1}", valueKey, fi.Mode);
                                }
                                ValueInfo value = null;
                                if (_values.TryGetValue(valueKey, out value))
                                	columnList.Add(value.Serialize(fi.Mode));
                                else
                                	columnList.Add(null);
                            }
                        }
                    // grand total of row
                    if (GrandTotalsEnabled)
                    	for (int i = 0; (i < ValueFields.Count); i++)
                        	columnList.Add(rowInfo.Values[i].Serialize());
                    rowList.Add(columnList.ToArray());
                }
            if (GrandTotalsEnabled)
            {
                // grand totals for columns
                List<object> grandTotalRowList = new List<object>();
                grandTotalRowList.Add("$GrandTotals");
                foreach (DimensionInfo columnInfo in sortedColumns)
                	for (int i = 0; (i < ValueFields.Count); i++)
                    	grandTotalRowList.Add(columnInfo.Values[i].Serialize());
                grandTotalRowList.Add(RecordCount);
                rowList.Add(grandTotalRowList.ToArray());
            }
            int rowCount = rowList.Count;
            int columnCount = ((object[])(rowList[0])).Length;
            if (GrandTotalsEnabled)
            {
                rowCount--;
                columnCount--;
            }
            int newColumnCount = columnCount;
            // eliminate extra columns
            foreach (FieldInfo info in ColumnFields.Values)
            	if (info.ShowTop != 0)
                {
                    int otherColumnIndex = (info.ShowTop + 1);
                    newColumnCount = otherColumnIndex;
                    if (GrandTotalsEnabled)
                    	newColumnCount++;
                    if (columnCount > (otherColumnIndex + 1))
                    {
                        if (info.CollapseOther)
                        	newColumnCount++;
                        for (int i = 0; (i < rowList.Count); i++)
                        {
                            object[] row = ((object[])(rowList[i]));
                            object[] newRow = new object[newColumnCount];
                            for (int j = 0; (j < otherColumnIndex); j++)
                            	newRow[j] = row[j];
                            if (GrandTotalsEnabled)
                            	newRow[(newColumnCount - 1)] = row[(row.Length - 1)];
                            if (info.CollapseOther)
                            	if (i == 0)
                                	newRow[otherColumnIndex] = "$Other";
                                else
                                {
                                    ValueInfo vi = new ValueInfo();
                                    if (info.Mode == "count")
                                    	vi.Mode = "sum";
                                    else
                                    	vi.Mode = info.Mode;
                                    for (int k = otherColumnIndex; (k < columnCount); k++)
                                    	vi.Add(row[k]);
                                    newRow[otherColumnIndex] = vi.Serialize();
                                }
                            rowList[i] = newRow;
                        }
                    }
                    break;
                }
            // Eliminate extra rows
            foreach (FieldInfo info in RowFields.Values)
            	if (info.ShowTop != 0)
                {
                    int otherRowIndex = (info.ShowTop + 1);
                    if (rowCount > (otherRowIndex + 1))
                    {
                        int numToRemove = (rowList.Count - otherRowIndex);
                        int removeIndex = otherRowIndex;
                        if (GrandTotalsEnabled)
                        {
                            numToRemove--;
                            object grandTotalRow = rowList.ElementAt((rowList.Count - 1));
                            rowList.RemoveAt((rowList.Count - 1));
                            rowList.Add(grandTotalRow);
                        }
                        if (info.CollapseOther)
                        {
                            // step through columns
                            List<ValueInfo> otherRowList = new List<ValueInfo>();
                            for (int i = 1; (i < newColumnCount); i++)
                            {
                                ValueInfo vi = new ValueInfo();
                                string mode = ValueFields.Values.ElementAt(0).Mode;
                                if (mode == "count")
                                	vi.Mode = "sum";
                                else
                                	vi.Mode = mode;
                                // step through rows
                                for (int j = otherRowIndex; (j < rowCount); j++)
                                	vi.Add(((object[])(rowList[j]))[i]);
                                otherRowList.Add(vi);
                            }
                            object[] otherRow = new object[newColumnCount];
                            otherRow[0] = "$Other";
                            for (int k = 1; (k <= otherRowList.Count); k++)
                            	otherRow[k] = otherRowList[(k - 1)].Serialize();
                            rowList.Add(otherRow);
                        }
                        rowList.RemoveRange(removeIndex, numToRemove);
                    }
                    break;
                }
            return rowList.ToArray();
        }
        
        static string GetLabel(object[] list)
        {
            if (list == null)
            	return String.Empty;
            StringBuilder columnBuilder = new StringBuilder();
            string lastValue = String.Empty;
            bool firstRowValue = true;
            foreach (object val in list)
            	if (!((val is string)) || !(String.IsNullOrEmpty(((string)(val)))))
                {
                    if (!((val.ToString() == lastValue)))
                    {
                        lastValue = val.ToString();
                        if (firstRowValue)
                        	firstRowValue = false;
                        else
                        	columnBuilder.Append(", ");
                        columnBuilder.Append(val);
                    }
                }
            return columnBuilder.ToString();
        }
    }
    
    public class ValueInfo
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _count;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object _sum;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object _min;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object _max;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object _average;
        
        public FieldInfo Field;
        
        private Type _fieldType;
        
        private string _mode;
        
        private List<object> _values = new List<object>();
        
        public static System.Type[] SignedNumberTypes = new System.Type[] {
                typeof(sbyte),
                typeof(short),
                typeof(int),
                typeof(long)};
        
        public static Type[] UnsignedNumberTypes = new System.Type[] {
                typeof(byte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong)};
        
        public static Type[] FloatNumberTypes = new Type[] {
                typeof(float),
                typeof(double),
                typeof(decimal)};
        
        public ValueInfo()
        {
        }
        
        public ValueInfo(FieldInfo fi)
        {
            this.Field = fi;
        }
        
        private int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                this._count = value;
            }
        }
        
        public virtual object Sum
        {
            get
            {
                return this._sum;
            }
            set
            {
                this._sum = value;
            }
        }
        
        public virtual object Min
        {
            get
            {
                return this._min;
            }
            set
            {
                this._min = value;
            }
        }
        
        public virtual object Max
        {
            get
            {
                return this._max;
            }
            set
            {
                this._max = value;
            }
        }
        
        public virtual object Average
        {
            get
            {
                return this._average;
            }
            set
            {
                this._average = value;
            }
        }
        
        public virtual Type FieldType
        {
            get
            {
                if ((_fieldType == null) && (Field != null))
                	_fieldType = Field.FieldType;
                return _fieldType;
            }
            set
            {
                _fieldType = value;
            }
        }
        
        public virtual string Mode
        {
            get
            {
                if (!(String.IsNullOrEmpty(_mode)))
                	return _mode;
                if (Field != null)
                	return Field.Mode;
                return "count";
            }
            set
            {
                _mode = value;
            }
        }
        
        public virtual List<object> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
            }
        }
        
        public virtual void Add(object value)
        {
            Count++;
            if (value == null)
            	return;
            if (FieldType == null)
            	FieldType = value.GetType();
            Values.Add(value);
            // additional processing based on type
            if (FieldType == typeof(bool))
            {
                if (Sum == null)
                	Sum = ((long)(0));
                if ((bool)(value))
                	Sum = (((long)(Sum)) + 1);
            }
            else
            	if (SignedNumberTypes.Contains(FieldType))
                {
                    long val = Convert.ToInt64(value);
                    if (Sum == null)
                    	Sum = val;
                    else
                    	Sum = (((long)(Sum)) + val);
                    if ((Min == null) || (val < ((long)(Min))))
                    	Min = val;
                    if ((Max == null) || (val > ((long)(Max))))
                    	Max = val;
                    Average = (((long)(Sum)) / Convert.ToInt64(Count));
                }
                else
                	if (UnsignedNumberTypes.Contains(FieldType))
                    {
                        ulong val = Convert.ToUInt64(value);
                        if (Sum == null)
                        	Sum = val;
                        else
                        	Sum = (((ulong)(Sum)) + val);
                        if ((Min == null) || (val < ((ulong)(Min))))
                        	Min = val;
                        if ((Max == null) || (val > ((ulong)(Max))))
                        	Max = val;
                        Average = (((ulong)(Sum)) / Convert.ToUInt64(Count));
                    }
                    else
                    	if ((FieldType == typeof(float)) || (FieldType == typeof(double)))
                        {
                            double val = Convert.ToDouble(value);
                            if (Sum == null)
                            	Sum = val;
                            else
                            	Sum = (((double)(Sum)) + val);
                            if ((Min == null) || (val < ((double)(Min))))
                            	Min = val;
                            if ((Max == null) || (val > ((double)(Max))))
                            	Max = val;
                            Average = (((double)(Sum)) / Convert.ToDouble(Count));
                        }
                        else
                        	if (FieldType == typeof(decimal))
                            {
                                decimal val = Convert.ToDecimal(value);
                                if (Sum == null)
                                	Sum = val;
                                else
                                	Sum = (((decimal)(Sum)) + val);
                                if ((Min == null) || (val < ((decimal)(Min))))
                                	Min = val;
                                if ((Max == null) || (val > ((decimal)(Max))))
                                	Max = val;
                                Average = (((decimal)(Sum)) / Convert.ToDecimal(Count));
                            }
                            else
                            	if (FieldType == typeof(DateTime))
                                {
                                    DateTime val = ((DateTime)(value));
                                    if ((Min == null) || (val < ((DateTime)(Min))))
                                    	Min = val;
                                    if ((Max == null) || (val > ((DateTime)(Max))))
                                    	Max = val;
                                }
        }
        
        public virtual void Add(object value, string mode)
        {
            if (Mode != mode)
            	Mode = mode;
            Add(value);
        }
        
        public virtual object Serialize()
        {
            return Serialize(Mode);
        }
        
        public virtual object Serialize(string mode)
        {
            Dictionary<string, object> valueList = new Dictionary<string, object>();
            valueList.Add("count", Count);
            if (SignedNumberTypes.Contains(FieldType) || (UnsignedNumberTypes.Contains(FieldType) || FloatNumberTypes.Contains(FieldType)))
            {
                valueList.Add("sum", Sum);
                valueList.Add("min", Min);
                valueList.Add("max", Max);
                valueList.Add("avg", Average);
            }
            else
            	if (FieldType == typeof(DateTime))
                {
                    valueList.Add("min", Min);
                    valueList.Add("max", Max);
                }
            object value = null;
            if (mode == "calendar")
            {
                List<object> calValues = new List<object>();
                foreach (object o in Values)
                	if (o.GetType() == typeof(object[]))
                    	calValues.Add(o);
                value = calValues.ToArray();
            }
            else
            	if (!(valueList.TryGetValue(mode, out value)) || (value == null))
                	value = valueList["count"];
            return value;
        }
    }
    
    public enum SortDirection
    {
        
        None,
        
        Ascending,
        
        Descending,
    }
    
    public class FieldInfo
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DataField _field;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DataField _valueField;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Type _fieldType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _mode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bucket;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _expandBuckets;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ValueInfo _values;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _showTop;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _showFirst;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DataField[] _calendarFields;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _collapseOther;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SortDirection _sortDirection;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _sortByValue;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _hideBlank;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _raw;
        
        public FieldInfo(DataField field) : 
                this(field, "Count")
        {
        }
        
        public FieldInfo(DataField field, string mode)
        {
            this.Field = field;
            this.Mode = mode.ToLower();
            ExpandBuckets = false;
        }
        
        public virtual DataField Field
        {
            get
            {
                return this._field;
            }
            set
            {
                this._field = value;
            }
        }
        
        public virtual DataField ValueField
        {
            get
            {
                return this._valueField;
            }
            set
            {
                this._valueField = value;
            }
        }
        
        public virtual Type FieldType
        {
            get
            {
                return this._fieldType;
            }
            set
            {
                this._fieldType = value;
            }
        }
        
        public virtual string Mode
        {
            get
            {
                return this._mode;
            }
            set
            {
                this._mode = value;
            }
        }
        
        public virtual string Bucket
        {
            get
            {
                return this._bucket;
            }
            set
            {
                this._bucket = value;
            }
        }
        
        public virtual bool ExpandBuckets
        {
            get
            {
                return this._expandBuckets;
            }
            set
            {
                this._expandBuckets = value;
            }
        }
        
        public virtual ValueInfo Values
        {
            get
            {
                return this._values;
            }
            set
            {
                this._values = value;
            }
        }
        
        public virtual object Min
        {
            get
            {
                return _values.Min;
            }
        }
        
        public virtual object Max
        {
            get
            {
                return _values.Max;
            }
        }
        
        public virtual int ShowTop
        {
            get
            {
                return this._showTop;
            }
            set
            {
                this._showTop = value;
            }
        }
        
        public virtual int ShowFirst
        {
            get
            {
                return this._showFirst;
            }
            set
            {
                this._showFirst = value;
            }
        }
        
        public virtual DataField[] CalendarFields
        {
            get
            {
                return this._calendarFields;
            }
            set
            {
                this._calendarFields = value;
            }
        }
        
        public virtual bool CollapseOther
        {
            get
            {
                return this._collapseOther;
            }
            set
            {
                this._collapseOther = value;
            }
        }
        
        public virtual SortDirection SortDirection
        {
            get
            {
                return this._sortDirection;
            }
            set
            {
                this._sortDirection = value;
            }
        }
        
        public virtual bool SortByValue
        {
            get
            {
                return this._sortByValue;
            }
            set
            {
                this._sortByValue = value;
            }
        }
        
        public virtual bool HideBlank
        {
            get
            {
                return this._hideBlank;
            }
            set
            {
                this._hideBlank = value;
            }
        }
        
        public virtual bool Raw
        {
            get
            {
                return this._raw;
            }
            set
            {
                this._raw = value;
            }
        }
        
        public virtual void Add(object value)
        {
            if (String.IsNullOrEmpty(Bucket))
            	return;
            if ((FieldType == null) && (value != null))
            	FieldType = value.GetType();
            if (_values == null)
            	_values = new ValueInfo(this);
            _values.Add(value);
        }
        
        public virtual bool EqualToMax(object value)
        {
            if ((value == null) || (Max == null))
            	return true;
            Type type = value.GetType();
            if (type == typeof(long))
            	return ((long)(value)).Equals(((long)(Max)));
            else
            	if (type == typeof(ulong))
                	return ((ulong)(value)).Equals(((ulong)(Max)));
                else
                	if (type == typeof(double))
                    	return ((double)(value)).Equals(((double)(Max)));
                    else
                    	if (type == typeof(decimal))
                        	return ((decimal)(value)).Equals(((decimal)(Max)));
                        else
                        	if (type == typeof(DateTime))
                            	return ((DateTime)(value)).Equals(((DateTime)(Max)));
                            else
                            	if (type == typeof(TimeSpan))
                                	return ((TimeSpan)(value)).Equals(((TimeSpan)(Max)));
            return true;
        }
        
        public override string ToString()
        {
            return Field.Label;
        }
    }
    
    public class DimensionInfo
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _key;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object[] _labels;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private List<ValueInfo> _values;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _depth;
        
        public DimensionInfo(string key, object[] labels, int depth, SortedDictionary<int, FieldInfo> valueFields)
        {
            this.Key = key;
            this.Labels = labels;
            this.Depth = depth;
            this.Values = new List<ValueInfo>();
            foreach (FieldInfo fi in valueFields.Values)
            	Values.Add(new ValueInfo(fi));
        }
        
        public virtual string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }
        
        public virtual object[] Labels
        {
            get
            {
                return this._labels;
            }
            set
            {
                this._labels = value;
            }
        }
        
        public virtual List<ValueInfo> Values
        {
            get
            {
                return this._values;
            }
            set
            {
                this._values = value;
            }
        }
        
        public virtual int Depth
        {
            get
            {
                return this._depth;
            }
            set
            {
                this._depth = value;
            }
        }
        
        public virtual int Count
        {
            get
            {
                return Labels.Length;
            }
        }
        
        public override string ToString()
        {
            return Key;
        }
    }
    
    public class DimensionInfoComparer : IComparer<DimensionInfo>
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private List<FieldInfo> _compareFields;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private List<FieldInfo> _valueFields;
        
        public DimensionInfoComparer(SortedDictionary<int, FieldInfo> fieldList)
        {
            CompareFields = new List<FieldInfo>(fieldList.Values);
            ValueFields = new List<FieldInfo>();
        }
        
        public virtual List<FieldInfo> CompareFields
        {
            get
            {
                return this._compareFields;
            }
            set
            {
                this._compareFields = value;
            }
        }
        
        public virtual List<FieldInfo> ValueFields
        {
            get
            {
                return this._valueFields;
            }
            set
            {
                this._valueFields = value;
            }
        }
        
        int IComparer<DimensionInfo>.Compare(DimensionInfo x, DimensionInfo y)
        {
            try
            {
                int result = 0;
                // sort by dimension fields
                for (int i = 0; (i < CompareFields.Count); i++)
                {
                    SortDirection dir = CompareFields[i].SortDirection;
                    bool mode = CompareFields[i].SortByValue;
                    if (dir != SortDirection.None)
                    {
                        object xValue = null;
                        object yValue = null;
                        if (!(mode))
                        {
                            xValue = x.Key.Split('|')[i];
                            yValue = y.Key.Split('|')[i];
                        }
                        else
                        {
                            xValue = x.Values[i].Serialize();
                            yValue = y.Values[i].Serialize();
                        }
                        if ((xValue == null) || (yValue == null))
                        	return 0;
                        if (xValue != yValue)
                        {
                            result = ((IComparable)(xValue)).CompareTo(((IComparable)(yValue)));
                            if (dir == SortDirection.Descending)
                            	result = (result * -1);
                            if (result != 0)
                            	return result;
                        }
                    }
                }
                // sort by value fields
                for (int j = 0; (j < ValueFields.Count); j++)
                {
                    SortDirection dir = ValueFields[j].SortDirection;
                    bool mode = ValueFields[j].SortByValue;
                    if (dir != SortDirection.None)
                    {
                        object xValue = null;
                        object yValue = null;
                        if (!(mode))
                        {
                            xValue = x.Key.Split('|')[j];
                            yValue = y.Key.Split('|')[j];
                        }
                        else
                        {
                            xValue = x.Values[j].Serialize();
                            yValue = y.Values[j].Serialize();
                        }
                        if ((xValue == null) || (yValue == null))
                        	return 0;
                        if (xValue != yValue)
                        {
                            result = ((IComparable)(xValue)).CompareTo(((IComparable)(yValue)));
                            if (dir == SortDirection.Descending)
                            	result = (result * -1);
                            if (result != 0)
                            	return result;
                        }
                    }
                }
            }
            catch (Exception )
            {
            }
            return x.Key.CompareTo(y.Key);
        }
    }
}
