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
using System.IO;
using System.Data.SqlClient;

namespace MyCompany.Rules
{
    public partial class MemberServiceBillDueBusinessRules : MyCompany.Data.BusinessRules
    {

        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Report" and argument that matches "SendEmailToMember".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(MemberServiceBillDueModel instance)
        {
            // This is the placeholder for method implementation.

            if ((instance.MemberID > 0))
            {
                string fileName;
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("kabir31bd@gmail.com");
                mail.To.Add(instance.Email);
                mail.Subject = "Member Due Bill";
                mail.Body = "check mail with attachment";

                LocalReport report = new LocalReport();
                report.ReportPath = HttpContext.Current.Server.MapPath("/Controls/") + "MemberDueStatement.rdlc";
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "DataSet1";//This refers to the dataset name in the RDLC file
                rds.Value = GetDailyDueStatement(instance.MemberID.Value);
                report.DataSources.Add(rds);
                //Byte[] mybytes = report.Render("WORD");
                Byte[] mybytes = report.Render("PDF"); //for exporting to PDF
                //string PDFPath = @"PDF\MemberDueFile_" + instance.MemberID.Value.ToString() + ".doc";

                string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
                fileName = "MemberDueFile_" + instance.MemberID.Value.ToString() + ".pdf";
                Microsoft.Reporting.WebForms.Warning[] warnings = null;

                string[] streamids = null;
                String mimeType = null;
                String encoding = null;
                String extension = null;
                Byte[] bytes = null;
                
               // bytes = LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
                bool IsExitsPDF = File.Exists(PDFPath + fileName);

                FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                byte[] data = new byte[fs.Length];
                fs.Write(mybytes, 0, mybytes.Length);
                fs.Close();
                //PdfLocation = PDFPath + fileName;
                /*
                using (FileStream fs1 = File.Create(@fileName))
                {
                    fs1.Write(mybytes, 0, mybytes.Length);
                    byte[] data1 = new byte[fs1.Length];
                    fs1.Write(mybytes, 0, mybytes.Length);
                    fs1.Close();
                }
                */
                IsExitsPDF = File.Exists(PDFPath + fileName);

                ///FileStream fs1 = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                ///PdfLocation = PDFPath + fileName;
                if (IsExitsPDF)
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(PDFPath + fileName);
                    mail.Attachments.Add(attachment);

                }
                

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("kabir31bd@gmail.com", "kabir31bd@!#");
                SmtpServer.EnableSsl = true;
                try
                {
                    SmtpServer.Send(mail);
                    Result.ShowAlert("Successfully Send Mail to Member");
                    this.PreventDefault();
                }
                catch 
                { 
                    
                }

            }
        }

        private DataTable GetDailyDueStatement(int pMemberID)
        {
            DataTable dt = new DataTable();
            string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
            SqlConnection con = new SqlConnection(conr);
            {
                SqlCommand cmd = new SqlCommand("USP_Member_DueStatementAsOn", con);
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
