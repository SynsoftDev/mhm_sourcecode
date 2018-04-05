using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal;
using MHMDal.Interfaces;
using MHMDal.Repository;
using MHMDal.Models;
using System.Data;
using System.Diagnostics;

namespace MHMBLL
{
    public class FamilySheetCalculation
    {
        MHMCache MHMCache = new MHMCache();
        IPlanAttributeMaster planAttributeMaster = new PlanAttributeMasterRepo();
        IPlanBenefitMaster planBenefitMaster = new PlanBenefitMasterRepo();
        IMHMBenefitMappingMaster mhmPlanBenefitMappingMaster = new MHMBenefitMappingMasterRepo();
        IMHMBenefitCostByAreaMaster mhmBenefitCostByArea = new MHMBenefitCostByAreaMasterRepo();
        IIssuerMaster issuerMaster = new IssuerMasterRepo();
        IHSAFunding hsaFunding = new HSAFundingRepo();
        IMHMCommonBenefitMaster commonBenefit = new MHMCommonBenefitMasterRepo();

        public void Tier1MOOP(PlanBenefitMst MHMBenefit, bool MedicalDrugMaximumOutofPocketIntegrated, int NoOfMember,
            decimal PotentialPmt, DataRow Row1, bool NonEmbeddedOOPLimits, bool IsRx, bool IsHSA,
            decimal? PlanLmt_Md_Ind_MOOP_1, decimal? PlanLmt_Md_Fam_MOOP_1, decimal? PlanLmt_Rx_Ind_MOOP_1, decimal? PlanLmt_Rx_Fam_MOOP_1,
            decimal? PlanLmt_Td_Ind_MOOP_1, decimal? PlanLmt_Td_Fam_MOOP_1,
            ref decimal? RT_BAL_Rx_Fam_MOOP_1, ref decimal? RT_BAL_Md_Ind_MOOP_1, ref decimal? RT_BAL_Md_Fam_MOOP_1,
            ref decimal? RT_BAL_Rx_Ind_MOOP_1
            //newline
            , ref decimal? RT_BAL_Td_Ind_MOOP_1, ref decimal? RT_BAL_Td_Fam_MOOP_1)
        {
            if (!(bool)MHMBenefit.IsExclFromInnMOOP)
            {
                //Calculate MOOP
                if (MedicalDrugMaximumOutofPocketIntegrated)
                {
                    #region TRUE

                    if (NoOfMember == 1)
                    {
                        var RemainingIndividualOOPLimit = PlanLmt_Td_Ind_MOOP_1 - RT_BAL_Td_Ind_MOOP_1;
                        if (PotentialPmt <= RemainingIndividualOOPLimit)
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                            RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + PotentialPmt;
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                            RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                        }
                    }
                    else
                    {
                        if (NonEmbeddedOOPLimits == true)
                        {
                            var RemainingFamilyOOPLimit = PlanLmt_Td_Fam_MOOP_1 - RT_BAL_Td_Fam_MOOP_1;
                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + PotentialPmt;
                            }
                            else
                            {

                                Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + RemainingFamilyOOPLimit;
                            }
                        }
                        else
                        {
                            var RemainingFamilyOOPLimit = PlanLmt_Td_Fam_MOOP_1 - RT_BAL_Td_Fam_MOOP_1;
                            var RemainingIndividualOOPLimit = PlanLmt_Td_Ind_MOOP_1 - RT_BAL_Td_Ind_MOOP_1;

                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                if (PotentialPmt <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                    RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + PotentialPmt;
                                    RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + PotentialPmt;
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + RemainingIndividualOOPLimit;
                                    RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                                }
                            }
                            else
                            {
                                if (RemainingFamilyOOPLimit <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                    RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + RemainingFamilyOOPLimit;
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + RemainingIndividualOOPLimit;
                                }
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region FALSE

                    if (NoOfMember == 1)
                    {
                        var RemainingIndividualOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_1 - RT_BAL_Rx_Ind_MOOP_1 : PlanLmt_Md_Ind_MOOP_1 - RT_BAL_Md_Ind_MOOP_1;

                        RemainingIndividualOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_1, RT_BAL_Md_Ind_MOOP_1, RemainingIndividualOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                        if (PotentialPmt <= RemainingIndividualOOPLimit)
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + PotentialPmt;
                            }
                            else
                            {
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + PotentialPmt;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                            }
                        }
                    }
                    else
                    {
                        if (NonEmbeddedOOPLimits == true)
                        {
                            var RemainingFamilyOOPLimit = IsRx ? PlanLmt_Rx_Fam_MOOP_1 - RT_BAL_Rx_Fam_MOOP_1 : PlanLmt_Md_Fam_MOOP_1 - RT_BAL_Md_Fam_MOOP_1;

                            RemainingFamilyOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_1, RT_BAL_Md_Fam_MOOP_1, RemainingFamilyOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + PotentialPmt;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + PotentialPmt;
                                }
                            }
                            else
                            {

                                Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingFamilyOOPLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingFamilyOOPLimit;
                                }
                            }
                        }
                        else
                        {
                            var RemainingFamilyOOPLimit = IsRx ? PlanLmt_Rx_Fam_MOOP_1 - RT_BAL_Rx_Fam_MOOP_1 : PlanLmt_Md_Fam_MOOP_1 - RT_BAL_Md_Fam_MOOP_1;
                            var RemainingIndividualOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_1 - RT_BAL_Rx_Ind_MOOP_1 : PlanLmt_Md_Ind_MOOP_1 - RT_BAL_Md_Ind_MOOP_1;

                            RemainingFamilyOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_1, RT_BAL_Md_Fam_MOOP_1, RemainingFamilyOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                            RemainingIndividualOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_1, RT_BAL_Md_Ind_MOOP_1, RemainingIndividualOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);


                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                if (PotentialPmt <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + PotentialPmt;
                                        RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + PotentialPmt;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + PotentialPmt;
                                        RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + PotentialPmt;
                                    }
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingIndividualOOPLimit;
                                        RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingIndividualOOPLimit;
                                        RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                                    }
                                }
                            }
                            else
                            {
                                if (RemainingFamilyOOPLimit <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingFamilyOOPLimit;
                                        RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingFamilyOOPLimit;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingFamilyOOPLimit;
                                        RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingFamilyOOPLimit;
                                    }
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingIndividualOOPLimit;
                                        RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingIndividualOOPLimit;
                                        RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualOOPLimit;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                }
            }
        }

        public void Tier2MOOP(PlanBenefitMst MHMBenefit, bool MedicalDrugMaximumOutofPocketIntegrated, int NoOfMember,
            decimal PotentialPmt, DataRow Row1, bool NonEmbeddedOOPLimits, bool IsRx, bool IsHSA,
            decimal? PlanLmt_Md_Ind_MOOP_2, decimal? PlanLmt_Md_Fam_MOOP_2, decimal? PlanLmt_Rx_Ind_MOOP_2, decimal? PlanLmt_Rx_Fam_MOOP_2,
            decimal? PlanLmt_Td_Ind_MOOP_2, decimal? PlanLmt_Td_Fam_MOOP_2,
            ref decimal? RT_BAL_Rx_Fam_MOOP_2, ref decimal? RT_BAL_Md_Ind_MOOP_2, ref decimal? RT_BAL_Md_Fam_MOOP_2,
            ref decimal? RT_BAL_Rx_Ind_MOOP_2
            //newline
            , ref decimal? RT_BAL_Td_Ind_MOOP_2, ref decimal? RT_BAL_Td_Fam_MOOP_2)
        {
            if (!(bool)MHMBenefit.IsExclFromInnMOOP)
            {
                //Calculate MOOP
                if (MedicalDrugMaximumOutofPocketIntegrated)
                {
                    #region TRUE

                    if (NoOfMember == 1)
                    {
                        var RemainingIndividualOOPLimit = PlanLmt_Td_Ind_MOOP_2 - RT_BAL_Td_Ind_MOOP_2;
                        if (PotentialPmt <= RemainingIndividualOOPLimit)
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                            RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + PotentialPmt;
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                            RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                        }
                    }
                    else
                    {
                        if (NonEmbeddedOOPLimits == true)
                        {
                            var RemainingFamilyOOPLimit = PlanLmt_Td_Fam_MOOP_2 - RT_BAL_Td_Fam_MOOP_2;
                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + PotentialPmt;
                            }
                            else
                            {

                                Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + RemainingFamilyOOPLimit;
                            }
                        }
                        else
                        {
                            var RemainingFamilyOOPLimit = PlanLmt_Td_Fam_MOOP_2 - RT_BAL_Td_Fam_MOOP_2;
                            var RemainingIndividualOOPLimit = PlanLmt_Td_Ind_MOOP_2 - RT_BAL_Td_Ind_MOOP_2;

                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                if (PotentialPmt <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                    RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + PotentialPmt;
                                    RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + PotentialPmt;
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + RemainingIndividualOOPLimit;
                                    RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                                }
                            }
                            else
                            {
                                if (RemainingFamilyOOPLimit <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                    RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + RemainingFamilyOOPLimit;
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + RemainingIndividualOOPLimit;
                                }
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region FALSE

                    if (NoOfMember == 1)
                    {
                        var RemainingIndividualOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_2 - RT_BAL_Rx_Ind_MOOP_2 : PlanLmt_Md_Ind_MOOP_2 - RT_BAL_Md_Ind_MOOP_2;
                        
                        RemainingIndividualOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_2, RT_BAL_Md_Ind_MOOP_2, RemainingIndividualOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                        if (PotentialPmt <= RemainingIndividualOOPLimit)
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + PotentialPmt;
                            }
                            else
                            {
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + PotentialPmt;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                            }
                        }
                    }
                    else
                    {
                        if (NonEmbeddedOOPLimits == true)
                        {
                            var RemainingFamilyOOPLimit = IsRx ? PlanLmt_Rx_Fam_MOOP_2 - RT_BAL_Rx_Fam_MOOP_2 : PlanLmt_Md_Fam_MOOP_2 - RT_BAL_Md_Fam_MOOP_2;

                            RemainingFamilyOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_2, RT_BAL_Md_Fam_MOOP_2, RemainingFamilyOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                            
                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + PotentialPmt;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + PotentialPmt;
                                }
                            }
                            else
                            {

                                Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingFamilyOOPLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingFamilyOOPLimit;
                                }
                            }
                        }
                        else
                        {
                            var RemainingFamilyOOPLimit = IsRx ? PlanLmt_Rx_Fam_MOOP_2 - RT_BAL_Rx_Fam_MOOP_2 : PlanLmt_Md_Fam_MOOP_2 - RT_BAL_Md_Fam_MOOP_2;
                            var RemainingIndividualOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_2 - RT_BAL_Rx_Ind_MOOP_2 : PlanLmt_Md_Ind_MOOP_2 - RT_BAL_Md_Ind_MOOP_2;

                            RemainingFamilyOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_2, RT_BAL_Md_Fam_MOOP_2, RemainingFamilyOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                            RemainingIndividualOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_2, RT_BAL_Md_Ind_MOOP_2, RemainingIndividualOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                            
                            if (PotentialPmt <= RemainingFamilyOOPLimit)
                            {
                                if (PotentialPmt <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = PotentialPmt;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + PotentialPmt;
                                        RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + PotentialPmt;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + PotentialPmt;
                                        RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + PotentialPmt;
                                    }
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingIndividualOOPLimit;
                                        RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingIndividualOOPLimit;
                                        RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                                    }
                                }
                            }
                            else
                            {
                                if (RemainingFamilyOOPLimit <= RemainingIndividualOOPLimit)  //tested
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingFamilyOOPLimit;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingFamilyOOPLimit;
                                        RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingFamilyOOPLimit;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingFamilyOOPLimit;
                                        RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingFamilyOOPLimit;
                                    }
                                }
                                else
                                {
                                    Row1["TotalPaymentsuptoOOPLimit"] = RemainingIndividualOOPLimit;
                                    if (IsRx)
                                    {
                                        RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingIndividualOOPLimit;
                                        RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                                    }
                                    else
                                    {
                                        RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingIndividualOOPLimit;
                                        RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualOOPLimit;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                }
            }
        }

        public void Tier1DedLimit(bool MedicalDrugMaximumOutofPocketIntegrated, bool MedicalDrugDeductiblesIntegrated, int NoOfMember, bool IsRx, bool IsHSA,
            decimal PotentialPmt, DataRow Row1,
            decimal? PlanLmt_Md_Ind_Ded_1, decimal? PlanLmt_Md_Fam_Ded_1, decimal? PlanLmt_Rx_Ind_Ded_1,
            decimal? PlanLmt_Rx_Fam_Ded_1, decimal? PlanLmt_Md_Ind_MOOP_1, decimal? PlanLmt_Md_Fam_MOOP_1,
            decimal? PlanLmt_Rx_Ind_MOOP_1, decimal? PlanLmt_Rx_Fam_MOOP_1,
            //newline
            decimal? PlanLmt_Td_Ind_Ded_1, decimal? PlanLmt_Td_Fam_Ded_1, decimal? PlanLmt_Td_Ind_MOOP_1, decimal? PlanLmt_Td_Fam_MOOP_1,
            ref decimal? RT_BAL_Md_Ind_Ded_1, ref decimal? RT_BAL_Md_Fam_Ded_1, ref decimal? RT_BAL_Rx_Ind_Ded_1,
            ref decimal? RT_BAL_Rx_Fam_Ded_1, ref decimal? RT_BAL_Md_Ind_MOOP_1, ref decimal? RT_BAL_Md_Fam_MOOP_1,
            ref decimal? RT_BAL_Rx_Ind_MOOP_1, ref decimal? RT_BAL_Rx_Fam_MOOP_1,
            //newline
            ref decimal? RT_BAL_Td_Ind_Ded_1, ref decimal? RT_BAL_Td_Fam_Ded_1, ref decimal? RT_BAL_Td_Ind_MOOP_1, ref decimal? RT_BAL_Td_Fam_MOOP_1)
        {
            // Check if it is inetegrated medical drug Deductible and MOOP
            if (MedicalDrugDeductiblesIntegrated)
            {
                #region TRUE

                //Check if it is individual member case
                if (NoOfMember == 1)
                {
                    //pick up Deductible limits as per individual deductible and MOOP integrated
                    var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                    var RemainingIndividualMOOPLimit = PlanLmt_Td_Ind_MOOP_1 - RT_BAL_Td_Ind_MOOP_1;

                    if (!MedicalDrugMaximumOutofPocketIntegrated)
                    {
                        if (IsRx)
                            RemainingIndividualMOOPLimit = PlanLmt_Rx_Ind_MOOP_1 - RT_BAL_Rx_Ind_MOOP_1;
                        else
                            RemainingIndividualMOOPLimit = PlanLmt_Md_Ind_MOOP_1 - RT_BAL_Md_Ind_MOOP_1;

                        RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_1, RT_BAL_Md_Ind_MOOP_1, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                    }


                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;

                    if (PotentialPmt <= RemainingIndividualDeductibleLimit)
                    {
                        Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;

                        RT_BAL_Td_Ind_Ded_1 = RT_BAL_Td_Ind_Ded_1 + PotentialPmt;



                        if (!MedicalDrugMaximumOutofPocketIntegrated)
                        {
                            if (IsRx)
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + PotentialPmt;
                            else
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + PotentialPmt;

                        }
                        else
                        {
                            RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + PotentialPmt;
                        }

                    }
                    else
                    {
                        Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                        RT_BAL_Td_Ind_Ded_1 = RT_BAL_Td_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                        if (!MedicalDrugMaximumOutofPocketIntegrated)
                        {
                            if (IsRx)
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                            else
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;

                        }
                        else
                        {
                            RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                        }

                    }
                    // Add MOOP balance 
                }
                else
                {
                    var RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_1 - RT_BAL_Td_Fam_Ded_1;
                    var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                    var RemainingIndividualMOOPLimit = PlanLmt_Td_Ind_MOOP_1 - RT_BAL_Td_Ind_MOOP_1;
                    var RemainingFamilyMOOPLimit = PlanLmt_Td_Fam_MOOP_1 - RT_BAL_Td_Fam_MOOP_1;

                    if (!MedicalDrugMaximumOutofPocketIntegrated)
                    {
                        if (IsRx)
                        {
                            RemainingIndividualMOOPLimit = PlanLmt_Rx_Ind_MOOP_1 - RT_BAL_Rx_Ind_MOOP_1;
                            RemainingFamilyMOOPLimit = PlanLmt_Rx_Fam_MOOP_1 - RT_BAL_Rx_Fam_MOOP_1;
                        }
                        else
                        {
                            RemainingIndividualMOOPLimit = PlanLmt_Md_Ind_MOOP_1 - RT_BAL_Md_Ind_MOOP_1;
                            RemainingFamilyMOOPLimit = PlanLmt_Md_Fam_MOOP_1 - RT_BAL_Md_Fam_MOOP_1;
                        }

                        RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_1, RT_BAL_Md_Ind_MOOP_1, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                        RemainingFamilyMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_1, RT_BAL_Md_Fam_MOOP_1, RemainingFamilyMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                     }


                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;
                    RemainingFamilyDeductibleLimit = RemainingFamilyDeductibleLimit > RemainingFamilyMOOPLimit ? RemainingFamilyMOOPLimit : RemainingFamilyDeductibleLimit;

                    if (PotentialPmt <= RemainingFamilyDeductibleLimit)
                    {
                        if (PotentialPmt <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;
                            RT_BAL_Td_Fam_Ded_1 = RT_BAL_Td_Fam_Ded_1 + PotentialPmt;
                            RT_BAL_Td_Ind_Ded_1 = RT_BAL_Td_Ind_Ded_1 + PotentialPmt;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + PotentialPmt;
                                    RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + PotentialPmt;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + PotentialPmt;
                                    RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + PotentialPmt;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + PotentialPmt;
                                RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + PotentialPmt;
                            }


                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Fam_Ded_1 = RT_BAL_Td_Fam_Ded_1 + RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Ind_Ded_1 = RT_BAL_Td_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                            }


                        }
                    }
                    else
                    {
                        if (RemainingFamilyDeductibleLimit <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingFamilyDeductibleLimit;
                            RT_BAL_Td_Fam_Ded_1 = RT_BAL_Td_Fam_Ded_1 + RemainingFamilyDeductibleLimit;
                            RT_BAL_Td_Ind_Ded_1 = RT_BAL_Td_Ind_Ded_1 + RemainingFamilyDeductibleLimit;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingFamilyDeductibleLimit;
                                    RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingFamilyDeductibleLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingFamilyDeductibleLimit;
                                    RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingFamilyDeductibleLimit;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + RemainingFamilyDeductibleLimit;
                            }

                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Fam_Ded_1 = RT_BAL_Td_Fam_Ded_1 + RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Ind_Ded_1 = RT_BAL_Td_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_1 = RT_BAL_Td_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Td_Ind_MOOP_1 = RT_BAL_Td_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                            }

                        }
                    }

                }

                #endregion
            }
            else
            {
                #region FALSE
                if (NoOfMember == 1)
                {
                    //pick up Deductible limits as per individual deductible and MOOP integrated
                    var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;
                    var RemainingIndividualMOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_1 - RT_BAL_Rx_Ind_MOOP_1 : PlanLmt_Md_Ind_MOOP_1 - RT_BAL_Md_Ind_MOOP_1;

                    RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_1, RT_BAL_Md_Ind_MOOP_1, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;

                    if (PotentialPmt <= RemainingIndividualDeductibleLimit)
                    {
                        Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;
                        if (IsRx)
                        {
                            RT_BAL_Rx_Ind_Ded_1 = RT_BAL_Rx_Ind_Ded_1 + PotentialPmt;

                            RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + PotentialPmt;
                        }
                        else
                        {
                            RT_BAL_Md_Ind_Ded_1 = RT_BAL_Md_Ind_Ded_1 + PotentialPmt;

                            RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + PotentialPmt;
                        }
                    }
                    else
                    {
                        Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                        if (IsRx)
                        {
                            RT_BAL_Rx_Ind_Ded_1 = RT_BAL_Rx_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                            RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                        }
                        else
                        {
                            RT_BAL_Md_Ind_Ded_1 = RT_BAL_Md_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                            RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                        }
                    }
                }
                else
                {
                    var RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_1 - RT_BAL_Rx_Fam_Ded_1 : PlanLmt_Md_Fam_Ded_1 - RT_BAL_Md_Fam_Ded_1;
                    var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;


                    var RemainingIndividualMOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_1 - RT_BAL_Rx_Ind_MOOP_1 : PlanLmt_Md_Ind_MOOP_1 - RT_BAL_Md_Ind_MOOP_1;
                    var RemainingFamilyMOOPLimit = IsRx ? PlanLmt_Rx_Fam_MOOP_1 - RT_BAL_Rx_Fam_MOOP_1 : PlanLmt_Md_Fam_MOOP_1 - RT_BAL_Md_Fam_MOOP_1;

                    RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_1, RT_BAL_Md_Ind_MOOP_1, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                    RemainingFamilyMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_1, RT_BAL_Md_Fam_MOOP_1, RemainingFamilyMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;
                    RemainingFamilyDeductibleLimit = RemainingFamilyDeductibleLimit > RemainingFamilyMOOPLimit ? RemainingFamilyMOOPLimit : RemainingFamilyDeductibleLimit;


                    if (PotentialPmt <= RemainingFamilyDeductibleLimit)
                    {
                        if (PotentialPmt <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_1 = RT_BAL_Rx_Fam_Ded_1 + PotentialPmt;
                                RT_BAL_Rx_Ind_Ded_1 = RT_BAL_Rx_Ind_Ded_1 + PotentialPmt;

                                RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + PotentialPmt;
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + PotentialPmt;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_1 = RT_BAL_Md_Fam_Ded_1 + PotentialPmt;
                                RT_BAL_Md_Ind_Ded_1 = RT_BAL_Md_Ind_Ded_1 + PotentialPmt;

                                RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + PotentialPmt;
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + PotentialPmt;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_1 = RT_BAL_Rx_Fam_Ded_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_Ded_1 = RT_BAL_Rx_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_1 = RT_BAL_Md_Fam_Ded_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_Ded_1 = RT_BAL_Md_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                            }
                        }
                    }
                    else
                    {
                        if (RemainingFamilyDeductibleLimit <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingFamilyDeductibleLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_1 = RT_BAL_Rx_Fam_Ded_1 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Rx_Ind_Ded_1 = RT_BAL_Rx_Ind_Ded_1 + RemainingFamilyDeductibleLimit;

                                RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingFamilyDeductibleLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_1 = RT_BAL_Md_Fam_Ded_1 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Md_Ind_Ded_1 = RT_BAL_Md_Ind_Ded_1 + RemainingFamilyDeductibleLimit;

                                RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingFamilyDeductibleLimit;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_1 = RT_BAL_Rx_Fam_Ded_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_Ded_1 = RT_BAL_Rx_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Rx_Fam_MOOP_1 = RT_BAL_Rx_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_MOOP_1 = RT_BAL_Rx_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_1 = RT_BAL_Md_Fam_Ded_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_Ded_1 = RT_BAL_Md_Ind_Ded_1 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Md_Fam_MOOP_1 = RT_BAL_Md_Fam_MOOP_1 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_MOOP_1 = RT_BAL_Md_Ind_MOOP_1 + RemainingIndividualDeductibleLimit;
                            }
                        }
                    }
                }


                #endregion
            }
        }

        public void Tier2DedLimit(bool MedicalDrugMaximumOutofPocketIntegrated, bool MedicalDrugDeductiblesIntegrated, int NoOfMember, bool IsRx, bool IsHSA,
            decimal PotentialPmt, DataRow Row1,
            decimal? PlanLmt_Md_Ind_Ded_2, decimal? PlanLmt_Md_Fam_Ded_2, decimal? PlanLmt_Rx_Ind_Ded_2,
            decimal? PlanLmt_Rx_Fam_Ded_2, decimal? PlanLmt_Md_Ind_MOOP_2, decimal? PlanLmt_Md_Fam_MOOP_2,
            decimal? PlanLmt_Rx_Ind_MOOP_2, decimal? PlanLmt_Rx_Fam_MOOP_2,
            //newline
            decimal? PlanLmt_Td_Ind_Ded_2, decimal? PlanLmt_Td_Fam_Ded_2, decimal? PlanLmt_Td_Ind_MOOP_2, decimal? PlanLmt_Td_Fam_MOOP_2,
            ref decimal? RT_BAL_Md_Ind_Ded_2, ref decimal? RT_BAL_Md_Fam_Ded_2, ref decimal? RT_BAL_Rx_Ind_Ded_2,
            ref decimal? RT_BAL_Rx_Fam_Ded_2, ref decimal? RT_BAL_Md_Ind_MOOP_2, ref decimal? RT_BAL_Md_Fam_MOOP_2,
            ref decimal? RT_BAL_Rx_Ind_MOOP_2, ref decimal? RT_BAL_Rx_Fam_MOOP_2,
            //newline
            ref decimal? RT_BAL_Td_Ind_Ded_2, ref decimal? RT_BAL_Td_Fam_Ded_2, ref decimal? RT_BAL_Td_Ind_MOOP_2, ref decimal? RT_BAL_Td_Fam_MOOP_2)
        {
            // Check if it is inetegrated medical drug Deductible and MOOP
            if (MedicalDrugDeductiblesIntegrated)
            {
                #region TRUE

                //Check if it is individual member case
                if (NoOfMember == 1)
                {
                    //pick up Deductible limits as per individual deductible and MOOP integrated
                    var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;
                    var RemainingIndividualMOOPLimit = PlanLmt_Td_Ind_MOOP_2 - RT_BAL_Td_Ind_MOOP_2;

                    if (!MedicalDrugMaximumOutofPocketIntegrated)
                    {
                        if (IsRx)
                            RemainingIndividualMOOPLimit = PlanLmt_Rx_Ind_MOOP_2 - RT_BAL_Rx_Ind_MOOP_2;
                        else
                            RemainingIndividualMOOPLimit = PlanLmt_Md_Ind_MOOP_2 - RT_BAL_Md_Ind_MOOP_2;

                        RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_2, RT_BAL_Md_Ind_MOOP_2, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                    }
                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;



                    if (PotentialPmt <= RemainingIndividualDeductibleLimit)
                    {
                        Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;
                        RT_BAL_Td_Ind_Ded_2 = RT_BAL_Td_Ind_Ded_2 + PotentialPmt;

                        if (!MedicalDrugMaximumOutofPocketIntegrated)
                        {
                            if (IsRx)
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + PotentialPmt;
                            else
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + PotentialPmt;

                        }
                        else
                        {
                            RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + PotentialPmt;
                        }
                    }
                    else
                    {
                        Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                        RT_BAL_Td_Ind_Ded_2 = RT_BAL_Td_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                        if (!MedicalDrugMaximumOutofPocketIntegrated)
                        {
                            if (IsRx)
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                            else
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;

                        }
                        else
                        {
                            RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                        }
                    }
                }
                else
                {
                    var RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_2 - RT_BAL_Td_Fam_Ded_2;
                    var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;

                    var RemainingIndividualMOOPLimit = PlanLmt_Td_Ind_MOOP_2 - RT_BAL_Td_Ind_MOOP_2;
                    var RemainingFamilyMOOPLimit = PlanLmt_Td_Fam_MOOP_2 - RT_BAL_Td_Fam_MOOP_2;

                    if (!MedicalDrugMaximumOutofPocketIntegrated)
                    {
                        if (IsRx)
                        {
                            RemainingIndividualMOOPLimit = PlanLmt_Rx_Ind_MOOP_2 - RT_BAL_Rx_Ind_MOOP_2;
                            RemainingFamilyMOOPLimit = PlanLmt_Rx_Fam_MOOP_2 - RT_BAL_Rx_Fam_MOOP_2;
                        }
                        else
                        {
                            RemainingIndividualMOOPLimit = PlanLmt_Md_Ind_MOOP_2 - RT_BAL_Md_Ind_MOOP_2;
                            RemainingFamilyMOOPLimit = PlanLmt_Md_Fam_MOOP_2 - RT_BAL_Md_Fam_MOOP_2;
                        }

                        RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_2, RT_BAL_Md_Ind_MOOP_2, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                        RemainingFamilyMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_2, RT_BAL_Md_Fam_MOOP_2, RemainingFamilyMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                    }

                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;
                    RemainingFamilyDeductibleLimit = RemainingFamilyDeductibleLimit > RemainingFamilyMOOPLimit ? RemainingFamilyMOOPLimit : RemainingFamilyDeductibleLimit;

                    if (PotentialPmt <= RemainingFamilyDeductibleLimit)
                    {
                        if (PotentialPmt <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;
                            RT_BAL_Td_Fam_Ded_2 = RT_BAL_Td_Fam_Ded_2 + PotentialPmt;
                            RT_BAL_Td_Ind_Ded_2 = RT_BAL_Td_Ind_Ded_2 + PotentialPmt;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + PotentialPmt;
                                    RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + PotentialPmt;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + PotentialPmt;
                                    RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + PotentialPmt;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + PotentialPmt;
                                RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + PotentialPmt;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Fam_Ded_2 = RT_BAL_Td_Fam_Ded_2 + RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Ind_Ded_2 = RT_BAL_Td_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                            }
                        }
                    }
                    else
                    {
                        if (RemainingFamilyDeductibleLimit <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingFamilyDeductibleLimit;
                            RT_BAL_Td_Fam_Ded_2 = RT_BAL_Td_Fam_Ded_2 + RemainingFamilyDeductibleLimit;
                            RT_BAL_Td_Ind_Ded_2 = RT_BAL_Td_Ind_Ded_2 + RemainingFamilyDeductibleLimit;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingFamilyDeductibleLimit;
                                    RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingFamilyDeductibleLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingFamilyDeductibleLimit;
                                    RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingFamilyDeductibleLimit;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + RemainingFamilyDeductibleLimit;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Fam_Ded_2 = RT_BAL_Td_Fam_Ded_2 + RemainingIndividualDeductibleLimit;
                            RT_BAL_Td_Ind_Ded_2 = RT_BAL_Td_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                            if (!MedicalDrugMaximumOutofPocketIntegrated)
                            {
                                if (IsRx)
                                {
                                    RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                                }
                                else
                                {
                                    RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                    RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                                }
                            }
                            else
                            {
                                RT_BAL_Td_Fam_MOOP_2 = RT_BAL_Td_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Td_Ind_MOOP_2 = RT_BAL_Td_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region FALSE
                if (NoOfMember == 1)
                {
                    //pick up Deductible limits as per individual deductible and MOOP integrated
                    var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;

                    var RemainingIndividualMOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_2 - RT_BAL_Rx_Ind_MOOP_2 : PlanLmt_Md_Ind_MOOP_2 - RT_BAL_Md_Ind_MOOP_2;

                    RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_2, RT_BAL_Md_Ind_MOOP_2, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);

                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;


                    if (PotentialPmt <= RemainingIndividualDeductibleLimit)
                    {
                        Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;
                        if (IsRx)
                        {
                            RT_BAL_Rx_Ind_Ded_2 = RT_BAL_Rx_Ind_Ded_2 + PotentialPmt;

                            RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + PotentialPmt;
                        }
                        else
                        {
                            RT_BAL_Md_Ind_Ded_2 = RT_BAL_Md_Ind_Ded_2 + PotentialPmt;

                            RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + PotentialPmt;
                        }
                    }
                    else
                    {
                        Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                        if (IsRx)
                        {
                            RT_BAL_Rx_Ind_Ded_2 = RT_BAL_Rx_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                            RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                        }
                        else
                        {
                            RT_BAL_Md_Ind_Ded_2 = RT_BAL_Md_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                            RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                        }
                    }
                }
                else
                {
                    var RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_2 - RT_BAL_Rx_Fam_Ded_2 : PlanLmt_Md_Fam_Ded_2 - RT_BAL_Md_Fam_Ded_2;
                    var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;

                    var RemainingIndividualMOOPLimit = IsRx ? PlanLmt_Rx_Ind_MOOP_2 - RT_BAL_Rx_Ind_MOOP_2 : PlanLmt_Md_Ind_MOOP_2 - RT_BAL_Md_Ind_MOOP_2;
                    var RemainingFamilyMOOPLimit = IsRx ? PlanLmt_Rx_Fam_MOOP_2 - RT_BAL_Rx_Fam_MOOP_2 : PlanLmt_Md_Fam_MOOP_2 - RT_BAL_Md_Fam_MOOP_2;

                    RemainingIndividualMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Ind_MOOP_2, RT_BAL_Md_Ind_MOOP_2, RemainingIndividualMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);
                    RemainingFamilyMOOPLimit = ValidateLegalMooplimit(RT_BAL_Rx_Fam_MOOP_2, RT_BAL_Md_Fam_MOOP_2, RemainingFamilyMOOPLimit, IsHSA, NoOfMember == 1 ? 1 : 2);


                    //we have to check if moop remaining limit is less than deductable limit then only moop remaining limit should be considered
                    RemainingIndividualDeductibleLimit = RemainingIndividualDeductibleLimit > RemainingIndividualMOOPLimit ? RemainingIndividualMOOPLimit : RemainingIndividualDeductibleLimit;
                    RemainingFamilyDeductibleLimit = RemainingFamilyDeductibleLimit > RemainingFamilyMOOPLimit ? RemainingFamilyMOOPLimit : RemainingFamilyDeductibleLimit;


                    if (PotentialPmt <= RemainingFamilyDeductibleLimit)
                    {
                        if (PotentialPmt <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = PotentialPmt;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_2 = RT_BAL_Rx_Fam_Ded_2 + PotentialPmt;
                                RT_BAL_Rx_Ind_Ded_2 = RT_BAL_Rx_Ind_Ded_2 + PotentialPmt;

                                RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + PotentialPmt;
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + PotentialPmt;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_2 = RT_BAL_Md_Fam_Ded_2 + PotentialPmt;
                                RT_BAL_Md_Ind_Ded_2 = RT_BAL_Md_Ind_Ded_2 + PotentialPmt;

                                RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + PotentialPmt;
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + PotentialPmt;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_2 = RT_BAL_Rx_Fam_Ded_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_Ded_2 = RT_BAL_Rx_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_2 = RT_BAL_Md_Fam_Ded_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_Ded_2 = RT_BAL_Md_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                            }
                        }
                    }
                    else
                    {
                        if (RemainingFamilyDeductibleLimit <= RemainingIndividualDeductibleLimit)  //tested
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingFamilyDeductibleLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_2 = RT_BAL_Rx_Fam_Ded_2 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Rx_Ind_Ded_2 = RT_BAL_Rx_Ind_Ded_2 + RemainingFamilyDeductibleLimit;

                                RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingFamilyDeductibleLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_2 = RT_BAL_Md_Fam_Ded_2 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Md_Ind_Ded_2 = RT_BAL_Md_Ind_Ded_2 + RemainingFamilyDeductibleLimit;

                                RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingFamilyDeductibleLimit;
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingFamilyDeductibleLimit;
                            }
                        }
                        else
                        {
                            Row1["TotalPaymentsuptoDeductible"] = RemainingIndividualDeductibleLimit;
                            if (IsRx)
                            {
                                RT_BAL_Rx_Fam_Ded_2 = RT_BAL_Rx_Fam_Ded_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_Ded_2 = RT_BAL_Rx_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Rx_Fam_MOOP_2 = RT_BAL_Rx_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Rx_Ind_MOOP_2 = RT_BAL_Rx_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                            }
                            else
                            {
                                RT_BAL_Md_Fam_Ded_2 = RT_BAL_Md_Fam_Ded_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_Ded_2 = RT_BAL_Md_Ind_Ded_2 + RemainingIndividualDeductibleLimit;

                                RT_BAL_Md_Fam_MOOP_2 = RT_BAL_Md_Fam_MOOP_2 + RemainingIndividualDeductibleLimit;
                                RT_BAL_Md_Ind_MOOP_2 = RT_BAL_Md_Ind_MOOP_2 + RemainingIndividualDeductibleLimit;
                            }
                        }
                    }
                }


                #endregion
            }
        }

        public decimal CheckNull(decimal? value)
        {
            if (value != null) return (decimal)value; else return 0;
        }

        public decimal? ValidateLegalMooplimit(decimal? BAL_Rx, decimal? BAL_Md, decimal? RemianingLimit, bool IsHSA, int UsagesCode)
        {
            var HSAFundingFromCache = MHMCache.GetMyCachedItem("HSAFunding");
            IEnumerable<HSAFunding> lstHsaFunding;
            if (HSAFundingFromCache != null)
            {
                lstHsaFunding = (IEnumerable<HSAFunding>)HSAFundingFromCache;
            }
            else
            {
                lstHsaFunding = hsaFunding.GetHSAFunding();
            }

            var MaxMooplimit = IsHSA ? lstHsaFunding.Where(r => r.Type == UsagesCode).Select(t => t.HSAMOOPLimit).FirstOrDefault() : lstHsaFunding.Where(r => r.Type == UsagesCode).Select(t => t.QHPMOOPLimit).FirstOrDefault();
            var TotalConsumedMoop = BAL_Rx + BAL_Md;
            var RemainingMoopLimit = MaxMooplimit - TotalConsumedMoop;
            return RemianingLimit > RemainingMoopLimit ? RemainingMoopLimit : RemianingLimit;
        }

        public FamilySheetResult CalculateFamilySheetNew(PlanAttributeMst objPlanAttribute, int UsageCode, int NoOfMember, List<FamilyMemberUsesList> familyMemberUsesList, IEnumerable<PlanBenefitMst> objPlanBenefitMaster, IEnumerable<MHMBenefitCostByAreaMst> objMHMBenefitCostByAreaMaster, bool? NonEmbeddedOOPLimits, int TierIntention, JobMaster JobDetails)
        {
            try
            {
                //deciding factor it's Integrated Md/Rx Ded  (plan) OR Integrated Md/Rx MOOP  (plan)
                bool MedicalDrugDeductiblesIntegrated = (bool)objPlanAttribute.MedicalDrugDeductiblesIntegrated;
                bool MedicalDrugMaximumOutofPocketIntegrated = (bool)objPlanAttribute.MedicalDrugMaximumOutofPocketIntegrated;

                if (NonEmbeddedOOPLimits == null) { NonEmbeddedOOPLimits = false; }
                #region Limit and Balance Declaration


                decimal? PlanLmt_Md_Ind_Ded_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.MEHBDedInnTier1FamilyPerPerson);
                decimal? PlanLmt_Md_Fam_Ded_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.MEHBDedInnTier1FamilyPerGroup);
                decimal? PlanLmt_Md_Ind_MOOP_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.MEHBInnTier1FamilyPerPersonMOOP);
                decimal? PlanLmt_Md_Fam_MOOP_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.MEHBInnTier1FamilyPerGroupMOOP);
                decimal? PlanLmt_Rx_Ind_Ded_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.DEHBDedInnTier1FamilyPerPerson);
                decimal? PlanLmt_Rx_Fam_Ded_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.DEHBDedInnTier1FamilyPerGroup);
                decimal? PlanLmt_Rx_Ind_MOOP_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.DEHBInnTier1FamilyPerPersonMOOP);
                decimal? PlanLmt_Rx_Fam_MOOP_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.DEHBInnTier1FamilyPerGroupMOOP);
                decimal? PlanLmt_Td_Ind_Ded_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.TEHBDedInnTier1FamilyPerPerson);
                decimal? PlanLmt_Td_Fam_Ded_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.TEHBDedInnTier1FamilyPerGroup);
                decimal? PlanLmt_Td_Ind_MOOP_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.TEHBInnTier1FamilyPerPersonMOOP);
                decimal? PlanLmt_Td_Fam_MOOP_1 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.TEHBInnTier1FamilyPerGroupMOOP);


                decimal? PlanLmt_Md_Ind_Ded_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBDedInnTier2Individual) : CheckNull(objPlanAttribute.MEHBDedInnTier2FamilyPerPerson);
                decimal? PlanLmt_Md_Fam_Ded_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBDedInnTier2Individual) : CheckNull(objPlanAttribute.MEHBDedInnTier2FamilyPerGroup);
                decimal? PlanLmt_Md_Ind_MOOP_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBInnTier2IndividualMOOP) : CheckNull(objPlanAttribute.MEHBInnTier2FamilyPerPersonMOOP);
                decimal? PlanLmt_Md_Fam_MOOP_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.MEHBInnTier2IndividualMOOP) : CheckNull(objPlanAttribute.MEHBInnTier2FamilyPerGroupMOOP);
                decimal? PlanLmt_Rx_Ind_Ded_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBDedInnTier2Individual) : CheckNull(objPlanAttribute.DEHBDedInnTier2FamilyPerPerson);
                decimal? PlanLmt_Rx_Fam_Ded_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBDedInnTier2Individual) : CheckNull(objPlanAttribute.DEHBDedInnTier2FamilyPerGroup);
                decimal? PlanLmt_Rx_Ind_MOOP_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBInnTier2IndividualMOOP) : CheckNull(objPlanAttribute.DEHBInnTier2FamilyPerPersonMOOP);
                decimal? PlanLmt_Rx_Fam_MOOP_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.DEHBInnTier2IndividualMOOP) : CheckNull(objPlanAttribute.DEHBInnTier2FamilyPerGroupMOOP);
                decimal? PlanLmt_Td_Ind_Ded_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBDedInnTier2Individual) : CheckNull(objPlanAttribute.TEHBDedInnTier2FamilyPerPerson);
                decimal? PlanLmt_Td_Fam_Ded_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBDedInnTier2Individual) : CheckNull(objPlanAttribute.TEHBDedInnTier2FamilyPerGroup);
                decimal? PlanLmt_Td_Ind_MOOP_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBInnTier2IndividualMOOP) : CheckNull(objPlanAttribute.TEHBInnTier2FamilyPerPersonMOOP);
                decimal? PlanLmt_Td_Fam_MOOP_2 = UsageCode == 1 ? CheckNull(objPlanAttribute.TEHBInnTier2IndividualMOOP) : CheckNull(objPlanAttribute.TEHBInnTier2FamilyPerGroupMOOP);


                Dictionary<string, decimal?> Limits = new Dictionary<string, decimal?>();

                if (!MedicalDrugDeductiblesIntegrated)
                {
                    //PlanLmt_Md_Ind_Ded_1 = PlanLmt_Rx_Ind_Ded_1 = Constants.MaximumLegalOOPSingle < PlanLmt_Md_Ind_Ded_1 + PlanLmt_Rx_Ind_Ded_1 ? Constants.MaximumLegalOOPSingle : PlanLmt_Md_Ind_Ded_1 + PlanLmt_Rx_Ind_Ded_1;
                    //PlanLmt_Md_Fam_Ded_1 = PlanLmt_Rx_Fam_Ded_1 = Constants.MaximumLegalOOPFamily < PlanLmt_Md_Fam_Ded_1 + PlanLmt_Rx_Fam_Ded_1 ? Constants.MaximumLegalOOPFamily : PlanLmt_Md_Fam_Ded_1 + PlanLmt_Rx_Fam_Ded_1;

                    //PlanLmt_Md_Ind_Ded_2 = PlanLmt_Rx_Ind_Ded_2 = Constants.MaximumLegalOOPSingle < PlanLmt_Md_Ind_Ded_2 + PlanLmt_Rx_Ind_Ded_2 ? Constants.MaximumLegalOOPSingle : PlanLmt_Md_Ind_Ded_2 + PlanLmt_Rx_Ind_Ded_2;
                    //PlanLmt_Md_Fam_Ded_2 = PlanLmt_Rx_Fam_Ded_2 = Constants.MaximumLegalOOPFamily < PlanLmt_Md_Fam_Ded_2 + PlanLmt_Rx_Fam_Ded_2 ? Constants.MaximumLegalOOPFamily : PlanLmt_Md_Fam_Ded_2 + PlanLmt_Rx_Fam_Ded_2;

                    Limits.Add("DeductibleSingle", TierIntention == 1 ? CheckNull(objPlanAttribute.DEHBDedInnTier1Individual) + CheckNull(objPlanAttribute.MEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.DEHBDedInnTier2Individual) + CheckNull(objPlanAttribute.MEHBDedInnTier2Individual));
                    Limits.Add("DeductibleFamilyPerPerson", TierIntention == 1 ? CheckNull(objPlanAttribute.DEHBDedInnTier1FamilyPerPerson) + CheckNull(objPlanAttribute.MEHBDedInnTier1FamilyPerPerson) : CheckNull(objPlanAttribute.DEHBDedInnTier2FamilyPerPerson) + CheckNull(objPlanAttribute.MEHBDedInnTier2FamilyPerPerson));
                    Limits.Add("DeductibleFamilyPerGroup", TierIntention == 1 ? CheckNull(objPlanAttribute.DEHBDedInnTier1FamilyPerGroup) + CheckNull(objPlanAttribute.MEHBDedInnTier1FamilyPerGroup) : CheckNull(objPlanAttribute.DEHBDedInnTier2FamilyPerGroup) + CheckNull(objPlanAttribute.MEHBDedInnTier2FamilyPerGroup));
                }
                else
                {
                    Limits.Add("DeductibleSingle", TierIntention == 1 ? CheckNull(objPlanAttribute.TEHBDedInnTier1Individual) : CheckNull(objPlanAttribute.TEHBDedInnTier2Individual));
                    Limits.Add("DeductibleFamilyPerPerson", TierIntention == 1 ? CheckNull(objPlanAttribute.TEHBDedInnTier1FamilyPerPerson) : CheckNull(objPlanAttribute.TEHBDedInnTier2FamilyPerPerson));
                    Limits.Add("DeductibleFamilyPerGroup", TierIntention == 1 ? CheckNull(objPlanAttribute.TEHBDedInnTier1FamilyPerGroup) : CheckNull(objPlanAttribute.TEHBDedInnTier2FamilyPerGroup));
                }

                if (!MedicalDrugMaximumOutofPocketIntegrated)
                {
                    //PlanLmt_Md_Ind_MOOP_1 = PlanLmt_Rx_Ind_MOOP_1 = Constants.MaximumLegalOOPSingle < PlanLmt_Md_Ind_MOOP_1 + PlanLmt_Rx_Ind_MOOP_1 ? Constants.MaximumLegalOOPSingle : PlanLmt_Md_Ind_MOOP_1 + PlanLmt_Rx_Ind_MOOP_1;
                    //PlanLmt_Md_Fam_MOOP_1 = PlanLmt_Rx_Fam_MOOP_1 = Constants.MaximumLegalOOPFamily < PlanLmt_Md_Fam_MOOP_1 + PlanLmt_Rx_Fam_MOOP_1 ? Constants.MaximumLegalOOPFamily : PlanLmt_Md_Fam_MOOP_1 + PlanLmt_Rx_Fam_MOOP_1;

                    //PlanLmt_Md_Ind_MOOP_2 = PlanLmt_Rx_Ind_MOOP_2 = Constants.MaximumLegalOOPSingle < PlanLmt_Md_Ind_MOOP_2 + PlanLmt_Rx_Ind_MOOP_2 ? Constants.MaximumLegalOOPSingle : PlanLmt_Md_Ind_MOOP_2 + PlanLmt_Rx_Ind_MOOP_2;
                    //PlanLmt_Md_Fam_MOOP_2 = PlanLmt_Rx_Fam_MOOP_2 = Constants.MaximumLegalOOPFamily < PlanLmt_Md_Fam_MOOP_2 + PlanLmt_Rx_Fam_MOOP_2 ? Constants.MaximumLegalOOPFamily : PlanLmt_Md_Fam_MOOP_2 + PlanLmt_Rx_Fam_MOOP_2;

                    Limits.Add("OPLSingle", TierIntention == 1 ? CheckNull(objPlanAttribute.DEHBInnTier1IndividualMOOP) + CheckNull(objPlanAttribute.MEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.DEHBInnTier2IndividualMOOP) + CheckNull(objPlanAttribute.MEHBInnTier2IndividualMOOP));
                    Limits.Add("OPLFamilyPerPerson", TierIntention == 1 ? CheckNull(objPlanAttribute.DEHBInnTier1FamilyPerPersonMOOP) + CheckNull(objPlanAttribute.MEHBInnTier1FamilyPerPersonMOOP) : CheckNull(objPlanAttribute.DEHBInnTier2FamilyPerPersonMOOP) + CheckNull(objPlanAttribute.MEHBInnTier2FamilyPerPersonMOOP));
                    Limits.Add("OPLFamilyPerGroup", TierIntention == 1 ? CheckNull(objPlanAttribute.DEHBInnTier1FamilyPerGroupMOOP) + CheckNull(objPlanAttribute.MEHBInnTier1FamilyPerGroupMOOP) : CheckNull(objPlanAttribute.DEHBInnTier2FamilyPerGroupMOOP) + CheckNull(objPlanAttribute.MEHBInnTier2FamilyPerGroupMOOP));
                }
                else
                {
                    Limits.Add("OPLSingle", TierIntention == 1 ? CheckNull(objPlanAttribute.TEHBInnTier1IndividualMOOP) : CheckNull(objPlanAttribute.TEHBInnTier2IndividualMOOP));
                    Limits.Add("OPLFamilyPerPerson", TierIntention == 1 ? CheckNull(objPlanAttribute.TEHBInnTier1FamilyPerPersonMOOP) : CheckNull(objPlanAttribute.TEHBInnTier2FamilyPerPersonMOOP));
                    Limits.Add("OPLFamilyPerGroup", TierIntention == 1 ? CheckNull(objPlanAttribute.TEHBInnTier1FamilyPerGroupMOOP) : CheckNull(objPlanAttribute.TEHBInnTier2FamilyPerGroupMOOP));
                }

                decimal? RT_BAL_Md_Fam_Ded_1 = 0;
                decimal? RT_BAL_Md_Fam_MOOP_1 = 0;
                decimal? RT_BAL_Rx_Fam_Ded_1 = 0;
                decimal? RT_BAL_Rx_Fam_MOOP_1 = 0;
                decimal? RT_BAL_Td_Fam_Ded_1 = 0;
                decimal? RT_BAL_Td_Fam_MOOP_1 = 0;
                decimal? RT_BAL_Md_Fam_Ded_2 = 0;
                decimal? RT_BAL_Md_Fam_MOOP_2 = 0;
                decimal? RT_BAL_Rx_Fam_Ded_2 = 0;
                decimal? RT_BAL_Rx_Fam_MOOP_2 = 0;
                decimal? RT_BAL_Td_Fam_Ded_2 = 0;
                decimal? RT_BAL_Td_Fam_MOOP_2 = 0;

                decimal? RT_BAL_Md_Ind_Ded_1 = 0;
                decimal? RT_BAL_Md_Ind_MOOP_1 = 0;
                decimal? RT_BAL_Rx_Ind_Ded_1 = 0;
                decimal? RT_BAL_Rx_Ind_MOOP_1 = 0;
                decimal? RT_BAL_Td_Ind_Ded_1 = 0;
                decimal? RT_BAL_Td_Ind_MOOP_1 = 0;
                decimal? RT_BAL_Md_Ind_Ded_2 = 0;
                decimal? RT_BAL_Md_Ind_MOOP_2 = 0;
                decimal? RT_BAL_Rx_Ind_Ded_2 = 0;
                decimal? RT_BAL_Rx_Ind_MOOP_2 = 0;
                decimal? RT_BAL_Td_Ind_Ded_2 = 0;
                decimal? RT_BAL_Td_Ind_MOOP_2 = 0;

                #endregion

                IEnumerable<MHMBenefitCostByAreaMst> objMHMBenefitCostByAreas;
                objMHMBenefitCostByAreas = objMHMBenefitCostByAreaMaster;


                //var lstPlanBenifits = objPlanBenefitMaster.Select(r => new PlanBenefits() { BenefitId = (long)r.MHMBenefitId, CoIns = (Decimal)r.CoinsInnTier1, CoPay = (Decimal)r.CopayInnTier1, CopayPaymentFinal = Convert.ToString(r.IsSubjToDedTier1), CostSharingType = r.CostSharingType }).ToList();
                //var lstPlanBenifits = objPlanBenefitMaster.ToList();
                var lstPlanBenifits = objPlanBenefitMaster.AsQueryable()
                    .LeftJoin(
                    commonBenefit.GetMHMCommonBenefitMaster().AsQueryable(),
                    benefit => benefit.MHMBenefitId,
                    commonbenefit => commonbenefit.MHMBenefitID,
                    (benefit, commonbenefit) => new
                    {
                        MHMBenefit = benefit,
                        MHMCommonBenefit = commonbenefit
                    }
                    ).ToList();



                //var tets=lstPlanBenifits1.FirstOrDefault().MHMBenefit.

                DataSet ds = new DataSet();

                DataTable FinalTotal = new DataTable("FinalTotal");
                FinalTotal.Columns.Add("BenifitId", typeof(int));
                FinalTotal.Columns.Add("ActualCostofServices", typeof(decimal));
                FinalTotal.Columns.Add("ExcludedQuantity", typeof(decimal));
                FinalTotal.Columns.Add("ExcludedAmount", typeof(decimal));
                FinalTotal.Columns.Add("CopayPaid", typeof(decimal));
                FinalTotal.Columns.Add("PotentialPmt", typeof(decimal));
                FinalTotal.Columns.Add("TotalPaymentsuptoDeductible", typeof(decimal));
                FinalTotal.Columns.Add("RunningTotalofPaymentsuptoDeductible", typeof(decimal));
                FinalTotal.Columns.Add("CoInsurancePaid", typeof(decimal));
                FinalTotal.Columns.Add("TotalPaymentsuptoOOPLimit", typeof(decimal));
                FinalTotal.Columns.Add("RunningTotalofPaymentsuptoOOPLimit", typeof(decimal));
                FinalTotal.Columns.Add("EmployeePayment", typeof(decimal));
                FinalTotal.Columns.Add("InsurancePays", typeof(decimal));

                decimal FamilyHRAReimbursementTotal = 0;

                for (int f = 1; f <= NoOfMember; f++)
                {
                    RT_BAL_Md_Ind_Ded_1 = 0;
                    RT_BAL_Md_Ind_MOOP_1 = 0;
                    RT_BAL_Rx_Ind_Ded_1 = 0;
                    RT_BAL_Rx_Ind_MOOP_1 = 0;
                    RT_BAL_Td_Ind_Ded_1 = 0;
                    RT_BAL_Td_Ind_MOOP_1 = 0;
                    RT_BAL_Md_Ind_Ded_2 = 0;
                    RT_BAL_Md_Ind_MOOP_2 = 0;
                    RT_BAL_Rx_Ind_Ded_2 = 0;
                    RT_BAL_Rx_Ind_MOOP_2 = 0;
                    RT_BAL_Td_Ind_Ded_2 = 0;
                    RT_BAL_Td_Ind_MOOP_2 = 0;

                    //fetching benefitusage of each member
                    var MemberBenefitUsage = familyMemberUsesList.Where(r => r.FamilyMemberNumber == f).First();

                    DataTable dt = new DataTable("Family" + f);
                    dt.Columns.Add("BenifitId", typeof(int));
                    dt.Columns.Add("ActualCostofServices", typeof(decimal));
                    dt.Columns.Add("ExcludedQuantity", typeof(decimal));
                    dt.Columns.Add("ExcludedAmount", typeof(decimal));
                    dt.Columns.Add("CopayPaid", typeof(decimal));
                    dt.Columns.Add("PotentialPmt", typeof(decimal));
                    dt.Columns.Add("TotalPaymentsuptoDeductible", typeof(decimal));
                    dt.Columns.Add("RunningTotalofPaymentsuptoDeductible", typeof(decimal));
                    dt.Columns.Add("CoInsurancePaid", typeof(decimal));
                    dt.Columns.Add("RunningTotalofPotentialPayment2", typeof(decimal));
                    dt.Columns.Add("TotalPaymentsuptoOOPLimit", typeof(decimal));
                    dt.Columns.Add("RunningTotalofPaymentsuptoOOPLimit", typeof(decimal));
                    dt.Columns.Add("EmployeePayment", typeof(decimal));
                    dt.Columns.Add("InsurancePays", typeof(decimal));

                    //count of benefit usages
                    int BenefitUsesCount = MemberBenefitUsage.BenefitUses.Count();

                    //order by benefit usages based on benefit id
                    MemberBenefitUsage.BenefitUses = MemberBenefitUsage.BenefitUses.OrderBy(r => r.BenefitId).ToList();

                    for (int i = 0; i < BenefitUsesCount; i++)
                    {
                        if (i == 31)
                        {
                            var test = "test";
                        }
                        DataRow Row1 = dt.NewRow();
                        var Benefit = MemberBenefitUsage.BenefitUses.ElementAt(i);
                        //Change r.MHMBenefit.IsCovered == false then it will go to excluded not covered field. Just amount
                        var benefitData = lstPlanBenifits.Where(r => r.MHMBenefit.MHMBenefitId == Benefit.BenefitId).FirstOrDefault();
                        decimal LmtExcludedAmount = 0;

                        if (benefitData != null)
                        {
                            benefitData.MHMBenefit.IsExclFromInnMOOP = benefitData.MHMBenefit.IsExclFromInnMOOP == null ? false : benefitData.MHMBenefit.IsExclFromInnMOOP;
                            benefitData.MHMBenefit.IsExclFromOonMOOP = benefitData.MHMBenefit.IsExclFromOonMOOP == null ? false : benefitData.MHMBenefit.IsExclFromOonMOOP;

                            var CostSharingType = "";
                            // Decide if we need to select Tier1 or Tier2 CostSharing Type
                            switch (TierIntention)
                            {
                                case 1:
                                    CostSharingType = benefitData.MHMBenefit.CostSharingType1;
                                    break;
                                case 2:
                                    CostSharingType = benefitData.MHMBenefit.CostSharingType2;
                                    break;
                            }

                            if (benefitData.MHMBenefit.IsCovered == true && CostSharingType != "None")
                            {
                                decimal? LimitQuantity = 0;
                                decimal? LimitAmount = 0;

                                if (benefitData.MHMBenefit.CopayInnTier1 == null)
                                {
                                    benefitData.MHMBenefit.CopayInnTier1 = 0;
                                }
                                if (benefitData.MHMBenefit.CoinsInnTier1 == null)
                                {
                                    benefitData.MHMBenefit.CoinsInnTier1 = 0;
                                }
                                if (benefitData.MHMBenefit.CoinsMaxAmt == null)
                                {
                                    benefitData.MHMBenefit.CoinsMaxAmt = 0;
                                }
                                if (benefitData.MHMBenefit.MaxCoinsInnTier1Amt == null)
                                {
                                    benefitData.MHMBenefit.MaxCoinsInnTier1Amt = 0;
                                }
                                if (benefitData.MHMBenefit.CopayInnTier2 == null)
                                {
                                    benefitData.MHMBenefit.CopayInnTier2 = 0;
                                }
                                if (benefitData.MHMBenefit.CoinsInnTier2 == null)
                                {
                                    benefitData.MHMBenefit.CoinsInnTier2 = 0;
                                }
                                if (benefitData.MHMBenefit.MaxCoinsInnTier2Amt == null)
                                {
                                    benefitData.MHMBenefit.MaxCoinsInnTier2Amt = 0;
                                }
                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay == null)
                                {
                                    benefitData.MHMBenefit.MaxQtyBeforeCoPay = 0;
                                }

                                //here we fetch eligible limit qty and Limit Unit
                                if (benefitData.MHMBenefit.LimitUnit == "QtyYear")
                                {
                                    LimitQuantity = benefitData.MHMBenefit.LimitQty;
                                }
                                else if (benefitData.MHMBenefit.LimitUnit == "Qty6mnth")
                                {
                                    LimitQuantity = benefitData.MHMBenefit.LimitQty * 2;
                                }
                                else if (benefitData.MHMBenefit.LimitUnit == "QtyMnth")
                                {
                                    LimitQuantity = benefitData.MHMBenefit.LimitQty * 12;
                                }
                                else if (benefitData.MHMBenefit.LimitUnit == "$Year")
                                {
                                    LimitAmount = benefitData.MHMBenefit.LimitQty;
                                }
                                else if (benefitData.MHMBenefit.LimitUnit == "$6mnth")
                                {
                                    LimitAmount = benefitData.MHMBenefit.LimitQty * 2;
                                }
                                else if (benefitData.MHMBenefit.LimitUnit == "$Mnth")
                                {
                                    LimitAmount = benefitData.MHMBenefit.LimitQty * 12;
                                }

                                Row1["BenifitId"] = Benefit.BenefitId;

                                decimal EligibleQuantity = 0;
                                decimal EligibleAmount = 0;

                                var SingleBenefitCost = Benefit.UsageQty == 0 ? 0 : Benefit.UsageCost / Benefit.UsageQty;

                                if (benefitData.MHMBenefit.LimitUnit != null && benefitData.MHMBenefit.LimitQty != null)
                                {
                                    //Computing eligible quantity & amount and maintain excluded quantity & amount
                                    if (benefitData.MHMBenefit.LimitUnit.Contains("$"))
                                    {
                                        EligibleAmount = Benefit.UsageCost > LimitAmount && LimitAmount != 0 ? (decimal)LimitAmount : Benefit.UsageCost;
                                        LmtExcludedAmount = Convert.ToDecimal(Benefit.UsageCost - EligibleAmount);
                                        Row1["ActualCostofServices"] = EligibleAmount;
                                        EligibleQuantity = EligibleAmount != 0 ? EligibleAmount / SingleBenefitCost : 0;
                                    }
                                    else
                                    {
                                        var BenefitQuantity = Benefit.UsageQty;
                                        //var SingleBenefitCost = Benefit.UsageQty == 0 ? 0 : Benefit.UsageCost / Benefit.UsageQty;

                                        //Computing eligible quantity and maintain excluded quantity 
                                        EligibleQuantity = BenefitQuantity > LimitQuantity && LimitQuantity != 0 ? (decimal)LimitQuantity : BenefitQuantity;
                                        Row1["ExcludedQuantity"] = BenefitQuantity - EligibleQuantity;

                                        //Calculate eligible amount and excluded amount based on eligible quantity
                                        var ElgAmount = EligibleQuantity * SingleBenefitCost;
                                        LmtExcludedAmount = Convert.ToDecimal(Benefit.UsageCost - ElgAmount);

                                        Row1["ActualCostofServices"] = ElgAmount;
                                    }
                                    Row1["ExcludedAmount"] = 0;
                                }
                                else
                                {
                                    Row1["ExcludedQuantity"] = 0;
                                    Row1["ExcludedAmount"] = 0;
                                    Row1["ActualCostofServices"] = Benefit.UsageCost;
                                    EligibleAmount = Benefit.UsageCost;
                                    EligibleQuantity = Benefit.UsageQty;
                                }

                                //Check 
                                bool Tier2Limit = true;
                                if (TierIntention == 2 && CostSharingType == "None")
                                {
                                    Tier2Limit = false;
                                }

                                var CopayPaymentFinal = benefitData.MHMBenefit.IsSubjToDedTier1;

                                //right now hard coded 3 which is for Rx
                                bool IsRx = benefitData.MHMCommonBenefit.CategoryId == 3;
                                //if (MedicalDrugDeductiblesIntegrated)
                                //{ IsRx = false; }

                                Row1["PotentialPmt"] = Row1["ActualCostofServices"];
                                Row1["CopayPaid"] = 0;
                                Row1["CoInsurancePaid"] = 0;
                                Row1["TotalPaymentsuptoOOPLimit"] = 0;
                                Row1["TotalPaymentsuptoDeductible"] = 0;

                                var PotentialPmt = Convert.ToDecimal(Row1["PotentialPmt"]);

                                switch (CostSharingType)
                                {
                                    //Logic to execute Cost sharing type

                                    #region DeductibleOnly

                                    case "DeductibleOnly":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1

                                                // Check if it is inetegrated medical drug Deductible and reduce the deductible and MOOP 
                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + +Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);

                                                // Total paid by employee will be upto deductable limits 
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                //if actual cost of service exceed deductible limit then remaining will be paid by insurance company
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) > (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["ExcludedAmount"])) ? Convert.ToDecimal(Row1["ActualCostofServices"]) - (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["ExcludedAmount"])) : 0;

                                                dt.Rows.Add(Row1);

                                                break;
                                            case 2:

                                                // Check if it is integrated medical drug Deductible and MOOP
                                                //if tier2limit is none then use the deductaible and MOOP of tier1 and reduce it  
                                                if (Tier2Limit)
                                                    Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                else
                                                    Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + +Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);

                                                // Total paid by employee will be deductable limits 
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                //if actual cost of service exceed deductible limit then remaining will be paid by insurance company
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) > (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["ExcludedAmount"])) ? Convert.ToDecimal(Row1["ActualCostofServices"]) - (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["ExcludedAmount"])) : 0;

                                                dt.Rows.Add(Row1);

                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region DeductibleThenCopay

                                    case "DedThenCopay":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1
                                                // Check if it is inetegrated medical drug Deductible and reduce corresponding MOOP 
                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                //Check if deductable limit is covering the actualcost of service
                                                if (Convert.ToDecimal(Row1["ActualCostofServices"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]))
                                                {
                                                    //adjusting quantity based on amount consumed under deductible
                                                    var DedConsumedQuantity = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) != 0 ? Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) / SingleBenefitCost : 0;
                                                    EligibleQuantity = Math.Ceiling(EligibleQuantity - DedConsumedQuantity);

                                                    // Calculate Copay
                                                    var Copay = benefitData.MHMBenefit.CopayInnTier1;

                                                    Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                    
                                                    if (Convert.ToDecimal(Row1["CopayPaid"]) > (EligibleQuantity * SingleBenefitCost))
                                                    {
                                                        Row1["CopayPaid"] = EligibleQuantity * SingleBenefitCost;
                                                    }

                                                    // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP
                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                    //Adjust copay to MOOP
                                                    if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                    {
                                                        Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                    }
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["ExcludedAmount"]));

                                                dt.Rows.Add(Row1);


                                                break;
                                            case 2:

                                                //if Tier2limit is none then use the deductaible and MOOP of tier1 and reduce it  
                                                if (Tier2Limit)
                                                    Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                else
                                                    Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);


                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                //Check if deductable limit is covering the actualcost of service

                                                if (Convert.ToDecimal(Row1["ActualCostofServices"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]))
                                                {
                                                    //adjusting quantity based on amount consumed under deductible
                                                    var DedConsumedQuantity = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) != 0 ? Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) / SingleBenefitCost : 0;
                                                    EligibleQuantity = Math.Ceiling(EligibleQuantity - DedConsumedQuantity);
                                                    // Calculate Copay
                                                    var Copay = benefitData.MHMBenefit.CopayInnTier2;

                                                    Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                    
                                                    if (Convert.ToDecimal(Row1["CopayPaid"]) > (EligibleQuantity * SingleBenefitCost))
                                                    {
                                                        Row1["CopayPaid"] = EligibleQuantity * SingleBenefitCost;
                                                    }

                                                    // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP
                                                    if (Tier2Limit)
                                                        Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                    else
                                                        Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                    //Adjust copay to MOOP
                                                    if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                    {
                                                        Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                    }
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["ExcludedAmount"]));

                                                dt.Rows.Add(Row1);
                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region DeductibleThenCoIns

                                    case "DedThenCoIns":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1
                                                // Check if it is inetegrated medical drug Deductible and reduce MOOP
                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);


                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                //Check if deductable limit is covering the actualcost of service
                                                if (PotentialPmt > Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]))
                                                {
                                                    // Calculate CoIns taking Tier1 cost
                                                    var CoIns = benefitData.MHMBenefit.CoinsInnTier1;
                                                    EligibleQuantity = (PotentialPmt - Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"])) / SingleBenefitCost;

                                                    Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoIns / 100;

                                                    // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                    //Check Max Coinsurance for tier 1
                                                    var TotalMaxCoinsInnTier1 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                    if (TotalMaxCoinsInnTier1 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier1)
                                                    {
                                                        Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier1;
                                                    }

                                                    //Exaust MOOP till it reaches its limit taking CoInsurance amount 
                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                    //Adjust copay to MOOP
                                                    if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                    {
                                                        Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                    }

                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]));

                                                dt.Rows.Add(Row1);


                                                break;
                                            case 2:

                                                // Check if it is inetegrated medical drug Deductible and reduce MOOP
                                                if (Tier2Limit)
                                                    Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                else
                                                    Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, PotentialPmt, Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                //Check if deductable limit is covering the actualcost of service
                                                if (PotentialPmt > Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]))
                                                {
                                                    // Calculate CoIns taking Tier2 cost
                                                    var CoIns = benefitData.MHMBenefit.CoinsInnTier2;
                                                    EligibleQuantity = (PotentialPmt - Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"])) / SingleBenefitCost;
                                                    Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoIns / 100;

                                                    // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                    //Check Max Coinsurance for tier 2
                                                    var TotalMaxCoinsInnTier2 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier2Amt;

                                                    if (TotalMaxCoinsInnTier2 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier2)
                                                    {
                                                        Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier2;
                                                    }

                                                    //Exaust MOOP till it reaches its limit taking CoInsurance amount 
                                                    if (Tier2Limit)
                                                        Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                    else
                                                        Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                    //Adjust copay to MOOP
                                                    if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                    {
                                                        Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                    }

                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - (Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]));

                                                dt.Rows.Add(Row1);



                                                break;
                                        }

                                        break;

                                    #endregion

                                    #region CopayBeforeDedThenNoCharge

                                    case "CopayBeforeDedThenNoCharge":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1
                                                // Check if it is inetegrated medical drug Deductible
                                                if (MedicalDrugDeductiblesIntegrated)
                                                {
                                                    #region TRUE
                                                    //Check if it is individual member case
                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        //Checking if member has any deductible limits. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            // Calculate Copay
                                                            var Copay = benefitData.MHMBenefit.CopayInnTier1;

                                                            // Check max qty before copay if > 0 
                                                            if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                            {
                                                                Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                            }
                                                            else
                                                            {
                                                                Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                            }

                                                            if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                            {
                                                                Row1["CopayPaid"] = PotentialPmt;
                                                            }
                                                            
                                                            //compute and reduce MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                          }


                                                    }
                                                    else
                                                    {
                                                        var RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_1 - RT_BAL_Td_Fam_Ded_1;
                                                        var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)  //tested
                                                            {
                                                                // Calculate Copay
                                                                var Copay = benefitData.MHMBenefit.CopayInnTier1;

                                                                // Check max qty before copay if > 0 
                                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                                {
                                                                    Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                                }
                                                                else
                                                                {
                                                                    Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                                }

                                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                                {
                                                                    Row1["CopayPaid"] = PotentialPmt;
                                                                }
                                                                
                                                                //compute MOOP Tier1
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }

                                                            }

                                                        }

                                                    }

                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region FALSE

                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;
                                                        //Check if member has any deductible limits
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            // Calculate Copay
                                                            var Copay = benefitData.MHMBenefit.CopayInnTier1;

                                                            // Check max qty before copay if > 0 
                                                            if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                            {
                                                                Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                            }
                                                            else
                                                            {
                                                                Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                            }

                                                            if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                            {
                                                                Row1["CopayPaid"] = PotentialPmt;
                                                            }
                                                            
                                                            //compute MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                         }
                                                    }
                                                    else
                                                    {
                                                        var RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_1 - RT_BAL_Rx_Fam_Ded_1 : PlanLmt_Md_Fam_Ded_1 - RT_BAL_Md_Fam_Ded_1;
                                                        var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)
                                                            {
                                                                // Calculate Copay
                                                                var Copay = benefitData.MHMBenefit.CopayInnTier1;

                                                                // Check max qty before copay if > 0 
                                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                                {
                                                                    Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                                }
                                                                else
                                                                {
                                                                    Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                                }

                                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                                {
                                                                    Row1["CopayPaid"] = PotentialPmt;
                                                                }
                                                                
                                                                //compute MOOP Tier1
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }

                                                            }
                                                        }
                                                    }


                                                    #endregion
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]);
                                                //Difference of actual cost of service and copay will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);

                                                break;
                                            case 2:
                                                // Check if it is inetegrated medical drug Deductible
                                                if (MedicalDrugDeductiblesIntegrated)
                                                {
                                                    #region TRUE
                                                    //Check if it is individual member case
                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;
                                                        else
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        //Checking if member has any deductible limits. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            // Calculate Copay
                                                            var Copay = benefitData.MHMBenefit.CopayInnTier2;

                                                            // Check max qty before copay if > 0 
                                                            if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                            {
                                                                Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                            }
                                                            else
                                                            {
                                                                Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                            }

                                                            if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                            {
                                                                Row1["CopayPaid"] = PotentialPmt;
                                                            }
                                                            
                                                            //compute MOOP Tier2
                                                            //if Tier2limit is none then use the deductaible and MOOP of tier1 and reduce it  
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {
                                                        decimal? RemainingFamilyDeductibleLimit = 0;
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                        {
                                                            RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_2 - RT_BAL_Td_Fam_Ded_2;
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;
                                                        }
                                                        else
                                                        {
                                                            RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_1 - RT_BAL_Td_Fam_Ded_1;
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;
                                                        }

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)  //tested
                                                            {
                                                                // Calculate Copay
                                                                var Copay = benefitData.MHMBenefit.CopayInnTier2;

                                                                // Check max qty before copay if > 0 
                                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                                {
                                                                    Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                                }
                                                                else
                                                                {
                                                                    Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                                }

                                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                                {
                                                                    Row1["CopayPaid"] = PotentialPmt;
                                                                }
                                                            
                                                                //compute MOOP Tier2
                                                                if (Tier2Limit)
                                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }
                                                          }
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region FALSE

                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;
                                                        else
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Check if member has any deductible limits
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            // Calculate Copay
                                                            var Copay = benefitData.MHMBenefit.CopayInnTier2;

                                                            // Check max qty before copay if > 0 
                                                            if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                            {
                                                                Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                            }
                                                            else
                                                            {
                                                                Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                            }

                                                            if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                            {
                                                                Row1["CopayPaid"] = PotentialPmt;
                                                            }

                                                            //compute MOOP Tier2
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {

                                                        decimal? RemainingFamilyDeductibleLimit = 0;
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                        {
                                                            RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_2 - RT_BAL_Rx_Fam_Ded_2 : PlanLmt_Md_Fam_Ded_2 - RT_BAL_Md_Fam_Ded_2;
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;
                                                        }
                                                        else
                                                        {
                                                            RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_1 - RT_BAL_Rx_Fam_Ded_1 : PlanLmt_Md_Fam_Ded_1 - RT_BAL_Md_Fam_Ded_1;
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;
                                                        }

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)
                                                            {
                                                                // Calculate Copay
                                                                var Copay = benefitData.MHMBenefit.CopayInnTier2;

                                                                // Check max qty before copay if > 0 
                                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                                {
                                                                    Row1["CopayPaid"] = benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay;
                                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                                }
                                                                else
                                                                {
                                                                    Row1["CopayPaid"] = EligibleQuantity * Copay;
                                                                }

                                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                                {
                                                                    Row1["CopayPaid"] = PotentialPmt;
                                                                }

                                                                //compute MOOP Tier2
                                                                if (Tier2Limit)
                                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }

                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]);
                                                //Difference of actual cost of service and copay will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);

                                                break;

                                        }

                                        break;

                                    #endregion

                                    #region CopayBeforeDedThenCoInsAfterDed DedApplied

                                    case "CPBefCIAftrIsDed":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1

                                                // Calculate Copay
                                                var Copay = benefitData.MHMBenefit.CopayInnTier1;
                                                // Check max qty before copay if > 0 
                                                decimal CopayPaid = 0;
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                    EligibleQuantity = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay);
                                                }

                                                // Check if it is inetegrated medical drug Deductible
                                                if (MedicalDrugDeductiblesIntegrated)
                                                {
                                                    #region TRUE

                                                    //Check if it is individual member case
                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier1 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            if (TotalMaxCoinsInnTier1 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier1)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier1;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_1 - RT_BAL_Td_Fam_Ded_1;
                                                        var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 && RemainingFamilyDeductibleLimit > 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier1 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            if (TotalMaxCoinsInnTier1 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier1)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier1;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }


                                                    }

                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region FALSE

                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier1 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            if (TotalMaxCoinsInnTier1 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier1)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier1;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_1 - RT_BAL_Rx_Fam_Ded_1 : PlanLmt_Md_Fam_Ded_1 - RT_BAL_Md_Fam_Ded_1;
                                                        var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Check if member has any deductible limits
                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 && RemainingFamilyDeductibleLimit > 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier1 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            if (TotalMaxCoinsInnTier1 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier1)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier1;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }

                                                    #endregion
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //Difference of actual cost of service and copay will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) - Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                dt.Rows.Add(Row1);

                                                break;
                                            case 2:

                                                // Calculate Copay
                                                var Copay2 = benefitData.MHMBenefit.CopayInnTier2;
                                                // Check max qty before copay if > 0 
                                                decimal CopayPaid2 = 0;
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                    EligibleQuantity = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay);
                                                }

                                                // Check if it is inetegrated medical drug Deductible
                                                if (MedicalDrugDeductiblesIntegrated)
                                                {
                                                    #region TRUE
                                                    //Check if it is individual member case
                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;
                                                        else
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay2) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            if (Tier2Limit)
                                                                Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier2 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            if (TotalMaxCoinsInnTier2 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier2)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier2;
                                                            }

                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        decimal? RemainingFamilyDeductibleLimit = 0;
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                        {
                                                            RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_2 - RT_BAL_Td_Fam_Ded_2;
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;
                                                        }
                                                        else
                                                        {
                                                            RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_1 - RT_BAL_Td_Fam_Ded_1;
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;
                                                        }

                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 && RemainingFamilyDeductibleLimit > 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay2) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            if (Tier2Limit)
                                                                Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier2 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            if (TotalMaxCoinsInnTier2 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier2)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier2;
                                                            }

                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region FALSE

                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;
                                                        else
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay2) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            //Vaibhav commented
                                                            //Row1["CopayPaid"] = CopayPaid;
                                                            if (Tier2Limit)
                                                                Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier2 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            if (TotalMaxCoinsInnTier2 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier2)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier2;
                                                            }

                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        decimal? RemainingFamilyDeductibleLimit = 0;
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                        {
                                                            RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_2 - RT_BAL_Rx_Fam_Ded_2 : PlanLmt_Md_Fam_Ded_2 - RT_BAL_Md_Fam_Ded_2;
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;
                                                        }
                                                        else
                                                        {
                                                            RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_1 - RT_BAL_Rx_Fam_Ded_1 : PlanLmt_Md_Fam_Ded_1 - RT_BAL_Md_Fam_Ded_1;
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;
                                                        }

                                                        //Check if member has any deductible limits
                                                        decimal DedEligibleQty = RemainingIndividualDeductibleLimit != 0 && RemainingFamilyDeductibleLimit > 0 ? Convert.ToDecimal(RemainingIndividualDeductibleLimit / Copay2) : 0;
                                                        DedEligibleQty = DedEligibleQty <= EligibleQuantity ? DedEligibleQty : EligibleQuantity;
                                                        EligibleQuantity = Math.Ceiling(EligibleQuantity - DedEligibleQty);

                                                        if (DedEligibleQty > 0)
                                                        {
                                                            if (Tier2Limit)
                                                                Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(DedEligibleQty * Copay2), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                        }

                                                        if (EligibleQuantity > 0)
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = EligibleQuantity * SingleBenefitCost * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            var TotalMaxCoinsInnTier2 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            if (TotalMaxCoinsInnTier2 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier2)
                                                            {
                                                                Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier2;
                                                            }

                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }


                                                    #endregion
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //Difference of actual cost of service and copay will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["CoInsurancePaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);

                                                break;

                                        }

                                        break;

                                    #endregion

                                    #region CopayBeforeDedThenCoInsAfterDed DedNotApplied

                                    case "CPBefCIAftrDedNo":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1

                                                // Calculate Copay
                                                var Copay = benefitData.MHMBenefit.CopayInnTier1;
                                                // Check max qty before copay if > 0 
                                                decimal CopayPaid = 0;
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    CopayPaid = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay);
                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                }
                                                else
                                                {
                                                    CopayPaid = Convert.ToDecimal(EligibleQuantity * Copay);
                                                }

                                                if (CopayPaid > PotentialPmt)
                                                {
                                                    CopayPaid = PotentialPmt;
                                                }
                                                
                                                // Check if it is inetegrated medical drug Deductible
                                                if (MedicalDrugDeductiblesIntegrated)
                                                {
                                                    #region TRUE

                                                    //Check if it is individual member case
                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            Row1["CopayPaid"] = CopayPaid;
                                                            //Tier1DedLimit(MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                       }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier1Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier1Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_1 - RT_BAL_Td_Fam_Ded_1;
                                                        var RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)  //tested
                                                            {
                                                                Row1["CopayPaid"] = CopayPaid;
                                                                //Tier1DedLimit(MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1);
                                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                                //compute and reduce MOOP Tier1
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }

                                                         }
                                                            else
                                                            {
                                                                Row1["ExcludedAmount"] = 0;
                                                                // Calculate Coins
                                                                var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier1 / 100;
                                                                // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                                //Check Max Coinsurance for tier 1
                                                                if (benefitData.MHMBenefit.MaxCoinsInnTier1Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier1Amt)
                                                                {
                                                                    Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                    Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                }

                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier1Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier1Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }

                                                    }

                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region FALSE

                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            Row1["CopayPaid"] = CopayPaid;
                                                            //Tier1DedLimit(MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier1Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier1Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_1 - RT_BAL_Rx_Fam_Ded_1 : PlanLmt_Md_Fam_Ded_1 - RT_BAL_Md_Fam_Ded_1;
                                                        var RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)  //tested
                                                            {
                                                                Row1["CopayPaid"] = CopayPaid;
                                                                //Tier1DedLimit(MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1);
                                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                                //compute and reduce MOOP Tier1
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }

                                                           }
                                                            else
                                                            {
                                                                Row1["ExcludedAmount"] = 0;
                                                                // Calculate Coins
                                                                var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier1 / 100;
                                                                // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                                //Check Max Coinsurance for tier 1
                                                                if (benefitData.MHMBenefit.MaxCoinsInnTier1Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier1Amt)
                                                                {
                                                                    Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                    Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                }

                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier1 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier1Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier1Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                            }

                                                            Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }

                                                    #endregion
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //Difference of actual cost of service and copay will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["CoInsurancePaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);

                                                break;
                                            case 2:

                                                // Calculate Copay
                                                var Copay2 = benefitData.MHMBenefit.CopayInnTier2;
                                                // Check max qty before copay if > 0 
                                                decimal CopayPaid2 = 0;
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    CopayPaid2 = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay * Copay2);
                                                    //Row1["CoInsurancePaid"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                }
                                                else
                                                {
                                                    CopayPaid2 = Convert.ToDecimal(EligibleQuantity * Copay2);
                                                }

                                                if (CopayPaid2 > PotentialPmt)
                                                {
                                                    CopayPaid2 = PotentialPmt;
                                                }
                                                //var CopayBenefitQuantity2 = benefitData.MHMBenefit.MaxQtyBeforeCoPay != 0 ? EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay : EligibleQuantity;
                                                //var CopayPaid2 = CopayBenefitQuantity2 * Copay2;

                                                // Check if it is inetegrated medical drug Deductible
                                                if (MedicalDrugDeductiblesIntegrated)
                                                {
                                                    #region TRUE
                                                    //Check if it is individual member case
                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;
                                                        else
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;


                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            Row1["CopayPaid"] = CopayPaid2;
                                                            if (Tier2Limit)
                                                                Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier2Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier2Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            }

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        decimal? RemainingFamilyDeductibleLimit = 0;
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                        {
                                                            RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_2 - RT_BAL_Td_Fam_Ded_2;
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_2 - RT_BAL_Td_Ind_Ded_2;
                                                        }
                                                        else
                                                        {
                                                            RemainingFamilyDeductibleLimit = PlanLmt_Td_Fam_Ded_1 - RT_BAL_Td_Fam_Ded_1;
                                                            RemainingIndividualDeductibleLimit = PlanLmt_Td_Ind_Ded_1 - RT_BAL_Td_Ind_Ded_1;
                                                        }

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)  //tested
                                                            {
                                                                Row1["CopayPaid"] = CopayPaid2;
                                                                if (Tier2Limit)
                                                                    Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                                //compute and reduce MOOP Tier1
                                                                if (Tier2Limit)
                                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }

                                                          }
                                                            else
                                                            {
                                                                Row1["ExcludedAmount"] = 0;
                                                                // Calculate Coins
                                                                var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier2 / 100;
                                                                // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                                //Check Max Coinsurance for tier 1
                                                                if (benefitData.MHMBenefit.MaxCoinsInnTier2Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier2Amt)
                                                                {
                                                                    Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                    Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                }

                                                                if (Tier2Limit)
                                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier2Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier2Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            }

                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }

                                                    }

                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region FALSE

                                                    if (NoOfMember == 1)
                                                    {
                                                        //pick up Deductible limits as per individual deductible and MOOP integrated
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;
                                                        else
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;

                                                        //Checking if member has any deductible limits to adjust copay. 
                                                        //Copay will be paid by employee till MOOP but his Deductible limit will not be reduced.
                                                        if (RemainingIndividualDeductibleLimit > 0)
                                                        {
                                                            Row1["CopayPaid"] = CopayPaid2;
                                                            if (Tier2Limit)
                                                                Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);
                                                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }

                                                       }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier2Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier2Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            }

                                                            //compute and reduce MOOP Tier1
                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {

                                                        decimal? RemainingFamilyDeductibleLimit = 0;
                                                        decimal? RemainingIndividualDeductibleLimit = 0;
                                                        if (Tier2Limit)
                                                        {
                                                            RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_2 - RT_BAL_Rx_Fam_Ded_2 : PlanLmt_Md_Fam_Ded_2 - RT_BAL_Md_Fam_Ded_2;
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_2 - RT_BAL_Rx_Ind_Ded_2 : PlanLmt_Md_Ind_Ded_2 - RT_BAL_Md_Ind_Ded_2;
                                                        }
                                                        else
                                                        {
                                                            RemainingFamilyDeductibleLimit = IsRx ? PlanLmt_Rx_Fam_Ded_1 - RT_BAL_Rx_Fam_Ded_1 : PlanLmt_Md_Fam_Ded_1 - RT_BAL_Md_Fam_Ded_1;
                                                            RemainingIndividualDeductibleLimit = IsRx ? PlanLmt_Rx_Ind_Ded_1 - RT_BAL_Rx_Ind_Ded_1 : PlanLmt_Md_Ind_Ded_1 - RT_BAL_Md_Ind_Ded_1;
                                                        }

                                                        //Check if member has any deductible limits
                                                        if (RemainingFamilyDeductibleLimit > 0)
                                                        {
                                                            if (RemainingIndividualDeductibleLimit > 0)  //tested
                                                            {
                                                                Row1["CopayPaid"] = CopayPaid2;
                                                                if (Tier2Limit)
                                                                    Tier2DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_2, PlanLmt_Md_Fam_Ded_2, PlanLmt_Rx_Ind_Ded_2, PlanLmt_Rx_Fam_Ded_2, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_Ded_2, PlanLmt_Td_Fam_Ded_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Md_Ind_Ded_2, ref RT_BAL_Md_Fam_Ded_2, ref RT_BAL_Rx_Ind_Ded_2, ref RT_BAL_Rx_Fam_Ded_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Td_Ind_Ded_2, ref RT_BAL_Td_Fam_Ded_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1DedLimit(MedicalDrugMaximumOutofPocketIntegrated, MedicalDrugDeductiblesIntegrated, NoOfMember, IsRx, objPlanAttribute.IsHSAEligible, Convert.ToDecimal(Row1["CopayPaid"]), Row1, PlanLmt_Md_Ind_Ded_1, PlanLmt_Md_Fam_Ded_1, PlanLmt_Rx_Ind_Ded_1, PlanLmt_Rx_Fam_Ded_1, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_Ded_1, PlanLmt_Td_Fam_Ded_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Md_Ind_Ded_1, ref RT_BAL_Md_Fam_Ded_1, ref RT_BAL_Rx_Ind_Ded_1, ref RT_BAL_Rx_Fam_Ded_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Td_Ind_Ded_1, ref RT_BAL_Td_Fam_Ded_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                                //compute and reduce MOOP Tier1
                                                                if (Tier2Limit)
                                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }

                                                           }
                                                            else
                                                            {
                                                                Row1["ExcludedAmount"] = 0;
                                                                // Calculate Coins
                                                                var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier2 / 100;
                                                                // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                                //Check Max Coinsurance for tier 1
                                                                if (benefitData.MHMBenefit.MaxCoinsInnTier2Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier2Amt)
                                                                {
                                                                    Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                    Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                }

                                                                if (Tier2Limit)
                                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                                else
                                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                                //Adjust copay to MOOP
                                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                                {
                                                                    Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Row1["ExcludedAmount"] = 0;
                                                            // Calculate Coins
                                                            var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                            Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier2 / 100;
                                                            // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                            //Check Max Coinsurance for tier 1
                                                            if (benefitData.MHMBenefit.MaxCoinsInnTier2Amt > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > benefitData.MHMBenefit.MaxCoinsInnTier2Amt)
                                                            {
                                                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["CoInsurancePaid"]) - benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                                Row1["CoInsurancePaid"] = benefitData.MHMBenefit.MaxCoinsInnTier2Amt;
                                                            }

                                                            if (Tier2Limit)
                                                                Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                            else
                                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                            //Adjust copay to MOOP
                                                            if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                            {
                                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                            }
                                                        }
                                                    }


                                                    #endregion
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //Difference of actual cost of service and copay will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["CoInsurancePaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);

                                                break;

                                        }

                                        break;

                                    #endregion

                                    #region CopayOnly

                                    case "CopayOnly":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                // Calculate Copay
                                                var CopayTier1 = benefitData.MHMBenefit.CopayInnTier1;

                                                // Check max qty before copay if > 0 
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay * CopayTier1);
                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                    //Row1["CoInsurancePaid"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                }
                                                else
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(EligibleQuantity * CopayTier1);
                                                }

                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                {
                                                    Row1["CopayPaid"] = PotentialPmt;
                                                }

                                                //Calculate MOOP
                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                //Adjust copay to MOOP
                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);


                                                break;
                                            case 2:

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);
                                                // Calculate Copay
                                                var CopayTier2 = benefitData.MHMBenefit.CopayInnTier2;

                                                // Check max qty before copay if > 0 
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay * CopayTier2);
                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                    //Row1["CoInsurancePaid"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                }
                                                else
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(EligibleQuantity * CopayTier2);
                                                }

                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                {
                                                    Row1["CopayPaid"] = PotentialPmt;
                                                }

                                                //Calculate MOOP
                                                if (Tier2Limit)
                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                else
                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                //Adjust copay to MOOP
                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CopayPaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);

                                                break;

                                        }
                                        break;

                                    #endregion

                                    #region CopayOnlyExcludedFromMOOP

                                    case "CopayOnlyExcludedFromMOOP":
                                        //Narsing :- we still want to reduce MOOP and extra MOOP will be tracked as excluded amount.
                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                // Calculate Copay
                                                var CopayTier1 = benefitData.MHMBenefit.CopayInnTier1;

                                                // Check max qty before copay if > 0 
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay * CopayTier1);
                                                    Row1["ExcludedAmount"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                    //Row1["CoInsurancePaid"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                }
                                                else
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(EligibleQuantity * CopayTier1);
                                                }

                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                {
                                                    Row1["CopayPaid"] = PotentialPmt;
                                                }
                                                //var CopayBenefitQuantityTier1 = benefitData.MHMBenefit.MaxQtyBeforeCoPay != 0 ? EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay : EligibleQuantity;
                                                //Row1["CopayPaid"] = CopayBenefitQuantityTier1 * CopayTier1;

                                                // Copay will be charged on full quantity to employee and we will reduce MOOP. 
                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                // We will not reset copay upto MOOP because he has to pay copay on every visit
                                                //Adjust copay to MOOP
                                                decimal finalCopaytoAdjustinMOOP1 = Convert.ToDecimal(Row1["CopayPaid"]);
                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && finalCopaytoAdjustinMOOP1 > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                {
                                                    finalCopaytoAdjustinMOOP1 = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + finalCopaytoAdjustinMOOP1;

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);

                                                // Total paid by employee will be copay
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]);
                                                //Difference of actual cost of service and Employee payment will be paid by insurance company.
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);


                                                break;
                                            case 2:

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                // Calculate Copay
                                                var CopayTier2 = benefitData.MHMBenefit.CopayInnTier1;

                                                // Check max qty before copay if > 0 
                                                if (benefitData.MHMBenefit.MaxQtyBeforeCoPay > 0 && EligibleQuantity > benefitData.MHMBenefit.MaxQtyBeforeCoPay)
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(benefitData.MHMBenefit.MaxQtyBeforeCoPay * CopayTier2);
                                                    Row1["CoInsurancePaid"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                    //Row1["CoInsurancePaid"] = (EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay) * SingleBenefitCost;
                                                }
                                                else
                                                {
                                                    Row1["CopayPaid"] = Convert.ToDecimal(EligibleQuantity * CopayTier2);
                                                }

                                                if (Convert.ToDecimal(Row1["CopayPaid"]) > PotentialPmt)
                                                {
                                                    Row1["CopayPaid"] = PotentialPmt;
                                                }
                                                //var CopayBenefitQuantityTier2 = benefitData.MHMBenefit.MaxQtyBeforeCoPay != 0 ? EligibleQuantity - benefitData.MHMBenefit.MaxQtyBeforeCoPay : EligibleQuantity;
                                                //Row1["CopayPaid"] = CopayBenefitQuantityTier2 * CopayTier2;

                                                // Copay will be charged on full quantity to employee and we will reduce MOOP. 
                                                if (Tier2Limit)
                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                else
                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CopayPaid"]), Row1, (bool)NonEmbeddedOOPLimits, (bool)IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                // We will not reset copay upto MOOP because he has to pay copay on every visit
                                                //Adjust copay to MOOP
                                                decimal finalCopaytoAdjustinMOOP2 = Convert.ToDecimal(Row1["CopayPaid"]);
                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && finalCopaytoAdjustinMOOP2 > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                {
                                                    finalCopaytoAdjustinMOOP2 = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + finalCopaytoAdjustinMOOP2;

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be copay
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CopayPaid"]);
                                                //Difference of actual cost of service and Employee payment will be paid by insurance company.
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CopayPaid"]) - Convert.ToDecimal(Row1["ExcludedAmount"]);

                                                dt.Rows.Add(Row1);

                                                break;



                                        }

                                        break;

                                    #endregion

                                    #region CoInsOnly

                                    case "CoInsOnly":

                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                // Calculate Coins
                                                var CoInsTier1 = benefitData.MHMBenefit.CoinsInnTier1;
                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier1 / 100;
                                                // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                //Check Max Coinsurance for tier 1
                                                var TotalMaxCoinsInnTier1 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier1Amt;
                                                if (TotalMaxCoinsInnTier1 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier1)
                                                {
                                                    Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier1;
                                                }

                                                Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                //Adjust copay to MOOP
                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                {
                                                    Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                dt.Rows.Add(Row1);


                                                break;
                                            case 2:

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                // Calculate Coins
                                                var CoInsTier2 = benefitData.MHMBenefit.CoinsInnTier2;
                                                Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["ActualCostofServices"]) * CoInsTier2 / 100;
                                                // Copay will be charged on full quantity and we will not check MOOP in case of IsExcludedMOOP

                                                //Check Max Coinsurance for tier 1
                                                var TotalMaxCoinsInnTier2 = EligibleQuantity * benefitData.MHMBenefit.MaxCoinsInnTier2Amt;

                                                if (TotalMaxCoinsInnTier2 > 0 && Convert.ToDecimal(Row1["CoInsurancePaid"]) > TotalMaxCoinsInnTier2)
                                                {
                                                    Row1["CoInsurancePaid"] = TotalMaxCoinsInnTier2;
                                                }
                                                if (Tier2Limit)
                                                    Tier2MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_2, PlanLmt_Md_Fam_MOOP_2, PlanLmt_Rx_Ind_MOOP_2, PlanLmt_Rx_Fam_MOOP_2, PlanLmt_Td_Ind_MOOP_2, PlanLmt_Td_Fam_MOOP_2, ref RT_BAL_Rx_Fam_MOOP_2, ref RT_BAL_Md_Ind_MOOP_2, ref RT_BAL_Md_Fam_MOOP_2, ref RT_BAL_Rx_Ind_MOOP_2, ref RT_BAL_Td_Ind_MOOP_2, ref RT_BAL_Td_Fam_MOOP_2);
                                                else
                                                    Tier1MOOP(benefitData.MHMBenefit, MedicalDrugMaximumOutofPocketIntegrated, NoOfMember, Convert.ToDecimal(Row1["CoInsurancePaid"]), Row1, (bool)NonEmbeddedOOPLimits, IsRx, objPlanAttribute.IsHSAEligible, PlanLmt_Md_Ind_MOOP_1, PlanLmt_Md_Fam_MOOP_1, PlanLmt_Rx_Ind_MOOP_1, PlanLmt_Rx_Fam_MOOP_1, PlanLmt_Td_Ind_MOOP_1, PlanLmt_Td_Fam_MOOP_1, ref RT_BAL_Rx_Fam_MOOP_1, ref RT_BAL_Md_Ind_MOOP_1, ref RT_BAL_Md_Fam_MOOP_1, ref RT_BAL_Rx_Ind_MOOP_1, ref RT_BAL_Td_Ind_MOOP_1, ref RT_BAL_Td_Fam_MOOP_1);

                                                //Adjust copay to MOOP
                                                if (benefitData.MHMBenefit.IsExclFromInnMOOP == false && Convert.ToDecimal(Row1["CoInsurancePaid"]) > Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]))
                                                {
                                                    Row1["CoInsurancePaid"] = Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                }

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + +Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = Convert.ToDecimal(Row1["CoInsurancePaid"]);
                                                //if actual cost of service exceed deductible limit & copay then remaining will be paid by insurance company 
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]) - Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                dt.Rows.Add(Row1);

                                                break;
                                        }
                                        break;

                                    #endregion

                                    #region NoCharge
                                    case "NoCharge":
                                        switch (TierIntention)
                                        {
                                            case 1:  // Tier1

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + +Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = 0;
                                                //if actual cost of service will be paid by insurance company
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]);

                                                dt.Rows.Add(Row1);

                                                break;
                                            case 2:

                                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]);

                                                Row1["TotalPaymentsuptoOOPLimit"] = Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["CopayPaid"]) + +Convert.ToDecimal(Row1["CoInsurancePaid"]);

                                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]);
                                                // Total paid by employee will be deductable limits and copay if any
                                                Row1["EmployeePayment"] = 0;
                                                //if actual cost of service will be paid by insurance company
                                                Row1["InsurancePays"] = Convert.ToDecimal(Row1["ActualCostofServices"]);

                                                dt.Rows.Add(Row1);

                                                break;
                                        }
                                        break;
                                    #endregion
                                }

                                Row1["ExcludedAmount"] = Convert.ToDecimal(Row1["ExcludedAmount"]) + LmtExcludedAmount;

                            }
                            else
                            {
                                //If benefit IsCovered == false
                                Row1["BenifitId"] = Benefit.BenefitId;
                                Row1["ActualCostofServices"] = Benefit.UsageCost;
                                Row1["ExcludedQuantity"] = 0;
                                Row1["ExcludedAmount"] = Benefit.UsageCost;
                                Row1["CopayPaid"] = 0;
                                Row1["PotentialPmt"] = 0;
                                Row1["TotalPaymentsuptoDeductible"] = 0;
                                Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]); ;
                                Row1["CoInsurancePaid"] = 0;
                                Row1["RunningTotalofPotentialPayment2"] = 0;
                                Row1["TotalPaymentsuptoOOPLimit"] = 0;
                                Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]); ;
                                Row1["EmployeePayment"] = 0;
                                Row1["InsurancePays"] = 0;
                                //add row to datatable
                                dt.Rows.Add(Row1);
                            }
                        }
                        else
                        {
                            //If benefit IsCovered == false
                            Row1["BenifitId"] = Benefit.BenefitId;
                            Row1["ActualCostofServices"] = Benefit.UsageCost;
                            Row1["ExcludedQuantity"] = 0;
                            Row1["ExcludedAmount"] = Benefit.UsageCost;
                            Row1["CopayPaid"] = 0;
                            Row1["PotentialPmt"] = 0;
                            Row1["TotalPaymentsuptoDeductible"] = 0;
                            Row1["RunningTotalofPaymentsuptoDeductible"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoDeductible"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoDeductible"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoDeductible"]); ;
                            Row1["CoInsurancePaid"] = 0;
                            Row1["RunningTotalofPotentialPayment2"] = 0;
                            Row1["TotalPaymentsuptoOOPLimit"] = 0;
                            Row1["RunningTotalofPaymentsuptoOOPLimit"] = dt.Rows.Count == 0 ? Row1["TotalPaymentsuptoOOPLimit"] : Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["RunningTotalofPaymentsuptoOOPLimit"]) + Convert.ToDecimal(Row1["TotalPaymentsuptoOOPLimit"]); ;
                            Row1["EmployeePayment"] = 0;
                            Row1["InsurancePays"] = 0;
                            //add row to datatable
                            dt.Rows.Add(Row1);
                        }
                    }

                    DataRow Total = dt.NewRow();
                    Total["ActualCostofServices"] = dt.Compute("Sum(ActualCostofServices)", "") != DBNull.Value ? dt.Compute("Sum(ActualCostofServices)", "") : 0;
                    Total["ExcludedAmount"] = dt.Compute("Sum(ExcludedAmount)", "") != DBNull.Value ? dt.Compute("Sum(ExcludedAmount)", "") : 0;
                    Total["CopayPaid"] = dt.Compute("Sum(CopayPaid)", "") != DBNull.Value ? dt.Compute("Sum(CopayPaid)", "") : 0;
                    Total["PotentialPmt"] = dt.Compute("Sum(PotentialPmt)", "") != DBNull.Value ? dt.Compute("Sum(PotentialPmt)", "") : 0;
                    Total["TotalPaymentsuptoDeductible"] = dt.Compute("Sum(TotalPaymentsuptoDeductible)", "") != DBNull.Value ? dt.Compute("Sum(TotalPaymentsuptoDeductible)", "") : 0;
                    Total["RunningTotalofPaymentsuptoDeductible"] = dt.Compute("Sum(RunningTotalofPaymentsuptoDeductible)", "") != DBNull.Value ? dt.Compute("Sum(RunningTotalofPaymentsuptoDeductible)", "") : 0;
                    Total["CoInsurancePaid"] = dt.Compute("Sum(CoInsurancePaid)", "") != DBNull.Value ? dt.Compute("Sum(CoInsurancePaid)", "") : 0;
                    Total["TotalPaymentsuptoOOPLimit"] = dt.Compute("Sum(TotalPaymentsuptoOOPLimit)", "") != DBNull.Value ? dt.Compute("Sum(TotalPaymentsuptoOOPLimit)", "") : 0;
                    Total["RunningTotalofPaymentsuptoOOPLimit"] = dt.Compute("Sum(RunningTotalofPaymentsuptoOOPLimit)", "") != DBNull.Value ? dt.Compute("Sum(RunningTotalofPaymentsuptoOOPLimit)", "") : 0;
                    Total["EmployeePayment"] = dt.Compute("Sum(EmployeePayment)", "") != DBNull.Value ? dt.Compute("Sum(EmployeePayment)", "") : 0;
                    Total["InsurancePays"] = dt.Compute("Sum(InsurancePays)", "") != DBNull.Value ? dt.Compute("Sum(InsurancePays)", "") : 0;
                    dt.Rows.Add(Total);
                    ds.Tables.Add(dt);

                    var OOPMedicalCosts = Convert.ToInt64(Total["CopayPaid"]) + Convert.ToInt64(Total["TotalPaymentsuptoDeductible"]) + Convert.ToInt64(Total["CoInsurancePaid"]) + Convert.ToInt64(Total["ExcludedAmount"]);

                }

                decimal dcActualCostofServices = 0;
                decimal dcExcludedAmount = 0;
                decimal dcCopayPaid = 0;
                decimal dcPotentialPmt = 0;
                decimal dcTotalPaymentsuptoDeductible = 0;
                decimal dcRunningTotalofPaymentsuptoDeductible = 0;
                decimal dcCoInsurancePaid = 0;
                decimal dcTotalPaymentsuptoOOPLimit = 0;
                decimal dcRunningTotalofPaymentsuptoOOPLimit = 0;
                decimal dcEmployeePayment = 0;
                decimal dcInsurancePays = 0;

                for (int j = 0; j < NoOfMember; j++)
                {
                    int FinalUsesCount = ds.Tables[j].Rows.Count - 1;
                    if (FinalUsesCount > 0)
                    {
                        dcActualCostofServices = dcActualCostofServices + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["ActualCostofServices"]);
                        dcExcludedAmount = dcExcludedAmount + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["ExcludedAmount"]);
                        dcCopayPaid = dcCopayPaid + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["CopayPaid"]);
                        dcPotentialPmt = dcPotentialPmt + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["PotentialPmt"]);
                        dcTotalPaymentsuptoDeductible = dcTotalPaymentsuptoDeductible + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["TotalPaymentsuptoDeductible"]);
                        dcRunningTotalofPaymentsuptoDeductible = dcRunningTotalofPaymentsuptoDeductible + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["RunningTotalofPaymentsuptoDeductible"]);
                        dcCoInsurancePaid = dcCoInsurancePaid + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["CoInsurancePaid"]);
                        dcTotalPaymentsuptoOOPLimit = dcTotalPaymentsuptoOOPLimit + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["TotalPaymentsuptoOOPLimit"]);
                        dcRunningTotalofPaymentsuptoOOPLimit = dcRunningTotalofPaymentsuptoOOPLimit + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["RunningTotalofPaymentsuptoOOPLimit"]);
                        dcEmployeePayment = dcEmployeePayment + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["EmployeePayment"]);
                        dcInsurancePays = dcInsurancePays + Convert.ToDecimal(ds.Tables[j].Rows[FinalUsesCount]["InsurancePays"]);
                    }
                }

                DataRow FinalTotalRow = FinalTotal.NewRow();
                FinalTotalRow["ActualCostofServices"] = dcActualCostofServices;
                FinalTotalRow["ExcludedAmount"] = dcExcludedAmount;
                FinalTotalRow["CopayPaid"] = dcCopayPaid;
                FinalTotalRow["PotentialPmt"] = dcPotentialPmt;
                FinalTotalRow["TotalPaymentsuptoDeductible"] = dcTotalPaymentsuptoDeductible;
                FinalTotalRow["RunningTotalofPaymentsuptoDeductible"] = dcRunningTotalofPaymentsuptoDeductible;
                FinalTotalRow["CoInsurancePaid"] = dcCoInsurancePaid;
                FinalTotalRow["TotalPaymentsuptoOOPLimit"] = dcTotalPaymentsuptoOOPLimit;
                FinalTotalRow["RunningTotalofPaymentsuptoOOPLimit"] = dcRunningTotalofPaymentsuptoOOPLimit;
                FinalTotalRow["EmployeePayment"] = dcEmployeePayment;
                FinalTotalRow["InsurancePays"] = dcInsurancePays;

                if (objPlanAttribute.IsHRAeligible) { FamilyHRAReimbursementTotal = new HRACalculation().CalculateHRA(UsageCode, dcTotalPaymentsuptoDeductible, JobDetails); }
                FinalTotal.Rows.Add(FinalTotalRow);
                ds.Tables.Add(FinalTotal);


                FamilySheetResult result = new FamilySheetResult()
              {
                  ExcludedAmount = Convert.ToInt64(FinalTotalRow["ExcludedAmount"]),
                  Copays = Convert.ToInt64(FinalTotalRow["CopayPaid"]),
                  PaymentsuptoDeductible = Convert.ToInt64(FinalTotalRow["TotalPaymentsuptoDeductible"]),
                  Coinsurance = Convert.ToInt64(FinalTotalRow["CoInsurancePaid"]),
                  PaymentsByInsurance = Convert.ToInt64(FinalTotalRow["InsurancePays"]),
                  TotalOOPCost = Convert.ToInt64(FinalTotalRow["EmployeePayment"]) + Convert.ToInt64(FinalTotalRow["ExcludedAmount"]),
                  FamilyHRAReimbursementTotal = FamilyHRAReimbursementTotal,
                  Limits = Limits
              };

                return result;

            }
            catch (Exception ex)
            {
                throw new System.Exception("Error occured while doing family sheet calculation. Please contact your system administrator.");
                //throw new System.Exception("Error in Family Sheet Calculation. Plan Id :" + planId + " Msg :" + msg1);
                //throw new System.Exception("Error occured while doing family sheet calculation. Please contact your system administrator. Error : " + ex.Message + " Plan Id : " + planId);
            }
        }

    }
}
