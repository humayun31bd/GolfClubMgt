using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class Controls_MemberSMSPanel : System.Web.UI.UserControl
{
    private SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyCompany"].ConnectionString);   

    protected void Page_Load(object sender, EventArgs e)
    {
        SMSBalance();
        MemberListBind();

    }
    protected void btnMemberSendSMS_Click(object sender, EventArgs e)
    {
        clsGeneralLib oclsGeneralLib = new clsGeneralLib();

        string SmSMsg = txtSMSBody.Text;
        if (!string.IsNullOrEmpty(SmSMsg))
        {
            int iMemberID = 0;
            string sMemberCode = "";
            string sMembername = "";
            string sMemberMobileNo = "";
            foreach (GridViewRow row in Member_grid.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.Cells[0].FindControl("chkRowMember") as CheckBox);
                    if (chkRow.Checked)
                    {
                        if (!string.IsNullOrEmpty(row.Cells[1].Text))
                        {
                            iMemberID = Convert.ToInt32(row.Cells[1].Text);
                        }
                        if (!string.IsNullOrEmpty(row.Cells[2].Text))
                        {
                            sMemberCode = row.Cells[2].Text;
                        }
                        if (!string.IsNullOrEmpty(row.Cells[3].Text))
                        {
                            sMembername = row.Cells[3].Text;
                        }
                        if (!string.IsNullOrEmpty(row.Cells[4].Text))
                        {
                            sMemberMobileNo = row.Cells[4].Text;
                        }
                        try
                        {
                            int iRet = oclsGeneralLib.SendSMSTo(sMemberMobileNo, sMembername, SmSMsg);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }


    }

    protected void SMSBalance()
    {
        string strSMSBalance = "select * from SMSBalance ";
        DataTable dtSMSBalance = new DataTable();
        SqlDataAdapter daSMSBalance = new SqlDataAdapter(strSMSBalance, conn);
        try
        {
            daSMSBalance.Fill(dtSMSBalance);
        }
        catch
        {
        }

        if (dtSMSBalance.Rows.Count > 0)
        {
            if (dtSMSBalance.Rows[0]["SMSBalanceQty"] != DBNull.Value)
            {
                lblSMSBalance.Text = "Your SMS Balance is left : " + dtSMSBalance.Rows[0]["SMSBalanceQty"].ToString();
            }
            if (dtSMSBalance.Rows[0]["SMSPurchasedQty"] != DBNull.Value)
            {
                lblSmSQty.Text = "Your SMS quantity is : " + dtSMSBalance.Rows[0]["SMSPurchasedQty"].ToString();
            }
        }
    }
    private void MemberListBind()
    {
        #region [ Member List Table/ grid ]
        try
        {
            if (conn.State == 0)
            {
                conn.Open();
            }
            string SQL = " select  MemberID, MemberCode, NameOfMember,CellPhone MobileNo from dbo.MemberInfo where MemberOfType='Full Member'";
            SqlDataAdapter da = new SqlDataAdapter(SQL, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Member_grid.DataSource = dt;
            Member_grid.DataBind();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
            ////PopUp(" Error - Grid/Administration Governance Table Data Load Problem\n" + ex.InnerException.ToString());
            return;
        }
        #endregion
    }
    
    protected void btnSendSMSCancel_Click(object sender, EventArgs e)
    {
    }
    protected void Member_grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
    }
    protected void chk_select_Member_CheckedChanged(object sender, EventArgs e)
    {
        
    }
    protected void btnMemberSendSMS1_Click(object sender, EventArgs e)
    {

    }
    protected void btnSendSMSCancel1_Click(object sender, EventArgs e)
    {

    }
    protected void chk_select_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk_allSelect = (CheckBox)Member_grid.HeaderRow.FindControl("chk_select");
        if (chk_allSelect.Checked)
        {
            foreach (GridViewRow gvRow in Member_grid.Rows)
            {
                CheckBox chk_box = (CheckBox)gvRow.FindControl("chkRowMember");
                chk_box.Checked = true;
            }
        }
        else
        {
            foreach (GridViewRow gvRow in Member_grid.Rows)
            {
                CheckBox chk_box = (CheckBox)gvRow.FindControl("chkRowMember");
                chk_box.Checked = false;
            }
        }
    }
}
