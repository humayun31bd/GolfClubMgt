using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class DailyRegistrationModel : BusinessRulesObjectModel
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
        private DateTime? _collectionDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _greenFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _caddieFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _ballBoyFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _golfCartFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _totalBill;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _paidAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _payTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _paidBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _chequeNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _chequeDT;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _payAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _dueAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _collectionBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _companyAddress;
        
        public DailyRegistrationModel()
        {
        }
        
        public DailyRegistrationModel(BusinessRules r) : 
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
        
        public decimal? GreenFee
        {
            get
            {
                return _greenFee;
            }
            set
            {
                _greenFee = value;
                UpdateFieldValue("GreenFee", value);
            }
        }
        
        public decimal? CaddieFee
        {
            get
            {
                return _caddieFee;
            }
            set
            {
                _caddieFee = value;
                UpdateFieldValue("CaddieFee", value);
            }
        }
        
        public decimal? BallBoyFee
        {
            get
            {
                return _ballBoyFee;
            }
            set
            {
                _ballBoyFee = value;
                UpdateFieldValue("BallBoyFee", value);
            }
        }
        
        public decimal? GolfCartFee
        {
            get
            {
                return _golfCartFee;
            }
            set
            {
                _golfCartFee = value;
                UpdateFieldValue("GolfCartFee", value);
            }
        }
        
        public decimal? TotalBill
        {
            get
            {
                return _totalBill;
            }
            set
            {
                _totalBill = value;
                UpdateFieldValue("TotalBill", value);
            }
        }
        
        public decimal? PaidAmount
        {
            get
            {
                return _paidAmount;
            }
            set
            {
                _paidAmount = value;
                UpdateFieldValue("PaidAmount", value);
            }
        }
        
        public int? PayTypeID
        {
            get
            {
                return _payTypeID;
            }
            set
            {
                _payTypeID = value;
                UpdateFieldValue("PayTypeID", value);
            }
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
        
        public string ChequeNo
        {
            get
            {
                return _chequeNo;
            }
            set
            {
                _chequeNo = value;
                UpdateFieldValue("ChequeNo", value);
            }
        }
        
        public DateTime? ChequeDT
        {
            get
            {
                return _chequeDT;
            }
            set
            {
                _chequeDT = value;
                UpdateFieldValue("ChequeDT", value);
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
        
        public string BankName
        {
            get
            {
                return _bankName;
            }
            set
            {
                _bankName = value;
                UpdateFieldValue("BankName", value);
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
