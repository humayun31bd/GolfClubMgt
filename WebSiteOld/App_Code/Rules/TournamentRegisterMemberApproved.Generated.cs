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
	public partial class TournamentRegisterMemberApprovedBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("TournamentRegisterMemberApproved", RowKind.New)]
        public void BuildNewTournamentRegisterMemberApproved()
        {
            UpdateFieldValue("IsApproved", 1);
        }
    }
}
