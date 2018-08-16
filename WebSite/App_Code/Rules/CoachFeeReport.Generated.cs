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
	public partial class CoachFeeReportBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("CoachFeeReport", RowKind.New)]
        public void BuildNewCoachFeeReport()
        {
            UpdateFieldValue("Mrdate", DateTime.Now);
        }
    }
}
