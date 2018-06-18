using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;
using System.Configuration;
using System.Data.SqlClient;
using MyCompany.Web;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MyCompany.Rules
{
	public partial class MemberCardBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Report" and argument that matches "SendPINforCard".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(MemberCardModel instance)
        {
            // This is the placeholder for method implementation.
            string pMessage = string.Empty;
            int MemberCardID = Convert.ToInt32(instance.MemberCardID.ToString());
            string memMobileNo = instance.MobileNo.ToString();
            int MemberID = Convert.ToInt32(instance.MemberID.ToString());
            string MemberName = instance.MemberNameOfMember.ToString();
            string[] strmobile;
            if (memMobileNo.Length <= 11)
            {
            }
            strmobile = FormatMobileNo(memMobileNo);
            if (strmobile[0] == "0")
            {
                memMobileNo = strmobile[1].ToString();
                pMessage = "Please dodnot disclose these pin number.";
                //int iRet = SendSMSTo(MemberCardID, memMobileNo, MemberName, pMessage);
            }            
            RandomGenerator generator = new RandomGenerator();
            int rand = generator.RandomNumber(1000, 9999);
            //string pass = Convert.ToString(generator.RandomNumber());  
            string pass = rand.ToString();
            int duplcatepinnotfound = 0;
            while (duplcatepinnotfound != 1)
            {
                rand = generator.RandomNumber(1000, 9999);
                pass = Convert.ToString(rand.ToString());
                string passRet = GetDuplicateNumber(rand.ToString());
                if (passRet.Length == 0)
                {
                    
                    duplcatepinnotfound = 1;
                    if (duplcatepinnotfound == 1)
                    {
                        UpdatePinToCard(MemberCardID, pass);
                        pMessage = pMessage + pass;
                        SendSMSTo(MemberCardID, memMobileNo, MemberName, pMessage);
                        break;
                    }
                    rand = generator.RandomNumber(1000, 9999);
                    //pass = generator.RandomPassword();
                }
                else
                {
                    break;
                }
            }

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

        public int SendSMSTo(int pMemberCardID,string pMobileNO, string pMemberName,string pMessage)
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

                            #region [Database Update]

                            if (rowsAffected1 > 0)
                            {
                                string strShortNoticeDetails = "update dbo.MemberCard set IsSendSMS=1, SendSMSDate='" + System.DateTime.Now + "' where MemberCardID=" + pMemberCardID.ToString();
                                cmd1 = new SqlCommand();
                                if (connection.State == 0)
                                {
                                    connection.Open();
                                }
                                cmd1.Connection = connection;
                                cmd1.CommandType = CommandType.Text;
                                cmd1.CommandText = strShortNoticeDetails;
                                rowsAffected3 = cmd1.ExecuteNonQuery();
                            }
                            #endregion


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


        public class RandomGenerator
        {
            // Generate a random number between two numbers  
            public int RandomNumber(int min, int max)
            {
                Random random = new Random();
                return random.Next(min, max);
            }

            // Generate a random string with a given size  
            public string RandomString(int size, bool lowerCase)
            {
                StringBuilder builder = new StringBuilder();
                Random random = new Random();
                char ch;
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                if (lowerCase)
                    return builder.ToString().ToLower();
                return builder.ToString();
            }

            // Generate a random password  
            public string RandomPassword()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(RandomString(4, true));
                builder.Append(RandomNumber(1000, 9999));
                builder.Append(RandomString(2, false));
                return builder.ToString();
            }
        }

        public string GetDuplicateNumber(string pRandomNumber)
        {
            string sCardPIN = string.Empty;
            string CloudSrv = ConfigurationManager.AppSettings["CloudServer"].ToString();
            CloudSrv = "45.35.4.235";
            string cldSrvdb = ConfigurationManager.AppSettings["CloudServerdb"].ToString();
            SqlConnection connection = new SqlConnection();
            string sConnectionType = "SQLSERVER";
            connection.ConnectionString = connHelper("SQLSERVER", CloudSrv, cldSrvdb, "MyCloudAdmin", "MyCloudAdmin@786!#");
            
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                sCardPIN = "Failed.Connection";
                /// Log Database connection failed
                ///throw new Exception("Database connection failed. Please try it later.");
                ///MessageBox.Show("Cloud Server Status : " + ex.Message);
                ///
                return (sCardPIN);
            }

            #region [Get Existing PIN Search ]
            string strSMSBalance = "";

            strSMSBalance = "Select CardPIN from dbo.MemberCard where CardPIN = '" + pRandomNumber + "'";
            SqlCommand cmd1 = new SqlCommand();
            DataTable dtCardPINs = new DataTable();
            SqlDataAdapter daSMSBalance = new SqlDataAdapter();
            cmd1.Connection = connection;
            cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = strSMSBalance;
            daSMSBalance.SelectCommand = cmd1;
            try
            {
                daSMSBalance.Fill(dtCardPINs);
            }
            catch(Exception ex )
            {
                sCardPIN = "Failed.Database Access";
                /// Log Database connection failed
                ///throw new Exception("Database connection failed. Please try it later.");
                ///MessageBox.Show("Cloud Server Status : " + ex.Message);
                ///
                return (sCardPIN);
            }

            if (dtCardPINs.Rows.Count > 0)
            {
                if (dtCardPINs.Rows[0]["CardPIN"] != DBNull.Value)
                {
                    sCardPIN = Convert.ToString(dtCardPINs.Rows[0]["CardPIN"]);
                }
            }
            return (sCardPIN);
            #endregion
        }
        public string UpdatePinToCard(int pMemberCardID, string pRandomNumber)
        {
            string sCardPIN = string.Empty;
            string CloudSrv = ConfigurationManager.AppSettings["CloudServerdb"].ToString();
            CloudSrv = "45.35.4.235";
            string cldSrvdb = ConfigurationManager.AppSettings["CloudServerdb"].ToString();
            SqlConnection connection = new SqlConnection();
            string sConnectionType = "SQLSERVER";
            connection.ConnectionString = connHelper("SQLSERVER", CloudSrv, cldSrvdb, "MyCloudAdmin", "MyCloudAdmin@786!#");

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                sCardPIN = "Failed.Connection";
                /// Log Database connection failed
                ///throw new Exception("Database connection failed. Please try it later.");
                ///MessageBox.Show("Cloud Server Status : " + ex.Message);
                ///
                return (sCardPIN);
            }

            #region [Get Existing PIN Search ]
            string strSMSBalance = "";

            strSMSBalance = "Update dbo.MemberCard set CardPIN ='" + pRandomNumber + "' where MemberCardID =" + pMemberCardID + "";
            SqlCommand cmd1 = new SqlCommand();
            DataTable dtCardPINs = new DataTable();
            SqlDataAdapter daSMSBalance = new SqlDataAdapter();
            cmd1.Connection = connection;
            cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = strSMSBalance;
            daSMSBalance.SelectCommand = cmd1;
            try
            {
                daSMSBalance.Fill(dtCardPINs);
            }
            catch
            {
                sCardPIN = "Failed.Database Access";
                /// Log Database connection failed
                ///throw new Exception("Database connection failed. Please try it later.");
                ///MessageBox.Show("Cloud Server Status : " + ex.Message);
                ///
                return (sCardPIN);
            }
            return (sCardPIN);
            #endregion
        }
    }
}
