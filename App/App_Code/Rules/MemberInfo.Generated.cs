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
	public partial class MemberInfoBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberInfo", RowKind.New)]
        public void BuildNewMemberInfo()
        {
            UpdateFieldValue("MemberTypeID", 1);
        }
    }
}
