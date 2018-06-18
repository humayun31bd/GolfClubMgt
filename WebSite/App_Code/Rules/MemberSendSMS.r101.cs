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
        /// with a command name that matches "Custom" and argument that matches "CustomSMS".
        /// </summary>
        [Rule("r101")]
        public void r101Implementation(MemberSendSMSModel instance)
        {
            // This is the placeholder for method implementation.
            string Parameters_SMSBODY ="Send test sms";


            using (SqlText memInfo = new SqlText(@"Select PredefineSMS From [dbo].[SMSConfig]"))
            {
                try
                {
                    #region [Read Data]
                    if (memInfo.Read())
                    {
                        if (memInfo["PredefineSMS"] != DBNull.Value)
                        {
                            Parameters_SMSBODY = Convert.ToString(memInfo["PredefineSMS"].ToString());
                        }
                    }
                    #endregion
                }
                catch 
                { 
                    
                }
            }

            clsGeneralLib oclsGeneralLib = new clsGeneralLib();
            // This is the placeholder for method implementation.
            //int count = instance.MemberID.Value;

            int memid = Convert.ToInt32(instance.MemberID);
            clsGeneralLib.Member omem = new clsGeneralLib.Member();
                omem = oclsGeneralLib.getMemberByID(memid);
                oclsGeneralLib.SendSMSTo(omem.MobileNo, omem.MemberName, Parameters_SMSBODY);
                Result.ShowAlert("Successfully send sms to Member");
                this.PreventDefault();
        }
    }
}
