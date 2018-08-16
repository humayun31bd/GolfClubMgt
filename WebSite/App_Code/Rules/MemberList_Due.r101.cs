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
	public partial class MemberList_DueBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Report" and argument that matches "PrintDueStatement".
        /// </summary>
        [Rule("r101")]
        public void r101Implementation(MemberList_DueModel instance, int Parameters_MemberCaregoryID, string Parameters_MemberCode)
        {
            // This is the placeholder for method implementation.
            //Result.NavigateUrl = String.Format("~/Pages/LedgerBook.aspx?&Cashbookfilter",instance.AccFundID,instance.);
            string mySQL = String.Format("Pages/ClubReport.aspx?_ReportID=3&_MemberCaregoryID=" + Parameters_MemberCaregoryID.ToString() + "&_MemberCode=" + Parameters_MemberCode  + "&BookType=DueStatement");
            Result.NavigateUrl = mySQL;
        }
    }
}
