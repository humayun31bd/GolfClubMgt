using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberCardTransactionDepositModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCardTranID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberNameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCellPhone;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCategoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberStatus;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberNationality;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCardID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCardNumber;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _payTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _payTypePayTypeName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _depositAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _paidAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _tranDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _createdBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _createdDt;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isPosted;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isSmsSend;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _sendTime;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isApproved;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _approvedBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _bankID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _chequeNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _chequeDT;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _particulars;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _mrno;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _refNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _bankCardID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankCardTypeName;
        
        public MemberCardTransactionDepositModel()
        {
        }
        
        public MemberCardTransactionDepositModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? MemberCardTranID
        {
            get
            {
                return _memberCardTranID;
            }
            set
            {
                _memberCardTranID = value;
                UpdateFieldValue("MemberCardTranID", value);
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
        
        public string MemberStatus
        {
            get
            {
                return _memberStatus;
            }
            set
            {
                _memberStatus = value;
                UpdateFieldValue("MemberStatus", value);
            }
        }
        
        public string MemberNationality
        {
            get
            {
                return _memberNationality;
            }
            set
            {
                _memberNationality = value;
                UpdateFieldValue("MemberNationality", value);
            }
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
        
        public string MemberCardNumber
        {
            get
            {
                return _memberCardNumber;
            }
            set
            {
                _memberCardNumber = value;
                UpdateFieldValue("MemberCardNumber", value);
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
        
        public string PayTypePayTypeName
        {
            get
            {
                return _payTypePayTypeName;
            }
            set
            {
                _payTypePayTypeName = value;
                UpdateFieldValue("PayTypePayTypeName", value);
            }
        }
        
        public decimal? DepositAmount
        {
            get
            {
                return _depositAmount;
            }
            set
            {
                _depositAmount = value;
                UpdateFieldValue("DepositAmount", value);
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
        
        public DateTime? TranDate
        {
            get
            {
                return _tranDate;
            }
            set
            {
                _tranDate = value;
                UpdateFieldValue("TranDate", value);
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
        
        public bool? IsPosted
        {
            get
            {
                return _isPosted;
            }
            set
            {
                _isPosted = value;
                UpdateFieldValue("IsPosted", value);
            }
        }
        
        public bool? IsSmsSend
        {
            get
            {
                return _isSmsSend;
            }
            set
            {
                _isSmsSend = value;
                UpdateFieldValue("IsSmsSend", value);
            }
        }
        
        public DateTime? SendTime
        {
            get
            {
                return _sendTime;
            }
            set
            {
                _sendTime = value;
                UpdateFieldValue("SendTime", value);
            }
        }
        
        public bool? IsApproved
        {
            get
            {
                return _isApproved;
            }
            set
            {
                _isApproved = value;
                UpdateFieldValue("IsApproved", value);
            }
        }
        
        public string ApprovedBy
        {
            get
            {
                return _approvedBy;
            }
            set
            {
                _approvedBy = value;
                UpdateFieldValue("ApprovedBy", value);
            }
        }
        
        public int? BankID
        {
            get
            {
                return _bankID;
            }
            set
            {
                _bankID = value;
                UpdateFieldValue("BankID", value);
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
        
        public string Particulars
        {
            get
            {
                return _particulars;
            }
            set
            {
                _particulars = value;
                UpdateFieldValue("Particulars", value);
            }
        }
        
        public string Mrno
        {
            get
            {
                return _mrno;
            }
            set
            {
                _mrno = value;
                UpdateFieldValue("Mrno", value);
            }
        }
        
        public string RefNo
        {
            get
            {
                return _refNo;
            }
            set
            {
                _refNo = value;
                UpdateFieldValue("RefNo", value);
            }
        }
        
        public int? BankCardID
        {
            get
            {
                return _bankCardID;
            }
            set
            {
                _bankCardID = value;
                UpdateFieldValue("BankCardID", value);
            }
        }
        
        public string BankCardTypeName
        {
            get
            {
                return _bankCardTypeName;
            }
            set
            {
                _bankCardTypeName = value;
                UpdateFieldValue("BankCardTypeName", value);
            }
        }
    }
}
