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
	public partial class TourFlightCreateBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("TourFlightCreate", RowKind.New)]
        public void BuildNewTourFlightCreate()
        {
            UpdateFieldValue("FlightStart", DateTime.Now);
            UpdateFieldValue("FlightEnd", DateTime.Now);
            UpdateFieldValue("IntervalInMinute", 10);
        }
    }
}
