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
	public partial class TournamentFeeBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("TournamentFee", RowKind.New)]
        public void BuildNewTournamentFee()
        {
            UpdateFieldValue("IsActive", 1);
        }
    }
}
