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
        /// with a command name that matches "Report" and argument that matches "SendSMSToMember".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(MemberSendSMSModel instance, string Parameters_SMSBODY)
        {
            // This is the placeholder for method implementation.



        }
    }
}
