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
	public partial class MemberBillApprovalBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Custom" and argument that matches "VoucherApproved".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(MemberBillApprovalModel instance)
        {
            // This is the placeholder for method implementation.
            
            string CONN_STRING = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
            SqlConnection connection = new SqlConnection(CONN_STRING);

            if (instance != null)
            {

                #region [sms send count balance update]

                string strBalanceUpdate = "USP_MemberBillPaymentApproved '" + instance.VoucherNo + "'";
                SqlCommand cmd1 = new SqlCommand();
                if (connection.State == 0)
                {
                    connection.Open();
                }
                cmd1.Connection = connection;
                cmd1.CommandType = CommandType.Text;
                cmd1.CommandText = strBalanceUpdate;
                int rowsAffected2 = cmd1.ExecuteNonQuery();
                #endregion

            }

        }
    }
}
