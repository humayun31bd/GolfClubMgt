using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using MyCompany.Data;
using MyCompany.Models;
using System.Data.SqlClient;

namespace MyCompany.Rules
{
	public partial class LockerBookingBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Update" and argument that matches "LockerClosed".
        /// </summary>
        [Rule("r107")]
        public void r107Implementation(LockerBookingModel instance,DateTime Parameters_FromDate)
        {
            // This is the placeholder for method implementation.

            PreventDefault();
                int noDays = 15;
                int lockerID = (int)instance.LockerID.Value;
                int lockerBookID = (int)instance.LockerBookID.Value;
                DateTime lockerClosedt = (DateTime)Parameters_FromDate;
                DateTime oCurrDate = DateTime.Now;

                if (lockerClosedt < oCurrDate.AddDays(-15))
                {
                    PreventDefault();
                    Result.ShowMessage("Locker not close after 15 days within the month");
                    return;
                }

                SqlCommand cmd1 = new SqlCommand();
                SqlConnection connection = new SqlConnection();
                string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
                connection.ConnectionString = conr;

                if (connection.State == 0)
                {
                    connection.Open();
                }
                cmd1.Connection = connection;

                string strInsert = "dbo.USP_LockerFreeDone ";
                cmd1.Parameters.Add("@LockerID", SqlDbType.Int).Value = lockerID;
                cmd1.Parameters.Add("@ClosDate", SqlDbType.DateTime).Value = Parameters_FromDate;
                cmd1.Parameters.Add("@LockerBookID", SqlDbType.Int).Value = lockerBookID;

                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.CommandText = strInsert;
                try
                {
                    int rowsAffected1 = cmd1.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Result.ShowMessage(ex.ToString());
                    return;
                }
                

            

        }
    }
}
