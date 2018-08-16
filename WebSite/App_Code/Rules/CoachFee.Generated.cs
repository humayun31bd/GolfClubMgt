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
	public partial class CoachFeeBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("CoachFee", RowKind.New)]
        public void BuildNewCoachFee()
        {
            UpdateFieldValue("Mrdate", DateTime.Now);
            UpdateFieldValue("Mtimefrom", DateTime.Now);
            UpdateFieldValue("MtimeTo", DateTime.Now);
            UpdateFieldValue("PaytypeID", 1);
        }
    }
}
