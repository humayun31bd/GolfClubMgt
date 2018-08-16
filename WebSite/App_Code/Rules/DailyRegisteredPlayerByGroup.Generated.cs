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
	public partial class DailyRegisteredPlayerByGroupBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("DailyRegisteredPlayerByGroup", RowKind.New)]
        public void BuildNewDailyRegisteredPlayerByGroup()
        {
            UpdateFieldValue("TotalPlayers", 0);
        }
    }
}
