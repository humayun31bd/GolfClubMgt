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
        /// with a command name that matches "Custom" and argument that matches "SendEmail".
        /// </summary>
        [Rule("r107")]
        public void r107Implementation(MemberInfoModel instance)
        {
            // This is the placeholder for method implementation.

            if (Arguments.SelectedValues.Count() > 0)
            {
                string value = string.Join(",", ActionArgs.Current.SelectedValues);
                /// Result.NavigateUrl = String.Format("~/Pages/StudentPassword.aspx?ClassStudent={0}", value, false);

            }
            MailMessage message = new MailMessage();
            message.From = new MailAddress("kabir31bd@gmail.com", "Sales, Code OnTime");

            // prepare smtp client
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; 
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("kabir31bd@gmail.com", "kabir31bd@!#");
            
            // compose and send an email message

            // use CustomerID to find the recipient's email
            string sendTo = "kaisarul@yahoo.com";
           //// message.Sender = "kabir31bd@gmail.com";
            message.To.Add(sendTo);
            message.Subject = String.Format("Hello {0} at {1}.", instance.NameOfMember, instance.CellPhone);
            message.Body = "Hello there!" + " My new Email Sender" ;
            smtp.Send(message);

            Result.ShowAlert("Email has been sent.");

        }
    }
}
