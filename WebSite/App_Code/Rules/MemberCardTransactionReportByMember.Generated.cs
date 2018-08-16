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
	public partial class MemberCardTransactionReportByMemberBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberCardTransactionReportByMember", RowKind.New)]
        public void BuildNewMemberCardTransactionReportByMember()
        {
            UpdateFieldValue("PayTypeID", 1);
            UpdateFieldValue("TranDate", DateTime.Now);
            UpdateFieldValue("CreatedDt", DateTime.Now);
        }
    }
}
