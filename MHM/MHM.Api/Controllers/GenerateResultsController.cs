using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MHMDal.Models;
using SynSecurity;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SelectPdf;
using MHM.Api.Helpers;
using System.Windows.Forms;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Drawing;
using System.Drawing;
using MHMBLL;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using MHM.Api.Models;

namespace MHM.Api.Controllers
{
    public class GenerateResultsController : ApiController
    {
        //MHMDal.Models.MHM DB;

        [HttpGet]
        [Route("api/GenerateResultsController/UpdateAllFinalNotSendCases")]
        public HttpResponseMessage UpdateAllFinalNotSendCases([FromUri]  List<int?> CaseStatusIds, string JobNumber, string JobRunStatus)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstCases = DB.Cases.Where(r => r.JobNumber == JobNumber && CaseStatusIds.Contains(r.StatusId)).ToList();
                    if (JobRunStatus != null)
                    {
                        lstCases = lstCases.Where(r => r.CaseJobRunStatus == JobRunStatus).ToList();
                    }

                    lstCases.ForEach(r => r.InProcessStatus = false);
                    DB.SaveChanges();

                    var Job = DB.JobMasters.Where(r => r.JobNumber == JobNumber).FirstOrDefault();
                    Job.JobRunStatus = Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.NULL);
                    DB.SaveChanges();

                    res.Add("Status", "true");
                    res.Add("Message", "Success");

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                catch (Exception ex)
                {
                    res.Add("Status", "false");
                    res.Add("Message", "Failed");

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, res);
                    return response;
                }
            }
        }

        [HttpGet]
        [Route("api/GenerateResultsController/GetAllFinalNotSendCases")]
        public HttpResponseMessage GetAllFinalNotSendCases([FromUri]  List<int?> CaseStatusIds, string JobNumber, string JobRunStatus)
        {
            using (var DB = new MHMDal.Models.MHM())
            {
                var lstCases = DB.Cases.Where(r => r.JobNumber == JobNumber && CaseStatusIds.Contains(r.StatusId) && r.InProcessStatus == false).ToList();
                if (JobRunStatus != null)
                {
                    lstCases = lstCases.Where(r => r.CaseJobRunStatus == JobRunStatus).ToList();
                }
                Dictionary<string, object> res = new Dictionary<string, object>();
                if (lstCases.Count() > 0)
                {
                    var oCase = lstCases.Take(1).FirstOrDefault();
                    oCase.InProcessStatus = true;
                    DB.SaveChanges();

                    oCase.Applicant.City = oCase.Applicant.City != null ? oCase.Applicant.City.Decrypt() : null;
                    oCase.Applicant.Email = oCase.Applicant.Email != null ? oCase.Applicant.Email.Decrypt() : null;
                    oCase.Applicant.FirstName = oCase.Applicant.FirstName != null ? oCase.Applicant.FirstName.Decrypt() : null;
                    oCase.Applicant.LastName = oCase.Applicant.LastName != null ? oCase.Applicant.LastName.Decrypt() : null;
                    oCase.Applicant.Mobile = oCase.Applicant.Mobile != null ? oCase.Applicant.Mobile.Decrypt() : null;
                    oCase.Applicant.Street = oCase.Applicant.Street != null ? oCase.Applicant.Street.Decrypt() : null;
                    oCase.Applicant.State = oCase.Applicant.State != null ? oCase.Applicant.State.Decrypt() : null;
                    oCase.Applicant.Zip = oCase.Applicant.Zip != null ? oCase.Applicant.Zip.Decrypt() : null;

                    oCase.Notes = oCase.Notes != null ? oCase.Notes.Decrypt() : null;

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
                    res.Add("TotalCount", lstCases.Count);
                }
                else
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("Case", "");
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                return response;
            }
        }

        [HttpPost]
        [Route("api/GenerateResultsController/GenerateResult")]
        //public HttpResponseMessage GenerateResult(string ZipCode, string CountyName, decimal Income, bool SubsidyStatus, int UsageCode, long IssuerId, int PlanID, long EmployerId, string JobNumber, int InsuranceTypeId, string BusinessYear, bool Welness, long ModifiedBy, JObject jObject, decimal HSAPercentage = 0, decimal TaxRate = 0, bool IsAmericanIndian = false, bool ResultStatus = false)
        public HttpResponseMessage GenerateResult(string ZipCode, string CountyName, decimal Income, bool SubsidyStatus, int UsageCode, long IssuerId, int PlanTypeID, long EmployerId, string JobNumber, string BusinessYear, bool Welness, int TierIntention, JObject jObject, DateTime? DedBalAvailDate, long ModifiedBy, decimal HSAPercentage = 0, decimal TaxRate = 0, bool IsAmericanIndian = false, bool ResultStatus = false, decimal DedBalAvailToRollOver = 0)
        {
            Response oResponse = new Response();
            string strCase = jObject["oCase"].ToString();
            Case oCase = (Case)JsonConvert.DeserializeObject(strCase, (typeof(Case)));

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    //string UserDatail = jObject["data"].ToString();
                    //string UsesDetail = jObject["UsesDetail"].ToString();
                    //List<FamilyMemberUsesList> lstFamilyMemberUses = (List<FamilyMemberUsesList>)JsonConvert.DeserializeObject(UsesDetail, (typeof(List<FamilyMemberUsesList>)));

                    //List<FamilyMemberList> fmlMemberList = (List<FamilyMemberList>)JsonConvert.DeserializeObject(UserDatail, (typeof(List<FamilyMemberList>)));  // new List<FamilyMemberList>(); 

                    List<FamilyMemberUsesList> lstFamilyMemberUses = new List<FamilyMemberUsesList>();
                    List<FamilyMemberList> fmlMemberList = new List<FamilyMemberList>();

                    int i = 1;
                    foreach (var item in oCase.Families)
                    {
                        fmlMemberList.Add(new FamilyMemberList() { Age = Convert.ToInt32(item.Age), SmokingStatus = Convert.ToBoolean(item.Smoking) });
                        List<BenefitDetails> BenefitUses = new List<BenefitDetails>();
                        foreach (var BenefitUserDetail in item.BenefitUserDetails)
                        {
                            BenefitUses.Add(new BenefitDetails() { BenefitId = BenefitUserDetail.MHMMappingBenefitId, UsageCost = Convert.ToDecimal(BenefitUserDetail.UsageCost) * Convert.ToInt32(BenefitUserDetail.UsageQty), UsageQty = Convert.ToInt32(BenefitUserDetail.UsageQty) });
                        }

                        lstFamilyMemberUses.Add(new FamilyMemberUsesList() { FamilyMemberNumber = i, BenefitUses = BenefitUses });
                        i++;
                    }


                    var objOptionSheet = new OptionSheetCalculation();


                    string SecondLowestPlanId = "", StateCode = "";
                    decimal HSALimit = 0, MaxEEHSA = 0;
                    decimal IndividualSubsidy = 0, ShopSubsidy = 0, FPL = 0;
                    int ACAPlanIdSub = 0, MemberRemoveChipEligibility = 0, MemberRemoveMedicaidEligibility = 0;
                    long RatingAreaId = 0;
                    var OrgionalFamilyMember = fmlMemberList.ToList();
                    string ProgID = Welness ? "B" : "A";

                    //List<CasePlanResult> data;

                    //if (String.IsNullOrEmpty(CountyName) || CountyName == "null")
                    //{
                    //    var Countylst = DB.qryZipCodeToRatingAreas.Where(m => m.Zip == ZipCode).Select(r => new { r.CountyName, r.RatingAreaID, r.StateCode, r.StateId, r.StateName, r.City });
                    //    if (Countylst.Count() > 0)
                    //    {
                    //        CountyName = Countylst.First().CountyName;
                    //    }
                    //}

                    new StateandRatingArea().GetStateCodeandRatingArea(ZipCode, CountyName, out RatingAreaId, out StateCode);
                    if (fmlMemberList.Count() > 0)
                        MaxEEHSA = new HSACalculation().CalculateAnnualHSA(UsageCode, fmlMemberList.First().Age, (decimal)HSAPercentage, BusinessYear, out HSALimit);

                    if (EmployerId == 99999)
                    {
                        IndividualSubsidy = new SubsidyCal().CalculateSubsidy(BusinessYear, Income, IsAmericanIndian, RatingAreaId, StateCode, fmlMemberList, SubsidyStatus, "Indi", out ACAPlanIdSub, out MemberRemoveMedicaidEligibility, out MemberRemoveChipEligibility, out FPL, out SecondLowestPlanId);
                    }
                    if (EmployerId == 100000)
                    {
                        ShopSubsidy = new SubsidyCal().CalculateSubsidy(BusinessYear, Income, IsAmericanIndian, RatingAreaId, StateCode, fmlMemberList, SubsidyStatus, "SHOP", out ACAPlanIdSub, out MemberRemoveMedicaidEligibility, out MemberRemoveChipEligibility, out FPL, out SecondLowestPlanId);
                    }

                    var data = objOptionSheet.CalculateOptionsNew(fmlMemberList, lstFamilyMemberUses, JobNumber, ZipCode, CountyName, Income, SubsidyStatus, UsageCode, Welness, HSAPercentage, TaxRate, (decimal)MaxEEHSA, IsAmericanIndian, ResultStatus, IndividualSubsidy, ShopSubsidy, RatingAreaId, ProgID, HSALimit, StateCode, ACAPlanIdSub, PlanTypeID, IssuerId, TierIntention,0);

                    bool caseStatus = UpdateCaseResults(oCase.CaseID, data);

                    if (caseStatus)
                    {
                        oCase = DB.Cases.Where(r => r.CaseID == oCase.CaseID).FirstOrDefault();
                        oCase.CaseJobRunStatus = Convert.ToString(MHM.Api.Models.EnumStatusModel.CaseJobRunStatus.GeneratedOnly);
                        oCase.CaseJobRunDt = DateTime.Now;
                        oCase.CaseJobRunUserID = ModifiedBy;
                        DB.SaveChanges();

                        var Job = DB.JobMasters.Where(r => r.JobNumber == oCase.JobNumber).FirstOrDefault();
                        Job.LastJobRunDt = DateTime.Now;
                        Job.LastJubRunUserID = ModifiedBy;
                        Job.LastJobRunStatus = Job.JobRunStatus;
                        if (Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.GenErrors) && Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.SendErrors))
                        {
                            Job.JobRunStatus = Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.AllGenerated);
                        }
                        DB.SaveChanges();

                        Dictionary<string, object> res = new Dictionary<string, object>();
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("Plans", data.Take(4));
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "failed to generate result";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oCase = DB.Cases.Where(r => r.CaseID == oCase.CaseID).FirstOrDefault();
                    oCase.CaseJobRunStatus = Convert.ToString(MHM.Api.Models.EnumStatusModel.CaseJobRunStatus.GenError);
                    oCase.CaseJobRunDt = DateTime.Now;
                    oCase.CaseJobRunMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    oCase.CaseJobRunUserID = ModifiedBy;
                    DB.SaveChanges();

                    var Job = DB.JobMasters.Where(r => r.JobNumber == oCase.JobNumber).FirstOrDefault();
                    Job.LastJobRunDt = DateTime.Now;
                    Job.LastJubRunUserID = ModifiedBy;
                    Job.LastJobRunStatus = Job.JobRunStatus;
                    if (Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.GenErrors) && Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.SendErrors))
                    {
                        Job.JobRunStatus = Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.GenErrors);
                    }
                    DB.SaveChanges();

                    oResponse.Status = false;
                    oResponse.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
            }
        }

        private bool UpdateCaseResults(long CaseID, List<CasePlanResult> lstCasePlanResult)
        {
            using (var DB = new MHMDal.Models.MHM())
            {
                using (var dbContextTransaction = DB.Database.BeginTransaction())
                {
                    try
                    {
                        //Remove CasePlanResult
                        var CasePlanResult = DB.CasePlanResults.Where(r => r.CaseId == CaseID).ToList();
                        DB.CasePlanResults.RemoveRange(CasePlanResult);
                        DB.SaveChanges();

                        var CasePlanResults = lstCasePlanResult.ToList();

                        // Save Case Result
                        foreach (var r in CasePlanResults)
                        {
                            DB.Entry(r).State = EntityState.Detached;

                            r.CaseId = CaseID;
                            r.GrossAnnualPremium = r.GrossAnnualPremium;
                            r.FederalSubsidy = r.FederalSubsidy;
                            r.NetAnnualPremium = r.NetAnnualPremium;
                            r.MonthlyPremium = r.MonthlyPremium;
                            r.Copays = r.Copays;
                            r.PaymentsToDeductibleLimit = r.PaymentsToDeductibleLimit;
                            r.CoinsuranceToOutOfPocketLimit = r.CoinsuranceToOutOfPocketLimit;
                            r.ContributedToYourHSAAccount = r.ContributedToYourHSAAccount;
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

                            DB.CasePlanResults.Add(r);
                            DB.SaveChanges();
                        }

                        dbContextTransaction.Commit();
                        return true;

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        [HttpPost, Route("api/GenerateResultsController/SendMail")]
        public async Task<HttpResponseMessage> SendMail()
        {
            Response oResponse = new Response();

            bool MailStatus = true;

            HttpContent requestContent = Request.Content;
            var Content = await requestContent.ReadAsStringAsync();

            var JsonData = JsonConvert.DeserializeObject<JsonResponse>(Content);
            byte[] data = Convert.FromBase64String(JsonData.Html);
            string Html = System.Text.Encoding.UTF8.GetString(data);
            long CaseId = Convert.ToInt64(JsonData.CaseId);
            long ModifiedBy = Convert.ToInt64(JsonData.ModifiedBy);

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    #region Generating PDF

                    // read parameters from the webpage
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
                    // create a new pdf document converting an url
                    PdfDocument doc = converter.ConvertHtmlString(Html, baseUrl);

                    // save pdf document
                    byte[] outPdfBuffer = doc.Save();

                    // close pdf document
                    doc.Close();

                    #endregion

                    string Mail = "";

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

                    Mail = EmailBodyText + EmailSignText;

                    Service.SendMailWithAttaitchment(JsonData.ApplicantEmail, JsonData.AgentEmail, "reports@myhealthmath.com", EmailSubjText, Mail, outPdfBuffer, JsonData.CaseTitle + ".pdf");

                    //var oCase = DB.Cases.Where(r => r.CaseID == CaseId).FirstOrDefault();

                    UpdateCaseToSent(CaseId, MailStatus, ModifiedBy);

                    oResponse.Status = true;
                    oResponse.Message = "Success";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;

                }
                catch (Exception ex)
                {
                    MailStatus = false;
                    UpdateCaseToSent(CaseId, MailStatus, ModifiedBy);
                    oResponse.Status = false;
                    oResponse.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [Route("api/GenerateResultsController/GetData")]
        public HttpResponseMessage GetData()
        {
            MHMDal.Models.MHM DB = new MHMDal.Models.MHM();

            var test = DB.Cases.Take(5).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, test);
        }

        public void UpdateCaseToSent(long CaseId, bool MailStatus, long ModifiedBy)
        {
            using (var DB = new MHMDal.Models.MHM())
            {
                var oCase = DB.Cases.Where(r => r.CaseID == CaseId).FirstOrDefault();
                try
                {
                    if (oCase.StatusId == 6 || oCase.StatusId == 8)
                    {
                        oCase.StatusId = MailStatus ? 8 : oCase.StatusId;
                    }
                    else
                    {
                        oCase.StatusId = MailStatus ? 2 : oCase.StatusId;
                    }
                    oCase.CaseJobRunStatus = MailStatus ? Convert.ToString(MHM.Api.Models.EnumStatusModel.CaseJobRunStatus.ReportSent) : Convert.ToString(MHM.Api.Models.EnumStatusModel.CaseJobRunStatus.SendError);
                    oCase.CaseJobRunDt = DateTime.Now;
                    oCase.CaseJobRunUserID = ModifiedBy;
                    DB.SaveChanges();

                    var Job = DB.JobMasters.Where(r => r.JobNumber == oCase.JobNumber).FirstOrDefault();
                    Job.LastJobRunDt = DateTime.Now;
                    Job.LastJubRunUserID = ModifiedBy;
                    Job.LastJobRunStatus = Job.JobRunStatus;
                    if (Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.GenErrors) && Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.SendErrors))
                    {
                        Job.JobRunStatus = MailStatus ? Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.AllSent) : Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.SendErrors);
                    }
                    DB.SaveChanges();

                }
                catch (Exception ex)
                {
                    oCase.CaseJobRunStatus = Convert.ToString(MHM.Api.Models.EnumStatusModel.CaseJobRunStatus.SendError);
                    oCase.CaseJobRunMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    oCase.CaseJobRunDt = DateTime.Now;
                    oCase.CaseJobRunUserID = ModifiedBy;
                    DB.SaveChanges();

                    var Job = DB.JobMasters.Where(r => r.JobNumber == oCase.JobNumber).FirstOrDefault();
                    Job.LastJobRunDt = DateTime.Now;
                    Job.LastJubRunUserID = ModifiedBy;
                    Job.LastJobRunStatus = Job.JobRunStatus;
                    if (Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.GenErrors) && Job.JobRunStatus != Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.SendErrors))
                    {
                        Job.JobRunStatus = Convert.ToString(MHM.Api.Models.EnumStatusModel.JobRunStatus.SendErrors);
                    }
                    DB.SaveChanges();
                }
            }
        }


    }
}
