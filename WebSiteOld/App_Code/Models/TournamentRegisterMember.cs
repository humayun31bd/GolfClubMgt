using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class TournamentRegisterMemberModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _tournamentRegisterID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _tournamentID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _tournamentName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _gameCategoryID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _gameCategoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _teeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _teeName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _holeTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _holeTypeDesc;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _tournamentFlightSchID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _tournamentFlightSchNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _regDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _regNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _gameFeeAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _caddieID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _caddieCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _ballBoyID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _ballBoyCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _caddieFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _ballBoyFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _caddieSubsidy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _ballBoySubsidy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _greenFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _golfCartFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _totalAmount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _voucherID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _voucherNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _collectedBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _payTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _payTypeName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _bankID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _chequeNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _chequeDt;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberBillID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isApproved;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _approvedBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _createdBy;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _createdDt;
        
        public TournamentRegisterMemberModel()
        {
        }
        
        public TournamentRegisterMemberModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? TournamentRegisterID
        {
            get
            {
                return _tournamentRegisterID;
            }
            set
            {
                _tournamentRegisterID = value;
                UpdateFieldValue("TournamentRegisterID", value);
            }
        }
        
        public int? TournamentID
        {
            get
            {
                return _tournamentID;
            }
            set
            {
                _tournamentID = value;
                UpdateFieldValue("TournamentID", value);
            }
        }
        
        public string TournamentName
        {
            get
            {
                return _tournamentName;
            }
            set
            {
                _tournamentName = value;
                UpdateFieldValue("TournamentName", value);
            }
        }
        
        public int? GameCategoryID
        {
            get
            {
                return _gameCategoryID;
            }
            set
            {
                _gameCategoryID = value;
                UpdateFieldValue("GameCategoryID", value);
            }
        }
        
        public string GameCategoryName
        {
            get
            {
                return _gameCategoryName;
            }
            set
            {
                _gameCategoryName = value;
                UpdateFieldValue("GameCategoryName", value);
            }
        }
        
        public int? TeeID
        {
            get
            {
                return _teeID;
            }
            set
            {
                _teeID = value;
                UpdateFieldValue("TeeID", value);
            }
        }
        
        public string TeeName
        {
            get
            {
                return _teeName;
            }
            set
            {
                _teeName = value;
                UpdateFieldValue("TeeName", value);
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
        
        public int? TournamentFlightSchID
        {
            get
            {
                return _tournamentFlightSchID;
            }
            set
            {
                _tournamentFlightSchID = value;
                UpdateFieldValue("TournamentFlightSchID", value);
            }
        }
        
        public string TournamentFlightSchNo
        {
            get
            {
                return _tournamentFlightSchNo;
            }
            set
            {
                _tournamentFlightSchNo = value;
                UpdateFieldValue("TournamentFlightSchNo", value);
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
        
        public decimal? GameFeeAmount
        {
            get
            {
                return _gameFeeAmount;
            }
            set
            {
                _gameFeeAmount = value;
                UpdateFieldValue("GameFeeAmount", value);
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
        
        public int? VoucherID
        {
            get
            {
                return _voucherID;
            }
            set
            {
                _voucherID = value;
                UpdateFieldValue("VoucherID", value);
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
        
        public string CollectedBy
        {
            get
            {
                return _collectedBy;
            }
            set
            {
                _collectedBy = value;
                UpdateFieldValue("CollectedBy", value);
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
        
        public int? MemberBillID
        {
            get
            {
                return _memberBillID;
            }
            set
            {
                _memberBillID = value;
                UpdateFieldValue("MemberBillID", value);
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
    }
}
