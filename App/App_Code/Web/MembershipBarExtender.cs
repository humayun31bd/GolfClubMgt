using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MyCompany.Web
{
	public partial class MembershipBarExtender : MembershipBarExtenderBase
    {
    }
    
    [TargetControlType(typeof(HtmlGenericControl))]
    [ToolboxItem(false)]
    public class MembershipBarExtenderBase : AquariumExtenderBase
    {
        
        public MembershipBarExtenderBase() : 
                base("Web.Membership")
        {
        }
        
        protected override bool RequiresMembershipScripts
        {
            get
            {
                return true;
            }
        }
        
        protected override void ConfigureDescriptor(ScriptBehaviorDescriptor descriptor)
        {
            descriptor.AddProperty("displayRememberMe", Properties["DisplayRememberMe"]);
            descriptor.AddProperty("rememberMeSet", Properties["RememberMeSet"]);
            descriptor.AddProperty("displaySignUp", Properties["DisplaySignUp"]);
            descriptor.AddProperty("displayPasswordRecovery", Properties["DisplayPasswordRecovery"]);
            descriptor.AddProperty("displayMyAccount", Properties["DisplayMyAccount"]);
            string s = ((string)(Properties["Welcome"]));
            if (!(String.IsNullOrEmpty(s)))
            	descriptor.AddProperty("welcome", Properties["Welcome"]);
            s = ((string)(Properties["User"]));
            if (!(String.IsNullOrEmpty(s)))
            	descriptor.AddProperty("user", Properties["User"]);
            descriptor.AddProperty("displayHelp", Properties["DisplayHelp"]);
            descriptor.AddProperty("displayLogin", Properties["DisplayLogin"]);
        }
    }
}
