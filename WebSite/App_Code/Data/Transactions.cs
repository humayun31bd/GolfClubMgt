using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Xml.XPath;
using MyCompany.Services;
using Newtonsoft.Json.Linq;

namespace MyCompany.Data
{
	public class DataConnection : Object, IDisposable
    {
        
        private string _connectionStringName;
        
        private bool _disposed;
        
        private bool _keepOpen;
        
        private bool _canClose;
        
        private DbConnection _connection;
        
        private string _parameterMarker;
        
        private string _leftQuote;
        
        private string _rightQuote;
        
        private DbTransaction _transaction;
        
        private bool _transactionsEnabled;
        
        public DataConnection(string connectionStringName) : 
                this(connectionStringName, false)
        {
        }
        
        public DataConnection(string connectionStringName, bool keepOpen)
        {
            this._connectionStringName = connectionStringName;
            this._keepOpen = keepOpen;
            IDictionary contextItems = HttpContext.Current.Items;
            this._connection = ((DbConnection)(contextItems[ToContextKey("connection")]));
            if (this._connection == null)
            {
                this._connection = SqlStatement.CreateConnection(connectionStringName, true, out _parameterMarker, out _leftQuote, out _rightQuote);
                this._canClose = true;
                if (keepOpen)
                {
                    JToken transactionsEnabled = ApplicationServices.Settings("odp.transactions.enabled");
                    this._transactionsEnabled = ((transactionsEnabled == null) || ((bool)(transactionsEnabled)));
                    BeginTransaction();
                    contextItems[ToContextKey("connection")] = _connection;
                    contextItems[ToContextKey("parameterMarker")] = _parameterMarker;
                    contextItems[ToContextKey("leftQuote")] = _leftQuote;
                    contextItems[ToContextKey("rightQuote")] = _rightQuote;
                }
            }
            else
            {
                _transaction = ((DbTransaction)(contextItems[ToContextKey("transaction")]));
                _parameterMarker = ((string)(contextItems[ToContextKey("parameterMarker")]));
                _leftQuote = ((string)(contextItems[ToContextKey("leftQuote")]));
                _rightQuote = ((string)(contextItems[ToContextKey("rightQuote")]));
            }
        }
        
        public DbConnection Connection
        {
            get
            {
                return _connection;
            }
        }
        
        public DbTransaction Transaction
        {
            get
            {
                return _transaction;
            }
        }
        
        public bool KeepOpen
        {
            get
            {
                return _keepOpen;
            }
        }
        
        public bool CanClose
        {
            get
            {
                return _canClose;
            }
        }
        
        public string ConnectionStringName
        {
            get
            {
                return _connectionStringName;
            }
        }
        
        public string ParameterMarker
        {
            get
            {
                return _parameterMarker;
            }
        }
        
        public string LeftQuote
        {
            get
            {
                return _leftQuote;
            }
        }
        
        public string RightQuote
        {
            get
            {
                return _rightQuote;
            }
        }
        
        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        
        public void Dispose(bool disposing)
        {
            Close();
            if (!(_disposed))
            {
                if ((_connection != null) && _canClose)
                	_connection.Dispose();
                _disposed = true;
            }
            if (disposing)
            	GC.SuppressFinalize(this);
        }
        
        public void Close()
        {
            if ((_connection != null) && (_connection.State == ConnectionState.Open))
            {
                if (_canClose)
                	_connection.Close();
                if (_keepOpen)
                {
                    IDictionary contextItems = HttpContext.Current.Items;
                    contextItems.Remove(ToContextKey("connection"));
                    contextItems.Remove(ToContextKey("transaction"));
                    contextItems.Remove(ToContextKey("parameterMarker"));
                    contextItems.Remove(ToContextKey("leftQuote"));
                    contextItems.Remove(ToContextKey("rightQuote"));
                }
            }
        }
        
        protected string ToContextKey(string name)
        {
            return String.Format("DataConnection_{0}_{1}", _connectionStringName, name);
        }
        
        public void BeginTransaction()
        {
            if (_transactionsEnabled)
            {
                if (this._transaction != null)
                	this._transaction.Dispose();
                this._transaction = this._connection.BeginTransaction();
                HttpContext.Current.Items[ToContextKey("transaction")] = this._transaction;
            }
        }
        
        public void Commit()
        {
            if (this._transaction != null)
            {
                this._transaction.Commit();
                this._transaction.Dispose();
                this._transaction = null;
            }
        }
        
        public void Rollback()
        {
            if (this._transaction != null)
            {
                this._transaction.Rollback();
                this._transaction.Dispose();
                this._transaction = null;
            }
        }
    }
    
    public class ControllerFieldValue : FieldValue
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _controller;
        
        public ControllerFieldValue() : 
                base()
        {
        }
        
        public ControllerFieldValue(string controller, string fieldName, object oldValue, object newValue) : 
                base(fieldName, oldValue, newValue)
        {
            this.Controller = controller;
        }
        
        public string Controller
        {
            get
            {
                return this._controller;
            }
            set
            {
                this._controller = value;
            }
        }
    }
    
    public class CommitResult
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _date;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _sequence;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _index;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string[] _errors;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private List<ControllerFieldValue> _values;
        
        public CommitResult()
        {
            _sequence = -1;
            _index = -1;
            _date = DateTime.Today.ToString("s");
            _values = new List<ControllerFieldValue>();
        }
        
        /// <summary>The timestamp indicating the start of ActionArgs log processing on the server.</summary>
        public string Date
        {
            get
            {
                return this._date;
            }
            set
            {
                this._date = value;
            }
        }
        
        /// <summary>The last committed sequence in the ActionArgs log. Equals -1 if no entries in the log were committed to the database.</summary>
        public int Sequence
        {
            get
            {
                return this._sequence;
            }
            set
            {
                this._sequence = value;
            }
        }
        
        /// <summary>The index of the ActionArgs entry in the log that has caused an error. Equals -1 if no errors were detected.</summary>
        public int Index
        {
            get
            {
                return this._index;
            }
            set
            {
                this._index = value;
            }
        }
        
        /// <summary>The array of errors reported when an entry in the log has failed to executed.</summary>
        public string[] Errors
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
        
        /// <summary>The list of values that includes resolved primary key values.</summary>
        public List<ControllerFieldValue> Values
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
        
        /// <summary>Indicates that the log has been committed sucessfully. Returns false if property Index has any value other than -1.</summary>
        public bool Success
        {
            get
            {
                return (Index == -1);
            }
        }
    }
    
    /// <summary>Provides a mechism to execute an array of ActionArgs instances in the context of a transaction.
    /// Transactions are enabled by default. The default "scope" is "all". The default "upload" is "all".
    /// </summary>
    /// <remarks>
    /// Use the following definition in touch-settings.json file to control Offline Data Processor (ODP):
    /// {
    /// "odp": {
    /// "enabled": true,
    /// "transactions": {
    /// "enabled": true,
    /// "scope": "sequence",
    /// "upload": "all"
    /// }
    /// }
    /// }
    /// </remarks>
    public class TransactionManager : TransactionManagerBase
    {
    }
    
    public class TransactionManagerBase
    {
        
        private SortedDictionary<string, ControllerConfiguration> _controllers;
        
        private SortedDictionary<string, object> _resolvedKeys;
        
        private FieldValue _pk;
        
        private CommitResult _commitResult;
        
        public TransactionManagerBase()
        {
            _controllers = new SortedDictionary<string, ControllerConfiguration>();
            _resolvedKeys = new SortedDictionary<string, object>();
        }
        
        protected virtual ControllerConfiguration LoadConfig(string controllerName)
        {
            ControllerConfiguration config = null;
            if (!(_controllers.TryGetValue(controllerName, out config)))
            {
                config = DataControllerBase.CreateConfigurationInstance(GetType(), controllerName);
                _controllers[controllerName] = config;
            }
            return config;
        }
        
        protected virtual void ResolvePrimaryKey(string controllerName, string fieldName, object oldValue, object newValue)
        {
            _resolvedKeys[String.Format("{0}${1}", controllerName, oldValue)] = newValue;
            _commitResult.Values.Add(new ControllerFieldValue(controllerName, fieldName, oldValue, newValue));
        }
        
        protected virtual void ProcessArguments(ControllerConfiguration config, ActionArgs args)
        {
            if (args.Values == null)
            	return;
            FieldValueDictionary values = new FieldValueDictionary(args);
            _pk = null;
            // detect negative primary keys
            XPathNavigator pkNav = config.SelectSingleNode("/c:dataController/c:fields/c:field[@isPrimaryKey=\'true\']");
            if (pkNav != null)
            {
                FieldValue fv = null;
                if (values.TryGetValue(pkNav.GetAttribute("name", String.Empty), out fv))
                {
                    int value = 0;
                    if ((fv.NewValue != null) && int.TryParse(Convert.ToString(fv.NewValue), out value))
                    {
                        if (value < 0)
                        {
                            if (args.CommandName == "Insert")
                            {
                                // request a new row from business rules
                                PageRequest newRowRequest = new PageRequest();
                                newRowRequest.Controller = args.Controller;
                                newRowRequest.View = args.View;
                                newRowRequest.Inserting = true;
                                newRowRequest.RequiresMetaData = true;
                                newRowRequest.MetadataFilter = new string[] {
                                        "fields"};
                                ViewPage page = ControllerFactory.CreateDataController().GetPage(newRowRequest.Controller, newRowRequest.View, newRowRequest);
                                if (page.NewRow != null)
                                	for (int i = 0; (i < page.NewRow.Length); i++)
                                    {
                                        object newValue = page.NewRow[i];
                                        if (newValue != null)
                                        {
                                            DataField field = page.Fields[i];
                                            if (field.IsPrimaryKey)
                                            {
                                                // resolve the value of the primary key
                                                ResolvePrimaryKey(args.Controller, fv.Name, value, newValue);
                                                value = 0;
                                                fv.NewValue = newValue;
                                            }
                                            else
                                            {
                                                // inject a missing default value in the arguments
                                                FieldValue newFieldValue = null;
                                                if (values.TryGetValue(field.Name, out newFieldValue))
                                                {
                                                    if (!(newFieldValue.Modified))
                                                    {
                                                        newFieldValue.NewValue = newValue;
                                                        newFieldValue.Modified = true;
                                                    }
                                                }
                                                else
                                                {
                                                    List<FieldValue> newValues = new List<FieldValue>(args.Values);
                                                    newFieldValue = new FieldValue(field.Name, newValue);
                                                    newValues.Add(newFieldValue);
                                                    args.Values = newValues.ToArray();
                                                    values[field.Name] = newFieldValue;
                                                }
                                            }
                                        }
                                    }
                            }
                            // resolve the primary key after the command execution
                            if (value < 0)
                            {
                                _pk = new FieldValue(fv.Name, value);
                                fv.NewValue = null;
                                fv.Modified = false;
                            }
                        }
                    }
                }
            }
            // resolve negative foreign keys
            if (_resolvedKeys.Count > 0)
            {
                XPathNodeIterator fkIterator = config.Select("/c:dataController/c:fields/c:field[c:items/@dataController]");
                while (fkIterator.MoveNext())
                {
                    FieldValue fv = null;
                    if (values.TryGetValue(fkIterator.Current.GetAttribute("name", String.Empty), out fv))
                    {
                        XPathNavigator itemsDataControllerNav = fkIterator.Current.SelectSingleNode("c:items/@dataController", config.Resolver);
                        object resolvedKey = null;
                        if (_resolvedKeys.TryGetValue(String.Format("{0}${1}", itemsDataControllerNav.Value, fv.NewValue), out resolvedKey))
                        	fv.NewValue = resolvedKey;
                    }
                }
            }
        }
        
        protected virtual void ProcessResult(ControllerConfiguration config, ActionResult result)
        {
            if (_pk != null)
            	foreach (FieldValue fv in result.Values)
                	if (fv.Name == _pk.Name)
                    {
                        ResolvePrimaryKey(config.ControllerName, fv.Name, _pk.Value, fv.Value);
                        break;
                    }
        }
        
        public virtual CommitResult Commit(JArray log)
        {
            _commitResult = new CommitResult();
            try
            {
                if (log.Count > 0)
                	using (DataConnection connection = new DataConnection(LoadConfig(((string)(log[0]["controller"]))).ConnectionStringName, true))
                    {
                        int index = -1;
                        int sequence = -1;
                        int lastSequence = sequence;
                        string transactionScope = ((string)(ApplicationServices.Settings("odp.transactions.scope")));
                        for (int i = 0; (i < log.Count); i++)
                        {
                            JToken entry = log[i];
                            string controller = ((string)(entry["controller"]));
                            string view = ((string)(entry["view"]));
                            ActionArgs executeArgs = entry["args"].ToObject<ActionArgs>();
                            if (executeArgs.Sequence.HasValue)
                            {
                                sequence = executeArgs.Sequence.Value;
                                if ((transactionScope == "sequence") && (sequence != lastSequence && (i > 0)))
                                {
                                    connection.Commit();
                                    _commitResult.Sequence = lastSequence;
                                    connection.BeginTransaction();
                                }
                                lastSequence = sequence;
                            }
                            ControllerConfiguration config = LoadConfig(executeArgs.Controller);
                            ProcessArguments(config, executeArgs);
                            ActionResult executeResult = ControllerFactory.CreateDataController().Execute(controller, view, executeArgs);
                            if (executeResult.Errors.Count > 0)
                            {
                                index = i;
                                _commitResult.Index = index;
                                _commitResult.Errors = executeResult.Errors.ToArray();
                                break;
                            }
                            else
                            	ProcessResult(config, executeResult);
                        }
                        if (index == -1)
                        {
                            connection.Commit();
                            _commitResult.Sequence = sequence;
                        }
                        else
                        {
                            connection.Rollback();
                            _commitResult.Index = index;
                        }
                    }
            }
            catch (Exception ex)
            {
                _commitResult.Errors = new string[] {
                        ex.Message};
                _commitResult.Index = 0;
            }
            return _commitResult;
        }
    }
}
