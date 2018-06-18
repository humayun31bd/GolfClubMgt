using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;
using System.Net.Mail;
using System.Net;

namespace MyCompany.Rules
{
	public partial class MemberInfoBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "SendSMSToMember".
        /// </summary>
        [Rule("r106")]
        public void r106Implementation(MemberInfoModel instance, string Parameters_SMSBODY)
        {
            // This is the placeholder for method implementation.

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
            //foreach (var id in instance.MemberID)
            //{ 

            //}

        }

        
    }
}
