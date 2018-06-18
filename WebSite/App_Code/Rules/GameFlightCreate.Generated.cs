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
	public partial class GameFlightCreateBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("GameFlightCreate", RowKind.New)]
        public void BuildNewGameFlightCreate()
        {
            UpdateFieldValue("GameDate", DateTime.Now.Date);
            UpdateFieldValue("FlightStart", DateTime.Now);
            UpdateFieldValue("FlightEnd", DateTime.Now);
            UpdateFieldValue("IntervalInMinute", 10);
            UpdateFieldValue("MaxPlayer", 4);
        }
    }
}
