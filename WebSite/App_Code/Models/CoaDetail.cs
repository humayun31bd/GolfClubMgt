using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MyCompany.Data;

namespace MyCompany.Models
{
	public partial class CoaDetailModel : BusinessRulesObjectModel
    {
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _coaDetailID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accFundID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _accFundCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _acccode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _accName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _accDesc;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accDestID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accBalTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accCurrency;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _consTranCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _coaControlID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _coaSubID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _coaMainID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _tranCode;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _subsYn;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accSubTypeID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private DateTime? _openDate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _openDebit;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _openCredit;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _accCurrencyRate;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private byte? _revenue;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _variableCost;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _monthlyExp;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private decimal? _budget;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _annex;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _dlt;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isCashNature;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool? _isBankNature;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accSubTypeID1;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accSubTypeID2;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankAccNum;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _branchName;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _bankAccType;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _accTypeSetupID;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _accNameBg;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _columnSerail;
        
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int? _slno;
        
        public CoaDetailModel()
        {
        }
        
        public CoaDetailModel(BusinessRules r) : 
                base(r)
        {
        }
        
        [System.ComponentModel.DataObjectField(true, true, false)]
        public int? CoaDetailID
        {
            get
            {
                return _coaDetailID;
            }
            set
            {
                _coaDetailID = value;
                UpdateFieldValue("CoaDetailID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccFundID
        {
            get
            {
                return _accFundID;
            }
            set
            {
                _accFundID = value;
                UpdateFieldValue("AccFundID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string AccFundCode
        {
            get
            {
                return _accFundCode;
            }
            set
            {
                _accFundCode = value;
                UpdateFieldValue("AccFundCode", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string Acccode
        {
            get
            {
                return _acccode;
            }
            set
            {
                _acccode = value;
                UpdateFieldValue("Acccode", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, false)]
        public string AccName
        {
            get
            {
                return _accName;
            }
            set
            {
                _accName = value;
                UpdateFieldValue("AccName", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string AccDesc
        {
            get
            {
                return _accDesc;
            }
            set
            {
                _accDesc = value;
                UpdateFieldValue("AccDesc", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccTypeID
        {
            get
            {
                return _accTypeID;
            }
            set
            {
                _accTypeID = value;
                UpdateFieldValue("AccTypeID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccDestID
        {
            get
            {
                return _accDestID;
            }
            set
            {
                _accDestID = value;
                UpdateFieldValue("AccDestID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccBalTypeID
        {
            get
            {
                return _accBalTypeID;
            }
            set
            {
                _accBalTypeID = value;
                UpdateFieldValue("AccBalTypeID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccCurrency
        {
            get
            {
                return _accCurrency;
            }
            set
            {
                _accCurrency = value;
                UpdateFieldValue("AccCurrency", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string ConsTranCode
        {
            get
            {
                return _consTranCode;
            }
            set
            {
                _consTranCode = value;
                UpdateFieldValue("ConsTranCode", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? CoaControlID
        {
            get
            {
                return _coaControlID;
            }
            set
            {
                _coaControlID = value;
                UpdateFieldValue("CoaControlID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? CoaSubID
        {
            get
            {
                return _coaSubID;
            }
            set
            {
                _coaSubID = value;
                UpdateFieldValue("CoaSubID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? CoaMainID
        {
            get
            {
                return _coaMainID;
            }
            set
            {
                _coaMainID = value;
                UpdateFieldValue("CoaMainID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string TranCode
        {
            get
            {
                return _tranCode;
            }
            set
            {
                _tranCode = value;
                UpdateFieldValue("TranCode", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public bool? SubsYn
        {
            get
            {
                return _subsYn;
            }
            set
            {
                _subsYn = value;
                UpdateFieldValue("SubsYn", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccSubTypeID
        {
            get
            {
                return _accSubTypeID;
            }
            set
            {
                _accSubTypeID = value;
                UpdateFieldValue("AccSubTypeID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public DateTime? OpenDate
        {
            get
            {
                return _openDate;
            }
            set
            {
                _openDate = value;
                UpdateFieldValue("OpenDate", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public decimal? OpenDebit
        {
            get
            {
                return _openDebit;
            }
            set
            {
                _openDebit = value;
                UpdateFieldValue("OpenDebit", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public decimal? OpenCredit
        {
            get
            {
                return _openCredit;
            }
            set
            {
                _openCredit = value;
                UpdateFieldValue("OpenCredit", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public decimal? AccCurrencyRate
        {
            get
            {
                return _accCurrencyRate;
            }
            set
            {
                _accCurrencyRate = value;
                UpdateFieldValue("AccCurrencyRate", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public byte? Revenue
        {
            get
            {
                return _revenue;
            }
            set
            {
                _revenue = value;
                UpdateFieldValue("Revenue", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public bool? VariableCost
        {
            get
            {
                return _variableCost;
            }
            set
            {
                _variableCost = value;
                UpdateFieldValue("VariableCost", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public bool? MonthlyExp
        {
            get
            {
                return _monthlyExp;
            }
            set
            {
                _monthlyExp = value;
                UpdateFieldValue("MonthlyExp", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public decimal? Budget
        {
            get
            {
                return _budget;
            }
            set
            {
                _budget = value;
                UpdateFieldValue("Budget", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string Annex
        {
            get
            {
                return _annex;
            }
            set
            {
                _annex = value;
                UpdateFieldValue("Annex", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string Dlt
        {
            get
            {
                return _dlt;
            }
            set
            {
                _dlt = value;
                UpdateFieldValue("Dlt", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public bool? IsCashNature
        {
            get
            {
                return _isCashNature;
            }
            set
            {
                _isCashNature = value;
                UpdateFieldValue("IsCashNature", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public bool? IsBankNature
        {
            get
            {
                return _isBankNature;
            }
            set
            {
                _isBankNature = value;
                UpdateFieldValue("IsBankNature", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccSubTypeID1
        {
            get
            {
                return _accSubTypeID1;
            }
            set
            {
                _accSubTypeID1 = value;
                UpdateFieldValue("AccSubTypeID1", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccSubTypeID2
        {
            get
            {
                return _accSubTypeID2;
            }
            set
            {
                _accSubTypeID2 = value;
                UpdateFieldValue("AccSubTypeID2", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
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
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string BankAccNum
        {
            get
            {
                return _bankAccNum;
            }
            set
            {
                _bankAccNum = value;
                UpdateFieldValue("BankAccNum", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string BranchName
        {
            get
            {
                return _branchName;
            }
            set
            {
                _branchName = value;
                UpdateFieldValue("BranchName", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string BankAccType
        {
            get
            {
                return _bankAccType;
            }
            set
            {
                _bankAccType = value;
                UpdateFieldValue("BankAccType", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? AccTypeSetupID
        {
            get
            {
                return _accTypeSetupID;
            }
            set
            {
                _accTypeSetupID = value;
                UpdateFieldValue("AccTypeSetupID", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public string AccNameBg
        {
            get
            {
                return _accNameBg;
            }
            set
            {
                _accNameBg = value;
                UpdateFieldValue("AccNameBg", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? ColumnSerail
        {
            get
            {
                return _columnSerail;
            }
            set
            {
                _columnSerail = value;
                UpdateFieldValue("ColumnSerail", value);
            }
        }
        
        [System.ComponentModel.DataObjectField(false, false, true)]
        public int? Slno
        {
            get
            {
                return _slno;
            }
            set
            {
                _slno = value;
                UpdateFieldValue("Slno", value);
            }
        }
    }
    
    [System.ComponentModel.DataObject(false)]
    public partial class CoaDetail : CoaDetailModel
    {
        
        public static List<MyCompany.Models.CoaDetail> Select(
                    int? coaDetailID, 
                    int? accFundID, 
                    string accFundCode, 
                    string acccode, 
                    string accName, 
                    string accDesc, 
                    int? accTypeID, 
                    int? accDestID, 
                    int? accBalTypeID, 
                    int? accCurrency, 
                    string consTranCode, 
                    int? coaControlID, 
                    int? coaSubID, 
                    int? coaMainID, 
                    string tranCode, 
                    bool? subsYn, 
                    int? accSubTypeID, 
                    DateTime? openDate, 
                    decimal? openDebit, 
                    decimal? openCredit, 
                    decimal? accCurrencyRate, 
                    byte? revenue, 
                    bool? variableCost, 
                    bool? monthlyExp, 
                    decimal? budget, 
                    string annex, 
                    string dlt, 
                    bool? isCashNature, 
                    bool? isBankNature, 
                    int? accSubTypeID1, 
                    int? accSubTypeID2, 
                    string bankName, 
                    string bankAccNum, 
                    string branchName, 
                    string bankAccType, 
                    int? accTypeSetupID, 
                    string accNameBg, 
                    int? columnSerail, 
                    int? slno)
        {
            return new CoaDetailFactory().Select(coaDetailID, accFundID, accFundCode, acccode, accName, accDesc, accTypeID, accDestID, accBalTypeID, accCurrency, consTranCode, coaControlID, coaSubID, coaMainID, tranCode, subsYn, accSubTypeID, openDate, openDebit, openCredit, accCurrencyRate, revenue, variableCost, monthlyExp, budget, annex, dlt, isCashNature, isBankNature, accSubTypeID1, accSubTypeID2, bankName, bankAccNum, branchName, bankAccType, accTypeSetupID, accNameBg, columnSerail, slno);
        }
        
        public static List<MyCompany.Models.CoaDetail> Select(string filter, string sort, string dataView, params System.Object[] parameters)
        {
            return new CoaDetailFactory().Select(filter, sort, dataView, new BusinessObjectParameters(parameters));
        }
        
        public static List<MyCompany.Models.CoaDetail> Select(string filter, string sort, params System.Object[] parameters)
        {
            return new CoaDetailFactory().Select(filter, sort, CoaDetailFactory.SelectView, new BusinessObjectParameters(parameters));
        }
        
        public static List<MyCompany.Models.CoaDetail> Select(string filter, params System.Object[] parameters)
        {
            return new CoaDetailFactory().Select(filter, null, CoaDetailFactory.SelectView, new BusinessObjectParameters(parameters));
        }
        
        public static MyCompany.Models.CoaDetail SelectSingle(string filter, params System.Object[] parameters)
        {
            return new CoaDetailFactory().SelectSingle(filter, new BusinessObjectParameters(parameters));
        }
        
        public static MyCompany.Models.CoaDetail SelectSingle(int? coaDetailID)
        {
            return new CoaDetailFactory().SelectSingle(coaDetailID);
        }
        
        public int Insert()
        {
            return new CoaDetailFactory().Insert(this);
        }
        
        public int Update()
        {
            return new CoaDetailFactory().Update(this);
        }
        
        public int Delete()
        {
            return new CoaDetailFactory().Delete(this);
        }
        
        public override string ToString()
        {
            return String.Format("CoaDetailID: {0}", this.CoaDetailID);
        }
    }
    
    [System.ComponentModel.DataObject(true)]
    public partial class CoaDetailFactory
    {
        
        public CoaDetailFactory()
        {
        }
        
        public static string SelectView
        {
            get
            {
                return Controller.GetSelectView("CoaDetail");
            }
        }
        
        public static string InsertView
        {
            get
            {
                return Controller.GetInsertView("CoaDetail");
            }
        }
        
        public static string UpdateView
        {
            get
            {
                return Controller.GetUpdateView("CoaDetail");
            }
        }
        
        public static string DeleteView
        {
            get
            {
                return Controller.GetDeleteView("CoaDetail");
            }
        }
        
        public static CoaDetailFactory Create()
        {
            return new CoaDetailFactory();
        }
        
        protected virtual PageRequest CreateRequest(
                    int? coaDetailID, 
                    int? accFundID, 
                    string accFundCode, 
                    string acccode, 
                    string accName, 
                    string accDesc, 
                    int? accTypeID, 
                    int? accDestID, 
                    int? accBalTypeID, 
                    int? accCurrency, 
                    string consTranCode, 
                    int? coaControlID, 
                    int? coaSubID, 
                    int? coaMainID, 
                    string tranCode, 
                    bool? subsYn, 
                    int? accSubTypeID, 
                    DateTime? openDate, 
                    decimal? openDebit, 
                    decimal? openCredit, 
                    decimal? accCurrencyRate, 
                    byte? revenue, 
                    bool? variableCost, 
                    bool? monthlyExp, 
                    decimal? budget, 
                    string annex, 
                    string dlt, 
                    bool? isCashNature, 
                    bool? isBankNature, 
                    int? accSubTypeID1, 
                    int? accSubTypeID2, 
                    string bankName, 
                    string bankAccNum, 
                    string branchName, 
                    string bankAccType, 
                    int? accTypeSetupID, 
                    string accNameBg, 
                    int? columnSerail, 
                    int? slno, 
                    string sort, 
                    int maximumRows, 
                    int startRowIndex)
        {
            List<string> filter = new List<string>();
            if (coaDetailID.HasValue)
            	filter.Add(("CoaDetailID:=" + coaDetailID.Value.ToString()));
            if (accFundID.HasValue)
            	filter.Add(("AccFundID:=" + accFundID.Value.ToString()));
            if (!(String.IsNullOrEmpty(accFundCode)))
            	filter.Add(("AccFundCode:*" + accFundCode));
            if (!(String.IsNullOrEmpty(acccode)))
            	filter.Add(("Acccode:*" + acccode));
            if (!(String.IsNullOrEmpty(accName)))
            	filter.Add(("AccName:*" + accName));
            if (!(String.IsNullOrEmpty(accDesc)))
            	filter.Add(("AccDesc:*" + accDesc));
            if (accTypeID.HasValue)
            	filter.Add(("AccTypeID:=" + accTypeID.Value.ToString()));
            if (accDestID.HasValue)
            	filter.Add(("AccDestID:=" + accDestID.Value.ToString()));
            if (accBalTypeID.HasValue)
            	filter.Add(("AccBalTypeID:=" + accBalTypeID.Value.ToString()));
            if (accCurrency.HasValue)
            	filter.Add(("AccCurrency:=" + accCurrency.Value.ToString()));
            if (!(String.IsNullOrEmpty(consTranCode)))
            	filter.Add(("ConsTranCode:*" + consTranCode));
            if (coaControlID.HasValue)
            	filter.Add(("CoaControlID:=" + coaControlID.Value.ToString()));
            if (coaSubID.HasValue)
            	filter.Add(("CoaSubID:=" + coaSubID.Value.ToString()));
            if (coaMainID.HasValue)
            	filter.Add(("CoaMainID:=" + coaMainID.Value.ToString()));
            if (!(String.IsNullOrEmpty(tranCode)))
            	filter.Add(("TranCode:*" + tranCode));
            if (subsYn.HasValue)
            	filter.Add(("SubsYn:=" + subsYn.Value.ToString()));
            if (accSubTypeID.HasValue)
            	filter.Add(("AccSubTypeID:=" + accSubTypeID.Value.ToString()));
            if (openDate.HasValue)
            	filter.Add(("OpenDate:=" + openDate.Value.ToString()));
            if (openDebit.HasValue)
            	filter.Add(("OpenDebit:=" + openDebit.Value.ToString()));
            if (openCredit.HasValue)
            	filter.Add(("OpenCredit:=" + openCredit.Value.ToString()));
            if (accCurrencyRate.HasValue)
            	filter.Add(("AccCurrencyRate:=" + accCurrencyRate.Value.ToString()));
            if (revenue.HasValue)
            	filter.Add(("Revenue:=" + revenue.Value.ToString()));
            if (variableCost.HasValue)
            	filter.Add(("VariableCost:=" + variableCost.Value.ToString()));
            if (monthlyExp.HasValue)
            	filter.Add(("MonthlyExp:=" + monthlyExp.Value.ToString()));
            if (budget.HasValue)
            	filter.Add(("Budget:=" + budget.Value.ToString()));
            if (!(String.IsNullOrEmpty(annex)))
            	filter.Add(("Annex:*" + annex));
            if (!(String.IsNullOrEmpty(dlt)))
            	filter.Add(("Dlt:*" + dlt));
            if (isCashNature.HasValue)
            	filter.Add(("IsCashNature:=" + isCashNature.Value.ToString()));
            if (isBankNature.HasValue)
            	filter.Add(("IsBankNature:=" + isBankNature.Value.ToString()));
            if (accSubTypeID1.HasValue)
            	filter.Add(("AccSubTypeID1:=" + accSubTypeID1.Value.ToString()));
            if (accSubTypeID2.HasValue)
            	filter.Add(("AccSubTypeID2:=" + accSubTypeID2.Value.ToString()));
            if (!(String.IsNullOrEmpty(bankName)))
            	filter.Add(("BankName:*" + bankName));
            if (!(String.IsNullOrEmpty(bankAccNum)))
            	filter.Add(("BankAccNum:*" + bankAccNum));
            if (!(String.IsNullOrEmpty(branchName)))
            	filter.Add(("BranchName:*" + branchName));
            if (!(String.IsNullOrEmpty(bankAccType)))
            	filter.Add(("BankAccType:*" + bankAccType));
            if (accTypeSetupID.HasValue)
            	filter.Add(("AccTypeSetupID:=" + accTypeSetupID.Value.ToString()));
            if (!(String.IsNullOrEmpty(accNameBg)))
            	filter.Add(("AccNameBg:*" + accNameBg));
            if (columnSerail.HasValue)
            	filter.Add(("ColumnSerail:=" + columnSerail.Value.ToString()));
            if (slno.HasValue)
            	filter.Add(("Slno:=" + slno.Value.ToString()));
            PageRequest request = new PageRequest((startRowIndex / maximumRows), maximumRows, sort, filter.ToArray());
            request.MetadataFilter = new string[] {
                    "fields"};
            return request;
        }
        
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public List<MyCompany.Models.CoaDetail> Select(
                    int? coaDetailID, 
                    int? accFundID, 
                    string accFundCode, 
                    string acccode, 
                    string accName, 
                    string accDesc, 
                    int? accTypeID, 
                    int? accDestID, 
                    int? accBalTypeID, 
                    int? accCurrency, 
                    string consTranCode, 
                    int? coaControlID, 
                    int? coaSubID, 
                    int? coaMainID, 
                    string tranCode, 
                    bool? subsYn, 
                    int? accSubTypeID, 
                    DateTime? openDate, 
                    decimal? openDebit, 
                    decimal? openCredit, 
                    decimal? accCurrencyRate, 
                    byte? revenue, 
                    bool? variableCost, 
                    bool? monthlyExp, 
                    decimal? budget, 
                    string annex, 
                    string dlt, 
                    bool? isCashNature, 
                    bool? isBankNature, 
                    int? accSubTypeID1, 
                    int? accSubTypeID2, 
                    string bankName, 
                    string bankAccNum, 
                    string branchName, 
                    string bankAccType, 
                    int? accTypeSetupID, 
                    string accNameBg, 
                    int? columnSerail, 
                    int? slno, 
                    string sort, 
                    int maximumRows, 
                    int startRowIndex, 
                    string dataView)
        {
            PageRequest request = CreateRequest(coaDetailID, accFundID, accFundCode, acccode, accName, accDesc, accTypeID, accDestID, accBalTypeID, accCurrency, consTranCode, coaControlID, coaSubID, coaMainID, tranCode, subsYn, accSubTypeID, openDate, openDebit, openCredit, accCurrencyRate, revenue, variableCost, monthlyExp, budget, annex, dlt, isCashNature, isBankNature, accSubTypeID1, accSubTypeID2, bankName, bankAccNum, branchName, bankAccType, accTypeSetupID, accNameBg, columnSerail, slno, sort, maximumRows, startRowIndex);
            request.RequiresMetaData = true;
            request.MetadataFilter = new string[] {
                    "fields"};
            ViewPage page = ControllerFactory.CreateDataController().GetPage("CoaDetail", dataView, request);
            return page.ToList<MyCompany.Models.CoaDetail>();
        }
        
        public int SelectCount(
                    int? coaDetailID, 
                    int? accFundID, 
                    string accFundCode, 
                    string acccode, 
                    string accName, 
                    string accDesc, 
                    int? accTypeID, 
                    int? accDestID, 
                    int? accBalTypeID, 
                    int? accCurrency, 
                    string consTranCode, 
                    int? coaControlID, 
                    int? coaSubID, 
                    int? coaMainID, 
                    string tranCode, 
                    bool? subsYn, 
                    int? accSubTypeID, 
                    DateTime? openDate, 
                    decimal? openDebit, 
                    decimal? openCredit, 
                    decimal? accCurrencyRate, 
                    byte? revenue, 
                    bool? variableCost, 
                    bool? monthlyExp, 
                    decimal? budget, 
                    string annex, 
                    string dlt, 
                    bool? isCashNature, 
                    bool? isBankNature, 
                    int? accSubTypeID1, 
                    int? accSubTypeID2, 
                    string bankName, 
                    string bankAccNum, 
                    string branchName, 
                    string bankAccType, 
                    int? accTypeSetupID, 
                    string accNameBg, 
                    int? columnSerail, 
                    int? slno, 
                    string sort, 
                    int maximumRows, 
                    int startRowIndex, 
                    string dataView)
        {
            PageRequest request = CreateRequest(coaDetailID, accFundID, accFundCode, acccode, accName, accDesc, accTypeID, accDestID, accBalTypeID, accCurrency, consTranCode, coaControlID, coaSubID, coaMainID, tranCode, subsYn, accSubTypeID, openDate, openDebit, openCredit, accCurrencyRate, revenue, variableCost, monthlyExp, budget, annex, dlt, isCashNature, isBankNature, accSubTypeID1, accSubTypeID2, bankName, bankAccNum, branchName, bankAccType, accTypeSetupID, accNameBg, columnSerail, slno, sort, -1, startRowIndex);
            request.RequiresMetaData = false;
            request.MetadataFilter = new string[] {
                    "fields"};
            request.RequiresRowCount = true;
            ViewPage page = ControllerFactory.CreateDataController().GetPage("CoaDetail", dataView, request);
            return page.TotalRowCount;
        }
        
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select, true)]
        public List<MyCompany.Models.CoaDetail> Select(
                    int? coaDetailID, 
                    int? accFundID, 
                    string accFundCode, 
                    string acccode, 
                    string accName, 
                    string accDesc, 
                    int? accTypeID, 
                    int? accDestID, 
                    int? accBalTypeID, 
                    int? accCurrency, 
                    string consTranCode, 
                    int? coaControlID, 
                    int? coaSubID, 
                    int? coaMainID, 
                    string tranCode, 
                    bool? subsYn, 
                    int? accSubTypeID, 
                    DateTime? openDate, 
                    decimal? openDebit, 
                    decimal? openCredit, 
                    decimal? accCurrencyRate, 
                    byte? revenue, 
                    bool? variableCost, 
                    bool? monthlyExp, 
                    decimal? budget, 
                    string annex, 
                    string dlt, 
                    bool? isCashNature, 
                    bool? isBankNature, 
                    int? accSubTypeID1, 
                    int? accSubTypeID2, 
                    string bankName, 
                    string bankAccNum, 
                    string branchName, 
                    string bankAccType, 
                    int? accTypeSetupID, 
                    string accNameBg, 
                    int? columnSerail, 
                    int? slno)
        {
            return Select(coaDetailID, accFundID, accFundCode, acccode, accName, accDesc, accTypeID, accDestID, accBalTypeID, accCurrency, consTranCode, coaControlID, coaSubID, coaMainID, tranCode, subsYn, accSubTypeID, openDate, openDebit, openCredit, accCurrencyRate, revenue, variableCost, monthlyExp, budget, annex, dlt, isCashNature, isBankNature, accSubTypeID1, accSubTypeID2, bankName, bankAccNum, branchName, bankAccType, accTypeSetupID, accNameBg, columnSerail, slno, null, Int32.MaxValue, 0, SelectView);
        }
        
        public List<MyCompany.Models.CoaDetail> Select(MyCompany.Models.CoaDetail qbe)
        {
            return Select(qbe.CoaDetailID, qbe.AccFundID, qbe.AccFundCode, qbe.Acccode, qbe.AccName, qbe.AccDesc, qbe.AccTypeID, qbe.AccDestID, qbe.AccBalTypeID, qbe.AccCurrency, qbe.ConsTranCode, qbe.CoaControlID, qbe.CoaSubID, qbe.CoaMainID, qbe.TranCode, qbe.SubsYn, qbe.AccSubTypeID, qbe.OpenDate, qbe.OpenDebit, qbe.OpenCredit, qbe.AccCurrencyRate, qbe.Revenue, qbe.VariableCost, qbe.MonthlyExp, qbe.Budget, qbe.Annex, qbe.Dlt, qbe.IsCashNature, qbe.IsBankNature, qbe.AccSubTypeID1, qbe.AccSubTypeID2, qbe.BankName, qbe.BankAccNum, qbe.BranchName, qbe.BankAccType, qbe.AccTypeSetupID, qbe.AccNameBg, qbe.ColumnSerail, qbe.Slno);
        }
        
        public List<MyCompany.Models.CoaDetail> Select(string filter, BusinessObjectParameters parameters)
        {
            return Select(filter, null, SelectView, parameters);
        }
        
        public List<MyCompany.Models.CoaDetail> SelectSingle(string filter, string sort, BusinessObjectParameters parameters)
        {
            return Select(filter, sort, SelectView, parameters);
        }
        
        public List<MyCompany.Models.CoaDetail> Select(string filter, string sort, string dataView, BusinessObjectParameters parameters)
        {
            PageRequest request = new PageRequest(0, Int32.MaxValue, sort, new string[0]);
            request.RequiresMetaData = true;
            request.MetadataFilter = new string[] {
                    "fields"};
            IDataController c = ControllerFactory.CreateDataController();
            IBusinessObject bo = ((IBusinessObject)(c));
            bo.AssignFilter(filter, parameters);
            ViewPage page = c.GetPage("CoaDetail", dataView, request);
            return page.ToList<MyCompany.Models.CoaDetail>();
        }
        
        public MyCompany.Models.CoaDetail SelectSingle(int? coaDetailID)
        {
            List<MyCompany.Models.CoaDetail> list = Select(coaDetailID, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            if (list.Count == 0)
            	return null;
            return list[0];
        }
        
        public MyCompany.Models.CoaDetail SelectSingle(string filter, BusinessObjectParameters parameters)
        {
            List<MyCompany.Models.CoaDetail> list = Select(filter, parameters);
            if (list.Count > 0)
            	return list[0];
            return null;
        }
        
        protected virtual FieldValue[] CreateFieldValues(MyCompany.Models.CoaDetail theCoaDetail, MyCompany.Models.CoaDetail original_CoaDetail)
        {
            List<FieldValue> values = new List<FieldValue>();
            values.Add(new FieldValue("CoaDetailID", original_CoaDetail.CoaDetailID, theCoaDetail.CoaDetailID, true));
            values.Add(new FieldValue("AccFundID", original_CoaDetail.AccFundID, theCoaDetail.AccFundID));
            values.Add(new FieldValue("AccFundCode", original_CoaDetail.AccFundCode, theCoaDetail.AccFundCode));
            values.Add(new FieldValue("Acccode", original_CoaDetail.Acccode, theCoaDetail.Acccode));
            values.Add(new FieldValue("AccName", original_CoaDetail.AccName, theCoaDetail.AccName));
            values.Add(new FieldValue("AccDesc", original_CoaDetail.AccDesc, theCoaDetail.AccDesc));
            values.Add(new FieldValue("AccTypeID", original_CoaDetail.AccTypeID, theCoaDetail.AccTypeID));
            values.Add(new FieldValue("AccDestID", original_CoaDetail.AccDestID, theCoaDetail.AccDestID));
            values.Add(new FieldValue("AccBalTypeID", original_CoaDetail.AccBalTypeID, theCoaDetail.AccBalTypeID));
            values.Add(new FieldValue("AccCurrency", original_CoaDetail.AccCurrency, theCoaDetail.AccCurrency));
            values.Add(new FieldValue("ConsTranCode", original_CoaDetail.ConsTranCode, theCoaDetail.ConsTranCode));
            values.Add(new FieldValue("CoaControlID", original_CoaDetail.CoaControlID, theCoaDetail.CoaControlID));
            values.Add(new FieldValue("CoaSubID", original_CoaDetail.CoaSubID, theCoaDetail.CoaSubID));
            values.Add(new FieldValue("CoaMainID", original_CoaDetail.CoaMainID, theCoaDetail.CoaMainID));
            values.Add(new FieldValue("TranCode", original_CoaDetail.TranCode, theCoaDetail.TranCode));
            values.Add(new FieldValue("SubsYn", original_CoaDetail.SubsYn, theCoaDetail.SubsYn));
            values.Add(new FieldValue("AccSubTypeID", original_CoaDetail.AccSubTypeID, theCoaDetail.AccSubTypeID));
            values.Add(new FieldValue("OpenDate", original_CoaDetail.OpenDate, theCoaDetail.OpenDate));
            values.Add(new FieldValue("OpenDebit", original_CoaDetail.OpenDebit, theCoaDetail.OpenDebit));
            values.Add(new FieldValue("OpenCredit", original_CoaDetail.OpenCredit, theCoaDetail.OpenCredit));
            values.Add(new FieldValue("AccCurrencyRate", original_CoaDetail.AccCurrencyRate, theCoaDetail.AccCurrencyRate));
            values.Add(new FieldValue("Revenue", original_CoaDetail.Revenue, theCoaDetail.Revenue));
            values.Add(new FieldValue("VariableCost", original_CoaDetail.VariableCost, theCoaDetail.VariableCost));
            values.Add(new FieldValue("MonthlyExp", original_CoaDetail.MonthlyExp, theCoaDetail.MonthlyExp));
            values.Add(new FieldValue("Budget", original_CoaDetail.Budget, theCoaDetail.Budget));
            values.Add(new FieldValue("Annex", original_CoaDetail.Annex, theCoaDetail.Annex));
            values.Add(new FieldValue("Dlt", original_CoaDetail.Dlt, theCoaDetail.Dlt));
            values.Add(new FieldValue("IsCashNature", original_CoaDetail.IsCashNature, theCoaDetail.IsCashNature));
            values.Add(new FieldValue("IsBankNature", original_CoaDetail.IsBankNature, theCoaDetail.IsBankNature));
            values.Add(new FieldValue("AccSubTypeID1", original_CoaDetail.AccSubTypeID1, theCoaDetail.AccSubTypeID1));
            values.Add(new FieldValue("AccSubTypeID2", original_CoaDetail.AccSubTypeID2, theCoaDetail.AccSubTypeID2));
            values.Add(new FieldValue("BankName", original_CoaDetail.BankName, theCoaDetail.BankName));
            values.Add(new FieldValue("BankAccNum", original_CoaDetail.BankAccNum, theCoaDetail.BankAccNum));
            values.Add(new FieldValue("BranchName", original_CoaDetail.BranchName, theCoaDetail.BranchName));
            values.Add(new FieldValue("BankAccType", original_CoaDetail.BankAccType, theCoaDetail.BankAccType));
            values.Add(new FieldValue("AccTypeSetupID", original_CoaDetail.AccTypeSetupID, theCoaDetail.AccTypeSetupID));
            values.Add(new FieldValue("AccNameBg", original_CoaDetail.AccNameBg, theCoaDetail.AccNameBg));
            values.Add(new FieldValue("ColumnSerail", original_CoaDetail.ColumnSerail, theCoaDetail.ColumnSerail));
            values.Add(new FieldValue("Slno", original_CoaDetail.Slno, theCoaDetail.Slno));
            return values.ToArray();
        }
        
        protected virtual int ExecuteAction(MyCompany.Models.CoaDetail theCoaDetail, MyCompany.Models.CoaDetail original_CoaDetail, string lastCommandName, string commandName, string dataView)
        {
            ActionArgs args = new ActionArgs();
            args.Controller = "CoaDetail";
            args.View = dataView;
            args.Values = CreateFieldValues(theCoaDetail, original_CoaDetail);
            args.LastCommandName = lastCommandName;
            args.CommandName = commandName;
            ActionResult result = ControllerFactory.CreateDataController().Execute("CoaDetail", dataView, args);
            result.RaiseExceptionIfErrors();
            result.AssignTo(theCoaDetail);
            return result.RowsAffected;
        }
        
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        public virtual int Update(MyCompany.Models.CoaDetail theCoaDetail, MyCompany.Models.CoaDetail original_CoaDetail)
        {
            return ExecuteAction(theCoaDetail, original_CoaDetail, "Edit", "Update", UpdateView);
        }
        
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update, true)]
        public virtual int Update(MyCompany.Models.CoaDetail theCoaDetail)
        {
            return Update(theCoaDetail, SelectSingle(theCoaDetail.CoaDetailID));
        }
        
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert, true)]
        public virtual int Insert(MyCompany.Models.CoaDetail theCoaDetail)
        {
            return ExecuteAction(theCoaDetail, new CoaDetail(), "New", "Insert", InsertView);
        }
        
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete, true)]
        public virtual int Delete(MyCompany.Models.CoaDetail theCoaDetail)
        {
            return ExecuteAction(theCoaDetail, theCoaDetail, "Select", "Delete", DeleteView);
        }
    }
}
