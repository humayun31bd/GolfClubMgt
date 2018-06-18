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
	public partial class GameFlightSchedule18HoleBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("GameFlightSchedule18Hole", RowKind.New)]
        public void BuildNewGameFlightSchedule18Hole()
        {
            UpdateFieldValue("HoleTypeID", 2);
        }
    }
}
