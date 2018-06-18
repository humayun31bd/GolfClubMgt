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
    public partial class GameFlightScheduleBusinessRules : MyCompany.Data.BusinessRules
    {

        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "CreateGameSchedule".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(GameFlightScheduleModel instance, DateTime @Parameters_GameDate, 
            DateTime @Parameters_FlightStart, DateTime @Parameters_FlightEnd, int @Parameters_IntervalInMinute, 
            int @Parameters_MaxPlayer)
        {


            // This is the placeholder for method implementation.
            ///Exec dbo.USP_GameCreateFlight  @Parameters_GameDate,@Parameters_FlightStart,@Parameters_FlightEnd,@Parameters_IntervalInMinute
            string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
            SqlConnection connection = new SqlConnection(conr);
            if (connection.State == 0)
            {
                connection.Open();
            }
            SqlCommand cmd = new SqlCommand("USP_GameCreateFlight", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@GameDate", SqlDbType.DateTime).Value = @Parameters_GameDate;
            cmd.Parameters.Add("@FlightStart", SqlDbType.DateTime).Value = @Parameters_FlightStart;
            cmd.Parameters.Add("@FlightEnd", SqlDbType.DateTime).Value = @Parameters_FlightEnd;
            cmd.Parameters.Add("@IntervalInMinute", SqlDbType.Int).Value = @Parameters_IntervalInMinute;
            cmd.Parameters.Add("@MaxPlayer", SqlDbType.Int).Value = @Parameters_MaxPlayer;

            try
            {
                int rowsAffected1 = cmd.ExecuteNonQuery();
                

            }
            catch (Exception ex)
            {

            }



        }
    }
}
