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
	public partial class MemberInfoViewBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberInfoView", RowKind.New)]
        public void BuildNewMemberInfoView()
        {
            UpdateFieldValue("MemberTypeID", 1);
        }
    }
}
