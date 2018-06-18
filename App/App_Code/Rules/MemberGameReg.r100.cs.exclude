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
using System.Configuration;

namespace MyCompany.Rules
{
	public partial class MemberGameRegBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Calculate".
        /// </summary>
        [Rule("r100")]
        public void r100Implementation(MemberGameRegModel instance)
        {
            // This is the placeholder for method implementation.

            //instance.GreenSubsidy = green;
            // This is the placeholder for method implementation.
            //int pMemberGroupID = 0, pMemberCategoryID = 0, pMemberStatusID = 0, pMemberID = 0;
            //int pCaddieID = 0, pBallBoyIDID = 0, pHoleTypeID = 0, pHandiCap = 0;
            //string pMemberOfType = "";
            //decimal pCaddieFee = 0m, pChildrenBallBoySubsidy = 0m, pSpouseBallBoySubsidy = 0m;
            //decimal pBallBoySubsiDy = 0m, pBallBoyFee = 0m, pCildrenCaddieSubsidy = 0m, pSpouseCaddieSubsidy = 0m;
            //decimal pCaddieSubsidy = 0m, pGreenFee = 0m;
            //pMemberID = Convert.ToInt32(instance.MemberID);
            //pCaddieID = Convert.ToInt32(instance.CaddieID);
            //pBallBoyIDID = Convert.ToInt32(instance.BallBoyID);
            //pHoleTypeID = Convert.ToInt32(instance.HoleTypeID);
            //if (pHoleTypeID == 0)
            //{
            //    pHoleTypeID = 1;
            //}
            //if ((pMemberID > 0))
            //{
            //    SqlConnection connection = new SqlConnection();
            //    string cldSrvdb = ConfigurationManager.AppSettings["CloudServerdb"].ToString();
            //    string CloudSrv = ConfigurationManager.AppSettings["CloudServer"].ToString();

            //    connection.ConnectionString = connHelper("SQLSERVER", CloudSrv, cldSrvdb, "MyCloudAdmin", "MyCloudAdmin@786!#");
            //    //#endregion
            //    try
            //    {
            //        connection.Open();
            //    }
            //    catch
            //    {
            //    }
            //    string strSelect = "";
            //    SqlCommand cmd1 = new SqlCommand();
            //    DataTable dtProvider = new DataTable();
            //    SqlDataAdapter daProvider = new SqlDataAdapter();
            //    #region [Reader Member record]
            //    if (pMemberID > 0)
            //    {
            //        strSelect = "select MemberID,MemberCategoryID,MemberStatusID,MemberOfType,HandiCap from MemberInfo where MemberID=" + pMemberID.ToString();
            //        cmd1.Connection = connection;
            //        cmd1.CommandType = CommandType.Text;
            //        cmd1.CommandText = strSelect;
            //        daProvider.SelectCommand = cmd1;
            //        daProvider.Fill(dtProvider);
            //        if (dtProvider.Rows.Count > 0)
            //        {
            //            if (dtProvider.Rows[0]["MemberCategoryID"] != DBNull.Value)
            //            {
            //                pMemberCategoryID = Convert.ToInt32(dtProvider.Rows[0]["MemberCategoryID"]);
            //            }
            //            if (dtProvider.Rows[0]["MemberStatusID"] != DBNull.Value)
            //            {
            //                pMemberStatusID = Convert.ToInt32(dtProvider.Rows[0]["MemberStatusID"]);
            //            }
            //            if (dtProvider.Rows[0]["MemberOfType"] != DBNull.Value)
            //            {
            //                pMemberOfType = Convert.ToString(dtProvider.Rows[0]["MemberOfType"]);
            //            }
            //            if (dtProvider.Rows[0]["HandiCap"] != DBNull.Value)
            //            {
            //                pHandiCap = Convert.ToInt32(dtProvider.Rows[0]["HandiCap"]);
            //            }
            //        }
            //    }
            //    #endregion
            //    #region [Reader Game Fee record]
            //    strSelect = "select GreenFee,CaddieFee,CaddieSubsidy,SpouseCaddieSubsidy,CildrenCaddieSubsidy,";
            //    strSelect = strSelect + " BallBoyFee,BallBoySubsiDy,SpouseBallBoySubsidy,ChildrenBallBoySubsidy,";
            //    strSelect = strSelect + " GolfCartFee from dbo.GameFee";
            //    strSelect = strSelect + " Where MemberCategoryID=" + pMemberCategoryID;
            //    strSelect = strSelect + " And MemberStatusID = " + pMemberStatusID;
            //    strSelect = strSelect + " And HoleTypeID= " + pHoleTypeID;
            //    //strSelect = strSelect + " pMemberID.ToString();";
            //    cmd1 = new SqlCommand();
            //    dtProvider = new DataTable();
            //    daProvider = new SqlDataAdapter();
            //    cmd1.Connection = connection;
            //    cmd1.CommandType = CommandType.Text;
            //    cmd1.CommandText = strSelect;
            //    daProvider.SelectCommand = cmd1;
            //    daProvider.Fill(dtProvider);
            //    if (dtProvider.Rows.Count > 0)
            //    {
            //        #region [read values]
            //        if (dtProvider.Rows[0]["CaddieFee"] != DBNull.Value)
            //        {
            //            pCaddieFee = Convert.ToDecimal(dtProvider.Rows[0]["CaddieFee"]);
            //        }
            //        if (dtProvider.Rows[0]["ChildrenBallBoySubsidy"] != DBNull.Value)
            //        {
            //            pChildrenBallBoySubsidy = Convert.ToDecimal(dtProvider.Rows[0]["ChildrenBallBoySubsidy"]);
            //        }
            //        if (dtProvider.Rows[0]["SpouseBallBoySubsidy"] != DBNull.Value)
            //        {
            //            pSpouseBallBoySubsidy = Convert.ToDecimal(dtProvider.Rows[0]["SpouseBallBoySubsidy"]);
            //        }
            //        if (dtProvider.Rows[0]["BallBoySubsiDy"] != DBNull.Value)
            //        {
            //            pBallBoySubsiDy = Convert.ToDecimal(dtProvider.Rows[0]["BallBoySubsiDy"]);
            //        }
            //        if (dtProvider.Rows[0]["BallBoyFee"] != DBNull.Value)
            //        {
            //            pBallBoyFee = Convert.ToDecimal(dtProvider.Rows[0]["BallBoyFee"]);
            //        }
            //        if (dtProvider.Rows[0]["CildrenCaddieSubsidy"] != DBNull.Value)
            //        {
            //            pCildrenCaddieSubsidy = Convert.ToDecimal(dtProvider.Rows[0]["CildrenCaddieSubsidy"]);
            //        }
            //        if (dtProvider.Rows[0]["SpouseCaddieSubsidy"] != DBNull.Value)
            //        {
            //            pSpouseCaddieSubsidy = Convert.ToDecimal(dtProvider.Rows[0]["SpouseCaddieSubsidy"]);
            //        }
            //        if (dtProvider.Rows[0]["CaddieSubsidy"] != DBNull.Value)
            //        {
            //            pCaddieSubsidy = Convert.ToDecimal(dtProvider.Rows[0]["CaddieSubsidy"]);
            //        }
            //        if (dtProvider.Rows[0]["GreenFee"] != DBNull.Value)
            //        {
            //            pGreenFee = Convert.ToDecimal(dtProvider.Rows[0]["GreenFee"]);
            //        }
            //        #endregion
            //    }
            //    #endregion
            //    //instance.HandiCap = pHandiCap;
            //    instance.GreenFee = pGreenFee;
            //    if (pCaddieID > 0)
            //    {
            //        instance.CaddieFee = pCaddieFee;
            //        if (pMemberOfType == "Full Member")
            //        {
            //            instance.CaddieSubsidy = pCaddieSubsidy;
            //        }
            //        if (pMemberOfType == "Spouse Of Member")
            //        {
            //            instance.CaddieSubsidy = pSpouseCaddieSubsidy;
            //        }
            //        if (pMemberOfType == "Children Of Member")
            //        {
            //            instance.CaddieSubsidy = pCildrenCaddieSubsidy;
            //        }
            //    }
            //    if (pBallBoyIDID > 0)
            //    {
            //        instance.BallBoyFee = pBallBoyFee;
            //        if (pMemberOfType == "Full Member")
            //        {
            //            instance.BallBoySubsidy = pBallBoySubsiDy;
            //        }
            //        if (pMemberOfType == "Spouse Of Member")
            //        {
            //            instance.BallBoySubsidy = pSpouseCaddieSubsidy;
            //        }
            //        if (pMemberOfType == "Children Of Member")
            //        {
            //            instance.BallBoySubsidy = pChildrenBallBoySubsidy;
            //        }
            //    }
            //    instance.GreenSubsidy = pGreenFee;
            //    if (pMemberOfType == "Full Member")
            //    {
            //        if (pCaddieID == 0)
            //        {
            //            pCaddieFee = 0;
            //        }
            //        if (pBallBoyIDID == 0)
            //        {
            //            pBallBoyFee = 0;
            //        }
            //        instance.TotalBill = (pGreenFee + pCaddieFee + pBallBoyFee);// -(pCaddieSubsidy + pSpouseCaddieSubsidy + pBallBoySubsiDy + pSpouseCaddieSubsidy + pChildrenBallBoySubsidy);
            //    }
            //    if (pMemberOfType == "Spouse Of Member")
            //    {
            //        if (pCaddieID == 0)
            //        {
            //            pCaddieFee = 0;
            //        }
            //        instance.TotalBill = (pGreenFee + pCaddieFee + pBallBoyFee) - (pCaddieSubsidy + pSpouseCaddieSubsidy + pBallBoySubsiDy + pSpouseCaddieSubsidy + pChildrenBallBoySubsidy);
            //    }
            //    if (pMemberOfType == "Children Of Member")
            //    {
            //        if (pCaddieID == 0)
            //        {
            //            pCaddieFee = 0;
            //        }
            //        instance.TotalBill = (pGreenFee + pCaddieFee + pBallBoyFee) - (pCaddieSubsidy + pSpouseCaddieSubsidy + pBallBoySubsiDy + pSpouseCaddieSubsidy + pChildrenBallBoySubsidy);
            //    }

            }
        
        //public string connHelper(string sConnectionType, string sqlServer, string initialCatalog, string userId, string password)
        //{
        //    string ssCon = "";
        //    if (sConnectionType != null)
        //    {
        //        if (sConnectionType == "SQLSERVER")
        //        {
        //            ssCon = (String.Format("Data Source={0};Connection Timeout=300;Initial Catalog={1};User Id={2};Password={3};", sqlServer, initialCatalog, userId, password));
        //        }
        //    }
        //    return ssCon;
        //}

    }
}
