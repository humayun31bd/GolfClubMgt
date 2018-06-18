using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Xml;
using System.Xml.XPath;
using MyCompany.Data;

namespace MyCompany.Web.Design
{
	public class ControllerDataSourceDesigner : DataSourceDesigner
    {
        
        private ControllerDataSource _control;
        
        private ControllerDataSourceDesignView _view;
        
        public override bool CanRefreshSchema
        {
            get
            {
                return true;
            }
        }
        
        public string DataController
        {
            get
            {
                return _control.DataController;
            }
            set
            {
                if (!((String.Compare(_control.DataController, value, false) == 0)))
                {
                    _control.DataController = value;
                    RefreshSchema(false);
                }
            }
        }
        
        public string DataView
        {
            get
            {
                return _control.DataView;
            }
            set
            {
                if (!((String.Compare(_control.DataView, value, false) == 0)))
                {
                    _control.DataView = value;
                    RefreshSchema(false);
                }
            }
        }
        
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _control = ((ControllerDataSource)(component));
        }
        
        public override DesignerDataSourceView GetView(string viewName)
        {
            if (!(viewName.Equals(ControllerDataSourceView.DefaultViewName)))
            	return null;
            IWebApplication webApp = ((IWebApplication)(this.Component.Site.GetService(typeof(IWebApplication))));
            if (webApp == null)
            	return null;
            IProjectItem item = webApp.GetProjectItemFromUrl("~/Controllers");
            if (_view == null)
            	_view = new ControllerDataSourceDesignView(this, ControllerDataSourceView.DefaultViewName);
            _view.DataController = _control.DataController;
            _view.DataView = _control.DataView;
            if (item != null)
            	_view.BasePath = item.PhysicalPath;
            return _view;
        }
        
        public override string[] GetViewNames()
        {
            return new string[] {
                    ControllerDataSourceView.DefaultViewName};
        }
        
        public override void RefreshSchema(bool preferSilent)
        {
            OnSchemaRefreshed(EventArgs.Empty);
        }
        
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            PropertyDescriptor typeNameProp = ((PropertyDescriptor)(properties["DataController"]));
            properties["DataController"] = TypeDescriptor.CreateProperty(GetType(), typeNameProp, new Attribute[0]);
            typeNameProp = ((PropertyDescriptor)(properties["DataView"]));
            properties["DataView"] = TypeDescriptor.CreateProperty(GetType(), typeNameProp, new Attribute[0]);
        }
    }
    
    public class ControllerDataSourceDesignView : DesignerDataSourceView
    {
        
        private string _basePath;
        
        private string _dataController;
        
        private string _dataView;
        
        public ControllerDataSourceDesignView(ControllerDataSourceDesigner owner, string viewName) : 
                base(owner, viewName)
        {
        }
        
        public string BasePath
        {
            get
            {
                return _basePath;
            }
            set
            {
                _basePath = value;
            }
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
        
        public override bool CanPage
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
        
        public override bool CanRetrieveTotalRowCount
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
        
        public override IDataSourceViewSchema Schema
        {
            get
            {
                XPathDocument document = null;
                Stream res = GetType().Assembly.GetManifestResourceStream(String.Format("MyCompany.Controllers.{0}.xml", DataController));
                if (res == null)
                	res = GetType().Assembly.GetManifestResourceStream(String.Format("MyCompany.{0}.xml", DataController));
                if (res != null)
                	document = new XPathDocument(res);
                else
                {
                    string dataControllerPath = Path.Combine(BasePath, (DataController + ".xml"));
                    document = new XPathDocument(dataControllerPath);
                }
                return new DataViewDesignSchema(document, DataView);
            }
        }
        
        public override IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData)
        {
            IDataSourceFieldSchema[] fields = Schema.GetFields();
            DataTable dt = new DataTable(DataView);
            foreach (IDataSourceFieldSchema field in fields)
            	dt.Columns.Add(field.Name, field.DataType);
            for (int i = 0; (i < minimumRows); i++)
            {
                DataRow row = dt.NewRow();
                foreach (IDataSourceFieldSchema field in fields)
                {
                    string typeName = field.DataType.Name;
                    object v = i;
                    if (typeName == "String")
                    	v = "abc";
                    else
                    	if (typeName == "DateTime")
                        	v = DateTime.Now;
                        else
                        	if (typeName == "Boolean")
                            	v = ((i % 2) == 1);
                            else
                            	if (typeName == "Guid")
                                	v = Guid.NewGuid();
                                else
                                	if (!(typeName.Contains("Int")))
                                    	v = (Convert.ToDouble(i) / 10);
                    row[field.Name] = v;
                }
                dt.Rows.Add(row);
            }
            dt.AcceptChanges();
            isSampleData = true;
            return dt.DefaultView;
        }
    }
    
    public class DataViewDesignSchema : IDataSourceViewSchema
    {
        
        private string _name;
        
        private XmlNamespaceManager _nm;
        
        private XPathNavigator _view;
        
        public DataViewDesignSchema(XPathDocument document, string dataView)
        {
            // initialize the schema
            XPathNavigator navigator = document.CreateNavigator();
            _nm = new XmlNamespaceManager(navigator.NameTable);
            _nm.AddNamespace("a", "urn:schemas-codeontime-com:data-aquarium");
            _name = ((string)(navigator.Evaluate("string(/a:dataController/@name)", _nm)));
            // find the data view metadata
            if (String.IsNullOrEmpty(dataView))
            	_view = navigator.SelectSingleNode("//a:view", _nm);
            else
            	_view = navigator.SelectSingleNode(String.Format("//a:view[@id=\'{0}\']", dataView), _nm);
        }
        
        string IDataSourceViewSchema.Name
        {
            get
            {
                return _name;
            }
        }
        
        IDataSourceViewSchema[] IDataSourceViewSchema.GetChildren()
        {
            return null;
        }
        
        IDataSourceFieldSchema[] IDataSourceViewSchema.GetFields()
        {
            List<IDataSourceFieldSchema> fields = new List<IDataSourceFieldSchema>();
            if (_view != null)
            {
                XPathNodeIterator dataFieldIterator = _view.Select(".//a:dataField", _nm);
                while (dataFieldIterator.MoveNext())
                	fields.Add(new DataViewFieldSchema(dataFieldIterator.Current, _nm));
                XPathNodeIterator systemFieldIterator = _view.Select(String.Format("//a:field[not(@name=//a:view[@id=\'{0}\']//a:dataField/@fieldName) and @isPrimaryKe" +
                            "y=\'true\']", _view.GetAttribute("id", String.Empty)), _nm);
                while (systemFieldIterator.MoveNext())
                	fields.Add(new DataViewFieldSchema(systemFieldIterator.Current, _nm));
            }
            return fields.ToArray();
        }
    }
    
    public class DataViewFieldSchema : IDataSourceFieldSchema
    {
        
        private string _name;
        
        private Type _type;
        
        private bool _identity;
        
        private bool _readOnly;
        
        private bool _unique;
        
        private int _length;
        
        private bool _nullable;
        
        private bool _primaryKey;
        
        public DataViewFieldSchema(XPathNavigator fieldInfo, XmlNamespaceManager nm)
        {
            XPathNavigator field = fieldInfo;
            if (fieldInfo.LocalName == "dataField")
            {
                _name = fieldInfo.GetAttribute("fieldName", String.Empty);
                string aliasFieldName = fieldInfo.GetAttribute("aliasFieldName", String.Empty);
                if (!(String.IsNullOrEmpty(aliasFieldName)))
                	_name = aliasFieldName;
                field = fieldInfo.SelectSingleNode(String.Format("/a:dataController/a:fields/a:field[@name=\'{0}\']", _name), nm);
            }
            else
            	_name = fieldInfo.GetAttribute("name", String.Empty);
            _type = typeof(string);
            if (field != null)
            {
                _type = Controller.TypeMap[field.GetAttribute("type", String.Empty)];
                if (!(String.IsNullOrEmpty(field.GetAttribute("length", String.Empty))))
                	_length = Convert.ToInt32(field.GetAttribute("length", String.Empty));
                _identity = ((bool)(field.Evaluate("@isPrimaryKey=\'true\' and @readOnly=\'true\'")));
                _readOnly = ((bool)(field.Evaluate("@readOnly=\'true\'")));
                _unique = false;
                _nullable = ((bool)(field.Evaluate("not(@allowNulls=\'false\')")));
                _primaryKey = ((bool)(field.Evaluate("@isPrimaryKey=\'true\'")));
            }
        }
        
        Type IDataSourceFieldSchema.DataType
        {
            get
            {
                return _type;
            }
        }
        
        bool IDataSourceFieldSchema.Identity
        {
            get
            {
                return _identity;
            }
        }
        
        bool IDataSourceFieldSchema.IsReadOnly
        {
            get
            {
                return _readOnly;
            }
        }
        
        bool IDataSourceFieldSchema.IsUnique
        {
            get
            {
                return _unique;
            }
        }
        
        int IDataSourceFieldSchema.Length
        {
            get
            {
                return _length;
            }
        }
        
        string IDataSourceFieldSchema.Name
        {
            get
            {
                return _name;
            }
        }
        
        bool IDataSourceFieldSchema.Nullable
        {
            get
            {
                return _nullable;
            }
        }
        
        int IDataSourceFieldSchema.Precision
        {
            get
            {
                return 0;
            }
        }
        
        bool IDataSourceFieldSchema.PrimaryKey
        {
            get
            {
                return _primaryKey;
            }
        }
        
        int IDataSourceFieldSchema.Scale
        {
            get
            {
                return 0;
            }
        }
    }
}
