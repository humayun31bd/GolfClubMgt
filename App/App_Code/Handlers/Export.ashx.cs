using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using MyCompany.Data;

namespace MyCompany.Handlers
{
	public partial class Export : ExportBase
    {
    }
    
    public class ExportBase : GenericHandlerBase, IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        
        bool IHttpHandler.IsReusable
        {
            get
            {
                return true;
            }
        }
        
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            string fileName = null;
            string q = context.Request.Params["q"];
            if (!(String.IsNullOrEmpty(q)))
            {
                if (q.Contains("{"))
                {
                    q = Convert.ToBase64String(Encoding.Default.GetBytes(q));
                    context.Response.Redirect(((context.Request.AppRelativeCurrentExecutionFilePath + "?q=") 
                                    + HttpUtility.UrlEncode(q)));
                }
                q = Encoding.Default.GetString(Convert.FromBase64String(q));
                ActionArgs args = JsonConvert.DeserializeObject<ActionArgs>(q);
                // execute data export
                IDataController controller = ControllerFactory.CreateDataController();
                // create an Excel Web Query
                if ((args.CommandName == "ExportRowset") && !(context.Request.Url.AbsoluteUri.Contains("&d")))
                {
                    string webQueryUrl = ToClientUrl((context.Request.Url.AbsoluteUri + "&d"));
                    context.Response.Write(("Web\r\n1\r\n" + webQueryUrl));
                    context.Response.ContentType = "text/x-ms-iqy";
                    context.Response.AddHeader("Content-Disposition", String.Format(String.Format("attachment; filename={0}", GenerateOutputFileName(args, String.Format("{0}_{1}.iqy", args.Controller, args.View)))));
                    return;
                }
                // export data in the requested format
                ActionResult result = controller.Execute(args.Controller, args.View, args);
                fileName = ((string)(result.Values[0].Value));
                // send file to output
                if (File.Exists(fileName))
                {
                    if (args.CommandName == "ExportCsv")
                    {
                        context.Response.ContentType = "text/csv";
                        context.Response.AddHeader("Content-Disposition", String.Format(String.Format("attachment; filename={0}", GenerateOutputFileName(args, String.Format("{0}_{1}.csv", args.Controller, args.View)))));
                        context.Response.Charset = "utf-8";
                        context.Response.Write(Convert.ToChar(65279));
                    }
                    else
                    	if (args.CommandName == "ExportRowset")
                        	context.Response.ContentType = "text/xml";
                        else
                        	context.Response.ContentType = "application/rss+xml";
                    StreamReader reader = File.OpenText(fileName);
                    while (!(reader.EndOfStream))
                    {
                        string s = reader.ReadLine();
                        context.Response.Output.WriteLine(s);
                    }
                    reader.Close();
                    File.Delete(fileName);
                }
            }
            if (String.IsNullOrEmpty(fileName))
            	throw new HttpException(404, String.Empty);
        }
        
        protected virtual string ToClientUrl(string url)
        {
            return url;
        }
    }
}
