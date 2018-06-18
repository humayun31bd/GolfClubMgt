using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberGameRegApprovedModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _gregisterID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _flightSchID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _flightSchStartTime;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _flightSchFlightSchNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _playDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _holeTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _holeTypeDesc;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberNameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isSinglePlayer;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isGroupPlayer;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _caddiePermanent;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _caddieID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _caddieCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _caddieName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _ballBoyID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _ballBoyCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _ballBoyBallBoyName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _needGolfCart;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _golfCartID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _golfCartCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _golfCartHoleTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberGroupID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberGroupName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCategoryID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCategoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberStatusID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberStatus;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _greenFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _caddieFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _ballBoyFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _golfCartFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _regNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _regDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _handiCap;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _estTeeOffTime;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _delayTime;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _createdBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _createdDt;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isBooking;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isRegister;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _payTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _payTypePayTypeName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _chequeNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _bankID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _chequeDt;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _caddieSubsidy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _ballBoySubsidy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _greenSubsidy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _totalBill;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _paidAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _collectionBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _billText;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _bookingDate;
        
        public MemberGameRegApprovedModel()
        {
        }
        
        public MemberGameRegApprovedModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? GregisterID
        {
            get
            {
                return _gregisterID;
            }
            set
            {
                _gregisterID = value;
                UpdateFieldValue("GregisterID", value);
            }
        }
        
        public int? FlightSchID
        {
            get
            {
                return _flightSchID;
            }
            set
            {
                _flightSchID = value;
                UpdateFieldValue("FlightSchID", value);
            }
        }
        
        public string FlightSchStartTime
        {
            get
            {
                return _flightSchStartTime;
            }
            set
            {
                _flightSchStartTime = value;
                UpdateFieldValue("FlightSchStartTime", value);
            }
        }
        
        public int? FlightSchFlightSchNo
        {
            get
            {
                return _flightSchFlightSchNo;
            }
            set
            {
                _flightSchFlightSchNo = value;
                UpdateFieldValue("FlightSchFlightSchNo", value);
            }
        }
        
        public DateTime? PlayDate
        {
            get
            {
                return _playDate;
            }
            set
            {
                _playDate = value;
                UpdateFieldValue("PlayDate", value);
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
        
        public bool? IsSinglePlayer
        {
            get
            {
                return _isSinglePlayer;
            }
            set
            {
                _isSinglePlayer = value;
                UpdateFieldValue("IsSinglePlayer", value);
            }
        }
        
        public bool? IsGroupPlayer
        {
            get
            {
                return _isGroupPlayer;
            }
            set
            {
                _isGroupPlayer = value;
                UpdateFieldValue("IsGroupPlayer", value);
            }
        }
        
        public bool? CaddiePermanent
        {
            get
            {
                return _caddiePermanent;
            }
            set
            {
                _caddiePermanent = value;
                UpdateFieldValue("CaddiePermanent", value);
            }
        }
        
        public int? CaddieID
        {
            get
            {
                return _caddieID;
            }
            set
            {
                _caddieID = value;
                UpdateFieldValue("CaddieID", value);
            }
        }
        
        public string CaddieCode
        {
            get
            {
                return _caddieCode;
            }
            set
            {
                _caddieCode = value;
                UpdateFieldValue("CaddieCode", value);
            }
        }
        
        public string CaddieName
        {
            get
            {
                return _caddieName;
            }
            set
            {
                _caddieName = value;
                UpdateFieldValue("CaddieName", value);
            }
        }
        
        public int? BallBoyID
        {
            get
            {
                return _ballBoyID;
            }
            set
            {
                _ballBoyID = value;
                UpdateFieldValue("BallBoyID", value);
            }
        }
        
        public string BallBoyCode
        {
            get
            {
                return _ballBoyCode;
            }
            set
            {
                _ballBoyCode = value;
                UpdateFieldValue("BallBoyCode", value);
            }
        }
        
        public string BallBoyBallBoyName
        {
            get
            {
                return _ballBoyBallBoyName;
            }
            set
            {
                _ballBoyBallBoyName = value;
                UpdateFieldValue("BallBoyBallBoyName", value);
            }
        }
        
        public bool? NeedGolfCart
        {
            get
            {
                return _needGolfCart;
            }
            set
            {
                _needGolfCart = value;
                UpdateFieldValue("NeedGolfCart", value);
            }
        }
        
        public int? GolfCartID
        {
            get
            {
                return _golfCartID;
            }
            set
            {
                _golfCartID = value;
                UpdateFieldValue("GolfCartID", value);
            }
        }
        
        public string GolfCartCode
        {
            get
            {
                return _golfCartCode;
            }
            set
            {
                _golfCartCode = value;
                UpdateFieldValue("GolfCartCode", value);
            }
        }
        
        public int? GolfCartHoleTypeID
        {
            get
            {
                return _golfCartHoleTypeID;
            }
            set
            {
                _golfCartHoleTypeID = value;
                UpdateFieldValue("GolfCartHoleTypeID", value);
            }
        }
        
        public int? MemberGroupID
        {
            get
            {
                return _memberGroupID;
            }
            set
            {
                _memberGroupID = value;
                UpdateFieldValue("MemberGroupID", value);
            }
        }
        
        public string MemberGroupName
        {
            get
            {
                return _memberGroupName;
            }
            set
            {
                _memberGroupName = value;
                UpdateFieldValue("MemberGroupName", value);
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
        
        public int? MemberStatusID
        {
            get
            {
                return _memberStatusID;
            }
            set
            {
                _memberStatusID = value;
                UpdateFieldValue("MemberStatusID", value);
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
        
        public string RegNo
        {
            get
            {
                return _regNo;
            }
            set
            {
                _regNo = value;
                UpdateFieldValue("RegNo", value);
            }
        }
        
        public DateTime? RegDate
        {
            get
            {
                return _regDate;
            }
            set
            {
                _regDate = value;
                UpdateFieldValue("RegDate", value);
            }
        }
        
        public int? HandiCap
        {
            get
            {
                return _handiCap;
            }
            set
            {
                _handiCap = value;
                UpdateFieldValue("HandiCap", value);
            }
        }
        
        public DateTime? EstTeeOffTime
        {
            get
            {
                return _estTeeOffTime;
            }
            set
            {
                _estTeeOffTime = value;
                UpdateFieldValue("EstTeeOffTime", value);
            }
        }
        
        public int? DelayTime
        {
            get
            {
                return _delayTime;
            }
            set
            {
                _delayTime = value;
                UpdateFieldValue("DelayTime", value);
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
        
        public bool? IsBooking
        {
            get
            {
                return _isBooking;
            }
            set
            {
                _isBooking = value;
                UpdateFieldValue("IsBooking", value);
            }
        }
        
        public bool? IsRegister
        {
            get
            {
                return _isRegister;
            }
            set
            {
                _isRegister = value;
                UpdateFieldValue("IsRegister", value);
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
        
        public DateTime? ChequeDt
        {
            get
            {
                return _chequeDt;
            }
            set
            {
                _chequeDt = value;
                UpdateFieldValue("ChequeDt", value);
            }
        }
        
        public decimal? CaddieSubsidy
        {
            get
            {
                return _caddieSubsidy;
            }
            set
            {
                _caddieSubsidy = value;
                UpdateFieldValue("CaddieSubsidy", value);
            }
        }
        
        public decimal? BallBoySubsidy
        {
            get
            {
                return _ballBoySubsidy;
            }
            set
            {
                _ballBoySubsidy = value;
                UpdateFieldValue("BallBoySubsidy", value);
            }
        }
        
        public decimal? GreenSubsidy
        {
            get
            {
                return _greenSubsidy;
            }
            set
            {
                _greenSubsidy = value;
                UpdateFieldValue("GreenSubsidy", value);
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
        
        public string BillText
        {
            get
            {
                return _billText;
            }
            set
            {
                _billText = value;
                UpdateFieldValue("BillText", value);
            }
        }
        
        public DateTime? BookingDate
        {
            get
            {
                return _bookingDate;
            }
            set
            {
                _bookingDate = value;
                UpdateFieldValue("BookingDate", value);
            }
        }
    }
}
