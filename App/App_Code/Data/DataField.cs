using System;
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

namespace MyCompany.Data
{
	public enum DataFieldMaskType
    {
        
        None,
        
        Date,
        
        Number,
        
        Time,
        
        DateTime,
    }
    
    public enum DataFieldAggregate
    {
        
        None,
        
        Sum,
        
        Count,
        
        Average,
        
        Max,
        
        Min,
    }
    
    public enum OnDemandDisplayStyle
    {
        
        Thumbnail,
        
        Link,
        
        Signature,
    }
    
    public enum TextInputMode
    {
        
        Text,
        
        Password,
        
        RichText,
        
        Note,
        
        Static,
    }
    
    public enum FieldSearchMode
    {
        
        Default,
        
        Required,
        
        Suggested,
        
        Allowed,
        
        Forbidden,
    }
    
    public class DataField
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _name;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _aliasName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _tag;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _type;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _len;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _label;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _isPrimaryKey;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _readOnly;
        
        private string _defaultValue;
        
        private string _headerText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _footerText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _toolTip;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _watermark;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _hidden;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _allowQBE;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _allowSorting;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dataFormatString;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _copy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _hyperlinkFormatString;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _formatOnClient;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _sourceFields;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _categoryIndex;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _allowNulls;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _columns;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _rows;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _onDemand;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private FieldSearchMode _search;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _searchOptions;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsDataController;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsDataView;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsDataValueField;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsDataTextField;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsStyle;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _itemsPageSize;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsNewDataView;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsTargetController;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _itemsLetters;
        
        private List<object[]> _items;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DataFieldAggregate _aggregate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _onDemandHandler;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private OnDemandDisplayStyle _onDemandStyle;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private TextInputMode _textMode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DataFieldMaskType _maskType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _mask;
        
        private string _contextFields;
        
        private string _selectExpression;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _formula;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _showInSummary;
        
        private bool _isMirror;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _htmlEncode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _autoCompletePrefixLength;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _calculated;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _causesCalculate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _isVirtual;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _configuration;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _editor;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _autoSelect;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _searchOnStart;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _itemsDescription;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dataViewController;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dataViewId;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dataViewFilterSource;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dataViewFilterFields;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewShowInSummary;
        
        private bool _dataViewShowActionBar = true;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dataViewShowActionButtons;
        
        private bool _dataViewShowDescription = true;
        
        private bool _dataViewShowViewSelector = true;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewShowModalForms;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewSearchByFirstLetter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewSearchOnStart;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _dataViewPageSize;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewMultiSelect;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dataViewShowPager;
        
        private bool _dataViewShowPageSize = true;
        
        private bool _dataViewShowSearchBar = true;
        
        private bool _dataViewShowQuickFind = true;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewShowRowNumber;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewAutoSelectFirstRow;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _dataViewAutoHighlightFirstRow;
        
        public DataField()
        {
            _items = new List<object[]>();
            _formatOnClient = true;
        }
        
        public DataField(XPathNavigator field, IXmlNamespaceResolver nm) : 
                this()
        {
            this._name = field.GetAttribute("name", String.Empty);
            this._type = field.GetAttribute("type", String.Empty);
            string l = field.GetAttribute("length", String.Empty);
            if (!(String.IsNullOrEmpty(l)))
            	_len = Convert.ToInt32(l);
            this._label = field.GetAttribute("label", String.Empty);
            this._isPrimaryKey = (field.GetAttribute("isPrimaryKey", String.Empty) == "true");
            this._readOnly = (field.GetAttribute("readOnly", String.Empty) == "true");
            this._onDemand = (field.GetAttribute("onDemand", String.Empty) == "true");
            this._defaultValue = field.GetAttribute("default", String.Empty);
            this._allowNulls = !((field.GetAttribute("allowNulls", String.Empty) == "false"));
            this._hidden = (field.GetAttribute("hidden", String.Empty) == "true");
            this._allowQBE = !((field.GetAttribute("allowQBE", String.Empty) == "false"));
            this._allowSorting = !((field.GetAttribute("allowSorting", String.Empty) == "false"));
            this._sourceFields = field.GetAttribute("sourceFields", String.Empty);
            string onDemandStyle = field.GetAttribute("onDemandStyle", String.Empty);
            if (onDemandStyle == "Link")
            	this._onDemandStyle = OnDemandDisplayStyle.Link;
            else
            	if (onDemandStyle == "Signature")
                	this._onDemandStyle = OnDemandDisplayStyle.Signature;
            this._onDemandHandler = field.GetAttribute("onDemandHandler", String.Empty);
            this._contextFields = field.GetAttribute("contextFields", String.Empty);
            this._selectExpression = field.GetAttribute("select", String.Empty);
            bool computed = (field.GetAttribute("computed", String.Empty) == "true");
            if (computed)
            {
                _formula = ((string)(field.Evaluate("string(self::c:field/c:formula)", nm)));
                if (String.IsNullOrEmpty(_formula))
                	_formula = "null";
            }
            this._showInSummary = (field.GetAttribute("showInSummary", String.Empty) == "true");
            this._htmlEncode = !((field.GetAttribute("htmlEncode", String.Empty) == "false"));
            this._calculated = (field.GetAttribute("calculated", String.Empty) == "true");
            this._causesCalculate = (field.GetAttribute("causesCalculate", String.Empty) == "true");
            this._isVirtual = (field.GetAttribute("isVirtual", String.Empty) == "true");
            this._configuration = ((string)(field.Evaluate("string(self::c:field/c:configuration)", nm)));
            this._dataFormatString = field.GetAttribute("dataFormatString", String.Empty);
            _formatOnClient = !((field.GetAttribute("formatOnClient", String.Empty) == "false"));
            string editor = field.GetAttribute("editor", String.Empty);
            if (!(String.IsNullOrEmpty(editor)))
            	_editor = editor;
            XPathNavigator itemsNav = field.SelectSingleNode("c:items", nm);
            if (itemsNav != null)
            {
                this.ItemsDataController = itemsNav.GetAttribute("dataController", String.Empty);
                this.ItemsTargetController = itemsNav.GetAttribute("targetController", String.Empty);
            }
            XPathNavigator dataViewNav = field.SelectSingleNode("c:dataView", nm);
            if (dataViewNav != null)
            {
                this.DataViewController = dataViewNav.GetAttribute("controller", String.Empty);
                this.DataViewId = dataViewNav.GetAttribute("view", String.Empty);
                this.DataViewFilterSource = dataViewNav.GetAttribute("filterSource", String.Empty);
                this.DataViewFilterFields = dataViewNav.GetAttribute("filterFields", String.Empty);
                _allowQBE = true;
                _allowSorting = true;
                _len = 0;
                _columns = 0;
                _htmlEncode = true;
            }
        }
        
        public DataField(XPathNavigator field, IXmlNamespaceResolver nm, bool hidden) : 
                this(field, nm)
        {
            this._hidden = hidden;
        }
        
        public DataField(DataField field) : 
                this()
        {
            this._isMirror = true;
            this._name = (field.Name + "_Mirror");
            this._type = field.Type;
            this._len = field.Len;
            this._label = field.Label;
            this._readOnly = true;
            this._allowNulls = field.AllowNulls;
            this._allowQBE = field.AllowQBE;
            this._allowSorting = field.AllowSorting;
            this._dataFormatString = field.DataFormatString;
            this._aggregate = field.Aggregate;
            if (!(this._dataFormatString.Contains("{")))
            	this._dataFormatString = String.Format("{{0:{0}}}", this._dataFormatString);
            field._aliasName = this._name;
            this.FormatOnClient = false;
            field.FormatOnClient = true;
            field.DataFormatString = String.Empty;
            this._hidden = true;
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
        
        public string AliasName
        {
            get
            {
                return this._aliasName;
            }
            set
            {
                this._aliasName = value;
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
        
        public string Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
        
        public int Len
        {
            get
            {
                return this._len;
            }
            set
            {
                this._len = value;
            }
        }
        
        public string Label
        {
            get
            {
                return this._label;
            }
            set
            {
                this._label = value;
            }
        }
        
        public bool IsPrimaryKey
        {
            get
            {
                return this._isPrimaryKey;
            }
            set
            {
                this._isPrimaryKey = value;
            }
        }
        
        public bool ReadOnly
        {
            get
            {
                return this._readOnly;
            }
            set
            {
                this._readOnly = value;
            }
        }
        
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }
        
        public bool HasDefaultValue
        {
            get
            {
                return !(String.IsNullOrEmpty(_defaultValue));
            }
        }
        
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                _headerText = value;
                if (!(String.IsNullOrEmpty(value)) && String.IsNullOrEmpty(_label))
                	_label = value;
            }
        }
        
        public string FooterText
        {
            get
            {
                return this._footerText;
            }
            set
            {
                this._footerText = value;
            }
        }
        
        public string ToolTip
        {
            get
            {
                return this._toolTip;
            }
            set
            {
                this._toolTip = value;
            }
        }
        
        public string Watermark
        {
            get
            {
                return this._watermark;
            }
            set
            {
                this._watermark = value;
            }
        }
        
        public bool Hidden
        {
            get
            {
                return this._hidden;
            }
            set
            {
                this._hidden = value;
            }
        }
        
        public bool AllowQBE
        {
            get
            {
                return this._allowQBE;
            }
            set
            {
                this._allowQBE = value;
            }
        }
        
        public bool AllowSorting
        {
            get
            {
                return this._allowSorting;
            }
            set
            {
                this._allowSorting = value;
            }
        }
        
        public string DataFormatString
        {
            get
            {
                return this._dataFormatString;
            }
            set
            {
                this._dataFormatString = value;
            }
        }
        
        public string Copy
        {
            get
            {
                return this._copy;
            }
            set
            {
                this._copy = value;
            }
        }
        
        public string HyperlinkFormatString
        {
            get
            {
                return this._hyperlinkFormatString;
            }
            set
            {
                this._hyperlinkFormatString = value;
            }
        }
        
        public bool FormatOnClient
        {
            get
            {
                return this._formatOnClient;
            }
            set
            {
                this._formatOnClient = value;
            }
        }
        
        public string SourceFields
        {
            get
            {
                return this._sourceFields;
            }
            set
            {
                this._sourceFields = value;
            }
        }
        
        public int CategoryIndex
        {
            get
            {
                return this._categoryIndex;
            }
            set
            {
                this._categoryIndex = value;
            }
        }
        
        public bool AllowNulls
        {
            get
            {
                return this._allowNulls;
            }
            set
            {
                this._allowNulls = value;
            }
        }
        
        public int Columns
        {
            get
            {
                return this._columns;
            }
            set
            {
                this._columns = value;
            }
        }
        
        public int Rows
        {
            get
            {
                return this._rows;
            }
            set
            {
                this._rows = value;
            }
        }
        
        public bool OnDemand
        {
            get
            {
                return this._onDemand;
            }
            set
            {
                this._onDemand = value;
            }
        }
        
        public FieldSearchMode Search
        {
            get
            {
                return this._search;
            }
            set
            {
                this._search = value;
            }
        }
        
        public virtual string SearchOptions
        {
            get
            {
                return this._searchOptions;
            }
            set
            {
                this._searchOptions = value;
            }
        }
        
        public string ItemsDataController
        {
            get
            {
                return this._itemsDataController;
            }
            set
            {
                this._itemsDataController = value;
            }
        }
        
        public string ItemsDataView
        {
            get
            {
                return this._itemsDataView;
            }
            set
            {
                this._itemsDataView = value;
            }
        }
        
        public string ItemsDataValueField
        {
            get
            {
                return this._itemsDataValueField;
            }
            set
            {
                this._itemsDataValueField = value;
            }
        }
        
        public string ItemsDataTextField
        {
            get
            {
                return this._itemsDataTextField;
            }
            set
            {
                this._itemsDataTextField = value;
            }
        }
        
        public string ItemsStyle
        {
            get
            {
                return this._itemsStyle;
            }
            set
            {
                this._itemsStyle = value;
            }
        }
        
        public int ItemsPageSize
        {
            get
            {
                return this._itemsPageSize;
            }
            set
            {
                this._itemsPageSize = value;
            }
        }
        
        public string ItemsNewDataView
        {
            get
            {
                return this._itemsNewDataView;
            }
            set
            {
                this._itemsNewDataView = value;
            }
        }
        
        public string ItemsTargetController
        {
            get
            {
                return this._itemsTargetController;
            }
            set
            {
                this._itemsTargetController = value;
            }
        }
        
        public bool ItemsLetters
        {
            get
            {
                return this._itemsLetters;
            }
            set
            {
                this._itemsLetters = value;
            }
        }
        
        public List<object[]> Items
        {
            get
            {
                return _items;
            }
        }
        
        public DataFieldAggregate Aggregate
        {
            get
            {
                return this._aggregate;
            }
            set
            {
                this._aggregate = value;
            }
        }
        
        public string OnDemandHandler
        {
            get
            {
                return this._onDemandHandler;
            }
            set
            {
                this._onDemandHandler = value;
            }
        }
        
        public OnDemandDisplayStyle OnDemandStyle
        {
            get
            {
                return this._onDemandStyle;
            }
            set
            {
                this._onDemandStyle = value;
            }
        }
        
        public TextInputMode TextMode
        {
            get
            {
                return this._textMode;
            }
            set
            {
                this._textMode = value;
            }
        }
        
        public DataFieldMaskType MaskType
        {
            get
            {
                return this._maskType;
            }
            set
            {
                this._maskType = value;
            }
        }
        
        public string Mask
        {
            get
            {
                return this._mask;
            }
            set
            {
                this._mask = value;
            }
        }
        
        public string ContextFields
        {
            get
            {
                return _contextFields;
            }
        }
        
        public string Formula
        {
            get
            {
                return this._formula;
            }
            set
            {
                this._formula = value;
            }
        }
        
        public bool ShowInSummary
        {
            get
            {
                return this._showInSummary;
            }
            set
            {
                this._showInSummary = value;
            }
        }
        
        public bool IsMirror
        {
            get
            {
                return _isMirror;
            }
        }
        
        public bool HtmlEncode
        {
            get
            {
                return this._htmlEncode;
            }
            set
            {
                this._htmlEncode = value;
            }
        }
        
        public int AutoCompletePrefixLength
        {
            get
            {
                return this._autoCompletePrefixLength;
            }
            set
            {
                this._autoCompletePrefixLength = value;
            }
        }
        
        public bool Calculated
        {
            get
            {
                return this._calculated;
            }
            set
            {
                this._calculated = value;
            }
        }
        
        public bool CausesCalculate
        {
            get
            {
                return this._causesCalculate;
            }
            set
            {
                this._causesCalculate = value;
            }
        }
        
        public bool IsVirtual
        {
            get
            {
                return this._isVirtual;
            }
            set
            {
                this._isVirtual = value;
            }
        }
        
        public string Configuration
        {
            get
            {
                return this._configuration;
            }
            set
            {
                this._configuration = value;
            }
        }
        
        public string Editor
        {
            get
            {
                return this._editor;
            }
            set
            {
                this._editor = value;
            }
        }
        
        public bool AutoSelect
        {
            get
            {
                return this._autoSelect;
            }
            set
            {
                this._autoSelect = value;
            }
        }
        
        public bool SearchOnStart
        {
            get
            {
                return this._searchOnStart;
            }
            set
            {
                this._searchOnStart = value;
            }
        }
        
        public string ItemsDescription
        {
            get
            {
                return this._itemsDescription;
            }
            set
            {
                this._itemsDescription = value;
            }
        }
        
        public string DataViewController
        {
            get
            {
                return this._dataViewController;
            }
            set
            {
                this._dataViewController = value;
            }
        }
        
        public string DataViewId
        {
            get
            {
                return this._dataViewId;
            }
            set
            {
                this._dataViewId = value;
            }
        }
        
        public string DataViewFilterSource
        {
            get
            {
                return this._dataViewFilterSource;
            }
            set
            {
                this._dataViewFilterSource = value;
            }
        }
        
        public string DataViewFilterFields
        {
            get
            {
                return this._dataViewFilterFields;
            }
            set
            {
                this._dataViewFilterFields = value;
            }
        }
        
        public bool DataViewShowInSummary
        {
            get
            {
                return this._dataViewShowInSummary;
            }
            set
            {
                this._dataViewShowInSummary = value;
            }
        }
        
        public bool DataViewShowActionBar
        {
            get
            {
                return _dataViewShowActionBar;
            }
            set
            {
                _dataViewShowActionBar = value;
            }
        }
        
        public string DataViewShowActionButtons
        {
            get
            {
                return this._dataViewShowActionButtons;
            }
            set
            {
                this._dataViewShowActionButtons = value;
            }
        }
        
        public bool DataViewShowDescription
        {
            get
            {
                return _dataViewShowDescription;
            }
            set
            {
                _dataViewShowDescription = value;
            }
        }
        
        public bool DataViewShowViewSelector
        {
            get
            {
                return _dataViewShowViewSelector;
            }
            set
            {
                _dataViewShowViewSelector = value;
            }
        }
        
        public bool DataViewShowModalForms
        {
            get
            {
                return this._dataViewShowModalForms;
            }
            set
            {
                this._dataViewShowModalForms = value;
            }
        }
        
        public bool DataViewSearchByFirstLetter
        {
            get
            {
                return this._dataViewSearchByFirstLetter;
            }
            set
            {
                this._dataViewSearchByFirstLetter = value;
            }
        }
        
        public bool DataViewSearchOnStart
        {
            get
            {
                return this._dataViewSearchOnStart;
            }
            set
            {
                this._dataViewSearchOnStart = value;
            }
        }
        
        public int DataViewPageSize
        {
            get
            {
                return this._dataViewPageSize;
            }
            set
            {
                this._dataViewPageSize = value;
            }
        }
        
        public bool DataViewMultiSelect
        {
            get
            {
                return this._dataViewMultiSelect;
            }
            set
            {
                this._dataViewMultiSelect = value;
            }
        }
        
        public string DataViewShowPager
        {
            get
            {
                return this._dataViewShowPager;
            }
            set
            {
                this._dataViewShowPager = value;
            }
        }
        
        public bool DataViewShowPageSize
        {
            get
            {
                return _dataViewShowPageSize;
            }
            set
            {
                _dataViewShowPageSize = value;
            }
        }
        
        public bool DataViewShowSearchBar
        {
            get
            {
                return _dataViewShowSearchBar;
            }
            set
            {
                _dataViewShowSearchBar = value;
            }
        }
        
        public bool DataViewShowQuickFind
        {
            get
            {
                return _dataViewShowQuickFind;
            }
            set
            {
                _dataViewShowQuickFind = value;
            }
        }
        
        public bool DataViewShowRowNumber
        {
            get
            {
                return this._dataViewShowRowNumber;
            }
            set
            {
                this._dataViewShowRowNumber = value;
            }
        }
        
        public bool DataViewAutoSelectFirstRow
        {
            get
            {
                return this._dataViewAutoSelectFirstRow;
            }
            set
            {
                this._dataViewAutoSelectFirstRow = value;
            }
        }
        
        public bool DataViewAutoHighlightFirstRow
        {
            get
            {
                return this._dataViewAutoHighlightFirstRow;
            }
            set
            {
                this._dataViewAutoHighlightFirstRow = value;
            }
        }
        
        public string SelectExpression()
        {
            return _selectExpression;
        }
        
        public void NormalizeDataFormatString()
        {
            if (!(String.IsNullOrEmpty(_dataFormatString)))
            {
                string fmt = _dataFormatString;
                if (!(fmt.Contains("{")))
                	_dataFormatString = String.Format("{{0:{0}}}", fmt);
            }
            else
            	if (_type == "DateTime")
                	_dataFormatString = "{0:d}";
        }
        
        public string ExpressionName()
        {
            if (IsMirror)
            	return Name.Substring(0, (Name.Length - "_Mirror".Length));
            return Name;
        }
        
        public bool SupportsStaticItems()
        {
            return (!(String.IsNullOrEmpty(ItemsDataController)) && !(((ItemsStyle == "AutoComplete") || (ItemsStyle == "Lookup"))));
        }
        
        public bool IsMatchedByName(string sample)
        {
            string headerText = this.HeaderText;
            if (String.IsNullOrEmpty(headerText))
            	headerText = this.Label;
            if (String.IsNullOrEmpty(headerText))
            	headerText = this.Name;
            headerText = headerText.Replace(" ", String.Empty);
            return headerText.StartsWith(sample.Replace(" ", String.Empty), StringComparison.CurrentCultureIgnoreCase);
        }
        
        public override string ToString()
        {
            if (!(String.IsNullOrEmpty(Formula)))
            	return String.Format("{0} as {1}; SQL: {2}", Name, Type, Formula);
            else
            	return String.Format("{0} as {1}", Name, Type);
        }
        
        public bool IsTagged(string tag)
        {
            if (String.IsNullOrEmpty(this.Tag))
            	return false;
            return this.Tag.Contains(tag);
        }
    }
}
