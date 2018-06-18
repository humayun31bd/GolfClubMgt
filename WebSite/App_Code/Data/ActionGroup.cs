﻿using System;
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
	public class ActionGroup
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _scope;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _headerText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _flat;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _id;
        
        private List<Action> _actions;
        
        public ActionGroup()
        {
            this._actions = new List<Action>();
        }
        
        public ActionGroup(XPathNavigator actionGroup, IXmlNamespaceResolver resolver) : 
                this()
        {
            this._scope = actionGroup.GetAttribute("scope", String.Empty);
            this._headerText = actionGroup.GetAttribute("headerText", String.Empty);
            this._id = actionGroup.GetAttribute("id", String.Empty);
            _flat = (actionGroup.GetAttribute("flat", String.Empty) == "true");
            XPathNodeIterator actionIterator = actionGroup.Select("c:action", resolver);
            while (actionIterator.MoveNext())
            	if (Controller.UserIsInRole(actionIterator.Current.GetAttribute("roles", String.Empty)))
                	this.Actions.Add(new Action(actionIterator.Current, resolver));
        }
        
        public string Scope
        {
            get
            {
                return this._scope;
            }
            set
            {
                this._scope = value;
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
        
        public bool Flat
        {
            get
            {
                return this._flat;
            }
            set
            {
                this._flat = value;
            }
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
        
        public List<Action> Actions
        {
            get
            {
                return _actions;
            }
        }
    }
}
