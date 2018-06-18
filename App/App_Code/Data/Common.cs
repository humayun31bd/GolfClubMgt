using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;

namespace MyCompany.Data
{
	public class SelectClauseDictionary : SortedDictionary<string, string>
    {
        
        public new string this[string name]
        {
            get
            {
                string expression = null;
                if (TryGetValue(name.ToLower(), out expression))
                	return expression;
                return "null";
            }
            set
            {
                base[name.ToLower()] = value;
            }
        }
        
        public new bool ContainsKey(string name)
        {
            return base.ContainsKey(name.ToLower());
        }
        
        public new void Add(string key, string value)
        {
            base.Add(key.ToLower(), value);
        }
        
        public new bool TryGetValue(string key, out string value)
        {
            return base.TryGetValue(key.ToLower(), out value);
        }
    }
    
    public interface IDataController
    {
        
        ViewPage GetPage(string controller, string view, PageRequest request);
        
        object[] GetListOfValues(string controller, string view, DistinctValueRequest request);
        
        ActionResult Execute(string controller, string view, ActionArgs args);
    }
    
    public interface IAutoCompleteManager
    {
        
        string[] GetCompletionList(string prefixText, int count, string contextKey);
    }
    
    public interface IActionHandler
    {
        
        void BeforeSqlAction(ActionArgs args, ActionResult result);
        
        void AfterSqlAction(ActionArgs args, ActionResult result);
        
        void ExecuteAction(ActionArgs args, ActionResult result);
    }
    
    public interface IRowHandler
    {
        
        bool SupportsNewRow(PageRequest requet);
        
        void NewRow(PageRequest request, ViewPage page, object[] row);
        
        bool SupportsPrepareRow(PageRequest request);
        
        void PrepareRow(PageRequest request, ViewPage page, object[] row);
    }
    
    public interface IDataFilter
    {
        
        void Filter(SortedDictionary<string, object> filter);
    }
    
    public interface IDataFilter2
    {
        
        void Filter(string controller, string view, SortedDictionary<string, object> filter);
        
        void AssignContext(string controller, string view, string lookupContextController, string lookupContextView, string lookupContextFieldName);
    }
    
    public interface IDataEngine
    {
        
        DbDataReader ExecuteReader(PageRequest request);
    }
    
    public interface IPlugIn
    {
        
        ControllerConfiguration Config
        {
            get;
            set;
        }
        
        ControllerConfiguration Create(ControllerConfiguration config);
        
        void PreProcessPageRequest(PageRequest request, ViewPage page);
        
        void ProcessPageRequest(PageRequest request, ViewPage page);
        
        void PreProcessArguments(ActionArgs args, ActionResult result, ViewPage page);
        
        void ProcessArguments(ActionArgs args, ActionResult result, ViewPage page);
    }
    
    public class BusinessObjectParameters : SortedDictionary<string, object>
    {
        
        public BusinessObjectParameters(params Object[] values)
        {
            string parameterMarker = null;
            for (int i = 0; (i < values.Length); i++)
            {
                object v = values[i];
                if (v is FieldValue)
                {
                    FieldValue fv = ((FieldValue)(v));
                    Add(fv.Name, fv.Value);
                }
                else
                {
                    if (String.IsNullOrEmpty(parameterMarker))
                    	parameterMarker = SqlStatement.GetParameterMarker(String.Empty);
                    Add((parameterMarker 
                                    + ("p" + i.ToString())), v);
                }
            }
        }
    }
    
    public interface IBusinessObject
    {
        
        void AssignFilter(string filter, BusinessObjectParameters parameters);
    }
    
    public enum CommandConfigurationType
    {
        
        Select,
        
        Update,
        
        Insert,
        
        Delete,
        
        SelectCount,
        
        SelectDistinct,
        
        SelectAggregates,
        
        SelectFirstLetters,
        
        SelectExisting,
        
        Sync,
        
        None,
    }
}
