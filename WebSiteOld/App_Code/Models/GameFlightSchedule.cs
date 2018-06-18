using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class GameFlightScheduleModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _flightSchID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _flightSchNo;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _holeTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _holeTypeDesc;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _startTime;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _memberCount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _maxMemberCount;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _delayTime;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _gameDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _playerReg;
        
        public GameFlightScheduleModel()
        {
        }
        
        public GameFlightScheduleModel(BusinessRules r) : 
                base(r)
        {
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
        
        public int? FlightSchNo
        {
            get
            {
                return _flightSchNo;
            }
            set
            {
                _flightSchNo = value;
                UpdateFieldValue("FlightSchNo", value);
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
        
        public string StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                UpdateFieldValue("StartTime", value);
            }
        }
        
        public int? MemberCount
        {
            get
            {
                return _memberCount;
            }
            set
            {
                _memberCount = value;
                UpdateFieldValue("MemberCount", value);
            }
        }
        
        public int? MaxMemberCount
        {
            get
            {
                return _maxMemberCount;
            }
            set
            {
                _maxMemberCount = value;
                UpdateFieldValue("MaxMemberCount", value);
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
        
        public DateTime? GameDate
        {
            get
            {
                return _gameDate;
            }
            set
            {
                _gameDate = value;
                UpdateFieldValue("GameDate", value);
            }
        }
        
        public string PlayerReg
        {
            get
            {
                return _playerReg;
            }
            set
            {
                _playerReg = value;
                UpdateFieldValue("PlayerReg", value);
            }
        }
    }
}
