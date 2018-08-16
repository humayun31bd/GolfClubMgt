using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class MemberSendSMSModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCategoryID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberCategoryName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberStatusID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberStatus;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _parentMemberID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberInfo1NameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nameOfMember;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _fatherName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _subsriptionDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _genderID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _genderDesc;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _nationalityID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nationality;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nid;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _handiCap;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _dob;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _bloodGroupID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bloodGroupName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _memberFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _monthlySubcriptionFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _monthlyDonation;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _presentAddress;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _permanentAddress;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _fax;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _cellPhone;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _landPhone;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _email;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _fileName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _contentType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _length;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isActive;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberAge;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberOfType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _anualFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberGroupID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberGroupMemberGroupName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _memberTypeMemberTypeDesc;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bgfid;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nfcid;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _billingAddress;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _clubName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _contributionFee;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _nonMemberCatID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _nonMemberCategoryNonMemCateory;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _ocmnoOfDaysPlay;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _lastPaySubsDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _lastPayContDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _refNumber;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _recomendetMemberShipNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _ocmnoOfDaysPlayed;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _welfareContributionDT;
        
        public MemberSendSMSModel()
        {
        }
        
        public MemberSendSMSModel(BusinessRules r) : 
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
        
        public int? ParentMemberID
        {
            get
            {
                return _parentMemberID;
            }
            set
            {
                _parentMemberID = value;
                UpdateFieldValue("ParentMemberID", value);
            }
        }
        
        public string MemberInfo1NameOfMember
        {
            get
            {
                return _memberInfo1NameOfMember;
            }
            set
            {
                _memberInfo1NameOfMember = value;
                UpdateFieldValue("MemberInfo1NameOfMember", value);
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
        
        public string FatherName
        {
            get
            {
                return _fatherName;
            }
            set
            {
                _fatherName = value;
                UpdateFieldValue("FatherName", value);
            }
        }
        
        public DateTime? SubsriptionDate
        {
            get
            {
                return _subsriptionDate;
            }
            set
            {
                _subsriptionDate = value;
                UpdateFieldValue("SubsriptionDate", value);
            }
        }
        
        public int? GenderID
        {
            get
            {
                return _genderID;
            }
            set
            {
                _genderID = value;
                UpdateFieldValue("GenderID", value);
            }
        }
        
        public string GenderDesc
        {
            get
            {
                return _genderDesc;
            }
            set
            {
                _genderDesc = value;
                UpdateFieldValue("GenderDesc", value);
            }
        }
        
        public int? NationalityID
        {
            get
            {
                return _nationalityID;
            }
            set
            {
                _nationalityID = value;
                UpdateFieldValue("NationalityID", value);
            }
        }
        
        public string Nationality
        {
            get
            {
                return _nationality;
            }
            set
            {
                _nationality = value;
                UpdateFieldValue("Nationality", value);
            }
        }
        
        public string Nid
        {
            get
            {
                return _nid;
            }
            set
            {
                _nid = value;
                UpdateFieldValue("Nid", value);
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
        
        public DateTime? Dob
        {
            get
            {
                return _dob;
            }
            set
            {
                _dob = value;
                UpdateFieldValue("Dob", value);
            }
        }
        
        public int? BloodGroupID
        {
            get
            {
                return _bloodGroupID;
            }
            set
            {
                _bloodGroupID = value;
                UpdateFieldValue("BloodGroupID", value);
            }
        }
        
        public string BloodGroupName
        {
            get
            {
                return _bloodGroupName;
            }
            set
            {
                _bloodGroupName = value;
                UpdateFieldValue("BloodGroupName", value);
            }
        }
        
        public decimal? MemberFee
        {
            get
            {
                return _memberFee;
            }
            set
            {
                _memberFee = value;
                UpdateFieldValue("MemberFee", value);
            }
        }
        
        public decimal? MonthlySubcriptionFee
        {
            get
            {
                return _monthlySubcriptionFee;
            }
            set
            {
                _monthlySubcriptionFee = value;
                UpdateFieldValue("MonthlySubcriptionFee", value);
            }
        }
        
        public decimal? MonthlyDonation
        {
            get
            {
                return _monthlyDonation;
            }
            set
            {
                _monthlyDonation = value;
                UpdateFieldValue("MonthlyDonation", value);
            }
        }
        
        public string PresentAddress
        {
            get
            {
                return _presentAddress;
            }
            set
            {
                _presentAddress = value;
                UpdateFieldValue("PresentAddress", value);
            }
        }
        
        public string PermanentAddress
        {
            get
            {
                return _permanentAddress;
            }
            set
            {
                _permanentAddress = value;
                UpdateFieldValue("PermanentAddress", value);
            }
        }
        
        public string Fax
        {
            get
            {
                return _fax;
            }
            set
            {
                _fax = value;
                UpdateFieldValue("Fax", value);
            }
        }
        
        public string CellPhone
        {
            get
            {
                return _cellPhone;
            }
            set
            {
                _cellPhone = value;
                UpdateFieldValue("CellPhone", value);
            }
        }
        
        public string LandPhone
        {
            get
            {
                return _landPhone;
            }
            set
            {
                _landPhone = value;
                UpdateFieldValue("LandPhone", value);
            }
        }
        
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                UpdateFieldValue("Email", value);
            }
        }
        
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                UpdateFieldValue("FileName", value);
            }
        }
        
        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
                UpdateFieldValue("ContentType", value);
            }
        }
        
        public int? Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
                UpdateFieldValue("Length", value);
            }
        }
        
        public bool? IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                UpdateFieldValue("IsActive", value);
            }
        }
        
        public int? MemberAge
        {
            get
            {
                return _memberAge;
            }
            set
            {
                _memberAge = value;
                UpdateFieldValue("MemberAge", value);
            }
        }
        
        public string MemberOfType
        {
            get
            {
                return _memberOfType;
            }
            set
            {
                _memberOfType = value;
                UpdateFieldValue("MemberOfType", value);
            }
        }
        
        public decimal? AnualFee
        {
            get
            {
                return _anualFee;
            }
            set
            {
                _anualFee = value;
                UpdateFieldValue("AnualFee", value);
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
        
        public string MemberGroupMemberGroupName
        {
            get
            {
                return _memberGroupMemberGroupName;
            }
            set
            {
                _memberGroupMemberGroupName = value;
                UpdateFieldValue("MemberGroupMemberGroupName", value);
            }
        }
        
        public int? MemberTypeID
        {
            get
            {
                return _memberTypeID;
            }
            set
            {
                _memberTypeID = value;
                UpdateFieldValue("MemberTypeID", value);
            }
        }
        
        public string MemberTypeMemberTypeDesc
        {
            get
            {
                return _memberTypeMemberTypeDesc;
            }
            set
            {
                _memberTypeMemberTypeDesc = value;
                UpdateFieldValue("MemberTypeMemberTypeDesc", value);
            }
        }
        
        public string Bgfid
        {
            get
            {
                return _bgfid;
            }
            set
            {
                _bgfid = value;
                UpdateFieldValue("Bgfid", value);
            }
        }
        
        public string Nfcid
        {
            get
            {
                return _nfcid;
            }
            set
            {
                _nfcid = value;
                UpdateFieldValue("Nfcid", value);
            }
        }
        
        public string BillingAddress
        {
            get
            {
                return _billingAddress;
            }
            set
            {
                _billingAddress = value;
                UpdateFieldValue("BillingAddress", value);
            }
        }
        
        public string ClubName
        {
            get
            {
                return _clubName;
            }
            set
            {
                _clubName = value;
                UpdateFieldValue("ClubName", value);
            }
        }
        
        public decimal? ContributionFee
        {
            get
            {
                return _contributionFee;
            }
            set
            {
                _contributionFee = value;
                UpdateFieldValue("ContributionFee", value);
            }
        }
        
        public int? NonMemberCatID
        {
            get
            {
                return _nonMemberCatID;
            }
            set
            {
                _nonMemberCatID = value;
                UpdateFieldValue("NonMemberCatID", value);
            }
        }
        
        public string NonMemberCategoryNonMemCateory
        {
            get
            {
                return _nonMemberCategoryNonMemCateory;
            }
            set
            {
                _nonMemberCategoryNonMemCateory = value;
                UpdateFieldValue("NonMemberCategoryNonMemCateory", value);
            }
        }
        
        public int? OcmnoOfDaysPlay
        {
            get
            {
                return _ocmnoOfDaysPlay;
            }
            set
            {
                _ocmnoOfDaysPlay = value;
                UpdateFieldValue("OcmnoOfDaysPlay", value);
            }
        }
        
        public DateTime? LastPaySubsDate
        {
            get
            {
                return _lastPaySubsDate;
            }
            set
            {
                _lastPaySubsDate = value;
                UpdateFieldValue("LastPaySubsDate", value);
            }
        }
        
        public DateTime? LastPayContDate
        {
            get
            {
                return _lastPayContDate;
            }
            set
            {
                _lastPayContDate = value;
                UpdateFieldValue("LastPayContDate", value);
            }
        }
        
        public string RefNumber
        {
            get
            {
                return _refNumber;
            }
            set
            {
                _refNumber = value;
                UpdateFieldValue("RefNumber", value);
            }
        }
        
        public string RecomendetMemberShipNo
        {
            get
            {
                return _recomendetMemberShipNo;
            }
            set
            {
                _recomendetMemberShipNo = value;
                UpdateFieldValue("RecomendetMemberShipNo", value);
            }
        }
        
        public int? OcmnoOfDaysPlayed
        {
            get
            {
                return _ocmnoOfDaysPlayed;
            }
            set
            {
                _ocmnoOfDaysPlayed = value;
                UpdateFieldValue("OcmnoOfDaysPlayed", value);
            }
        }
        
        public DateTime? WelfareContributionDT
        {
            get
            {
                return _welfareContributionDT;
            }
            set
            {
                _welfareContributionDT = value;
                UpdateFieldValue("WelfareContributionDT", value);
            }
        }
    }
}
