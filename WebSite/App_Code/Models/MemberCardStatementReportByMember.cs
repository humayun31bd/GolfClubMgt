using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberCardStatementReportByMemberModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCardID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberNameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCategoryID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCellPhone;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cardNumber;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cardpin;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cardHashTag;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _createdDt;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _createdBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _updated;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _updatedBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _cardBalance;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _mobileNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isCardActive;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isVarifiedPin;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCategoryName;
        
        public MemberCardStatementReportByMemberModel()
        {
        }
        
        public MemberCardStatementReportByMemberModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? MemberCardID
        {
            get
            {
                return _memberCardID;
            }
            set
            {
                _memberCardID = value;
                UpdateFieldValue("MemberCardID", value);
            }
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
        
        public string MemberNameOfMember
        {
            get
            {
                return _memberNameOfMember;
            }
            set
            {
                _memberNameOfMember = value;
                UpdateFieldValue("MemberNameOfMember", value);
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
        
        public int? MemberCategoryID
        {
            get
            {
                return _memberCategoryID;
            }
            set
            {
                _memberCategoryID = value;
                UpdateFieldValue("MemberCategoryID", value);
            }
        }
        
        public string MemberCellPhone
        {
            get
            {
                return _memberCellPhone;
            }
            set
            {
                _memberCellPhone = value;
                UpdateFieldValue("MemberCellPhone", value);
            }
        }
        
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                _cardNumber = value;
                UpdateFieldValue("CardNumber", value);
            }
        }
        
        public string Cardpin
        {
            get
            {
                return _cardpin;
            }
            set
            {
                _cardpin = value;
                UpdateFieldValue("Cardpin", value);
            }
        }
        
        public string CardHashTag
        {
            get
            {
                return _cardHashTag;
            }
            set
            {
                _cardHashTag = value;
                UpdateFieldValue("CardHashTag", value);
            }
        }
        
        public DateTime? CreatedDt
        {
            get
            {
                return _createdDt;
            }
            set
            {
                _createdDt = value;
                UpdateFieldValue("CreatedDt", value);
            }
        }
        
        public string CreatedBy
        {
            get
            {
                return _createdBy;
            }
            set
            {
                _createdBy = value;
                UpdateFieldValue("CreatedBy", value);
            }
        }
        
        public DateTime? Updated
        {
            get
            {
                return _updated;
            }
            set
            {
                _updated = value;
                UpdateFieldValue("Updated", value);
            }
        }
        
        public string UpdatedBy
        {
            get
            {
                return _updatedBy;
            }
            set
            {
                _updatedBy = value;
                UpdateFieldValue("UpdatedBy", value);
            }
        }
        
        public decimal? CardBalance
        {
            get
            {
                return _cardBalance;
            }
            set
            {
                _cardBalance = value;
                UpdateFieldValue("CardBalance", value);
            }
        }
        
        public string MobileNo
        {
            get
            {
                return _mobileNo;
            }
            set
            {
                _mobileNo = value;
                UpdateFieldValue("MobileNo", value);
            }
        }
        
        public bool? IsCardActive
        {
            get
            {
                return _isCardActive;
            }
            set
            {
                _isCardActive = value;
                UpdateFieldValue("IsCardActive", value);
            }
        }
        
        public bool? IsVarifiedPin
        {
            get
            {
                return _isVarifiedPin;
            }
            set
            {
                _isVarifiedPin = value;
                UpdateFieldValue("IsVarifiedPin", value);
            }
        }
        
        public string MemberCategoryName
        {
            get
            {
                return _memberCategoryName;
            }
            set
            {
                _memberCategoryName = value;
                UpdateFieldValue("MemberCategoryName", value);
            }
        }
    }
}
