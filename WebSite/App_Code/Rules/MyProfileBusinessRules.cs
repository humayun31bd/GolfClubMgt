using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Services;

namespace MyCompany.Rules
{
	public class MyProfileBusinessRulesBase : BusinessRules
    {
        
        public static SortedDictionary<MembershipCreateStatus, string> CreateErrors = new SortedDictionary<MembershipCreateStatus, string>();
        
        static MyProfileBusinessRulesBase()
        {
            CreateErrors.Add(MembershipCreateStatus.DuplicateEmail, "Duplicate email address.");
            CreateErrors.Add(MembershipCreateStatus.DuplicateProviderUserKey, "Duplicate provider key.");
            CreateErrors.Add(MembershipCreateStatus.DuplicateUserName, "Duplicate user name.");
            CreateErrors.Add(MembershipCreateStatus.InvalidAnswer, "Invalid password recovery answer.");
            CreateErrors.Add(MembershipCreateStatus.InvalidEmail, "Invalid email address.");
            CreateErrors.Add(MembershipCreateStatus.InvalidPassword, "Invalid password.");
            CreateErrors.Add(MembershipCreateStatus.InvalidProviderUserKey, "Invalid provider user key.");
            CreateErrors.Add(MembershipCreateStatus.InvalidQuestion, "Invalid password recovery question.");
            CreateErrors.Add(MembershipCreateStatus.InvalidUserName, "Invalid user name.");
            CreateErrors.Add(MembershipCreateStatus.ProviderError, "Provider error.");
            CreateErrors.Add(MembershipCreateStatus.UserRejected, "User has been rejected.");
        }
        
        protected virtual void InsertUser(string username, string password, string confirmPassword, string email, string passwordQuestion, string passwordAnswer, bool isApproved, string comment, string roles)
        {
            PreventDefault();
            if (password != confirmPassword)
            	throw new Exception(Localize("PasswordAndConfirmationDoNotMatch", "Password and confirmation do not match."));
            // create a user
            MembershipCreateStatus status;
            Membership.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, out status);
            if (status != MembershipCreateStatus.Success)
            	throw new Exception(Localize(status.ToString(), CreateErrors[status]));
            // retrieve the primary key of the new user account
            MembershipUser newUser = Membership.GetUser(username);
            // update a comment
            if (!(String.IsNullOrEmpty(comment)))
            {
                newUser.Comment = comment;
                Membership.UpdateUser(newUser);
            }
            if (!(String.IsNullOrEmpty(roles)))
            	foreach (string role in roles.Split(','))
                	System.Web.Security.Roles.AddUserToRole(username, role);
        }
        
        [ControllerAction("MyProfile", "signUpForm", "Insert", ActionPhase.Before)]
        protected virtual void SignUpUser(string username, string password, string confirmPassword, string email, string passwordQuestion, string passwordAnswer)
        {
            InsertUser(username, password, confirmPassword, email, passwordQuestion, passwordAnswer, true, Localize("SelfRegisteredUser", "Self-registered user."), "Users");
        }
        
        [RowBuilder("MyProfile", "passwordRequestForm", RowKind.New)]
        protected virtual void NewPasswordRequestRow()
        {
            UpdateFieldValue("UserName", Context.Session["IdentityConfirmation"]);
        }
        
        [RowBuilder("MyProfile", "loginForm", RowKind.New)]
        protected virtual void NewLoginFormRow()
        {
            Uri urlReferrer = Context.Request.UrlReferrer;
            if (urlReferrer != null)
            {
                string url = urlReferrer.ToString();
                if (url.Contains("ReturnUrl=/_invoke/getidentity"))
                	UpdateFieldValue("DisplayRememberMe", false);
            }
        }
        
        [ControllerAction("MyProfile", "passwordRequestForm", "Custom", "RequestPassword", ActionPhase.Execute)]
        protected virtual void PasswordRequest(string userName)
        {
            PreventDefault();
            MembershipUser user = Membership.GetUser(userName);
            if ((user == null) || (!(String.IsNullOrEmpty(user.Comment)) && Regex.IsMatch(user.Comment, "Source:\\s+\\w+")))
            	Result.ShowAlert(Localize("UserNameDoesNotExist", "User name does not exist."), "UserName");
            else
            {
                Context.Session["IdentityConfirmation"] = userName;
                if (!(ApplicationServices.IsTouchClient))
                	Result.HideModal();
                Result.ShowModal("MyProfile", "identityConfirmationForm", "Edit", "identityConfirmationForm");
            }
        }
        
        [RowBuilder("MyProfile", "identityConfirmationForm", RowKind.Existing)]
        protected virtual void PrepareIdentityConfirmationRow()
        {
            string userName = ((string)(Context.Session["IdentityConfirmation"]));
            UpdateFieldValue("UserName", userName);
            UpdateFieldValue("PasswordAnswer", null);
            UpdateFieldValue("PasswordQuestion", Membership.GetUser(userName).PasswordQuestion);
        }
        
        [ControllerAction("MyProfile", "identityConfirmationForm", "Custom", "ConfirmIdentity", ActionPhase.Execute)]
        protected virtual void IdentityConfirmation(string userName, string passwordAnswer)
        {
            PreventDefault();
            MembershipUser user = Membership.GetUser(userName);
            if (user != null)
            {
                string newPassword = user.ResetPassword(passwordAnswer);
                // create an email and send it to the user
                MailMessage message = new MailMessage();
                message.To.Add(user.Email);
                message.Subject = String.Format(Localize("NewPasswordSubject", "New password for \'{0}\'."), userName);
                message.Body = newPassword;
                try
                {
                    SmtpClient client = new SmtpClient();
                    client.Send(message);
                    // hide modal popup and display a confirmation
                    Result.ExecuteOnClient("$app.alert(\'{0}\', function () {{ window.history.go(-2); }})", Localize("NewPasswordAlert", "A new password has been emailed to the address on file."));
                }
                catch (Exception error)
                {
                    Result.ShowAlert(error.Message);
                }
            }
        }
        
        [RowBuilder("MyProfile", "myAccountForm", RowKind.Existing)]
        protected virtual void PrepareCurrentUserRow()
        {
            UpdateFieldValue("UserName", UserName);
            UpdateFieldValue("Email", UserEmail);
            UpdateFieldValue("PasswordQuestion", Membership.GetUser().PasswordQuestion);
        }
        
        [ControllerAction("MyProfile", "identityConfirmationForm", "Custom", "BackToRequestPassword", ActionPhase.Execute)]
        protected virtual void BackToRequestPassword()
        {
            PreventDefault();
            Result.HideModal();
            if (!(ApplicationServices.IsTouchClient))
            	Result.ShowModal("MyProfile", "passwordRequestForm", "New", "passwordRequestForm");
        }
        
        [ControllerAction("MyProfile", "myAccountForm", "Update", ActionPhase.Before)]
        protected virtual void UpdateMyAccount(string userName, string oldPassword, string password, string confirmPassword, string email, string passwordQuestion, string passwordAnswer)
        {
            PreventDefault();
            MembershipUser user = Membership.GetUser(userName);
            if (user != null)
            {
                if (String.IsNullOrEmpty(oldPassword))
                {
                    Result.ShowAlert(Localize("EnterCurrentPassword", "Please enter your current password."), "OldPassword");
                    return;
                }
                if (!(Membership.ValidateUser(userName, oldPassword)))
                {
                    Result.ShowAlert(Localize("PasswordDoesNotMatchRecords", "Your password does not match our records."), "OldPassword");
                    return;
                }
                if (!(String.IsNullOrEmpty(password)) || !(String.IsNullOrEmpty(confirmPassword)))
                {
                    if (password != confirmPassword)
                    {
                        Result.ShowAlert(Localize("NewPasswordAndConfirmatinDoNotMatch", "New password and confirmation do not match."), "Password");
                        return;
                    }
                    if (!(user.ChangePassword(oldPassword, password)))
                    {
                        Result.ShowAlert(Localize("NewPasswordInvalid", "Your new password is invalid."), "Password");
                        return;
                    }
                }
                if (email != user.Email)
                {
                    user.Email = email;
                    Membership.UpdateUser(user);
                }
                if (user.PasswordQuestion != passwordQuestion && String.IsNullOrEmpty(passwordAnswer))
                {
                    Result.ShowAlert(Localize("EnterPasswordAnswer", "Please enter a password answer."), "PasswordAnswer");
                    return;
                }
                if (!(String.IsNullOrEmpty(passwordAnswer)))
                {
                    user.ChangePasswordQuestionAndAnswer(oldPassword, passwordQuestion, passwordAnswer);
                    Membership.UpdateUser(user);
                }
                Result.HideModal();
            }
            else
            	Result.ShowAlert(Localize("UserNotFound", "User not found."));
        }
        
        [ControllerAction("MyProfile", "Select", ActionPhase.Before)]
        public virtual void AccessControlValidation()
        {
            if (Context.User.Identity.IsAuthenticated)
            	return;
            if (!((((Request.View == "signUpForm") || (Request.View == "passwordRequestForm")) || ((Request.View == "identityConfirmationForm") || (Request.View == "loginForm")))))
            	throw new Exception("Not authorized");
        }
    }
}
