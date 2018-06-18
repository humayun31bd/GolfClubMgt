using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberDueStatementModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _billNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _categoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _payAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _billDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _collectionBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _sMemID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _onAccountOf;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _subsiDueOnDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _subscriptionDueDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _amount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyAddress;
        
        public MemberDueStatementModel()
        {
        }
        
        public MemberDueStatementModel(BusinessRules r) : 
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
        
        public string BillNo
        {
            get
            {
                return _billNo;
            }
            set
            {
                _billNo = value;
                UpdateFieldValue("BillNo", value);
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
        
        public decimal? PayAmount
        {
            get
            {
                return _payAmount;
            }
            set
            {
                _payAmount = value;
                UpdateFieldValue("PayAmount", value);
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
        
        public string CollectionBy
        {
            get
            {
                return _collectionBy;
            }
            set
            {
                _collectionBy = value;
                UpdateFieldValue("CollectionBy", value);
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
        
        public int? sMemID
        {
            get
            {
                return _sMemID;
            }
            set
            {
                _sMemID = value;
                UpdateFieldValue("sMemID", value);
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
        
        public DateTime? SubsiDueOnDate
        {
            get
            {
                return _subsiDueOnDate;
            }
            set
            {
                _subsiDueOnDate = value;
                UpdateFieldValue("SubsiDueOnDate", value);
            }
        }
        
        public DateTime? SubscriptionDueDate
        {
            get
            {
                return _subscriptionDueDate;
            }
            set
            {
                _subscriptionDueDate = value;
                UpdateFieldValue("SubscriptionDueDate", value);
            }
        }
        
        public decimal? Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                UpdateFieldValue("Amount", value);
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
