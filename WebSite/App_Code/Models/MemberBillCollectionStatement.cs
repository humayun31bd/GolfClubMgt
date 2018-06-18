using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberBillCollectionStatementModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _paidBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _billNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _categoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _collectionDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _billDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _account;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _subsiDueOnDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _subscriptionDueDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _amount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _fromDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _toDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyAddress;
        
        public MemberBillCollectionStatementModel()
        {
        }
        
        public MemberBillCollectionStatementModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public string PaidBy
        {
            get
            {
                return _paidBy;
            }
            set
            {
                _paidBy = value;
                UpdateFieldValue("PaidBy", value);
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
        
        public DateTime? CollectionDate
        {
            get
            {
                return _collectionDate;
            }
            set
            {
                _collectionDate = value;
                UpdateFieldValue("CollectionDate", value);
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
        
        public string Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                UpdateFieldValue("Account", value);
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
        
        public DateTime? FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                UpdateFieldValue("FromDate", value);
            }
        }
        
        public DateTime? ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                UpdateFieldValue("ToDate", value);
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
