using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;

namespace MyCompany.Rules
{
	public partial class MemberCardTransactionReportBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberCardTransactionReport", RowKind.New)]
        public void BuildNewMemberCardTransactionReport()
        {
            UpdateFieldValue("PayTypeID", 1);
            UpdateFieldValue("TranDate", DateTime.Now);
            UpdateFieldValue("CreatedDt", DateTime.Now);
        }
    }
}
