using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class CoachFeeReportModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _coachFeeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _mrno;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _mrdate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberGuestName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberProID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberInfoNameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _serviceID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _serviceName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _mtimefrom;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _mtimeTo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _holeTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _holeTypeDesc;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _amount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _paytypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _payTypePayTypeName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _chequeNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _chequeDT;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _bankID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _dT;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _fT;
        
        public CoachFeeReportModel()
        {
        }
        
        public CoachFeeReportModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? CoachFeeID
        {
            get
            {
                return _coachFeeID;
            }
            set
            {
                _coachFeeID = value;
                UpdateFieldValue("CoachFeeID", value);
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
        
        public DateTime? Mrdate
        {
            get
            {
                return _mrdate;
            }
            set
            {
                _mrdate = value;
                UpdateFieldValue("Mrdate", value);
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
        
        public string MemberGuestName
        {
            get
            {
                return _memberGuestName;
            }
            set
            {
                _memberGuestName = value;
                UpdateFieldValue("MemberGuestName", value);
            }
        }
        
        public int? MemberProID
        {
            get
            {
                return _memberProID;
            }
            set
            {
                _memberProID = value;
                UpdateFieldValue("MemberProID", value);
            }
        }
        
        public string MemberInfoNameOfMember
        {
            get
            {
                return _memberInfoNameOfMember;
            }
            set
            {
                _memberInfoNameOfMember = value;
                UpdateFieldValue("MemberInfoNameOfMember", value);
            }
        }
        
        public int? ServiceID
        {
            get
            {
                return _serviceID;
            }
            set
            {
                _serviceID = value;
                UpdateFieldValue("ServiceID", value);
            }
        }
        
        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
            set
            {
                _serviceName = value;
                UpdateFieldValue("ServiceName", value);
            }
        }
        
        public DateTime? Mtimefrom
        {
            get
            {
                return _mtimefrom;
            }
            set
            {
                _mtimefrom = value;
                UpdateFieldValue("Mtimefrom", value);
            }
        }
        
        public DateTime? MtimeTo
        {
            get
            {
                return _mtimeTo;
            }
            set
            {
                _mtimeTo = value;
                UpdateFieldValue("MtimeTo", value);
            }
        }
        
        public int? HoleTypeID
        {
            get
            {
                return _holeTypeID;
            }
            set
            {
                _holeTypeID = value;
                UpdateFieldValue("HoleTypeID", value);
            }
        }
        
        public string HoleTypeDesc
        {
            get
            {
                return _holeTypeDesc;
            }
            set
            {
                _holeTypeDesc = value;
                UpdateFieldValue("HoleTypeDesc", value);
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
        
        public int? PaytypeID
        {
            get
            {
                return _paytypeID;
            }
            set
            {
                _paytypeID = value;
                UpdateFieldValue("PaytypeID", value);
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
        
        public bool? DT
        {
            get
            {
                return _dT;
            }
            set
            {
                _dT = value;
                UpdateFieldValue("DT", value);
            }
        }
        
        public bool? FT
        {
            get
            {
                return _fT;
            }
            set
            {
                _fT = value;
                UpdateFieldValue("FT", value);
            }
        }
    }
}
