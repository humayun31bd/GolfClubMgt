using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class Controls_MemberBillCollectionNew : System.Web.UI.UserControl
{
    static int iMemberID = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    private bool GetMemberInfo(int pMemberID)
    {
        DataTable dt = new DataTable();
        string conr = System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString;
        SqlConnection con = new SqlConnection(conr);
        {
            SqlCommand cmd = new SqlCommand("USP_MemberCurrentDueGet", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = pMemberID;
            SqlDataAdapter adpt = new SqlDataAdapter(cmd);
            try
            {
                adpt.Fill(dt);

                string sMemberName = dt.Rows[0]["NameOfMember"].ToString();
                string sCategoryName = dt.Rows[0]["CategoryName"].ToString();

                lblMemberName.Text = sMemberName;
                lblCategory.Text = sCategoryName;
            }
            catch (SqlException ex)
            {
                return false;
                //popup("");
            }
            return true;
        }
    }

    protected void txtMemberID_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtMemberID.Text))
        {
            iMemberID = Convert.ToInt32(txtMemberID.Text);

            bool bret = GetMemberInfo(iMemberID);

        }
    }
}
