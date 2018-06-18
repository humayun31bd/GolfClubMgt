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
	public partial class MemberBill1BusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberBill1", RowKind.New)]
        public void BuildNewMemberBill1()
        {
            UpdateFieldValue("BillDate", DateTime.Now);
            UpdateFieldValue("CreatedDate", DateTime.Now);
            UpdateFieldValue("PayTypeID", 1);
        }
    }
}
