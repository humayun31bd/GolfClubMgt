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
	public partial class MemberInfoSMSBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberInfoSMS", RowKind.New)]
        public void BuildNewMemberInfoSMS()
        {
            UpdateFieldValue("MemberTypeID", 1);
        }
    }
}
