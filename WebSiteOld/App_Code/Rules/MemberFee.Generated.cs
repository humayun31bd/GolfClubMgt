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
	public partial class MemberFeeBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberFee", RowKind.New)]
        public void BuildNewMemberFee()
        {
            UpdateFieldValue("IsActive", 1);
        }
    }
}
