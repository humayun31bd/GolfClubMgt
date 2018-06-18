using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Security;
using Newtonsoft.Json;
using MyCompany.Handlers;
using MyCompany.Services;

namespace MyCompany.Data
{
	public partial class Controller : DataControllerBase
    {
    }
    
    public partial class DataControllerBase : IDataController, IAutoCompleteManager, IDataEngine, IBusinessObject
    {
        
        public const int MaximumDistinctValues = 200;
        
        public static Type[] SpecialConversionTypes = new Type[] {
                typeof(System.Guid),
                typeof(System.DateTimeOffset),
                typeof(System.TimeSpan)};
        
        public static SpecialConversionFunction[] SpecialConverters;
        
        public static Regex ISO8601DateStringMatcher = new Regex("^\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}$");
        
        public static string[] SpecialTypes = new string[] {
                "System.DateTimeOffset",
                "System.TimeSpan",
                "Microsoft.SqlServer.Types.SqlGeography",
                "Microsoft.SqlServer.Types.SqlHierarchyId"};
        
        private BusinessRules _serverRules;
        
        private FieldValue[] _originalFieldValues;
        
        private SortedDictionary<string, List<string>> _junctionTableMap;
        
        private string _junctionTableFieldName;
        
        public static Stream DefaultDataControllerStream = new MemoryStream();
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _allowPublicAccess;
        
        private DbParameterCollection _resultSetParameters;
        
        static DataControllerBase()
        {
            // initialize type map
            _typeMap = new SortedDictionary<string, Type>();
            _typeMap.Add("AnsiString", typeof(string));
            _typeMap.Add("Binary", typeof(byte[]));
            _typeMap.Add("Byte[]", typeof(byte[]));
            _typeMap.Add("Byte", typeof(byte));
            _typeMap.Add("Boolean", typeof(bool));
            _typeMap.Add("Currency", typeof(decimal));
            _typeMap.Add("Date", typeof(DateTime));
            _typeMap.Add("DateTime", typeof(DateTime));
            _typeMap.Add("Decimal", typeof(decimal));
            _typeMap.Add("Double", typeof(double));
            _typeMap.Add("Guid", typeof(Guid));
            _typeMap.Add("Int16", typeof(short));
            _typeMap.Add("Int32", typeof(int));
            _typeMap.Add("Int64", typeof(long));
            _typeMap.Add("Object", typeof(object));
            _typeMap.Add("SByte", typeof(sbyte));
            _typeMap.Add("Single", typeof(float));
            _typeMap.Add("String", typeof(string));
            _typeMap.Add("Time", typeof(TimeSpan));
            _typeMap.Add("TimeSpan", typeof(DateTime));
            _typeMap.Add("UInt16", typeof(ushort));
            _typeMap.Add("UInt32", typeof(uint));
            _typeMap.Add("UInt64", typeof(ulong));
            _typeMap.Add("VarNumeric", typeof(object));
            _typeMap.Add("AnsiStringFixedLength", typeof(string));
            _typeMap.Add("StringFixedLength", typeof(string));
            _typeMap.Add("Xml", typeof(string));
            _typeMap.Add("DateTime2", typeof(DateTime));
            _typeMap.Add("DateTimeOffset", typeof(DateTimeOffset));
            // initialize rowset type map
            _rowsetTypeMap = new SortedDictionary<string, string>();
            _rowsetTypeMap.Add("AnsiString", "string");
            _rowsetTypeMap.Add("Binary", "bin.base64");
            _rowsetTypeMap.Add("Byte", "u1");
            _rowsetTypeMap.Add("Boolean", "boolean");
            _rowsetTypeMap.Add("Currency", "float");
            _rowsetTypeMap.Add("Date", "date");
            _rowsetTypeMap.Add("DateTime", "dateTime");
            _rowsetTypeMap.Add("Decimal", "float");
            _rowsetTypeMap.Add("Double", "float");
            _rowsetTypeMap.Add("Guid", "uuid");
            _rowsetTypeMap.Add("Int16", "i2");
            _rowsetTypeMap.Add("Int32", "i4");
            _rowsetTypeMap.Add("Int64", "i8");
            _rowsetTypeMap.Add("Object", "string");
            _rowsetTypeMap.Add("SByte", "i1");
            _rowsetTypeMap.Add("Single", "float");
            _rowsetTypeMap.Add("String", "string");
            _rowsetTypeMap.Add("Time", "time");
            _rowsetTypeMap.Add("UInt16", "u2");
            _rowsetTypeMap.Add("UInt32", "u4");
            _rowsetTypeMap.Add("UIn64", "u8");
            _rowsetTypeMap.Add("VarNumeric", "float");
            _rowsetTypeMap.Add("AnsiStringFixedLength", "string");
            _rowsetTypeMap.Add("StringFixedLength", "string");
            _rowsetTypeMap.Add("Xml", "string");
            _rowsetTypeMap.Add("DateTime2", "dateTime");
            _rowsetTypeMap.Add("DateTimeOffset", "dateTime.tz");
            _rowsetTypeMap.Add("TimeSpan", "time");
            // initialize the special converters
            SpecialConverters = new SpecialConversionFunction[SpecialConversionTypes.Length];
            SpecialConverters[0] = ConvertToGuid;
            SpecialConverters[1] = ConvertToDateTimeOffset;
            SpecialConverters[2] = ConvertToTimeSpan;
        }
        
        public DataControllerBase()
        {
            Initialize();
        }
        
        protected virtual FieldValue[] OriginalFieldValues
        {
            get
            {
                return _originalFieldValues;
            }
        }
        
        protected virtual string HierarchyOrganizationFieldName
        {
            get
            {
                return "HierarchyOrganization__";
            }
        }
        
        public virtual bool AllowPublicAccess
        {
            get
            {
                return this._allowPublicAccess;
            }
            set
            {
                this._allowPublicAccess = value;
            }
        }
        
        protected virtual void Initialize()
        {
            CultureManager.Initialize();
        }
        
        public static bool StringIsNull(string s)
        {
            return ((s == "null") || (s == "%js%null"));
        }
        
        public static object ConvertToGuid(object o)
        {
            return new Guid(Convert.ToString(o));
        }
        
        public static object ConvertToDateTimeOffset(object o)
        {
            return System.DateTimeOffset.Parse(Convert.ToString(o));
        }
        
        public static object ConvertToTimeSpan(object o)
        {
            return System.TimeSpan.Parse(Convert.ToString(o));
        }
        
        public static object ConvertToType(Type targetType, object o)
        {
            if (targetType.IsGenericType)
            	targetType = targetType.GetProperty("Value").PropertyType;
            if ((o == null) || o.GetType().Equals(targetType))
            	return o;
            for (int i = 0; (i < SpecialConversionTypes.Length); i++)
            {
                Type t = SpecialConversionTypes[i];
                if (t == targetType)
                	return SpecialConverters[i](o);
            }
            if (o is IConvertible)
            	o = Convert.ChangeType(o, targetType);
            else
            	if (targetType.Equals(typeof(string)) && (o != null))
                	o = o.ToString();
            return o;
        }
        
        public static string ValueToString(object o)
        {
            if ((o != null) && (o.GetType() == typeof(System.DateTime)))
            	o = ((DateTime)(o)).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fff");
            return ("%js%" + JsonConvert.SerializeObject(o));
        }
        
        public static object StringToValue(string s)
        {
            return StringToValue(null, s);
        }
        
        public static object StringToValue(DataField field, string s)
        {
            if (!(String.IsNullOrEmpty(s)) && s.StartsWith("%js%"))
            {
                object v = JsonConvert.DeserializeObject(s.Substring(4));
                if ((v is string) && ISO8601DateStringMatcher.IsMatch(((string)(v))))
                	return System.DateTime.Parse(((string)(v)));
                if (!((v is string)) || ((field == null) || (field.Type == "String")))
                	return v;
                s = ((string)(v));
            }
            else
            	if (ISO8601DateStringMatcher.IsMatch(s))
                	return System.DateTime.Parse(s);
            if (field != null)
            	return TypeDescriptor.GetConverter(Controller.TypeMap[field.Type]).ConvertFromString(s);
            return s;
        }
        
        public static object ConvertObjectToValue(object o)
        {
            if (SpecialTypes.Contains(o.GetType().FullName))
            	return o.ToString();
            return o;
        }
        
        public static object EnsureJsonCompatibility(object o)
        {
            if (o != null)
            	if (o.GetType() == typeof(List<object[]>))
                	foreach (object[] values in ((List<object[]>)(o)))
                    	EnsureJsonCompatibility(values);
                else
                	if ((o is Array) && (o.GetType().GetElementType() == typeof(object)))
                    {
                        object[] row = ((object[])(o));
                        for (int i = 0; (i < row.Length); i++)
                        	row[i] = EnsureJsonCompatibility(row[i]);
                    }
                    else
                    	if (o is DateTime)
                        {
                            DateTime d = ((DateTime)(o));
                            return String.Format("{0:d4}-{1:d2}-{2:d2}T{3:d2}:{4:d2}:{5:d2}.{6:d3}", d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond);
                        }
            return o;
        }
        
        protected BusinessRules CreateBusinessRules()
        {
            return BusinessRules.Create(_config);
        }
        
        private void ApplyFieldFilter(ViewPage page)
        {
            if ((page.FieldFilter != null) && (page.FieldFilter.Length > 0))
            {
                List<DataField> newFields = new List<DataField>();
                foreach (DataField f in page.Fields)
                	if (f.IsPrimaryKey || page.IncludeField(f.Name))
                    	newFields.Add(f);
                page.Fields.Clear();
                page.Fields.AddRange(newFields);
                page.FieldFilter = null;
            }
        }
        
        protected virtual BusinessRules InitBusinessRules(PageRequest request, ViewPage page)
        {
            BusinessRules rules = _config.CreateBusinessRules();
            _serverRules = rules;
            if (_serverRules == null)
            	_serverRules = CreateBusinessRules();
            _serverRules.Page = page;
            _serverRules.RequiresRowCount = (page.RequiresRowCount && !((request.Inserting || request.DoesNotRequireData)));
            if (rules != null)
            	rules.BeforeSelect(request);
            else
            	_serverRules.ExecuteServerRules(request, ActionPhase.Before);
            return rules;
        }
        
        public virtual ViewPage[] GetPageList(PageRequest[] requests)
        {
            List<ViewPage> result = new List<ViewPage>();
            foreach (PageRequest r in requests)
            	result.Add(ControllerFactory.CreateDataController().GetPage(r.Controller, r.View, r));
            return result.ToArray();
        }
        
        public virtual ActionResult[] ExecuteList(ActionArgs[] requests)
        {
            List<ActionResult> result = new List<ActionResult>();
            foreach (ActionArgs r in requests)
            	result.Add(ControllerFactory.CreateDataController().Execute(r.Controller, r.View, r));
            return result.ToArray();
        }
        
        ViewPage IDataController.GetPage(string controller, string view, PageRequest request)
        {
            SelectView(controller, view);
            request.AssignContext(controller, this._viewId, _config);
            ViewPage page = new ViewPage(request);
            if (((page.FieldFilter != null) && !(page.Distinct)) && ((page.FieldFilter.Length > 0) && (Config.SelectSingleNode("/c:dataController/c:businessRules/c:rule[@commandName=\'Select\']") != null)))
            	page.FieldFilter = null;
            if (_config.PlugIn != null)
            	_config.PlugIn.PreProcessPageRequest(request, page);
            _config.AssignDynamicExpressions(page);
            page.ApplyDataFilter(_config.CreateDataFilter(), request.Controller, request.View, request.LookupContextController, request.LookupContextView, request.LookupContextFieldName);
            BusinessRules rules = InitBusinessRules(request, page);
            using (DataConnection connection = CreateConnection(this))
            {
                DbCommand selectCommand = CreateCommand(connection);
                if ((selectCommand == null) && _serverRules.EnableResultSet)
                {
                    PopulatePageFields(page);
                    EnsurePageFields(page, null);
                }
                if (page.RequiresMetaData && page.IncludeMetadata("categories"))
                	PopulatePageCategories(page);
                SyncRequestedPage(request, page, connection);
                ConfigureCommand(selectCommand, page, CommandConfigurationType.Select, null);
                if ((page.PageSize > 0) && !((request.Inserting || request.DoesNotRequireData)))
                {
                    EnsureSystemPageFields(request, page, selectCommand);
                    DbDataReader reader = ExecuteResultSetReader(page);
                    if (reader == null)
                    	if (selectCommand == null)
                        	reader = ExecuteVirtualReader(request, page);
                        else
                        	reader = selectCommand.ExecuteReader();
                    while (page.SkipNext())
                    	reader.Read();
                    List<int> fieldMap = null;
                    while (page.ReadNext() && reader.Read())
                    {
                        if (fieldMap == null)
                        {
                            fieldMap = new List<int>();
                            SortedDictionary<string, int> availableColumns = new SortedDictionary<string, int>();
                            for (int j = 0; (j < reader.FieldCount); j++)
                            	availableColumns[reader.GetName(j).ToLower()] = j;
                            for (int k = 0; (k < page.Fields.Count); k++)
                            {
                                int columnIndex = 0;
                                if (!(availableColumns.TryGetValue(page.Fields[k].Name.ToLower(), out columnIndex)))
                                	columnIndex = -1;
                                fieldMap.Add(columnIndex);
                            }
                        }
                        object[] values = new object[page.Fields.Count];
                        for (int i = 0; (i < values.Length); i++)
                        {
                            int columnIndex = fieldMap[i];
                            if (!((columnIndex == -1)))
                            {
                                DataField field = page.Fields[i];
                                object v = reader[field.Name];
                                if (!(DBNull.Value.Equals(v)))
                                {
                                    if (field.IsMirror)
                                    	v = String.Format(field.DataFormatString, v);
                                    else
                                    	if ((field.Type == "Guid") && (v.GetType() == typeof(byte[])))
                                        	v = new Guid(((byte[])(v)));
                                        else
                                        	v = ConvertObjectToValue(v);
                                    values[i] = v;
                                }
                                if (!(String.IsNullOrEmpty(field.SourceFields)))
                                	values[i] = CreateValueFromSourceFields(field, reader);
                            }
                        }
                        if (page.RequiresPivot)
                        	page.AddPivotValues(values);
                        else
                        	page.Rows.Add(values);
                    }
                    reader.Close();
                }
                if (_serverRules.RequiresRowCount)
                {
                    DbCommand countCommand = CreateCommand(connection);
                    ConfigureCommand(countCommand, page, CommandConfigurationType.SelectCount, null);
                    if (_serverRules.EnableResultSet)
                    	page.TotalRowCount = _serverRules.ResultSetSize;
                    else
                    	if (YieldsSingleRow(countCommand))
                        	page.TotalRowCount = 1;
                        else
                        	if ((page.Rows.Count < page.PageSize) && (page.PageIndex <= 0))
                            	page.TotalRowCount = page.Rows.Count;
                            else
                            	page.TotalRowCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    if (!(request.DoesNotRequireAggregates) && page.RequiresAggregates)
                    {
                        object[] aggregates = new object[page.Fields.Count];
                        if (_serverRules.EnableResultSet)
                        {
                            DataTable dt = ExecuteResultSetTable(page);
                            for (int j = 0; (j < aggregates.Length); j++)
                            {
                                DataField field = page.Fields[j];
                                if (field.Aggregate != DataFieldAggregate.None)
                                {
                                    string func = field.Aggregate.ToString();
                                    if (func == "Count")
                                    {
                                        SortedDictionary<string, string> uniqueValues = new SortedDictionary<string, string>();
                                        foreach (DataRow r in dt.Rows)
                                        {
                                            object v = r[field.Name];
                                            if (!(DBNull.Value.Equals(v)))
                                            	uniqueValues[v.ToString()] = null;
                                        }
                                        aggregates[j] = uniqueValues.Keys.Count;
                                    }
                                    else
                                    {
                                        if (func == "Average")
                                        	func = "avg";
                                        aggregates[j] = dt.Compute(String.Format("{0}([{1}])", func, field.Name), null);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DbCommand aggregateCommand = CreateCommand(connection);
                            ConfigureCommand(aggregateCommand, page, CommandConfigurationType.SelectAggregates, null);
                            DbDataReader reader = aggregateCommand.ExecuteReader();
                            if (reader.Read())
                            	for (int j = 0; (j < aggregates.Length); j++)
                                {
                                    DataField field = page.Fields[j];
                                    if (field.Aggregate != DataFieldAggregate.None)
                                    	aggregates[j] = reader[field.Name];
                                }
                            reader.Close();
                        }
                        for (int i = 0; (i < aggregates.Length); i++)
                        {
                            DataField field = page.Fields[i];
                            if (field.Aggregate != DataFieldAggregate.None)
                            {
                                object v = aggregates[i];
                                if (!(DBNull.Value.Equals(v)) && (v != null))
                                {
                                    if (!(field.FormatOnClient) && !(String.IsNullOrEmpty(field.DataFormatString)))
                                    	v = String.Format(field.DataFormatString, v);
                                    aggregates[i] = v;
                                }
                            }
                        }
                        page.Aggregates = aggregates;
                    }
                }
                if (request.RequiresFirstLetters && this._viewType != "Form")
                	if (!(page.RequiresRowCount))
                    	page.FirstLetters = String.Empty;
                    else
                    {
                        DbCommand firstLettersCommand = CreateCommand(connection);
                        string[] oldFilter = page.Filter;
                        ConfigureCommand(firstLettersCommand, page, CommandConfigurationType.SelectFirstLetters, null);
                        page.Filter = oldFilter;
                        if (!(String.IsNullOrEmpty(page.FirstLetters)))
                        {
                            DbDataReader reader = firstLettersCommand.ExecuteReader();
                            StringBuilder firstLetters = new StringBuilder(page.FirstLetters);
                            while (reader.Read())
                            {
                                firstLetters.Append(",");
                                string letter = Convert.ToString(reader[0]);
                                if (!(String.IsNullOrEmpty(letter)))
                                	firstLetters.Append(letter);
                            }
                            reader.Close();
                            page.FirstLetters = firstLetters.ToString();
                        }
                    }
            }
            if (_config.PlugIn != null)
            	_config.PlugIn.ProcessPageRequest(request, page);
            if (request.Inserting)
            	page.NewRow = new object[page.Fields.Count];
            if (request.Inserting)
            {
                if (_serverRules.SupportsCommand("Sql|Code", "New"))
                	_serverRules.ExecuteServerRules(request, ActionPhase.Execute, "New", page.NewRow);
            }
            else
            	if (_serverRules.SupportsCommand("Sql|Code", "Select") && !(page.Distinct))
                	foreach (object[] row in page.Rows)
                    	_serverRules.ExecuteServerRules(request, ActionPhase.Execute, "Select", row);
            if (!(request.Inserting))
            	PopulateManyToManyFields(page);
            if (rules != null)
            {
                IRowHandler rowHandler = rules;
                if (request.Inserting)
                {
                    if (rowHandler.SupportsNewRow(request))
                    	rowHandler.NewRow(request, page, page.NewRow);
                }
                else
                	if (rowHandler.SupportsPrepareRow(request))
                    	foreach (object[] row in page.Rows)
                        	rowHandler.PrepareRow(request, page, row);
                rules.ProcessPageRequest(request, page);
                if (rules.CompleteConfiguration())
                	ResetViewPage(page);
            }
            if (rules != null)
            	rules.AfterSelect(request);
            else
            	_serverRules.ExecuteServerRules(request, ActionPhase.After);
            _serverRules.Result.Merge(page);
            return page.ToResult(_config, _view);
        }
        
        public virtual void ResetViewPage(ViewPage page)
        {
            page.RequiresMetaData = true;
            SortedDictionary<string, int> fieldIndexes = new SortedDictionary<string, int>();
            for (int i = 0; (i < page.Fields.Count); i++)
            	fieldIndexes[page.Fields[i].Name] = i;
            page.Fields.Clear();
            page.Categories.Clear();
            PopulatePageFields(page);
            EnsurePageFields(page, _expressions);
            PopulatePageCategories(page);
            if (page.NewRow != null)
            	page.NewRow = ReorderRowValues(page, fieldIndexes, page.NewRow);
            if (page.Rows != null)
            	for (int j = 0; (j < page.Rows.Count); j++)
                	page.Rows[j] = ReorderRowValues(page, fieldIndexes, page.Rows[j]);
        }
        
        private object[] ReorderRowValues(ViewPage page, SortedDictionary<string, int> indexes, object[] row)
        {
            object[] newRow = new object[row.Length];
            for (int i = 0; (i < page.Fields.Count); i++)
            {
                DataField field = page.Fields[i];
                newRow[i] = row[indexes[field.Name]];
            }
            return newRow;
        }
        
        object[] IDataController.GetListOfValues(string controller, string view, DistinctValueRequest request)
        {
            SelectView(controller, view);
            ViewPage page = new ViewPage(request);
            page.ApplyDataFilter(_config.CreateDataFilter(), controller, view, request.LookupContextController, request.LookupContextView, request.LookupContextFieldName);
            List<object> distinctValues = new List<object>();
            BusinessRules rules = _config.CreateBusinessRules();
            _serverRules = rules;
            if (_serverRules == null)
            	_serverRules = CreateBusinessRules();
            _serverRules.Page = page;
            if (rules != null)
            	rules.BeforeSelect(request);
            else
            	_serverRules.ExecuteServerRules(request, ActionPhase.Before);
            if (_serverRules.EnableResultSet)
            {
                IDataReader reader = ExecuteResultSetReader(page);
                SortedDictionary<object, object> uniqueValues = new SortedDictionary<object, object>();
                bool hasNull = false;
                while (reader.Read())
                {
                    object v = reader[request.FieldName];
                    if (DBNull.Value.Equals(v))
                    	hasNull = true;
                    else
                    	uniqueValues[v] = v;
                }
                if (hasNull)
                	distinctValues.Add(null);
                foreach (object v in uniqueValues.Keys)
                	if (distinctValues.Count < page.PageSize)
                    	distinctValues.Add(ConvertObjectToValue(v));
                    else
                    	break;
            }
            else
            	using (DataConnection connection = CreateConnection(this))
                {
                    DbCommand command = CreateCommand(connection);
                    ConfigureCommand(command, page, CommandConfigurationType.SelectDistinct, null);
                    DbDataReader reader = command.ExecuteReader();
                    while (reader.Read() && (distinctValues.Count < page.PageSize))
                    {
                        object v = reader.GetValue(0);
                        if (!(DBNull.Value.Equals(v)))
                        	v = ConvertObjectToValue(v);
                        distinctValues.Add(v);
                    }
                    reader.Close();
                }
            if (rules != null)
            	rules.AfterSelect(request);
            else
            	_serverRules.ExecuteServerRules(request, ActionPhase.After);
            object[] result = distinctValues.ToArray();
            EnsureJsonCompatibility(result);
            return result;
        }
        
        ActionResult IDataController.Execute(string controller, string view, ActionArgs args)
        {
            ActionResult result = new ActionResult();
            SelectView(controller, view);
            try
            {
                _serverRules = _config.CreateBusinessRules();
                if (_serverRules == null)
                	_serverRules = CreateBusinessRules();
                IActionHandler handler = ((IActionHandler)(_serverRules));
                if (_config.PlugIn != null)
                	_config.PlugIn.PreProcessArguments(args, result, CreateViewPage());
                EnsureFieldValues(args);
                if (args.SqlCommandType != CommandConfigurationType.None)
                	using (DataConnection connection = CreateConnection(this))
                    {
                        DbCommand command = CreateCommand(connection, args);
                        if ((args.SelectedValues != null) && (((args.LastCommandName == "BatchEdit") && (args.CommandName == "Update")) || ((args.CommandName == "Delete") && (args.SelectedValues.Length > 1))))
                        {
                            ViewPage page = CreateViewPage();
                            PopulatePageFields(page);
                            string originalCommandText = command.CommandText;
                            foreach (string sv in args.SelectedValues)
                            {
                                result.Canceled = false;
                                _serverRules.ClearBlackAndWhiteLists();
                                string[] key = sv.Split(',');
                                int keyIndex = 0;
                                foreach (FieldValue v in OriginalFieldValues)
                                {
                                    DataField field = page.FindField(v.Name);
                                    if (field != null)
                                    	if (!(field.IsPrimaryKey))
                                        	v.Modified = true;
                                        else
                                        	if (v.Name == field.Name)
                                            {
                                                v.OldValue = key[keyIndex];
                                                v.Modified = false;
                                                keyIndex++;
                                            }
                                }
                                ExecutePreActionCommands(args, result, connection);
                                if (handler != null)
                                	handler.BeforeSqlAction(args, result);
                                else
                                	_serverRules.ExecuteServerRules(args, result, ActionPhase.Before);
                                if ((result.Errors.Count == 0) && !(result.Canceled))
                                {
                                    ConfigureCommand(command, null, args.SqlCommandType, args.Values);
                                    result.RowsAffected = (result.RowsAffected + command.ExecuteNonQuery());
                                    if (handler != null)
                                    	handler.AfterSqlAction(args, result);
                                    else
                                    	_serverRules.ExecuteServerRules(args, result, ActionPhase.After);
                                    command.CommandText = originalCommandText;
                                    command.Parameters.Clear();
                                    if (_config.PlugIn != null)
                                    	_config.PlugIn.ProcessArguments(args, result, page);
                                }
                            }
                        }
                        else
                        {
                            ExecutePreActionCommands(args, result, connection);
                            if (handler != null)
                            	handler.BeforeSqlAction(args, result);
                            else
                            	_serverRules.ExecuteServerRules(args, result, ActionPhase.Before);
                            if ((result.Errors.Count == 0) && !(result.Canceled))
                            {
                                if (args.CommandName == "Delete")
                                	ProcessManyToManyFields(args);
                                if (ConfigureCommand(command, null, args.SqlCommandType, args.Values))
                                {
                                    result.RowsAffected = command.ExecuteNonQuery();
                                    if (result.RowsAffected == 0)
                                    {
                                        result.RowNotFound = true;
                                        result.Errors.Add(Localizer.Replace("RecordChangedByAnotherUser", "The record has been changed by another user."));
                                    }
                                    else
                                    	ExecutePostActionCommands(args, result, connection);
                                }
                                if ((args.CommandName == "Insert") || (args.CommandName == "Update"))
                                	ProcessManyToManyFields(args);
                                if (handler != null)
                                	handler.AfterSqlAction(args, result);
                                else
                                	_serverRules.ExecuteServerRules(args, result, ActionPhase.After);
                                if (_config.PlugIn != null)
                                	_config.PlugIn.ProcessArguments(args, result, CreateViewPage());
                            }
                        }
                    }
                else
                	if (args.CommandName.StartsWith("Export"))
                    	ExecuteDataExport(args, result);
                    else
                    	if (args.CommandName.Equals("PopulateDynamicLookups"))
                        	PopulateDynamicLookups(args, result);
                        else
                        	if (args.CommandName.Equals("ProcessImportFile"))
                            	ImportProcessor.Execute(args);
                            else
                            	if (args.CommandName.Equals("Execute"))
                                	using (DataConnection connection = CreateConnection(this))
                                    {
                                        DbCommand command = CreateCommand(connection, args);
                                        command.ExecuteNonQuery();
                                    }
                                else
                                	_serverRules.ProcessSpecialActions(args, result);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(System.Reflection.TargetInvocationException))
                	ex = ex.InnerException;
                HandleException(ex, args, result);
            }
            result.EnsureJsonCompatibility();
            return result;
        }
        
        private void EnsureFieldValues(ActionArgs args)
        {
            _originalFieldValues = args.Values;
            ViewPage page = CreateViewPage();
            FieldValueDictionary fieldValues = new FieldValueDictionary();
            if (args.Values == null)
            	args.Values = new FieldValue[0];
            if (args.Values.Length > 0)
            	fieldValues.AddRange(args.Values);
            List<FieldValue> missingValues = new List<FieldValue>();
            foreach (DataField f in page.Fields)
            	if (!(fieldValues.ContainsKey(f.Name)))
                	missingValues.Add(new FieldValue(f.Name));
            if (missingValues.Count > 0)
            {
                List<FieldValue> newValues = new List<FieldValue>(args.Values);
                newValues.AddRange(missingValues);
                args.Values = newValues.ToArray();
            }
        }
        
        private bool SupportsLimitInSelect(object command)
        {
            return command.ToString().Contains("MySql");
        }
        
        private bool SupportsSkipInSelect(object command)
        {
            return command.ToString().Contains("Firebird");
        }
        
        protected virtual void SyncRequestedPage(PageRequest request, ViewPage page, DataConnection connection)
        {
            if (((request.SyncKey == null) || (request.SyncKey.Length == 0)) || (page.PageSize < 0))
            	return;
            DbCommand syncCommand = CreateCommand(connection);
            ConfigureCommand(syncCommand, page, CommandConfigurationType.Sync, null);
            List<DataField> keyFields = page.EnumerateSyncFields();
            if ((keyFields.Count > 0) && (keyFields.Count == request.SyncKey.Length))
            {
                bool useSkip = (_serverRules.EnableResultSet || SupportsSkipInSelect(syncCommand));
                if (!(useSkip))
                	for (int i = 0; (i < keyFields.Count); i++)
                    {
                        DataField field = keyFields[i];
                        DbParameter p = syncCommand.CreateParameter();
                        p.ParameterName = String.Format("{0}PrimaryKey_{1}", _parameterMarker, field.Name);
                        try
                        {
                            AssignParameterValue(p, field, request.SyncKey[i]);
                        }
                        catch (Exception )
                        {
                            return;
                        }
                        syncCommand.Parameters.Add(p);
                    }
                DbDataReader reader;
                if (_serverRules.EnableResultSet)
                	reader = ExecuteResultSetReader(page);
                else
                	reader = syncCommand.ExecuteReader();
                if (!(useSkip))
                {
                    if (reader.Read())
                    {
                        long rowIndex = Convert.ToInt64(reader[0]);
                        page.PageIndex = Convert.ToInt32(Math.Floor((Convert.ToDouble((rowIndex - 1)) / Convert.ToDouble(page.PageSize))));
                        page.PageOffset = 0;
                    }
                }
                else
                {
                    long rowIndex = 1;
                    List<int> keyFieldIndexes = new List<int>();
                    foreach (DataField pkField in keyFields)
                    	keyFieldIndexes.Add(reader.GetOrdinal(pkField.Name));
                    while (reader.Read())
                    {
                        int matchCount = 0;
                        foreach (int primaryKeyFieldIndex in keyFieldIndexes)
                        	if (Convert.ToString(reader[primaryKeyFieldIndex]) == Convert.ToString(request.SyncKey[matchCount]))
                            	matchCount++;
                            else
                            	break;
                        if (matchCount == keyFieldIndexes.Count)
                        {
                            page.PageIndex = Convert.ToInt32(Math.Floor((Convert.ToDouble((rowIndex - 1)) / Convert.ToDouble(page.PageSize))));
                            page.PageOffset = 0;
                            page.ResetSkipCount(false);
                            break;
                        }
                        else
                        	rowIndex++;
                    }
                }
                reader.Close();
            }
        }
        
        protected virtual void HandleException(Exception ex, ActionArgs args, ActionResult result)
        {
            while (ex != null)
            {
                result.Errors.Add(ex.Message);
                ex = ex.InnerException;
            }
        }
        
        DbDataReader IDataEngine.ExecuteReader(PageRequest request)
        {
            _viewPage = new ViewPage(request);
            if (_config == null)
            {
                _config = CreateConfiguration(request.Controller);
                SelectView(request.Controller, request.View);
            }
            _viewPage.ApplyDataFilter(_config.CreateDataFilter(), request.Controller, request.View, null, null, null);
            InitBusinessRules(request, _viewPage);
            DbConnection connection = CreateConnection();
            DbCommand selectCommand = CreateCommand(connection);
            ConfigureCommand(selectCommand, _viewPage, CommandConfigurationType.Select, null);
            return selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
        }
        
        string[] IAutoCompleteManager.GetCompletionList(string prefixText, int count, string contextKey)
        {
            if (contextKey == null)
            	return null;
            string[] arguments = contextKey.Split(',');
            if (arguments.Length != 3)
            	return null;
            DistinctValueRequest request = new DistinctValueRequest();
            request.FieldName = arguments[2];
            string filter = (request.FieldName + ":");
            foreach (string s in prefixText.Split(',', ';'))
            {
                string query = Controller.ConvertSampleToQuery(s);
                if (!(String.IsNullOrEmpty(query)))
                	filter = (filter + query);
            }
            request.Filter = new string[] {
                    filter};
            request.AllowFieldInFilter = true;
            request.MaximumValueCount = count;
            request.Controller = arguments[0];
            request.View = arguments[1];
            object[] list = ControllerFactory.CreateDataController().GetListOfValues(arguments[0], arguments[1], request);
            List<string> result = new List<string>();
            foreach (object o in list)
            	result.Add(Convert.ToString(o));
            return result.ToArray();
        }
        
        void IBusinessObject.AssignFilter(string filter, BusinessObjectParameters parameters)
        {
            _viewFilter = filter;
            _parameters = parameters;
        }
        
        public static string GetSelectView(string controller)
        {
            ControllerUtilities c = new ControllerUtilities();
            return c.GetActionView(controller, "editForm1", "Select");
        }
        
        public static string GetUpdateView(string controller)
        {
            ControllerUtilities c = new ControllerUtilities();
            return c.GetActionView(controller, "editForm1", "Update");
        }
        
        public static string GetInsertView(string controller)
        {
            ControllerUtilities c = new ControllerUtilities();
            return c.GetActionView(controller, "createForm1", "Insert");
        }
        
        public static string GetDeleteView(string controller)
        {
            ControllerUtilities c = new ControllerUtilities();
            return c.GetActionView(controller, "editForm1", "Delete");
        }
        
        private void PopulateManyToManyFields(ViewPage page)
        {
            string primaryKeyField = String.Empty;
            foreach (DataField field in page.Fields)
            	if (!(String.IsNullOrEmpty(field.ItemsTargetController)))
                {
                    if (String.IsNullOrEmpty(primaryKeyField))
                    	foreach (DataField f in page.Fields)
                        	if (f.IsPrimaryKey)
                            {
                                primaryKeyField = f.Name;
                                break;
                            }
                    PopulateManyToManyField(page, field, primaryKeyField);
                }
        }
        
        public void PopulateManyToManyField(ViewPage page, DataField field, string primaryKeyField)
        {
            if (_junctionTableFieldName != field.Name)
            {
                _junctionTableFieldName = field.Name;
                _junctionTableMap = null;
            }
            if (_junctionTableMap == null)
            {
                _junctionTableMap = new SortedDictionary<string, List<string>>();
                if (page.Rows.Count > 0)
                {
                    // read contents of junction table from the database for each row of the page
                    int foreignKeyIndex = page.IndexOfField(primaryKeyField);
                    StringBuilder listOfForeignKeys = new StringBuilder();
                    foreach (object[] row in page.Rows)
                    {
                        if (listOfForeignKeys.Length > 0)
                        	listOfForeignKeys.Append("$or$");
                        listOfForeignKeys.Append(DataControllerBase.ConvertObjectToValue(row[foreignKeyIndex]));
                    }
                    string targetForeignKey1 = null;
                    string targetForeignKey2 = null;
                    ViewPage.InitializeManyToManyProperties(field, page.Controller, out targetForeignKey1, out targetForeignKey2);
                    string filter = String.Format("{0}:$in${1}", targetForeignKey1, listOfForeignKeys.ToString());
                    PageRequest request = new PageRequest(0, Int32.MaxValue, null, new string[] {
                                filter});
                    request.RequiresMetaData = true;
                    ViewPage manyToManyPage = ControllerFactory.CreateDataController().GetPage(field.ItemsTargetController, null, request);
                    // enumerate values in junction table
                    int targetForeignKey1Index = manyToManyPage.IndexOfField(targetForeignKey1);
                    int targetForeignKey2Index = manyToManyPage.IndexOfField(targetForeignKey2);
                    // determine text field for items
                    SortedDictionary<object, object> items = new SortedDictionary<object, object>();
                    List<object> keyList = new List<object>();
                    int targetTextIndex = -1;
                    if (!(field.SupportsStaticItems()))
                    	foreach (DataField f in manyToManyPage.Fields)
                        	if (f.Name == targetForeignKey2)
                            {
                                if (!(String.IsNullOrEmpty(f.AliasName)))
                                	targetTextIndex = manyToManyPage.IndexOfField(f.AliasName);
                                else
                                	targetTextIndex = manyToManyPage.IndexOfField(f.Name);
                                break;
                            }
                    foreach (object[] row in manyToManyPage.Rows)
                    {
                        object v1 = row[targetForeignKey1Index];
                        object v2 = row[targetForeignKey2Index];
                        if (v1 != null)
                        {
                            string s1 = Convert.ToString(v1);
                            List<string> values = null;
                            if (!(_junctionTableMap.TryGetValue(s1, out values)))
                            {
                                values = new List<string>();
                                _junctionTableMap[s1] = values;
                            }
                            values.Add(Convert.ToString(v2));
                            if (!((targetTextIndex == -1)))
                            {
                                object text = row[targetTextIndex];
                                if (!(items.ContainsKey(v2)))
                                {
                                    items.Add(v2, text);
                                    keyList.Add(v2);
                                }
                            }
                        }
                    }
                    if (items.Count != 0)
                    	foreach (object k in keyList)
                        {
                            object v = items[k];
                            field.Items.Add(new object[] {
                                        k,
                                        v});
                        }
                }
            }
            foreach (object[] values in page.Rows)
            {
                string key = Convert.ToString(page.SelectFieldValue(primaryKeyField, values));
                List<string> keyValues = null;
                if (_junctionTableMap.TryGetValue(key, out keyValues))
                	page.UpdateFieldValue(field.Name, values, String.Join(",", keyValues.ToArray()));
            }
        }
        
        private void ProcessManyToManyFields(ActionArgs args)
        {
            XPathNodeIterator m2mFields = Config.Select("/c:dataController/c:fields/c:field[c:items/@targetController!=\'\']");
            if (m2mFields.Count > 0)
            {
                XPathNavigator primaryKeyNode = Config.SelectSingleNode("/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']");
                FieldValue primaryKey = args.SelectFieldValueObject(primaryKeyNode.GetAttribute("name", String.Empty));
                while (m2mFields.MoveNext())
                {
                    DataField field = new DataField(m2mFields.Current, Config.Resolver);
                    FieldValue fv = args.SelectFieldValueObject(field.Name);
                    if (fv != null)
                    {
                        if (args.CommandName == "Delete")
                        	fv.NewValue = null;
                        ProcessManyToManyField(args.Controller, field, fv, primaryKey.Value);
                        fv.Modified = false;
                    }
                }
            }
        }
        
        public void ProcessManyToManyField(string controllerName, DataField field, FieldValue fieldValue, object primaryKey)
        {
            List<string> oldValues = BusinessRulesBase.ValueToList(((string)(fieldValue.OldValue)));
            List<string> newValues = BusinessRulesBase.ValueToList(((string)(fieldValue.NewValue)));
            if (!(BusinessRulesBase.ListsAreEqual(oldValues, newValues)))
            {
                string targetForeignKey1 = null;
                string targetForeignKey2 = null;
                ViewPage.InitializeManyToManyProperties(field, controllerName, out targetForeignKey1, out targetForeignKey2);
                IDataController controller = ControllerFactory.CreateDataController();
                foreach (string s in oldValues)
                	if (!(newValues.Contains(s)))
                    {
                        ActionArgs deleteArgs = new ActionArgs();
                        deleteArgs.Controller = field.ItemsTargetController;
                        deleteArgs.CommandName = "Delete";
                        deleteArgs.LastCommandName = "Select";
                        deleteArgs.Values = new FieldValue[] {
                                new FieldValue(targetForeignKey1, primaryKey, primaryKey),
                                new FieldValue(targetForeignKey2, s, s),
                                new FieldValue("_IgnorePrimaryKeyInWhere", true)};
                        ActionResult result = controller.Execute(field.ItemsTargetController, null, deleteArgs);
                        result.RaiseExceptionIfErrors();
                    }
                foreach (string s in newValues)
                	if (!(oldValues.Contains(s)))
                    {
                        ActionArgs updateArgs = new ActionArgs();
                        updateArgs.Controller = field.ItemsTargetController;
                        updateArgs.CommandName = "Insert";
                        updateArgs.LastCommandName = "New";
                        updateArgs.Values = new FieldValue[] {
                                new FieldValue(targetForeignKey1, primaryKey),
                                new FieldValue(targetForeignKey2, s)};
                        ActionResult result = controller.Execute(field.ItemsTargetController, null, updateArgs);
                        result.RaiseExceptionIfErrors();
                    }
            }
        }
        
        public virtual Stream GetDataControllerStream(string controller)
        {
            return null;
        }
        
        public virtual string GetSurvey(string surveyName)
        {
            string root = Path.Combine(HttpRuntime.AppDomainAppPath, "js", "surveys");
            string survey = ControllerConfigurationUtility.GetFileText(Path.Combine(root, (surveyName + ".min.js")), Path.Combine(root, (surveyName + ".js")));
            string layout = ControllerConfigurationUtility.GetFileText(Path.Combine(root, (surveyName + ".html")), Path.Combine(root, (surveyName + ".htm")));
            string rules = ControllerConfigurationUtility.GetFileText(Path.Combine(root, (surveyName + ".rules.min.js")), Path.Combine(root, (surveyName + ".rules.js")));
            if (String.IsNullOrEmpty(survey))
            	survey = ControllerConfigurationUtility.GetResourceText(String.Format("MyCompany.Surveys.{0}.min.js", surveyName), String.Format("MyCompany.{0}.min.js", surveyName), String.Format("MyCompany.Surveys.{0}.js", surveyName), String.Format("MyCompany.{0}.js", surveyName));
            if (String.IsNullOrEmpty(survey))
            	throw new HttpException(404, "Not found.");
            if (String.IsNullOrEmpty(layout))
            	layout = ControllerConfigurationUtility.GetResourceText(String.Format("MyCompany.Surveys.{0}.html", surveyName), String.Format("MyCompany.Surveys.{0}.htm", surveyName), String.Format("MyCompany.{0}.html", surveyName), String.Format("MyCompany.{0}.htm", surveyName));
            if (String.IsNullOrEmpty(rules))
            	rules = ControllerConfigurationUtility.GetResourceText(String.Format("MyCompany.Surveys.{0}.rules.min.js", surveyName), String.Format("MyCompany.{0}.rules.min.js", surveyName), String.Format("MyCompany.Surveys.{0}.rules.js", surveyName), String.Format("MyCompany.{0}.rules.js", surveyName));
            StringBuilder sb = new StringBuilder();
            if (!(String.IsNullOrEmpty(rules)))
            {
                sb.AppendLine("(function() {");
                sb.AppendFormat("$app.survey(\'register\', \'{0}\', function () {{", surveyName);
                sb.AppendLine();
                sb.AppendLine(rules);
                sb.AppendLine("});");
                sb.AppendLine("})();");
            }
            if (!(String.IsNullOrEmpty(layout)))
            	survey = Regex.Replace(survey, "}\\s*\\)\\s*;?\\s*$", String.Format(", layout: \'{0}\' }});", HttpUtility.JavaScriptStringEncode(layout)));
            sb.Append(survey);
            return sb.ToString();
        }
        
        protected virtual DbDataReader ExecuteVirtualReader(PageRequest request, ViewPage page)
        {
            DataTable table = new DataTable();
            foreach (DataField field in page.Fields)
            	table.Columns.Add(field.Name, typeof(int));
            DataRow r = table.NewRow();
            if (page.ContainsField("PrimaryKey"))
            	r["PrimaryKey"] = 1;
            table.Rows.Add(r);
            return new DataTableReader(table);
        }
        
        protected virtual string GetRequestedViewType(ViewPage page)
        {
            string viewType = page.ViewType;
            if (String.IsNullOrEmpty(viewType))
            	viewType = _view.GetAttribute("type", String.Empty);
            return viewType;
        }
        
        protected virtual void EnsureSystemPageFields(PageRequest request, ViewPage page, DbCommand command)
        {
            if (page.Distinct)
            {
                int i = 0;
                while (i < page.Fields.Count)
                	if (page.Fields[i].IsPrimaryKey)
                    	page.Fields.RemoveAt(i);
                    else
                    	i++;
                DataField field = new DataField();
                field.Name = "group_count_";
                field.Type = "Double";
                page.Fields.Add(field);
            }
            if (!(RequiresHierarchy(page)))
            	return;
            bool requiresHierarchyOrganization = false;
            foreach (DataField field in page.Fields)
            	if (field.IsTagged("hierarchy-parent"))
                	requiresHierarchyOrganization = true;
                else
                	if (field.IsTagged("hierarchy-organization"))
                    {
                        requiresHierarchyOrganization = false;
                        break;
                    }
            if (requiresHierarchyOrganization)
            {
                DataField field = new DataField();
                field.Name = HierarchyOrganizationFieldName;
                field.Type = "String";
                field.Tag = "hierarchy-organization";
                field.Len = 255;
                field.Columns = 20;
                field.Hidden = true;
                field.ReadOnly = true;
                page.Fields.Add(field);
            }
        }
        
        protected virtual bool RequiresHierarchy(ViewPage page)
        {
            if (!((GetRequestedViewType(page) == "DataSheet")))
            	return false;
            foreach (DataField field in page.Fields)
            	if (field.IsTagged("hierarchy-parent"))
                {
                    if ((page.Filter != null) && (page.Filter.Length > 0))
                    	return false;
                    return true;
                }
            return false;
        }
        
        protected virtual bool DatabaseEngineIs(DbCommand command, params System.String[] flavors)
        {
            return DatabaseEngineIs(command.GetType().FullName, flavors);
        }
        
        protected virtual bool DatabaseEngineIs(string typeName, params System.String[] flavors)
        {
            foreach (string s in flavors)
            	if (typeName.Contains(s))
                	return true;
            return false;
        }
        
        protected virtual bool ValidateViewAccess(string controller, string view, string access)
        {
            HttpContext context = HttpContext.Current;
            if (AllowPublicAccess || (context.Request.Params["_validationKey"] == BlobAdapter.ValidationKey))
            	return true;
            bool allow = true;
            string executionFilePath = context.Request.AppRelativeCurrentExecutionFilePath;
            if (!(executionFilePath.StartsWith("~/appservices/", StringComparison.OrdinalIgnoreCase)) && !(executionFilePath.Equals("~/charthost.aspx", StringComparison.OrdinalIgnoreCase)))
            {
                if (!(context.User.Identity.IsAuthenticated) && !(controller.StartsWith("aspnet_")))
                	allow = (access == "Public");
            }
            return allow;
        }
        
        DataTable ExecuteResultSetTable(ViewPage page)
        {
            if (_serverRules.ResultSet == null)
            	return null;
            SelectClauseDictionary expressions = new SelectClauseDictionary();
            foreach (DataColumn c in _serverRules.ResultSet.Columns)
            	expressions[c.ColumnName] = c.ColumnName;
            if (page.Fields.Count == 0)
            {
                PopulatePageFields(page);
                EnsurePageFields(page, null);
            }
            DataView resultView = new DataView(_serverRules.ResultSet);
            resultView.Sort = page.SortExpression;
            using (DbConnection connection = CreateConnection(false))
            {
                DbCommand command = connection.CreateCommand();
                StringBuilder sb = new StringBuilder();
                _resultSetParameters = command.Parameters;
                expressions.Add("_DataView_RowFilter_", "true");
                AppendFilterExpressionsToWhere(sb, page, command, expressions, String.Empty);
                string filter = sb.ToString();
                if (filter.StartsWith("where"))
                	filter = filter.Substring(5);
                filter = Regex.Replace(filter, (Regex.Escape(_parameterMarker) + "\\w+"), DoReplaceResultSetParameter);
                resultView.RowFilter = filter;
                if (page.PageSize > 0)
                	page.TotalRowCount = resultView.Count;
            }
            if (RequiresPreFetching(page))
            	page.ResetSkipCount(true);
            DataTable result = resultView.ToTable();
            string[] fieldFilter = page.RequestedFieldFilter();
            if ((fieldFilter != null) && (fieldFilter.Length > 0))
            {
                int fieldIndex = 0;
                while (fieldIndex < page.Fields.Count)
                {
                    string fieldName = page.Fields[fieldIndex].Name;
                    if ((Array.IndexOf(fieldFilter, fieldName) == -1) && fieldName != "group_count_")
                    	page.Fields.RemoveAt(fieldIndex);
                    else
                    	fieldIndex++;
                }
            }
            if (page.Distinct)
            {
                DataTable groupedTable = result.DefaultView.ToTable(true, fieldFilter);
                groupedTable.Columns.Add(new DataColumn("group_count_", typeof(int)));
                foreach (DataRow r in groupedTable.Rows)
                {
                    StringBuilder filterExpression = new StringBuilder();
                    foreach (string fieldName in fieldFilter)
                    {
                        if (filterExpression.Length > 0)
                        	filterExpression.Append("and");
                        filterExpression.AppendFormat("({0}=\'{1}\')", fieldName, r[fieldName].ToString().Replace("\'", "\\\'\\\'"));
                    }
                    result.DefaultView.RowFilter = filterExpression.ToString();
                    r["group_count_"] = result.DefaultView.Count;
                }
                result = groupedTable;
            }
            _serverRules.ResultSetSize = result.Rows.Count;
            return result;
        }
        
        DbDataReader ExecuteResultSetReader(ViewPage page)
        {
            if (_serverRules.ResultSet == null)
            	return null;
            return ExecuteResultSetTable(page).CreateDataReader();
        }
        
        protected virtual string DoReplaceResultSetParameter(Match m)
        {
            DbParameter p = _resultSetParameters[m.Value];
            return String.Format("\'{0}\'", p.Value.ToString().Replace("\'", "\'\'"));
        }
        
        bool RequiresPreFetching(ViewPage page)
        {
            string viewType = page.ViewType;
            if (String.IsNullOrEmpty(viewType))
            	viewType = _view.GetAttribute("type", String.Empty);
            return (page.PageSize != Int32.MaxValue && new ControllerUtilities().SupportsCaching(page, viewType));
        }
        
        public delegate object SpecialConversionFunction(object o);
    }
    
    public partial class ControllerUtilities : ControllerUtilitiesBase
    {
    }
    
    public class ControllerUtilitiesBase
    {
        
        public virtual bool SupportsScrollingInDataSheet
        {
            get
            {
                return false;
            }
        }
        
        public virtual string GetActionView(string controller, string view, string action)
        {
            return view;
        }
        
        public virtual bool UserIsInRole(params System.String[] roles)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            	return true;
            int count = 0;
            foreach (string r in roles)
            	if (!(String.IsNullOrEmpty(r)))
                	foreach (string role in r.Split(','))
                    {
                        string testRole = role.Trim();
                        if (!(String.IsNullOrEmpty(testRole)))
                        {
                            string roleKey = ("IsInRole_" + testRole);
                            object isInRole = context.Items[roleKey];
                            if (isInRole == null)
                            {
                                isInRole = context.User.IsInRole(testRole);
                                context.Items[roleKey] = isInRole;
                            }
                            if ((bool)(isInRole))
                            	return true;
                        }
                        count++;
                    }
            return (count == 0);
        }
        
        public virtual bool SupportsLastEnteredValues(string controller)
        {
            return false;
        }
        
        public virtual bool SupportsCaching(ViewPage page, string viewType)
        {
            if (viewType == "DataSheet")
            {
                if (!(SupportsScrollingInDataSheet) && !(ApplicationServices.IsTouchClient))
                	page.SupportsCaching = false;
            }
            else
            	if (viewType == "Grid")
                {
                    if (!(ApplicationServices.IsTouchClient))
                    	page.SupportsCaching = false;
                }
                else
                	page.SupportsCaching = false;
            return page.SupportsCaching;
        }
        
        public static string ValidateName(string name)
        {
            if (!(String.IsNullOrEmpty(name)))
            	return name.Replace("\'", "\'");
            return name;
        }
    }
    
    public class ControllerFactory
    {
        
        public static IDataController CreateDataController()
        {
            return new Controller();
        }
        
        public static IAutoCompleteManager CreateAutoCompleteManager()
        {
            return new Controller();
        }
        
        public static IDataEngine CreateDataEngine()
        {
            return new Controller();
        }
        
        public static Stream GetDataControllerStream(string controller)
        {
            return new Controller().GetDataControllerStream(controller);
        }
        
        public static string GetSurvey(string survey)
        {
            return new Controller().GetSurvey(survey);
        }
    }
    
    public partial class StringEncryptor : StringEncryptorBase
    {
    }
    
    public class StringEncryptorBase
    {
        
        public virtual string Encrypt(string s)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(s));
        }
        
        public virtual string Decrypt(string s)
        {
            return Encoding.Default.GetString(Convert.FromBase64String(s));
        }
    }
}
