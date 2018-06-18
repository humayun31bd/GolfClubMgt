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
	public partial class CloseBookingLockerBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("CloseBookingLocker", RowKind.New)]
        public void BuildNewCloseBookingLocker()
        {
            UpdateFieldValue("ClosingDate", DateTime.Now);
        }
    }
}
