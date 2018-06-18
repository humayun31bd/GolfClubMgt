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
	public partial class LockerBillFilterBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("LockerBillFilter", RowKind.New)]
        public void BuildNewLockerBillFilter()
        {
            UpdateFieldValue("FromDate", DateTime.Now);
            UpdateFieldValue("ToDate", DateTime.Now);
        }
    }
}
