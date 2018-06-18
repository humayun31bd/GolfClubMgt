﻿using System;
using System.Data;
using System.IO;
using System.Web;
using Microsoft.Reporting.WebForms;
using MyCompany.Data;

namespace MyCompany.Handlers
{
	public partial class Report : ReportBase
    {
        
        protected override ReportData Render(PageRequest request, DataTable table, string reportTemplate, string reportFormat)
        {
            HttpContext context = HttpContext.Current;
            string q = context.Request["q"];
            // render a report using Microsoft Report Viewer
            string mimeType = null;
            string encoding = null;
            string fileNameExtension = null;
            string[] streams = null;
            Warning[] warnings = null;
            byte[] data = null;
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
                data = report.Render(reportFormat, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            }
            return new ReportData(data, mimeType, fileNameExtension, encoding);
        }
    }
}
