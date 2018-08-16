using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberBillApprovalModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _clubAccountID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accountCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _accountName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _payTypeName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _payTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _tranDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _amount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isApproved;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _approvedBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _voucherNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _voucherTranID;
        
        public MemberBillApprovalModel()
        {
        }
        
        public MemberBillApprovalModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? ClubAccountID
        {
            get
            {
                return _clubAccountID;
            }
            set
            {
                _clubAccountID = value;
                UpdateFieldValue("ClubAccountID", value);
            }
        }
        
        public int? AccountCode
        {
            get
            {
                return _accountCode;
            }
            set
            {
                _accountCode = value;
                UpdateFieldValue("AccountCode", value);
            }
        }
        
        public string AccountName
        {
            get
            {
                return _accountName;
            }
            set
            {
                _accountName = value;
                UpdateFieldValue("AccountName", value);
            }
        }
        
        public string PayTypeName
        {
            get
            {
                return _payTypeName;
            }
            set
            {
                _payTypeName = value;
                UpdateFieldValue("PayTypeName", value);
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
        
        public string VoucherNo
        {
            get
            {
                return _voucherNo;
            }
            set
            {
                _voucherNo = value;
                UpdateFieldValue("VoucherNo", value);
            }
        }
        
        public int? VoucherTranID
        {
            get
            {
                return _voucherTranID;
            }
            set
            {
                _voucherTranID = value;
                UpdateFieldValue("VoucherTranID", value);
            }
        }
    }
}
