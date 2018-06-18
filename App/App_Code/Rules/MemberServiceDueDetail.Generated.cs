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
	public partial class MemberServiceDueDetailBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberServiceDueDetail", RowKind.New)]
        public void BuildNewMemberServiceDueDetail()
        {
            UpdateFieldValue("ServiceDate", DateTime.Now);
        }
    }
}
