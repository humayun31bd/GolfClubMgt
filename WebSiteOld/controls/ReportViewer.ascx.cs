using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class Controls_ReportViewer : System.Web.UI.UserControl
{
    static int _ReportID = 0;
    static int _MemberBillID = 0;
    static string _VoucherHeadID = "0";
    static string _bookType = "Cash";
    static string _vchrType = "CP";
    static DateTime _FrDate;
    static DateTime _ToDate;
    static int _MemberID = 0;
    static int pAccSubTypeID = 1;
    static string pAccSubCode = "";
    static int pAccCode = 1;
    static int pCashBookCodeID = 0;
    static int pFinancialYearID = 0;
    static int pRequisitionID = 0;
    static int _VoucherTypeID = 0;
    static int pLedgerID = 0;
    static int pProjectID = 0;
    static int _CashRequisitionID = 0;
    static int _Option = 4;
    static int pGregisterID = 0;
    static string fileName;
    static string PdfLocation;
    static int pMemberID = 0;
    static int pPayTypeID = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Request.QueryString["_ReportID"] != null)
            {
                _ReportID = Convert.ToInt32(Request.QueryString["_ReportID"].ToString());
            }
            if (_ReportID == 1)
            {
                #region [Member Bill]
                if (Request.QueryString["_MemberBillID"] != null)
                {
                    _MemberBillID = Convert.ToInt32(Request.QueryString["_MemberBillID"].ToString());
                }
                if (Request.QueryString["_MemberID"] != null)
                {
                    _MemberID = Convert.ToInt32(Request.QueryString["_MemberID"].ToString());
                }
                ShowReportMoneyReceipt(_MemberBillID, _MemberID);
                #endregion
            }
            if (_ReportID == 2)
            {
                #region [Member Game Register Bill]
                if (Request.QueryString["_GregisterID"] != null)
                {
                    pGregisterID = Convert.ToInt32(Request.QueryString["_GregisterID"].ToString());
                }
                //if (Request.QueryString["_MemberID"] != null)
                //{
                //    _MemberID = Convert.ToInt32(Request.QueryString["_MemberID"].ToString());
                //}
                ShowReportGRegisterMR(pGregisterID, _MemberID);
                #endregion
            }
            if (_ReportID == 3)
            {
                #region [Member Game Register Bill]
                if (Request.QueryString["_MemberID"] != null)
                {
                    pMemberID = Convert.ToInt32(Request.QueryString["_MemberID"].ToString());
                }
                if (Request.QueryString["BookType"] != null)
                {
                    _bookType = Convert.ToString(Request.QueryString["BookType"].ToString());
                }
                if (Request.QueryString["_PayTypeID"] != null)
                {
                    pPayTypeID = Convert.ToInt32(Request.QueryString["_PayTypeID"].ToString());
                }
                if (Request.QueryString["_FrYear"] != null)
                {
                    int _FrYear = Convert.ToInt32(Request.QueryString["_FrYear"].ToString());
                    int _FrMonth = Convert.ToInt32(Request.QueryString["_FrMonth"].ToString());
                    int _FrDay = Convert.ToInt32(Request.QueryString["_FrDay"].ToString());
                    _FrDate = Convert.ToDateTime(_FrMonth.ToString() + "/" + _FrDay.ToString() + "/" + _FrYear.ToString() + " 00:00:00");
                }
                if (Request.QueryString["_ToYear"] != null)
                {
                    int _ToYear = Convert.ToInt32(Request.QueryString["_ToYear"].ToString());
                    int _ToMonth = Convert.ToInt32(Request.QueryString["_ToMonth"].ToString());
                    int _ToDay = Convert.ToInt32(Request.QueryString["_ToDay"].ToString());
                    int _ToHour = 0;
                    int _ToMinute = 0;
                    if (Request.QueryString["_ToHour"] != null)
                    {
                        _ToHour = Convert.ToInt32(Request.QueryString["_ToHour"].ToString());
                    }
                    if (Request.QueryString["_ToMinute"] != null)
                    {
                        _ToMinute = Convert.ToInt32(Request.QueryString["_ToMinute"].ToString());
                    }
                    if (_ToHour > 0)
                    {
                        _ToDate = Convert.ToDateTime(_ToMonth.ToString() + "/" + _ToDay.ToString() + "/" + _ToYear.ToString() + " " + _ToHour.ToString() + ":" + _ToMinute.ToString() + ":59");
                    }
                    else
                    {
                        _ToDate = Convert.ToDateTime(_ToMonth.ToString() + "/" + _ToDay.ToString() + "/" + _ToYear.ToString() + " 23:59:59");
                    }
                }
                if (_bookType=="DailyRegPayment")
                {
                    ShowReportDailyRegistrationStatement(pMemberID, pPayTypeID, _FrDate, _ToDate);
                }
                if (_bookType == "DailyRegCaddiePayment")
                {
                    ShowReportDailyRegCaddieStatement(pMemberID, _FrDate, _ToDate);
                }
                if (_bookType == "DailyRegCaddieSubsidyPayment")
                {
                    ShowReportDailyRegCaddieSubsidyStatement(pMemberID, _FrDate, _ToDate);
                }
                if (_bookType == "DueStatement")
                {
                    ShowReportDailyDueStatement(pMemberID, _FrDate, _ToDate);
                }
                #endregion
            }
        }
    }
    private void ShowReportDailyDueStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        ReportViewer2.Reset();
        DataTable dt = GetDailyDueStatement(pMemberID, frDate, toDate);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "DailyDueStatement.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "DailyDueStatement" + DateTime.Now.ToFileTime() + ".pdf";


        bytes = ReportViewer2.LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
        bool IsExitsPDF = File.Exists(PDFPath + fileName);

        FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        byte[] data = new byte[fs.Length];
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        PdfLocation = PDFPath + fileName;

        //report.Attributes.Add("src", "../PDF/" + fileName);
        report.Attributes.Add("src", "../PDF/" + fileName);
        //return_BalancheRptForm.Visible = true;
    }
    private void ShowReportDailyRegCaddieSubsidyStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        ReportViewer2.Reset();
        DataTable dt = GetDailyRegCaddieSubsidyStatement(pMemberID, frDate, toDate);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "DailyRegCaddieBallBoySubsidyStatement.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "DailyRegCaddieBallBoySubsidyStatement" + DateTime.Now.ToFileTime() + ".pdf";


        bytes = ReportViewer2.LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
        bool IsExitsPDF = File.Exists(PDFPath + fileName);

        FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        byte[] data = new byte[fs.Length];
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        PdfLocation = PDFPath + fileName;

        //report.Attributes.Add("src", "../PDF/" + fileName);
        report.Attributes.Add("src", "../PDF/" + fileName);
        //return_BalancheRptForm.Visible = true;
    }
    private void ShowReportDailyRegCaddieStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        ReportViewer2.Reset();
        DataTable dt = GetDailyRegCaddieStatement(pMemberID, frDate, toDate);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "DailyRegCaddieBallBoyStatement.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "DailyRegCaddieBallBoyStatement" + DateTime.Now.ToFileTime() + ".pdf";


        bytes = ReportViewer2.LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
        bool IsExitsPDF = File.Exists(PDFPath + fileName);

        FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        byte[] data = new byte[fs.Length];
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        PdfLocation = PDFPath + fileName;

        //report.Attributes.Add("src", "../PDF/" + fileName);
        report.Attributes.Add("src", "../PDF/" + fileName);
        //return_BalancheRptForm.Visible = true;
    }
    private void ShowReportDailyRegistrationStatement(int pMemberID, int pPayTypeID, DateTime frDate, DateTime toDate)
    {
        ReportViewer2.Reset();
        DataTable dt = GetDailyRegistrationStatement(pMemberID,pPayTypeID, frDate, toDate);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "DailyRegistrationStatement.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "DailyRegistrationStatement" + DateTime.Now.ToFileTime() + ".pdf";


        bytes = ReportViewer2.LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
        bool IsExitsPDF = File.Exists(PDFPath + fileName);

        FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        byte[] data = new byte[fs.Length];
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        PdfLocation = PDFPath + fileName;

        //report.Attributes.Add("src", "../PDF/" + fileName);
        report.Attributes.Add("src", "../PDF/" + fileName);
        //return_BalancheRptForm.Visible = true;
    }
    private void ShowReportMoneyReceipt(int pMemberBillID,int pMemberID)
    {
        ReportViewer2.Reset();
        DataTable dt = getMonthlyMR(pMemberBillID,pMemberID);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MonthlyMR.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "MonthlyMR" + DateTime.Now.ToFileTime() + ".pdf";


        bytes = ReportViewer2.LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
        bool IsExitsPDF = File.Exists(PDFPath + fileName);

        FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        byte[] data = new byte[fs.Length];
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        PdfLocation = PDFPath + fileName;

        //report.Attributes.Add("src", "../PDF/" + fileName);
        report.Attributes.Add("src", "../PDF/" + fileName);
        //return_BalancheRptForm.Visible = true;
    }
    private void ShowReportGRegisterMR(int pGRegisterID, int pMemberID)
    {
        ReportViewer2.Reset();
        DataTable dt = getMonthlyGameRegMR(pGRegisterID, pMemberID);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "GameRegisterMR.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "GameRegisterMR" + DateTime.Now.ToFileTime() + ".pdf";


        bytes = ReportViewer2.LocalReport.Render("PDF", "", out mimeType, out encoding, out extension, out streamids, out warnings);
        bool IsExitsPDF = File.Exists(PDFPath + fileName);

        FileStream fs = new FileStream(PDFPath + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        byte[] data = new byte[fs.Length];
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        PdfLocation = PDFPath + fileName;

        //report.Attributes.Add("src", "../PDF/" + fileName);
        report.Attributes.Add("src", "../PDF/" + fileName);
        //return_BalancheRptForm.Visible = true;
    }

    private DataTable getMonthlyMR(int pMemberBillID, int pMemberID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_MonthlyBill_VoucherPrint", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberBillID", SqlDbType.Int).Value = pMemberBillID;
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
    private DataTable getMonthlyGameRegMR(int pMemberBillID, int pMemberID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_GameRegister_VoucherPrint", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@GRegisterID", SqlDbType.Int).Value = pMemberBillID;
            //cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
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

    private DataTable GetDailyRegistrationStatement(int pMemberID, int pPayTypeID, DateTime frDate, DateTime toDate)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_DailyRegistrationStatement", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            cmd.Parameters.Add("@FrDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate;
            cmd.Parameters.Add("@PayTypeID", SqlDbType.Int).Value = pPayTypeID;
            //cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
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

    private DataTable GetDailyRegCaddieStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_CaddieBallBoyFeeReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            cmd.Parameters.Add("@GFromDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@GToDate", SqlDbType.DateTime).Value = toDate;
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


    private DataTable GetDailyRegCaddieSubsidyStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_CaddieBallBoySubsidyFeeReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            cmd.Parameters.Add("@GFromDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@GToDate", SqlDbType.DateTime).Value = toDate;
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

    private DataTable GetDailyDueStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_Member_DueStatement", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            cmd.Parameters.Add("@GFromDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@GToDate", SqlDbType.DateTime).Value = toDate;
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
