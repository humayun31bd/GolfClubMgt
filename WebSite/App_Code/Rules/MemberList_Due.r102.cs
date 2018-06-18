using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;
using System.Net.Mail;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.IO;

namespace MyCompany.Rules
{
	public partial class MemberList_DueBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "custom" and argument that matches "SendEmaiToMemberl".
        /// </summary>
        [Rule("r102")]
        public void r102Implementation(MemberList_DueModel instance)
        {
            // This is the placeholder for method implementation.

            if ((instance.MemberID>0) && (instance.DueAmount>0))
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("kabir31bd@gmail.com");
                mail.To.Add(instance.Email);
                mail.Subject = "Member Due Bill";
                mail.Body = "check mail with attachment";

                LocalReport report = new LocalReport();
                report.ReportPath = HttpContext.Current.Server.MapPath("/Controls/") + "DailyDueStatement.rdlc";
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet1";//This refers to the dataset name in the RDLC file
                rds.Value = GetDailyDueStatement(instance.MemberID.Value);
                report.DataSources.Add(rds);
                Byte[] mybytes = report.Render("WORD");
                //Byte[] mybytes = report.Render("PDF"); for exporting to PDF

                string PDFPath = @"D:\GolfEmail\MemberDueFile_" + instance.MemberID.Value.ToString() + ".doc";

                using (FileStream fs = File.Create(@"D:\GolfEmail\MemberDueFile_" + instance.MemberID.Value.ToString() + ".doc"))
                {
                    fs.Write(mybytes, 0, mybytes.Length);
                }

                //#region [due Report Generate]
                //DataTable dt = GetDailyDueStatement(instance.MemberID);
                //ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                //ReportViewer2.LocalReport.DataSources.Add(rds);
                //this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                //ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "DailyDueStatement.rdlc";

                //ReportViewer2.LocalReport.Refresh();

                //string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
                //Microsoft.Reporting.WebForms.Warning[] warnings = null;
                //string[] streamids = null;
                //String mimeType = null;
                //String encoding = null;
                //String extension = null;
                //Byte[] bytes = null;


                //fileName = "DailyDueStatement" + DateTime.Now.ToFileTime() + ".pdf";


                //bytes = ReportViewer2.LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
                //bool IsExitsPDF = File.Exists(PDFPath + fileName);

                //FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //byte[] data = new byte[fs.Length];
                //fs.Write(bytes, 0, bytes.Length);
                //fs.Close();
                //PdfLocation = PDFPath + fileName;


                //#endregion
                
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(PDFPath);
                mail.Attachments.Add(attachment);
                

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("kabir31bd@gmail.com", "kabir31bd@!#");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }

        }


        private DataTable GetDailyDueStatement(int pMemberID)
        {
            DataTable dt = new DataTable();
            string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
            SqlConnection con = new SqlConnection(conr);
            {
                SqlCommand cmd = new SqlCommand("USP_MemberList_Due", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                try
                {
                    adpt.Fill(dt);
                }
                catch (SqlException ex)
                {
                    //popup("");
                }

            }
            return dt;
        }

    }
}
