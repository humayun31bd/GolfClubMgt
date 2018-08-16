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
//    static string _VoucherHeadID = "0";
    static string _bookType = "Cash";
//    static string _vchrType = "CP";
    static DateTime _FrDate;
    static DateTime _ToDate;
    static int _MemberID = 0;
    static string _MemberCode = "";
    static int pMemberCardTranID = 0;
    //static int pAccSubTypeID = 1;
//    static string pAccSubCode = "";
//    static int pAccCode = 1;
//    static int pCashBookCodeID = 0;
//    static int pFinancialYearID = 0;
//    static int pRequisitionID = 0;
//    static int _VoucherTypeID = 0;
//    static int pLedgerID = 0;
//    static int pProjectID = 0;
    static string pMemberCode;
    static int pMemberCaregoryID;
    static int pGregisterID = 0;
    static string fileName;
    static string PdfLocation;
    static int pMemberID = 0;
    static int pPayTypeID = 0;
    static int pCoachFeeID = 0;


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
                if (Request.QueryString["_MemberCode"] != null)
                {
                    _MemberCode = Convert.ToString(Request.QueryString["_MemberCode"].ToString());
                }
                ShowReportMoneyReceipt(_MemberBillID, _MemberCode);
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

                if (Request.QueryString["_MemberCode"] != null)
                {
                    pMemberCode = Convert.ToString(Request.QueryString["_MemberCode"].ToString());
                }
                if (Request.QueryString["_MemberCaregoryID"] != null)
                {
                    pMemberCaregoryID = Convert.ToInt32(Request.QueryString["_MemberCaregoryID"].ToString());
                }


                if (_bookType == "DailyCoachFee")
                {
                    ShowReportDailyCoachFeeStatement(_FrDate, _ToDate);
                }
                if (_bookType == "DailyRegPayment")
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
                    ShowReportDailyDueStatement(pMemberCaregoryID, pMemberCode);
                }
                if (_bookType == "BillCollectionStatement")
                {
                    ShowReportBillCollectionStatement(pMemberID, _FrDate, _ToDate, pPayTypeID);
                }
                if (_bookType == "BillCollectionSummary")
                {
                    ShowReportBillCollectionSummaryStatement(pMemberID, _FrDate, _ToDate, pPayTypeID);
                }
                #endregion
            }
            if (_ReportID == 4)
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
                ShowReportGRegisterExtraMR(pGregisterID, _MemberID);
                #endregion
            }
            if (_ReportID == 5)
            {
                #region [Member CardTransaction Statement]
                if (Request.QueryString["_MemberID"] != null)
                {
                    pMemberID = Convert.ToInt32(Request.QueryString["_MemberID"].ToString());
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
                ShowReportMemberCardStatement(pMemberID, _FrDate, _ToDate);
                #endregion
            }
            if (_ReportID == 6)
            {
                #region [Member CardTransaction MR]
                if (Request.QueryString["_MemberCardTranID"] != null)
                {
                    pMemberCardTranID = Convert.ToInt32(Request.QueryString["_MemberCardTranID"].ToString());
                }
                ShowReportMemberCardMR(pMemberCardTranID);
                #endregion
            }
            if (_ReportID == 7)
            {
                #region [Member Pro Coachfee Print]
                if (Request.QueryString["_CoachFeeID"] != null)
                {
                    pCoachFeeID = Convert.ToInt32(Request.QueryString["_CoachFeeID"].ToString());
                }
                ShowReportProCoachfeeMR(pCoachFeeID);
                #endregion
            }
        }
    }

    private void ShowReportDailyCoachFeeStatement(DateTime frDate, DateTime toDate)
    {
        ReportViewer2.Reset();
        DataTable dt = DailyCoachFeeStatement( frDate, toDate);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "CoachFeeStatement.rdlc";
        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "CoachFeeStatement" + DateTime.Now.ToFileTime() + ".pdf";


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

    private void ShowReportProCoachfeeMR(int pCoachFeeID)
    {
        ReportViewer2.Reset();
        DataTable dt = CoachFeeMR_Print(pCoachFeeID);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "CoachFeeMR.rdlc";
        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "MemberCardTransactionMR" + DateTime.Now.ToFileTime() + ".pdf";


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
    private void ShowReportMemberCardMR(int pMemberCardTranID)
    {
        ReportViewer2.Reset();
        DataTable dt = CardTransactionMR_Print(pMemberCardTranID);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCardTransactionMR.rdlc";
        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "MemberCardTransactionMR" + DateTime.Now.ToFileTime() + ".pdf";


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
    private void ShowReportMemberCardStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        ReportViewer2.Reset();
        DataTable dt = GetMemberCardStatement(pMemberID, frDate, toDate);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCardStatementReport.rdlc";
        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "MemberCardStatementReport" + DateTime.Now.ToFileTime() + ".pdf";


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
    private void ShowReportBillCollectionSummaryStatement(int pMemberID, DateTime frDate, DateTime toDate, int pPayTypeID)
    {
        ReportViewer2.Reset();
        DataTable dt = GetMember_CollectionSummary(pMemberID, frDate, toDate, pPayTypeID);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        if (pPayTypeID == 0)
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCollectionSummaryReport.rdlc";
        }
        if (pPayTypeID == 1)
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCollectionSummaryCashReport.rdlc";
        }
        if (pPayTypeID == 2)
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCollectionSummaryChequeReport.rdlc";
        }
        if (pPayTypeID == 3)
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCollectionSummaryMemberCardReport.rdlc";
        }
        if (pPayTypeID == 4)
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCollectionSummaryBankCardReport.rdlc";
        }
        if (pPayTypeID == 5)
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCollectionSummaryDueReport.rdlc";
        }
        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "MemberCollectionSummaryReport" + DateTime.Now.ToFileTime() + ".pdf";


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
    private void ShowReportBillCollectionStatement(int pMemberID, DateTime frDate, DateTime toDate, int pPayTypeID)
    {
        ReportViewer2.Reset();
        DataTable dt = GetMember_CollectionStatement(pMemberID, frDate, toDate, pPayTypeID);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberCollectionReport.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "MemberCollectionReport" + DateTime.Now.ToFileTime() + ".pdf";


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
    private void ShowReportDailyDueStatement(int pMemberCaregoryID, string pMemberCode)
    {
        ReportViewer2.Reset();
        DataTable dt = GetDailyDueStatement( pMemberCaregoryID, pMemberCode);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;

        if (!string.IsNullOrEmpty(pMemberCode))
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "MemberAllDueStatement.rdlc";
        }
        else
        {
            ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "DailyDueStatement.rdlc";
        }
        

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        if (!string.IsNullOrEmpty(pMemberCode))
        {
            fileName = "MemberDueStatement" + DateTime.Now.ToFileTime() + ".pdf";
        }
        else
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
        ReportParameter[] parameter = new ReportParameter[]
        {
           new ReportParameter("FromDate", frDate.ToString()),
           new ReportParameter("ToDate", toDate.ToString())
        };
        ReportViewer2.LocalReport.SetParameters(parameter);
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
    private void ShowReportMoneyReceipt(int pMemberBillID,string pMemberCode)
    {
        ReportViewer2.Reset();
        
        int iret = GetMemberByID(pMemberCode);


        DataTable dt = getMonthlyMR(pMemberBillID,_MemberID);
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

    private void ShowReportGRegisterExtraMR(int pGRegisterID, int pMemberID)
    {
        ReportViewer2.Reset();
        DataTable dt = getMonthlyGameRegExtraMR(pGRegisterID, pMemberID);
        ReportDataSource rds = new ReportDataSource("DataSet1", dt);
        ReportViewer2.LocalReport.DataSources.Add(rds);
        this.ReportViewer2.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        ReportViewer2.LocalReport.ReportPath = Server.MapPath("/Controls/") + "GameRegisterMRExtra.rdlc";

        ReportViewer2.LocalReport.Refresh();

        string PDFPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")) + "PDF\\";
        Microsoft.Reporting.WebForms.Warning[] warnings = null;
        string[] streamids = null;
        String mimeType = null;
        String encoding = null;
        String extension = null;
        Byte[] bytes = null;


        fileName = "GameRegisterMRExtra" + DateTime.Now.ToFileTime() + ".pdf";


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

    private DataTable DailyCoachFeeStatement(DateTime frDate, DateTime toDate)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_CoachFeePrint", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@TranDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@TranToDate", SqlDbType.DateTime).Value = toDate;
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

    private DataTable getMonthlyGameRegExtraMR(int pMemberBillID, int pMemberID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_GameRegisterExtra_VoucherPrint", con);
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

    private DataTable GetMemberCardStatement(int pMemberID, DateTime frDate, DateTime toDate)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_MemberCardTransaction_Print", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate;
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
    private DataTable CardTransactionMR_Print(int pMemberCardTranID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_CardTransactionMR_Print", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberCardTranID", SqlDbType.Int).Value = pMemberCardTranID;
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

    private DataTable CoachFeeMR_Print(int pCoachFeeID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_CoachFeeMR_Print", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@CoachFeeID", SqlDbType.Int).Value = pCoachFeeID;
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

    private DataTable GetDailyDueStatement(int pMemberCaregoryID, string pMemberCode)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_Member_DueStatement", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberCaregoryID", SqlDbType.Int).Value = pMemberCaregoryID;
            cmd.Parameters.Add("@MemberCode", SqlDbType.NVarChar, 50).Value = pMemberCode;
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


    private DataTable GetMember_CollectionStatement(int pMemberID, DateTime frDate, DateTime toDate, int pPayTypeID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_Member_CollectionStatement", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            cmd.Parameters.Add("@FrDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate;
            cmd.Parameters.Add("@PayTypeID", SqlDbType.Int).Value = pPayTypeID;
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
    private DataTable GetMember_CollectionSummary(int pMemberID, DateTime frDate, DateTime toDate, int pPayTypeID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_Member_CollectionSummary", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            cmd.Parameters.Add("@FrDate", SqlDbType.DateTime).Value = frDate;
            cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate;
            cmd.Parameters.Add("@PayTypeID", SqlDbType.Int).Value = pPayTypeID;
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
    private int GetMemberByID(string pMemCode)
    {
        DataTable dt = new DataTable();
        string CONN_STRING = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(CONN_STRING);
        {
            //
            string sSQL = " ";
            sSQL = sSQL + " select  M.MemberID from dbo.MemberInfo M ";

            if (!string.IsNullOrEmpty(pMemCode.ToString()))
            {
                sSQL = sSQL + " where M.MemberCode = '" + pMemCode + "'";
            }

            SqlCommand cmd = new SqlCommand(sSQL, con);

            cmd.CommandType = CommandType.Text;
            //cmd.Parameters.Add("@VoucherHeadID", SqlDbType.NVarChar).Value = pVoucherHeadID;
            SqlDataAdapter adpt = new SqlDataAdapter(cmd);
            try
            {

                adpt.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow rw in dt.Rows)
                    {
                        if (rw["MemberID"] != DBNull.Value)
                        {
                            _MemberID = Convert.ToInt32(rw["MemberID"].ToString());
                        }
                    }
                }
            }
            catch
            {
            }
        }
        return _MemberID;
    }

}
