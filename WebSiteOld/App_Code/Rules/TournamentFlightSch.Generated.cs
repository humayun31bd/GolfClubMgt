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
	public partial class TournamentFlightSchBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("TournamentFlightSch", RowKind.New)]
        public void BuildNewTournamentFlightSch()
        {
            UpdateFieldValue("Date", DateTime.Now.Date);
            UpdateFieldValue("Delay", 10);
            UpdateFieldValue("NumberofPlay", 4);
        }
    }
}
