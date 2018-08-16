using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class LockerBookingModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _lockerBookID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _lockerID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _lockerCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _lockerIsFree;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberNameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _bookStart;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _bookEnd;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bookStatus;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCategoryID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCategoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _lockerBillAmount;
        
        public LockerBookingModel()
        {
        }
        
        public LockerBookingModel(BusinessRules r) : 
                base(r)
        {
        }
        
        public int? LockerBookID
        {
            get
            {
                return _lockerBookID;
            }
            set
            {
                _lockerBookID = value;
                UpdateFieldValue("LockerBookID", value);
            }
        }
        
        public int? LockerID
        {
            get
            {
                return _lockerID;
            }
            set
            {
                _lockerID = value;
                UpdateFieldValue("LockerID", value);
            }
        }
        
        public string LockerCode
        {
            get
            {
                return _lockerCode;
            }
            set
            {
                _lockerCode = value;
                UpdateFieldValue("LockerCode", value);
            }
        }
        
        public bool? LockerIsFree
        {
            get
            {
                return _lockerIsFree;
            }
            set
            {
                _lockerIsFree = value;
                UpdateFieldValue("LockerIsFree", value);
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
        
        public DateTime? BookStart
        {
            get
            {
                return _bookStart;
            }
            set
            {
                _bookStart = value;
                UpdateFieldValue("BookStart", value);
            }
        }
        
        public DateTime? BookEnd
        {
            get
            {
                return _bookEnd;
            }
            set
            {
                _bookEnd = value;
                UpdateFieldValue("BookEnd", value);
            }
        }
        
        public string BookStatus
        {
            get
            {
                return _bookStatus;
            }
            set
            {
                _bookStatus = value;
                UpdateFieldValue("BookStatus", value);
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
        
        public decimal? LockerBillAmount
        {
            get
            {
                return _lockerBillAmount;
            }
            set
            {
                _lockerBillAmount = value;
                UpdateFieldValue("LockerBillAmount", value);
            }
        }
    }
}
