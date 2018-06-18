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
	public partial class MemberSubsFilterBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberSubsFilter", RowKind.New)]
        public void BuildNewMemberSubsFilter()
        {
            UpdateFieldValue("SubsiEndDate", DateTime.Now);
        }
    }
}
