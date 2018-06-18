using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;

namespace MyCompany.Rules
{
	public partial class GameFlightScheduleBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "CreateGameSchedule".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(GameFlightScheduleModel instance, DateTime @Parameters_FlightStart,DateTime @Parameters_FlightEnd, int @Parameters_IntervalInMinute)
        {
            // This is the placeholder for method implementation.


        }
    }
}
