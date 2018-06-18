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
	public partial class GameFlightSchedule9HoleBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("GameFlightSchedule9Hole", RowKind.New)]
        public void BuildNewGameFlightSchedule9Hole()
        {
            UpdateFieldValue("HoleTypeID", 1);
        }
    }
}
