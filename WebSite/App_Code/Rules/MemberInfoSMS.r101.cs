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
	public partial class MemberInfoSMSBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Report" and argument that matches "CustomSMS".
        /// </summary>
        [Rule("r101")]
        public void r101Implementation(MemberInfoSMSModel instance)
        {
            string Parameters_SMSBODY = "Send sms";
            /*
            // This is the placeholder for method implementation.
            clsGeneralLib oclsGeneralLib = new clsGeneralLib();
            int memid = Convert.ToInt32(instance.MemberID);
            clsGeneralLib.Member omem = new clsGeneralLib.Member();
            omem = oclsGeneralLib.getMemberByID(memid);
            oclsGeneralLib.SendSMSTo(omem.MobileNo, omem.MemberName, Parameters_SMSBODY);
            //Result.ShowAlert("Successfully send sms to Member");
            //Result.Continue();
            */

            clsGeneralLib oclsGeneralLib = new clsGeneralLib();
            int sendonly = 1;
            // This is the placeholder for method implementation.
            //int count = instance.MemberID.Value;
            if (Arguments.SelectedValues.Count() > 0)
            {
                //MemberInfoModel oMemberInfoModel = new MemberInfoModel();
                foreach (string MemberID in ActionArgs.Current.SelectedValues)
                {

                    int memid = Convert.ToInt32(MemberID);
                    clsGeneralLib.Member omem = new clsGeneralLib.Member();
                    if (sendonly == 1)
                    {
                        omem = oclsGeneralLib.getMemberByID(memid);
                        oclsGeneralLib.SendSMSTo(omem.MobileNo, omem.MemberName, Parameters_SMSBODY);
                        sendonly = 0;
                        Result.ShowAlert("Successfully send sms to Member");
                        this.PreventDefault();
                    }
                }
                //string value = string.Join(",", ActionArgs.Current.SelectedValues);
                /// Result.NavigateUrl = String.Format("~/Pages/StudentPassword.aspx?ClassStudent={0}", value, false);

            }
            else
            {
                Result.ShowAlert("Please Select Member");
                this.PreventDefault();
                Result.NavigateUrl = String.Format("~/Pages/MemberInfo.aspx");
            }
        
        }
    }
}
