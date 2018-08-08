using MHM.Api.ViewModel;
using MHMDal.Models;
using Newtonsoft.Json;
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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using MHM.Api.Helpers;
using System.Web;
using ClosedXML.Excel;
using System.IO;
using System.Data.Entity;
using SelectPdf;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Threading;
using MHMBLL;

namespace MHM.Api.Controllers
{

    //[RoutePrefix("api/case")]
    public class CaseController : ApiController
    {

        string[] CaseStatus = { "Open", "Final-Sent", "New", "Closed", "Final-Not Sent", "Test" };


        SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ConnectionString);
        SqlCommand cmd;

        //MHMDal.Models.MHM DB;
        private ApplicationUserManager _userManager;

        static private int rowsPerSheet = 10000;
        static private DataTable ResultsData = new DataTable();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// This api is used to save new case
        /// </summary>
        /// <param name="oCase">Json object of Case</param>
        /// <returns>Status</returns>
        [ActionName("save")]
        [Route("api/case/save")]
        [Authorize]
        public HttpResponseMessage Save(Case oCase)
        {
            Response oResponse = new Response();
            using (var DB = new MHMDal.Models.MHM())
            {
                using (var dbContextTransaction = DB.Database.BeginTransaction())
                {
                    try
                    {
                        //  oCase.InterviewDate = null;
                        if (ModelState.IsValid)
                        {
                            if (oCase.Applicant != null)
                            {
                                oCase.CreatedDateTime = DateTime.Now;
                                Applicant oApplicant = new Applicant();
                                oApplicant.EmployerId = oCase.Applicant.EmployerId;
                                oApplicant.FirstName = oCase.Applicant.FirstName.Encrypt();
                                oApplicant.LastName = oCase.Applicant.LastName.Encrypt();
                                oApplicant.CurrentPlan = oCase.Applicant.CurrentPlan;
                                oApplicant.CurrentPremium = oCase.Applicant.CurrentPremium;
                                oApplicant.Origin = oCase.Applicant.Origin;
                                oApplicant.City = oCase.Applicant.City != null ? oCase.Applicant.City.Encrypt() : null;
                                oApplicant.State = oCase.Applicant.State != null ? oCase.Applicant.State.Encrypt() : null;
                                oApplicant.Street = oCase.Applicant.Street != null ? oCase.Applicant.Street.Encrypt() : null;
                                oApplicant.Zip = oCase.Applicant.Zip != null ? oCase.Applicant.Zip.Encrypt() : null;
                                oApplicant.Email = oCase.Applicant.Email.Encrypt();
                                oApplicant.Mobile = oCase.Applicant.Mobile.Encrypt();
                                oApplicant.InsuranceTypeId = oCase.Applicant.InsuranceTypeId;
                                oApplicant.PreferredLanguage = oCase.Applicant.PreferredLanguage;
                                oApplicant.Createdby = oCase.Createdby;
                                oApplicant.CreatedDateTime = oCase.CreatedDateTime;

                                DB.Applicants.Add(oApplicant);
                                DB.SaveChanges();
                                oCase.ApplicantID = oApplicant.ApplicantID;
                            }

                            Case OCaseModel = new Case();
                            OCaseModel.ApplicantID = oCase.ApplicantID;
                            OCaseModel.CaseTitle = oCase.CaseTitle;
                            OCaseModel.Createdby = oCase.Createdby;
                            OCaseModel.CreatedDateTime = DateTime.Now;
                            OCaseModel.FPL = oCase.FPL;
                            OCaseModel.HSAAmount = oCase.HSAAmount;
                            OCaseModel.HSAFunding = oCase.HSAFunding;
                            OCaseModel.HSALimit = oCase.HSALimit;
                            OCaseModel.IssuerID = oCase.IssuerID;
                            OCaseModel.MAGIncome = oCase.MAGIncome;
                            OCaseModel.MonthlySubsidy = oCase.MonthlySubsidy;
                            OCaseModel.Notes = oCase.Notes != null ? oCase.Notes.Encrypt() : null;
                            OCaseModel.PlanID = oCase.PlanID;
                            OCaseModel.PreviousYrHSA = oCase.PreviousYrHSA;
                            OCaseModel.TaxRate = oCase.TaxRate;
                            OCaseModel.TotalMedicalUsage = oCase.TotalMedicalUsage;
                            OCaseModel.UsageID = oCase.UsageID;
                            OCaseModel.Welness = oCase.Welness;
                            OCaseModel.Year = oCase.Year;
                            OCaseModel.ZipCode = oCase.ZipCode;
                            OCaseModel.CountyName = oCase.CountyName;
                            OCaseModel.RatingAreaId = oCase.RatingAreaId;
                            OCaseModel.Applicant = null;
                            OCaseModel.Families = null;
                            OCaseModel.IsSubsidy = oCase.IsSubsidy;
                            OCaseModel.StatusId = oCase.StatusId;
                            OCaseModel.JobNumber = !string.IsNullOrEmpty(oCase.JobNumber) ? oCase.JobNumber : null;
                            OCaseModel.CaseSource = oCase.CaseSource;
                            OCaseModel.CaseJobRunStatus = oCase.CaseJobRunStatus;
                            OCaseModel.DedBalAvailDate = oCase.DedBalAvailDate;
                            OCaseModel.DedBalAvailToRollOver = oCase.DedBalAvailToRollOver;
                            OCaseModel.TierIntention = oCase.TierIntention;
                            OCaseModel.PrimaryCase = oCase.PrimaryCase;
                            OCaseModel.AlternateCase = oCase.AlternateCase;
                            OCaseModel.InterviewDate = oCase.InterviewDate;
                            OCaseModel.Agent = oCase.Agent;
                            DB.Cases.Add(OCaseModel);
                            DB.SaveChanges();

                            oCase.CaseID = OCaseModel.CaseID;

                            // oCase.Families.ToList().ForEach(r => r.CaseNumId = oCase.CaseID);
                            // DB.Families.AddRange(oCase.Families);
                            // DB.SaveChanges();

                            foreach (var itemFM in oCase.Families)
                            {
                                Family oFamily = new Family()
                                {
                                    CaseNumId = oCase.CaseID,
                                    //Gender = itemFM.Gender.Encrypt(),
                                    //DOB = itemFM.DOB.Encrypt(),
                                    Gender = GenerateEncryptedString.GetEncryptedString(itemFM.Gender),
                                    DOB = GenerateEncryptedString.GetEncryptedString(itemFM.DOB),
                                    Age = itemFM.Age,
                                    Createdby = oCase.Createdby,
                                    CreatedDateTime = DateTime.Now,
                                    IsPrimary = itemFM.IsPrimary,
                                    Smoking = itemFM.Smoking,
                                    TotalMedicalUsage = itemFM.TotalMedicalUsage
                                };

                                DB.Families.Add(oFamily);
                                DB.SaveChanges();
                                var BenefitUserDetails = itemFM.BenefitUserDetails.ToList();
                                BenefitUserDetails.ForEach(r =>
                                {
                                    r.UsageCost = r.UsageCost;
                                    r.UsageQty = r.UsageQty;
                                    r.UsageNotes = r.UsageNotes != null ? r.UsageNotes.Encrypt() : null;
                                    r.FamilyID = oFamily.FamilyID;
                                    r.Createdby = oCase.Createdby;
                                    r.CreatedDateTime = DateTime.Now;
                                });
                                DB.BenefitUserDetails.AddRange(BenefitUserDetails);
                                DB.SaveChanges();

                                var Criticalillness = itemFM.Criticalillnesses.ToList();
                                Criticalillness.ForEach(r =>
                                {
                                    r.FamilyID = oFamily.FamilyID;
                                    r.Createdby = oCase.Createdby;
                                    r.CreatedDateTime = DateTime.Now;
                                });
                                DB.Criticalillnesses.AddRange(Criticalillness);
                                DB.SaveChanges();

                            }

                            // Save Case Result
                            var CasePlanResults = oCase.CasePlanResults.ToList();
                            if (CasePlanResults.Count() > 0)
                            {
                                CasePlanResults.ForEach(r =>
                                {
                                    r.PlanId = r.PlanId;
                                    r.PersonalHSAContribution = r.PersonalHSAContribution;
                                    r.ReferralForSpecialist = r.ReferralForSpecialist;

                                    r.CaseId = oCase.CaseID;
                                    r.GrossAnnualPremium = r.GrossAnnualPremium;
                                    r.FederalSubsidy = r.FederalSubsidy;
                                    r.NetAnnualPremium = r.NetAnnualPremium;
                                    r.MonthlyPremium = r.MonthlyPremium;
                                    r.Copays = r.Copays;
                                    r.PaymentsToDeductibleLimit = r.PaymentsToDeductibleLimit;
                                    r.CoinsuranceToOutOfPocketLimit = r.CoinsuranceToOutOfPocketLimit;
                                    r.ContributedToYourHSAAccount = r.ContributedToYourHSAAccount;
                                    r.EmployerHRAReimbursement = r.EmployerHRAReimbursement;
                                    r.TaxSavingFromHSAAccount = r.TaxSavingFromHSAAccount;
                                    r.Medical = r.Medical;
                                    r.TotalPaid = r.TotalPaid;
                                    r.PaymentsByInsuranceCo = r.PaymentsByInsuranceCo;
                                    r.DeductibleSingle = r.DeductibleSingle;
                                    r.DeductibleFamilyPerPerson = r.DeductibleFamilyPerPerson;
                                    r.DeductibleFamilyPerGroup = r.DeductibleFamilyPerGroup;
                                    r.OPLSingle = r.OPLSingle;
                                    r.OPLFamilyPerPerson = r.OPLFamilyPerPerson;
                                    r.OPLFamilyPerGroup = r.OPLFamilyPerGroup;
                                    r.Coinsurance = r.Coinsurance;
                                    r.WorstCase = r.WorstCase;
                                    r.HRAReimbursedAmt = r.HRAReimbursedAmt;
                                    r.CreatedDateTime = DateTime.Now;
                                    r.Createdby = oCase.Createdby;
                                    r.CreatedDateTime = DateTime.Now;
                                });
                                DB.CasePlanResults.AddRange(CasePlanResults);
                                DB.SaveChanges();
                            }

                            dbContextTransaction.Commit();
                            Dictionary<string, object> oRes = new Dictionary<string, object>();
                            oRes.Add("Status", true);
                            oRes.Add("Message", "Success");
                            oRes.Add("CaseId", oCase.CaseID);
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oRes);
                            return response;

                        }
                        else
                        {
                            string messages = string.Join(Environment.NewLine, ModelState.Values
                                                                        .SelectMany(x => x.Errors)
                                                                        .Select(x => x.ErrorMessage));

                            oResponse.Status = false;
                            oResponse.Message = messages;
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        string Error = "";
                        foreach (var eve in e.EntityValidationErrors)
                        {

                            Error = "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + ' ' + eve.Entry.State;
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Error = "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + ' ' + ve.ErrorMessage;
                            }
                        }
                        oResponse.Status = false;
                        oResponse.Message = Error;

                        string ExceptionString = "Api : Save" + Environment.NewLine;
                        ExceptionString += "Request :  " + JsonConvert.SerializeObject(oCase) + Environment.NewLine;
                        ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                        var fileName = "Save - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                        Helpers.Service.LogError(fileName, ExceptionString);

                        dbContextTransaction.Rollback();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        oResponse.Status = false;
                        oResponse.Message = ex.ToString();

                        string ExceptionString = "Api : Save" + Environment.NewLine;
                        ExceptionString += "Request :  " + JsonConvert.SerializeObject(oCase) + Environment.NewLine;
                        ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                        var fileName = "Save - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                        Helpers.Service.LogError(fileName, ExceptionString);

                        dbContextTransaction.Rollback();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return response;
                    }
                }
            }
        }


        /// <summary>
        /// This api is used to save new case
        /// </summary>
        /// <param name="oCase">Json object of Case</param>
        /// <returns>Status</returns>
        [ActionName("UpdateCase")]
        [Route("api/case/UpdateCase")]
        [Authorize]
        public HttpResponseMessage UpdateCase(Case oCase)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                if (DB.Cases.Any(x => x.CaseID == oCase.CaseID))
                {
                    using (var dbContextTransaction = DB.Database.BeginTransaction())
                    {
                        try
                        {
                            if (ModelState.IsValid)
                            {
                                if (oCase.Applicant != null)
                                {
                                    Applicant oApplicant = DB.Applicants.Where(r => r.ApplicantID == oCase.ApplicantID).FirstOrDefault();
                                    oApplicant.EmployerId = oCase.Applicant.EmployerId;
                                    oApplicant.FirstName = oCase.Applicant.FirstName.Encrypt();
                                    oApplicant.LastName = oCase.Applicant.LastName.Encrypt();
                                    oApplicant.CurrentPlan = oCase.Applicant.CurrentPlan;
                                    oApplicant.CurrentPremium = oCase.Applicant.CurrentPremium;
                                    oApplicant.Origin = oCase.Applicant.Origin;
                                    oApplicant.City = oCase.Applicant.City != null ? oCase.Applicant.City.Encrypt() : null;
                                    oApplicant.State = oCase.Applicant.State != null ? oCase.Applicant.State.Encrypt() : null;
                                    oApplicant.Street = oCase.Applicant.Street != null ? oCase.Applicant.Street.Encrypt() : null;
                                    oApplicant.Zip = oCase.Applicant.Zip != null ? oCase.Applicant.Zip.Encrypt() : null;
                                    oApplicant.Email = oCase.Applicant.Email.Encrypt();
                                    oApplicant.Mobile = oCase.Applicant.Mobile.Encrypt();
                                    oApplicant.InsuranceTypeId = oCase.Applicant.InsuranceTypeId;
                                    oApplicant.PreferredLanguage = oCase.Applicant.PreferredLanguage;
                                    oApplicant.ModifiedBy = oCase.ModifiedBy;
                                    oApplicant.ModifiedDateTime = DateTime.Now;
                                    DB.SaveChanges();
                                }

                                Case OCaseModel = DB.Cases.Where(r => r.CaseID == oCase.CaseID).FirstOrDefault();
                                OCaseModel.ApplicantID = oCase.ApplicantID;
                                OCaseModel.CaseTitle = oCase.CaseTitle;
                                OCaseModel.ModifiedBy = oCase.ModifiedBy;
                                OCaseModel.ModifiedDateTime = DateTime.Now;
                                OCaseModel.FPL = oCase.FPL;
                                OCaseModel.HSAAmount = oCase.HSAAmount;
                                OCaseModel.HSAFunding = oCase.HSAFunding;
                                OCaseModel.HSALimit = oCase.HSALimit;
                                OCaseModel.IssuerID = oCase.IssuerID;
                                OCaseModel.MAGIncome = oCase.MAGIncome;
                                OCaseModel.MonthlySubsidy = oCase.MonthlySubsidy;
                                OCaseModel.Notes = oCase.Notes != null ? oCase.Notes.Encrypt() : null;
                                OCaseModel.PlanID = oCase.PlanID;
                                OCaseModel.PreviousYrHSA = oCase.PreviousYrHSA;
                                OCaseModel.TaxRate = oCase.TaxRate;
                                OCaseModel.TotalMedicalUsage = oCase.TotalMedicalUsage;
                                OCaseModel.UsageID = oCase.UsageID;
                                OCaseModel.Welness = oCase.Welness;
                                OCaseModel.Year = oCase.Year;
                                OCaseModel.ZipCode = oCase.ZipCode;
                                OCaseModel.CountyName = oCase.CountyName;
                                OCaseModel.RatingAreaId = oCase.RatingAreaId;
                                OCaseModel.Families = null;
                                OCaseModel.IsSubsidy = oCase.IsSubsidy;
                                OCaseModel.StatusId = oCase.StatusId;
                                OCaseModel.JobNumber = !string.IsNullOrEmpty(oCase.JobNumber) ? oCase.JobNumber : null;
                                OCaseModel.CaseSource = oCase.CaseSource;
                                OCaseModel.CaseJobRunStatus = oCase.CaseJobRunStatus;
                                OCaseModel.DedBalAvailDate = oCase.DedBalAvailDate;
                                OCaseModel.DedBalAvailToRollOver = oCase.DedBalAvailToRollOver;
                                OCaseModel.TierIntention = oCase.TierIntention;
                                OCaseModel.PrimaryCase = oCase.PrimaryCase;
                                OCaseModel.AlternateCase = oCase.AlternateCase;
                                OCaseModel.InterviewDate = oCase.InterviewDate;
                                OCaseModel.Agent = oCase.Agent;
                                DB.SaveChanges();


                                //Remove Families, BenefitUsesDetails, Criticalillness
                                var families = DB.Families.Where(r => r.CaseNumId == oCase.CaseID).ToList();
                                foreach (var item in families)
                                {
                                    var BenefitUses = DB.BenefitUserDetails.Where(r => r.FamilyID == item.FamilyID).ToList();
                                    DB.BenefitUserDetails.RemoveRange(BenefitUses);

                                    var Criticalillnes = DB.Criticalillnesses.Where(r => r.FamilyID == item.FamilyID).ToList();
                                    DB.Criticalillnesses.RemoveRange(Criticalillnes);

                                }
                                DB.Families.RemoveRange(families);
                                //Remove CasePlanResult
                                var CasePlanResult = DB.CasePlanResults.Where(r => r.CaseId == oCase.CaseID).ToList();
                                DB.CasePlanResults.RemoveRange(CasePlanResult);
                                DB.SaveChanges();


                                var lstfamilies = oCase.Families.ToList();
                                foreach (var itemFM in lstfamilies)
                                {
                                    var BenefitUserDetails = itemFM.BenefitUserDetails.ToList();
                                    var Criticalillness = itemFM.Criticalillnesses.ToList();

                                    DB.Entry(itemFM).State = EntityState.Detached;

                                    itemFM.CaseNumId = oCase.CaseID;
                                    //itemFM.Gender = itemFM.Gender.Encrypt();
                                    //itemFM.DOB = itemFM.DOB.Encrypt();
                                    itemFM.Gender = GenerateEncryptedString.GetEncryptedString(itemFM.Gender);
                                    itemFM.DOB = GenerateEncryptedString.GetEncryptedString(itemFM.DOB);
                                    itemFM.Age = itemFM.Age;
                                    itemFM.ModifiedBy = oCase.Createdby;
                                    itemFM.ModifiedDateTime = DateTime.Now;
                                    itemFM.IsPrimary = itemFM.IsPrimary;
                                    itemFM.Smoking = itemFM.Smoking;
                                    itemFM.TotalMedicalUsage = itemFM.TotalMedicalUsage;
                                    itemFM.BenefitUserDetails = null;
                                    itemFM.Criticalillnesses = null;
                                    DB.Families.Add(itemFM);
                                    DB.SaveChanges();


                                    BenefitUserDetails.ForEach(r =>
                                    {
                                        r.UsageCost = r.UsageCost;
                                        r.UsageQty = r.UsageQty;
                                        r.UsageNotes = r.UsageNotes != null ? r.UsageNotes.Encrypt() : null;
                                        r.FamilyID = itemFM.FamilyID;
                                        r.ModifiedBy = oCase.Createdby;
                                        r.ModifiedDateTime = DateTime.Now;
                                    });
                                    DB.BenefitUserDetails.AddRange(BenefitUserDetails);
                                    DB.SaveChanges();


                                    Criticalillness.ForEach(r =>
                                    {
                                        r.Id = 0;
                                        r.FamilyID = itemFM.FamilyID;
                                        r.ModifiedBy = oCase.Createdby;
                                        r.ModifiedDateTime = DateTime.Now;
                                    });
                                    DB.Criticalillnesses.AddRange(Criticalillness);
                                    DB.SaveChanges();

                                }

                                // Save Case Result
                                var CasePlanResults = oCase.CasePlanResults.ToList();
                                foreach (var r in CasePlanResults)
                                {
                                    DB.Entry(r).State = EntityState.Detached;

                                    r.PlanId = r.PlanId;
                                    r.PersonalHSAContribution = r.PersonalHSAContribution;
                                    r.ReferralForSpecialist = r.ReferralForSpecialist;

                                    r.CaseId = oCase.CaseID;
                                    r.GrossAnnualPremium = r.GrossAnnualPremium;
                                    r.FederalSubsidy = r.FederalSubsidy;
                                    r.NetAnnualPremium = r.NetAnnualPremium;
                                    r.MonthlyPremium = r.MonthlyPremium;
                                    r.Copays = r.Copays;
                                    r.PaymentsToDeductibleLimit = r.PaymentsToDeductibleLimit;
                                    r.CoinsuranceToOutOfPocketLimit = r.CoinsuranceToOutOfPocketLimit;
                                    r.ContributedToYourHSAAccount = r.ContributedToYourHSAAccount;
                                    r.EmployerHRAReimbursement = r.EmployerHRAReimbursement;
                                    r.TaxSavingFromHSAAccount = r.TaxSavingFromHSAAccount;
                                    r.Medical = r.Medical;
                                    r.TotalPaid = r.TotalPaid;
                                    r.PaymentsByInsuranceCo = r.PaymentsByInsuranceCo;
                                    r.DeductibleSingle = r.DeductibleSingle;
                                    r.DeductibleFamilyPerPerson = r.DeductibleFamilyPerPerson;
                                    r.DeductibleFamilyPerGroup = r.DeductibleFamilyPerGroup;
                                    r.OPLSingle = r.OPLSingle;
                                    r.OPLFamilyPerPerson = r.OPLFamilyPerPerson;
                                    r.OPLFamilyPerGroup = r.OPLFamilyPerGroup;
                                    r.Coinsurance = r.Coinsurance;
                                    r.WorstCase = r.WorstCase;
                                    r.CreatedDateTime = DateTime.Now;
                                    r.HRAReimbursedAmt = r.HRAReimbursedAmt;
                                    DB.CasePlanResults.Add(r);
                                    DB.SaveChanges();
                                }

                                dbContextTransaction.Commit();
                                Dictionary<string, object> oRes = new Dictionary<string, object>();
                                oRes.Add("Status", true);
                                oRes.Add("Message", "Success");
                                oRes.Add("CaseId", oCase.CaseID);
                                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oRes);
                                return response;

                            }
                            else
                            {

                                string messages = string.Join(Environment.NewLine, ModelState.Values
                                                    .SelectMany(x => x.Errors)
                                                    .Select(x => x.ErrorMessage));

                                oResponse.Status = false;
                                oResponse.Message = messages;
                                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                                return response;
                            }
                        }
                        catch (Exception ex)
                        {
                            oResponse.Status = false;
                            oResponse.Message = ex.ToString();

                            string ExceptionString = "Api : UpdateCase" + Environment.NewLine;
                            ExceptionString += "Request :  " + JsonConvert.SerializeObject(oCase) + Environment.NewLine;
                            ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                            var fileName = "UpdateCase - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                            Helpers.Service.LogError(fileName, ExceptionString);

                            dbContextTransaction.Rollback();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                            return response;
                        }
                    }
                }
                else
                {
                    oResponse.Status = true;
                    oResponse.Message = "Case not found";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;

                }
            }
        }

        [Route("api/case/UpdateCaseOPT/{CaseId}/{Status}")]
        [Authorize]
        [HttpPost]
        public HttpResponseMessage UpdateCaseOPT(long CaseId, Boolean Status)
        {
            Response oResponse = new Response();


            try
            {
                var DB = new MHMDal.Models.MHM();
                if (Status)
                {
                    Case OCaseModel = DB.Cases.Where(r => r.CaseID == CaseId).FirstOrDefault();
                    if (!DB.Cases.Any(r => r.Applicant.FirstName == OCaseModel.Applicant.FirstName && r.Applicant.LastName == OCaseModel.Applicant.LastName && r.Applicant.Mobile == OCaseModel.Applicant.Mobile && r.CaseID != OCaseModel.CaseID && r.CaseStatusMst.StatusCode != "Test" && r.CaseStatusMst.StatusCode != "Test - Sent"))
                    {
                        OCaseModel.PrimaryCase = Status;
                        DB.SaveChanges();
                        oResponse.Status = true;
                        oResponse.Message = "Update Success";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "One case already exist in Opt.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                else
                {
                    Case OCaseModel = DB.Cases.Where(r => r.CaseID == CaseId).FirstOrDefault();
                    OCaseModel.PrimaryCase = Status;
                    DB.SaveChanges();
                    oResponse.Status = true;
                    oResponse.Message = "Update Success";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.ToString();

                string ExceptionString = "Api : UpdateCaseOPT" + Environment.NewLine;
                ExceptionString += "Request :  CaseId: " + CaseId + " / Status: " + Status + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateCaseOPT - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [Route("api/case/UpdateCaseAlternate/{CaseId}/{Status}")]
        [Authorize]
        [HttpPost]
        public HttpResponseMessage UpdateCaseAlternate(long CaseId, Boolean Status)
        {
            Response oResponse = new Response();


            try
            {
                var DB = new MHMDal.Models.MHM();
                Case OCaseModel = DB.Cases.Where(r => r.CaseID == CaseId).FirstOrDefault();
                OCaseModel.AlternateCase = Status;
                DB.SaveChanges();

                oResponse.Status = true;
                oResponse.Message = "Update Success";
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return response;
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.ToString();

                string ExceptionString = "Api : UpdateCaseAlternate" + Environment.NewLine;
                ExceptionString += "Request :  CaseId: " + CaseId + " / Status: " + Status + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "UpdateCaseAlternate - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        /// <summary>
        /// This api is used to get case details
        /// </summary>
        /// <param name="CaseID">CaseNumber/ID</param>
        /// <returns>Case Details</returns>
        [ActionName("Details")]
        [HttpGet]
        [Route("api/case/details/{CaseID}")]
        [Authorize]
        public HttpResponseMessage Details(long CaseID)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    //var UserId = User.Identity.GetUserId();
                    //var UserRole = UserManager.GetRoles(UserId).First();
                    //string UserEmail = UserManager.GetEmail(UserId);
                    Case oCase = DB.Cases.Where(m => m.CaseID == CaseID).FirstOrDefault();
                    //.Include(b => b.Applicant).Include(b => b.Families).Include(b => b.CasePlanResults)
                    // if (UserRole == "Admin")  //hard-coded value
                    // {
                    //oCase = DB.Cases.Where(m => m.CaseID == CaseID).FirstOrDefault();
                    // }
                    // else
                    // {
                    //     var clientCompanyId = DB.Users.Where(m => m.Email == UserEmail).Select(r => r.ClientCompanyId).FirstOrDefault();
                    //     var users = DB.Users.Where(r => r.ClientCompanyId == clientCompanyId).Select(r => r.UserID).ToList();
                    //    oCase = DB.Cases.Where(m => m.CaseID == CaseID && users.Contains(m.Createdby)).FirstOrDefault();
                    //}

                    if (oCase != null)
                    {
                        Dictionary<string, object> res = new Dictionary<string, object>();
                        oCase.Applicant.City = oCase.Applicant.City != null ? oCase.Applicant.City.Decrypt() : null;
                        oCase.Applicant.Email = oCase.Applicant.Email != null ? oCase.Applicant.Email.Decrypt() : null;
                        oCase.Applicant.FirstName = oCase.Applicant.FirstName != null ? oCase.Applicant.FirstName.Decrypt() : null;
                        oCase.Applicant.LastName = oCase.Applicant.LastName != null ? oCase.Applicant.LastName.Decrypt() : null;
                        oCase.Applicant.Mobile = oCase.Applicant.Mobile != null ? oCase.Applicant.Mobile.Decrypt() : null;
                        oCase.Applicant.Street = oCase.Applicant.Street != null ? oCase.Applicant.Street.Decrypt() : null;
                        oCase.Applicant.State = oCase.Applicant.State != null ? oCase.Applicant.State.Decrypt() : null;
                        oCase.Applicant.Zip = oCase.Applicant.Zip != null ? oCase.Applicant.Zip.Decrypt() : null;
                        oCase.Applicant.EmployerName = oCase.Applicant.EmployerMst.EmployerName;
                        oCase.Notes = oCase.Notes != null ? oCase.Notes.Decrypt() : null;
                        oCase.Applicant.CurrentPlan = oCase.Applicant.CurrentPlan;
                        oCase.JobMaster.PlanYearStartDt = oCase.JobMaster.PlanYearStartDt;
                        oCase.JobMaster.PlanYearEndDt = oCase.JobMaster.PlanYearEndDt;
                        //oCase.InterviewDate=oCase.InterviewDate!= null ? oCase.Applicant.InterviewDate.Decrypt() : null;

                        IFormatProvider yyyymmddFormat = new System.Globalization.CultureInfo(String.Empty, false);


                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); //dd/MM/yyyy

                        DateTime date;

                        oCase.Families.ToList().ForEach(r =>
                        {
                            // r.Gender = r.Gender.Decrypt();
                            r.Gender = GenerateEncryptedString.GetDecryptedString(r.Gender);
                            var DOB = GenerateEncryptedString.GetDecryptedString(r.DOB);
                            var status = DateTime.TryParse(DOB, out date);
                            if (!status) { date = DateTime.Parse(DOB, CultureInfo.CreateSpecificCulture("fr-FR")); }
                            r.DOB = date.ToString("MM/dd/yyyy");
                            r.Criticalillnesses.ToList();
                            r.BenefitUserDetails.ToList().ForEach(t => { t.UsageNotes = t.UsageNotes != null ? t.UsageNotes.Decrypt() : null; });
                        });

                        oCase.CasePlanResults.ToList().ForEach(r =>
                        {
                            r.PlanId = r.PlanId;
                            r.PersonalHSAContribution = r.PersonalHSAContribution;
                            r.ReferralForSpecialist = r.ReferralForSpecialist;

                            r.GrossAnnualPremium = r.GrossAnnualPremium;
                            r.FederalSubsidy = r.FederalSubsidy;
                            r.NetAnnualPremium = r.NetAnnualPremium;
                            r.MonthlyPremium = r.MonthlyPremium;
                            r.Copays = r.Copays;
                            r.PaymentsToDeductibleLimit = r.PaymentsToDeductibleLimit;
                            r.CoinsuranceToOutOfPocketLimit = r.CoinsuranceToOutOfPocketLimit;
                            r.ContributedToYourHSAAccount = r.ContributedToYourHSAAccount != null ? r.ContributedToYourHSAAccount : null;
                            r.TaxSavingFromHSAAccount = r.TaxSavingFromHSAAccount;
                            r.Medical = r.Medical;
                            r.TotalPaid = r.TotalPaid;
                            r.PaymentsByInsuranceCo = r.PaymentsByInsuranceCo;
                            r.DeductibleSingle = r.DeductibleSingle;
                            r.DeductibleFamilyPerPerson = r.DeductibleFamilyPerPerson;
                            r.DeductibleFamilyPerGroup = r.DeductibleFamilyPerGroup;
                            r.OPLSingle = r.OPLSingle;
                            r.OPLFamilyPerPerson = r.OPLFamilyPerPerson;
                            r.OPLFamilyPerGroup = r.OPLFamilyPerGroup;
                            r.Coinsurance = r.Coinsurance;
                            r.WorstCase = r.WorstCase;
                            r.PlanName = r.PlanName;
                            r.HRAReimbursedAmt = r.HRAReimbursedAmt;
                            //test = test + 1;
                        });
                        oCase.CasePlanResults = oCase.CasePlanResults.Take(5).ToList();
                        oCase.CaseStatusMst = oCase.CaseStatusMst;
                        oCase.IssuerMst = oCase.IssuerMst;

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("Case", oCase);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Case does not exist.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : Details" + Environment.NewLine;
                    ExceptionString += "Request :  CaseID " + CaseID + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "Details - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to get all cases
        /// </summary>
        /// <param name="lstParameter">List of parameters</param>
        /// <param name="EmailId">Email Id</param>
        /// <param name="searchby">Search by value</param>
        /// <param name="sortby">Sory by</param>
        /// <param name="desc">Ascending or Descending</param>
        /// <param name="page">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <returns></returns>
        [ActionName("GetAll")]
        [HttpGet]
        [Route("api/case/getall")]
        [Authorize]
        public HttpResponseMessage GetAll([FromUri]  List<string> lstParameter, string EmailId, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null)  //
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                //to get current user id
                var UserId = User.Identity.GetUserId();
                var UserRole = UserManager.GetRoles(UserId).First();
                string UserEmail = UserManager.GetEmail(UserId);

                List<CaseListViewModel> oModellst = new List<CaseListViewModel>();
                DataTable dtcases = new DataTable();
                try
                {
                    objCon.Open();
                    cmd = new SqlCommand("get_Case_List", objCon);
                    cmd.Parameters.AddWithValue("@UserEmail", UserEmail);
                    cmd.Parameters.AddWithValue("@UserRole", UserRole);
                    cmd.Parameters.AddWithValue("@PageNo", page);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    if (!string.IsNullOrEmpty(searchby))
                    {
                        cmd.Parameters.AddWithValue("@CaseId", searchby.Trim());
                    }
                    if (startDate != null)
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                    }
                    if (endDate != null)
                    {
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                    }

                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        cmd.Parameters.AddWithValue("@EmployerCompany", lstParameter[0].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]) && string.IsNullOrEmpty(searchby))
                    {
                        cmd.Parameters.AddWithValue("@CaseTitle", lstParameter[1].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[2]))
                    {
                        cmd.Parameters.AddWithValue("@CreatedBy", lstParameter[2].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[3]))
                    {
                        cmd.Parameters.AddWithValue("@MobileNo", lstParameter[3].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[4]))
                    {
                        cmd.Parameters.AddWithValue("@JobNumber", lstParameter[4].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[5]))
                    {
                        //List<CaseStatusParamter> CaseStatusArray = (List<CaseStatusParamter>)JsonConvert.DeserializeObject(lstParameter[5].ToString(), (typeof(List<CaseStatusParamter>)));
                        //var result = string.Join(",", CaseStatusArray.Select(r => r.Id));

                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(lstParameter[5].ToString(), (typeof(DataTable)));

                        SqlParameter param = new SqlParameter("@CaseStatusIds", SqlDbType.Structured)
                        {
                            TypeName = "dbo.tblCaseStatus",
                            Value = dt
                        };
                        cmd.Parameters.Add(param);
                        //cmd.Parameters.AddWithValue("@CaseStatusIds", result);
                    }
                    if (!string.IsNullOrEmpty(lstParameter[6]))
                    {
                        cmd.Parameters.AddWithValue("@BusinessYear", lstParameter[6].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[7]))
                    {
                        cmd.Parameters.AddWithValue("@PrimaryCase", lstParameter[7].ToString().Trim());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[8]))
                    {
                        cmd.Parameters.AddWithValue("@AlternateCase", lstParameter[8].ToString().Trim());
                    }

                    if (desc)
                    {
                        cmd.Parameters.AddWithValue("@SortOrder", "DESC");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SortOrder", "ASC");
                    }

                    switch (sortby)
                    {
                        case "Name":
                            cmd.Parameters.AddWithValue("@SortColumn", "ApplicantName");

                            break;

                        case "Carrier":
                            cmd.Parameters.AddWithValue("@SortColumn", "Carrier");
                            break;

                        case "CreateDate":
                            cmd.Parameters.AddWithValue("@SortColumn", "CreatedDateTime");
                            break;

                        case "CaseTitle":
                            cmd.Parameters.AddWithValue("@SortColumn", "CaseTitle");
                            break;

                        case "CaseID":
                            cmd.Parameters.AddWithValue("@SortColumn", "CaseID");
                            break;

                        default:
                            cmd.Parameters.AddWithValue("@SortColumn", "CreatedDateTime");
                            break;
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtcases);

                    foreach (DataRow dr in dtcases.Rows)
                    {
                        var cse = new CaseListViewModel();

                        cse.CaseID = Convert.ToInt64(dr["CaseID"]);
                        cse.Carrier = dr["carrier"] != DBNull.Value ? dr["carrier"].ToString() : "";
                        cse.CaseTitle = dr["CaseTitle"] != DBNull.Value ? dr["CaseTitle"].ToString() : "";
                        //ApplicantLName = dr["LastName"].ToString();
                        cse.ApplicantName = string.Join(" ", dr["ApplicantName"].ToString().Split(' ').Select(c => { c = c.Decrypt(); return c; }).ToArray());
                        cse.CreatedBy = dr["CreatedBy"].ToString();
                        cse.MobileNo = dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString().Decrypt() : "";
                        cse.EmployerName = dr["EmployerName"] != DBNull.Value ? dr["EmployerName"].ToString() : "";
                        cse.ComapnyName = dr["CompanyName"] != DBNull.Value ? dr["CompanyName"].ToString() : "";
                        cse.JobNumber = dr["JobNumber"] != DBNull.Value ? dr["JobNumber"].ToString() : "";
                        cse.StatusCode = dr["StatusCode"] != DBNull.Value ? dr["StatusCode"].ToString() : "";
                        cse.Editable = dr["Editable"] != DBNull.Value ? Convert.ToBoolean(dr["Editable"]) : true;
                        cse.BusinessYear = dr["BusinessYear"] != DBNull.Value ? dr["BusinessYear"].ToString() : "";
                        cse.CaseSource = dr["CaseSource"] != DBNull.Value ? dr["CaseSource"].ToString() : "";
                        cse.CreatedDateTime = dr["CreatedDateTime"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedDateTime"]) : DateTime.Now;
                        cse.UsageType = dr["UsageType"] != DBNull.Value ? dr["UsageType"].ToString() : "";
                        cse.Agent = dr["Agent"] != DBNull.Value ? dr["Agent"].ToString() : "";
                        cse.PrimaryCase = dr["PrimaryCase"] != DBNull.Value ? Convert.ToBoolean(dr["PrimaryCase"]) : false;
                        cse.AlternateCase = dr["AlternateCase"] != DBNull.Value ? Convert.ToBoolean(dr["AlternateCase"]) : false;
                        oModellst.Add(cse);
                    }

                    int total = dtcases.Rows.Count;

                    if (total > 0)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("Cases", oModellst);
                        res.Add("TotalCount", dtcases.Rows[0]["TotalCount"].ToString());
                        res.Add("UserData", "User Id : " + UserId + " UserEmail : " + UserEmail + " UserRole : " + UserRole);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Case does not exist.");
                        res.Add("UserData", "User Id : " + UserId + " UserEmail : " + UserEmail + " UserRole : " + UserRole);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetAll" + Environment.NewLine;
                    ExceptionString += "Request :  " + " lstParameter " + lstParameter + " ,EmailId " + EmailId + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + " ,startDate " + startDate + " ,endDate " + endDate + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetAll - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
                finally
                {
                    cmd.Dispose();
                    objCon.Close();
                }
            }
        }

        /// <summary>
        ///  This api is used to get all Employer
        /// </summary>
        /// <param name="Year">Year</param>
        /// <returns>List of employers whose plans available for selected year</returns>
        [ActionName("Employer")]
        [HttpGet]
        [Route("api/case/employer")]
        [Authorize]
        public EmployerViewModel Employer()
        {
            EmployerViewModel oEmployerViewModel = new EmployerViewModel();
            List<EmployerMst> oModellst = new List<EmployerMst>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    oModellst = DB.EmployerMsts.ToList();

                    oEmployerViewModel.Employers = oModellst;
                    if (oEmployerViewModel.Employers.Count > 0)
                    {
                        oEmployerViewModel.Status = true;
                        oEmployerViewModel.Message = "Success";
                    }
                    else
                    {
                        oEmployerViewModel.Status = true;
                        oEmployerViewModel.Message = "Employer does not exist.";
                    }
                    return oEmployerViewModel;
                }

                catch (Exception ex)
                {
                    oEmployerViewModel.Status = false;
                    oEmployerViewModel.Message = ex.Message;

                    string ExceptionString = "Api : Employer" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oEmployerViewModel) + Environment.NewLine;
                    var fileName = "Employer - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);
                    return oEmployerViewModel;
                }
            }
        }

        /// <summary>
        /// This api is used to get all the carriers
        /// </summary>
        /// <param name="EmployerId">Employer Id</param>
        /// <param name="ZipCode">Zipcode</param>
        /// <param name="CountyId">County Id</param>
        /// <param name="InsuranceTypeId">Insurance Type Id</param>
        /// <param name="BusinessYear">Business year</param>
        /// <returns>list of carriers</returns>
        [ActionName("Carrier")]
        [HttpGet]
        [Route("api/case/carrier/{EmployerId}/{ZipCode}/{CountyName}/{InsuranceTypeId}/{BusinessYear}")]
        [Authorize]
        public CarrierViewModel Carrier(long EmployerId, string ZipCode, string CountyName, int InsuranceTypeId, string BusinessYear)
        {
            List<IssuerMst> lstIssuer = new List<IssuerMst>();
            CarrierViewModel oCarrierViewModel = new CarrierViewModel();
            List<IssuerMst> oModellst = new List<IssuerMst>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    if (EmployerId == MHMBLL.Constants.DefaultEmployerID)
                    {

                        var IssuerIds = DB.PlanAttributeMsts.Where(t => t.MrktCover == "Indi" && t.BusinessYear == BusinessYear).Select(r => r.CarrierId).Distinct().ToList();
                        var StateCode = DB.qryZipCodeToRatingAreas.Where(r => r.Zip == ZipCode && r.CountyName == CountyName).FirstOrDefault().StateCode;
                        lstIssuer = DB.IssuerMsts.Where(r => IssuerIds.Contains(r.Id) && r.StateCode == StateCode && r.Status == true).ToList();
                    }
                    else if (EmployerId == MHMBLL.Constants.ShopEmployerId)
                    {

                        var IssuerIds = DB.PlanAttributeMsts.Where(t => t.MrktCover == "Shop" && t.BusinessYear == BusinessYear).Select(r => r.CarrierId).Distinct().ToList();
                        var StateCode = DB.qryZipCodeToRatingAreas.Where(r => r.Zip == ZipCode && r.CountyName == CountyName).FirstOrDefault().StateCode;
                        lstIssuer = DB.IssuerMsts.Where(r => IssuerIds.Contains(r.Id) && r.StateCode == StateCode && r.Status == true).ToList();
                    }
                    else
                    {
                        //Vaibhav EmployerId
                        //var IssuerIds = DB.PlanAttributeMsts.Where(t => t.MrktCover == "Group" && t.EmployerId == EmployerId && t.BusinessYear == BusinessYear).Select(r => r.CarrierId).Distinct().ToList();
                        var IssuerIds = DB.PlanAttributeMsts.Where(t => t.MrktCover == "Group" && t.BusinessYear == BusinessYear).Select(r => r.CarrierId).Distinct().ToList();

                        lstIssuer = DB.IssuerMsts.Where(r => IssuerIds.Contains(r.Id) && r.Status == true).ToList();
                    }

                    //var result = DB.IssuerMsts.ToList();
                    foreach (var item in lstIssuer)
                    {
                        oModellst.Add(new IssuerMst { Id = item.Id, IssuerCode = item.IssuerCode, IssuerName = item.IssuerName + " (" + item.IssuerCode + ")", Abbreviations = item.Abbreviations });
                    }

                    oCarrierViewModel.Carriers = oModellst;
                    if (oCarrierViewModel.Carriers.Count > 0)
                    {
                        oCarrierViewModel.Status = true;
                        oCarrierViewModel.Message = "Success";
                    }
                    else
                    {
                        oCarrierViewModel.Status = true;
                        oCarrierViewModel.Message = "Carrier does not exist.";
                    }
                    return oCarrierViewModel;
                }

                catch (Exception ex)
                {
                    oCarrierViewModel.Status = false;
                    oCarrierViewModel.Message = ex.Message;

                    string ExceptionString = "Api : Carrier" + Environment.NewLine;
                    ExceptionString += "Request :  " + " EmployerId " + EmployerId + " ,ZipCode " + ZipCode + " ,CountyName " + CountyName + " ,InsuranceTypeId " + InsuranceTypeId + " ,BusinessYear " + BusinessYear + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oCarrierViewModel) + Environment.NewLine;
                    var fileName = "Carrier - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);
                    return oCarrierViewModel;
                }
            }
        }

        /// <summary>
        /// This api is used to get all plan types
        /// </summary>
        /// <returns>List of plan types</returns>
        [ActionName("Plan")]
        [HttpGet]
        [Route("api/case/plan")]
        [Authorize]
        public PlanViewModel Plan()
        {
            PlanViewModel oPlanViewModel = new PlanViewModel();
            List<PlanMaster> oModellst = new List<PlanMaster>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    var result = DB.PlanMasters.OrderBy(x => x.PlanType).ToList();
                    foreach (var item in result)
                    {
                        oModellst.Add(new PlanMaster { PlanID = item.PlanID, PlanType = item.PlanType });
                    }
                    oPlanViewModel.Plans = oModellst;
                    if (oPlanViewModel.Plans.Count > 0)
                    {
                        oPlanViewModel.Status = true;
                        oPlanViewModel.Message = "Success";
                    }
                    else
                    {
                        oPlanViewModel.Status = true;
                        oPlanViewModel.Message = "Plan type cannot be found.";
                    }
                    return oPlanViewModel;
                }

                catch (Exception ex)
                {
                    oPlanViewModel.Status = false;
                    oPlanViewModel.Message = ex.Message;

                    string ExceptionString = "Api : Plan" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oPlanViewModel) + Environment.NewLine;
                    var fileName = "Plan - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);
                    return oPlanViewModel;
                }
            }
        }

        /// <summary>
        /// This api is used to get all Usages Code
        /// </summary>
        /// <returns>List of Usages Code</returns>
        [ActionName("UsageCode")]
        [HttpGet]
        [Route("api/case/usagecode")]
        [Authorize]
        public UsageCodeViewModel UsageCode()
        {
            UsageCodeViewModel oUsageCodeViewModel = new UsageCodeViewModel();
            List<UsageCodeMaster> oModellst = new List<UsageCodeMaster>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    var result = DB.UsageCodeMasters.ToList();
                    foreach (var item in result)
                    {
                        oModellst.Add(new UsageCodeMaster { UsagaId = item.UsagaId, UsageType = item.UsageType });
                    }

                    oUsageCodeViewModel.Usagecodes = oModellst;
                    if (oUsageCodeViewModel.Usagecodes.Count > 0)
                    {
                        oUsageCodeViewModel.Status = true;
                        oUsageCodeViewModel.Message = "Success";
                    }
                    else
                    {
                        oUsageCodeViewModel.Status = true;
                        oUsageCodeViewModel.Message = "Usage Code does not exist.";
                    }
                    return oUsageCodeViewModel;
                }

                catch (Exception ex)
                {
                    oUsageCodeViewModel.Status = false;
                    oUsageCodeViewModel.Message = ex.Message;

                    string ExceptionString = "Api : UsageCode" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oUsageCodeViewModel) + Environment.NewLine;
                    var fileName = "UsageCode - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);
                    return oUsageCodeViewModel;
                }
            }
        }

        /// <summary>
        /// This api is used to get Insurance Type based on Employer and Year
        /// </summary>
        /// <param name="EmployerId">Employer id</param>
        /// <param name="Year">Year</param>
        /// <returns>List of Insurance Type</returns>
        [ActionName("InsuranceType")]
        [HttpGet]
        [Route("api/case/InsuranceType/{EmployerId}/{Year}")]
        [Authorize]
        public InsuranceTypeViewModel InsuranceType(int EmployerId, string Year)
        {
            InsuranceTypeViewModel oInsuranceTypeViewModel = new InsuranceTypeViewModel();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    //Vaibhav EmployerId
                    //var result = DB.PlanAttributeMsts.Where(r => r.EmployerId == EmployerId && r.BusinessYear == Year).Select(t => new { t.InsuranceType1.InsuranceType1, t.InsuranceType1.InsuranceTypeId }).Distinct().ToList();
                    var result = DB.InsuranceTypes.Select(t => new { t.InsuranceType1, t.InsuranceTypeId }).ToList();
                    oInsuranceTypeViewModel.InsuranceTypes = result;
                    if (oInsuranceTypeViewModel.InsuranceTypes.Count > 0)
                    {
                        oInsuranceTypeViewModel.Status = true;
                        oInsuranceTypeViewModel.Message = "Success";
                    }
                    else
                    {
                        oInsuranceTypeViewModel.Status = true;
                        oInsuranceTypeViewModel.Message = "Insurance Types does not exist.";
                    }
                    return oInsuranceTypeViewModel;
                }

                catch (Exception ex)
                {
                    oInsuranceTypeViewModel.Status = false;
                    oInsuranceTypeViewModel.Message = ex.Message;

                    string ExceptionString = "Api : InsuranceType" + Environment.NewLine;
                    ExceptionString += "Request :  " + " EmployerId " + EmployerId + " ,Year " + Year + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oInsuranceTypeViewModel) + Environment.NewLine;
                    var fileName = "InsuranceType - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);
                    return oInsuranceTypeViewModel;
                }
            }
        }

        #region NewChanges(17-Feb)

        /// <summary>
        /// Get all Medical Usages based on Rating area and state
        /// </summary>
        /// <param name="RatingAreaId">Rating Area Id</param>
        /// <param name="StateCode">State Code</param>
        /// <returns>list of medical usages</returns>
        [ActionName("MedicalUsage")]
        [HttpGet]
        [Route("api/case/medicalusage/{ZipCode}/{CountyName}/{EmployerId}/{StateCode}")]
        //[Authorize]
        public MedicalUsageViewModel MedicalUsage(string ZipCode, string CountyName, long EmployerId, string StateCode)
        {
            MedicalUsageViewModel oMedicalUsageViewModel = new MedicalUsageViewModel();
            List<MedicalUsagelistViewModel> oModellst = new List<MedicalUsagelistViewModel>();
            long RatingAreaId;

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    if (EmployerId == MHMBLL.Constants.DefaultEmployerID)
                    {
                        //&& r.Businessyear == System.DateTime.Now.Year.ToString()
                        RatingAreaId = (long)DB.qryZipCodeToRatingAreas.Where(r => r.Zip == ZipCode && r.CountyName == CountyName && (r.MarketCoverage == "Both" || r.MarketCoverage == "Indi")).FirstOrDefault().RatingAreaID;
                    }
                    else
                    {
                        //
                        RatingAreaId = (long)DB.qryZipCodeToRatingAreas.Where(r => r.Zip == ZipCode && r.CountyName == CountyName && (r.MarketCoverage == "Both" || r.MarketCoverage == "Group")).FirstOrDefault().RatingAreaID;
                    }

                    DataTable dtusage = new DataTable();
                    objCon.Open();
                    cmd = new SqlCommand("get_Medicalusage_List", objCon);
                    cmd.Parameters.AddWithValue("@RatingAreaID", RatingAreaId);
                    cmd.Parameters.AddWithValue("@StateCode", StateCode);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtusage);

                    foreach (DataRow dr in dtusage.Rows)
                    {
                        oModellst.Add(new MedicalUsagelistViewModel
                        {
                            CategoryId = Convert.ToInt16(dr["CategoryId"]),
                            CategoryName = dr["CategoryName"].ToString(),
                            MHMBenefitID = Convert.ToInt16(dr["MHMBenefitID"]),
                            MHMBenefitName = dr["MHMBenefitName"].ToString(),
                            IsDefault = Convert.ToBoolean(dr["IsDefault"]),
                            MHMBenefitCost = Convert.ToDecimal(dr["MHMBenefitCost"])
                        });
                    }

                    oMedicalUsageViewModel.MedicalsUsage = oModellst;
                    oMedicalUsageViewModel.RatingAreaId = RatingAreaId;
                    if (oMedicalUsageViewModel.MedicalsUsage.Count > 0)
                    {
                        oMedicalUsageViewModel.Status = true;
                        oMedicalUsageViewModel.Message = "Success";
                    }
                    else
                    {
                        oMedicalUsageViewModel.Status = false;
                        oMedicalUsageViewModel.Message = "This benefit does not exist for this Rating Area and State.";
                    }
                    return oMedicalUsageViewModel;
                }

                catch (Exception ex)
                {
                    oMedicalUsageViewModel.Status = false;
                    oMedicalUsageViewModel.Message = ex.Message;

                    string ExceptionString = "Api : MedicalUsage" + Environment.NewLine;
                    ExceptionString += "Request :  " + " ZipCode " + ZipCode + " ,CountyName " + CountyName + " ,EmployerId " + EmployerId + " ,StateCode " + StateCode + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oMedicalUsageViewModel) + Environment.NewLine;
                    var fileName = "MedicalUsage - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);
                    return oMedicalUsageViewModel;
                }
                finally
                {
                    if (cmd != null) cmd.Dispose();
                    if (objCon != null) objCon.Close();
                }
            }
        }

        #endregion

        /// <summary>
        /// This is api is used to get Critical illness list
        /// </summary>
        /// <returns>list of Critical illness</returns>
        [ActionName("GetCriticalillness")]
        [HttpGet]
        [Route("api/case/GetCriticalillness")]
        [Authorize]
        public HttpResponseMessage GetCriticalillness()
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    var Criticalillnesslst = DB.CriticalillnessMsts.Select(r => new { r.IllnessId, r.IllnessName });
                    if (Criticalillnesslst.Count() > 0)
                    {
                        Dictionary<string, object> res = new Dictionary<string, object>();

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("CriticalillnessList", Criticalillnesslst.ToList());
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;

                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Critical illness does not exist.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }

                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetCriticalillness" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetCriticalillness - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This is api is used to get county list
        /// </summary>
        /// <param name="zipcode">zipcode</param>
        /// <returns>list of county</returns>
        [ActionName("GetCounty")]
        [HttpGet]
        [Route("api/case/getcounty/{zipcode}")]
        //[Authorize]
        public HttpResponseMessage GetCounty(string zipcode)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    var Countylst = DB.qryZipCodeToRatingAreas.Where(m => m.Zip == zipcode).Select(r => new { r.CountyName, r.RatingAreaID, r.StateCode, r.StateId, r.StateName, r.City });

                    //var Countylst = DB.qryZipCodeToRatingAreas.Where(m => m.Zip == zipcode && m.Businessyear == System.DateTime.Now.Year.ToString()).Select(r => new { r.CountyName, r.RatingAreaID, r.StateCode, r.StateId, r.StateName, r.City });
                    if (Countylst.Count() > 0)
                    {
                        Dictionary<string, object> res = new Dictionary<string, object>();

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("CountyList", Countylst.ToList());
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;

                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Zipcode does not exist.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetCounty" + Environment.NewLine;
                    ExceptionString += "Request :  " + " zipcode " + zipcode + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetCounty - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This is api is used to get case status master
        /// </summary>
        /// <returns>list of county</returns>
        [HttpGet]
        [Route("api/case/GetCaseStatusMst")]
        [Authorize]
        public HttpResponseMessage GetCaseStatusMst()
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    var CaseStatusMstList = DB.CaseStatusMsts.Where(r => r.IsActive == true).OrderBy(r => r.Sortby).Select(r => new { r.StatusId, r.StatusCode, r.Descr, r.Editable, r.Parent });
                    if (CaseStatusMstList.Count() > 0)
                    {
                        Dictionary<string, object> res = new Dictionary<string, object>();

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("CaseStatusMst", CaseStatusMstList.ToList());
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;

                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Case Status does not exist.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetCaseStatusMst" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetCaseStatusMst - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to get all benefit categories
        /// </summary>
        /// <returns>List of benefit categories</returns>
        [ActionName("GetUsageCategory")]
        [HttpGet]
        [Route("api/case/getusagecategory")]
        [Authorize]
        public CategoryViewModel GetUsageCategory()
        {
            CategoryViewModel response = new CategoryViewModel();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    var result = DB.CategoryMsts.OrderBy(x => x.CategoryName).ToList();
                    if (result.Count() > 0)
                    {
                        response.Message = "Record Found";
                        response.Status = true;
                        response.CategoryMst = result;
                    }
                    else
                    {
                        response.Message = "Benefit Category cannot be found.";
                        response.Status = true;
                    }
                    return response;
                }

                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Status = false;

                    string ExceptionString = "Api : GetUsageCategory" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(response) + Environment.NewLine;
                    var fileName = "GetUsageCategory - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);
                    return response;
                }
            }
        }


        /// <summary>
        /// This api used is used to send pdf on Customer email
        /// </summary>
        /// <returns>Status</returns>
        [HttpPost, Route("api/case/upload")]
        public async Task<HttpResponseMessage> Upload()
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    HttpContent requestContent = Request.Content;
                    var Content = await requestContent.ReadAsStringAsync();

                    var JsonData = JsonConvert.DeserializeObject<JsonResponse>(Content);
                    byte[] data = Convert.FromBase64String(JsonData.Html);
                    string Html = System.Text.Encoding.UTF8.GetString(data);
                    long CaseId = Convert.ToInt64(JsonData.CaseId);

                    #region Generating PDF

                    // read parameters from the webpage
                    string htmlString = Html;
                    string baseUrl = "app.myhealthmath.com";

                    string pdf_page_size = "A4";
                    SelectPdf.PdfPageSize pageSize = (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize),
                        pdf_page_size, true);

                    string pdf_orientation = "Portrait";
                    SelectPdf.PdfPageOrientation pdfOrientation =
                        (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                        pdf_orientation, true);

                    int webPageWidth = 1024;


                    int webPageHeight = 0;

                    // instantiate a html to pdf converter object
                    HtmlToPdf converter = new HtmlToPdf();

                    // set converter options
                    converter.Options.PdfPageSize = pageSize;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;
                    //converter.Options.EmbedFonts = false;

                    // create a new pdf document converting an url
                    PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

                    // save pdf document
                    byte[] outPdfBuffer = doc.Save();


                    //doc.Save("test.pdf");

                    // close pdf document
                    doc.Close();

                    #endregion

                    string Mail = "";

                    if (string.IsNullOrEmpty(JsonData.JobNumber))
                    {
                        Mail = "Dear " + JsonData.ApplicantName + "&nbsp;-" + Environment.NewLine + " <br/>" + Environment.NewLine + " <br/>";
                        Mail = Mail + "Attached is your MyHealthMath report.";
                        Mail = Mail + " We've done the math behind the amount you will pay in premiums (gold color), ";
                        Mail = Mail + "as well as an estimate of your share of your medical expenses (green color)." + Environment.NewLine + " <br/>" + Environment.NewLine + " <br/>";
                        Mail = Mail + "The graph shows the totals, and the table below shows the breakdown behind the totals; ";
                        Mail = Mail + "color coding ties them together." + Environment.NewLine + " <br/>" + Environment.NewLine + " <br/>";

                        Mail = Mail + "We hope that you find these calculations helpful. Please let us know if you have any questions." + Environment.NewLine + " <br/>" + Environment.NewLine + " <br/>";

                        Mail = Mail + "Best regards," + Environment.NewLine + " <br/>" + Environment.NewLine + " <br/>";

                        Mail = Mail + JsonData.AgentName + Environment.NewLine + " <br/>";
                        Mail = Mail + "MyHealthMath Analyst" + Environment.NewLine + " <br/>";
                        Mail = Mail + JsonData.AgentEmail + Environment.NewLine + " <br/>";
                        Mail = Mail + JsonData.AgentPhone;

                        Service.SendMailWithAttaitchment(JsonData.ApplicantEmail, JsonData.AgentEmail, "reports@myhealthmath.com", "Case : " + JsonData.CaseTitle, Mail, outPdfBuffer, JsonData.CaseTitle + ".pdf");
                    }
                    else
                    {
                        var MailDetails = DB.JobMasters.Where(r => r.JobNumber == JsonData.JobNumber).Select(t => new { t.EmailSubjText, t.EmailBodyText, t.EmailSignText }).First();
                        string EmailSubjText = MailDetails.EmailSubjText.ToString();
                        string EmailBodyText = MailDetails.EmailBodyText.ToString();
                        string EmailSignText = MailDetails.EmailSignText.ToString();
                        if (EmailSubjText.Contains("##CaseTitle##")) { EmailSubjText = EmailSubjText.Replace("##CaseTitle##", JsonData.CaseTitle); }
                        if (EmailBodyText.Contains("##ApplicantName##")) { EmailBodyText = EmailBodyText.Replace("##ApplicantName##", JsonData.ApplicantName.ToString()); }
                        if (EmailSignText.Contains("##AgentName##")) { EmailSignText = EmailSignText.Replace("##AgentName##", JsonData.AgentName); }
                        if (EmailSignText.Contains("##AgentEmail##")) { EmailSignText = EmailSignText.Replace("##AgentEmail##", JsonData.AgentEmail); }
                        if (EmailSignText.Contains("##AgentPhone##")) { EmailSignText = EmailSignText.Replace("##AgentPhone##", String.Format("{0:(###) ###-####}", Convert.ToInt64(JsonData.AgentPhone))); }
                        if (EmailBodyText.Contains("##PlanTotalCostRange##")) { EmailBodyText = EmailBodyText.Replace("##PlanTotalCostRange##", "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.PlanTotalCostRange))); }
                        if (EmailBodyText.Contains("##TotalEmployerContribution##"))
                        {

                            var str = "";
                            if (Convert.ToDecimal(JsonData.EmployerHSAContribution) != 0 && Convert.ToDecimal(JsonData.EmployerHRAReimbursement) > 0)
                            {
                                str = "If you enroll in the " + JsonData.OptimalPlanName + " (your optimal plan), your employer will contribute approximately " + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.TotalEmployerContribution)) + " to your expenses. This contribution consists of:" + Environment.NewLine;
                                str += "<ul style=" + "'margin-bottom:-10px;'" + "><li>" + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.EmployerPremiumContribution)) + " to your premium,</li>";
                                str += "<li>" + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.EmployerHSAContribution)) + " to your health savings account, and</li>";
                                str += "<li>an estimated " + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.EmployerHRAReimbursement)) + " in HRA reimbursements</li></ul>";
                            }
                            else if (Convert.ToDecimal(JsonData.EmployerHSAContribution) != 0 && Convert.ToDecimal(JsonData.EmployerHRAReimbursement) == 0)
                            {
                                str = "If you enroll in the " + JsonData.OptimalPlanName + " (your optimal plan), your employer will contribute approximately " + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.TotalEmployerContribution)) + " to your expenses. This contribution consists of:" + Environment.NewLine;
                                str += "<ul style=" + "'margin-bottom:-10px;'" + "><li>" + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.EmployerPremiumContribution)) + " to your premium</li>";
                                str += "<li>" + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.EmployerHSAContribution)) + " to your health savings account</li></ul>";
                            }
                            else if (Convert.ToDecimal(JsonData.EmployerHSAContribution) == 0 && Convert.ToDecimal(JsonData.EmployerHRAReimbursement) > 0)
                            {
                                str = "If you enroll in the " + JsonData.OptimalPlanName + " (your optimal plan), your employer will contribute approximately " + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.TotalEmployerContribution)) + " to your expenses. This contribution consists of:" + Environment.NewLine;
                                str += "<ul style=" + "'margin-bottom:-10px;'" + "><li>" + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.EmployerPremiumContribution)) + " to your premium</li>";
                                str += "<li>an estimated " + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.EmployerHRAReimbursement)) + " in HRA reimbursements</li></ul>";
                            }
                            else if (Convert.ToDecimal(JsonData.EmployerHSAContribution) == 0 && Convert.ToDecimal(JsonData.EmployerHRAReimbursement) == 0)
                            {
                                str = "If you enroll in the " + JsonData.OptimalPlanName + " (your optimal plan), your employer will contribute approximately " + "$" + String.Format("{0:n0}", Convert.ToDecimal(JsonData.TotalEmployerContribution)) + " to your premium.<br><br>";
                            }
                            EmailBodyText = EmailBodyText.Replace("##TotalEmployerContribution##", str);
                        }

                        Mail = EmailBodyText + EmailSignText;

                        Service.SendMailWithAttaitchment(JsonData.ApplicantEmail, JsonData.AgentEmail, "reports@myhealthmath.com", EmailSubjText, Mail, outPdfBuffer, JsonData.CaseTitle + ".pdf");
                    }

                    var oCase = DB.Cases.Where(r => r.CaseID == CaseId).FirstOrDefault();

                    if (oCase.StatusId == 6 || oCase.StatusId == 8)
                    {
                        oCase.StatusId = 8;
                    }
                    else
                    {
                        oCase.StatusId = 2;
                    }
                    //    oCase.StatusId = 2;
                    DB.SaveChanges();

                    oResponse.Status = true;
                    oResponse.Message = "Success";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }

                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : Upload" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "Upload - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to download pdf
        /// </summary>
        /// <returns>PDF</returns>
        [HttpPost]
        [Route("api/case/pdfgeneration")]
        public HttpResponseMessage PdfGenerate()
        {
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    Response oResponse = new Response();

                    HttpContent requestContent = Request.Content;
                    var Content = requestContent.ReadAsStringAsync();

                    var JsonData = JsonConvert.DeserializeObject<JsonResponse>(Content.Result);
                    byte[] data = Convert.FromBase64String(JsonData.Html);
                    string Html = System.Text.Encoding.UTF8.GetString(data);

                    // read parameters from the webpage
                    string htmlString = Html;
                    string baseUrl = "app.myhealthmath.com";

                    string pdf_page_size = "A4";
                    SelectPdf.PdfPageSize pageSize = (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize),
                        pdf_page_size, true);

                    string pdf_orientation = "Portrait";
                    SelectPdf.PdfPageOrientation pdfOrientation =
                        (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                        pdf_orientation, true);

                    int webPageWidth = 1024;

                    int webPageHeight = 0;

                    // instantiate a html to pdf converter object
                    HtmlToPdf converter = new HtmlToPdf();
                    //SelectPdf.
                    // set converter options
                    converter.Options.PdfPageSize = pageSize;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;

                    // create a new pdf document converting an url
                    PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

                    //var fontPath1 = HttpContext.Current.Server.MapPath("~/fonts") + "\\hind-guntur-v3-latin-regular.ttf";
                    //doc.AddFont(fontPath1);
                    //var fontPath2 = HttpContext.Current.Server.MapPath("~/fonts") + "\\hind-guntur-v3-latin-300.ttf";
                    //doc.AddFont(fontPath2);
                    //var fontPath3 = HttpContext.Current.Server.MapPath("~/fonts") + "\\hind-guntur-v3-latin-500.ttf";
                    //doc.AddFont(fontPath3);
                    //var fontPath4 = HttpContext.Current.Server.MapPath("~/fonts") + "\\hind-guntur-v3-latin-600.ttf";
                    //doc.AddFont(fontPath4);
                    //PdfFont font = doc.AddFont(PdfStandardFont.Helvetica);
                    //font.Size = 24;
                    //SelectPdf.PdfTextSection obj = new PdfTextSection(1, 2, "Hello Worlds,", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif,12.0F, System.Drawing.FontStyle.Bold));
                    //converter.Footer.Add(obj);
                    //doc.Fonts.Add();

                    // save pdf document
                    byte[] outPdfBuffer = doc.Save();

                    // close pdf document
                    doc.Close();

                    //string htmlToConvert = Html;

                    //// Create a HTML to PDF converter object with default settings
                    //HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

                    //// Set license key received after purchase to use the converter in licensed mode
                    //// Leave it not set to use the converter in demo mode
                    //htmlToPdfConverter.LicenseKey = "fvDh8eDx4fHg4P/h8eLg/+Dj/+jo6Og=";

                    //// Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
                    //// Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
                    //htmlToPdfConverter.ConversionDelay = 2;
                    //htmlToPdfConverter.PdfFooterOptions.FooterHeight = 0;

                    //// Convert the HTML string to a PDF document in a memory buffer
                    //byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlToConvert, "http://mhm.synsoftglobal.net/");

                    System.IO.MemoryStream memStream = new System.IO.MemoryStream(outPdfBuffer);


                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StreamContent(memStream);
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = JsonData.CaseTitle + ".pdf";
                    result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    result.Content.Headers.ContentLength = memStream.Length;
                    return result;
                }

                catch (Exception ex)
                {
                    string ExceptionString = "Api : PdfGenerate" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(ex.Message) + Environment.NewLine;
                    var fileName = "PdfGenerate - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    return result;
                }
            }
        }

        /// <summary>
        /// This api is used to check Case Title already exist or not
        /// </summary>
        /// <param name="CaseTitle">Case Title</param>
        /// <returns>Status</returns>
        [HttpGet]
        [Route("api/case/CheckCaseTitle/{CaseTitle}/{CaseId}")]
        public HttpResponseMessage CheckCaseTitle(string CaseTitle, long? CaseId)
        {
            Response oRes = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    var lstCases = DB.Cases.Select(r => new { r.CaseID, r.CaseTitle }).Where(t => t.CaseTitle != "" && t.CaseTitle != null).ToList();
                    //lstCases.ForEach(
                    //    r => r.CaseTitle = r.CaseTitle.Decrypt()
                    //    );

                    bool CaseTitleStatus;

                    if (CaseId != null && CaseId != 0)
                    {
                        CaseTitleStatus = lstCases.Any(r => r.CaseTitle.ToLower() == CaseTitle.ToLower() && r.CaseID != CaseId);

                        Dictionary<string, object> res = new Dictionary<string, object>();
                        res.Add("Status", true);
                        res.Add("Message", "Success");
                        res.Add("CaseTitleStatus", CaseTitleStatus);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        CaseTitleStatus = lstCases.Any(r => r.CaseTitle.ToLower() == CaseTitle.ToLower());
                        Dictionary<string, object> res = new Dictionary<string, object>();
                        res.Add("Status", true);
                        res.Add("Message", "Success");
                        res.Add("CaseTitleStatus", CaseTitleStatus);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }

                catch (Exception ex)
                {
                    oRes.Status = false;
                    oRes.Message = ex.Message;

                    string ExceptionString = "Api : CheckCaseTitle" + Environment.NewLine;
                    ExceptionString += "Request :  " + " CaseTitle " + CaseTitle + " ,CaseId " + CaseId + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(ex.Message) + Environment.NewLine;
                    var fileName = "CheckCaseTitle - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oRes);
                    return response;
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("api/case/GetHSALimit/{MaxMemberAge}/{UsagesCode}/{BusinessYear}")]
        public HttpResponseMessage GetHSALimit(int MaxMemberAge, int UsagesCode, string BusinessYear)
        {
            Response oRes = new Response();
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    decimal HSALimit = 0, HSAPercentage = 0;
                    var MaxEEHSA = new HSACalculation().CalculateAnnualHSA(UsagesCode, MaxMemberAge, 0, BusinessYear, out HSALimit);

                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("Status", true);
                    res.Add("Message", "Success");
                    res.Add("HSALimit", HSALimit);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }

                catch (Exception ex)
                {
                    oRes.Status = false;
                    oRes.Message = ex.Message;

                    string ExceptionString = "Api : GetHSALimit" + Environment.NewLine;
                    ExceptionString += "Request :  " + " MaxMemberAge " + MaxMemberAge + " ,UsagesCode " + UsagesCode + " ,BusinessYear " + BusinessYear + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(ex.Message) + Environment.NewLine;
                    var fileName = "GetHSALimit - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oRes);
                    return response;
                    throw;
                }
            }
        }

        /// <summary>
        /// This api is used to get all cases
        /// </summary>
        /// <param name="lstParameter">List of parameters</param>
        /// <param name="EmailId">Email Id</param>
        /// <param name="searchby">Search by value</param>
        /// <param name="sortby">Sory by</param>
        /// <param name="desc">Ascending or Descending</param>
        /// <param name="page">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize]
        [Route("api/case/GetCaseReport")]
        public async Task<HttpResponseMessage> GetCaseReport([FromUri]  List<string> lstParameter, string EmailId, string searchby = null, string sortby = null, bool desc = true, DateTime? startDate = null, DateTime? endDate = null)  //
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                //to get current user id
                var UserId = User.Identity.GetUserId();
                var UserRole = UserManager.GetRoles(UserId).First();
                string UserEmail = UserManager.GetEmail(UserId);

                objCon.Open();
                cmd = new SqlCommand("", objCon);
                XLWorkbook xlWorkbook = new XLWorkbook();

                try
                {
                    cmd.Parameters.AddWithValue("@UserEmail", UserEmail);
                    cmd.Parameters.AddWithValue("@UserRole", UserRole);
                    //cmd.Parameters.AddWithValue("@UserEmail", "vchaurasiya4@gmail.com");
                    //cmd.Parameters.AddWithValue("@UserRole", "Admin");
                    if (!string.IsNullOrEmpty(searchby))
                    {
                        cmd.Parameters.AddWithValue("@CaseId", searchby.Trim());
                    }
                    if (startDate != null)
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                    }
                    if (endDate != null)
                    {
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                    }

                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        cmd.Parameters.AddWithValue("@EmployerCompany", lstParameter[0].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]) && string.IsNullOrEmpty(searchby))
                    {
                        cmd.Parameters.AddWithValue("@CaseTitle", lstParameter[1].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[2]))
                    {
                        cmd.Parameters.AddWithValue("@CreatedBy", lstParameter[2].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[3]))
                    {
                        cmd.Parameters.AddWithValue("@MobileNo", lstParameter[3].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[4]))
                    {
                        cmd.Parameters.AddWithValue("@JobNumber", lstParameter[4].ToLower());
                    }
                    if (!string.IsNullOrEmpty(lstParameter[5]))
                    {
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(lstParameter[5].ToString(), (typeof(DataTable)));

                        SqlParameter param = new SqlParameter("@CaseStatusIds", SqlDbType.Structured)
                        {
                            TypeName = "dbo.tblCaseStatus",
                            Value = dt
                        };
                        cmd.Parameters.Add(param);
                    }
                    if (!string.IsNullOrEmpty(lstParameter[6]))
                    {
                        cmd.Parameters.AddWithValue("@BusinessYear", lstParameter[6].ToString().Trim());
                    }

                    if (desc)
                    {
                        cmd.Parameters.AddWithValue("@SortOrder", "DESC");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SortOrder", "ASC");
                    }

                    switch (sortby)
                    {
                        case "Name":
                            cmd.Parameters.AddWithValue("@SortColumn", "ApplicantName");

                            break;

                        case "Carrier":
                            cmd.Parameters.AddWithValue("@SortColumn", "Carrier");
                            break;

                        case "CreateDate":
                            cmd.Parameters.AddWithValue("@SortColumn", "CreatedDateTime");
                            break;

                        case "CaseTitle":
                            cmd.Parameters.AddWithValue("@SortColumn", "CaseTitle");
                            break;

                        case "CaseID":
                            cmd.Parameters.AddWithValue("@SortColumn", "CaseID");
                            break;

                        default:
                            cmd.Parameters.AddWithValue("@SortColumn", "CreatedDateTime");
                            break;
                    }

                    SqlCommand objcmd1 = cmd;
                    objcmd1.CommandText = "get_AllApplicantsExcel_List";
                    DataTable dtApplicants = await GetDatatable(objcmd1);
                    dtApplicants.TableName = "Applicants";

                    if (dtApplicants.Rows.Count > 0)
                    {
                        xlWorkbook.Worksheets.Add(dtApplicants);
                        dtApplicants.Dispose();
                        objcmd1.Dispose();

                        SqlCommand objcmd2 = cmd;
                        objcmd2.CommandText = "get_AllCasesExcel_List";
                        DataTable dtCases = await GetDatatable(objcmd2);
                        dtCases.TableName = "Cases";
                        xlWorkbook.Worksheets.Add(dtCases);
                        dtCases.Dispose();
                        objcmd2.Dispose();

                        SqlCommand objcmd3 = cmd;
                        objcmd3.CommandText = "get_AllFamilysExcel_List";
                        DataTable dtFamilys = await GetDatatable(objcmd3);
                        dtFamilys.TableName = "Family";
                        xlWorkbook.Worksheets.Add(dtFamilys);
                        dtFamilys.Dispose();
                        objcmd3.Dispose();

                        SqlCommand objcmd4 = cmd;
                        objcmd4.CommandText = "get_AllBenefitUserDetailsExcel_List";
                        DataTable dtBenefitUserDetails = await GetDatatable(objcmd4);
                        dtBenefitUserDetails.TableName = "BenefitUserDetails";
                        xlWorkbook.Worksheets.Add(dtBenefitUserDetails);
                        dtBenefitUserDetails.Dispose();
                        objcmd4.Dispose();

                        SqlCommand objcmd5 = cmd;
                        objcmd5.CommandText = "get_AllCriticalillnessExcel_List";
                        DataTable dtCriticalillness = await GetDatatable(objcmd5);
                        dtCriticalillness.TableName = "CriticalIllness";
                        xlWorkbook.Worksheets.Add(dtCriticalillness);
                        dtCriticalillness.Dispose();
                        objcmd5.Dispose();

                        SqlCommand objcmd6 = cmd;
                        objcmd6.CommandText = "get_AllCasePlanResultsExcel_List";
                        DataTable dtCasePlanResults = await GetDatatable(objcmd6);
                        dtCasePlanResults.TableName = "CasePlanResults";
                        xlWorkbook.Worksheets.Add(dtCasePlanResults);
                        dtCasePlanResults.Dispose();
                        objcmd6.Dispose();

                        MemoryStream stream = new MemoryStream();
                        Stream fs = new MemoryStream();

                        xlWorkbook.SaveAs(fs);
                        fs.Position = 0;
                        fs.CopyTo(stream);

                        byte[] bytes = stream.ToArray();

                        System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes);

                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StreamContent(memStream);
                        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = "Report.xlsx";
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/ms-excel");
                        response.Content.Headers.ContentLength = memStream.Length;
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Case does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {

                    oResponse.Status = false;
                    oResponse.Message = ex.ToString();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;

                }
                finally
                {
                    xlWorkbook.Dispose();
                    xlWorkbook = null;
                    cmd.Dispose();
                    objCon.Close();
                }
            }
        }

        [NonAction]
        public Task<DataTable> GetDatatable(SqlCommand cmd)
        {
            DataTable dt = new DataTable();

            try
            {
                return Task.Run(() =>
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    cmd.Dispose();
                    return dt;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Reading Data from Database"); ;
            }
            finally
            {
                dt.Dispose();
            }
        }

        //[HttpGet]
        //[Route("api/case/ImportData")]
        //public HttpResponseMessage ImportExcelData()
        //{
        //    DB = new MHMDal.Models.MHM();
        //    ImportExcel obj = new ImportExcel();
        //    DataSet ds = obj.ImportExcel1();
        //    //   foreach (DataRow row in ds.Tables["Case"].Rows[0])
        //    //  {
        //    DataRow row = ds.Tables["Case"].Rows[0];
        //    int CaseNumber = Convert.ToInt32(row["CaseNumber"]);
        //    DataRow Applicant = ds.Tables["Applicant"].Select("CaseNumber = " + CaseNumber).First();
        //    DataRow[] Families = ds.Tables["Families"].Select("CaseNumber = " + CaseNumber);

        //    Case oCase = new Case();

        //    var EmployerName = Applicant["EmployerId"].ToString();

        //    using (var dbContextTransaction = DB.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            Applicant oApplicant = new Applicant();
        //            oApplicant.EmployerId = DB.EmployerMsts.Where(r => r.EmployerName == EmployerName).Select(t => t.EmployerId).First();
        //            oApplicant.FirstName = Applicant["FirstName/LastName"].ToString().Encrypt();
        //            oApplicant.LastName = "A".Encrypt();
        //            oApplicant.CurrentPlan = Applicant["CurrentPlan"].ToString();
        //            oApplicant.CurrentPremium = Applicant["CurrentPremium"].ToString();
        //            oApplicant.City = Applicant["City"].ToString().Encrypt();
        //            oApplicant.State = Applicant["State"].ToString().Encrypt();
        //            oApplicant.Street = Applicant["Street"].ToString().Encrypt();
        //            oApplicant.Zip = Applicant["Zip"].ToString().Encrypt();
        //            oApplicant.Email = Applicant["Email"].ToString().Encrypt();
        //            oApplicant.Mobile = Applicant["Mobile"].ToString().Encrypt();
        //            oApplicant.Createdby = 18;
        //            oApplicant.CreatedDateTime = DateTime.Now;

        //            DB.Applicants.Add(oApplicant);
        //            DB.SaveChanges();
        //            string zipCode = Applicant["Zip"].ToString();
        //            oCase.ApplicantID = oApplicant.ApplicantID;
        //            oCase.CaseTitle = "Case " + CaseNumber;
        //            oCase.Createdby = 18;
        //            oCase.CreatedDateTime = DateTime.Now;
        //            oCase.FPL = row["FPL"].ToString();
        //            oCase.HSAAmount = row["HSAAmount"].ToString();
        //            oCase.HSAFunding = row["HSAFunding"].ToString();
        //            oCase.HSALimit = row["HSALimit"].ToString();
        //            oCase.MAGIncome = row["MAGIncome"].ToString();
        //            oCase.MonthlySubsidy = row["MonthlySubsidy"].ToString();
        //            oCase.Notes = row["Notes"] != null ? row["Notes"].ToString().Encrypt() : null;
        //            oCase.PreviousYrHSA = false;
        //            oCase.TaxRate = row["TaxRate"].ToString();
        //            oCase.TotalMedicalUsage = row["TotalMedicalUsage"].ToString();
        //            oCase.UsageID = Convert.ToInt32(row["UsageID"]);
        //            oCase.Welness = false;
        //            oCase.Year = "2015";
        //            oCase.ZipCode = zipCode;
        //            oCase.CountyName = DB.qryZipStateCounties.Where(r => r.Zip == zipCode).Select(t => t.CountyName).First();
        //            oCase.RatingAreaId = DB.qryZipCodeToRatingAreas.Where(r => r.Zip == zipCode).Select(t => t.RatingAreaID).First();
        //            oCase.Applicant = null;
        //            oCase.Families = null;
        //            oCase.IsSubsidy = true;
        //            oCase.StatusId = 1;
        //            oCase.CaseSource = "Import from Excel";
        //            DB.Cases.Add(oCase);
        //            DB.SaveChanges();

        //            foreach (DataRow itemFM in Families)
        //            {
        //                int FamilyNumber = itemFM["IsPrimary"].ToString() == "Primary" ? 1 : Convert.ToInt32(itemFM["IsPrimary"]);

        //                DataRow[] BenefitUserDetails = ds.Tables["BenefitUseDetail"].Select("CaseNumber = " + CaseNumber + " and FamilyNumber=" + FamilyNumber);

        //                Family oFamily = new Family()
        //                {
        //                    CaseNumId = oCase.CaseID,
        //                    Gender = itemFM["Gender"].ToString().Encrypt(),
        //                    DOB = itemFM["DOB"].ToString().Encrypt(),
        //                    Age = DifferenceTotalYears(Convert.ToDateTime(itemFM["DOB"]), System.DateTime.Now).ToString(),
        //                    Createdby = 18,
        //                    CreatedDateTime = DateTime.Now,
        //                    IsPrimary = itemFM["IsPrimary"].ToString() == "Primary" ? true : false,
        //                    Smoking = itemFM["Smoking"].ToString() == "N" ? "false" : "true",
        //                    TotalMedicalUsage = BenefitUserDetails.Sum(r => Convert.ToDecimal(r["UsageCost"])).ToString(),
        //                };
        //                DB.Families.Add(oFamily);
        //                DB.SaveChanges();

        //                foreach (DataRow BenefitUserDetail in BenefitUserDetails)
        //                {
        //                    var BenefitName = BenefitUserDetail["BenefitName"].ToString();
        //                    DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                    BenefitUserDetail oBenenfitUses = new BenefitUserDetail()
        //                    {
        //                        UsageCost = BenefitUserDetail["UsageCost"].ToString(),
        //                        UsageQty = BenefitUserDetail["UsageQty"].ToString(),
        //                        MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID,
        //                        //MHMMappingBenefitId =  BenefitUserDetail["MHMMappingBenefitId"],
        //                        UsageNotes = BenefitNotes["UsageNotes"] != null ? BenefitNotes["UsageNotes"].ToString().Encrypt() : null,
        //                        FamilyID = oFamily.FamilyID,
        //                        Createdby = 18,
        //                        CreatedDateTime = DateTime.Now
        //                    };
        //                    DB.BenefitUserDetails.Add(oBenenfitUses);
        //                    DB.SaveChanges();
        //                }
        //            }

        //            DataRow[] CasePlanResults = ds.Tables["CasePlanResult"].Select("CaseNumber = " + CaseNumber);
        //            foreach (DataRow result in CasePlanResults)
        //            {
        //                CasePlanResult oCasePlanResult = new CasePlanResult()
        //                {
        //                    CaseId = oCase.CaseID,
        //                    GrossAnnualPremium = result["GrossAnnualPremium"].ToString().ToString(),
        //                    FederalSubsidy = result["FederalSubsidy"].ToString(),
        //                    NetAnnualPremium = result["NetAnnualPremium"].ToString(),
        //                    MonthlyPremium = result["MonthlyPremium"].ToString(),
        //                    Copays = result["Copays"].ToString(),
        //                    PaymentsToDeductibleLimit = result["PaymentsToDeductibleLimit"].ToString(),
        //                    CoinsuranceToOutOfPocketLimit = result["CoinsuranceToOutOfPocketLimit"].ToString(),
        //                    ContributedToYourHSAAccount = result["ContributedToYourHSAAccount"].ToString(),
        //                    TaxSavingFromHSAAccount = result["TaxSavingFromHSAAccount"].ToString(),
        //                    Medical = result["Medical"].ToString(),
        //                    TotalPaid = result["TotalPaid"].ToString(),
        //                    PaymentsByInsuranceCo = result["PaymentsByInsuranceCo"].ToString(),
        //                    DeductibleSingle = result["DeductibleSingle/DeductibleFamily"].ToString(),
        //                    DeductibleFamily = result["DeductibleSingle/DeductibleFamily"].ToString(),
        //                    OPLSingle = result["OPLSingle/OPLFamily"].ToString(),
        //                    OPLFamily = result["OPLSingle/OPLFamily"].ToString(),
        //                    Coinsurance = result["Coinsurance"].ToString(),
        //                    WorstCase = result["WorstCase"].ToString(),
        //                    CreatedDateTime = DateTime.Now
        //                };

        //                DB.CasePlanResults.Add(oCasePlanResult);
        //                DB.SaveChanges();
        //            }
        //            dbContextTransaction.Commit();
        //            return Request.CreateResponse(HttpStatusCode.OK, "Hello");
        //        }
        //        catch (Exception ex)
        //        {
        //            dbContextTransaction.Rollback();
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed");
        //        }
        //    }
        //}

        //[HttpGet]
        //[Route("api/case/ImportData1")]
        //public HttpResponseMessage ImportExcelData1()
        //{
        //    DB = new MHMDal.Models.MHM();
        //    //C:\Users\dev4\Desktop\MHMOldCases.xlsx
        //    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(@"C:\Users\dev4\Desktop\New Case & Plans\MHM_Group_Master_Imports-2016-10-11\MHM_Group_Master_Imports.xlsm", false))
        //    {
        //        var results = doc.WorkbookPart.Workbook.Sheets;
        //        foreach (Sheet item in results)
        //        {
        //            if (item.Name == "CaseImport") { ImportGroupSheet(doc, item); }
        //            // else if (item.Name == "Indiv") { ImportIndivdualSheet(doc, item); }
        //        }
        //        //Read the first Sheets from Excel file.
        //        //Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        [HttpGet]
        [Route("api/case/GetAgentNames")]
        public HttpResponseMessage GetAgentNames()
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var user = DB.AspNetUsers.OrderByDescending(r => r.Id);

                    Dictionary<string, object> res = new Dictionary<string, object>();
                    List<UsersViewModel> UserList = new List<UsersViewModel>();

                    foreach (var item in user)
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        User oUser = DB.Users.Where(m => m.Email == item.Email).FirstOrDefault();

                        if (oUser != null)
                        {
                            UserList.Add(new UsersViewModel
                            {
                                id = oUser.UserID,
                                Name = oUser.FirstName + " " + oUser.LastName,
                            });
                        }
                    }
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("AgentsList", UserList.OrderBy(x => x.Name).Distinct());
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetAgentNames" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetAgentNames - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Route("api/case/GetJobNumbers")]
        public HttpResponseMessage GetJobNumbers()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstJobMasters = DB.JobMasters.OrderByDescending(r => r.CreatedDateTime).ToList();

                    res.Add("Status", true);
                    res.Add("Message", "success");
                    res.Add("JobNumbers", lstJobMasters.Select(r =>
                    new
                    {
                        r.JobNumber,
                        r.JobDesc,
                        r.EmployerId
                    }));
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.ToString();

                    string ExceptionString = "Api : GetJobNumbers" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetJobNumbers - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }

        }

        [HttpGet]
        [Route("api/case/GetEmployerCompanies")]
        public HttpResponseMessage GetEmployerCompanies()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstEmployerCompanies = DB.EmployerMsts.ToList();

                    res.Add("Status", true);
                    res.Add("Message", "success");
                    res.Add("EmployerCompanies", lstEmployerCompanies.OrderBy(x => x.EmployerName).Select(r =>
                    new
                    {
                        r.EmployerId,
                        r.EmployerName
                    }));
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.ToString();

                    string ExceptionString = "Api : GetEmployerCompanies" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetEmployerCompanies - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }

        }

        public int DifferenceTotalYears(DateTime start, DateTime end)
        {
            // Get difference in total months.
            int months = ((end.Year - start.Year) * 12) + (end.Month - start.Month);

            // substract 1 month if end month is not completed
            if (end.Day < start.Day)
            {
                months--;
            }

            int totalyears = Convert.ToInt32(months / 12);
            return totalyears;
        }

        //private void ImportIndivdualSheet(SpreadsheetDocument doc, Sheet sheet)
        //{
        //    ImportExcel obj = new ImportExcel();

        //    //Get the Worksheet instance.
        //    Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

        //    //Fetch all the rows present in the Worksheet.
        //    IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
        //    List<CellFormats> formats = new List<CellFormats>(doc.WorkbookPart.WorkbookStylesPart.Stylesheet.Descendants<CellFormats>());

        //    var tempRow = rows.Skip(7).Take(1).FirstOrDefault();
        //    List<Cell> tempCells = ImportExcel.GetRowCells(tempRow).ToList();
        //    int c = tempCells.Count();

        //    using (var dbContextTransaction = DB.Database.BeginTransaction())
        //    {
        //        try
        //        {

        //            Applicant oApplicant = new Applicant();
        //            oApplicant.CreatedDateTime = Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 1));
        //            string EmployerName = obj.GetValue(tempCells, formats, doc, 2);
        //            if (string.IsNullOrEmpty(EmployerName)) { oApplicant.EmployerId = null; }
        //            else { oApplicant.EmployerId = DB.EmployerMsts.Where(r => r.EmployerName == EmployerName).Select(t => t.EmployerId).First(); }
        //            oApplicant.FirstName = obj.GetValue(tempCells, formats, doc, 3).Split(' ').First().Encrypt();
        //            oApplicant.LastName = obj.GetValue(tempCells, formats, doc, 3).Split(' ').Last().Encrypt();
        //            oApplicant.CurrentPlan = obj.GetValue(tempCells, formats, doc, 5);
        //            oApplicant.CurrentPremium = obj.GetValue(tempCells, formats, doc, 6);
        //            oApplicant.Street = obj.GetValue(tempCells, formats, doc, 7).Encrypt();
        //            oApplicant.City = obj.GetValue(tempCells, formats, doc, 8).Encrypt();
        //            oApplicant.State = obj.GetValue(tempCells, formats, doc, 9).Encrypt();
        //            oApplicant.Zip = ("0" + obj.GetValue(tempCells, formats, doc, 10)).Encrypt();
        //            oApplicant.Email = obj.GetValue(tempCells, formats, doc, 11).Encrypt();
        //            oApplicant.Mobile = obj.GetValue(tempCells, formats, doc, 12).Encrypt();
        //            oApplicant.EmployerId = 100000;
        //            oApplicant.Createdby = 18;
        //            DB.Applicants.Add(oApplicant);
        //            DB.SaveChanges();

        //            Case oCase = new Case();
        //            string zipCode = "0" + obj.GetValue(tempCells, formats, doc, 10);
        //            oCase.ApplicantID = oApplicant.ApplicantID;
        //            oCase.CaseTitle = "Case 1";
        //            oCase.Createdby = 18;
        //            oCase.ModifiedDateTime = Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 1)); ;
        //            oCase.FPL = obj.GetValue(tempCells, formats, doc, 20);
        //            oCase.HSAAmount = obj.GetValue(tempCells, formats, doc, 22);
        //            oCase.HSAFunding = obj.GetValue(tempCells, formats, doc, 17);
        //            oCase.HSALimit = obj.GetValue(tempCells, formats, doc, 21);
        //            oCase.MAGIncome = obj.GetValue(tempCells, formats, doc, 13);
        //            oCase.MonthlySubsidy = obj.GetValue(tempCells, formats, doc, 19);
        //            oCase.Notes = string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 19)) != null ? obj.GetValue(tempCells, formats, doc, 19).ToString().Encrypt() : null;
        //            oCase.PreviousYrHSA = false;
        //            oCase.TaxRate = obj.GetValue(tempCells, formats, doc, 16);
        //            oCase.TotalMedicalUsage = obj.GetValue(tempCells, formats, doc, 14);
        //            oCase.UsageID = Convert.ToInt32(obj.GetValue(tempCells, formats, doc, 15));
        //            oCase.Welness = false;
        //            oCase.Year = "2015";
        //            oCase.ZipCode = zipCode;
        //            oCase.CountyName = DB.qryZipStateCounties.Where(r => r.Zip == zipCode).Select(t => t.CountyName).First();
        //            oCase.RatingAreaId = DB.qryZipCodeToRatingAreas.Where(r => r.Zip == zipCode).Select(t => t.RatingAreaID).First();
        //            oCase.Applicant = null;
        //            oCase.Families = null;
        //            oCase.IsSubsidy = true;
        //            oCase.StatusId = 1;
        //            oCase.CaseSource = "Import from Excel";
        //            oCase.CreatedDateTime = oApplicant.CreatedDateTime;
        //            DB.Cases.Add(oCase);
        //            DB.SaveChanges();

        //            Dictionary<string, string> BenefitCost = new Dictionary<string, string>();
        //            BenefitCost.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 23));
        //            BenefitCost.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 24));
        //            BenefitCost.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 25));
        //            BenefitCost.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 26));
        //            BenefitCost.Add("Counseling", obj.GetValue(tempCells, formats, doc, 27));
        //            BenefitCost.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 28));
        //            BenefitCost.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 29));
        //            BenefitCost.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 30));
        //            BenefitCost.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 31));
        //            BenefitCost.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 32));
        //            BenefitCost.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 33));
        //            BenefitCost.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 34));
        //            BenefitCost.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 35));
        //            BenefitCost.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 36));
        //            BenefitCost.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 37));
        //            BenefitCost.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 38));
        //            BenefitCost.Add("Dental", obj.GetValue(tempCells, formats, doc, 39));
        //            BenefitCost.Add("Maternity", obj.GetValue(tempCells, formats, doc, 40));
        //            BenefitCost.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 41));
        //            BenefitCost.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 42));
        //            BenefitCost.Add("Vision", obj.GetValue(tempCells, formats, doc, 43));
        //            BenefitCost.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 44));
        //            BenefitCost.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 45));
        //            BenefitCost.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 46));
        //            BenefitCost.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 47));
        //            BenefitCost.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 48));
        //            BenefitCost.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 49));

        //            Dictionary<string, string> BenefitNotes = new Dictionary<string, string>();
        //            BenefitNotes.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 236));
        //            BenefitNotes.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 237));
        //            BenefitNotes.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 238));
        //            BenefitNotes.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 239));
        //            BenefitNotes.Add("Counseling", obj.GetValue(tempCells, formats, doc, 240));
        //            BenefitNotes.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 241));
        //            BenefitNotes.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 242));
        //            BenefitNotes.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 243));
        //            BenefitNotes.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 244));
        //            BenefitNotes.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 245));
        //            BenefitNotes.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 246));
        //            BenefitNotes.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 247));
        //            BenefitNotes.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 248));
        //            BenefitNotes.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 249));
        //            BenefitNotes.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 250));
        //            BenefitNotes.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 251));
        //            BenefitNotes.Add("Dental", obj.GetValue(tempCells, formats, doc, 252));
        //            BenefitNotes.Add("Maternity", obj.GetValue(tempCells, formats, doc, 253));
        //            BenefitNotes.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 254));
        //            BenefitNotes.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 255));
        //            BenefitNotes.Add("Vision", obj.GetValue(tempCells, formats, doc, 256));
        //            BenefitNotes.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 257));
        //            BenefitNotes.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 258));
        //            BenefitNotes.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 259));
        //            BenefitNotes.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 260));
        //            BenefitNotes.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 261));
        //            BenefitNotes.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 262));

        //            DateTime CheckDate;

        //            #region Family 1

        //            Family family1 = new Family();
        //            var famileMember1DOB = obj.GetValue(tempCells, formats, doc, 52);

        //            if (DateTime.TryParse(famileMember1DOB, out CheckDate))
        //            {
        //                family1.CaseNumId = oCase.CaseID;
        //                family1.Gender = obj.GetValue(tempCells, formats, doc, 51).Encrypt();
        //                family1.DOB = obj.GetValue(tempCells, formats, doc, 52).Encrypt();
        //                family1.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 52)), System.DateTime.Now).ToString();
        //                family1.Createdby = oCase.ModifiedBy;
        //                family1.CreatedDateTime = oApplicant.CreatedDateTime;
        //                family1.IsPrimary = obj.GetValue(tempCells, formats, doc, 50) == "Primary" ? true : false;
        //                family1.Smoking = obj.GetValue(tempCells, formats, doc, 53) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 53)) ? "false" : "true";
        //                family1.TotalMedicalUsage = "";
        //                family1.BenefitUserDetails = null;
        //                family1.Criticalillnesses = null;
        //                DB.Families.Add(family1);
        //                DB.SaveChanges();

        //                Dictionary<string, string> BenefitUser1 = new Dictionary<string, string>();
        //                BenefitUser1.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 54));
        //                BenefitUser1.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 55));
        //                BenefitUser1.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 56));
        //                BenefitUser1.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 57));
        //                BenefitUser1.Add("Counseling", obj.GetValue(tempCells, formats, doc, 58));
        //                BenefitUser1.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 59));
        //                BenefitUser1.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 60));
        //                BenefitUser1.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 61));
        //                BenefitUser1.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 62));
        //                BenefitUser1.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 63));
        //                BenefitUser1.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 64));
        //                BenefitUser1.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 65));
        //                BenefitUser1.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 66));
        //                BenefitUser1.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 67));
        //                BenefitUser1.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 68));
        //                BenefitUser1.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 69));
        //                BenefitUser1.Add("Dental", obj.GetValue(tempCells, formats, doc, 70));
        //                BenefitUser1.Add("Maternity", obj.GetValue(tempCells, formats, doc, 71));
        //                BenefitUser1.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 72));
        //                BenefitUser1.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 73));
        //                BenefitUser1.Add("Vision", obj.GetValue(tempCells, formats, doc, 74));
        //                BenefitUser1.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 75));
        //                BenefitUser1.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 76));
        //                BenefitUser1.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 77));
        //                BenefitUser1.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 78));
        //                BenefitUser1.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 79));
        //                BenefitUser1.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 80));

        //                foreach (var BenefitUser in BenefitUser1)
        //                {
        //                    if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                    {
        //                        var BenefitName = BenefitUser.Key;
        //                        //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                        BenefitUserDetail oBenenfitUses = new BenefitUserDetail();
        //                        oBenenfitUses.UsageCost = (Convert.ToInt64(BenefitCost[BenefitName]) * Convert.ToInt64(BenefitUser.Value)).ToString();
        //                        oBenenfitUses.UsageQty = BenefitUser.Value.ToString();
        //                        oBenenfitUses.MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID;
        //                        //MHMMappingBenefitId =  BenefitUserDetail["MHMMappingBenefitId"],
        //                        var test1 = BenefitNotes[BenefitName];
        //                        oBenenfitUses.UsageNotes = !string.IsNullOrEmpty(BenefitNotes[BenefitName]) ? BenefitNotes[BenefitName].ToString().Encrypt() : null;
        //                        oBenenfitUses.FamilyID = family1.FamilyID;
        //                        oBenenfitUses.Createdby = 18;
        //                        oBenenfitUses.CreatedDateTime = oApplicant.CreatedDateTime;

        //                        DB.BenefitUserDetails.Add(oBenenfitUses);
        //                        DB.SaveChanges();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Family 2

        //            Family family2 = new Family();
        //            var famileMember2DOB = obj.GetValue(tempCells, formats, doc, 83);

        //            if (DateTime.TryParse(famileMember2DOB, out CheckDate))
        //            {
        //                family2.CaseNumId = oCase.CaseID;
        //                family2.Gender = obj.GetValue(tempCells, formats, doc, 82).Encrypt();
        //                family2.DOB = obj.GetValue(tempCells, formats, doc, 83).Encrypt();
        //                family2.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 83)), System.DateTime.Now).ToString();
        //                family2.Createdby = oCase.ModifiedBy;
        //                family2.CreatedDateTime = oApplicant.CreatedDateTime;
        //                family2.IsPrimary = obj.GetValue(tempCells, formats, doc, 81) == "Primary" ? true : false;
        //                family2.Smoking = obj.GetValue(tempCells, formats, doc, 84) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                family2.TotalMedicalUsage = "";
        //                family2.BenefitUserDetails = null;
        //                family2.Criticalillnesses = null;
        //                DB.Families.Add(family2);
        //                DB.SaveChanges();

        //                Dictionary<string, string> BenefitUser2 = new Dictionary<string, string>();
        //                BenefitUser2.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 85));
        //                BenefitUser2.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 86));
        //                BenefitUser2.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 87));
        //                BenefitUser2.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 88));
        //                BenefitUser2.Add("Counseling", obj.GetValue(tempCells, formats, doc, 89));
        //                BenefitUser2.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 90));
        //                BenefitUser2.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 91));
        //                BenefitUser2.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 92));
        //                BenefitUser2.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 93));
        //                BenefitUser2.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 94));
        //                BenefitUser2.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 95));
        //                BenefitUser2.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 96));
        //                BenefitUser2.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 97));
        //                BenefitUser2.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 98));
        //                BenefitUser2.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 99));
        //                BenefitUser2.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 100));
        //                BenefitUser2.Add("Dental", obj.GetValue(tempCells, formats, doc, 101));
        //                BenefitUser2.Add("Maternity", obj.GetValue(tempCells, formats, doc, 102));
        //                BenefitUser2.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 103));
        //                BenefitUser2.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 104));
        //                BenefitUser2.Add("Vision", obj.GetValue(tempCells, formats, doc, 105));
        //                BenefitUser2.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 106));
        //                BenefitUser2.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 107));
        //                BenefitUser2.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 108));
        //                BenefitUser2.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 109));
        //                BenefitUser2.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 110));
        //                BenefitUser2.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 111));

        //                foreach (var BenefitUser in BenefitUser2)
        //                {
        //                    if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                    {
        //                        var BenefitName = BenefitUser.Key;
        //                        //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                        BenefitUserDetail oBenenfitUses = new BenefitUserDetail()
        //                        {
        //                            UsageCost = (Convert.ToInt64(BenefitCost[BenefitName]) * Convert.ToInt64(BenefitUser.Value)).ToString(),
        //                            UsageQty = BenefitUser.Value.ToString(),
        //                            MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID,
        //                            //MHMMappingBenefitId =  BenefitUserDetail["MHMMappingBenefitId"],
        //                            //UsageNotes = BenefitNotes["UsageNotes"] != null ? BenefitNotes["UsageNotes"].ToString().Encrypt() : null,
        //                            FamilyID = family2.FamilyID,
        //                            Createdby = 18,
        //                            CreatedDateTime = oApplicant.CreatedDateTime
        //                        };
        //                        DB.BenefitUserDetails.Add(oBenenfitUses);
        //                        DB.SaveChanges();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Family 3

        //            Family family3 = new Family();
        //            var famileMember3DOB = obj.GetValue(tempCells, formats, doc, 114);

        //            if (DateTime.TryParse(famileMember3DOB, out CheckDate))
        //            {
        //                family3.CaseNumId = oCase.CaseID;
        //                family3.Gender = obj.GetValue(tempCells, formats, doc, 113).Encrypt();
        //                family3.DOB = obj.GetValue(tempCells, formats, doc, 114).Encrypt();
        //                family3.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 114)), System.DateTime.Now).ToString();
        //                family3.Createdby = oCase.ModifiedBy;
        //                family3.CreatedDateTime = oApplicant.CreatedDateTime;
        //                family3.IsPrimary = obj.GetValue(tempCells, formats, doc, 112) == "Primary" ? true : false;
        //                family3.Smoking = obj.GetValue(tempCells, formats, doc, 115) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                family3.TotalMedicalUsage = "";
        //                family3.BenefitUserDetails = null;
        //                family3.Criticalillnesses = null;
        //                DB.Families.Add(family3);
        //                DB.SaveChanges();

        //                Dictionary<string, string> BenefitUser3 = new Dictionary<string, string>();
        //                BenefitUser3.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 116));
        //                BenefitUser3.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 117));
        //                BenefitUser3.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 118));
        //                BenefitUser3.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 119));
        //                BenefitUser3.Add("Counseling", obj.GetValue(tempCells, formats, doc, 120));
        //                BenefitUser3.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 121));
        //                BenefitUser3.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 122));
        //                BenefitUser3.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 123));
        //                BenefitUser3.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 124));
        //                BenefitUser3.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 125));
        //                BenefitUser3.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 126));
        //                BenefitUser3.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 127));
        //                BenefitUser3.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 128));
        //                BenefitUser3.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 129));
        //                BenefitUser3.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 130));
        //                BenefitUser3.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 131));
        //                BenefitUser3.Add("Dental", obj.GetValue(tempCells, formats, doc, 132));
        //                BenefitUser3.Add("Maternity", obj.GetValue(tempCells, formats, doc, 133));
        //                BenefitUser3.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 134));
        //                BenefitUser3.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 135));
        //                BenefitUser3.Add("Vision", obj.GetValue(tempCells, formats, doc, 136));
        //                BenefitUser3.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 137));
        //                BenefitUser3.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 138));
        //                BenefitUser3.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 139));
        //                BenefitUser3.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 140));
        //                BenefitUser3.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 141));
        //                BenefitUser3.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 142));

        //                foreach (var BenefitUser in BenefitUser3)
        //                {
        //                    if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                    {
        //                        var BenefitName = BenefitUser.Key;
        //                        //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                        BenefitUserDetail oBenenfitUses = new BenefitUserDetail()
        //                        {
        //                            UsageCost = (Convert.ToInt64(BenefitCost[BenefitName]) * Convert.ToInt64(BenefitUser.Value)).ToString(),
        //                            UsageQty = BenefitUser.Value.ToString(),
        //                            MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID,
        //                            //MHMMappingBenefitId =  BenefitUserDetail["MHMMappingBenefitId"],
        //                            //UsageNotes = BenefitNotes["UsageNotes"] != null ? BenefitNotes["UsageNotes"].ToString().Encrypt() : null,
        //                            FamilyID = family3.FamilyID,
        //                            Createdby = 18,
        //                            CreatedDateTime = oApplicant.CreatedDateTime
        //                        };
        //                        DB.BenefitUserDetails.Add(oBenenfitUses);
        //                        DB.SaveChanges();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Family 4

        //            Family family4 = new Family();
        //            var famileMember4DOB = obj.GetValue(tempCells, formats, doc, 145);
        //            if (DateTime.TryParse(famileMember4DOB, out CheckDate))
        //            {
        //                family4.CaseNumId = oCase.CaseID;
        //                family4.Gender = obj.GetValue(tempCells, formats, doc, 144).Encrypt();
        //                family4.DOB = obj.GetValue(tempCells, formats, doc, 145).Encrypt();
        //                family4.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 145)), System.DateTime.Now).ToString();
        //                family4.Createdby = oCase.ModifiedBy;
        //                family4.CreatedDateTime = oApplicant.CreatedDateTime;
        //                family4.IsPrimary = obj.GetValue(tempCells, formats, doc, 143) == "Primary" ? true : false;
        //                family4.Smoking = obj.GetValue(tempCells, formats, doc, 146) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                family4.TotalMedicalUsage = "";
        //                family4.BenefitUserDetails = null;
        //                family4.Criticalillnesses = null;
        //                DB.Families.Add(family4);
        //                DB.SaveChanges();

        //                Dictionary<string, string> BenefitUser4 = new Dictionary<string, string>();
        //                BenefitUser4.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 147));
        //                BenefitUser4.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 148));
        //                BenefitUser4.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 149));
        //                BenefitUser4.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 150));
        //                BenefitUser4.Add("Counseling", obj.GetValue(tempCells, formats, doc, 151));
        //                BenefitUser4.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 152));
        //                BenefitUser4.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 153));
        //                BenefitUser4.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 154));
        //                BenefitUser4.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 155));
        //                BenefitUser4.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 156));
        //                BenefitUser4.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 157));
        //                BenefitUser4.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 158));
        //                BenefitUser4.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 159));
        //                BenefitUser4.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 160));
        //                BenefitUser4.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 161));
        //                BenefitUser4.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 162));
        //                BenefitUser4.Add("Dental", obj.GetValue(tempCells, formats, doc, 163));
        //                BenefitUser4.Add("Maternity", obj.GetValue(tempCells, formats, doc, 164));
        //                BenefitUser4.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 165));
        //                BenefitUser4.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 166));
        //                BenefitUser4.Add("Vision", obj.GetValue(tempCells, formats, doc, 167));
        //                BenefitUser4.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 168));
        //                BenefitUser4.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 169));
        //                BenefitUser4.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 170));
        //                BenefitUser4.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 171));
        //                BenefitUser4.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 172));
        //                BenefitUser4.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 173));

        //                foreach (var BenefitUser in BenefitUser4)
        //                {
        //                    if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                    {
        //                        var BenefitName = BenefitUser.Key;
        //                        //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                        BenefitUserDetail oBenenfitUses = new BenefitUserDetail()
        //                        {
        //                            UsageCost = (Convert.ToInt64(BenefitCost[BenefitName]) * Convert.ToInt64(BenefitUser.Value)).ToString(),
        //                            UsageQty = BenefitUser.Value.ToString(),
        //                            MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID,
        //                            //MHMMappingBenefitId =  BenefitUserDetail["MHMMappingBenefitId"],
        //                            //UsageNotes = BenefitNotes["UsageNotes"] != null ? BenefitNotes["UsageNotes"].ToString().Encrypt() : null,
        //                            FamilyID = family4.FamilyID,
        //                            Createdby = 18,
        //                            CreatedDateTime = oApplicant.CreatedDateTime
        //                        };
        //                        DB.BenefitUserDetails.Add(oBenenfitUses);
        //                        DB.SaveChanges();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Family 5

        //            Family family5 = new Family();
        //            var famileMember5DOB = obj.GetValue(tempCells, formats, doc, 176);

        //            if (DateTime.TryParse(famileMember5DOB, out CheckDate))
        //            {
        //                family5.CaseNumId = oCase.CaseID;
        //                family5.Gender = obj.GetValue(tempCells, formats, doc, 175).Encrypt();
        //                family5.DOB = obj.GetValue(tempCells, formats, doc, 176).Encrypt();
        //                family5.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 176)), System.DateTime.Now).ToString();
        //                family5.Createdby = oCase.ModifiedBy;
        //                family5.CreatedDateTime = oApplicant.CreatedDateTime;
        //                family5.IsPrimary = obj.GetValue(tempCells, formats, doc, 174) == "Primary" ? true : false;
        //                family5.Smoking = obj.GetValue(tempCells, formats, doc, 177) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                family5.TotalMedicalUsage = "";
        //                family5.BenefitUserDetails = null;
        //                family5.Criticalillnesses = null;
        //                DB.Families.Add(family5);
        //                DB.SaveChanges();

        //                Dictionary<string, string> BenefitUser5 = new Dictionary<string, string>();
        //                BenefitUser5.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 178));
        //                BenefitUser5.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 179));
        //                BenefitUser5.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 180));
        //                BenefitUser5.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 181));
        //                BenefitUser5.Add("Counseling", obj.GetValue(tempCells, formats, doc, 182));
        //                BenefitUser5.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 183));
        //                BenefitUser5.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 184));
        //                BenefitUser5.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 185));
        //                BenefitUser5.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 186));
        //                BenefitUser5.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 187));
        //                BenefitUser5.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 188));
        //                BenefitUser5.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 189));
        //                BenefitUser5.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 190));
        //                BenefitUser5.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 191));
        //                BenefitUser5.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 192));
        //                BenefitUser5.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 193));
        //                BenefitUser5.Add("Dental", obj.GetValue(tempCells, formats, doc, 194));
        //                BenefitUser5.Add("Maternity", obj.GetValue(tempCells, formats, doc, 195));
        //                BenefitUser5.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 196));
        //                BenefitUser5.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 197));
        //                BenefitUser5.Add("Vision", obj.GetValue(tempCells, formats, doc, 198));
        //                BenefitUser5.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 199));
        //                BenefitUser5.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 200));
        //                BenefitUser5.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 201));
        //                BenefitUser5.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 202));
        //                BenefitUser5.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 203));
        //                BenefitUser5.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 204));

        //                foreach (var BenefitUser in BenefitUser5)
        //                {
        //                    if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                    {
        //                        var BenefitName = BenefitUser.Key;
        //                        //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                        BenefitUserDetail oBenenfitUses = new BenefitUserDetail()
        //                        {
        //                            UsageCost = (Convert.ToInt64(BenefitCost[BenefitName]) * Convert.ToInt64(BenefitUser.Value)).ToString(),
        //                            UsageQty = BenefitUser.Value.ToString(),
        //                            MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID,
        //                            //MHMMappingBenefitId =  BenefitUserDetail["MHMMappingBenefitId"],
        //                            //UsageNotes = BenefitNotes["UsageNotes"] != null ? BenefitNotes["UsageNotes"].ToString().Encrypt() : null,
        //                            FamilyID = family5.FamilyID,
        //                            Createdby = 18,
        //                            CreatedDateTime = oApplicant.CreatedDateTime
        //                        };
        //                        DB.BenefitUserDetails.Add(oBenenfitUses);
        //                        DB.SaveChanges();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Family 6

        //            Family family6 = new Family();
        //            var famileMember6DOB = obj.GetValue(tempCells, formats, doc, 207);
        //            if (DateTime.TryParse(famileMember6DOB, out CheckDate))
        //            {
        //                family6.CaseNumId = oCase.CaseID;
        //                family6.Gender = obj.GetValue(tempCells, formats, doc, 206).Encrypt();
        //                family6.DOB = obj.GetValue(tempCells, formats, doc, 207).Encrypt();
        //                family6.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 207)), System.DateTime.Now).ToString();
        //                family6.Createdby = oCase.ModifiedBy;
        //                family6.CreatedDateTime = oApplicant.CreatedDateTime;
        //                family6.IsPrimary = obj.GetValue(tempCells, formats, doc, 205) == "Primary" ? true : false;
        //                family6.Smoking = obj.GetValue(tempCells, formats, doc, 208) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                family6.TotalMedicalUsage = "";
        //                family6.BenefitUserDetails = null;
        //                family6.Criticalillnesses = null;
        //                DB.Families.Add(family6);
        //                DB.SaveChanges();

        //                Dictionary<string, string> BenefitUser6 = new Dictionary<string, string>();
        //                BenefitUser6.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 209));
        //                BenefitUser6.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 210));
        //                BenefitUser6.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 211));
        //                BenefitUser6.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 212));
        //                BenefitUser6.Add("Counseling", obj.GetValue(tempCells, formats, doc, 213));
        //                BenefitUser6.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 214));
        //                BenefitUser6.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 215));
        //                BenefitUser6.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 216));
        //                BenefitUser6.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 217));
        //                BenefitUser6.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 218));
        //                BenefitUser6.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 219));
        //                BenefitUser6.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 220));
        //                BenefitUser6.Add("Skilled Nursing Services", obj.GetValue(tempCells, formats, doc, 221));
        //                BenefitUser6.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 222));
        //                BenefitUser6.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 223));
        //                BenefitUser6.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 224));
        //                BenefitUser6.Add("Dental", obj.GetValue(tempCells, formats, doc, 225));
        //                BenefitUser6.Add("Maternity", obj.GetValue(tempCells, formats, doc, 226));
        //                BenefitUser6.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 227));
        //                BenefitUser6.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 228));
        //                BenefitUser6.Add("Vision", obj.GetValue(tempCells, formats, doc, 229));
        //                BenefitUser6.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 230));
        //                BenefitUser6.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 231));
        //                BenefitUser6.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 232));
        //                BenefitUser6.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 233));
        //                BenefitUser6.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 234));
        //                BenefitUser6.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 235));

        //                foreach (var BenefitUser in BenefitUser6)
        //                {
        //                    if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                    {
        //                        var BenefitName = BenefitUser.Key;
        //                        //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                        BenefitUserDetail oBenenfitUses = new BenefitUserDetail()
        //                        {
        //                            UsageCost = (Convert.ToInt64(BenefitCost[BenefitName]) * Convert.ToInt64(BenefitUser.Value)).ToString(),
        //                            UsageQty = BenefitUser.Value.ToString(),
        //                            MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID,
        //                            //MHMMappingBenefitId =  BenefitUserDetail["MHMMappingBenefitId"],
        //                            //UsageNotes = BenefitNotes["UsageNotes"] != null ? BenefitNotes["UsageNotes"].ToString().Encrypt() : null,
        //                            FamilyID = family6.FamilyID,
        //                            Createdby = 18,
        //                            CreatedDateTime = oApplicant.CreatedDateTime
        //                        };
        //                        DB.BenefitUserDetails.Add(oBenenfitUses);
        //                        DB.SaveChanges();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region CasePlanResult 1

        //            CasePlanResult CasePlanResult1 = new CasePlanResult();

        //            CasePlanResult1.CaseId = oCase.CaseID;
        //            CasePlanResult1.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 263);
        //            CasePlanResult1.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 264);
        //            CasePlanResult1.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 265);
        //            CasePlanResult1.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 266);
        //            CasePlanResult1.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 267);
        //            //MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //            CasePlanResult1.Copays = obj.GetValue(tempCells, formats, doc, 268);
        //            CasePlanResult1.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 269);
        //            CasePlanResult1.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 270);
        //            //ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //            CasePlanResult1.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //            CasePlanResult1.Medical = obj.GetValue(tempCells, formats, doc, 272);
        //            CasePlanResult1.TotalPaid = obj.GetValue(tempCells, formats, doc, 273);
        //            CasePlanResult1.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 274);
        //            CasePlanResult1.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 275).Substring(0, obj.GetValue(tempCells, formats, doc, 275).IndexOf('/')).Replace("$", "");
        //            CasePlanResult1.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 275).Substring(obj.GetValue(tempCells, formats, doc, 275).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult1.OPLSingle = obj.GetValue(tempCells, formats, doc, 276).Substring(obj.GetValue(tempCells, formats, doc, 276).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult1.OPLFamily = obj.GetValue(tempCells, formats, doc, 276).Substring(obj.GetValue(tempCells, formats, doc, 276).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult1.Coinsurance = obj.GetValue(tempCells, formats, doc, 277);
        //            CasePlanResult1.WorstCase = obj.GetValue(tempCells, formats, doc, 278);
        //            CasePlanResult1.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 279);
        //            CasePlanResult1.CreatedDateTime = oApplicant.CreatedDateTime;


        //            DB.CasePlanResults.Add(CasePlanResult1);
        //            DB.SaveChanges();

        //            #endregion

        //            #region CasePlanResult 2

        //            CasePlanResult CasePlanResult2 = new CasePlanResult();

        //            CasePlanResult2.CaseId = oCase.CaseID;
        //            CasePlanResult2.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 280);
        //            CasePlanResult2.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 281);
        //            CasePlanResult2.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 282);
        //            CasePlanResult2.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 283);
        //            CasePlanResult2.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 284);
        //            //MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //            CasePlanResult2.Copays = obj.GetValue(tempCells, formats, doc, 285);
        //            CasePlanResult2.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 286);
        //            CasePlanResult2.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 287);
        //            //ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //            CasePlanResult2.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 288);
        //            CasePlanResult2.Medical = obj.GetValue(tempCells, formats, doc, 289);
        //            CasePlanResult2.TotalPaid = obj.GetValue(tempCells, formats, doc, 290);
        //            CasePlanResult2.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 291);
        //            CasePlanResult2.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 292).Substring(0, obj.GetValue(tempCells, formats, doc, 292).IndexOf('/')).Replace("$", "");
        //            CasePlanResult2.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 292).Substring(obj.GetValue(tempCells, formats, doc, 292).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult2.OPLSingle = obj.GetValue(tempCells, formats, doc, 293).Substring(obj.GetValue(tempCells, formats, doc, 293).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult2.OPLFamily = obj.GetValue(tempCells, formats, doc, 293).Substring(obj.GetValue(tempCells, formats, doc, 293).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult2.Coinsurance = obj.GetValue(tempCells, formats, doc, 294);
        //            CasePlanResult2.WorstCase = obj.GetValue(tempCells, formats, doc, 295);
        //            CasePlanResult2.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 296);
        //            CasePlanResult2.CreatedDateTime = oApplicant.CreatedDateTime;


        //            DB.CasePlanResults.Add(CasePlanResult2);
        //            DB.SaveChanges();

        //            #endregion

        //            #region CasePlanResult 3

        //            CasePlanResult CasePlanResult3 = new CasePlanResult();

        //            CasePlanResult3.CaseId = oCase.CaseID;
        //            CasePlanResult3.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 297);
        //            CasePlanResult3.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 298);
        //            CasePlanResult3.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 299);
        //            CasePlanResult3.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 300);
        //            CasePlanResult3.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 301);
        //            //MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //            CasePlanResult3.Copays = obj.GetValue(tempCells, formats, doc, 302);
        //            CasePlanResult3.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 303);
        //            CasePlanResult3.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 304);
        //            //ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //            CasePlanResult3.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 305);
        //            CasePlanResult3.Medical = obj.GetValue(tempCells, formats, doc, 306);
        //            CasePlanResult3.TotalPaid = obj.GetValue(tempCells, formats, doc, 307);
        //            CasePlanResult3.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 308);
        //            CasePlanResult3.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 309).Substring(0, obj.GetValue(tempCells, formats, doc, 309).IndexOf('/')).Replace("$", "");
        //            CasePlanResult3.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 309).Substring(obj.GetValue(tempCells, formats, doc, 309).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult3.OPLSingle = obj.GetValue(tempCells, formats, doc, 310).Substring(obj.GetValue(tempCells, formats, doc, 310).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult3.OPLFamily = obj.GetValue(tempCells, formats, doc, 310).Substring(obj.GetValue(tempCells, formats, doc, 310).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult3.Coinsurance = obj.GetValue(tempCells, formats, doc, 311);
        //            CasePlanResult3.WorstCase = obj.GetValue(tempCells, formats, doc, 312);
        //            CasePlanResult3.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 313);
        //            CasePlanResult3.CreatedDateTime = oApplicant.CreatedDateTime;


        //            DB.CasePlanResults.Add(CasePlanResult3);
        //            DB.SaveChanges();

        //            #endregion

        //            #region CasePlanResult 4

        //            CasePlanResult CasePlanResult4 = new CasePlanResult();

        //            CasePlanResult4.CaseId = oCase.CaseID;
        //            CasePlanResult4.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 314);
        //            CasePlanResult4.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 315);
        //            CasePlanResult4.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 316);
        //            CasePlanResult4.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 317);
        //            CasePlanResult4.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 318);
        //            //MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //            CasePlanResult4.Copays = obj.GetValue(tempCells, formats, doc, 319);
        //            CasePlanResult4.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 320);
        //            CasePlanResult4.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 321);
        //            //ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //            CasePlanResult4.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 322);
        //            CasePlanResult4.Medical = obj.GetValue(tempCells, formats, doc, 323);
        //            CasePlanResult4.TotalPaid = obj.GetValue(tempCells, formats, doc, 324);
        //            CasePlanResult4.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 325);
        //            CasePlanResult4.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 326).Substring(0, obj.GetValue(tempCells, formats, doc, 326).IndexOf('/')).Replace("$", "");
        //            CasePlanResult4.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 326).Substring(obj.GetValue(tempCells, formats, doc, 326).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult4.OPLSingle = obj.GetValue(tempCells, formats, doc, 327).Substring(obj.GetValue(tempCells, formats, doc, 327).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult4.OPLFamily = obj.GetValue(tempCells, formats, doc, 327).Substring(obj.GetValue(tempCells, formats, doc, 327).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult4.Coinsurance = obj.GetValue(tempCells, formats, doc, 328);
        //            CasePlanResult4.WorstCase = obj.GetValue(tempCells, formats, doc, 329);
        //            CasePlanResult4.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 330);
        //            CasePlanResult4.CreatedDateTime = oApplicant.CreatedDateTime;


        //            DB.CasePlanResults.Add(CasePlanResult4);
        //            DB.SaveChanges();

        //            #endregion

        //            #region CasePlanResult 5

        //            CasePlanResult CasePlanResult5 = new CasePlanResult();

        //            CasePlanResult5.CaseId = oCase.CaseID;
        //            CasePlanResult5.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 331);
        //            CasePlanResult5.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 332);
        //            CasePlanResult5.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 333);
        //            CasePlanResult5.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 334);
        //            CasePlanResult5.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 335);
        //            //MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //            CasePlanResult5.Copays = obj.GetValue(tempCells, formats, doc, 336);
        //            CasePlanResult5.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 337);
        //            CasePlanResult5.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 338);
        //            //ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //            CasePlanResult5.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 339);
        //            CasePlanResult5.Medical = obj.GetValue(tempCells, formats, doc, 340);
        //            CasePlanResult5.TotalPaid = obj.GetValue(tempCells, formats, doc, 341);
        //            CasePlanResult5.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 342);
        //            CasePlanResult5.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 343).Substring(0, obj.GetValue(tempCells, formats, doc, 343).IndexOf('/')).Replace("$", "");
        //            CasePlanResult5.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 343).Substring(obj.GetValue(tempCells, formats, doc, 343).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult5.OPLSingle = obj.GetValue(tempCells, formats, doc, 344).Substring(obj.GetValue(tempCells, formats, doc, 344).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult5.OPLFamily = obj.GetValue(tempCells, formats, doc, 344).Substring(obj.GetValue(tempCells, formats, doc, 344).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult5.Coinsurance = obj.GetValue(tempCells, formats, doc, 345);
        //            CasePlanResult5.WorstCase = obj.GetValue(tempCells, formats, doc, 346);
        //            CasePlanResult5.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 347);
        //            CasePlanResult5.CreatedDateTime = oApplicant.CreatedDateTime;


        //            DB.CasePlanResults.Add(CasePlanResult5);
        //            DB.SaveChanges();

        //            #endregion

        //            #region CasePlanResult 6

        //            CasePlanResult CasePlanResult6 = new CasePlanResult();

        //            CasePlanResult6.CaseId = oCase.CaseID;
        //            CasePlanResult6.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 348);
        //            CasePlanResult6.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 349);
        //            CasePlanResult6.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 350);
        //            CasePlanResult6.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 351);
        //            CasePlanResult6.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 352);
        //            //MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //            CasePlanResult6.Copays = obj.GetValue(tempCells, formats, doc, 353);
        //            CasePlanResult6.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 354);
        //            CasePlanResult6.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 355);
        //            //ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //            CasePlanResult6.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 356);
        //            CasePlanResult6.Medical = obj.GetValue(tempCells, formats, doc, 357);
        //            CasePlanResult6.TotalPaid = obj.GetValue(tempCells, formats, doc, 358);
        //            CasePlanResult6.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 359);
        //            CasePlanResult6.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 360).Substring(0, obj.GetValue(tempCells, formats, doc, 360).IndexOf('/')).Replace("$", "");
        //            CasePlanResult6.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 360).Substring(obj.GetValue(tempCells, formats, doc, 360).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult6.OPLSingle = obj.GetValue(tempCells, formats, doc, 361).Substring(obj.GetValue(tempCells, formats, doc, 361).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult6.OPLFamily = obj.GetValue(tempCells, formats, doc, 361).Substring(obj.GetValue(tempCells, formats, doc, 361).IndexOf('/') + 1).Replace("$", "");
        //            CasePlanResult6.Coinsurance = obj.GetValue(tempCells, formats, doc, 362);
        //            CasePlanResult6.WorstCase = obj.GetValue(tempCells, formats, doc, 363);
        //            CasePlanResult6.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 364);
        //            CasePlanResult6.CreatedDateTime = oApplicant.CreatedDateTime;


        //            DB.CasePlanResults.Add(CasePlanResult6);
        //            DB.SaveChanges();

        //            #endregion


        //            dbContextTransaction.Commit();

        //        }
        //        catch (DbEntityValidationException e)
        //        {
        //            string Error = "";
        //            foreach (var eve in e.EntityValidationErrors)
        //            {

        //                Error = "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + ' ' + eve.Entry.State;
        //                foreach (var ve in eve.ValidationErrors)
        //                {
        //                    Error = "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + ' ' + ve.ErrorMessage;
        //                }
        //            }
        //            dbContextTransaction.Rollback();

        //        }
        //        catch (Exception ex)
        //        {
        //            dbContextTransaction.Rollback();

        //        }
        //    }
        //}

        //private void ImportGroupSheet(SpreadsheetDocument doc, Sheet sheet)
        //{
        //    string ErrorMsg = "";
        //    string CaseNo = "";

        //    ImportExcel obj = new ImportExcel();

        //    //Get the Worksheet instance.
        //    Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

        //    //Fetch all the rows present in the Worksheet.
        //    IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
        //    List<CellFormats> formats = new List<CellFormats>(doc.WorkbookPart.WorkbookStylesPart.Stylesheet.Descendants<CellFormats>());

        //    var Cases = rows.Skip(355).Take(1).ToList();
        //    foreach (var tempRow in Cases)
        //    {


        //        List<Cell> tempCells = ImportExcel.GetRowCells(tempRow).ToList();
        //        int c = tempCells.Count();

        //        using (var dbContextTransaction = DB.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                CaseNo = "Imported Case - " + obj.GetValue(tempCells, formats, doc, 0);

        //                Applicant oApplicant = new Applicant();
        //                var testdate = obj.GetValue(tempCells, formats, doc, 1).ToString();
        //                oApplicant.CreatedDateTime = Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 1));
        //                string JobNumber = obj.GetValue(tempCells, formats, doc, 2);
        //                if (string.IsNullOrEmpty(JobNumber)) { oApplicant.EmployerId = null; }
        //                else { oApplicant.EmployerId = DB.JobMasters.Where(t => t.JobNumber == JobNumber).Select(r => r.EmployerId).FirstOrDefault(); }
        //                if (obj.GetValue(tempCells, formats, doc, 3).ToString().Contains(' '))
        //                {
        //                    oApplicant.FirstName = obj.GetValue(tempCells, formats, doc, 3).Split(' ').First().Encrypt();
        //                    oApplicant.LastName = obj.GetValue(tempCells, formats, doc, 3).Split(' ').Last().Encrypt();
        //                }
        //                else if (obj.GetValue(tempCells, formats, doc, 3).ToString().Contains('.'))
        //                {
        //                    oApplicant.FirstName = obj.GetValue(tempCells, formats, doc, 3).Split('.').First().Encrypt();
        //                    oApplicant.LastName = obj.GetValue(tempCells, formats, doc, 3).Split('.').Last().Encrypt();
        //                }
        //                else
        //                {
        //                    oApplicant.FirstName = obj.GetValue(tempCells, formats, doc, 3).Encrypt();
        //                    oApplicant.LastName = String.Empty.Encrypt();
        //                }
        //                oApplicant.CurrentPlan = obj.GetValue(tempCells, formats, doc, 5);
        //                oApplicant.CurrentPremium = obj.GetValue(tempCells, formats, doc, 6);
        //                oApplicant.Street = obj.GetValue(tempCells, formats, doc, 7).Encrypt();
        //                oApplicant.City = obj.GetValue(tempCells, formats, doc, 8).Encrypt();
        //                oApplicant.State = obj.GetValue(tempCells, formats, doc, 9).Encrypt();
        //                if (obj.GetValue(tempCells, formats, doc, 10).Length == 4)
        //                {
        //                    oApplicant.Zip = ("0" + obj.GetValue(tempCells, formats, doc, 10)).Encrypt();
        //                }
        //                else
        //                {
        //                    oApplicant.Zip = obj.GetValue(tempCells, formats, doc, 10).Encrypt();
        //                }
        //                oApplicant.Email = obj.GetValue(tempCells, formats, doc, 11).ToLower().Encrypt();
        //                oApplicant.Mobile = obj.GetValue(tempCells, formats, doc, 12).Encrypt();
        //                oApplicant.Createdby = 18;
        //                DB.Applicants.Add(oApplicant);
        //                DB.SaveChanges();

        //                Case oCase = new Case();
        //                string zipCode = "";
        //                if (obj.GetValue(tempCells, formats, doc, 10).Length == 4)
        //                {
        //                    zipCode = ("0" + obj.GetValue(tempCells, formats, doc, 10));
        //                }
        //                else
        //                {
        //                    zipCode = obj.GetValue(tempCells, formats, doc, 10);
        //                }
        //                oCase.ApplicantID = oApplicant.ApplicantID;
        //                oCase.CaseTitle = CaseNo;
        //                oCase.Createdby = 18;
        //                oCase.CreatedDateTime = Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 1));
        //                if (obj.GetValue(tempCells, formats, doc, 20) == "")
        //                {
        //                    oCase.FPL = "0";
        //                }
        //                else
        //                {
        //                    oCase.FPL = (Convert.ToDecimal(obj.GetValue(tempCells, formats, doc, 20)) * 100).ToString();
        //                }

        //                oCase.HSAAmount = obj.GetValue(tempCells, formats, doc, 22);
        //                var test = obj.GetValue(tempCells, formats, doc, 17);
        //                if (obj.GetValue(tempCells, formats, doc, 17) == "")
        //                {
        //                    oCase.HSAFunding = "0";
        //                }
        //                else
        //                {
        //                    oCase.HSAFunding = (Convert.ToDecimal(obj.GetValue(tempCells, formats, doc, 17)) * 100).ToString();
        //                }

        //                oCase.HSALimit = obj.GetValue(tempCells, formats, doc, 21);
        //                oCase.MAGIncome = obj.GetValue(tempCells, formats, doc, 13);
        //                oCase.MonthlySubsidy = obj.GetValue(tempCells, formats, doc, 19);
        //                oCase.Notes = string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 4)) != null ? obj.GetValue(tempCells, formats, doc, 4).ToString().Encrypt() : null;
        //                //oCase.Notes = string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 19)) != null ? obj.GetValue(tempCells, formats, doc, 19).ToString().Encrypt() : null;
        //                oCase.PreviousYrHSA = false;
        //                if (obj.GetValue(tempCells, formats, doc, 16) == "")
        //                {
        //                    oCase.TaxRate = "0";
        //                }
        //                else
        //                {
        //                    oCase.TaxRate = (Convert.ToDecimal(obj.GetValue(tempCells, formats, doc, 16)) * 100).ToString();
        //                }

        //                oCase.TotalMedicalUsage = obj.GetValue(tempCells, formats, doc, 14);
        //                oCase.UsageID = Convert.ToInt32(obj.GetValue(tempCells, formats, doc, 15));
        //                oCase.Welness = false;
        //                oCase.Year = "2017";
        //                oCase.ZipCode = zipCode;
        //                if (DB.qryZipStateCounties.Where(r => r.Zip == zipCode).Count() > 0)
        //                {
        //                    oCase.CountyName = DB.qryZipStateCounties.Where(r => r.Zip == zipCode).Select(t => t.CountyName).First();
        //                }
        //                else
        //                {
        //                    dbContextTransaction.Rollback();
        //                    ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t County Name not found for Zipcode : " + zipCode;
        //                    if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                    {
        //                        System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                    }
        //                    else
        //                    {
        //                        System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                    }

        //                    continue;
        //                }
        //                if (DB.qryZipCodeToRatingAreas.Where(r => r.Zip == zipCode).Count() > 0)
        //                {
        //                    oCase.RatingAreaId = DB.qryZipCodeToRatingAreas.Where(r => r.Zip == zipCode).Select(t => t.RatingAreaID).First();
        //                }
        //                else
        //                {
        //                    dbContextTransaction.Rollback();
        //                    ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t Rating Area not found for Zipcode : " + zipCode;
        //                    if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                    {
        //                        System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                    }
        //                    else
        //                    {
        //                        System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                    }
        //                    continue;
        //                }
        //                oCase.Applicant = null;
        //                oCase.Families = null;
        //                oCase.IsSubsidy = true;
        //                oCase.StatusId = 1;
        //                oCase.JobNumber = JobNumber;
        //                oCase.CaseSource = "Import from Excel";
        //                oCase.CreatedDateTime = oApplicant.CreatedDateTime;
        //                DB.Cases.Add(oCase);
        //                DB.SaveChanges();

        //                Dictionary<string, string> BenefitCost = new Dictionary<string, string>();
        //                BenefitCost.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 23));
        //                BenefitCost.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 24));
        //                BenefitCost.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 25));
        //                BenefitCost.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 26));
        //                BenefitCost.Add("Counseling", obj.GetValue(tempCells, formats, doc, 27));
        //                BenefitCost.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 28));
        //                BenefitCost.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 29));
        //                BenefitCost.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 30));
        //                BenefitCost.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 31));
        //                BenefitCost.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 32));
        //                BenefitCost.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 33));
        //                BenefitCost.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 34));
        //                BenefitCost.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 35));
        //                BenefitCost.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 36));
        //                BenefitCost.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 37));
        //                BenefitCost.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 38));
        //                BenefitCost.Add("Dental", obj.GetValue(tempCells, formats, doc, 39));
        //                BenefitCost.Add("Maternity", obj.GetValue(tempCells, formats, doc, 40));
        //                BenefitCost.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 41));
        //                BenefitCost.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 42));
        //                BenefitCost.Add("Vision", obj.GetValue(tempCells, formats, doc, 43));
        //                BenefitCost.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 44));
        //                BenefitCost.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 45));
        //                BenefitCost.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 46));
        //                BenefitCost.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 47));
        //                BenefitCost.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 48));
        //                BenefitCost.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 49));

        //                Dictionary<string, string> BenefitNotes = new Dictionary<string, string>();
        //                BenefitNotes.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 236));
        //                BenefitNotes.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 237));
        //                BenefitNotes.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 238));
        //                BenefitNotes.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 239));
        //                BenefitNotes.Add("Counseling", obj.GetValue(tempCells, formats, doc, 240));
        //                BenefitNotes.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 241));
        //                BenefitNotes.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 242));
        //                BenefitNotes.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 243));
        //                BenefitNotes.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 244));
        //                BenefitNotes.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 245));
        //                BenefitNotes.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 246));
        //                BenefitNotes.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 247));
        //                BenefitNotes.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 248));
        //                BenefitNotes.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 249));
        //                BenefitNotes.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 250));
        //                BenefitNotes.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 251));
        //                BenefitNotes.Add("Dental", obj.GetValue(tempCells, formats, doc, 252));
        //                BenefitNotes.Add("Maternity", obj.GetValue(tempCells, formats, doc, 253));
        //                BenefitNotes.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 254));
        //                BenefitNotes.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 255));
        //                BenefitNotes.Add("Vision", obj.GetValue(tempCells, formats, doc, 256));
        //                BenefitNotes.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 257));
        //                BenefitNotes.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 258));
        //                BenefitNotes.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 259));
        //                BenefitNotes.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 260));
        //                BenefitNotes.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 261));
        //                BenefitNotes.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 262));

        //                DateTime CheckDate;

        //                #region Family 1

        //                Family family1 = new Family();
        //                var famileMember1DOB = obj.GetValue(tempCells, formats, doc, 52);

        //                if (DateTime.TryParse(famileMember1DOB, out CheckDate))
        //                {
        //                    family1.CaseNumId = oCase.CaseID;
        //                    family1.Gender = obj.GetValue(tempCells, formats, doc, 51).Encrypt();
        //                    family1.DOB = obj.GetValue(tempCells, formats, doc, 52).Encrypt();
        //                    family1.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 52)), System.DateTime.Now).ToString();
        //                    family1.Createdby = oCase.ModifiedBy;
        //                    family1.CreatedDateTime = oApplicant.CreatedDateTime;
        //                    family1.IsPrimary = obj.GetValue(tempCells, formats, doc, 50) == "Primary" ? true : false;
        //                    family1.Smoking = obj.GetValue(tempCells, formats, doc, 53) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 53)) ? "false" : "true";
        //                    family1.TotalMedicalUsage = "";
        //                    family1.BenefitUserDetails = null;
        //                    family1.Criticalillnesses = null;
        //                    DB.Families.Add(family1);
        //                    DB.SaveChanges();

        //                    Dictionary<string, string> BenefitUser1 = new Dictionary<string, string>();
        //                    BenefitUser1.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 54));
        //                    BenefitUser1.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 55));
        //                    BenefitUser1.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 56));
        //                    BenefitUser1.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 57));
        //                    BenefitUser1.Add("Counseling", obj.GetValue(tempCells, formats, doc, 58));
        //                    BenefitUser1.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 59));
        //                    BenefitUser1.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 60));
        //                    BenefitUser1.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 61));
        //                    BenefitUser1.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 62));
        //                    BenefitUser1.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 63));
        //                    BenefitUser1.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 64));
        //                    BenefitUser1.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 65));
        //                    BenefitUser1.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 66));
        //                    BenefitUser1.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 67));
        //                    BenefitUser1.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 68));
        //                    BenefitUser1.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 69));
        //                    BenefitUser1.Add("Dental", obj.GetValue(tempCells, formats, doc, 70));
        //                    BenefitUser1.Add("Maternity", obj.GetValue(tempCells, formats, doc, 71));
        //                    BenefitUser1.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 72));
        //                    BenefitUser1.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 73));
        //                    BenefitUser1.Add("Vision", obj.GetValue(tempCells, formats, doc, 74));
        //                    BenefitUser1.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 75));
        //                    BenefitUser1.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 76));
        //                    BenefitUser1.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 77));
        //                    BenefitUser1.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 78));
        //                    BenefitUser1.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 79));
        //                    BenefitUser1.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 80));

        //                    foreach (var BenefitUser in BenefitUser1)
        //                    {
        //                        if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                        {
        //                            var BenefitName = BenefitUser.Key;
        //                            //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                            BenefitUserDetail oBenenfitUses = new BenefitUserDetail();
        //                            oBenenfitUses.UsageCost = Convert.ToDecimal(BenefitCost[BenefitName]).ToString();
        //                            oBenenfitUses.UsageQty = BenefitUser.Value.ToString();
        //                            if (DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).Count() > 0)
        //                            {
        //                                oBenenfitUses.MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID;
        //                            }
        //                            else
        //                            {
        //                                dbContextTransaction.Rollback();
        //                                ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t Family Member 1 \t Benefit Name not found : " + BenefitName;
        //                                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                                {
        //                                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                else
        //                                {
        //                                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                continue;
        //                            }
        //                            oBenenfitUses.UsageNotes = !string.IsNullOrEmpty(BenefitNotes[BenefitName]) ? BenefitNotes[BenefitName].ToString().Encrypt() : null;
        //                            oBenenfitUses.FamilyID = family1.FamilyID;
        //                            oBenenfitUses.Createdby = 18;
        //                            oBenenfitUses.CreatedDateTime = oApplicant.CreatedDateTime;

        //                            DB.BenefitUserDetails.Add(oBenenfitUses);
        //                            DB.SaveChanges();
        //                            oBenenfitUses = null;
        //                        }
        //                    }
        //                }
        //                #endregion

        //                #region Family 2

        //                Family family2 = new Family();
        //                var famileMember2DOB = obj.GetValue(tempCells, formats, doc, 83);

        //                if (DateTime.TryParse(famileMember2DOB, out CheckDate))
        //                {
        //                    family2.CaseNumId = oCase.CaseID;
        //                    family2.Gender = obj.GetValue(tempCells, formats, doc, 82).Encrypt();
        //                    family2.DOB = obj.GetValue(tempCells, formats, doc, 83).Encrypt();
        //                    family2.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 83)), System.DateTime.Now).ToString();
        //                    family2.Createdby = oCase.ModifiedBy;
        //                    family2.CreatedDateTime = oApplicant.CreatedDateTime;
        //                    family2.IsPrimary = obj.GetValue(tempCells, formats, doc, 81) == "Primary" ? true : false;
        //                    family2.Smoking = obj.GetValue(tempCells, formats, doc, 84) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                    family2.TotalMedicalUsage = "";
        //                    family2.BenefitUserDetails = null;
        //                    family2.Criticalillnesses = null;
        //                    DB.Families.Add(family2);
        //                    DB.SaveChanges();

        //                    Dictionary<string, string> BenefitUser2 = new Dictionary<string, string>();
        //                    BenefitUser2.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 85));
        //                    BenefitUser2.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 86));
        //                    BenefitUser2.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 87));
        //                    BenefitUser2.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 88));
        //                    BenefitUser2.Add("Counseling", obj.GetValue(tempCells, formats, doc, 89));
        //                    BenefitUser2.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 90));
        //                    BenefitUser2.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 91));
        //                    BenefitUser2.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 92));
        //                    BenefitUser2.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 93));
        //                    BenefitUser2.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 94));
        //                    BenefitUser2.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 95));
        //                    BenefitUser2.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 96));
        //                    BenefitUser2.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 97));
        //                    BenefitUser2.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 98));
        //                    BenefitUser2.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 99));
        //                    BenefitUser2.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 100));
        //                    BenefitUser2.Add("Dental", obj.GetValue(tempCells, formats, doc, 101));
        //                    BenefitUser2.Add("Maternity", obj.GetValue(tempCells, formats, doc, 102));
        //                    BenefitUser2.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 103));
        //                    BenefitUser2.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 104));
        //                    BenefitUser2.Add("Vision", obj.GetValue(tempCells, formats, doc, 105));
        //                    BenefitUser2.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 106));
        //                    BenefitUser2.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 107));
        //                    BenefitUser2.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 108));
        //                    BenefitUser2.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 109));
        //                    BenefitUser2.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 110));
        //                    BenefitUser2.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 111));

        //                    foreach (var BenefitUser in BenefitUser2)
        //                    {
        //                        if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                        {
        //                            var BenefitName = BenefitUser.Key;
        //                            //DataRow BenefitNotes = ds.Tables["BenefitNotes"].Select("CaseNumber = " + CaseNumber + " and BenefitName='" + BenefitName + "'").First();

        //                            BenefitUserDetail oBenenfitUses = new BenefitUserDetail();
        //                            oBenenfitUses.UsageCost = Convert.ToDecimal(BenefitCost[BenefitName]).ToString();
        //                            oBenenfitUses.UsageQty = BenefitUser.Value.ToString();
        //                            if (DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).Count() > 0)
        //                            {
        //                                oBenenfitUses.MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID;
        //                            }
        //                            else
        //                            {
        //                                dbContextTransaction.Rollback();
        //                                ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t Family Member 2 \t Benefit Name not found : " + BenefitName;
        //                                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                                {
        //                                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                else
        //                                {
        //                                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                continue;
        //                            }
        //                            oBenenfitUses.UsageNotes = !string.IsNullOrEmpty(BenefitNotes[BenefitName]) ? BenefitNotes[BenefitName].ToString().Encrypt() : null;
        //                            oBenenfitUses.FamilyID = family2.FamilyID;
        //                            oBenenfitUses.Createdby = 18;
        //                            oBenenfitUses.CreatedDateTime = oApplicant.CreatedDateTime;

        //                            DB.BenefitUserDetails.Add(oBenenfitUses);
        //                            DB.SaveChanges();
        //                            oBenenfitUses = null;
        //                        }
        //                    }
        //                }
        //                #endregion

        //                #region Family 3

        //                Family family3 = new Family();
        //                var famileMember3DOB = obj.GetValue(tempCells, formats, doc, 114);

        //                if (DateTime.TryParse(famileMember3DOB, out CheckDate))
        //                {
        //                    family3.CaseNumId = oCase.CaseID;
        //                    family3.Gender = obj.GetValue(tempCells, formats, doc, 113).Encrypt();
        //                    family3.DOB = obj.GetValue(tempCells, formats, doc, 114).Encrypt();
        //                    family3.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 114)), System.DateTime.Now).ToString();
        //                    family3.Createdby = oCase.ModifiedBy;
        //                    family3.CreatedDateTime = oApplicant.CreatedDateTime;
        //                    family3.IsPrimary = obj.GetValue(tempCells, formats, doc, 112) == "Primary" ? true : false;
        //                    family3.Smoking = obj.GetValue(tempCells, formats, doc, 115) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                    family3.TotalMedicalUsage = "";
        //                    family3.BenefitUserDetails = null;
        //                    family3.Criticalillnesses = null;
        //                    DB.Families.Add(family3);
        //                    DB.SaveChanges();

        //                    Dictionary<string, string> BenefitUser3 = new Dictionary<string, string>();
        //                    BenefitUser3.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 116));
        //                    BenefitUser3.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 117));
        //                    BenefitUser3.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 118));
        //                    BenefitUser3.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 119));
        //                    BenefitUser3.Add("Counseling", obj.GetValue(tempCells, formats, doc, 120));
        //                    BenefitUser3.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 121));
        //                    BenefitUser3.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 122));
        //                    BenefitUser3.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 123));
        //                    BenefitUser3.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 124));
        //                    BenefitUser3.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 125));
        //                    BenefitUser3.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 126));
        //                    BenefitUser3.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 127));
        //                    BenefitUser3.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 128));
        //                    BenefitUser3.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 129));
        //                    BenefitUser3.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 130));
        //                    BenefitUser3.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 131));
        //                    BenefitUser3.Add("Dental", obj.GetValue(tempCells, formats, doc, 132));
        //                    BenefitUser3.Add("Maternity", obj.GetValue(tempCells, formats, doc, 133));
        //                    BenefitUser3.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 134));
        //                    BenefitUser3.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 135));
        //                    BenefitUser3.Add("Vision", obj.GetValue(tempCells, formats, doc, 136));
        //                    BenefitUser3.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 137));
        //                    BenefitUser3.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 138));
        //                    BenefitUser3.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 139));
        //                    BenefitUser3.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 140));
        //                    BenefitUser3.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 141));
        //                    BenefitUser3.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 142));

        //                    foreach (var BenefitUser in BenefitUser3)
        //                    {
        //                        if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                        {
        //                            var BenefitName = BenefitUser.Key;

        //                            BenefitUserDetail oBenenfitUses = new BenefitUserDetail();
        //                            oBenenfitUses.UsageCost = Convert.ToDecimal(BenefitCost[BenefitName]).ToString();
        //                            oBenenfitUses.UsageQty = BenefitUser.Value.ToString();
        //                            if (DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).Count() > 0)
        //                            {
        //                                oBenenfitUses.MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID;
        //                            }
        //                            else
        //                            {
        //                                dbContextTransaction.Rollback();
        //                                ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t Family Member 3 \t Benefit Name not found : " + BenefitName;
        //                                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                                {
        //                                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                else
        //                                {
        //                                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                continue;
        //                            }
        //                            oBenenfitUses.UsageNotes = !string.IsNullOrEmpty(BenefitNotes[BenefitName]) ? BenefitNotes[BenefitName].ToString().Encrypt() : null;
        //                            oBenenfitUses.FamilyID = family3.FamilyID;
        //                            oBenenfitUses.Createdby = 18;
        //                            oBenenfitUses.CreatedDateTime = oApplicant.CreatedDateTime;

        //                            DB.BenefitUserDetails.Add(oBenenfitUses);
        //                            DB.SaveChanges();
        //                            oBenenfitUses = null;
        //                        }
        //                    }
        //                }
        //                #endregion

        //                #region Family 4

        //                Family family4 = new Family();
        //                var famileMember4DOB = obj.GetValue(tempCells, formats, doc, 145);
        //                if (DateTime.TryParse(famileMember4DOB, out CheckDate))
        //                {
        //                    family4.CaseNumId = oCase.CaseID;
        //                    family4.Gender = obj.GetValue(tempCells, formats, doc, 144).Encrypt();
        //                    family4.DOB = obj.GetValue(tempCells, formats, doc, 145).Encrypt();
        //                    family4.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 145)), System.DateTime.Now).ToString();
        //                    family4.Createdby = oCase.ModifiedBy;
        //                    family4.CreatedDateTime = oApplicant.CreatedDateTime;
        //                    family4.IsPrimary = obj.GetValue(tempCells, formats, doc, 143) == "Primary" ? true : false;
        //                    family4.Smoking = obj.GetValue(tempCells, formats, doc, 146) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                    family4.TotalMedicalUsage = "";
        //                    family4.BenefitUserDetails = null;
        //                    family4.Criticalillnesses = null;
        //                    DB.Families.Add(family4);
        //                    DB.SaveChanges();

        //                    Dictionary<string, string> BenefitUser4 = new Dictionary<string, string>();
        //                    BenefitUser4.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 147));
        //                    BenefitUser4.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 148));
        //                    BenefitUser4.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 149));
        //                    BenefitUser4.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 150));
        //                    BenefitUser4.Add("Counseling", obj.GetValue(tempCells, formats, doc, 151));
        //                    BenefitUser4.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 152));
        //                    BenefitUser4.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 153));
        //                    BenefitUser4.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 154));
        //                    BenefitUser4.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 155));
        //                    BenefitUser4.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 156));
        //                    BenefitUser4.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 157));
        //                    BenefitUser4.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 158));
        //                    BenefitUser4.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 159));
        //                    BenefitUser4.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 160));
        //                    BenefitUser4.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 161));
        //                    BenefitUser4.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 162));
        //                    BenefitUser4.Add("Dental", obj.GetValue(tempCells, formats, doc, 163));
        //                    BenefitUser4.Add("Maternity", obj.GetValue(tempCells, formats, doc, 164));
        //                    BenefitUser4.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 165));
        //                    BenefitUser4.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 166));
        //                    BenefitUser4.Add("Vision", obj.GetValue(tempCells, formats, doc, 167));
        //                    BenefitUser4.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 168));
        //                    BenefitUser4.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 169));
        //                    BenefitUser4.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 170));
        //                    BenefitUser4.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 171));
        //                    BenefitUser4.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 172));
        //                    BenefitUser4.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 173));

        //                    foreach (var BenefitUser in BenefitUser4)
        //                    {
        //                        if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                        {
        //                            var BenefitName = BenefitUser.Key;

        //                            BenefitUserDetail oBenenfitUses = new BenefitUserDetail();
        //                            oBenenfitUses.UsageCost = Convert.ToDecimal(BenefitCost[BenefitName]).ToString();
        //                            oBenenfitUses.UsageQty = BenefitUser.Value.ToString();
        //                            if (DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).Count() > 0)
        //                            {
        //                                oBenenfitUses.MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID;
        //                            }
        //                            else
        //                            {
        //                                dbContextTransaction.Rollback();
        //                                ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t Family Member 3 \t Benefit Name not found : " + BenefitName;
        //                                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                                {
        //                                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                else
        //                                {
        //                                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                continue;
        //                            }
        //                            oBenenfitUses.UsageNotes = !string.IsNullOrEmpty(BenefitNotes[BenefitName]) ? BenefitNotes[BenefitName].ToString().Encrypt() : null;
        //                            oBenenfitUses.FamilyID = family4.FamilyID;
        //                            oBenenfitUses.Createdby = 18;
        //                            oBenenfitUses.CreatedDateTime = oApplicant.CreatedDateTime;

        //                            DB.BenefitUserDetails.Add(oBenenfitUses);
        //                            DB.SaveChanges();
        //                            oBenenfitUses = null;
        //                        }
        //                    }
        //                }
        //                #endregion

        //                #region Family 5

        //                Family family5 = new Family();
        //                var famileMember5DOB = obj.GetValue(tempCells, formats, doc, 176);

        //                if (DateTime.TryParse(famileMember5DOB, out CheckDate))
        //                {
        //                    family5.CaseNumId = oCase.CaseID;
        //                    family5.Gender = obj.GetValue(tempCells, formats, doc, 175).Encrypt();
        //                    family5.DOB = obj.GetValue(tempCells, formats, doc, 176).Encrypt();
        //                    family5.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 176)), System.DateTime.Now).ToString();
        //                    family5.Createdby = oCase.ModifiedBy;
        //                    family5.CreatedDateTime = oApplicant.CreatedDateTime;
        //                    family5.IsPrimary = obj.GetValue(tempCells, formats, doc, 174) == "Primary" ? true : false;
        //                    family5.Smoking = obj.GetValue(tempCells, formats, doc, 177) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                    family5.TotalMedicalUsage = "";
        //                    family5.BenefitUserDetails = null;
        //                    family5.Criticalillnesses = null;
        //                    DB.Families.Add(family5);
        //                    DB.SaveChanges();

        //                    Dictionary<string, string> BenefitUser5 = new Dictionary<string, string>();
        //                    BenefitUser5.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 178));
        //                    BenefitUser5.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 179));
        //                    BenefitUser5.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 180));
        //                    BenefitUser5.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 181));
        //                    BenefitUser5.Add("Counseling", obj.GetValue(tempCells, formats, doc, 182));
        //                    BenefitUser5.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 183));
        //                    BenefitUser5.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 184));
        //                    BenefitUser5.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 185));
        //                    BenefitUser5.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 186));
        //                    BenefitUser5.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 187));
        //                    BenefitUser5.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 188));
        //                    BenefitUser5.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 189));
        //                    BenefitUser5.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 190));
        //                    BenefitUser5.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 191));
        //                    BenefitUser5.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 192));
        //                    BenefitUser5.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 193));
        //                    BenefitUser5.Add("Dental", obj.GetValue(tempCells, formats, doc, 194));
        //                    BenefitUser5.Add("Maternity", obj.GetValue(tempCells, formats, doc, 195));
        //                    BenefitUser5.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 196));
        //                    BenefitUser5.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 197));
        //                    BenefitUser5.Add("Vision", obj.GetValue(tempCells, formats, doc, 198));
        //                    BenefitUser5.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 199));
        //                    BenefitUser5.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 200));
        //                    BenefitUser5.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 201));
        //                    BenefitUser5.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 202));
        //                    BenefitUser5.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 203));
        //                    BenefitUser5.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 204));

        //                    foreach (var BenefitUser in BenefitUser5)
        //                    {
        //                        if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                        {
        //                            var BenefitName = BenefitUser.Key;

        //                            BenefitUserDetail oBenenfitUses = new BenefitUserDetail();
        //                            oBenenfitUses.UsageCost = Convert.ToDecimal(BenefitCost[BenefitName]).ToString();
        //                            oBenenfitUses.UsageQty = BenefitUser.Value.ToString();
        //                            if (DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).Count() > 0)
        //                            {
        //                                oBenenfitUses.MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID;
        //                            }
        //                            else
        //                            {
        //                                dbContextTransaction.Rollback();
        //                                ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t Family Member 3 \t Benefit Name not found : " + BenefitName;
        //                                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                                {
        //                                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                else
        //                                {
        //                                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                continue;
        //                            }
        //                            oBenenfitUses.UsageNotes = !string.IsNullOrEmpty(BenefitNotes[BenefitName]) ? BenefitNotes[BenefitName].ToString().Encrypt() : null;
        //                            oBenenfitUses.FamilyID = family5.FamilyID;
        //                            oBenenfitUses.Createdby = 18;
        //                            oBenenfitUses.CreatedDateTime = oApplicant.CreatedDateTime;

        //                            DB.BenefitUserDetails.Add(oBenenfitUses);
        //                            DB.SaveChanges();
        //                            oBenenfitUses = null;
        //                        }
        //                    }
        //                }
        //                #endregion

        //                #region Family 6

        //                Family family6 = new Family();
        //                var famileMember6DOB = obj.GetValue(tempCells, formats, doc, 207);
        //                if (DateTime.TryParse(famileMember6DOB, out CheckDate))
        //                {
        //                    family6.CaseNumId = oCase.CaseID;
        //                    family6.Gender = obj.GetValue(tempCells, formats, doc, 206).Encrypt();
        //                    family6.DOB = obj.GetValue(tempCells, formats, doc, 207).Encrypt();
        //                    family6.Age = DifferenceTotalYears(Convert.ToDateTime(obj.GetValue(tempCells, formats, doc, 207)), System.DateTime.Now).ToString();
        //                    family6.Createdby = oCase.ModifiedBy;
        //                    family6.CreatedDateTime = oApplicant.CreatedDateTime;
        //                    family6.IsPrimary = obj.GetValue(tempCells, formats, doc, 205) == "Primary" ? true : false;
        //                    family6.Smoking = obj.GetValue(tempCells, formats, doc, 208) == "N" || string.IsNullOrEmpty(obj.GetValue(tempCells, formats, doc, 84)) ? "false" : "true";
        //                    family6.TotalMedicalUsage = "";
        //                    family6.BenefitUserDetails = null;
        //                    family6.Criticalillnesses = null;
        //                    DB.Families.Add(family6);
        //                    DB.SaveChanges();

        //                    Dictionary<string, string> BenefitUser6 = new Dictionary<string, string>();
        //                    BenefitUser6.Add("Emergency Room", obj.GetValue(tempCells, formats, doc, 209));
        //                    BenefitUser6.Add("Hospital Inpatient Admission", obj.GetValue(tempCells, formats, doc, 210));
        //                    BenefitUser6.Add("Office Visit PCP (Primary Care Provider)", obj.GetValue(tempCells, formats, doc, 211));
        //                    BenefitUser6.Add("Office Visit Specialist", obj.GetValue(tempCells, formats, doc, 212));
        //                    BenefitUser6.Add("Counseling", obj.GetValue(tempCells, formats, doc, 213));
        //                    BenefitUser6.Add("Substance / Recovery", obj.GetValue(tempCells, formats, doc, 214));
        //                    BenefitUser6.Add("Outpatient Adv Diag Tests (MRI, CT scan)", obj.GetValue(tempCells, formats, doc, 215));
        //                    BenefitUser6.Add("Speech Therapy", obj.GetValue(tempCells, formats, doc, 216));
        //                    BenefitUser6.Add("Physical Therapy (annual visits capped)", obj.GetValue(tempCells, formats, doc, 217));
        //                    BenefitUser6.Add("Preventive Care", obj.GetValue(tempCells, formats, doc, 218));
        //                    BenefitUser6.Add("Laboratory Outpatient / Professional Svcs", obj.GetValue(tempCells, formats, doc, 219));
        //                    BenefitUser6.Add("Outpatient Diagnostic Tests (xray, EKG)", obj.GetValue(tempCells, formats, doc, 220));
        //                    BenefitUser6.Add("Skilled Nursing Services (misc 2)", obj.GetValue(tempCells, formats, doc, 221));
        //                    BenefitUser6.Add("Outpatient Facility Fee (Ambul Surgery Ctr)", obj.GetValue(tempCells, formats, doc, 222));
        //                    BenefitUser6.Add("Outpatient Surgery - Hospital Facility", obj.GetValue(tempCells, formats, doc, 223));
        //                    BenefitUser6.Add("Urgent Care", obj.GetValue(tempCells, formats, doc, 224));
        //                    BenefitUser6.Add("Dental", obj.GetValue(tempCells, formats, doc, 225));
        //                    BenefitUser6.Add("Maternity", obj.GetValue(tempCells, formats, doc, 226));
        //                    BenefitUser6.Add("Chiropractic (annual visits capped)", obj.GetValue(tempCells, formats, doc, 227));
        //                    BenefitUser6.Add("Accupuncture", obj.GetValue(tempCells, formats, doc, 228));
        //                    BenefitUser6.Add("Vision", obj.GetValue(tempCells, formats, doc, 229));
        //                    BenefitUser6.Add("Hearing / Speech exams", obj.GetValue(tempCells, formats, doc, 230));
        //                    BenefitUser6.Add("Rx Tier 1 - Generic", obj.GetValue(tempCells, formats, doc, 231));
        //                    BenefitUser6.Add("Rx Tier 2 - Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 232));
        //                    BenefitUser6.Add("Rx Tier 3 - Non-Preferred Brand Name", obj.GetValue(tempCells, formats, doc, 233));
        //                    BenefitUser6.Add("Rx Tier 4 - Specialty Drugs", obj.GetValue(tempCells, formats, doc, 234));
        //                    BenefitUser6.Add("Rx Tier 5 - Other", obj.GetValue(tempCells, formats, doc, 235));

        //                    foreach (var BenefitUser in BenefitUser6)
        //                    {
        //                        if (!string.IsNullOrEmpty(BenefitUser.Value))
        //                        {
        //                            var BenefitName = BenefitUser.Key;

        //                            BenefitUserDetail oBenenfitUses = new BenefitUserDetail();
        //                            oBenenfitUses.UsageCost = Convert.ToDecimal(BenefitCost[BenefitName]).ToString();
        //                            oBenenfitUses.UsageQty = BenefitUser.Value.ToString();
        //                            if (DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).Count() > 0)
        //                            {
        //                                oBenenfitUses.MHMMappingBenefitId = DB.MHMCommonBenefitsMsts.Where(t => t.MHMBenefitName == BenefitName).First().MHMBenefitID;
        //                            }
        //                            else
        //                            {
        //                                dbContextTransaction.Rollback();
        //                                ErrorMsg = Environment.NewLine + "Case No : " + oCase.CaseTitle + "\t Family Member 3 \t Benefit Name not found : " + BenefitName;
        //                                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                                {
        //                                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                else
        //                                {
        //                                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                                }
        //                                continue;
        //                            }
        //                            oBenenfitUses.UsageNotes = !string.IsNullOrEmpty(BenefitNotes[BenefitName]) ? BenefitNotes[BenefitName].ToString().Encrypt() : null;
        //                            oBenenfitUses.FamilyID = family6.FamilyID;
        //                            oBenenfitUses.Createdby = 18;
        //                            oBenenfitUses.CreatedDateTime = oApplicant.CreatedDateTime;

        //                            DB.BenefitUserDetails.Add(oBenenfitUses);
        //                            DB.SaveChanges();
        //                            oBenenfitUses = null;
        //                        }
        //                    }
        //                }
        //                #endregion

        //                //#region CasePlanResult 1

        //                //CasePlanResult CasePlanResult1 = new CasePlanResult();

        //                //CasePlanResult1.CaseId = oCase.CaseID;
        //                //CasePlanResult1.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 263);
        //                //CasePlanResult1.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 264);
        //                //CasePlanResult1.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 265);
        //                //CasePlanResult1.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 266);
        //                //CasePlanResult1.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 267);
        //                ////MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //                //CasePlanResult1.Copays = obj.GetValue(tempCells, formats, doc, 268);
        //                //CasePlanResult1.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 269);
        //                //CasePlanResult1.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 270);
        //                ////ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //                //CasePlanResult1.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //                //CasePlanResult1.Medical = obj.GetValue(tempCells, formats, doc, 272);
        //                //CasePlanResult1.TotalPaid = obj.GetValue(tempCells, formats, doc, 273);
        //                //CasePlanResult1.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 274);
        //                //CasePlanResult1.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 275).Substring(0, obj.GetValue(tempCells, formats, doc, 275).IndexOf('/')).Replace("$", "");
        //                //CasePlanResult1.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 275).Substring(obj.GetValue(tempCells, formats, doc, 275).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult1.OPLSingle = obj.GetValue(tempCells, formats, doc, 276).Substring(obj.GetValue(tempCells, formats, doc, 276).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult1.OPLFamily = obj.GetValue(tempCells, formats, doc, 276).Substring(obj.GetValue(tempCells, formats, doc, 276).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult1.Coinsurance = obj.GetValue(tempCells, formats, doc, 277);
        //                //CasePlanResult1.WorstCase = obj.GetValue(tempCells, formats, doc, 278);
        //                //CasePlanResult1.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 279);
        //                //CasePlanResult1.CreatedDateTime = oApplicant.CreatedDateTime;


        //                //DB.CasePlanResults.Add(CasePlanResult1);
        //                //DB.SaveChanges();

        //                //#endregion

        //                //#region CasePlanResult 2

        //                //CasePlanResult CasePlanResult2 = new CasePlanResult();

        //                //CasePlanResult2.CaseId = oCase.CaseID;
        //                //CasePlanResult2.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 280);
        //                //CasePlanResult2.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 281);
        //                //CasePlanResult2.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 282);
        //                //CasePlanResult2.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 283);
        //                //CasePlanResult2.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 284);
        //                ////MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //                //CasePlanResult2.Copays = obj.GetValue(tempCells, formats, doc, 285);
        //                //CasePlanResult2.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 286);
        //                //CasePlanResult2.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 287);
        //                ////ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //                //CasePlanResult2.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 288);
        //                //CasePlanResult2.Medical = obj.GetValue(tempCells, formats, doc, 289);
        //                //CasePlanResult2.TotalPaid = obj.GetValue(tempCells, formats, doc, 290);
        //                //CasePlanResult2.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 291);
        //                //CasePlanResult2.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 292).Substring(0, obj.GetValue(tempCells, formats, doc, 292).IndexOf('/')).Replace("$", "");
        //                //CasePlanResult2.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 292).Substring(obj.GetValue(tempCells, formats, doc, 292).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult2.OPLSingle = obj.GetValue(tempCells, formats, doc, 293).Substring(obj.GetValue(tempCells, formats, doc, 293).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult2.OPLFamily = obj.GetValue(tempCells, formats, doc, 293).Substring(obj.GetValue(tempCells, formats, doc, 293).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult2.Coinsurance = obj.GetValue(tempCells, formats, doc, 294);
        //                //CasePlanResult2.WorstCase = obj.GetValue(tempCells, formats, doc, 295);
        //                //CasePlanResult2.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 296);
        //                //CasePlanResult2.CreatedDateTime = oApplicant.CreatedDateTime;


        //                //DB.CasePlanResults.Add(CasePlanResult2);
        //                //DB.SaveChanges();

        //                //#endregion

        //                //#region CasePlanResult 3

        //                //CasePlanResult CasePlanResult3 = new CasePlanResult();

        //                //CasePlanResult3.CaseId = oCase.CaseID;
        //                //CasePlanResult3.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 297);
        //                //CasePlanResult3.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 298);
        //                //CasePlanResult3.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 299);
        //                //CasePlanResult3.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 300);
        //                //CasePlanResult3.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 301);
        //                ////MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //                //CasePlanResult3.Copays = obj.GetValue(tempCells, formats, doc, 302);
        //                //CasePlanResult3.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 303);
        //                //CasePlanResult3.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 304);
        //                ////ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //                //CasePlanResult3.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 305);
        //                //CasePlanResult3.Medical = obj.GetValue(tempCells, formats, doc, 306);
        //                //CasePlanResult3.TotalPaid = obj.GetValue(tempCells, formats, doc, 307);
        //                //CasePlanResult3.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 308);
        //                //CasePlanResult3.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 309).Substring(0, obj.GetValue(tempCells, formats, doc, 309).IndexOf('/')).Replace("$", "");
        //                //CasePlanResult3.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 309).Substring(obj.GetValue(tempCells, formats, doc, 309).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult3.OPLSingle = obj.GetValue(tempCells, formats, doc, 310).Substring(obj.GetValue(tempCells, formats, doc, 310).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult3.OPLFamily = obj.GetValue(tempCells, formats, doc, 310).Substring(obj.GetValue(tempCells, formats, doc, 310).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult3.Coinsurance = obj.GetValue(tempCells, formats, doc, 311);
        //                //CasePlanResult3.WorstCase = obj.GetValue(tempCells, formats, doc, 312);
        //                //CasePlanResult3.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 313);
        //                //CasePlanResult3.CreatedDateTime = oApplicant.CreatedDateTime;


        //                //DB.CasePlanResults.Add(CasePlanResult3);
        //                //DB.SaveChanges();

        //                //#endregion

        //                //#region CasePlanResult 4

        //                //CasePlanResult CasePlanResult4 = new CasePlanResult();

        //                //CasePlanResult4.CaseId = oCase.CaseID;
        //                //CasePlanResult4.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 314);
        //                //CasePlanResult4.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 315);
        //                //CasePlanResult4.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 316);
        //                //CasePlanResult4.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 317);
        //                //CasePlanResult4.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 318);
        //                ////MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //                //CasePlanResult4.Copays = obj.GetValue(tempCells, formats, doc, 319);
        //                //CasePlanResult4.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 320);
        //                //CasePlanResult4.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 321);
        //                ////ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //                //CasePlanResult4.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 322);
        //                //CasePlanResult4.Medical = obj.GetValue(tempCells, formats, doc, 323);
        //                //CasePlanResult4.TotalPaid = obj.GetValue(tempCells, formats, doc, 324);
        //                //CasePlanResult4.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 325);
        //                //CasePlanResult4.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 326).Substring(0, obj.GetValue(tempCells, formats, doc, 326).IndexOf('/')).Replace("$", "");
        //                //CasePlanResult4.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 326).Substring(obj.GetValue(tempCells, formats, doc, 326).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult4.OPLSingle = obj.GetValue(tempCells, formats, doc, 327).Substring(obj.GetValue(tempCells, formats, doc, 327).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult4.OPLFamily = obj.GetValue(tempCells, formats, doc, 327).Substring(obj.GetValue(tempCells, formats, doc, 327).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult4.Coinsurance = obj.GetValue(tempCells, formats, doc, 328);
        //                //CasePlanResult4.WorstCase = obj.GetValue(tempCells, formats, doc, 329);
        //                //CasePlanResult4.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 330);
        //                //CasePlanResult4.CreatedDateTime = oApplicant.CreatedDateTime;


        //                //DB.CasePlanResults.Add(CasePlanResult4);
        //                //DB.SaveChanges();

        //                //#endregion

        //                //#region CasePlanResult 5

        //                //CasePlanResult CasePlanResult5 = new CasePlanResult();

        //                //CasePlanResult5.CaseId = oCase.CaseID;
        //                //CasePlanResult5.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 331);
        //                //CasePlanResult5.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 332);
        //                //CasePlanResult5.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 333);
        //                //CasePlanResult5.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 334);
        //                //CasePlanResult5.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 335);
        //                ////MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //                //CasePlanResult5.Copays = obj.GetValue(tempCells, formats, doc, 336);
        //                //CasePlanResult5.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 337);
        //                //CasePlanResult5.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 338);
        //                ////ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //                //CasePlanResult5.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 339);
        //                //CasePlanResult5.Medical = obj.GetValue(tempCells, formats, doc, 340);
        //                //CasePlanResult5.TotalPaid = obj.GetValue(tempCells, formats, doc, 341);
        //                //CasePlanResult5.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 342);
        //                //CasePlanResult5.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 343).Substring(0, obj.GetValue(tempCells, formats, doc, 343).IndexOf('/')).Replace("$", "");
        //                //CasePlanResult5.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 343).Substring(obj.GetValue(tempCells, formats, doc, 343).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult5.OPLSingle = obj.GetValue(tempCells, formats, doc, 344).Substring(obj.GetValue(tempCells, formats, doc, 344).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult5.OPLFamily = obj.GetValue(tempCells, formats, doc, 344).Substring(obj.GetValue(tempCells, formats, doc, 344).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult5.Coinsurance = obj.GetValue(tempCells, formats, doc, 345);
        //                //CasePlanResult5.WorstCase = obj.GetValue(tempCells, formats, doc, 346);
        //                //CasePlanResult5.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 347);
        //                //CasePlanResult5.CreatedDateTime = oApplicant.CreatedDateTime;


        //                //DB.CasePlanResults.Add(CasePlanResult5);
        //                //DB.SaveChanges();

        //                //#endregion

        //                //#region CasePlanResult 6

        //                //CasePlanResult CasePlanResult6 = new CasePlanResult();

        //                //CasePlanResult6.CaseId = oCase.CaseID;
        //                //CasePlanResult6.GovtPlanNumber = obj.GetValue(tempCells, formats, doc, 348);
        //                //CasePlanResult6.PlanIdIndiv1 = obj.GetValue(tempCells, formats, doc, 349);
        //                //CasePlanResult6.GrossAnnualPremium = obj.GetValue(tempCells, formats, doc, 350);
        //                //CasePlanResult6.FederalSubsidy = obj.GetValue(tempCells, formats, doc, 351);
        //                //CasePlanResult6.NetAnnualPremium = obj.GetValue(tempCells, formats, doc, 352);
        //                ////MonthlyPremium = obj.GetValue(tempCells, formats, doc, 268);
        //                //CasePlanResult6.Copays = obj.GetValue(tempCells, formats, doc, 353);
        //                //CasePlanResult6.PaymentsToDeductibleLimit = obj.GetValue(tempCells, formats, doc, 354);
        //                //CasePlanResult6.CoinsuranceToOutOfPocketLimit = obj.GetValue(tempCells, formats, doc, 355);
        //                ////ContributedToYourHSAAccount = obj.GetValue(tempCells, formats, doc, 271);
        //                //CasePlanResult6.TaxSavingFromHSAAccount = obj.GetValue(tempCells, formats, doc, 356);
        //                //CasePlanResult6.Medical = obj.GetValue(tempCells, formats, doc, 357);
        //                //CasePlanResult6.TotalPaid = obj.GetValue(tempCells, formats, doc, 358);
        //                //CasePlanResult6.PaymentsByInsuranceCo = obj.GetValue(tempCells, formats, doc, 359);
        //                //CasePlanResult6.DeductibleSingle = obj.GetValue(tempCells, formats, doc, 360).Substring(0, obj.GetValue(tempCells, formats, doc, 360).IndexOf('/')).Replace("$", "");
        //                //CasePlanResult6.DeductibleFamily = obj.GetValue(tempCells, formats, doc, 360).Substring(obj.GetValue(tempCells, formats, doc, 360).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult6.OPLSingle = obj.GetValue(tempCells, formats, doc, 361).Substring(obj.GetValue(tempCells, formats, doc, 361).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult6.OPLFamily = obj.GetValue(tempCells, formats, doc, 361).Substring(obj.GetValue(tempCells, formats, doc, 361).IndexOf('/') + 1).Replace("$", "");
        //                //CasePlanResult6.Coinsurance = obj.GetValue(tempCells, formats, doc, 362);
        //                //CasePlanResult6.WorstCase = obj.GetValue(tempCells, formats, doc, 363);
        //                //CasePlanResult6.MedicalNetwork = obj.GetValue(tempCells, formats, doc, 364);
        //                //CasePlanResult6.CreatedDateTime = oApplicant.CreatedDateTime;


        //                //DB.CasePlanResults.Add(CasePlanResult6);
        //                //DB.SaveChanges();

        //                //#endregion


        //                dbContextTransaction.Commit();

        //            }
        //            catch (DbEntityValidationException e)
        //            {
        //                string Error = "";
        //                foreach (var eve in e.EntityValidationErrors)
        //                {

        //                    Error = "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + ' ' + eve.Entry.State;
        //                    foreach (var ve in eve.ValidationErrors)
        //                    {
        //                        Error = "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + ' ' + ve.ErrorMessage;
        //                    }
        //                }
        //                ErrorMsg = Environment.NewLine + "Case No : " + CaseNo + "\t Error : " + Error;
        //                dbContextTransaction.Rollback();
        //                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                {
        //                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                }
        //                else
        //                {
        //                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ErrorMsg = Environment.NewLine + "Case No : " + CaseNo + "\t ErrorMessage " + ex.Message + "\t Inner Exception : " + ex.InnerException;
        //                dbContextTransaction.Rollback();
        //                if (File.Exists(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt"))
        //                {
        //                    System.IO.File.AppendAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                }
        //                else
        //                {
        //                    System.IO.File.WriteAllText(@"E:\mhm\MHM\MHM.Api\ErrorMsg.txt", ErrorMsg);
        //                }
        //            }
        //        }
        //    }
        //}

        [HttpGet]
        [Route("api/case/UpdateAllCases")]
        public HttpResponseMessage UpdateAllCases()
        {
            using (var DB = new MHMDal.Models.MHM())
            {
                Response oResponse = new Response();

                var cases = DB.Cases.ToList();

                foreach (var ocase in cases)
                {
                    var oCase = ocase;

                    using (var dbContextTransaction = DB.Database.BeginTransaction())
                    {
                        //Remove Families, BenefitUsesDetails, Criticalillness
                        var families = DB.Families.Where(r => r.CaseNumId == oCase.CaseID).ToList();
                        foreach (var item in families)
                        {
                            var itemFM = DB.Families.Where(r => r.FamilyID == item.FamilyID).First();
                            itemFM.Gender = GenerateEncryptedString.GetEncryptedString(itemFM.Gender.Decrypt());
                            itemFM.DOB = GenerateEncryptedString.GetEncryptedString(itemFM.DOB.Decrypt());
                            DB.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                }

                Dictionary<string, object> oRes = new Dictionary<string, object>();
                oRes.Add("Status", true);
                oRes.Add("Message", "Success");
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oRes);
                return response;
            }
        }

    }



    public class JsonResponse
    {
        public string ApplicantEmail { get; set; }
        public string AgentEmail { get; set; }
        public string CaseTitle { get; set; }
        public string ApplicantName { get; set; }
        public string AgentName { get; set; }
        public string AgentPhone { get; set; }
        public string Html { get; set; }
        public string JobNumber { get; set; }
        public string CaseId { get; set; }
        public string PlanTotalCostRange { get; set; }
        public string ModifiedBy { get; set; }

        public string EmployerHSAContribution { get; set; }
        public string EmployerPremiumContribution { get; set; }
        public string EmployerHRAReimbursement { get; set; }
        public string TotalEmployerContribution { get; set; }
        public string OptimalPlanName { get; set; }
    }

    public class CaseStatusParamter
    {
        public string Id { get; set; }
    }

}
