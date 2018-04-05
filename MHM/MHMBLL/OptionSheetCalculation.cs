using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal;
using MHMDal.Interfaces;
using MHMDal.Repository;
using MHMDal.Models;
using System.Data.SqlClient;
using System.Diagnostics;

namespace MHMBLL
{
    public class OptionSheetCalculation
    {
        MHMCache MHMCache = new MHMCache();
        IPlanAttributeMaster planAttributeMaster = new PlanAttributeMasterRepo();
        IPlanBenefitMaster planBenefitMaster = new PlanBenefitMasterRepo();
        IMHMBenefitMappingMaster mhmPlanBenefitMappingMaster = new MHMBenefitMappingMasterRepo();
        IMHMBenefitCostByAreaMaster mhmBenefitCostByArea = new MHMBenefitCostByAreaMasterRepo();
        IIssuerMaster issuerMaster = new IssuerMasterRepo();
        ICSRRateMaster csrRateMaster = new CSRRateMasterRepo();
        IqryZipCodeToRatingAreas qryZipCodeToRatingAreasMaster = new qryZipCodeToRatingAreasRepo();
        MHMDal.Interfaces.IRules rule = new RulesRepo();
        IJobMaster jobMaster = new JobMasterRepo();

        public decimal CheckNull(decimal? value)
        {
            if (value != null) return (decimal)value; else return 0;
        }

        public List<CasePlanResult> CalculateOptionsNew(List<FamilyMemberList> lstFamilyMembers, List<FamilyMemberUsesList> lstFamilyMemberUses, string JobNumber, string ZipCode, string CountyName, decimal Income, bool SubsidyStatus, int UsageCode, bool Welness, decimal HSAPercentage, decimal TaxRate, decimal MaxEEHSA, bool IsAmericanIndian, bool ResultStatus, decimal IndividualSubsidy, decimal ShopSubsidy, long RatingAreaId, string ProgID, decimal HSALimit, string StateCode, int ACAPlanIdSub, int PlanTypeID, long IssuerId, int TierIntention, int CurrentPlan)
        {
            List<PlanAttributeMst> lstPlanAttributeMst = new List<PlanAttributeMst>();
            //PlanAttributeMst objPlanAttribute;
            List<CasePlanResult> result = new List<CasePlanResult>();

            var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanAttributeMaster");
            if (PlanAttributeMasterFromCache != null)
            {
                lstPlanAttributeMst = ((List<PlanAttributeMst>)PlanAttributeMasterFromCache).ToList();
            }
            else
            {
                var PlanAttributeMasterList = planAttributeMaster.GetPlanAttributeMaster().Where(r => r.OpenForEnrollment == true && r.ApprovalStatus == (int)MHMBLL.EnumStatusModel.CaseApprovalStatus.InProduction).ToList();
                MHMCache.AddToMyCache("PlanAttributeMaster", PlanAttributeMasterList, MyCachePriority.Default);
                lstPlanAttributeMst = (List<PlanAttributeMst>)PlanAttributeMasterList.ToList();
            }

            //Adding Default Plans
            int DefaultPlanIdSub = ACAPlanIdSub;
            var DefaultPlansClass = rule.GetRules().Where(r => r.RuleName == "DefaultPlans" && r.RuleStatus == true).Select(t => t.ClassName).SingleOrDefault();
            if (DefaultPlansClass != null)
            {
                IRule IRule = (IRule)Activator.CreateInstance(Type.GetType("MHMBLL." + DefaultPlansClass));

                Dictionary<string, object> InObject = new Dictionary<string, object>();
                Dictionary<string, object> OutObject = new Dictionary<string, object>();

                IRule.ExecuteRule(InObject, out OutObject);
                DefaultPlanIdSub = (int)OutObject["DefaultPlanIdSub"];
            }

            //Fetch ACAPlanSubid and defauly plan to ne included by default. Example 01 and 05
            var Default_ACAtPlanList = lstPlanAttributeMst.Where(r => r.PlanId.Contains("-0" + ACAPlanIdSub) || r.PlanId.Contains("-0" + DefaultPlanIdSub)).ToList();


            //Vaibhav EmployerId
            //var GroupPlanList = lstPlanAttributeMst.Where(r => r.MrktCover == Constants.GroupPlanType && r.StateCode == StateCode && r.EmployerId == EmployerId && r.InsuranceType == InsuranceTypeId && r.BusinessYear == BusinessYear).ToList();

            var OrgionallstFamilyMembers = lstFamilyMembers;

            //Fetching Job related plans
            var jobDetail = jobMaster.GetJobMaster().Where(r => r.JobNumber == JobNumber);

            var lstJobPlansId = jobDetail.Select(r => r.JobPlansMsts.Select(t => new { t.BusinessYear, t.PlanId }).ToList()).FirstOrDefault();
            var lstPlans = lstPlanAttributeMst.Where(r => lstJobPlansId.Select(t => t.BusinessYear).Contains(r.BusinessYear) && lstJobPlansId.Select(t => t.PlanId).Contains(r.PlanId)).ToList();

            //List<PlanAttributeMst> lstPlans = jobDetail.Select(r => r.JobPlansMsts.Select(t => t.PlanAttributeMst).ToList()).FirstOrDefault();
            //var lstPlans = jobPlans.FirstOrDefault().Where(r => r.StateCode == StateCode && r.BusinessYear == BusinessYear).ToList();

            if (IssuerId > 0) { lstPlans.RemoveAll(r => r.CarrierId != IssuerId); }
            if (PlanTypeID > 0) { lstPlans.RemoveAll(r => r.PlanType != PlanTypeID); }

            //Remove catastrophic plans
            if (!(OrgionallstFamilyMembers.Count() == 1 && OrgionallstFamilyMembers.First().Age < 31) || OrgionallstFamilyMembers.Count() > 1)
                lstPlans.RemoveAll(r => r.MetalLevel == "Catastrophic" && r.MrktCover == "Indi");

            if (lstPlans.Count() == 0) { throw new Exception("No plan found matching the selected criteria."); }

            //Inject 01 and 05 plans to job plans
            lstPlans.AddRange(Default_ACAtPlanList.Where(r => lstPlans.Select(t => t.StandardComponentId).Contains(r.StandardComponentId)).ToList());

            //Getting all the master tables from Cache whhose are going to use inside the foreach loop for familysheet calculation.
            #region CacheDataFecthingforFamilySheetCalculation

            IEnumerable<PlanBenefitMst> objPlanBenefitMaster;
            var PlanBenefitMasterFromCache = MHMCache.GetMyCachedItem("PlanBenefitMaster");
            if (PlanBenefitMasterFromCache != null)
            {
                objPlanBenefitMaster = (IEnumerable<PlanBenefitMst>)PlanBenefitMasterFromCache;
            }
            else
            {
                var PlanBenefitMasterList = planBenefitMaster.GetPlanBenefitMaster();
                MHMCache.AddToMyCache("PlanBenefitMaster", PlanBenefitMasterList, MyCachePriority.Default);
                objPlanBenefitMaster = PlanBenefitMasterList;
            }

            //string IsserCode = planId.Substring(0, 5);

            IEnumerable<MHMBenefitMappingMst> objMHMBenefitMappingMaster;
            var MHMBenefitMappingMasterFromCache = MHMCache.GetMyCachedItem("MHMBenefitMappingMaster");
            if (MHMBenefitMappingMasterFromCache != null)
            {
                objMHMBenefitMappingMaster = (IEnumerable<MHMBenefitMappingMst>)MHMBenefitMappingMasterFromCache;
            }
            else
            {
                var MHMBenefitMappingMasterList = mhmPlanBenefitMappingMaster.GetMHMBenefitMappingMaster();
                MHMCache.AddToMyCache("MHMBenefitMappingMaster", MHMBenefitMappingMasterList, MyCachePriority.Default);
                objMHMBenefitMappingMaster = MHMBenefitMappingMasterList;
            }

            IEnumerable<MHMBenefitCostByAreaMst> objMHMBenefitCostByAreaMaster;
            var MHMBenefitCostByAreaMasterFromCache = MHMCache.GetMyCachedItem("MHMBenefitCostByAreaMaster");
            if (MHMBenefitCostByAreaMasterFromCache != null)
            {
                objMHMBenefitCostByAreaMaster = (IEnumerable<MHMBenefitCostByAreaMst>)MHMBenefitCostByAreaMasterFromCache;
            }
            else
            {
                var MHMBenefitCostByAreaMasterList = mhmBenefitCostByArea.GetMHMBenefitCostByAreaMaster();
                MHMCache.AddToMyCache("MHMBenefitCostByAreaMaster", MHMBenefitCostByAreaMasterList, MyCachePriority.Default);
                objMHMBenefitCostByAreaMaster = MHMBenefitCostByAreaMasterList;
            }

            //For Plan Mapping
            IEnumerable<IssuerMst> objIssuerMaster;
            var IssuerMasterFromCache = MHMCache.GetMyCachedItem("IssuerMaster");
            if (IssuerMasterFromCache != null)
            {
                objIssuerMaster = (IEnumerable<IssuerMst>)IssuerMasterFromCache;
            }
            else
            {
                var IssuerMasterList = issuerMaster.GetIssuerMaster();
                MHMCache.AddToMyCache("IssuerMaster", IssuerMasterList, MyCachePriority.Default);
                objIssuerMaster = IssuerMasterList;
            }

            IEnumerable<CSR_Rate_Mst> CSRRateList;

            var CSRRateListFromCache = MHMCache.GetMyCachedItem("CSR_Rate_Mst");
            if (CSRRateListFromCache != null)
            {
                CSRRateList = (IEnumerable<CSR_Rate_Mst>)CSRRateListFromCache;
            }
            else
            {
                CSRRateList = csrRateMaster.GetCSRRateMaster();
                MHMCache.AddToMyCache("CSR_Rate_Mst", CSRRateList, MyCachePriority.Default);
            }

            #endregion

            //var _CSRRateList = CSRRateList.Where(r => r.BusinessYear == BusinessYear && r.RatingAreaId == RatingAreaId).ToList();

            //if (!lstPlans.Any(p => p.Id == CurrentPlan))
            //{
            //    lstPlans.Add(lstPlanAttributeMst.Where(item => item.Id == CurrentPlan).FirstOrDefault());
            //}

            foreach (PlanAttributeMst objPlanAttribute in lstPlans)
            {
                Decimal BaseSum;
                if (objPlanAttribute.MrktCover == "Indi" || objPlanAttribute.MrktCover == "Shop")
                {
                    var _CSRRateList = CSRRateList.Where(r => r.PlanID == objPlanAttribute.StandardComponentId && r.BusinessYear == objPlanAttribute.BusinessYear && r.RatingAreaId == RatingAreaId).ToList();
                    decimal CompanyHSA = 0, TrueOOPCost = 0, HSAContribution = 0;
                    BaseSum = new BaseRateCal().BaseRate(lstFamilyMembers, _CSRRateList, (int)EnumStatusModel.CalculationType.Premium);

                    if (BaseSum != 0)
                    {

                        decimal? Coinsurance = objPlanAttribute.TEHBDedInnTier1Coinsurance;

                        var memberCount = OrgionallstFamilyMembers.Count();

                        FamilySheetCalculation objSheet = new FamilySheetCalculation();

                        IEnumerable<PlanBenefitMst> objPlanBenefits;
                        objPlanBenefits = objPlanBenefitMaster.Where(r => r.PlanId == objPlanAttribute.PlanId && r.BusinessYear == objPlanAttribute.BusinessYear).ToList();

                        FamilySheetResult objFamilyResult = objSheet.CalculateFamilySheetNew(objPlanAttribute, UsageCode, memberCount, lstFamilyMemberUses, objPlanBenefits, objMHMBenefitCostByAreaMaster, objPlanAttribute.NonEmbeddedOOPLimits, TierIntention, jobDetail.FirstOrDefault());

                        TrueOOPCost = objFamilyResult.TotalOOPCost;

                        //check for plan is HSA Eligible or not
                        //HSAContribution = MaxEEHSA;
                        HSAContribution = objPlanAttribute.IsHSAEligible ? MaxEEHSA : 0;
                        var HSAEETaxSavings = objPlanAttribute.IsHSAEligible ? (TaxRate * (MaxEEHSA)) / 100 : 0;
                        var GrossAnnualPremium = BaseSum * 12;
                        decimal FederalSubsidy = objPlanAttribute.MrktCover == "Indi" ? IndividualSubsidy * 12 : ShopSubsidy * 12;


                        decimal PersonalHSACon = 0;

                        //Remove Subsidy from Catastrophic Plans
                        #region Changes for remove subsidy from Catastrophic plans as changes of 06-04-2016
                        var CatastrophicSubsidyClass = rule.GetRules().Where(r => r.RuleName == "CatastrophicSubsidy" && r.RuleStatus == true).Select(t => t.ClassName).SingleOrDefault();
                        if (CatastrophicSubsidyClass != null)
                        {
                            IRule IRule = (IRule)Activator.CreateInstance(Type.GetType("MHMBLL." + CatastrophicSubsidyClass));

                            Dictionary<string, object> InObject = new Dictionary<string, object>();
                            Dictionary<string, object> OutObject = new Dictionary<string, object>();
                            InObject.Add("MetalLevel", objPlanAttribute.MetalLevel);
                            InObject.Add("SubsidyAmount", FederalSubsidy);
                            IRule.ExecuteRule(InObject, out OutObject);
                            FederalSubsidy = (decimal)OutObject["SubsidyAmount"];
                        }
                        #endregion

                        var NetPremium = GrossAnnualPremium - FederalSubsidy;
                        if (NetPremium < 0) NetPremium = 0;

                        CasePlanResult PlanResult = new CasePlanResult()
                        {
                            PlanId = objPlanAttribute.Id,
                            PersonalHSAContribution = PersonalHSACon,
                            ReferralForSpecialist = objPlanAttribute.ReferralForSpecialist,

                            GovtPlanNumber = objPlanAttribute.PlanNumber,
                            PlanIdIndiv1 = objPlanAttribute.PlanId,
                            Year = objPlanAttribute.BusinessYear,
                            GrossAnnualPremium = Convert.ToInt64(GrossAnnualPremium),
                            FederalSubsidy = Convert.ToInt64(FederalSubsidy),
                            NetAnnualPremium = Convert.ToInt64(NetPremium),
                            ExcludedAmount = objFamilyResult.ExcludedAmount,
                            Copays = objFamilyResult.Copays,
                            PaymentsToDeductibleLimit = objFamilyResult.PaymentsuptoDeductible,
                            CoinsuranceToOutOfPocketLimit = objFamilyResult.Coinsurance,
                            TaxSavingFromHSAAccount = (HSAEETaxSavings * -1),
                            Medical = Convert.ToInt64(TrueOOPCost - HSAEETaxSavings),
                            TotalPaid = Convert.ToInt64(NetPremium + (TrueOOPCost - HSAEETaxSavings)),
                            PaymentsByInsuranceCo = objFamilyResult.PaymentsByInsurance,
                            MonthlyPremium = Convert.ToInt64(NetPremium / 12),
                            //Check with Tobey :- We are just sending Tier1 range of deductible and MOOP of Medical
                            //DeductibleSingle = objPlanAttribute.MedicalDrugDeductiblesIntegrated == true ? objPlanAttribute.TEHBDedInnTier1Individual : objPlanAttribute.DEHBDedInnTier1Individual,
                            //DeductibleFamilyPerPerson = objPlanAttribute.MedicalDrugDeductiblesIntegrated == true ? objPlanAttribute.TEHBDedInnTier1FamilyPerPerson : objPlanAttribute.DEHBDedInnTier1FamilyPerPerson,
                            //DeductibleFamilyPerGroup = objPlanAttribute.MedicalDrugDeductiblesIntegrated == true ? objPlanAttribute.TEHBDedInnTier1FamilyPerGroup : objPlanAttribute.DEHBDedInnTier1FamilyPerGroup,
                            //OPLSingle = objPlanAttribute.MedicalDrugMaximumOutofPocketIntegrated == true ? objPlanAttribute.TEHBInnTier1IndividualMOOP : objPlanAttribute.DEHBInnTier1IndividualMOOP,
                            //OPLFamilyPerPerson = objPlanAttribute.MedicalDrugMaximumOutofPocketIntegrated == true ? objPlanAttribute.TEHBInnTier1FamilyPerPersonMOOP : objPlanAttribute.DEHBInnTier1FamilyPerPersonMOOP,
                            //OPLFamilyPerGroup = objPlanAttribute.MedicalDrugMaximumOutofPocketIntegrated == true ? objPlanAttribute.TEHBInnTier1FamilyPerGroupMOOP : objPlanAttribute.DEHBInnTier1FamilyPerGroupMOOP,
                            DeductibleSingle = objFamilyResult.Limits["DeductibleSingle"],
                            DeductibleFamilyPerPerson = objFamilyResult.Limits["DeductibleFamilyPerPerson"],
                            DeductibleFamilyPerGroup = objFamilyResult.Limits["DeductibleFamilyPerGroup"],
                            OPLSingle = objFamilyResult.Limits["OPLSingle"],
                            OPLFamilyPerPerson = objFamilyResult.Limits["OPLFamilyPerPerson"],
                            OPLFamilyPerGroup = objFamilyResult.Limits["OPLFamilyPerGroup"],
                            Coinsurance = Convert.ToInt64(Coinsurance),
                            WorstCase = memberCount > 1 ?
                            Convert.ToInt64(NetPremium - HSAEETaxSavings + objFamilyResult.Limits["OPLFamilyPerGroup"])
                            : Convert.ToInt64(NetPremium - HSAEETaxSavings + objFamilyResult.Limits["OPLSingle"]),
                            MedicalNetwork = objPlanAttribute.NetworkURL,
                            PlanName = !String.IsNullOrEmpty(objPlanAttribute.MetalLevel) ? objPlanAttribute.PlanMarketingName + " / " + objPlanAttribute.MetalLevel : objPlanAttribute.PlanMarketingName,
                        };
                        result.Add(PlanResult);
                        objFamilyResult = null;
                        PlanResult = null;
                    }
                }
                else
                {
                    var _CSRRateList = CSRRateList.Where(r => r.PlanID == objPlanAttribute.PlanId && r.BusinessYear == objPlanAttribute.BusinessYear && r.RatingAreaId == UsageCode && r.Age == ProgID).ToList();
                    decimal Subsidy = 0, CompanyHSA = 0, TrueOOPCost = 0, EmployeeHSAContribution = 0, HSAContribution = 0, TotalERHSA = 0;
                    BaseSum = new BaseRateCal().GroupPlanBaseRate(_CSRRateList, out Subsidy, out TotalERHSA);

                    if (BaseSum != 0)
                    {

                        decimal? Coinsurance = objPlanAttribute.TEHBDedInnTier1Coinsurance;

                        var memberCount = OrgionallstFamilyMembers.Count();

                        FamilySheetCalculation objSheet = new FamilySheetCalculation();

                        IEnumerable<PlanBenefitMst> objPlanBenefits;
                        objPlanBenefits = objPlanBenefitMaster.Where(r => r.PlanId == objPlanAttribute.PlanId && r.BusinessYear == objPlanAttribute.BusinessYear).ToList();

                        FamilySheetResult objFamilyResult = objSheet.CalculateFamilySheetNew(objPlanAttribute, UsageCode, memberCount, lstFamilyMemberUses, objPlanBenefits, objMHMBenefitCostByAreaMaster, objPlanAttribute.NonEmbeddedOOPLimits, TierIntention, jobDetail.FirstOrDefault());


                        TrueOOPCost = objFamilyResult.TotalOOPCost;

                        //HSA calculation
                        var CompanyBaseHSA = TotalERHSA;    //Employer HSA
                        var CompanyHSAMatch = new HSACalculation().CalculateCompanyHSA(UsageCode, MaxEEHSA, jobDetail.FirstOrDefault());
                        var ActualCompanyHSA = objPlanAttribute.IsHSAEligible ? CompanyBaseHSA + CompanyHSAMatch : 0;

                        //var ActualEEHSAContribution = HSALimit - ActualCompanyHSA > MaxEEHSA ? HSALimit - ActualCompanyHSA : MaxEEHSA;
                        var ActualEEHSAContribution = MaxEEHSA > HSALimit - ActualCompanyHSA ? HSALimit - ActualCompanyHSA : MaxEEHSA;

                        TotalERHSA = ActualCompanyHSA;
                        var MainCompanyHSA = ActualCompanyHSA;

                        decimal PersonalHSACon = 0;
                        if (objPlanAttribute.IsHSAEligible)
                        {
                            //MaxEEHSA ==> (HSAPercentage / 100) * ContributeLimit
                            //MainCompanyHSA  ==> Employer HSA Contribution
                            if (MaxEEHSA < (HSALimit - MainCompanyHSA))
                            {
                                PersonalHSACon = MaxEEHSA;
                            }
                            if (MaxEEHSA > (HSALimit - ActualCompanyHSA))
                            {
                                PersonalHSACon = HSALimit - ActualCompanyHSA;
                            }
                            //MaxEEHSA - MainCompanyHSA : 0;
                        }

                        var HSAEETaxSavings = objPlanAttribute.IsHSAEligible ? (TaxRate * ActualEEHSAContribution) / 100 : 0;

                        //CompanyHSA = MainCompanyHSA;

                        //var HSALimitRemaining = HSALimit - MainCompanyHSA;
                        //EmployeeHSAContribution = HSALimitRemaining > MaxEEHSA ? MaxEEHSA : HSALimitRemaining;
                        //var HSAEETaxSavings = objPlanAttribute.IsHSAEligible ? (TaxRate * EmployeeHSAContribution) / 100 : 0;
                        //HSAContribution = objPlanAttribute.IsHSAEligible ? MainCompanyHSA : 0;

                        var GrossAnnualPremium = BaseSum * 12;
                        decimal FederalSubsidy = Subsidy * 12;

                        var NetPremium = GrossAnnualPremium - FederalSubsidy;
                        if (NetPremium < 0) NetPremium = 0;

                        var HRAAmount = objFamilyResult.FamilyHRAReimbursementTotal;
                        var worstCaseHRA = objPlanAttribute.IsHRAeligible ? UsageCode > 1 ? CheckNull(jobDetail.FirstOrDefault().HRAMaxReimburseDependent) : CheckNull(jobDetail.FirstOrDefault().HRAMaxReimbursePrimary) : 0;

                        CasePlanResult PlanResult = new CasePlanResult()
                        {
                            PlanId = objPlanAttribute.Id,
                            PersonalHSAContribution = PersonalHSACon,
                            ReferralForSpecialist = objPlanAttribute.ReferralForSpecialist,

                            GovtPlanNumber = objPlanAttribute.PlanNumber,
                            PlanIdIndiv1 = objPlanAttribute.PlanId,
                            Year = objPlanAttribute.BusinessYear,
                            GrossAnnualPremium = Convert.ToInt64(GrossAnnualPremium),
                            FederalSubsidy = Convert.ToInt64(FederalSubsidy),
                            NetAnnualPremium = Convert.ToInt64(NetPremium),
                            ExcludedAmount = objFamilyResult.ExcludedAmount,
                            Copays = objFamilyResult.Copays,
                            PaymentsToDeductibleLimit = objFamilyResult.PaymentsuptoDeductible,
                            CoinsuranceToOutOfPocketLimit = objFamilyResult.Coinsurance,
                            ContributedToYourHSAAccount = Convert.ToInt64(MainCompanyHSA),
                            EmployerHRAReimbursement = Convert.ToInt64(HRAAmount),
                            TaxSavingFromHSAAccount = (HSAEETaxSavings * -1),
                            Medical = Convert.ToInt64(TrueOOPCost - MainCompanyHSA - HSAEETaxSavings - HRAAmount),
                            TotalPaid = Convert.ToInt64(NetPremium + TrueOOPCost - MainCompanyHSA - HSAEETaxSavings - HRAAmount),
                            PaymentsByInsuranceCo = objFamilyResult.PaymentsByInsurance,
                            MonthlyPremium = Convert.ToInt64(NetPremium / 12),
                            DeductibleSingle = objFamilyResult.Limits["DeductibleSingle"],
                            DeductibleFamilyPerPerson = objFamilyResult.Limits["DeductibleFamilyPerPerson"],
                            DeductibleFamilyPerGroup = objFamilyResult.Limits["DeductibleFamilyPerGroup"],
                            OPLSingle = objFamilyResult.Limits["OPLSingle"],
                            OPLFamilyPerPerson = objFamilyResult.Limits["OPLFamilyPerPerson"],
                            OPLFamilyPerGroup = objFamilyResult.Limits["OPLFamilyPerGroup"],
                            Coinsurance = Convert.ToInt64(Coinsurance),
                            WorstCase = memberCount > 1 ?
                        Convert.ToInt64(NetPremium + objFamilyResult.Limits["OPLFamilyPerGroup"] - TotalERHSA - HSAEETaxSavings + objFamilyResult.ExcludedAmount) :
                        Convert.ToInt64(NetPremium + objFamilyResult.Limits["OPLSingle"] - TotalERHSA - HSAEETaxSavings + objFamilyResult.ExcludedAmount),
                            //Convert.ToInt64(NetPremium + objFamilyResult.Limits["OPLFamilyPerGroup"] - TotalERHSA - worstCaseHRA - HSAEETaxSavings + objFamilyResult.ExcludedAmount) :
                            //Convert.ToInt64(NetPremium + objFamilyResult.Limits["OPLSingle"] - TotalERHSA - worstCaseHRA - HSAEETaxSavings + objFamilyResult.ExcludedAmount),
                            MedicalNetwork = objPlanAttribute.NetworkURL,
                            PlanName = !String.IsNullOrEmpty(objPlanAttribute.MetalLevel) ? objPlanAttribute.PlanMarketingName + " / " + objPlanAttribute.MetalLevel : objPlanAttribute.PlanMarketingName,
                            HRAReimbursedAmt = Convert.ToInt64(HRAAmount),
                            TotalEmployerContribution_Pre = Convert.ToInt64(TotalERHSA + FederalSubsidy + HRAAmount),
                            TotalEmployerContribution_Post = Convert.ToInt64((1 + TaxRate / 100) * ((1 * TotalERHSA) + (1 * FederalSubsidy)))
                        };
                        result.Add(PlanResult);
                        objFamilyResult = null;
                        PlanResult = null;
                    }
                }
            }

            List<CasePlanResult> PlanResults = result.AsEnumerable().OrderBy(r => Convert.ToDecimal(r.TotalPaid)).ToList();

            if (ResultStatus)
            {
                return PlanResults.Select((entry, index) => new CasePlanResult()
                {
                    PlanId = entry.PlanId,
                    PersonalHSAContribution = entry.PersonalHSAContribution,
                    ReferralForSpecialist = entry.ReferralForSpecialist,

                    GovtPlanNumber = entry.GovtPlanNumber,
                    PlanIdIndiv1 = entry.PlanIdIndiv1,
                    Year = entry.Year,
                    GrossAnnualPremium = entry.GrossAnnualPremium,
                    FederalSubsidy = entry.FederalSubsidy,
                    NetAnnualPremium = entry.NetAnnualPremium,
                    ExcludedAmount = entry.ExcludedAmount,
                    Copays = entry.Copays,
                    PaymentsToDeductibleLimit = entry.PaymentsToDeductibleLimit,
                    CoinsuranceToOutOfPocketLimit = entry.CoinsuranceToOutOfPocketLimit,
                    ContributedToYourHSAAccount = entry.ContributedToYourHSAAccount,
                    TaxSavingFromHSAAccount = entry.TaxSavingFromHSAAccount,
                    Medical = entry.Medical,
                    TotalPaid = entry.TotalPaid,
                    PaymentsByInsuranceCo = entry.PaymentsByInsuranceCo,
                    MonthlyPremium = entry.MonthlyPremium,
                    DeductibleSingle = entry.DeductibleSingle,
                    DeductibleFamilyPerPerson = entry.DeductibleFamilyPerPerson,
                    DeductibleFamilyPerGroup = entry.DeductibleFamilyPerGroup,
                    OPLSingle = entry.OPLSingle,
                    OPLFamilyPerPerson = entry.OPLFamilyPerPerson,
                    OPLFamilyPerGroup = entry.OPLFamilyPerGroup,
                    Coinsurance = entry.Coinsurance,
                    WorstCase = entry.WorstCase,
                    MedicalNetwork = entry.MedicalNetwork,
                    PlanName = entry.PlanName,
                    HRAReimbursedAmt = entry.HRAReimbursedAmt,
                    EmployerHRAReimbursement = entry.EmployerHRAReimbursement,
                    TotalEmployerContribution_Pre = entry.TotalEmployerContribution_Pre,
                    TotalEmployerContribution_Post = entry.TotalEmployerContribution_Post,
                    Rank = index + 1
                }).ToList();
            }
            return PlanResults.Select((entry, index) => new CasePlanResult()
            {
                PlanId = entry.PlanId,
                PersonalHSAContribution = entry.PersonalHSAContribution,
                ReferralForSpecialist = entry.ReferralForSpecialist,

                GovtPlanNumber = entry.GovtPlanNumber,
                PlanIdIndiv1 = entry.PlanIdIndiv1,
                Year = entry.Year,
                GrossAnnualPremium = entry.GrossAnnualPremium,
                FederalSubsidy = entry.FederalSubsidy,
                NetAnnualPremium = entry.NetAnnualPremium,
                ExcludedAmount = entry.ExcludedAmount,
                Copays = entry.Copays,
                PaymentsToDeductibleLimit = entry.PaymentsToDeductibleLimit,
                CoinsuranceToOutOfPocketLimit = entry.CoinsuranceToOutOfPocketLimit,
                ContributedToYourHSAAccount = entry.ContributedToYourHSAAccount,
                TaxSavingFromHSAAccount = entry.TaxSavingFromHSAAccount,
                Medical = entry.Medical,
                TotalPaid = entry.TotalPaid,
                PaymentsByInsuranceCo = entry.PaymentsByInsuranceCo,
                MonthlyPremium = entry.MonthlyPremium,
                DeductibleSingle = entry.DeductibleSingle,
                DeductibleFamilyPerPerson = entry.DeductibleFamilyPerPerson,
                DeductibleFamilyPerGroup = entry.DeductibleFamilyPerGroup,
                OPLSingle = entry.OPLSingle,
                OPLFamilyPerPerson = entry.OPLFamilyPerPerson,
                OPLFamilyPerGroup = entry.OPLFamilyPerGroup,
                Coinsurance = entry.Coinsurance,
                WorstCase = entry.WorstCase,
                MedicalNetwork = entry.MedicalNetwork,
                PlanName = entry.PlanName,
                HRAReimbursedAmt = entry.HRAReimbursedAmt,
                EmployerHRAReimbursement = entry.EmployerHRAReimbursement,
                TotalEmployerContribution_Pre = entry.TotalEmployerContribution_Pre,
                TotalEmployerContribution_Post = entry.TotalEmployerContribution_Post,
                Rank = index + 1
            }).Take(5).ToList();

        }

    }
}
