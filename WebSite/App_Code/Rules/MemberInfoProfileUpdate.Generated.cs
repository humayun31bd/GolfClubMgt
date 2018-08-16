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
	public partial class MemberInfoProfileUpdateBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberInfoProfileUpdate", RowKind.New)]
        public void BuildNewMemberInfoProfileUpdate()
        {
            UpdateFieldValue("MemberTypeID", 1);
        }
    }
}
