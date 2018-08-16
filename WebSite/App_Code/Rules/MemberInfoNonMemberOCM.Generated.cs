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
	public partial class MemberInfoNonMemberOCMBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberInfoNonMemberOCM", RowKind.New)]
        public void BuildNewMemberInfoNonMemberOCM()
        {
            UpdateFieldValue("MemberTypeID", 2);
            UpdateFieldValue("NonMemberCatID", 2);
            UpdateFieldValue("OcmnoOfDaysPlay", 15);
        }
    }
}
