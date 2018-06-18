using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MyCompany.Data;

namespace MyCompany.Web
{
	public enum DataViewSelectionMode
    {
        
        Single,
        
        Multiple,
    }
    
    public enum ActionButtonLocation
    {
        
        Auto,
        
        None,
        
        Top,
        
        Bottom,
        
        TopAndBottom,
    }
    
    public enum PagerLocation
    {
        
        None,
        
        Top,
        
        Bottom,
        
        TopAndBottom,
    }
    
    public enum AutoHideMode
    {
        
        Nothing,
        
        Self,
        
        Container,
    }
    
    public class FieldFilter
    {
        
        private string _fieldName;
        
        private RowFilterOperation _operation;
        
        private object _value;
        
        public FieldFilter()
        {
        }
        
        public FieldFilter(string fieldName, RowFilterOperation operation) : 
                this(fieldName, operation, null)
        {
        }
        
        public FieldFilter(string fieldName, RowFilterOperation operation, object value)
        {
            this.FieldName = fieldName;
            this.Operation = operation;
            this.Value = value;
        }
        
        public string FieldName
        {
            get
            {
                return _fieldName;
            }
            set
            {
                _fieldName = value;
            }
        }
        
        public RowFilterOperation Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;
            }
        }
        
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if ((((value != null) && (value is String)) && (Operation == RowFilterOperation.Like)) && !(((string)(value)).Contains("%")))
                	_value = String.Format("%{0}%", value);
                else
                	_value = value;
            }
        }
    }
    
    public partial class DataViewExtender : DataViewExtenderBase
    {
    }
    
    [TargetControlType(typeof(Panel))]
    [TargetControlType(typeof(HtmlContainerControl))]
    public class DataViewExtenderBase : AquariumExtenderBase
    {
        
        private AutoHideMode _autoHide;
        
        private string _controller;
        
        private string _view;
        
        private int _pageSize;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _showActionBar;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ActionButtonLocation _showActionButtons;
        
        private bool _showSearchBar;
        
        private bool _showModalForms;
        
        private string _filterSource;
        
        private string _filterFields;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _visibleWhen;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _roles;
        
        private string _startCommandName;
        
        private string _startCommandArgument;
        
        private string _selectedValue;
        
        private DataViewSelectionMode _selectionMode;
        
        private bool _showInSummary;
        
        private bool _showDescription;
        
        private bool _showViewSelector;
        
        private bool _searchOnStart;
        
        private int _summaryFieldCount;
        
        private string _tag;
        
        private bool _showDetailsInListMode;
        
        private PagerLocation _showPager;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _showPageSize;
        
        private bool _enabled = true;
        
        private int _tabIndex;
        
        private bool _lookupMode;
        
        private string _lookupValue;
        
        private string _lookupText;
        
        private string _lookupPostBackExpression;
        
        private bool _allowCreateLookupItems;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _showRowNumber;
        
        private bool _showQuickFind;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _searchByFirstLetter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _autoSelectFirstRow;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _autoHighlightFirstRow;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _refreshInterval;
        
        public DataViewExtenderBase() : 
                base("Web.DataView")
        {
            this._pageSize = 10;
            this._showActionBar = true;
            this._showActionButtons = ActionButtonLocation.TopAndBottom;
            this._showDetailsInListMode = true;
            this._showPager = PagerLocation.Bottom;
            this._showPageSize = true;
            this._showSearchBar = true;
            _showDescription = true;
            _showViewSelector = true;
            _summaryFieldCount = 5;
            _showQuickFind = true;
        }
        
        [System.ComponentModel.Description("Specifies user interface element that will be hidden if data view can be automati" +
            "cally hidden.")]
        [System.ComponentModel.DefaultValue(AutoHideMode.Nothing)]
        public AutoHideMode AutoHide
        {
            get
            {
                return _autoHide;
            }
            set
            {
                _autoHide = value;
            }
        }
        
        [System.ComponentModel.Description("The name of the data controller. Controllers are stored in the \"~/Controllers\" fo" +
            "lder of your project. Do not include the file extension.")]
        public string Controller
        {
            get
            {
                return _controller;
            }
            set
            {
                _controller = value;
            }
        }
        
        [System.ComponentModel.Description("The name of the startup view in the data controller. The first view is displayed " +
            "if the property is left blank.")]
        public string View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
            }
        }
        
        [System.ComponentModel.Description("The number of rows displayed by grid views of the data controller.")]
        [System.ComponentModel.DefaultValue(10)]
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }
        
        [System.ComponentModel.Description("Specifies if the action bar is displayed above the views of the data controller.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowActionBar
        {
            get
            {
                return this._showActionBar;
            }
            set
            {
                this._showActionBar = value;
            }
        }
        
        [System.ComponentModel.Description("Specifies if the action buttons are displayed in the form views of the data contr" +
            "oller.")]
        [System.ComponentModel.DefaultValue(ActionButtonLocation.TopAndBottom)]
        public ActionButtonLocation ShowActionButtons
        {
            get
            {
                return this._showActionButtons;
            }
            set
            {
                this._showActionButtons = value;
            }
        }
        
        [System.ComponentModel.Description("Specifies if the search bar is enabled in the views of the data controller.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowSearchBar
        {
            get
            {
                return _showSearchBar;
            }
            set
            {
                _showSearchBar = value;
            }
        }
        
        [System.ComponentModel.Description("Specifies that form views are displayed as modal popups. The default form renderi" +
            "ng mode is in-place.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowModalForms
        {
            get
            {
                return _showModalForms;
            }
            set
            {
                _showModalForms = value;
            }
        }
        
        [System.ComponentModel.Description(@"Defines the external source of filtering values. This may be the name of URL parameter or DHTML element in the page. Data view extender will automatically recognize if the DHTML element is also extended and will interface with the client-side extender object.")]
        public string FilterSource
        {
            get
            {
                return _filterSource;
            }
            set
            {
                _filterSource = value;
            }
        }
        
        [System.ComponentModel.Description("Specify the field(s) of the data controller that shall be filtered with the value" +
            "s from the source defined by the FilterSource property.")]
        public string FilterFields
        {
            get
            {
                return _filterFields;
            }
            set
            {
                _filterFields = value;
            }
        }
        
        [System.ComponentModel.Description("The JavaScript expression that must evaluate as true if the data view is visible." +
            "")]
        public string VisibleWhen
        {
            get
            {
                return this._visibleWhen;
            }
            set
            {
                this._visibleWhen = value;
            }
        }
        
        [System.ComponentModel.Description("The comma-separated list of roles allowed to see the data view on the page.")]
        public string Roles
        {
            get
            {
                return this._roles;
            }
            set
            {
                this._roles = value;
            }
        }
        
        [System.ComponentModel.Description("Specify a command that must be executed when the data view is instantiated.")]
        public string StartCommandName
        {
            get
            {
                return _startCommandName;
            }
            set
            {
                _startCommandName = value;
            }
        }
        
        [System.ComponentModel.Description("Specify an argument of a command that must be executed when the data view is inst" +
            "antiated.")]
        public string StartCommandArgument
        {
            get
            {
                return _startCommandArgument;
            }
            set
            {
                _startCommandArgument = value;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public string SelectedValue
        {
            get
            {
                if (_selectedValue == null)
                	_selectedValue = Page.Request.Params[String.Format("{0}_{1}_SelectedValue", ClientID, Controller)];
                return _selectedValue;
            }
            set
            {
                _selectedValue = value;
            }
        }
        
        [System.ComponentModel.Description("The selection mode for the data view.")]
        [System.ComponentModel.DefaultValue(DataViewSelectionMode.Single)]
        public DataViewSelectionMode SelectionMode
        {
            get
            {
                return _selectionMode;
            }
            set
            {
                _selectionMode = value;
            }
        }
        
        [System.ComponentModel.Description("The data view is presented in the page summary.")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowInSummary
        {
            get
            {
                return _showInSummary;
            }
            set
            {
                _showInSummary = value;
            }
        }
        
        [System.ComponentModel.Description("The view descripition is displayed at the top the data extender target.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowDescription
        {
            get
            {
                return _showDescription;
            }
            set
            {
                _showDescription = value;
            }
        }
        
        [System.ComponentModel.Description("The view selector is displayed in the action bar.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowViewSelector
        {
            get
            {
                return _showViewSelector;
            }
            set
            {
                _showViewSelector = value;
            }
        }
        
        [System.ComponentModel.Description("Display grid view in search mode and do not retreive the data when view is displa" +
            "yed for the first time.")]
        [System.ComponentModel.DefaultValue(false)]
        public bool SearchOnStart
        {
            get
            {
                return _searchOnStart;
            }
            set
            {
                _searchOnStart = value;
            }
        }
        
        [System.ComponentModel.Description("The maximum number of fields that can be contributed to the summary.")]
        [System.ComponentModel.DefaultValue(5)]
        public int SummaryFieldCount
        {
            get
            {
                return _summaryFieldCount;
            }
            set
            {
                _summaryFieldCount = value;
            }
        }
        
        [System.ComponentModel.Description("The identifying string passed from the client to server. Use it to filter actions" +
            " and to program business rules.")]
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }
        
        [System.ComponentModel.Description("The identifying string passed from the client to server. Use it to filter actions" +
            " and to program business rules. Separate multiple tags with comma or space.")]
        public string Tags
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }
        
        [System.ComponentModel.Description("The child data views are hidden if the active view is displaying a list of record" +
            "s.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowDetailsInListMode
        {
            get
            {
                return _showDetailsInListMode;
            }
            set
            {
                _showDetailsInListMode = value;
            }
        }
        
        [System.ComponentModel.Description("Specifies if the pager is displayed at the top or/and bottom of the views.")]
        [System.ComponentModel.DefaultValue(true)]
        public PagerLocation ShowPager
        {
            get
            {
                return _showPager;
            }
            set
            {
                _showPager = value;
            }
        }
        
        [System.ComponentModel.Description("The page size information is displayed in the pager area of data views.")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowPageSize
        {
            get
            {
                return this._showPageSize;
            }
            set
            {
                this._showPageSize = value;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public int TabIndex
        {
            get
            {
                return _tabIndex;
            }
            set
            {
                _tabIndex = value;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public string LookupValue
        {
            get
            {
                return _lookupValue;
            }
            set
            {
                _lookupValue = value;
                _lookupMode = true;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public string LookupText
        {
            get
            {
                return _lookupText;
            }
            set
            {
                _lookupText = value;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public string LookupPostBackExpression
        {
            get
            {
                return _lookupPostBackExpression;
            }
            set
            {
                _lookupPostBackExpression = value;
            }
        }
        
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Browsable(false)]
        public bool AllowCreateLookupItems
        {
            get
            {
                return _allowCreateLookupItems;
            }
            set
            {
                _allowCreateLookupItems = value;
            }
        }
        
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowRowNumber
        {
            get
            {
                return this._showRowNumber;
            }
            set
            {
                this._showRowNumber = value;
            }
        }
        
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowQuickFind
        {
            get
            {
                return _showQuickFind;
            }
            set
            {
                _showQuickFind = value;
            }
        }
        
        [System.ComponentModel.DefaultValue(false)]
        public bool SearchByFirstLetter
        {
            get
            {
                return this._searchByFirstLetter;
            }
            set
            {
                this._searchByFirstLetter = value;
            }
        }
        
        [System.ComponentModel.DefaultValue(false)]
        public bool AutoSelectFirstRow
        {
            get
            {
                return this._autoSelectFirstRow;
            }
            set
            {
                this._autoSelectFirstRow = value;
            }
        }
        
        [System.ComponentModel.DefaultValue(false)]
        public bool AutoHighlightFirstRow
        {
            get
            {
                return this._autoHighlightFirstRow;
            }
            set
            {
                this._autoHighlightFirstRow = value;
            }
        }
        
        [System.ComponentModel.DefaultValue(0)]
        public int RefreshInterval
        {
            get
            {
                return this._refreshInterval;
            }
            set
            {
                this._refreshInterval = value;
            }
        }
        
        protected override void ConfigureDescriptor(ScriptBehaviorDescriptor descriptor)
        {
            Page.ClientScript.RegisterHiddenField(String.Format("{0}_{1}_SelectedValue", ClientID, Controller), SelectedValue);
            descriptor.AddProperty("appId", this.TargetControlID);
            descriptor.AddProperty("controller", this.Controller);
            descriptor.AddProperty("viewId", this.View);
            descriptor.AddProperty("pageSize", this.PageSize);
            if (!(ShowActionBar))
            	descriptor.AddProperty("showActionBar", false);
            if (ShowActionButtons != ActionButtonLocation.TopAndBottom)
            	descriptor.AddProperty("showActionButtons", ShowActionButtons.ToString());
            if (ShowPager != PagerLocation.Bottom)
            	descriptor.AddProperty("showPager", ShowPager.ToString());
            if (!(ShowPageSize))
            	descriptor.AddProperty("showPageSize", false);
            if (!(ShowDetailsInListMode))
            	descriptor.AddProperty("showDetailsInListMode", false);
            if (ShowSearchBar)
            	descriptor.AddProperty("showSearchBar", true);
            if (ShowModalForms)
            	descriptor.AddProperty("showModalForms", true);
            if (SearchOnStart)
            	descriptor.AddProperty("searchOnStart", true);
            if (_lookupMode)
            {
                descriptor.AddProperty("mode", "Lookup");
                descriptor.AddProperty("lookupValue", LookupValue);
                descriptor.AddProperty("lookupText", LookupText);
                if (!(String.IsNullOrEmpty(LookupPostBackExpression)))
                	descriptor.AddProperty("lookupPostBackExpression", LookupPostBackExpression);
                if (AllowCreateLookupItems)
                	descriptor.AddProperty("newViewId", MyCompany.Data.Controller.LookupActionArgument(Controller, "New"));
            }
            if (!(String.IsNullOrEmpty(FilterSource)))
            {
                Control source = NamingContainer.FindControl(FilterSource);
                if (source != null)
                {
                    descriptor.AddProperty("filterSource", source.ClientID);
                    if (source is DataViewExtender)
                    	descriptor.AddProperty("appFilterSource", ((DataViewExtender)(source)).TargetControlID);
                }
                else
                	descriptor.AddProperty("filterSource", this.FilterSource);
            }
            if (!(String.IsNullOrEmpty(FilterFields)))
            	descriptor.AddProperty("filterFields", this.FilterFields);
            descriptor.AddProperty("cookie", Guid.NewGuid().ToString());
            if (!(String.IsNullOrEmpty(StartCommandName)))
            	descriptor.AddProperty("startCommandName", StartCommandName);
            if (!(String.IsNullOrEmpty(StartCommandArgument)))
            	descriptor.AddProperty("startCommandArgument", StartCommandArgument);
            if (SelectionMode == DataViewSelectionMode.Multiple)
            	descriptor.AddProperty("selectionMode", "Multiple");
            if (!(Enabled))
            	descriptor.AddProperty("enabled", false);
            if (TabIndex > 0)
            	descriptor.AddProperty("tabIndex", TabIndex);
            if (ShowInSummary)
            	descriptor.AddProperty("showInSummary", "true");
            if (!(ShowDescription))
            	descriptor.AddProperty("showDescription", false);
            if (!(ShowViewSelector))
            	descriptor.AddProperty("showViewSelector", false);
            if (!(String.IsNullOrEmpty(Tag)))
            	descriptor.AddProperty("tag", Tag);
            if (SummaryFieldCount != 5)
            	descriptor.AddProperty("summaryFieldCount", SummaryFieldCount);
            if (SearchByFirstLetter)
            	descriptor.AddProperty("showFirstLetters", true);
            if (AutoSelectFirstRow)
            	descriptor.AddProperty("autoSelectFirstRow", true);
            if (AutoHighlightFirstRow)
            	descriptor.AddProperty("autoHighlightFirstRow", true);
            if (RefreshInterval > 0)
            	descriptor.AddProperty("refreshInterval", RefreshInterval);
            if (!(ShowQuickFind))
            	descriptor.AddProperty("showQuickFind", false);
            if (ShowRowNumber)
            	descriptor.AddProperty("showRowNumber", true);
            if (AutoHide != AutoHideMode.Nothing)
            	descriptor.AddProperty("autoHide", Convert.ToInt16(AutoHide));
            if (Properties.ContainsKey("StartupFilter"))
            	descriptor.AddProperty("startupFilter", Properties["StartupFilter"]);
            string visibleWhenExpression = VisibleWhen;
            if (!(String.IsNullOrEmpty(Roles)) && !(DataControllerBase.UserIsInRole(Roles)))
            	if (String.IsNullOrEmpty(visibleWhenExpression))
                	visibleWhenExpression = "false";
                else
                	visibleWhenExpression = String.Format("({0}) && false", visibleWhenExpression);
            if (!(String.IsNullOrEmpty(visibleWhenExpression)))
            	descriptor.AddProperty("visibleWhen", visibleWhenExpression);
        }
        
        protected override void ConfigureScripts(List<ScriptReference> scripts)
        {
        }
        
        public void AssignFilter(List<FieldFilter> filter)
        {
            this.AssignFilter(filter.ToArray());
        }
        
        public void AssignStartupFilter(List<FieldFilter> filter)
        {
            this.AssignStartupFilter(filter.ToArray());
        }
        
        private SortedDictionary<string, string> CreateFilterExpressions(FieldFilter[] filter)
        {
            // prepare a list of filter expressions
            SortedDictionary<string, string> list = new SortedDictionary<string, string>();
            foreach (FieldFilter ff in filter)
            {
                string filterExpression = null;
                if (!(list.TryGetValue(ff.FieldName, out filterExpression)))
                	filterExpression = String.Empty;
                else
                	filterExpression = (filterExpression + "\\0");
                if (ff.Value is Array)
                {
                    object[] values = ((object[])(ff.Value));
                    if (ff.Operation == RowFilterOperation.Between)
                    	ff.Value = String.Format("{0}$and${1}", DataControllerBase.ValueToString(values[0]), DataControllerBase.ValueToString(values[1]));
                    else
                    	if ((ff.Operation == RowFilterOperation.Includes) || (ff.Operation == RowFilterOperation.DoesNotInclude))
                        {
                            StringBuilder svb = new StringBuilder();
                            foreach (object o in values)
                            {
                                if (svb.Length > 0)
                                	svb.Append("$or$");
                                svb.Append(DataControllerBase.ValueToString(o));
                            }
                            ff.Value = svb.ToString();
                        }
                }
                else
                	ff.Value = DataControllerBase.ValueToString(ff.Value);
                if (ff.Operation == RowFilterOperation.None)
                	filterExpression = null;
                else
                	filterExpression = (filterExpression 
                                + (RowFilterAttribute.ComparisonOperations[Convert.ToInt32(ff.Operation)] + Convert.ToString(ff.Value).Replace("\'", "\\\'")));
                list[ff.FieldName] = filterExpression;
            }
            return list;
        }
        
        public void AssignFilter(FieldFilter[] filter)
        {
            SortedDictionary<string, string> list = CreateFilterExpressions(filter);
            // create a filter
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("var dv = Web.DataView.find(\'{0}\');dv.beginFilter();var f;", this.ID);
            foreach (string fieldName in list.Keys)
            	if (String.IsNullOrEmpty(list[fieldName]))
                	sb.AppendFormat("f=dv.findField(\'{0}\');if(f)dv.removeFromFilter(f);", fieldName);
                else
                	sb.AppendFormat("f=dv.findField(\'{0}\');if(f)dv.applyFilter(f,\':\', \'{1}\');", fieldName, list[fieldName]);
            sb.Append("dv.endFilter();");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(DataViewExtender), ("AsyncPostBackScript" + this.ID), sb.ToString(), true);
        }
        
        public void AssignStartupFilter(FieldFilter[] filter)
        {
            SortedDictionary<string, string> list = CreateFilterExpressions(filter);
            List<string> dataViewFilter = new List<string>();
            foreach (string fieldName in list.Keys)
            	dataViewFilter.Add(String.Format("{0}:{1}", fieldName, list[fieldName]));
            Properties["StartupFilter"] = dataViewFilter;
        }
    }
}
