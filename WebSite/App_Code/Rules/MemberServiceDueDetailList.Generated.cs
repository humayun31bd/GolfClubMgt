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
	public partial class MemberServiceDueDetailListBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberServiceDueDetailList", RowKind.New)]
        public void BuildNewMemberServiceDueDetailList()
        {
            UpdateFieldValue("ServiceDate", DateTime.Now);
        }
    }
}
