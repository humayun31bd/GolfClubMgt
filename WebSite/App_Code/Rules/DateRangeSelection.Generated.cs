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
	public partial class DateRangeSelectionBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("DateRangeSelection", RowKind.New)]
        public void BuildNewDateRangeSelection()
        {
            UpdateFieldValue("FromDate", DateTime.Now.Date);
            UpdateFieldValue("ToDate", DateTime.Now.Date);
        }
    }
}
