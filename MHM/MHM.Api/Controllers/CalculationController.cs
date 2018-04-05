using MHMBLL;
using MHMDal.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SynSecurity;
using System.Diagnostics;

namespace MHM.Api.Controllers
{
    public class CalculationController : ApiController
    {
        #region OldCode


        ///// <summary>
        ///// This api is used to calculate Top Plans
        ///// </summary>
        ///// <param name="ZipCode">Zipcode</param>
        ///// <param name="CountyId">County Id</param>
        ///// <param name="Income">Income</param>
        ///// <param name="IsAmericanIndian">Is American Indian</param>
        ///// <param name="SubsidyStatus">Subsidy(True/False)</param>
        ///// <param name="UsageCode">Usage Code</param>
        ///// <param name="IssuerId">Issuer Id</param>
        ///// <param name="PlanID">Plan Id</param>
        ///// <param name="EmployerId">Employer id</param>
        ///// <param name="InsuranceTypeId">Insurance Type Id</param>
        ///// <param name="BusinessYear">Business Year</param>
        ///// <param name="Welness">Wellness</param>
        ///// <param name="jObject">Json Object Contain Member and their Benefit Uses Details</param>
        ///// <param name="HSAPercentage">HSA Percentage</param>
        ///// <param name="TaxRate">Tax Rate</param>
        ///// <returns>Top 6 plans</returns>
        //[HttpPost]
        //[Route("api/operation/CalculatePlans")]
        ////[Authorize]
        //public HttpResponseMessage CalculatePlans(string ZipCode, string CountyName, decimal Income, bool SubsidyStatus, int UsageCode, long IssuerId, int PlanID, long EmployerId, string JobNumber, int InsuranceTypeId, string BusinessYear, bool Welness, JObject jObject, decimal HSAPercentage = 0, decimal TaxRate = 0, bool IsAmericanIndian = false, bool ResultStatus = false)
        //{
        //    Response oResponse = new Response();

        //    try
        //    {
        //        string UserDatail = jObject["data"].ToString();
        //        string UsesDetail = jObject["UsesDetail"].ToString();
        //        List<FamilyMemberUsesList> lstFamilyMemberUses = (List<FamilyMemberUsesList>)JsonConvert.DeserializeObject(UsesDetail, (typeof(List<FamilyMemberUsesList>)));

        //        List<FamilyMemberList> fmlMemberList = (List<FamilyMemberList>)JsonConvert.DeserializeObject(UserDatail, (typeof(List<FamilyMemberList>)));  // new List<FamilyMemberList>(); 

        //        string SecondLowestPlanId = "", StateCode = "";
        //        decimal HSALimit = 0, MaxEEHSA = 0;
        //        decimal Std_Premium = 0, Subsidy = 0, FPL = 0;
        //        int ACAPlanIdSub = 0, MemberRemoveChipEligibility = 0, MemberRemoveMedicaidEligibility = 0;
        //        long RatingAreaId = 0;
        //        var OrgionalFamilyMember = fmlMemberList.ToList();
        //        string ProgID;

        //        List<CasePlanResult> data;

        //        if (EmployerId == Constants.DefaultEmployerID || EmployerId == Constants.ShopEmployerId)
        //        {
        //            string MrktCover = "Indi";
        //            //Here we are calculating for Indivisual Plans and Shop Plans
        //            MaxEEHSA = new HSACalculation().CalculateAnnualHSA(UsageCode, fmlMemberList.Max(r => r.Age), (decimal)HSAPercentage, out HSALimit);
        //            if (EmployerId == Constants.ShopEmployerId) { SubsidyStatus = false; MrktCover = "SHOP"; }
        //            Subsidy = new SubsidyCal().CalculateSubsidy(ZipCode, CountyName, BusinessYear, Income, IsAmericanIndian, fmlMemberList, SubsidyStatus, MrktCover, out Std_Premium, out ACAPlanIdSub, out RatingAreaId, out MemberRemoveMedicaidEligibility, out MemberRemoveChipEligibility, out FPL, out StateCode, out SecondLowestPlanId);
        //            data = new OptionSheetCalculation().CalculateOptions(RatingAreaId, StateCode, (decimal)TaxRate, MaxEEHSA, fmlMemberList, OrgionalFamilyMember, lstFamilyMemberUses, Subsidy, ACAPlanIdSub, IssuerId, PlanID, BusinessYear, EmployerId, ResultStatus);
        //        }
        //        else
        //        {
        //            //Here we are calculating for group plans
        //            if (Welness) ProgID = "B"; else ProgID = "A";
        //            MaxEEHSA = new HSACalculation().CalculateAnnualHSA(UsageCode, fmlMemberList.Max(r => r.Age), (decimal)HSAPercentage, out HSALimit);
        //            data = new OptionSheetCalculation().CalculateGroupPlanOptions(ZipCode, CountyName, UsageCode, ProgID, EmployerId, JobNumber, InsuranceTypeId, (decimal)TaxRate, (decimal)MaxEEHSA, BusinessYear, OrgionalFamilyMember, lstFamilyMemberUses, IssuerId, HSALimit, ResultStatus);
        //        }

        //        if (data.Count() > 0)
        //        {
        //            Dictionary<string, object> res = new Dictionary<string, object>();
        //            res.Add("Status", "true");
        //            res.Add("Message", "Success");
        //            res.Add("Plans", data);
        //            res.Add("ChipEligibilityCount", MemberRemoveChipEligibility);
        //            res.Add("MedicaidEligibilityCount", MemberRemoveMedicaidEligibility);
        //            res.Add("SubsidyAmount", decimal.Round(Subsidy));
        //            res.Add("HSALimit", HSALimit);
        //            res.Add("HSAAmount", MaxEEHSA);
        //            res.Add("FPL", FPL);
        //            res.Add("SecondLowestPlanId", SecondLowestPlanId);
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
        //            return response;
        //        }
        //        else
        //        {
        //            oResponse.Status = false;
        //            oResponse.Message = "No plan found matching the selected criteria.";
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oResponse.Status = false;
        //        oResponse.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
        //        return response;
        //    }
        //}

        #endregion

        [HttpPost]
        [Route("api/operation/CalculatePlans")]
        //[Authorize]
        public HttpResponseMessage CalculatePlans(string ZipCode, string CountyName, decimal Income, bool SubsidyStatus, int UsageCode, long IssuerId, int PlanTypeID, long EmployerId, string JobNumber, string BusinessYear, bool Welness, int TierIntention, JObject jObject, DateTime? DedBalAvailDate, decimal HSAPercentage = 0, decimal TaxRate = 0, bool IsAmericanIndian = false, bool ResultStatus = false, decimal DedBalAvailToRollOver = 0)
        {
            Response oResponse = new Response();

            try
            {
                string UserDatail = jObject["data"].ToString();
                string UsesDetail = jObject["UsesDetail"].ToString();
                List<FamilyMemberUsesList> lstFamilyMemberUses = (List<FamilyMemberUsesList>)JsonConvert.DeserializeObject(UsesDetail, (typeof(List<FamilyMemberUsesList>)));

                List<FamilyMemberList> fmlMemberList = (List<FamilyMemberList>)JsonConvert.DeserializeObject(UserDatail, (typeof(List<FamilyMemberList>)));  // new List<FamilyMemberList>(); 

                var objOptionSheet = new OptionSheetCalculation();


                string SecondLowestPlanId = "", StateCode = "";
                decimal HSALimit = 0, MaxEEHSA = 0;
                decimal IndividualSubsidy = 0, ShopSubsidy = 0, FPL = 0;
                int ACAPlanIdSub = 0, MemberRemoveChipEligibility = 0, MemberRemoveMedicaidEligibility = 0;
                long RatingAreaId = 0;
                var OrgionalFamilyMember = fmlMemberList.ToList();
                string ProgID = Welness ? "B" : "A";

                //List<CasePlanResult> data;
                new StateandRatingArea().GetStateCodeandRatingArea(ZipCode, CountyName, out RatingAreaId, out StateCode);
                MaxEEHSA = new HSACalculation().CalculateAnnualHSA(UsageCode, fmlMemberList.First().Age, (decimal)HSAPercentage, BusinessYear, out HSALimit);

                if (EmployerId == 99999)
                {
                    IndividualSubsidy = new SubsidyCal().CalculateSubsidy(BusinessYear, Income, IsAmericanIndian, RatingAreaId, StateCode, fmlMemberList, SubsidyStatus, "Indi", out ACAPlanIdSub, out MemberRemoveMedicaidEligibility, out MemberRemoveChipEligibility, out FPL, out SecondLowestPlanId);
                }
                if (EmployerId == 100000)
                {
                    ShopSubsidy = new SubsidyCal().CalculateSubsidy(BusinessYear, Income, IsAmericanIndian, RatingAreaId, StateCode, fmlMemberList, SubsidyStatus, "SHOP", out ACAPlanIdSub, out MemberRemoveMedicaidEligibility, out MemberRemoveChipEligibility, out FPL, out SecondLowestPlanId);
                }

                var data = objOptionSheet.CalculateOptionsNew(fmlMemberList, lstFamilyMemberUses, JobNumber, ZipCode, CountyName, Income, SubsidyStatus, UsageCode, Welness, HSAPercentage, TaxRate, (decimal)MaxEEHSA, IsAmericanIndian, ResultStatus, IndividualSubsidy, ShopSubsidy, RatingAreaId, ProgID, HSALimit, StateCode, ACAPlanIdSub, PlanTypeID, IssuerId, TierIntention, 0);
                
                if (data.Count() > 0)
                {
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("Plans", data);
                    res.Add("ChipEligibilityCount", MemberRemoveChipEligibility);
                    res.Add("MedicaidEligibilityCount", MemberRemoveMedicaidEligibility);
                    //res.Add("SubsidyAmount", decimal.Round(Subsidy));
                    res.Add("HSALimit", HSALimit);
                    res.Add("HSAAmount", MaxEEHSA);
                    res.Add("FPL", FPL);
                    res.Add("SecondLowestPlanId", SecondLowestPlanId);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    oResponse.Status = false;
                    oResponse.Message = "No plan found matching the selected criteria.";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                string ExceptionString = "Api : CalculatePlans" + Environment.NewLine;
                ExceptionString += "Request :  " + " ZipCode " + ZipCode + " ,CountyName " + CountyName + " ,Income " + Income + " ,SubsidyStatus " + SubsidyStatus + " ,UsageCode " + UsageCode + " ,IssuerId " + IssuerId + " ,PlanTypeID " + PlanTypeID + " ,EmployerId " + EmployerId + " ,JobNumber " + JobNumber + " ,BusinessYear " + BusinessYear + " ,Welness " + Welness + " ,TierIntention " + TierIntention + " ,jObject " + JsonConvert.SerializeObject(jObject) + " ,DedBalAvailDate " + DedBalAvailDate + " ,HSAPercentage " + HSAPercentage + " ,TaxRate " + TaxRate + " ,IsAmericanIndian " + IsAmericanIndian + " ,ResultStatus " + ResultStatus + " ,DedBalAvailToRollOver " + DedBalAvailToRollOver + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "CalculatePlans - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");

                Helpers.Service.LogError(fileName, ExceptionString);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return response;
            }
        }


    }
}

