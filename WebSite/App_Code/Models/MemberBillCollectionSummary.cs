using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberBillCollectionSummaryModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _collectionDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _account;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _cashAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _chequeAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _memberCardAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _bankCardAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _dueAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _totalAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _fromDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _toDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyAddress;
        
        public MemberBillCollectionSummaryModel()
        {
        }
        
        public MemberBillCollectionSummaryModel(BusinessRules r) : 
                base(r)
        {
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
        
        public decimal? CashAmount
        {
            get
            {
                return _cashAmount;
            }
            set
            {
                _cashAmount = value;
                UpdateFieldValue("CashAmount", value);
            }
        }
        
        public decimal? ChequeAmount
        {
            get
            {
                return _chequeAmount;
            }
            set
            {
                _chequeAmount = value;
                UpdateFieldValue("ChequeAmount", value);
            }
        }
        
        public decimal? MemberCardAmount
        {
            get
            {
                return _memberCardAmount;
            }
            set
            {
                _memberCardAmount = value;
                UpdateFieldValue("MemberCardAmount", value);
            }
        }
        
        public decimal? BankCardAmount
        {
            get
            {
                return _bankCardAmount;
            }
            set
            {
                _bankCardAmount = value;
                UpdateFieldValue("BankCardAmount", value);
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
        
        public decimal? TotalAmount
        {
            get
            {
                return _totalAmount;
            }
            set
            {
                _totalAmount = value;
                UpdateFieldValue("TotalAmount", value);
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
