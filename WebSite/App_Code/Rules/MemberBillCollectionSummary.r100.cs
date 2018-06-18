using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;

namespace MyCompany.Rules
{
	public partial class MemberBillCollectionSummaryBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Report" and argument that matches "PrintBillSummary".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(MemberBillCollectionSummaryModel instance, int Parameters_MemberID, int Parameters_PayTypeID, DateTime Parameters_FromDate, DateTime Parameters_ToDate)
        {
            if ((Parameters_FromDate.Date < DateTime.Now.Date) && (Parameters_FromDate.Date != Parameters_ToDate.Date))
            {
                Parameters_FromDate = Parameters_FromDate.AddDays(1);
                Parameters_ToDate = Parameters_ToDate.AddDays(1);
            }
            if ((Parameters_FromDate.Date >= DateTime.Now.Date) && (Parameters_FromDate.Date != Parameters_ToDate.Date))
            {
                Parameters_FromDate = Parameters_FromDate.AddDays(1);
                Parameters_ToDate = Parameters_ToDate.AddDays(1);
            }
            if ((Parameters_FromDate.Date == DateTime.Now.Date) || (Parameters_FromDate.Date == Parameters_ToDate.Date))
            {
                //if (Parameters_FromDate.Date == Parameters_ToDate.Date)
                //{
                //    Parameters_FromDate = Parameters_FromDate.AddDays(1);
                //    Parameters_ToDate = Parameters_ToDate.AddDays(1);
                //}
            }
            if ((Parameters_ToDate.Date == DateTime.Now.Date))
            {
                //Parameters_ToDate = Parameters_ToDate.AddDays(1);
            }
            if ((Parameters_FromDate.Date < DateTime.Now.Date) && (Parameters_FromDate.Date > DateTime.Now.Date))
            {
                Parameters_FromDate = Parameters_FromDate.AddDays(-1);
            }
            if ((Parameters_FromDate.Date < DateTime.Now.Date) && (Parameters_ToDate.Date > DateTime.Now.Date))
            {
                Parameters_ToDate = Parameters_ToDate.AddDays(-1);
            }
            // This is the placeholder for method implementation.
            //Result.NavigateUrl = String.Format("~/Pages/LedgerBook.aspx?&Cashbookfilter",instance.AccFundID,instance.);
            string mySQL = String.Format("Pages/ClubReport.aspx?_ReportID=3&_MemberID=" + Parameters_MemberID.ToString() + "&_PayTypeID=" + Parameters_PayTypeID.ToString() + "&_FrYear=" + Parameters_FromDate.Year.ToString() + "&_FrMonth=" + Parameters_FromDate.Month.ToString() + "&_FrDay=" + Parameters_FromDate.Day.ToString() + "&_ToYear=" + Parameters_ToDate.Year.ToString() + "&_ToMonth=" + Parameters_ToDate.Month.ToString() + "&_ToDay=" + Parameters_ToDate.Day.ToString() + "&BookType=BillCollectionSummary");
            Result.NavigateUrl = mySQL;

        }
    }
}
