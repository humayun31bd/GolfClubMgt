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
	public partial class GregistrationBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("Gregistration", RowKind.New)]
        public void BuildNewGregistration()
        {
            UpdateFieldValue("PlayDate", DateTime.Now.Date);
            UpdateFieldValue("HoleTypeID", 1);
            UpdateFieldValue("GolfCartHoleTypeID", 1);
            UpdateFieldValue("RegDate", DateTime.Now);
            UpdateFieldValue("CreatedDt", DateTime.Now);
            UpdateFieldValue("PayTypeID", 1);
        }
    }
}
