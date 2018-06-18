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
	public partial class MemberBillApprovedBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberBillApproved", RowKind.New)]
        public void BuildNewMemberBillApproved()
        {
            UpdateFieldValue("BillDate", DateTime.Now);
            UpdateFieldValue("CreatedDate", DateTime.Now);
            UpdateFieldValue("PayTypeID", 1);
        }
    }
}
