using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MyCompany.Data;

namespace MyCompany.Web
{
	public class MembershipManager : Control, INamingContainer
    {
        
        private string _servicePath;
        
        public MembershipManager()
        {
        }
        
        [System.ComponentModel.Description("A path to a data controller web service.")]
        [System.ComponentModel.DefaultValue("~/Services/DataControllerService.asmx")]
        public string ServicePath
        {
            get
            {
                if (String.IsNullOrEmpty(_servicePath))
                	return "~/Services/DataControllerService.asmx";
                return _servicePath;
            }
            set
            {
                _servicePath = value;
            }
        }
        
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "d";
            Controls.Add(div);
            MembershipManagerExtender manager = new MembershipManagerExtender();
            manager.ID = "b";
            manager.TargetControlID = div.ID;
            manager.ServicePath = ServicePath;
            Controls.Add(manager);
        }
    }
}
