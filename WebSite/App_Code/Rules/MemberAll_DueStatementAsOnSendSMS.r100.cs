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
	public partial class MemberAll_DueStatementAsOnSendSMSBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "DueSendSMS".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(MemberAll_DueStatementAsOnSendSMSModel instance)
        {
            string Parameters_SMSBODY = "";

            // This is the placeholder for method implementation.

            clsGeneralLib oclsGeneralLib = new clsGeneralLib();
            int sendonly = 1;

            Parameters_SMSBODY = oclsGeneralLib.GetDueSMSMessage();

            // This is the placeholder for method implementation.
            //int count = instance.MemberID.Value;
            if (Arguments.SelectedValues.Count() > 0)
            {
                //MemberInfoModel oMemberInfoModel = new MemberInfoModel();

                int memid = Convert.ToInt32(Arguments.SelectedValues[0].ToString());
                clsGeneralLib.Member omem = new clsGeneralLib.Member();
                if (sendonly == 1)
                {
                    omem = oclsGeneralLib.getMemberByID(memid);
                    if (!string.IsNullOrEmpty(Parameters_SMSBODY))
                    {
                        oclsGeneralLib.SendSMSTo(omem.MobileNo, omem.MemberName, Parameters_SMSBODY);
                        sendonly = 0;
                        Result.ShowAlert("Successfully send sms to Member");
                    }
                    else
                    {
                        Result.ShowAlert("Please set your sms text in sms config Memnu. ");
                        this.PreventDefault();
                    }
                    
                }
            }
            else
            {
                Result.ShowAlert("Please Select Member");
                this.PreventDefault();
                ///Result.NavigateUrl = String.Format("~/Pages/MemberInfo.aspx");
            }

        }
    }
}
