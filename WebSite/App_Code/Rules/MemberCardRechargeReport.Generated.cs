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
	public partial class MemberCardRechargeReportBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberCardRechargeReport", RowKind.New)]
        public void BuildNewMemberCardRechargeReport()
        {
            UpdateFieldValue("PayTypeID", 1);
            UpdateFieldValue("TranDate", DateTime.Now);
            UpdateFieldValue("CreatedDt", DateTime.Now);
        }
    }
}
