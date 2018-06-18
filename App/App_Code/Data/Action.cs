using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Security;

namespace MyCompany.Data
{
	public class Action
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _id;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _commandName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _commandArgument;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _headerText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _description;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cssClass;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _confirmation;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _notify;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _whenLastCommandName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _whenLastCommandArgument;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _whenKeySelected;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _whenClientScript;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _causesValidation;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _whenTag;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _whenHRef;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _whenView;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _key;
        
        public Action()
        {
        }
        
        public Action(XPathNavigator action, IXmlNamespaceResolver resolver)
        {
            this._id = action.GetAttribute("id", String.Empty);
            this._commandName = action.GetAttribute("commandName", String.Empty);
            this._commandArgument = action.GetAttribute("commandArgument", String.Empty);
            this._headerText = action.GetAttribute("headerText", String.Empty);
            this._description = action.GetAttribute("description", String.Empty);
            this._cssClass = action.GetAttribute("cssClass", String.Empty);
            this._confirmation = action.GetAttribute("confirmation", String.Empty);
            this._notify = action.GetAttribute("notify", String.Empty);
            this._whenLastCommandName = action.GetAttribute("whenLastCommandName", String.Empty);
            this._whenLastCommandArgument = action.GetAttribute("whenLastCommandArgument", String.Empty);
            this._causesValidation = !((action.GetAttribute("causesValidation", String.Empty) == "false"));
            this._whenKeySelected = (action.GetAttribute("whenKeySelected", String.Empty) == "true");
            this._whenTag = action.GetAttribute("whenTag", String.Empty);
            this._whenHRef = action.GetAttribute("whenHRef", String.Empty);
            this._whenView = action.GetAttribute("whenView", String.Empty);
            this._whenClientScript = action.GetAttribute("whenClientScript", String.Empty);
            this._key = action.GetAttribute("key", String.Empty);
        }
        
        public string Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }
        
        public string CommandName
        {
            get
            {
                return this._commandName;
            }
            set
            {
                this._commandName = value;
            }
        }
        
        public string CommandArgument
        {
            get
            {
                return this._commandArgument;
            }
            set
            {
                this._commandArgument = value;
            }
        }
        
        public string HeaderText
        {
            get
            {
                return this._headerText;
            }
            set
            {
                this._headerText = value;
            }
        }
        
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }
        
        public string CssClass
        {
            get
            {
                return this._cssClass;
            }
            set
            {
                this._cssClass = value;
            }
        }
        
        public string Confirmation
        {
            get
            {
                return this._confirmation;
            }
            set
            {
                this._confirmation = value;
            }
        }
        
        public string Notify
        {
            get
            {
                return this._notify;
            }
            set
            {
                this._notify = value;
            }
        }
        
        public string WhenLastCommandName
        {
            get
            {
                return this._whenLastCommandName;
            }
            set
            {
                this._whenLastCommandName = value;
            }
        }
        
        public string WhenLastCommandArgument
        {
            get
            {
                return this._whenLastCommandArgument;
            }
            set
            {
                this._whenLastCommandArgument = value;
            }
        }
        
        public bool WhenKeySelected
        {
            get
            {
                return this._whenKeySelected;
            }
            set
            {
                this._whenKeySelected = value;
            }
        }
        
        public string WhenClientScript
        {
            get
            {
                return this._whenClientScript;
            }
            set
            {
                this._whenClientScript = value;
            }
        }
        
        public bool CausesValidation
        {
            get
            {
                return this._causesValidation;
            }
            set
            {
                this._causesValidation = value;
            }
        }
        
        public string WhenTag
        {
            get
            {
                return this._whenTag;
            }
            set
            {
                this._whenTag = value;
            }
        }
        
        public string WhenHRef
        {
            get
            {
                return this._whenHRef;
            }
            set
            {
                this._whenHRef = value;
            }
        }
        
        public string WhenView
        {
            get
            {
                return this._whenView;
            }
            set
            {
                this._whenView = value;
            }
        }
        
        public string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }
    }
}
