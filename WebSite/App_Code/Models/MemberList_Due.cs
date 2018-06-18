using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberList_DueModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _email;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _onAccountOf;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _dueAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _billDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyAddress;
        
        public MemberList_DueModel()
        {
        }
        
        public MemberList_DueModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? MemberID
        {
            get
            {
                return _memberID;
            }
            set
            {
                _memberID = value;
                UpdateFieldValue("MemberID", value);
            }
        }
        
        public string MemberCode
        {
            get
            {
                return _memberCode;
            }
            set
            {
                _memberCode = value;
                UpdateFieldValue("MemberCode", value);
            }
        }
        
        public string NameOfMember
        {
            get
            {
                return _nameOfMember;
            }
            set
            {
                _nameOfMember = value;
                UpdateFieldValue("NameOfMember", value);
            }
        }
        
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                UpdateFieldValue("Email", value);
            }
        }
        
        public string OnAccountOf
        {
            get
            {
                return _onAccountOf;
            }
            set
            {
                _onAccountOf = value;
                UpdateFieldValue("OnAccountOf", value);
            }
        }
        
        public decimal? DueAmount
        {
            get
            {
                return _dueAmount;
            }
            set
            {
                _dueAmount = value;
                UpdateFieldValue("DueAmount", value);
            }
        }
        
        public DateTime? BillDate
        {
            get
            {
                return _billDate;
            }
            set
            {
                _billDate = value;
                UpdateFieldValue("BillDate", value);
            }
        }
        
        public string CompanyName
        {
            get
            {
                return _companyName;
            }
            set
            {
                _companyName = value;
                UpdateFieldValue("CompanyName", value);
            }
        }
        
        public string CompanyAddress
        {
            get
            {
                return _companyAddress;
            }
            set
            {
                _companyAddress = value;
                UpdateFieldValue("CompanyAddress", value);
            }
        }
    }
}
