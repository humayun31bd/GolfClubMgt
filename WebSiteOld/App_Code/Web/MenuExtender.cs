using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MyCompany.Data;
using MyCompany.Services;

namespace MyCompany.Web
{
	public enum MenuHoverStyle
    {
        
        Auto = 1,
        
        Click = 1,
        
        ClickAndStay = 1,
    }
    
    public enum MenuPresentationStyle
    {
        
        MultiLevel,
        
        TwoLevel,
        
        NavigationButton,
    }
    
    public enum MenuOrientation
    {
        
        Horizontal,
    }
    
    public enum MenuPopupPosition
    {
        
        Left,
        
        Right,
    }
    
    public enum MenuItemDescriptionStyle
    {
        
        None,
        
        Inline,
        
        ToolTip,
    }
    
    [TargetControlType(typeof(Panel))]
    [TargetControlType(typeof(HtmlContainerControl))]
    [DefaultProperty("TargetControlID")]
    public class MenuExtender : System.Web.UI.WebControls.HierarchicalDataBoundControl, IExtenderControl
    {
        
        private string _items;
        
        private ScriptManager _sm;
        
        private string _targetControlID;
        
        private bool _visible;
        
        private MenuHoverStyle _hoverStyle;
        
        private MenuPopupPosition _popupPosition;
        
        private MenuItemDescriptionStyle _itemDescriptionStyle;
        
        private bool _showSiteActions;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private MenuPresentationStyle _presentationStyle;
        
        public MenuExtender() : 
                base()
        {
            this.Visible = true;
            ItemDescriptionStyle = MenuItemDescriptionStyle.ToolTip;
            HoverStyle = MenuHoverStyle.Auto;
        }
        
        [IDReferenceProperty]
        [Category("Behavior")]
        [DefaultValue("")]
        public string TargetControlID
        {
            get
            {
                return _targetControlID;
            }
            set
            {
                _targetControlID = value;
            }
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }
        
        public MenuHoverStyle HoverStyle
        {
            get
            {
                return _hoverStyle;
            }
            set
            {
                _hoverStyle = value;
            }
        }
        
        public MenuPopupPosition PopupPosition
        {
            get
            {
                return _popupPosition;
            }
            set
            {
                _popupPosition = value;
            }
        }
        
        public MenuItemDescriptionStyle ItemDescriptionStyle
        {
            get
            {
                return _itemDescriptionStyle;
            }
            set
            {
                _itemDescriptionStyle = value;
            }
        }
        
        [System.ComponentModel.Description("The \"Site Actions\" menu is automatically displayed.")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowSiteActions
        {
            get
            {
                return _showSiteActions;
            }
            set
            {
                _showSiteActions = value;
            }
        }
        
        [System.ComponentModel.Description("Specifies the menu presentation style.")]
        [System.ComponentModel.DefaultValue(MenuPresentationStyle.MultiLevel)]
        public MenuPresentationStyle PresentationStyle
        {
            get
            {
                return this._presentationStyle;
            }
            set
            {
                this._presentationStyle = value;
            }
        }
        
        protected override void PerformDataBinding()
        {
            base.PerformDataBinding();
            if (!(IsBoundUsingDataSourceID) && (DataSource != null))
            	return;
            HierarchicalDataSourceView view = GetData(String.Empty);
            IHierarchicalEnumerable enumerable = view.Select();
            if (enumerable != null)
            {
                StringBuilder sb = new StringBuilder();
                RecursiveDataBindInternal(enumerable, sb);
                _items = sb.ToString();
            }
        }
        
        private void RecursiveDataBindInternal(IHierarchicalEnumerable enumerable, StringBuilder sb)
        {
            bool first = true;
            if (this.Site != null)
            	return;
            foreach (object item in enumerable)
            {
                IHierarchyData data = enumerable.GetHierarchyData(item);
                if (null != data)
                {
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(data);
                    if (props.Count > 0)
                    {
                        string title = ((string)(props["Title"].GetValue(data)));
                        string description = ((string)(props["Description"].GetValue(data)));
                        string url = ((string)(props["Url"].GetValue(data)));
                        string cssClass = null;
                        bool isPublic = false;
                        if (item is SiteMapNode)
                        {
                            cssClass = ((SiteMapNode)(item))["cssClass"];
                            isPublic = ("true" == ((string)(((SiteMapNode)(item))["public"])));
                        }
                        bool resourceAuthorized = true;
                        if (resourceAuthorized)
                        {
                            if (first)
                            	first = false;
                            else
                            	sb.Append(",");
                            sb.AppendFormat("{{title:\"{0}\",url:\"{1}\"", BusinessRules.JavaScriptString(title), BusinessRules.JavaScriptString(url));
                            if (!(String.IsNullOrEmpty(description)))
                            	sb.AppendFormat(",description:\"{0}\"", BusinessRules.JavaScriptString(description));
                            if (url == Page.Request.RawUrl)
                            	sb.Append(",selected:true");
                            if (!(String.IsNullOrEmpty(cssClass)))
                            	sb.AppendFormat(",cssClass:\"{0}\"", cssClass);
                            if (data.HasChildren)
                            {
                                IHierarchicalEnumerable childrenEnumerable = data.GetChildren();
                                if (null != childrenEnumerable)
                                {
                                    sb.Append(",\"children\":[");
                                    RecursiveDataBindInternal(childrenEnumerable, sb);
                                    sb.Append("]");
                                }
                            }
                            sb.Append("}");
                        }
                    }
                }
            }
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _sm = ScriptManager.GetCurrent(Page);
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AquariumExtenderBase.RegisterFrameworkSettings(Page);
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (null == _sm)
            	return;
            string script = String.Format("Web.Menu.Nodes.{0}=[{1}];", this.ClientID, _items);
            Control target = Page.Form.FindControl(TargetControlID);
            if ((null != target) && target.Visible)
            	ScriptManager.RegisterStartupScript(this, typeof(MenuExtender), "Nodes", script, true);
            _sm.RegisterExtenderControl<MenuExtender>(this, target);
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            bool isTouchUI = ApplicationServices.IsTouchClient;
            if ((null == _sm) || (_sm.IsInAsyncPostBack || isTouchUI))
            	return;
            _sm.RegisterScriptDescriptors(this);
        }
        
        IEnumerable<ScriptDescriptor> IExtenderControl.GetScriptDescriptors(Control targetControl)
        {
            ScriptBehaviorDescriptor descriptor = new ScriptBehaviorDescriptor("Web.Menu", targetControl.ClientID);
            descriptor.AddProperty("id", this.ClientID);
            if (HoverStyle != MenuHoverStyle.Auto)
            	descriptor.AddProperty("hoverStyle", Convert.ToInt32(HoverStyle));
            if (PopupPosition != MenuPopupPosition.Left)
            	descriptor.AddProperty("popupPosition", Convert.ToInt32(PopupPosition));
            if (ItemDescriptionStyle != MenuItemDescriptionStyle.ToolTip)
            	descriptor.AddProperty("itemDescriptionStyle", Convert.ToInt32(ItemDescriptionStyle));
            if (ShowSiteActions)
            	descriptor.AddProperty("showSiteActions", "true");
            if (PresentationStyle != MenuPresentationStyle.MultiLevel)
            	descriptor.AddProperty("presentationStyle", Convert.ToInt32(PresentationStyle));
            return new ScriptBehaviorDescriptor[] {
                    descriptor};
        }
        
        IEnumerable<ScriptReference> IExtenderControl.GetScriptReferences()
        {
            return AquariumExtenderBase.StandardScripts();
        }
    }
}
