using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal;
using MHMDal.Interfaces;
using MHMDal.Repository;
using MHMDal.Models;

namespace MHMBLL
{
    public class SubsidyCal
    {
        MHMCache MHMCache = new MHMCache();
        //ICountyMaster countyMaster = new CountyMasterRepo();
        ICSRRateMaster csrRateMaster = new CSRRateMasterRepo();
        IFedPovertyLevelMaster fedPovertyLevelMaster = new FedPovertyLevelMasterRepo();
        IFPLBracketLookupMaster fplBracketLookupMster = new FPLBracketLookupMasterRepo();
        IFPLCapMaster fplCapMaster = new FPLCapMasterRepo();
        IRatingAreaMaster ratingAreaMaster = new RatingAreaMasterRepo();
        //IZipCodeMaster zipCodeMaster = new ZipCodeMasterRepo();

        IMedicaidEligibility medicaidEligibility = new MedicaidEligibilityRepo();
        IChipEligibility chipEligibility = new ChipEligibilityRepo();
        IPlanAttributeMaster planAttributeMaster = new PlanAttributeMasterRepo();
        MHMDal.Interfaces.IRules rule = new RulesRepo();

        /// <summary>
        /// This method is used to calculate subsidy amount
        /// </summary>
        /// <param name="zipCode">Zipcode</param>
        /// <param name="Income">Income</param>
        /// <param name="IsAmericanIndian">Is American Indian</param>
        /// <param name="lstFamilyMembers">List of Family Members</param>
        /// <param name="Std_Premium">Out Base Premium Amount</param>
        /// <param name="ACAPlanId">Out Aca Plan Id</param>
        /// <param name="RatingArea">Out Rating Area Id</param>
        /// <param name="MemberRemoveMedicaidEligibilityBase">Out Count of Members removed under Medicaid eligibiligy</param>
        /// <param name="MemberRemoveChipEligibilityBase">Out Count of Members removed under Chip eligibiligy</param>
        /// <returns></returns>
        public decimal CalculateSubsidy(string BusinessYear, decimal Income, bool IsAmericanIndian, long RatingAreaId, string StateCode, List<FamilyMemberList> lstFamilyMembers, bool SubsidyStatus, string MrktCover, out int ACAPlanId, out int MemberRemoveMedicaidEligibilityBase, out int MemberRemoveChipEligibilityBase, out decimal FPLout, out string SecondLowestPlanId)
        {
            String SecondLowestBaserate;
            int FamilyMemberCount, AmericanIndianValue, ACAPlanIdSub;
            decimal FPL, MagiasPctFPL, CapLow, CapHigh, BaseFPLBottom, BaseFPLTop, ProratedCapperFPLoverBottom, AddtionalIncomeCapRate, IncomeCap, IncomeCapAnnual;
            decimal IncomeCapMonthly, SilverPlanReferencePremium, SubsidyAvailableMonthly;

            #region Changes 31-03-2016

            List<CSR_Rate_Mst> CSRRateList;

            var CSRRateListFromCache = MHMCache.GetMyCachedItem("CSR_Rate_Mst");
            if (CSRRateListFromCache != null)
            {
                CSRRateList = (List<CSR_Rate_Mst>)CSRRateListFromCache;
            }
            else
            {
                CSRRateList = csrRateMaster.GetCSRRateMaster().ToList();
                MHMCache.AddToMyCache("CSR_Rate_Mst", CSRRateList, MyCachePriority.Default);
            }
            CSRRateList.ForEach(r => r.EHBRate = r.IndividualRate * r.EHBPercentTotalPremium);
            #endregion

            List<PlanAttributeMst> lstPlanAttributeMst = new List<PlanAttributeMst>();
            List<CasePlanResult> result = new List<CasePlanResult>();

            var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanAttributeMaster");
            if (PlanAttributeMasterFromCache != null)
            {
                lstPlanAttributeMst = ((List<PlanAttributeMst>)PlanAttributeMasterFromCache).Where(r => r.OpenForEnrollment == true && r.ApprovalStatus == (int)MHMBLL.EnumStatusModel.CaseApprovalStatus.InProduction && r.StateCode == StateCode).ToList();
            }
            else
            {
                var PlanAttributeMasterList = planAttributeMaster.GetPlanAttributeMaster().Where(r => r.OpenForEnrollment == true).ToList();
                MHMCache.AddToMyCache("PlanAttributeMaster", PlanAttributeMasterList, MyCachePriority.Default);
                lstPlanAttributeMst = (List<PlanAttributeMst>)PlanAttributeMasterList.Where(r => r.StateCode == StateCode).ToList();
            }

            var planAttr = lstPlanAttributeMst.Where(r => r.MrktCover == MrktCover && r.StateCode == StateCode && r.MetalLevel == "Silver" && r.BusinessYear == BusinessYear).Select(r => r.StandardComponentId).Distinct().ToList();
            var csrBenifits = CSRRateList.Where(r => r.BusinessYear == BusinessYear && planAttr.Contains(r.PlanID)).ToList();

            string SecondLowestBasePlan = GetSecondLowestPlan(RatingAreaId, lstFamilyMembers.First().Age, csrBenifits);   //&& !r.PlanID.StartsWith("33653ME")
            if (SecondLowestBasePlan != "") { SecondLowestBaserate = SecondLowestBasePlan; } else { throw new System.Exception("Second Lowest Base Rate not found."); }
            SecondLowestPlanId = SecondLowestBasePlan;
            FamilyMemberCount = lstFamilyMembers.Count();

            var FedPovertyLevelMasterFromCache = MHMCache.GetMyCachedItem("FedPovertyLevelMaster");
            try
            {
                if (FedPovertyLevelMasterFromCache != null)
                {
                    FPL = (decimal)((IEnumerable<FedPovertyLevelMst>)FedPovertyLevelMasterFromCache).Where(r => r.HouseholdSize == FamilyMemberCount && r.BusinessYear == BusinessYear).FirstOrDefault().FPL;
                }
                else
                {
                    var FedPovertyLevelList = fedPovertyLevelMaster.GetFedPovertyLevelMaster();
                    MHMCache.AddToMyCache("FedPovertyLevelMaster", FedPovertyLevelList, MyCachePriority.Default);
                    FPL = (decimal)FedPovertyLevelList.Where(r => r.HouseholdSize == FamilyMemberCount && r.BusinessYear == BusinessYear).FirstOrDefault().FPL;
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Unable to calculate FPL.");
            }

            MagiasPctFPL = (Income / FPL) * 100;    //FPL%

            int MemberRemoveMedicaidEligibility = 0, MemberRemoveChipEligibility = 0;

            #region Subsidy Calculation Changes

            //Determine AEligibility of Adults base on State Table

            //Determine if Adult is Childless
            var ChildStatus = lstFamilyMembers.Any(r => r.Age <= 18);

            decimal StateFPL = 0;

            //Determine MedicaidRuleClass Eligibility
            var MedicaidRuleClass = rule.GetRules().Where(r => r.RuleName == "Medicaid" && r.RuleStatus == true).Select(t => t.ClassName).SingleOrDefault();
            if (MedicaidRuleClass != null)
            {
                IRule IRule = (IRule)Activator.CreateInstance(Type.GetType("MHMBLL." + MedicaidRuleClass));

                Dictionary<string, object> InObject = new Dictionary<string, object>();
                InObject.Add("lstFamilyMembers", lstFamilyMembers);
                InObject.Add("StateCode", StateCode);
                InObject.Add("MagiasPctFPL", MagiasPctFPL);
                InObject.Add("IsChild", ChildStatus);
                InObject.Add("BusinessYear", BusinessYear);
                Dictionary<string, object> OutObject = new Dictionary<string, object>();

                //Get MedicaidPercentage based on ChildStatus and State
                IRule.ExecuteRule(InObject, out OutObject);
                MemberRemoveMedicaidEligibility = (int)OutObject["MemberCount"];
            }

            //Determine Chip Eligibility of Children base on Child Age Bracket
            if (ChildStatus)
            {
                var ChipRuleClass = rule.GetRules().Where(r => r.RuleName == "Chip" && r.RuleStatus == true).Select(t => t.ClassName).SingleOrDefault();
                if (ChipRuleClass != null)
                {
                    IRule IRule = (IRule)Activator.CreateInstance(Type.GetType("MHMBLL." + ChipRuleClass));

                    Dictionary<string, object> InObject = new Dictionary<string, object>();
                    InObject.Add("lstFamilyMembers", lstFamilyMembers);
                    InObject.Add("StateCode", StateCode);
                    InObject.Add("MagiasPctFPL", MagiasPctFPL);
                    InObject.Add("BusinessYear", BusinessYear);
                    Dictionary<string, object> OutObject = new Dictionary<string, object>();

                    IRule.ExecuteRule(InObject, out OutObject);
                    MemberRemoveChipEligibility = (int)OutObject["MemberCount"];
                }
            }

            MemberRemoveMedicaidEligibilityBase = MemberRemoveMedicaidEligibility;
            MemberRemoveChipEligibilityBase = MemberRemoveChipEligibility;

            #endregion

            var _CSRRateList = CSRRateList.Where(r => r.BusinessYear == BusinessYear && r.RatingAreaId == RatingAreaId && r.PlanID == SecondLowestBaserate).ToList();

            //SilverPlanReferencePremium = new BaseRateCal().BaseRate(SecondLowestBaserate, lstFamilyMembers, _CSRRateList, (int)EnumStatusModel.CalculationType.Subsidy);
            SilverPlanReferencePremium = new BaseRateCal().BaseRate(lstFamilyMembers, _CSRRateList, (int)EnumStatusModel.CalculationType.Subsidy);

            if (MagiasPctFPL > 100)
            {
                var FplCapMasterFromCache = MHMCache.GetMyCachedItem("FPLCapMaster");
                try
                {
                    if (FplCapMasterFromCache != null)
                    {
                        var data = ((IEnumerable<FPLCapMst>)FplCapMasterFromCache).Where(r => r.BaseFPLTop > MagiasPctFPL && r.BaseFPLBottom < MagiasPctFPL && r.BusinessYear == BusinessYear).FirstOrDefault();
                        CapLow = (decimal)data.CapLow;
                        CapHigh = (decimal)data.CapHigh;
                        BaseFPLBottom = (decimal)data.BaseFPLBottom;
                        BaseFPLTop = (decimal)data.BaseFPLTop;
                    }
                    else
                    {
                        var FplCapList = fplCapMaster.GetFPLCapMaster();
                        MHMCache.AddToMyCache("FPLCapMaster", FplCapList, MyCachePriority.Default);
                        var data = FplCapList.Where(r => r.BaseFPLTop > MagiasPctFPL && r.BaseFPLBottom < MagiasPctFPL && r.BusinessYear == BusinessYear).FirstOrDefault();
                        CapLow = (decimal)data.CapLow;
                        CapHigh = (decimal)data.CapHigh;
                        BaseFPLBottom = (decimal)data.BaseFPLBottom;
                        BaseFPLTop = (decimal)data.BaseFPLTop;
                    }
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Unable to calculate FPL cap.");
                }

                ProratedCapperFPLoverBottom = (CapHigh - CapLow) / (BaseFPLTop - BaseFPLBottom);
                AddtionalIncomeCapRate = (MagiasPctFPL - BaseFPLBottom) * ProratedCapperFPLoverBottom;
                IncomeCap = AddtionalIncomeCapRate + CapLow;
                IncomeCapAnnual = (Income * IncomeCap) / 100;
                IncomeCapMonthly = IncomeCapAnnual / 12;

                //SilverPlanReferencePremium = new BaseRateCal().BaseRate(RatingAreaId, SecondLowestBaserate, lstFamilyMembers);

                if (SubsidyStatus)
                {
                    if ((MagiasPctFPL + Convert.ToDecimal(Constants.MagiasPctFPLPercentage * 100)) >= StateFPL)
                    {
                        if ((SilverPlanReferencePremium - IncomeCapMonthly) > 0) { SubsidyAvailableMonthly = (SilverPlanReferencePremium - IncomeCapMonthly); } else { SubsidyAvailableMonthly = 0; };
                    }
                    else
                    {
                        SubsidyAvailableMonthly = 0;
                    }
                }
                else
                {
                    SubsidyAvailableMonthly = 0;
                }
            }
            else
            {
                SubsidyAvailableMonthly = 0;
            }

            if (IsAmericanIndian)
            {
                if (MagiasPctFPL / 100 < Convert.ToDecimal(Constants.IsAmericanIndianPercentage)) { AmericanIndianValue = 2; } else { AmericanIndianValue = 3; }
                ACAPlanIdSub = AmericanIndianValue;
            }
            else
            {
                var FPLBracketLookupMasterFromCache = MHMCache.GetMyCachedItem("FPLBracketLookupMaster");
                try
                {
                    if (FPLBracketLookupMasterFromCache != null)
                    {
                        ACAPlanIdSub = (int)((IEnumerable<FPLBracketLookupMst>)FPLBracketLookupMasterFromCache).Where(r => r.FPLBracketLookup < MagiasPctFPL && r.BusinessYear == BusinessYear).OrderByDescending(t => t.FPLBracketLookup).FirstOrDefault().SubPlanId;
                    }
                    else
                    {
                        var FPLBracketLookupList = fplBracketLookupMster.GetFPLBracketLookupMaster();
                        MHMCache.AddToMyCache("FPLBracketLookupMaster", FPLBracketLookupList, MyCachePriority.Default);
                        ACAPlanIdSub = (int)FPLBracketLookupList.Where(r => r.FPLBracketLookup < MagiasPctFPL && r.BusinessYear == BusinessYear).OrderByDescending(t => t.FPLBracketLookup).FirstOrDefault().SubPlanId;
                    }
                }
                catch (Exception ex)
                {
                    throw new System.Exception("ACAPlanIdSub not found");
                }
            }
            //Std_Premium = SilverPlanReferencePremium;
            ACAPlanId = ACAPlanIdSub;
            FPLout = decimal.Round(MagiasPctFPL, 2);
            return SubsidyAvailableMonthly;
        }


        private string GetSecondLowestPlan(long RatingAreaId, int Age, IEnumerable<CSR_Rate_Mst> CSRRateList)
        {
            List<CSR_Rate_Mst> lstTemp;
            try
            {
                if (CSRRateList.Count() > 0)
                {
                    if (Age <= Constants.LowAgeLimit)
                        lstTemp = CSRRateList.Where(r => r.Age == "0-20" && r.RatingAreaId == RatingAreaId).OrderBy(r => r.EHBRate).ToList();
                    else if (Age > Constants.AboveAgeLimit)
                        lstTemp = CSRRateList.Where(r => r.Age == "65 and over" && r.RatingAreaId == RatingAreaId).OrderBy(r => r.EHBRate).ToList();
                    else
                        lstTemp = CSRRateList.Where(r => r.Age == Age.ToString() && r.RatingAreaId == RatingAreaId).OrderBy(r => r.EHBRate).ToList();

                    if (lstTemp.Count() > 1)
                        return lstTemp.Take(2).Skip(1).Select(T => T.PlanID).First();
                    else if (lstTemp.Count() == 1)
                        return lstTemp.First().PlanID;
                    else
                        return "";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Second Lowest Plan not found");
            }
        }

        public static void CreateInstance(string ClassType, ref object objN)
        {
            objN = Activator.CreateInstance(Type.GetType(ClassType));
        }

    }
}
