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
	public partial class MemberCategoryBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberCategory", RowKind.New)]
        public void BuildNewMemberCategory()
        {
            UpdateFieldValue("MemberTypeID", 1);
            UpdateFieldValue("MemberExpireType", 2);
            UpdateFieldValue("ExpireNumberCount", 1);
            UpdateFieldValue("MaxMemberInGroup", 1);
            UpdateFieldValue("IsMemberSpouse", 1);
            UpdateFieldValue("IsChildren", 1);
            UpdateFieldValue("ChildrenAgeLimit", 25);
            UpdateFieldValue("MaxChildCount", 5);
            UpdateFieldValue("IsActiveMember", 1);
            UpdateFieldValue("IsHandiCap", 1);
        }
    }
}
