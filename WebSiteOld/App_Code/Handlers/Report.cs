using System;
using System.Web.Caching;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using MyCompany.Data;
using MyCompany.Web;

namespace MyCompany.Handlers
{
	/// <summary>
    /// A collection of parameters controlling the process or report generation.
    /// </summary>
    public class ReportArgs
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _controller;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _view;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _templateName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _format;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _filterDetails;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _sortExpression;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private FieldFilter[] _filter;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _mimeType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _fileNameExtension;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _encoding;
        
        public ReportArgs()
        {
            View = "grid1";
        }
        
        /// <summary>
        /// The name of the data controller
        /// </summary>
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
        
        /// <summary>
        /// The ID of the view. Optional.
        /// </summary>
        public string View
        {
            get
            {
                return this._view;
            }
            set
            {
                this._view = value;
            }
        }
        
        /// <summary>
        /// The name of a custom RDLC template. Optional.
        /// </summary>
        public string TemplateName
        {
            get
            {
                return this._templateName;
            }
            set
            {
                this._templateName = value;
            }
        }
        
        /// <summary>
        /// Report output format. Supported values are Pdf, Word, Excel, and Tiff. The default value is Pdf. Optional.
        /// </summary>
        public string Format
        {
            get
            {
                return this._format;
            }
            set
            {
                this._format = value;
            }
        }
        
        /// <summary>
        /// Specifies a user-friendly description of the filter. The description is displayed on the automatically produced reports below the report header. Optional.
        /// </summary>
        public string FilterDetails
        {
            get
            {
                return this._filterDetails;
            }
            set
            {
                this._filterDetails = value;
            }
        }
        
        /// <summary>
        /// Sort expression that must be applied to the dataset prior to the report generation. Optional.
        /// </summary>
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
        
        /// <summary>
        /// A filter expression that must be applied to the dataset prior to the report generation. Optional.
        /// </summary>
        public FieldFilter[] Filter
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
        
        /// <summary>
        /// Specifies the MIME type of the report produced by Report.Execute() method.
        /// </summary>
        public string MimeType
        {
            get
            {
                return this._mimeType;
            }
            set
            {
                this._mimeType = value;
            }
        }
        
        /// <summary>
        /// Specifies the file name extension of the report produced by Report.Execute() method.
        /// </summary>
        public string FileNameExtension
        {
            get
            {
                return this._fileNameExtension;
            }
            set
            {
                this._fileNameExtension = value;
            }
        }
        
        /// <summary>
        /// Specifies the encoding of the report produced by Report.Execute() method.
        /// </summary>
        public string Encoding
        {
            get
            {
                return this._encoding;
            }
            set
            {
                this._encoding = value;
            }
        }
    }
    
    public partial class Report : ReportBase
    {
        
        /// <summary>
        /// Generates a report using the default or custom report template with optional sort expression and filter applied to the dataset.
        /// </summary>
        /// <param name="args">A collection of parameters that control the report generation.</param>
        /// <returns>A binary array representing the report data.</returns>
        public static byte[] Execute(ReportArgs args)
        {
            Report reportHandler = new Report();
            Stream output = new MemoryStream();
            reportHandler.OutputStream = output;
            reportHandler.Arguments = args;
            PageRequest request = new PageRequest();
            reportHandler.Request = request;
            request.Controller = args.Controller;
            request.View = args.View;
            request.SortExpression = args.SortExpression;
            if (args.Filter != null)
            {
                DataViewExtender dve = new DataViewExtender();
                dve.AssignStartupFilter(args.Filter);
                request.Filter = ((List<string>)(dve.Properties["StartupFilter"])).ToArray();
            }
            ((IHttpHandler)(reportHandler)).ProcessRequest(HttpContext.Current);
            // return report data
            output.Position = 0;
            byte[] data = new byte[output.Length];
            output.Read(data, 0, data.Length);
            return data;
        }
    }
    
    public class ReportBase : GenericHandlerBase, IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ReportArgs _arguments;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private PageRequest _request;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Stream _outputStream;
        
        private static Regex _validationKeyRegex = new Regex("/Blob.ashx\\?");
        
        protected ReportArgs Arguments
        {
            get
            {
                return this._arguments;
            }
            set
            {
                this._arguments = value;
            }
        }
        
        protected PageRequest Request
        {
            get
            {
                return this._request;
            }
            set
            {
                this._request = value;
            }
        }
        
        protected Stream OutputStream
        {
            get
            {
                return this._outputStream;
            }
            set
            {
                this._outputStream = value;
            }
        }
        
        bool IHttpHandler.IsReusable
        {
            get
            {
                return false;
            }
        }
        
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            string c = context.Request["c"];
            string q = context.Request["q"];
            PageRequest request = this.Request;
            if ((request == null) && (String.IsNullOrEmpty(c) || String.IsNullOrEmpty(q)))
            	throw new Exception("Invalid report request.");
            // create a data table for report
            string templateName = null;
            string aa = null;
            string reportFormat = null;
            if (request == null)
            {
                request = JsonConvert.DeserializeObject<PageRequest>(q);
                templateName = context.Request.Form["a"];
                aa = context.Request["aa"];
            }
            else
            {
                templateName = this.Arguments.TemplateName;
                reportFormat = this.Arguments.Format;
                request.FilterDetails = this.Arguments.FilterDetails;
            }
            request.PageIndex = 0;
            request.PageSize = Int32.MaxValue;
            request.RequiresMetaData = true;
            // try to generate a report via a business rule
            ActionArgs args = null;
            if (!(String.IsNullOrEmpty(aa)))
            {
                args = JsonConvert.DeserializeObject<ActionArgs>(aa);
                IDataController controller = ControllerFactory.CreateDataController();
                ActionResult result = controller.Execute(args.Controller, args.View, args);
                if (!(String.IsNullOrEmpty(result.NavigateUrl)))
                {
                    AppendDownloadTokenCookie();
                    context.Response.Redirect(result.NavigateUrl);
                }
                if (result.Canceled)
                {
                    AppendDownloadTokenCookie();
                    return;
                }
                result.RaiseExceptionIfErrors();
                // parse action data
                SortedDictionary<string, string> actionData = new SortedDictionary<string, string>();
                ((DataControllerBase)(controller)).Config.ParseActionData(args.Path, actionData);
                List<string> filter = new List<string>();
                foreach (string name in actionData.Keys)
                {
                    string v = actionData[name];
                    if (name.StartsWith("_"))
                    {
                        if (name == "_controller")
                        	request.Controller = v;
                        if (name == "_view")
                        	request.View = v;
                        if (name == "_sortExpression")
                        	request.SortExpression = v;
                        if (name == "_count")
                        	request.PageSize = Convert.ToInt32(v);
                        if (name == "_template")
                        	templateName = v;
                    }
                    else
                    	if (v == "@Arguments_SelectedValues")
                        	if ((args.SelectedValues != null) && (args.SelectedValues.Length > 0))
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (string key in args.SelectedValues)
                                {
                                    if (sb.Length > 0)
                                    	sb.Append("$or$");
                                    sb.Append(key);
                                }
                                filter.Add(String.Format("{0}:$in${1}", name, sb.ToString()));
                            }
                            else
                            	return;
                        else
                        	if (Regex.IsMatch(v, "^(\'|\").+(\'|\")$"))
                            	filter.Add(String.Format("{0}:={1}", name, v.Substring(1, (v.Length - 2))));
                            else
                            	if (args.Values != null)
                                	foreach (FieldValue fv in args.Values)
                                    	if (fv.Name == v)
                                        	filter.Add(String.Format("{0}:={1}", name, fv.Value));
                    request.Filter = filter.ToArray();
                }
            }
            // load report definition
            string reportTemplate = Controller.CreateReportInstance(null, templateName, request.Controller, request.View);
            ViewPage page = ControllerFactory.CreateDataController().GetPage(request.Controller, request.View, request);
            DataTable table = page.ToDataTable();
            // insert validation key
            reportTemplate = _validationKeyRegex.Replace(reportTemplate, String.Format("/Blob.ashx?_validationKey={0}&amp;", BlobAdapter.ValidationKey));
            // figure report output format
            if (this.Arguments == null)
            {
                Match m = Regex.Match(c, "^(ReportAs|Report)(Pdf|Excel|Image|Word|)$");
                reportFormat = m.Groups[2].Value;
            }
            if (String.IsNullOrEmpty(reportFormat))
            	reportFormat = "Pdf";
            // render a report
            string mimeType = null;
            string encoding = null;
            string fileNameExtension = null;
            string[] streams = null;
            Warning[] warnings = null;
            using (LocalReport report = new LocalReport())
            {
                report.EnableHyperlinks = true;
                report.EnableExternalImages = true;
                report.LoadReportDefinition(new StringReader(reportTemplate));
                report.DataSources.Add(new ReportDataSource(request.Controller, table));
                report.EnableExternalImages = true;
                foreach (ReportParameterInfo p in report.GetParameters())
                {
                    if (p.Name.Equals("FilterDetails") && !(String.IsNullOrEmpty(request.FilterDetails)))
                    	report.SetParameters(new ReportParameter("FilterDetails", request.FilterDetails));
                    if (p.Name.Equals("BaseUrl"))
                    {
                        string baseUrl = String.Format("{0}://{1}{2}", context.Request.Url.Scheme, context.Request.Url.Authority, context.Request.ApplicationPath.TrimEnd('/'));
                        report.SetParameters(new ReportParameter("BaseUrl", baseUrl));
                    }
                    if (p.Name.Equals("Query") && !(String.IsNullOrEmpty(q)))
                    	report.SetParameters(new ReportParameter("Query", HttpUtility.UrlEncode(q)));
                }
                report.SetBasePermissionsForSandboxAppDomain(new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted));
                byte[] reportData = report.Render(reportFormat, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                if (this.Arguments != null)
                {
                    this.Arguments.MimeType = mimeType;
                    this.Arguments.FileNameExtension = fileNameExtension;
                    this.Arguments.Encoding = encoding;
                    this.OutputStream.Write(reportData, 0, reportData.Length);
                }
                else
                {
                    // send report data to the client
                    context.Response.Clear();
                    context.Response.ContentType = mimeType;
                    context.Response.AddHeader("Content-Length", reportData.Length.ToString());
                    AppendDownloadTokenCookie();
                    string fileName = FormatFileName(context, request, fileNameExtension);
                    if (String.IsNullOrEmpty(fileName))
                    {
                        fileName = String.Format("{0}_{1}.{2}", request.Controller, request.View, fileNameExtension);
                        if (args != null)
                        	fileName = GenerateOutputFileName(args, fileName);
                    }
                    context.Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", fileName));
                    context.Response.OutputStream.Write(reportData, 0, reportData.Length);
                }
            }
        }
        
        protected virtual string FormatFileName(HttpContext context, PageRequest request, string extension)
        {
            return null;
        }
    }
}
