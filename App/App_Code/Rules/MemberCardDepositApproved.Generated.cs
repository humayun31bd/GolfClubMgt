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
	public partial class MemberCardDepositApprovedBusinessRules : MyCompany.Data.BusinessRules
    {
        
        [RowBuilder("MemberCardDepositApproved", RowKind.New)]
        public void BuildNewMemberCardDepositApproved()
        {
            UpdateFieldValue("PayTypeID", 1);
            UpdateFieldValue("TranDate", DateTime.Now);
            UpdateFieldValue("CreatedDt", DateTime.Now);
        }
    }
}
