﻿using System;
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
	public partial class MemberCardDepositReportBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view after an action
        /// with a command name that matches "Insert|Update".
        /// </summary>
        [Rule("r101")]
        public void r101Implementation(MemberCardDepositReportModel instance)
        {
            // This is the placeholder for method implementation.
        }
    }
}
