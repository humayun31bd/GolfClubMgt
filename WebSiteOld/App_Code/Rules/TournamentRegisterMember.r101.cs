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
	public partial class TournamentRegisterMemberBusinessRules : MyCompany.Data.BusinessRules
    {
        
        /// <summary>
        /// This method will execute in any view for an action
        /// with a command name that matches "Calculate".
        /// </summary>
        [Rule("r101")]
        public void r101Implementation(TournamentRegisterMemberModel instance)
        {
            // This is the placeholder for method implementation.

            int pMemberGroupID = 0, pMemberID = 0;
            decimal pMemberFee = 0m, @SpouseFee = 0m, @ChildrenFee=0m;
            int pCaddieID = 0, pBallBoyIDID = 0, pHoleTypeID = 0, pHandiCap = 0;
            int pMemberCategoryID = 0, pMemberStatusID=0;
            string pMemberOfType = "";
            decimal pCaddieFee = 0m, pChildrenBallBoySubsidy = 0m, pSpouseBallBoySubsidy = 0m;
            decimal pBallBoySubsiDy = 0m, pBallBoyFee = 0m, pCildrenCaddieSubsidy = 0m, pSpouseCaddieSubsidy = 0m;
            decimal pCaddieSubsidy = 0m, pGreenFee = 0m, pGolfCartFee=0m;
            if (instance.MemberID != null)
            {
                pMemberID = Convert.ToInt32(instance.MemberID);
            }
            if (instance.CaddieID != null)
            {
                pCaddieID = Convert.ToInt32(instance.CaddieID);
            }
            if (instance.BallBoyID != null)
            {
                pBallBoyIDID = Convert.ToInt32(instance.BallBoyID);
            }
            if (instance.HoleTypeID != null)
            {
                pHoleTypeID = Convert.ToInt32(instance.HoleTypeID);
            }
            instance.BallBoySubsidy = 0;
            instance.BallBoyFee = 0;
            instance.CaddieFee = 0;
            instance.CaddieSubsidy = 0;
            instance.GameFeeAmount = 0;
            instance.GolfCartFee = 0;
            instance.GreenFee = 0;
            instance.GameFeeAmount = 0;
            instance.RegDate = DateTime.Now;
            
            string strSelect = "";
            if (!String.IsNullOrEmpty(instance.MemberID.ToString()))
            {
                object total = null;
                using (SqlText memInfo = new SqlText(@"select MemberID,MemberCategoryID,MemberStatusID,MemberOfType,HandiCap from MemberInfo where MemberID= @MemberID"))
                {
                    #region [Read Data]
                    memInfo.AddParameter("@MemberID", instance.MemberID);
                    if (memInfo.Read())
                    {
                        if (memInfo["MemberCategoryID"] != DBNull.Value)
                        {
                            pMemberCategoryID = Convert.ToInt32(memInfo["MemberCategoryID"].ToString());
                        }
                        if (memInfo["MemberStatusID"] != DBNull.Value)
                        {
                            pMemberStatusID = Convert.ToInt32(memInfo["MemberStatusID"].ToString());
                        }
                        if (memInfo["MemberOfType"] != DBNull.Value)
                        {
                            pMemberOfType = Convert.ToString(memInfo["MemberOfType"].ToString());
                        }
                        if (memInfo["HandiCap"] != DBNull.Value)
                        {
                            pHandiCap = Convert.ToInt32(memInfo["HandiCap"].ToString());
                        }
                    }
                    #endregion
                    ///UpdateFieldValue("ShipName", findCustomer["ContactName"]);
                    ///
                    #region [Game Fees]
                    //object total = null;
                    using (SqlText Fees = new SqlText(@"select " +
                    "    GreenFee,CaddieFee,CaddieSubsidy,SpouseCaddieSubsidy,CildrenCaddieSubsidy, " +
                    "    BallBoyFee,BallBoySubsiDy,SpouseBallBoySubsidy,ChildrenBallBoySubsidy, " +
                    "    GolfCartFee from dbo.GameFee " +
                    "   where  " +
                    "    MemberCategoryID = @MemberCategoryID " +
                    "    And MemberStatusID = @MemberStatusID " +
                    "    And HoleTypeID = @HoleTypeID "
                    ))
                    {
                        #region [Read Data]
                        Fees.AddParameter("@MemberCategoryID", pMemberCategoryID);
                        Fees.AddParameter("@MemberStatusID", pMemberStatusID);
                        Fees.AddParameter("@HoleTypeID", pHoleTypeID);
                        if (Fees.Read())
                        {
                            if (Fees["GolfCartFee"] != DBNull.Value)
                            {
                                pGolfCartFee = Convert.ToDecimal(Fees["GolfCartFee"].ToString());
                            }
                            if (Fees["ChildrenBallBoySubsidy"] != DBNull.Value)
                            {
                                pChildrenBallBoySubsidy = Convert.ToDecimal(Fees["ChildrenBallBoySubsidy"].ToString());
                            }
                            if (Fees["SpouseBallBoySubsidy"] != DBNull.Value)
                            {
                                pSpouseBallBoySubsidy = Convert.ToDecimal(Fees["SpouseBallBoySubsidy"].ToString());
                            }
                            if (Fees["BallBoySubsiDy"] != DBNull.Value)
                            {
                                pBallBoySubsiDy = Convert.ToDecimal(Fees["BallBoySubsiDy"].ToString());
                            }
                            if (Fees["BallBoyFee"] != DBNull.Value)
                            {
                                pBallBoyFee = Convert.ToDecimal(Fees["BallBoyFee"].ToString());
                                instance.BallBoyFee = pBallBoyFee;
                            }


                            if (Fees["GreenFee"] != DBNull.Value)
                            {
                                pGreenFee = Convert.ToDecimal(Fees["GreenFee"].ToString());
                                instance.GreenFee = pGreenFee;
                            }
                            if (Fees["CaddieFee"] != DBNull.Value)
                            {
                                pCaddieFee = Convert.ToDecimal(Fees["CaddieFee"].ToString());
                                instance.CaddieFee = pCaddieFee;
                            }
                            if (Fees["CaddieSubsidy"] != DBNull.Value)
                            {
                                pCaddieSubsidy = Convert.ToDecimal(Fees["CaddieSubsidy"].ToString());
                                instance.CaddieSubsidy = pCaddieSubsidy;
                            }
                            if (Fees["SpouseCaddieSubsidy"] != DBNull.Value)
                            {
                                pSpouseCaddieSubsidy = Convert.ToDecimal(Fees["SpouseCaddieSubsidy"].ToString());
                            }
                            if (Fees["CildrenCaddieSubsidy"] != DBNull.Value)
                            {
                                pCildrenCaddieSubsidy = Convert.ToDecimal(Fees["CildrenCaddieSubsidy"].ToString());
                            }
                            #region [BallBoy Fee]
                            if (pBallBoyIDID > 0)
                            {
                                instance.BallBoyFee = pBallBoyFee;
                                if (pMemberOfType == "Full Member")
                                {
                                    instance.BallBoySubsidy = pBallBoySubsiDy;
                                }
                                if (pMemberOfType == "Spouse Of Member")
                                {
                                    instance.BallBoySubsidy = pSpouseBallBoySubsidy;
                                }
                                if (pMemberOfType == "Children Of Member")
                                {
                                    instance.BallBoySubsidy = pChildrenBallBoySubsidy;
                                }
                            }
                            #endregion
                            #region [Caddie Fee]
                            if (pCaddieID > 0)
                            {
                                instance.CaddieFee = pCaddieFee;
                                if (pMemberOfType == "Full Member")
                                {
                                    instance.CaddieSubsidy = pCaddieSubsidy;
                                }
                                if (pMemberOfType == "Spouse Of Member")
                                {
                                    instance.CaddieSubsidy = pSpouseCaddieSubsidy;
                                }
                                if (pMemberOfType == "Children Of Member")
                                {
                                    instance.CaddieSubsidy = pCildrenCaddieSubsidy;
                                }
                            }
                            if (pMemberOfType == "Full Member")
                            {
                                instance.TotalAmount = (pCaddieFee + pGreenFee + pBallBoyFee + pGolfCartFee) - (pCaddieSubsidy + pBallBoySubsiDy);
                            }
                            if (pMemberOfType == "Spouse Of Member")
                            {
                                instance.TotalAmount = (pCaddieFee + pGreenFee + pBallBoyFee + pGolfCartFee) - (pSpouseCaddieSubsidy + pSpouseBallBoySubsidy);
                            }
                            if (pMemberOfType == "Children Of Member")
                            {
                                instance.TotalAmount = (pCaddieFee + pGreenFee + pBallBoyFee + pGolfCartFee) - (pCildrenCaddieSubsidy + pChildrenBallBoySubsidy);
                            }
                            #endregion
                        }
                        #endregion
                        ///UpdateFieldValue("ShipName", findCustomer["ContactName"]);
                    }
                    #endregion
                }
            }


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

    }
}
