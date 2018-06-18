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
	[Serializable]
    public class PageRequest
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _tag;
        
        private string _viewType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _supportsCaching;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresFirstLetters;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _disableJSONCompatibility;
        
        private int _pageIndex;
        
        private int _pageSize;
        
        private int _pageOffset;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _sortExpression;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _groupExpression;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _filter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _systemFilter;
        
        private string _filterDetails;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object[] _syncKey;
        
        private string _contextKey;
        
        private bool _filterIsExternal;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresMetaData;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _fieldFilter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _metadataFilter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresSiteContentText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresPivot;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _pivotDefinitions;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _requiresRowCount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _doesNotRequireData;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _doesNotRequireAggregates;
        
        private string _controller;
        
        private string _view;
        
        private string _lastView;
        
        private string _lookupContextController;
        
        private string _lookupContextView;
        
        private string _lookupContextFieldName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _inserting;
        
        private string _lastCommandName;
        
        private string _lastCommandArgument;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private FieldValue[] _externalFilter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _quickFindHint;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _innerJoinPrimaryKey;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _innerJoinForeignKey;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _distinct;
        
        public PageRequest()
        {
            if ((HttpContext.Current != null) && (Current == null))
            	HttpContext.Current.Items["PageRequest_Current"] = this;
        }
        
        public PageRequest(int pageIndex, int pageSize, string sortExpression, string[] filter)
        {
            this._pageIndex = pageIndex;
            this._pageSize = pageSize;
            this._sortExpression = sortExpression;
            this._filter = filter;
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
        
        public string ViewType
        {
            get
            {
                return _viewType;
            }
            set
            {
                _viewType = ControllerUtilities.ValidateName(value);
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
        
        public bool RequiresFirstLetters
        {
            get
            {
                return this._requiresFirstLetters;
            }
            set
            {
                this._requiresFirstLetters = value;
            }
        }
        
        public bool DisableJSONCompatibility
        {
            get
            {
                return this._disableJSONCompatibility;
            }
            set
            {
                this._disableJSONCompatibility = value;
            }
        }
        
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = value;
            }
        }
        
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
        
        public string FilterDetails
        {
            get
            {
                return _filterDetails;
            }
            set
            {
                _filterDetails = value;
            }
        }
        
        public object[] SyncKey
        {
            get
            {
                return this._syncKey;
            }
            set
            {
                this._syncKey = value;
            }
        }
        
        public string ContextKey
        {
            get
            {
                return _contextKey;
            }
            set
            {
                _contextKey = value;
            }
        }
        
        public bool FilterIsExternal
        {
            get
            {
                return _filterIsExternal;
            }
            set
            {
                _filterIsExternal = value;
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
        
        public virtual string[] FieldFilter
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
        
        public virtual string[] MetadataFilter
        {
            get
            {
                return this._metadataFilter;
            }
            set
            {
                this._metadataFilter = value;
            }
        }
        
        public bool RequiresSiteContentText
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
        
        public bool RequiresPivot
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
        
        public string PivotDefinitions
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
        
        public bool RequiresRowCount
        {
            get
            {
                return this._requiresRowCount;
            }
            set
            {
                this._requiresRowCount = value;
            }
        }
        
        public bool DoesNotRequireData
        {
            get
            {
                return this._doesNotRequireData;
            }
            set
            {
                this._doesNotRequireData = value;
            }
        }
        
        public bool DoesNotRequireAggregates
        {
            get
            {
                return this._doesNotRequireAggregates;
            }
            set
            {
                this._doesNotRequireAggregates = value;
            }
        }
        
        public string Controller
        {
            get
            {
                return _controller;
            }
            set
            {
                _controller = ControllerUtilities.ValidateName(value);
            }
        }
        
        public string View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = ControllerUtilities.ValidateName(value);
            }
        }
        
        public string LastView
        {
            get
            {
                return _lastView;
            }
            set
            {
                _lastView = ControllerUtilities.ValidateName(value);
            }
        }
        
        public string LookupContextController
        {
            get
            {
                return _lookupContextController;
            }
            set
            {
                _lookupContextController = ControllerUtilities.ValidateName(value);
            }
        }
        
        public string LookupContextView
        {
            get
            {
                return _lookupContextView;
            }
            set
            {
                _lookupContextView = ControllerUtilities.ValidateName(value);
            }
        }
        
        public string LookupContextFieldName
        {
            get
            {
                return _lookupContextFieldName;
            }
            set
            {
                _lookupContextFieldName = ControllerUtilities.ValidateName(value);
            }
        }
        
        public bool Inserting
        {
            get
            {
                return this._inserting;
            }
            set
            {
                this._inserting = value;
            }
        }
        
        public string LastCommandName
        {
            get
            {
                return _lastCommandName;
            }
            set
            {
                _lastCommandName = ControllerUtilities.ValidateName(value);
            }
        }
        
        public string LastCommandArgument
        {
            get
            {
                return _lastCommandArgument;
            }
            set
            {
                _lastCommandArgument = ControllerUtilities.ValidateName(value);
            }
        }
        
        public FieldValue[] ExternalFilter
        {
            get
            {
                return this._externalFilter;
            }
            set
            {
                this._externalFilter = value;
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
        
        public bool IsModal
        {
            get
            {
                return (!(String.IsNullOrEmpty(_contextKey)) && _contextKey.Contains("_ModalDataView"));
            }
        }
        
        public static PageRequest Current
        {
            get
            {
                return ((PageRequest)(HttpContext.Current.Items["PageRequest_Current"]));
            }
        }
        
        public void AssignContext(string controller, string view, ControllerConfiguration config)
        {
            _controller = controller;
            _view = view;
            string referrer = String.Empty;
            if ((PageSize == 1000) && String.IsNullOrEmpty(this.SortExpression))
            {
                // we are processing a request to retrieve static lookup data
                XPathNavigator sortExpressionNode = config.SelectSingleNode("c:dataController/c:views/c:view[@id=\'{0}\']/@sortExpression", view);
                if ((sortExpressionNode != null) && !(String.IsNullOrEmpty(sortExpressionNode.Value)))
                	this.SortExpression = sortExpressionNode.Value;
            }
            if ((HttpContext.Current != null) && (HttpContext.Current.Request.UrlReferrer != null))
            	referrer = HttpContext.Current.Request.UrlReferrer.AbsolutePath;
            this._contextKey = string.Format("{0}/{1}.{2}.{3}", referrer, controller, view, ContextKey);
        }
    }
}
