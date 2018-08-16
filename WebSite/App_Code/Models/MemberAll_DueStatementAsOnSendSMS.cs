using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberAll_DueStatementAsOnSendSMSModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cellPhone;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _categoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _dueAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyAddress;
        
        public MemberAll_DueStatementAsOnSendSMSModel()
        {
        }
        
        public MemberAll_DueStatementAsOnSendSMSModel(BusinessRules r) : 
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
        
        public string CellPhone
        {
            get
            {
                return _cellPhone;
            }
            set
            {
                _cellPhone = value;
                UpdateFieldValue("CellPhone", value);
            }
        }
        
        public string CategoryName
        {
            get
            {
                return _categoryName;
            }
            set
            {
                _categoryName = value;
                UpdateFieldValue("CategoryName", value);
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
