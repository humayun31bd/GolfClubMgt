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
	public partial class MemberStatusChangeBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberStatusChange", RowKind.New)]
        public void BuildNewMemberStatusChange()
        {
            UpdateFieldValue("TransferDate", DateTime.Now);
        }
    }
}
