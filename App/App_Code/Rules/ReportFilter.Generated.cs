﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;

namespace MyCompany.Rules
{
	public partial class ReportFilterBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("ReportFilter", RowKind.New)]
        public void BuildNewReportFilter()
        {
            UpdateFieldValue("MemberID", 0);
            UpdateFieldValue("FromDate", DateTime.Now);
            UpdateFieldValue("ToDate", DateTime.Now);
            UpdateFieldValue("PaymentTypeID", 1);
        }
    }
}
