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
	public partial class MemberSendSMSBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberSendSMS", RowKind.New)]
        public void BuildNewMemberSendSMS()
        {
            UpdateFieldValue("MemberTypeID", 1);
        }
    }
}
