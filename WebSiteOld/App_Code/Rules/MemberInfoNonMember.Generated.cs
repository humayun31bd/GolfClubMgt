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
	public partial class MemberInfoNonMemberBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberInfoNonMember", RowKind.New)]
        public void BuildNewMemberInfoNonMember()
        {
            UpdateFieldValue("MemberTypeID", 2);
        }
    }
}
