using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using MyCompany.Data;
using MyCompany.Web.Design;

namespace MyCompany.Web
{
	[Designer(typeof(ControllerDataSourceDesigner))]
    [ToolboxData("<{0}:ControllerDataSource runat=\"server\"></{0}:ControllerDataSource>")]
    [PersistChildren(false)]
    [DefaultProperty("DataController")]
    [ParseChildren(true)]
    public class ControllerDataSource : DataSourceControl
    {
        
        private ControllerDataSourceView _view;
        
        private string _pageRequestParameterName;
        
        public ControllerDataSource() : 
                base()
        {
        }
        
        public string DataController
        {
            get
            {
                return GetView().DataController;
            }
            set
            {
                GetView().DataController = value;
            }
        }
        
        public string DataView
        {
            get
            {
                return GetView().DataView;
            }
            set
            {
                GetView().DataView = value;
            }
        }
        
        public string PageRequestParameterName
        {
            get
            {
                return _pageRequestParameterName;
            }
            set
            {
                _pageRequestParameterName = value;
            }
        }
        
        [MergableProperty(false)]
        [DefaultValue("")]
        [Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Versio" +
            "n=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ParameterCollection FilterParameters
        {
            get
            {
                return GetView().FilterParameters;
            }
        }
        
        protected ControllerDataSourceView GetView()
        {
            return ((ControllerDataSourceView)(GetView(String.Empty)));
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Page.LoadComplete += new EventHandler(this.PageLoadComplete);
        }
        
        private void PageLoadComplete(object sender, EventArgs e)
        {
            FilterParameters.UpdateValues(this.Context, this);
        }
        
        protected override ICollection GetViewNames()
        {
            return new string[] {
                    ControllerDataSourceView.DefaultViewName};
        }
        
        protected override DataSourceView GetView(string viewName)
        {
            if (_view == null)
            {
                _view = new ControllerDataSourceView(this, String.Empty);
                if (IsTrackingViewState)
                	((IStateManager)(_view)).TrackViewState();
            }
            return _view;
        }
        
        protected override void LoadViewState(object savedState)
        {
            Pair pair = ((Pair)(savedState));
            if (savedState == null)
            	base.LoadViewState(null);
            else
            {
                base.LoadViewState(pair.First);
                if (pair.Second != null)
                	((IStateManager)(GetView())).LoadViewState(pair.Second);
            }
        }
        
        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (_view != null)
            	((IStateManager)(_view)).TrackViewState();
        }
        
        protected override object SaveViewState()
        {
            Pair pair = new Pair();
            pair.First = base.SaveViewState();
            if (_view != null)
            	pair.Second = ((IStateManager)(_view)).SaveViewState();
            if ((pair.First == null) && (pair.Second == null))
            	return null;
            return pair;
        }
    }
    
    public class ControllerDataSourceView : DataSourceView, IStateManager
    {
        
        public static string DefaultViewName = "DataControllerView";
        
        private string _dataController;
        
        private string _dataView;
        
        private bool _tracking;
        
        private ControllerDataSource _owner;
        
        private ParameterCollection _filterParameters;
        
        public ControllerDataSourceView(IDataSource owner, string viewName) : 
                base(owner, viewName)
        {
            _owner = ((ControllerDataSource)(owner));
        }
        
        public string DataController
        {
            get
            {
                return _dataController;
            }
            set
            {
                _dataController = value;
            }
        }
        
        public string DataView
        {
            get
            {
                return _dataView;
            }
            set
            {
                _dataView = value;
            }
        }
        
        public ParameterCollection FilterParameters
        {
            get
            {
                if (_filterParameters == null)
                {
                    _filterParameters = new ParameterCollection();
                    _filterParameters.ParametersChanged += new EventHandler(this._filterParametersParametersChanged);
                }
                return _filterParameters;
            }
        }
        
        public override bool CanRetrieveTotalRowCount
        {
            get
            {
                return true;
            }
        }
        
        public override bool CanSort
        {
            get
            {
                return true;
            }
        }
        
        public override bool CanPage
        {
            get
            {
                return true;
            }
        }
        
        public override bool CanInsert
        {
            get
            {
                return true;
            }
        }
        
        public override bool CanUpdate
        {
            get
            {
                return true;
            }
        }
        
        public override bool CanDelete
        {
            get
            {
                return true;
            }
        }
        
        bool IStateManager.IsTrackingViewState
        {
            get
            {
                return _tracking;
            }
        }
        
        private void _filterParametersParametersChanged(object sender, EventArgs e)
        {
            OnDataSourceViewChanged(EventArgs.Empty);
        }
        
        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            int pageSize = Int32.MaxValue;
            if (arguments.MaximumRows > 0)
            	pageSize = arguments.MaximumRows;
            int pageIndex = (arguments.StartRowIndex / pageSize);
            PageRequest request = null;
            if (!(String.IsNullOrEmpty(_owner.PageRequestParameterName)))
            {
                string r = HttpContext.Current.Request.Params[_owner.PageRequestParameterName];
                if (!(String.IsNullOrEmpty(r)))
                {
                    request = JsonConvert.DeserializeObject<PageRequest>(r);
                    request.PageIndex = pageIndex;
                    request.PageSize = pageSize;
                    request.View = _owner.DataView;
                }
            }
            if (request == null)
            {
                request = new PageRequest(pageIndex, pageSize, arguments.SortExpression, null);
                List<string> filter = new List<string>();
                System.Collections.Specialized.IOrderedDictionary filterValues = FilterParameters.GetValues(HttpContext.Current, _owner);
                foreach (Parameter p in FilterParameters)
                {
                    object v = filterValues[p.Name];
                    if (v != null)
                    {
                        string query = (p.Name + ":");
                        if ((p.DbType == DbType.Object) || (p.DbType == DbType.String))
                        	foreach (string s in Convert.ToString(v).Split(',', ';'))
                            {
                                string q = Controller.ConvertSampleToQuery(s);
                                if (!(String.IsNullOrEmpty(q)))
                                	query = (query + q);
                            }
                        else
                        	query = String.Format("{0}={1}", query, v);
                        filter.Add(query);
                    }
                }
                request.Filter = filter.ToArray();
            }
            request.RequiresMetaData = true;
            request.RequiresRowCount = arguments.RetrieveTotalRowCount;
            ViewPage page = ControllerFactory.CreateDataController().GetPage(_dataController, _dataView, request);
            if (arguments.RetrieveTotalRowCount)
            	arguments.TotalRowCount = page.TotalRowCount;
            return page.ToDataTable().DefaultView;
        }
        
        protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            FieldValueDictionary fieldValues = new FieldValueDictionary();
            fieldValues.Assign(oldValues, false);
            fieldValues.Assign(keys, false);
            fieldValues.Assign(keys, true);
            fieldValues.Assign(values, true);
            return ExecuteAction("Edit", "Update", fieldValues);
        }
        
        protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
        {
            FieldValueDictionary fieldValues = new FieldValueDictionary();
            fieldValues.Assign(keys, false);
            fieldValues.Assign(keys, true);
            fieldValues.Assign(oldValues, true);
            return ExecuteAction("Select", "Delete", fieldValues);
        }
        
        protected override int ExecuteInsert(IDictionary values)
        {
            FieldValueDictionary fieldValues = new FieldValueDictionary();
            fieldValues.Assign(values, true);
            return ExecuteAction("New", "Insert", fieldValues);
        }
        
        protected int ExecuteAction(string lastCommandName, string commandName, FieldValueDictionary fieldValues)
        {
            ActionArgs args = new ActionArgs();
            args.Controller = DataController;
            args.View = DataView;
            args.LastCommandName = lastCommandName;
            args.CommandName = commandName;
            args.Values = fieldValues.Values.ToArray();
            ActionResult result = ControllerFactory.CreateDataController().Execute(DataController, DataView, args);
            result.RaiseExceptionIfErrors();
            return result.RowsAffected;
        }
        
        protected virtual void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                Pair pair = ((Pair)(savedState));
                if (pair.Second != null)
                	((IStateManager)(FilterParameters)).LoadViewState(pair.Second);
            }
        }
        
        protected virtual object SaveViewState()
        {
            Pair pair = new Pair();
            if (_filterParameters != null)
            	pair.Second = ((IStateManager)(_filterParameters)).SaveViewState();
            if ((pair.First == null) && (pair.Second == null))
            	return null;
            return pair;
        }
        
        protected virtual void TrackViewState()
        {
            _tracking = true;
            if (_filterParameters != null)
            	((IStateManager)(_filterParameters)).TrackViewState();
        }
        
        void IStateManager.LoadViewState(object state)
        {
            LoadViewState(state);
        }
        
        object IStateManager.SaveViewState()
        {
            return SaveViewState();
        }
        
        void IStateManager.TrackViewState()
        {
            TrackViewState();
        }
    }
}
