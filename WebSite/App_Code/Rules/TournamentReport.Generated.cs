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
	public partial class TournamentReportBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("TournamentReport", RowKind.New)]
        public void BuildNewTournamentReport()
        {
            UpdateFieldValue("rptOption", 1);
            UpdateFieldValue("FromDate", DateTime.Now);
            UpdateFieldValue("ToDate", DateTime.Now);
        }
    }
}
