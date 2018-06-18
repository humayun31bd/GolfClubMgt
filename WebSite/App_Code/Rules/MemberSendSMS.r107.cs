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
	public partial class MemberSendSMSBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "SendEmail".
        /// </summary>
        [Rule("r107")]
        public void r107Implementation(MemberSendSMSModel instance)
        {
            // This is the placeholder for method implementation.
        }
    }
}
