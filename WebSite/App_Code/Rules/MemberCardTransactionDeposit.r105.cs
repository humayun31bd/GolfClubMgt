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
	public partial class MemberCardTransactionDepositBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view after an action
        /// with a command name that matches "Insert".
        /// </summary>
        [Rule("r105")]
        public void r105Implementation(MemberCardTransactionDepositModel instance)
        {
            // This is the placeholder for method implementation.
            if (instance.MemberID > 0)
            {
                clsGeneralLib oclsGeneralLib = new clsGeneralLib();
                string pMessage = " Received with Thanks. " + instance.DepositAmount.ToString() + " Taka";

                ////string SendMsg = "Received with thanks.";
                clsGeneralLib.Member oMem = oclsGeneralLib.getMemberByID(instance.MemberID.Value);

                int iRet = oclsGeneralLib.SendSMSTo(oMem.MobileNo, oMem.MemberName, pMessage);
                ///Result.ShowAlert("SMS Send successfully.");
                ////Result.Continue();
            }



        }
    }
}
