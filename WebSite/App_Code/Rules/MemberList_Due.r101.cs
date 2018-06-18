﻿using System;
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
	public partial class MemberList_DueBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Report" and argument that matches "PrintDueStatement".
        /// </summary>
        [Rule("r101")]
        public void r101Implementation(MemberList_DueModel instance, int Parameters_MemberID, DateTime Parameters_FrDate, DateTime Parameters_ToDate)
        {
            if ((Parameters_FrDate.Date < DateTime.Now.Date) && (Parameters_FrDate.Date != Parameters_ToDate.Date))
            {
                Parameters_FrDate = Parameters_FrDate.AddDays(1);
                Parameters_ToDate = Parameters_ToDate.AddDays(1);
            }
            if ((Parameters_FrDate.Date == DateTime.Now.Date) || (Parameters_FrDate.Date == Parameters_ToDate.Date))
            {
                if (Parameters_FrDate.Date == Parameters_ToDate.Date)
                {
                    Parameters_FrDate = Parameters_FrDate.AddDays(1);
                    Parameters_ToDate = Parameters_ToDate.AddDays(1);
                }
            }
            if ((Parameters_ToDate.Date == DateTime.Now.Date))
            {
                Parameters_ToDate = Parameters_ToDate.AddDays(1);
            }
            if ((Parameters_FrDate.Date > DateTime.Now.Date))
            {
                Parameters_FrDate = Parameters_FrDate.AddDays(-1);
            }
            if ((Parameters_ToDate.Date > DateTime.Now.Date))
            {
                Parameters_ToDate = Parameters_ToDate.AddDays(-1);
            }
            // This is the placeholder for method implementation.
            //Result.NavigateUrl = String.Format("~/Pages/LedgerBook.aspx?&Cashbookfilter",instance.AccFundID,instance.);
            string mySQL = String.Format("Pages/ClubReport.aspx?_ReportID=3&_MemberID=" + Parameters_MemberID.ToString() + "&_FrYear=" + Parameters_FrDate.Year.ToString() + "&_FrMonth=" + Parameters_FrDate.Month.ToString() + "&_FrDay=" + Parameters_FrDate.Day.ToString() + "&_ToYear=" + Parameters_ToDate.Year.ToString() + "&_ToMonth=" + Parameters_ToDate.Month.ToString() + "&_ToDay=" + Parameters_ToDate.Day.ToString() + "&BookType=DueStatement");
            Result.NavigateUrl = mySQL;
        }
    }
}
