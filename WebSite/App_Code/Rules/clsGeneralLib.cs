using MyCompany.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for clsGeneralLib
/// </summary>
public class clsGeneralLib
{
	public clsGeneralLib()
	{
		//
		// TODO: Add constructor logic here
		//
    }
    public class Member
    {
        public int MemberID { get; set; }
        public string MemberCode { get; set; }
        public string MemberName { get; set; }
        public int MemberCategoryID { get; set; }
        public string MemberCategoryName { get; set; }
        public int MemberStatusID { get; set; }
        public string MemberStatusName { get; set; }
        public int MemberGroupID { get; set; }
        public string MemberGroupName { get; set; }
        public string MemberOfType { get; set; }
        public string MobileNo { get; set; }
        public int GameRegisterID { get; set; }
        public int TournamentRegisterID { get; set; }
        public int HandiCap { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public string[] FormatMobileNo(string mob)
    {
        string[] str_return = new string[2];
        string strPlus = "+";
        bool isPlus = mob.Contains(strPlus);
        if (isPlus == true)
        {
            int indexof = mob.IndexOf('+') + 1;
            string MinusPlus = mob.Substring(indexof);
            int moblength = MinusPlus.Length;
            if (moblength == 11)
            {
                MinusPlus = "88" + MinusPlus;
                str_return[0] = "0";
                str_return[1] = MinusPlus;
            }
            else if (moblength == 13)
            {
                str_return[0] = "0";
                str_return[1] = MinusPlus;
            }
            else
            {
                str_return[0] = "1";
                str_return[1] = MinusPlus;
            }
        }
        else
        {
            string MinusPlus = mob;
            int moblength = MinusPlus.Length;
            if (moblength == 10)
            {
                MinusPlus = "'880" + MinusPlus;
                str_return[0] = "0";
                str_return[1] = MinusPlus;
            }
            if (moblength == 11)
            {
                MinusPlus = "88" + MinusPlus;
                str_return[0] = "0";
                str_return[1] = MinusPlus;
            }
            else if (moblength == 13)
            {
                str_return[0] = "0";
                str_return[1] = MinusPlus;
            }
            else
            {
                str_return[0] = "1";
                str_return[1] = MinusPlus;
            }
        }
        return str_return;
    }
    public string connHelper(string sConnectionType, string sqlServer, string initialCatalog, string userId, string password)
    {
        string ssCon = "";
        if (sConnectionType != null)
        {
            if (sConnectionType == "SQLSERVER")
            {
                ssCon = (String.Format("Data Source={0};Connection Timeout=300;Initial Catalog={1};User Id={2};Password={3};", sqlServer, initialCatalog, userId, password));
            }
        }
        return ssCon;
    }

    public int SendSMSTo(string pMobileNO, string pMemberName, string pMessage)
    { 
        int isSendSMSSuccess = 100;
        #region [AppSetting]
        string debugSMSSttings = ConfigurationManager.AppSettings["debugSMS"].ToString();
        bool debugSMS = Convert.ToBoolean(debugSMSSttings);
        string StrProvider = ConfigurationManager.AppSettings["SMSProvider"].ToString();
        //string StrMasking = ConfigurationManager.AppSettings["SMSMaskName"].ToString();
        string SMSAccountName = ConfigurationManager.AppSettings["SMSAccountName"].ToString();
        //string SendSMSWithSchoolName = ConfigurationManager.AppSettings["SendSMSWithSchoolName"].ToString();
        //bool IsSchoolName = Convert.ToBoolean(SendSMSWithSchoolName);
        string StrSMSEndPoint = ConfigurationManager.AppSettings["SMSEndPoint"].ToString();

        string sIsSimulateSMS = ConfigurationManager.AppSettings["IsSimulateSMS"].ToString();
        bool IsSimulateSMS = Convert.ToBoolean(sIsSimulateSMS);

        //string SendSMSPresentAuto = ConfigurationManager.AppSettings["SendSMSPresentAuto"].ToString();
        //bool bSendSMSPresentAuto = Convert.ToBoolean(SendSMSPresentAuto);

        //if (!bSendSMSPresentAuto)
        //{
        //    return(isSendSMSSuccess);
        //}
        #endregion
        #region [default values]
        int providerid = 0;
        bool IsMaskingActive;
        string MaskName = string.Empty;
        int SMSLimit = 0;
        string SenderMobileNumber = string.Empty;
        //For balance update
        double SMSBalanceold = 0;
        int SMSPurchaseQtyold = 0;
        double SMSBalanceNew = 0;
        int SMSPurchaseQtyNew = 0;
        // end of balance update
        Boolean AllSmsSend = false;
        DateTime dAttendDate = DateTime.Now;
        //for present IT
        string smsto = string.Empty;
        string groupid = string.Empty;
        string groupname = string.Empty;
        string statusid = string.Empty;
        string statusname = string.Empty;
        string description = string.Empty;
        string smsCount = string.Empty;
        string messageId = string.Empty;
        string status = string.Empty;
        string statuscode = string.Empty;
        string statusDescription = string.Empty;
        // end 
        string strInsert = string.Empty;
        string strBalanceUpdate = string.Empty;
        string InstituteName = "";
        string HashAutor = string.Empty;

        AllSmsSend = false;
        string CloudSrv = ConfigurationManager.AppSettings["CloudServer"].ToString();
        CloudSrv = "45.35.4.235";
        string sConnectionType = ConfigurationManager.AppSettings["ConnectionType"].ToString();
        string cldSrvdb = ConfigurationManager.AppSettings["CloudServerdb"].ToString();
        SqlConnection connection = new SqlConnection();
        sConnectionType = "SQLSERVER";
        connection.ConnectionString = connHelper("SQLSERVER", CloudSrv, cldSrvdb, "MyCloudAdmin", "MyCloudAdmin@786!#");
        #endregion
        try
        {
            connection.Open();
        }
        catch (Exception ex)
        {
            isSendSMSSuccess = 101;
            /// Log Database connection failed
            throw new Exception("Database connection failed. Please try it later.");
            ///MessageBox.Show("Cloud Server Status : " + ex.Message);
        }
            
        #region [Reader Provider record]
        string strSelect = "select * from SMSProvider where SMSAccountName='" + SMSAccountName + "'";
        SqlCommand cmd1 = new SqlCommand();
        DataTable dtProvider = new DataTable();
        SqlDataAdapter daProvider = new SqlDataAdapter();
        cmd1.Connection = connection;
        cmd1.CommandType = CommandType.Text;
        cmd1.CommandText = strSelect;
        //cmd1.Transaction = Transection;
        daProvider.SelectCommand = cmd1;
        daProvider.Fill(dtProvider);
        if (dtProvider.Rows.Count > 0)
        {
            if (dtProvider.Rows[0]["IsMaskingActive"] != DBNull.Value)
            {
                IsMaskingActive = Convert.ToBoolean(dtProvider.Rows[0]["IsMaskingActive"]);
            }
            if (dtProvider.Rows[0]["SMSLimit"] != DBNull.Value)
            {
                SMSLimit = Convert.ToInt32(dtProvider.Rows[0]["SMSLimit"].ToString());
            }
            MaskName = dtProvider.Rows[0]["MaskName"].ToString();
            SenderMobileNumber = dtProvider.Rows[0]["SenderMobileNumber"].ToString();
            HashAutor = dtProvider.Rows[0]["SMSHashTag"].ToString();
        }
        #endregion
        #region [Get sms balance ]
        string strSMSBalance = "select * from SMSBalance where SMSAccountName='" + SMSAccountName + "'";
        cmd1 = new SqlCommand();
        DataTable dtSMSBalance = new DataTable();
        SqlDataAdapter daSMSBalance = new SqlDataAdapter();
        cmd1.Connection = connection;
        cmd1.CommandType = CommandType.Text;
        cmd1.CommandText = strSMSBalance;
        daSMSBalance.SelectCommand = cmd1;
        daSMSBalance.Fill(dtSMSBalance);

        if (dtSMSBalance.Rows.Count > 0)
        {
            if (dtSMSBalance.Rows[0]["SMSBalanceQty"] != DBNull.Value)
            {
                SMSBalanceold = Convert.ToDouble(dtSMSBalance.Rows[0]["SMSBalanceQty"]);
            }
            int NewSMSRequired = 3;//TeacherDB.Rows.Count;
            if (SMSBalanceold < NewSMSRequired)
            {
                isSendSMSSuccess = 102;
                ///MessageBox.Show("Balance is not eanough to send sms.....Please Purchase SMS....");
                return (isSendSMSSuccess);
            }
        }
        #endregion
        int TotalSMSDeducted = 0;
        // SendSMS("IPACSOFTWAR","8801912616994","test dynamic sms");
        #region [send sms to client]

        #region [variables]

        int rowsAffected3 = 0;
            int rowsAffected1 = 0;
                
                
            string SMSNoticeBody = string.Empty;
            string SmsSendDate = string.Empty;
            string MemberName = string.Empty;
            bool IsSensSMS = false;
                
            #endregion
            #region [Read Member Data]

            SMSNoticeBody = pMemberName + " : " + pMessage.ToString();

            #endregion
            if (connection.State == 0)
            {
                connection.Open();
            }
            try
            {
                if (IsSensSMS == false)
                {
                       
                    if (SMSBalanceold > 0)
                    {
                        #region [Enough balance to send sms]


                        string mobileno = pMobileNO;
                        string[] strmobile = FormatMobileNo(mobileno);
                        int SMSOutLogID = 0;
                        if (strmobile[0] == "0")
                        {
                            if (StrProvider == "PresentIT")
                            {
                                if (debugSMS == false)
                                {
                                    #region [PresentIT]


                                    cmd1 = new SqlCommand();
                                    if (connection.State == 0)
                                    {
                                        connection.Open();
                                    }
                                    cmd1.Connection = connection;
                                    if (SMSOutLogID == 0)
                                    {
                                        #region [insert record send sms]
                                        strInsert = "USP_InsertUpdateSMSDelevary";
                                        cmd1.Parameters.Add("@SMSOutLogID", SqlDbType.Int).Value = SMSOutLogID;
                                        cmd1.Parameters.Add("@ReceiptNumber", SqlDbType.NVarChar).Value = strmobile[1].ToString();
                                        cmd1.Parameters.Add("@SenderNumber", SqlDbType.NVarChar).Value = SenderMobileNumber;
                                        cmd1.Parameters.Add("@sMessage", SqlDbType.NVarChar).Value = SMSNoticeBody;
                                        cmd1.Parameters.Add("@delay", SqlDbType.Int).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@type", SqlDbType.Int).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@DeliveryStatusID", SqlDbType.Int).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@Result", SqlDbType.NVarChar).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@DeliveryTime", SqlDbType.DateTime).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@ProviderID", SqlDbType.Int).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@SMSGroupID", SqlDbType.Int).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@SMSGroupName", SqlDbType.NVarChar).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@StatusDescription", SqlDbType.NVarChar).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@SMSCount", SqlDbType.Int).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@MessageID", SqlDbType.Int).Value = DBNull.Value;
                                        cmd1.Parameters.Add("@SMSOutLogIDRef", SqlDbType.Int).Value = 0;
                                        cmd1.Parameters["@SMSOutLogIDRef"].Direction = ParameterDirection.Output;

                                        cmd1.CommandType = CommandType.StoredProcedure;

                                        cmd1.CommandText = strInsert;
                                        rowsAffected1 = cmd1.ExecuteNonQuery();
                                        #endregion
                                        SMSOutLogID = (int)cmd1.Parameters["@SMSOutLogIDRef"].Value;
                                    }

                                    #region [Json response]

                                    var restRequestClass = new RestRequestClass()
                                    {
                                        from = SenderMobileNumber,
                                        to = strmobile[1].ToString(),
                                        text = SMSNoticeBody
                                    };
                                    var json = JsonConvert.SerializeObject(restRequestClass);

                                    var client = new RestClient(StrSMSEndPoint);
                                    var request = new RestRequest(Method.POST);
                                    request.AddHeader("accept", "application/xml");
                                    request.AddHeader("content-type", "application/xml");
                                    request.AddHeader("authorization", "Basic " + HashAutor);
                                    request.AddParameter("application/json", json, ParameterType.RequestBody);

                                    try
                                    {
                                        if (!IsSimulateSMS)
                                        {
                                            IRestResponse response = client.Execute(request);
                                            if (!string.IsNullOrEmpty(response.Content))
                                            {
                                                #region [api response]

                                                JObject o = JObject.Parse(response.Content);
                                                smsto = o["messages"][0]["to"].ToString();
                                                groupid = o["messages"][0]["status"]["groupId"].ToString();
                                                groupname = o["messages"][0]["status"]["groupName"].ToString();
                                                statusid = o["messages"][0]["status"]["id"].ToString();
                                                statusname = o["messages"][0]["status"]["name"].ToString();
                                                description = o["messages"][0]["status"]["description"].ToString();
                                                smsCount = o["messages"][0]["smsCount"].ToString();
                                                messageId = o["messages"][0]["messageId"].ToString();
                                                status = response.ResponseStatus.ToString();
                                                statuscode = response.StatusCode.ToString();
                                                statusDescription = response.StatusDescription.ToString();
                                                if (!string.IsNullOrEmpty(smsCount))
                                                {
                                                    TotalSMSDeducted = TotalSMSDeducted + Convert.ToInt32(smsCount);
                                                    /// if failed
                                                    /// send mail to system Admin and sms to Administrator
                                                    ///
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        isSendSMSSuccess = 106;
                                        return (isSendSMSSuccess);
                                        ////MessageBox.Show(ex.Message.ToString());
                                    }
                                    #endregion
                                }

                                if (string.IsNullOrEmpty(statusid))
                                {
                                    statusid = "0";
                                }
                                if (string.IsNullOrEmpty(groupid))
                                {
                                    groupid = "0";
                                }
                                if (string.IsNullOrEmpty(smsCount))
                                {
                                    smsCount = "0";
                                }
                                if (SMSOutLogID > 0)
                                {
                                    cmd1 = new SqlCommand();
                                    if (connection.State == 0)
                                    {
                                        connection.Open();
                                    }
                                    cmd1.Connection = connection;
                                    #region [insert record for send sms]
                                    strInsert = "USP_InsertUpdateSMSDelevary";
                                    cmd1.Parameters.Add("@SMSOutLogID", SqlDbType.Int).Value = SMSOutLogID;
                                    cmd1.Parameters.Add("@ReceiptNumber", SqlDbType.NVarChar).Value = strmobile[1].ToString();
                                    cmd1.Parameters.Add("@SenderNumber", SqlDbType.NVarChar).Value = SenderMobileNumber;
                                    cmd1.Parameters.Add("@sMessage", SqlDbType.NVarChar).Value = SMSNoticeBody;
                                    cmd1.Parameters.Add("@delay", SqlDbType.Int).Value = 15;
                                    cmd1.Parameters.Add("@type", SqlDbType.Int).Value = 1;
                                    cmd1.Parameters.Add("@DeliveryStatusID", SqlDbType.Int).Value = statusid;
                                    cmd1.Parameters.Add("@Result", SqlDbType.NVarChar).Value = statusname;
                                    cmd1.Parameters.Add("@DeliveryTime", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd1.Parameters.Add("@ProviderID", SqlDbType.Int).Value = providerid;
                                    cmd1.Parameters.Add("@SMSGroupID", SqlDbType.Int).Value = groupid;
                                    cmd1.Parameters.Add("@SMSGroupName", SqlDbType.NVarChar).Value = groupname;
                                    cmd1.Parameters.Add("@StatusDescription", SqlDbType.NVarChar).Value = description;
                                    cmd1.Parameters.Add("@SMSCount", SqlDbType.Int).Value = smsCount;
                                    cmd1.Parameters.Add("@MessageID", SqlDbType.NVarChar).Value = messageId;
                                    cmd1.Parameters.Add("@SMSOutLogIDRef", SqlDbType.Int).Value = 0;
                                    cmd1.Parameters["@SMSOutLogIDRef"].Direction = ParameterDirection.Output;

                                    cmd1.CommandType = CommandType.StoredProcedure;

                                    cmd1.CommandText = strInsert;
                                    //cmd1.Transaction = Transection;
                                    rowsAffected1 = cmd1.ExecuteNonQuery();
                                    #endregion
                                }

                                    #endregion
                            }                                
                        }
                        else
                        {
                            #region [No Provider set]
                            strInsert = "INSERT INTO [dbo].[SMSDelivery] " +
                                        " (ReceiptNumber" +
                                        " ,SenderNumber" +
                                        " ,sMessage" +
                                        " ,delay " +
                                        " ,type" +
                                        " ,DeliveryStatusID" +
                                        " ,Result" +
                                        " ,DeliveryTime)" +
                                        " VALUES" +
                                        " ('" + strmobile[1].ToString() + "'" +
                                        " , '" + SenderMobileNumber + "'" +
                                        " , N'" + SMSNoticeBody + "'" +
                                        " , " + 15 + "" +
                                            " , 1" +
                                            " , 8 " +
                                            " , 'recipient mobile number is invalid'" +
                                            " , '" + DateTime.Now + "')";
                            cmd1 = new SqlCommand();
                            if (connection.State == 0)
                            {
                                connection.Open();
                            }
                            cmd1.Connection = connection;
                            cmd1.CommandType = CommandType.Text;
                            cmd1.CommandText = strInsert;
                            //cmd1.Transaction = Transection;
                            rowsAffected1 = cmd1.ExecuteNonQuery();
                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        isSendSMSSuccess = 102;
                        ///MessageBox.Show("Balance is not eanough to send sms.....Please Purchase SMS....");
                        return (isSendSMSSuccess);
                        //PopUp("SMS Balance is 0, Please Recharge Balance amount");
                        ///MessageBox.Show("SMS Balance is 0, Please Recharge Balance amount");
                    }
                    //end check sms balance     
                    AllSmsSend = true;
                }
                else
                {
                    AllSmsSend = true;
                }
            }
            catch (Exception ex)
            {
                isSendSMSSuccess = 103;
                ///MessageBox.Show("SMS Sending Fail: " + ex.Message.ToString());
                //Transection.Rollback();
            }
        #endregion
        #region [sms send count balance update]
            SMSBalanceNew = SMSBalanceold - TotalSMSDeducted;
        strBalanceUpdate = "Update SMSBalance set  SMSBalanceQty=" + SMSBalanceNew + " where SMSAccountName='" + SMSAccountName + "'";
        cmd1 = new SqlCommand();
        if (connection.State == 0)
        {
            connection.Open();
        }
        cmd1.Connection = connection;
        cmd1.CommandType = CommandType.Text;
        cmd1.CommandText = strBalanceUpdate;
        int rowsAffected2 = cmd1.ExecuteNonQuery();
        #endregion


        return (isSendSMSSuccess);
    }
    public Member getMemberByID(int pMemberID)
    {
        Member oMemberModel = new Member();
        DataTable dt = new DataTable();
        string CONN_STRING = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(CONN_STRING);
        {
            //
            string sSQL = "Select  MemberID,NameOfMember,MemberCode,M.MemberCategoryID,M.MemberStatusID,M.MemberGroupID,C.CategoryName,M.CellPhone,";
            sSQL = sSQL + " S.MemberStatus MemberStatusName,G.MemberGroupName,MemberOfType,HandiCap ";
            sSQL = sSQL + " From dbo.MemberInfo M Left Outer Join dbo.MemberCategory C ";
            sSQL = sSQL + " ON M.MemberCategoryID = C.MemberCategoryID Left Outer Join dbo.MemberStatus S";
            sSQL = sSQL + " ON M.MemberStatusID = S.MemberStatusID Left Outer Join dbo.MemberGroup G";
            sSQL = sSQL + " ON M.MemberGroupID = G.MemberGroupID";
            if (pMemberID>0)
            {
                sSQL = sSQL + " where M.MemberID = " + pMemberID + "";
            }

            SqlCommand cmd = new SqlCommand(sSQL, con);

            cmd.CommandType = CommandType.Text;
            //cmd.Parameters.Add("@VoucherHeadID", SqlDbType.NVarChar).Value = pVoucherHeadID;
            SqlDataAdapter adpt = new SqlDataAdapter(cmd);
            try
            {

                adpt.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow rw in dt.Rows)
                    {
                        if (rw["MemberID"] != DBNull.Value)
                        {
                            oMemberModel.MemberID = Convert.ToInt32(rw["MemberID"].ToString());
                        }
                        if (rw["MemberGroupID"] != DBNull.Value)
                        {
                            oMemberModel.MemberGroupID = Convert.ToInt32(rw["MemberGroupID"].ToString());
                        }
                        if (rw["NameOfMember"] != DBNull.Value)
                        {
                            oMemberModel.MemberName = Convert.ToString(rw["NameOfMember"].ToString());
                        }
                        if (rw["MemberCode"] != DBNull.Value)
                        {
                            oMemberModel.MemberCode = Convert.ToString(rw["MemberCode"].ToString());
                        }

                        if (rw["CellPhone"] != DBNull.Value)
                        {
                            oMemberModel.MobileNo = Convert.ToString(rw["CellPhone"].ToString());
                        }
                        if (rw["MemberGroupName"] != DBNull.Value)
                        {
                            oMemberModel.MemberGroupName = Convert.ToString(rw["MemberGroupName"].ToString());
                        }
                        if (rw["CategoryName"] != DBNull.Value)
                        {
                            oMemberModel.MemberCategoryName = Convert.ToString(rw["CategoryName"].ToString());
                        }
                        if (rw["MemberStatusName"] != DBNull.Value)
                        {
                            oMemberModel.MemberStatusName = Convert.ToString(rw["MemberStatusName"].ToString());
                        }
                        if (rw["MemberOfType"] != DBNull.Value)
                        {
                            oMemberModel.MemberOfType = Convert.ToString(rw["MemberOfType"].ToString());
                        }
                        if (rw["MemberStatusID"] != DBNull.Value)
                        {
                            oMemberModel.MemberStatusID = Convert.ToInt32(rw["MemberStatusID"].ToString());
                        }
                        if (rw["MemberCategoryID"] != DBNull.Value)
                        {
                            oMemberModel.MemberCategoryID = Convert.ToInt32(rw["MemberCategoryID"].ToString());
                        }
                        if (rw["HandiCap"] != DBNull.Value)
                        {
                            oMemberModel.HandiCap = Convert.ToInt32(rw["HandiCap"].ToString());
                        }
                        oMemberModel.ErrorCode = 0;
                        oMemberModel.ErrorMessage = "Success";
                    }
                }
                else
                {
                    oMemberModel.ErrorCode = 101;
                    oMemberModel.ErrorMessage = "Invalid Account";
                }
            }
            catch (SqlException ex)
            {
                oMemberModel.ErrorCode = 601;
                oMemberModel.ErrorMessage = ex.Message.ToString();
            }
        }
        return oMemberModel;
    }
}
