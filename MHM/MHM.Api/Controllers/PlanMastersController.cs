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
using System.Transactions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web;
using Newtonsoft.Json;
using MHMDal.Models;
using MHMDal.Interfaces;
using MHMDal.Repository;
using System.IO;
using ClosedXML.Excel;
using System.Data.Entity;
using MHM.Api.ViewModel;
using System.Data.Entity.Infrastructure;
using System.Text;
using MHMBLL;
using System.Data.Entity.Validation;
using MHM.Api.Models;
using System.Data.Entity.Core.Objects;
using MHM.Api.Helpers;

namespace MHM.Api.Controllers
{
    public class PlanMastersController : ApiController
    {
        //MHMDal.Models.MHM DB;
        IPlanAttributeMaster objPlanAttributeMst = new PlanAttributeMasterRepo();
        ICSRRateMaster objCSRRateMst = new CSRRateMasterRepo();
        IPlanBenefitMaster objPlanBenefitMst = new PlanBenefitMasterRepo();
        IMHMBenefitMappingMaster objMHMBenefitMappingMst = new MHMBenefitMappingMasterRepo();
        MHMCache MHMCache = new MHMCache();
        SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ConnectionString);
        SqlCommand objcmd;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetAllPlanAttributes")]
        public HttpResponseMessage GetAllPlanAttributes([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            List<PlanAttributeList> objPlanAttributeList = new List<PlanAttributeList>();

            try
            {
                //objConn.Open();
                objcmd = new SqlCommand("get_PlanAttributeMst_List", objConn);
                objcmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(searchby))
                {
                    objcmd.Parameters.Add("@Searchby", SqlDbType.VarChar).Value = searchby;
                }

                if (BusinessYear != null)
                {
                    objcmd.Parameters.Add("@BusinessYear", SqlDbType.VarChar).Value = BusinessYear;
                }

                if (!string.IsNullOrEmpty(lstParameter[0]))
                {
                    objcmd.Parameters.Add("@ApprovalStatus", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[0]);
                }
                if (!string.IsNullOrEmpty(lstParameter[1]))
                {
                    objcmd.Parameters.Add("@CarrierId", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[1]);
                }
                if (!string.IsNullOrEmpty(lstParameter[3]))
                {
                    objcmd.Parameters.Add("@MrktCover", SqlDbType.VarChar).Value = lstParameter[3].ToString();
                }
                if (!string.IsNullOrEmpty(lstParameter[4]))
                {
                    objcmd.Parameters.Add("@GroupName", SqlDbType.VarChar).Value = lstParameter[4].ToString();
                }
                if (!string.IsNullOrEmpty(lstParameter[5]))
                {
                    objcmd.Parameters.Add("@OpenForEnrollment", SqlDbType.Bit).Value = Convert.ToBoolean(lstParameter[5]);
                }
                if (!string.IsNullOrEmpty(lstParameter[6]))
                {
                    objcmd.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = lstParameter[6].ToString();
                }

                objcmd.Parameters.Add("@SortColumn", SqlDbType.VarChar).Value = sortby;
                if (desc) { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "desc"; }
                else { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "asc"; }

                objcmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = page;

                List<CaseStatus> lstApprovalStatus = new List<CaseStatus>();
                lstApprovalStatus.Add(new CaseStatus() { Id = 1, Value = "Not Reviewed" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 2, Value = "Reviewed, not Tested" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 3, Value = "Tested, Errors to Address" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 4, Value = "Tested, Approved" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 5, Value = "In Production" });

                objConn.Open();
                SqlDataReader objReader = objcmd.ExecuteReader();

                int total = 0;


                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {

                        PlanAttributeList objPlanAttribute = new PlanAttributeList();
                        objPlanAttribute.Id = Convert.ToInt64(objReader["Id"]);
                        objPlanAttribute.PlanId = objReader["PlanId"].ToString();
                        objPlanAttribute.BusinessYear = objReader["BusinessYear"] != DBNull.Value ? objReader["BusinessYear"].ToString() : "";
                        objPlanAttribute.CarrierId = objReader["CarrierId"] != DBNull.Value ? (Int64?)Convert.ToInt64(objReader["CarrierId"]) : null;
                        objPlanAttribute.IssuerName = objReader["IssuerName"] != DBNull.Value ? objReader["IssuerName"].ToString() : "";
                        objPlanAttribute.MrktCover = objReader["MrktCover"] != DBNull.Value ? objReader["MrktCover"].ToString() : "";
                        objPlanAttribute.PlanType = objReader["PlanType"] != DBNull.Value ? objReader["PlanType"].ToString() : "";
                        objPlanAttribute.MetalLevel = objReader["MetalLevel"] != DBNull.Value ? objReader["MetalLevel"].ToString() : "";
                        objPlanAttribute.OpenForEnrollment = objReader["OpenForEnrollment"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["OpenForEnrollment"]) : null;
                        objPlanAttribute.StateCode = objReader["StateCode"] != DBNull.Value ? objReader["StateCode"].ToString() : "";
                        var _approvalstatus = Convert.ToInt32(objReader["ApprovalStatus"]);
                        objPlanAttribute.ApprovalStatus = lstApprovalStatus.Where(t => t.Id == _approvalstatus).FirstOrDefault().Value;
                        objPlanAttribute.IsHSAEligible = objReader["IsHSAEligible"] != DBNull.Value ? Convert.ToBoolean(objReader["IsHSAEligible"]) : false;
                        objPlanAttribute.TEHBDedInnTier1Individual = objReader["TEHBDedInnTier1Individual"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["TEHBDedInnTier1Individual"]) : null;
                        objPlanAttribute.TEHBDedInnTier1FamilyPerPerson = objReader["TEHBDedInnTier1FamilyPerPerson"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["TEHBDedInnTier1FamilyPerPerson"]) : null;
                        objPlanAttribute.TEHBInnTier1IndividualMOOP = objReader["TEHBInnTier1IndividualMOOP"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["TEHBInnTier1IndividualMOOP"]) : null;
                        objPlanAttribute.TEHBInnTier1FamilyPerPersonMOOP = objReader["TEHBInnTier1FamilyPerPersonMOOP"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["TEHBInnTier1FamilyPerPersonMOOP"]) : null;
                        objPlanAttribute.TEHBDedInnTier1FamilyPerGroup = objReader["TEHBDedInnTier1FamilyPerGroup"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["TEHBDedInnTier1FamilyPerGroup"]) : null;
                        objPlanAttribute.TEHBInnTier1FamilyPerGroupMOOP = objReader["TEHBInnTier1FamilyPerGroupMOOP"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["TEHBInnTier1FamilyPerGroupMOOP"]) : null;
                        objPlanAttribute.UnassgnBen = objReader["UnassgnBen"] != DBNull.Value ? Convert.ToInt32(objReader["UnassgnBen"]) : 0;
                        objPlanAttribute.NoOfCases = objReader["NoOfCases"] != DBNull.Value ? Convert.ToInt32(objReader["NoOfCases"]) : 0;
                        objPlanAttribute.PlanMarketingName = objReader["PlanMarketingName"] != DBNull.Value ? objReader["PlanMarketingName"].ToString() : "";
                        objPlanAttribute.GroupName = objReader["GroupName"] != DBNull.Value ? objReader["GroupName"].ToString() : "";
                        objPlanAttribute.TotalCount = Convert.ToInt32(objReader["TotalCount"]);
                        objPlanAttributeList.Add(objPlanAttribute);
                    }
                    total = objPlanAttributeList.First().TotalCount;
                }

                objReader.Close();

                if (total > 0)
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");

                    res.Add("PlanAttributes", objPlanAttributeList);
                    res.Add("TotalCount", total);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "PlanAttributes does not exist.");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                string ExceptionString = "Api : GetAllPlanAttributes" + Environment.NewLine;
                ExceptionString += "Request :  " + " lstParameter " + lstParameter + " ,BusinessYear " + BusinessYear + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize" + pageSize + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetAllPlanAttributes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
            finally
            {
                objPlanAttributeList = null;
                objcmd.Dispose();
                objConn.Close();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetAllCSRRates")]
        public HttpResponseMessage GetAllCSRRates([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                //SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ToString());
                objcmd = new SqlCommand("get_CSR_Rate_Mst_List", objConn);
                objcmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(searchby))
                {
                    objcmd.Parameters.Add("@Searchby", SqlDbType.VarChar).Value = searchby;
                }

                if (BusinessYear != null)
                {
                    objcmd.Parameters.Add("@BusinessYear", SqlDbType.VarChar).Value = BusinessYear;
                }

                if (!string.IsNullOrEmpty(lstParameter[0]))
                {
                    objcmd.Parameters.Add("@RatingAreaId", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[0]);
                }
                if (!string.IsNullOrEmpty(lstParameter[1]))
                {
                    objcmd.Parameters.Add("@MrktCover", SqlDbType.VarChar).Value = lstParameter[1].ToString();
                }
                if (!string.IsNullOrEmpty(lstParameter[2]))
                {
                    objcmd.Parameters.Add("@CarrierId", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[2]);
                }

                objcmd.Parameters.Add("@SortColumn", SqlDbType.VarChar).Value = sortby;
                if (desc) { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "desc"; }
                else { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "asc"; }

                objcmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = page;

                objConn.Open();
                SqlDataReader objReader = objcmd.ExecuteReader();

                int total = 0;

                List<CSRRateMst> objCSRRateList = new List<CSRRateMst>();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {

                        CSRRateMst objCSRRate = new CSRRateMst();
                        objCSRRate.Id = Convert.ToInt64(objReader["Id"]);
                        objCSRRate.PlanId = objReader["PlanID"].ToString();
                        objCSRRate.RatingAreaId = Convert.ToInt64(objReader["RatingAreaId"]);
                        objCSRRate.BusinessYear = objReader["BusinessYear"].ToString();
                        objCSRRate.Age = objReader["Age"] != DBNull.Value ? objReader["Age"].ToString() : "";
                        objCSRRate.IndividualRate = objReader["IndividualRate"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["IndividualRate"]) : null;
                        objCSRRate.GrpCostAmt = objReader["GrpCostAmt"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["GrpCostAmt"]) : null;
                        objCSRRate.GrpEmplrPremAmt = objReader["GrpEmplrPremAmt"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["GrpEmplrPremAmt"]) : null;
                        objCSRRate.GrpHSAAmt = objReader["GrpHSAAmt"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["GrpHSAAmt"]) : null;
                        objCSRRate.IssuerName = objReader["IssuerName"] != DBNull.Value ? objReader["IssuerName"].ToString() : "";
                        objCSRRate.PlanName = objReader["PlanMarketingName"] != DBNull.Value ? objReader["PlanMarketingName"].ToString() : "";
                        objCSRRate.TotalCount = Convert.ToInt32(objReader["TotalCount"]);
                        objCSRRateList.Add(objCSRRate);
                    }
                    total = objCSRRateList.First().TotalCount;
                }

                if (total > 0)
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");

                    res.Add("CSRRates", objCSRRateList);
                    res.Add("TotalCount", total);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "CSR Rates does not exist.");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }

            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetAllCSRRates" + Environment.NewLine;
                ExceptionString += "Request :  " + " lstParameter " + lstParameter + " ,BusinessYear " + BusinessYear + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize" + pageSize + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetAllCSRRates - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
            finally
            {
                objcmd.Dispose();
                objConn.Close();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetCSRSearchPlanIds")]
        public HttpResponseMessage GetCSRSearchPlanIds(string keyword)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            List<PlanDetailsCSR> obj;

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var lstPlanAttributes = DB.PlanAttributeMsts.ToList();

                    //var lstPlanAttributes = objPlanAttributeMst.GetPlanAttributeMaster();

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var planAttributes = lstPlanAttributes.Where(x => x.StandardComponentId.StartsWith(keyword) || x.PlanMarketingName.ToLower().StartsWith(keyword.ToLower()) && x.OpenForEnrollment == true).ToList();
                        var sortedPlanAttributes = planAttributes.GroupBy(r => r.StandardComponentId).Select(g => g.First()).ToList();
                        obj = sortedPlanAttributes.Select(r => new PlanDetailsCSR { PlanId = r.StandardComponentId, PlanMarketingName = r.PlanMarketingName, IssuerId = r.CarrierId, IssuerName = r.IssuerMst.IssuerName, BusinessYear = r.BusinessYear }).Distinct().Take(30).ToList();
                    }
                    else
                    {
                        obj = lstPlanAttributes.Where(x => x.OpenForEnrollment == true).ToList().GroupBy(r => r.StandardComponentId).Select(g => g.First()).ToList().Select(r => new PlanDetailsCSR { PlanId = r.StandardComponentId, PlanMarketingName = r.PlanMarketingName, IssuerId = r.CarrierId, IssuerName = r.IssuerMst.IssuerName, BusinessYear = r.BusinessYear }).Take(30).ToList();
                    }

                }
                int total = obj.Count();

                if (total > 0)
                {
                    res.Add("PlanIds", obj.OrderBy(r => r.PlanId));
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "Plan Ids does not exist.");
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                return response;
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetCSRSearchPlanIds" + Environment.NewLine;
                ExceptionString += "Request :  " + " keyword " + keyword + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetCSRSearchPlanIds - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
            finally
            {
                obj = null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetSearchPlanIds")]
        public HttpResponseMessage GetSearchPlanIds(string keyword)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            List<PlanDetails> obj;

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var lstPlanAttributes = DB.PlanAttributeMsts.ToList();

                    //var lstPlanAttributes = objPlanAttributeMst.GetPlanAttributeMaster();

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var planAttributes = lstPlanAttributes.Where(x => x.PlanId.StartsWith(keyword) || x.PlanMarketingName.ToLower().StartsWith(keyword.ToLower()) && x.OpenForEnrollment == true).ToList();
                        var sortedPlanAttributes = planAttributes.GroupBy(r => r.PlanId).Select(g => g.First()).ToList();
                        obj = sortedPlanAttributes.Select(r => new PlanDetails { Id = r.Id, PlanId = r.PlanId, PlanMarketingName = r.PlanMarketingName, IssuerId = r.CarrierId, IssuerName = r.IssuerMst.IssuerName, BusinessYear = lstPlanAttributes.Where(x => x.PlanId == r.PlanId && x.OpenForEnrollment == true).Select(x => x.BusinessYear).ToList() }).Distinct().Take(30).ToList();
                    }
                    else
                    {
                        obj = lstPlanAttributes.Where(x => x.OpenForEnrollment == true).ToList().GroupBy(r => r.PlanId).Select(g => g.First()).ToList().Select(r => new PlanDetails { Id = r.Id, PlanId = r.PlanId, PlanMarketingName = r.PlanMarketingName, IssuerId = r.CarrierId, IssuerName = r.IssuerMst.IssuerName, BusinessYear = lstPlanAttributes.Where(x => x.PlanId == r.PlanId && x.OpenForEnrollment == true).Select(x => x.BusinessYear).ToList() }).Take(30).ToList();
                    }
                }

                int total = obj.Count();

                if (total > 0)
                {
                    res.Add("PlanIds", obj.OrderBy(r => r.PlanId));
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "Plan Ids does not exist.");
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                return response;
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetSearchPlanIds" + Environment.NewLine;
                ExceptionString += "Request :  " + " keyword " + keyword + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetSearchPlanIds - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
            finally
            {
                obj = null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetSearchMHMBenefitIds")]
        public HttpResponseMessage GetSearchMHMBenefitIds()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            List<BenefitsList> obj;

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstobjMHMCommonBenefitsMst = DB.MHMCommonBenefitsMsts.AsQueryable();
                    obj = lstobjMHMCommonBenefitsMst.Select(x => new BenefitsList { MHMBenefitID = x.MHMBenefitID, MHMBenefitName = x.MHMBenefitName }).OrderBy(r => r.MHMBenefitName).ToList();

                    int total = obj.Count();

                    if (total > 0)
                    {
                        res.Add("MHMBenefitIds", obj.OrderBy(r => r.MHMBenefitName));
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "MHMBenefit Ids does not exist.");
                    }

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetSearchMHMBenefitIds" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetSearchMHMBenefitIds - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetSearchIssuerIds")]
        public HttpResponseMessage GetSearchIssuerIds()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            List<IssuerssList> obj;
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    //var lstIssuers = DB.IssuerMsts.AsQueryable();
                    //obj = lstIssuers.OrderBy(x => x.IssuerName).Select(r => new IssuerssList { Id = r.Id, IssuerCode = r.IssuerCode, IssuerName = r.IssuerName }).ToList();
                    obj = DB.Database.SqlQuery<IssuerssList>(@"select Id, IssuerCode, IssuerName from IssuerMst order by IssuerName").ToList();

                    int total = obj.Count();

                    if (total > 0)
                    {
                        res.Add("CarrierIds", obj);
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Carrier Ids does not exist.");
                    }

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetSearchIssuerIds" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetSearchIssuerIds - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetAllPlanBenefits")]
        public HttpResponseMessage GetAllPlanBenefits([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            List<PlanBenefitList> objPlanBenefitList = new List<PlanBenefitList>();

            try
            {
                //SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ToString());
                //objConn.Open();
                objcmd = new SqlCommand("get_PlanBenefitMst_List", objConn);
                objcmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(searchby))
                {
                    objcmd.Parameters.Add("@Searchby", SqlDbType.VarChar).Value = searchby;
                }

                if (BusinessYear != null)
                {
                    objcmd.Parameters.Add("@BusinessYear", SqlDbType.VarChar).Value = BusinessYear;
                }

                if (!string.IsNullOrEmpty(lstParameter[0]))
                {
                    objcmd.Parameters.Add("@PlanId", SqlDbType.VarChar).Value = lstParameter[0].ToString();
                }
                if (!string.IsNullOrEmpty(lstParameter[1]))
                {
                    objcmd.Parameters.Add("@MHMBenefitId", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[1]);
                }
                if (!string.IsNullOrEmpty(lstParameter[2]))
                {
                    objcmd.Parameters.Add("@CarrierId", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[2]);
                }

                objcmd.Parameters.Add("@SortColumn", SqlDbType.VarChar).Value = sortby;
                if (desc) { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "desc"; }
                else { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "asc"; }

                objcmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = page;

                objConn.Open();
                SqlDataReader objReader = objcmd.ExecuteReader();

                int total = 0;

                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {

                        PlanBenefitList objPlanBenefit = new PlanBenefitList();
                        objPlanBenefit.Id = Convert.ToInt64(objReader["Id"]);
                        objPlanBenefit.PlanId = objReader["PlanId"].ToString();
                        objPlanBenefit.BenefitName = objReader["BenefitName"] != DBNull.Value ? objReader["BenefitName"].ToString() : "";
                        //objPlanBenefit.IsCovered = objReader["IsCovered"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["IsCovered"]) : null;
                        objPlanBenefit.CostSharingType1 = objReader["CostSharingType1"] != DBNull.Value ? objReader["CostSharingType1"].ToString() : "";
                        //objPlanBenefit.CostSharingType2 = objReader["CostSharingType2"] != DBNull.Value ? objReader["CostSharingType2"].ToString() : "";
                        objPlanBenefit.CopayInnTier1 = objReader["CopayInnTier1"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CopayInnTier1"]) : null;
                        objPlanBenefit.MHMBenefitId = objReader["MHMBenefitId"] != DBNull.Value ? (Int64?)Convert.ToInt64(objReader["MHMBenefitId"]) : null;
                        objPlanBenefit.CoinsInnTier1 = objReader["CoinsInnTier1"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CoinsInnTier1"]) : null;
                        objPlanBenefit.BusinessYear = objReader["BusinessYear"] != DBNull.Value ? objReader["BusinessYear"].ToString() : "";
                        objPlanBenefit.Category = objReader["Category"] != DBNull.Value ? objReader["Category"].ToString() : "";
                        //objPlanBenefit.Unassign = objReader["Unassign"] != DBNull.Value ? (Int32?)Convert.ToInt32(objReader["Unassign"]) : null;
                        //objPlanBenefit.IssuerId = objReader["IssuerId"] != DBNull.Value ? (Int64?)Convert.ToInt64(objReader["IssuerId"]) : null;
                        objPlanBenefit.IssuerName = objReader["IssuerName"] != DBNull.Value ? objReader["IssuerName"].ToString() : "";
                        objPlanBenefit.TotalCount = Convert.ToInt32(objReader["TotalCount"]);
                        objPlanBenefit.LimitQty = objReader["LimitQty"] != DBNull.Value ? Convert.ToInt32(objReader["LimitQty"]) : 0;
                        objPlanBenefit.LimitUnit = objReader["LimitUnit"] != DBNull.Value ? objReader["LimitUnit"].ToString() : "";
                        objPlanBenefitList.Add(objPlanBenefit);
                    }
                    total = objPlanBenefitList.First().TotalCount;
                }
                objReader.Close();
                if (total > 0)
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");

                    res.Add("PlanBenefits", objPlanBenefitList);
                    res.Add("TotalCount", total);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "Plan Benefits does not exist.");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetAllPlanBenefits" + Environment.NewLine;
                ExceptionString += "Request :  " + " lstParameter " + lstParameter + " ,BusinessYear " + BusinessYear + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize" + pageSize + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetAllPlanBenefits - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
            finally
            {
                objPlanBenefitList = null;
                objcmd.Dispose();
                objConn.Close();
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetPlanAttributes")]
        public HttpResponseMessage GetPlanAttributes(long Id, string EventType, string sortby, bool desc = true)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            List<PlanAttributeMst> lstPlanAttribute;

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    //lstPlanAttribute = objPlanAttributeMst.GetPlanAttributeMaster().ToList();
                    lstPlanAttribute = DB.PlanAttributeMsts.ToList();
                    int total = lstPlanAttribute.Count();

                    if (total > 0)
                    {
                        switch (sortby)
                        {
                            case "PlanId":
                                lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.PlanId).ToList() : lstPlanAttribute.OrderBy(x => x.PlanId).ToList();
                                break;

                            case "OpenForEnrollment":
                                lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.OpenForEnrollment).ToList() : lstPlanAttribute.OrderBy(x => x.OpenForEnrollment).ToList();
                                break;

                            case "Id":
                                lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.Id).ToList() : lstPlanAttribute.OrderBy(x => x.Id).ToList();
                                break;

                            case "Carrier":
                                lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.IssuerMst.IssuerName).ToList() : lstPlanAttribute.OrderBy(x => x.IssuerMst.IssuerName).ToList();
                                break;

                            case "PlanMarketingName":
                                lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.PlanMarketingName).ToList() : lstPlanAttribute.OrderBy(x => x.PlanMarketingName).ToList();
                                break;

                            case "GroupName":
                                lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.GroupName).ToList() : lstPlanAttribute.OrderBy(x => x.GroupName).ToList();
                                break;

                            default:
                                lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.CreatedDateTime).ToList() : lstPlanAttribute.OrderBy(x => x.CreatedDateTime).ToList();
                                break;
                        }
                    }

                    PlanAttributeMst PlanAttribute = lstPlanAttribute.Where(r => r.Id == Id).FirstOrDefault();
                    PlanAttribute.IssuerName = PlanAttribute.IssuerMst.IssuerName;
                    var nextRecord = lstPlanAttribute.SkipWhile(item => item.Id != Id).Skip(1).FirstOrDefault();
                    var preRecord = lstPlanAttribute.TakeWhile(x => x.Id != Id).LastOrDefault();

                    string CreateByName = PlanAttribute.Createdby != null ? DB.Users.Where(r => r.UserID == PlanAttribute.Createdby).FirstOrDefault().FirstName + " " + DB.Users.Where(r => r.UserID == PlanAttribute.Createdby).FirstOrDefault().LastName : "";
                    string ModifiedByName = PlanAttribute.ModifiedBy != null ? DB.Users.Where(r => r.UserID == PlanAttribute.ModifiedBy).FirstOrDefault().FirstName + " " + DB.Users.Where(r => r.UserID == PlanAttribute.ModifiedBy).FirstOrDefault().LastName : "";

                    PlanAttribute.UnassignedBen = DB.Database.SqlQuery<int>(@"select dbo.GetUnassignedBenefitNo('" + PlanAttribute.PlanId + "','" + PlanAttribute.BusinessYear + "')").FirstOrDefault();
                    string JobStatus = string.Join(",", PlanAttribute.JobPlansMsts.Where(r => r.JobMaster.JobStatus == "Open").Select(r => r.JobNumber).ToList());
                    //("select dbo.GetUnassignedBenefitNo", new ObjectParameter("@PlanId", "P204"), new ObjectParameter("@BusinessYear", "2017"));
                    //return result.First().GetDecimal(0);

                    if (PlanAttribute != null)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("PlanAttributes", PlanAttribute);
                        res.Add("CreatedBy", CreateByName);
                        res.Add("ModifiedBy", ModifiedByName);
                        res.Add("JobStatus", JobStatus);
                        res.Add("NextRecord", nextRecord != null ? nextRecord.Id.ToString() : "");
                        res.Add("PreviousRecord", preRecord != null ? preRecord.Id.ToString() : "");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Plan Attribute does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetPlanAttributes" + Environment.NewLine;
                    ExceptionString += "Request :  " + " Id " + Id + " ,EventType " + EventType + " ,sortby " + sortby + " ,desc" + desc + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetPlanAttributes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
                finally
                {
                    lstPlanAttribute = null;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/UpdatePlanAttributes")]
        public HttpResponseMessage UpdatePlanAttributes(long Id, bool Status)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var PlanAttribute = DB.PlanAttributeMsts.Where(r => r.Id == Id).FirstOrDefault();
                    if (PlanAttribute != null)
                    {
                        PlanAttribute.OpenForEnrollment = Status;
                        PlanAttribute.ModifiedDateTime = System.DateTime.Now;
                        DB.SaveChanges();

                        //Remove Cache of PlanAttributeMaster
                        var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanAttributeMaster");
                        if (PlanAttributeMasterFromCache != null)
                        {
                            MHMCache.RemoveMyCachedItem("PlanAttributeMaster");
                        }

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Plan Attribute does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UpdatePlanAttributes" + Environment.NewLine;
                    ExceptionString += "Request :  " + " Id " + Id + " ,Status " + Status + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdatePlanAttributes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/UpdatePlanAttribute")]
        public HttpResponseMessage UpdatePlanAttributes(PlanAttributeMst objPlanAttribute)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                        oResponse.Status = false;
                        oResponse.Message = Messages;
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return objResponse;
                    }
                    if (DB.PlanAttributeMsts.Any(o => o.Id == objPlanAttribute.Id))
                    {
                        objPlanAttribute.ModifiedDateTime = System.DateTime.Now;
                        var isOpenForEnrollment = DB.PlanAttributeMsts.Where(x => x.Id == objPlanAttribute.Id).Select(y => y.OpenForEnrollment).FirstOrDefault();
                        if (isOpenForEnrollment != objPlanAttribute.OpenForEnrollment)
                            objPlanAttribute.OpenForEnrollment_ChangedDate = System.DateTime.Now;
                        DB.PlanAttributeMsts.Attach(objPlanAttribute);
                        DB.Entry(objPlanAttribute).State = EntityState.Modified;
                        DB.SaveChanges();

                        //Remove Cache of PlanAttributeMaster
                        var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanAttributeMaster");
                        if (PlanAttributeMasterFromCache != null)
                        {
                            MHMCache.RemoveMyCachedItem("PlanAttributeMaster");
                        }

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Plan Attribute does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UpdatePlanAttributes" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objPlanAttribute " + JsonConvert.SerializeObject(objPlanAttribute) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdatePlanAttributes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/AddPlanAttribute")]
        public HttpResponseMessage AddPlanAttribute(PlanAttributeMst objPlanAttribute)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (DB.PlanAttributeMsts.Any(o => o.PlanId == objPlanAttribute.PlanId && o.BusinessYear == objPlanAttribute.BusinessYear))
                        {
                            res.Add("Status", "false");
                            res.Add("Message", "Plan Attribute with same PlanId and Business year already exists.");
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                        else
                        {
                            if (objPlanAttribute.OldPlanId != null && objPlanAttribute.OldPlanId != 0)
                            {
                                using (var dbContextTransaction = DB.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        objPlanAttribute.CreatedDateTime = System.DateTime.Now;
                                        objPlanAttribute.ModifiedDateTime = null;

                                        var OldPlanId = objPlanAttribute.OldPlanId;

                                        DB.PlanAttributeMsts.Add(objPlanAttribute);
                                        DB.SaveChanges();

                                        var OldPlan = DB.PlanAttributeMsts.Where(r => r.Id == OldPlanId).FirstOrDefault();

                                        var lstPlanBenefits = DB.PlanBenefitMsts.Where(r => r.PlanId == OldPlan.PlanId && r.BusinessYear == OldPlan.BusinessYear).ToList();
                                        lstPlanBenefits.ForEach(r => r.PlanId = objPlanAttribute.PlanId);
                                        lstPlanBenefits.ForEach(r => r.BusinessYear = objPlanAttribute.BusinessYear);
                                        lstPlanBenefits.ForEach(r => r.CreatedDateTime = System.DateTime.Now);
                                        lstPlanBenefits.ForEach(r => r.Createdby = objPlanAttribute.Createdby);
                                        DB.PlanBenefitMsts.AddRange(lstPlanBenefits);
                                        DB.SaveChanges();

                                        if (OldPlan.StandardComponentId != objPlanAttribute.StandardComponentId)
                                        {
                                            var lstCSRRates = DB.CSR_Rate_Mst.Where(r => r.PlanID == OldPlan.StandardComponentId && r.BusinessYear == OldPlan.BusinessYear).ToList();
                                            lstCSRRates.ForEach(r => r.PlanID = objPlanAttribute.StandardComponentId);
                                            lstCSRRates.ForEach(r => r.BusinessYear = objPlanAttribute.BusinessYear);
                                            lstCSRRates.ForEach(r => r.CreatedDateTime = System.DateTime.Now);
                                            lstCSRRates.ForEach(r => r.Createdby = objPlanAttribute.Createdby);
                                            DB.CSR_Rate_Mst.AddRange(lstCSRRates);
                                            DB.SaveChanges();
                                        }

                                        dbContextTransaction.Commit();
                                    }
                                    catch (Exception e)
                                    {
                                        dbContextTransaction.Rollback();
                                        throw e;
                                    }
                                }
                            }
                            else
                            {
                                objPlanAttribute.CreatedDateTime = System.DateTime.Now;
                                objPlanAttribute.ModifiedDateTime = null;
                                DB.PlanAttributeMsts.Add(objPlanAttribute);
                                DB.SaveChanges();
                            }

                            //Remove Cache of PlanAttributeMaster
                            var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanAttributeMaster");
                            if (PlanAttributeMasterFromCache != null)
                            {
                                MHMCache.RemoveMyCachedItem("PlanAttributeMaster");
                            }

                            oResponse.Status = true;
                            oResponse.Message = "Plan Attribute added successfully";
                            HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return objResponse;
                        }
                    }
                    else
                    {
                        var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                        oResponse.Status = false;
                        oResponse.Message = Messages;
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return objResponse;
                    }

                }
                catch (DbUpdateException ex)
                {
                    SqlException innerException = ex.InnerException.InnerException as SqlException;
                    if (innerException != null && (innerException.Number == 2627 || innerException.Number == 2601))
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Invalid PlanId";
                        HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return resp;
                    }
                    oResponse.Status = false;
                    oResponse.Message = !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message.ToString() : ex.Message;

                    string ExceptionString = "Api : AddPlanAttribute" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objPlanAttribute " + JsonConvert.SerializeObject(objPlanAttribute) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddPlanAttribute - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : AddPlanAttribute" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objPlanAttribute " + JsonConvert.SerializeObject(objPlanAttribute) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddPlanAttribute - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetCSRRate")]
        public HttpResponseMessage GetCSRRate(long Id, string EventType, string sortby, bool desc = true)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    //var lstCSRRate = objCSRRateMst.GetCSRRateMaster();

                    var lstCSRRate = DB.CSR_Rate_Mst.ToList();

                    switch (sortby)
                    {
                        case "Id":
                            lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.Id).ToList() : lstCSRRate.OrderBy(x => x.Id).ToList();
                            break;

                        case "PlanID":
                            lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.PlanID).ToList() : lstCSRRate.OrderBy(x => x.PlanID).ToList();
                            break;

                        case "Area":
                            lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.RatingAreaId).ToList() : lstCSRRate.OrderBy(x => x.RatingAreaId).ToList();
                            break;

                        case "Age":
                            lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.Age).ToList() : lstCSRRate.OrderBy(x => x.Age).ToList();
                            break;

                        default:
                            lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.CreatedDateTime).ToList() : lstCSRRate.OrderBy(x => x.CreatedDateTime).ToList();
                            break;
                    }

                    CSR_Rate_Mst CSRRate = lstCSRRate.Where(r => r.Id == Id).FirstOrDefault();

                    if (CSRRate != null)
                    {
                        var nextRecord = lstCSRRate.SkipWhile(item => item.Id != Id).Skip(1).FirstOrDefault();
                        var preRecord = lstCSRRate.TakeWhile(x => x.Id != Id).LastOrDefault();

                        string CreateByName = CSRRate.Createdby != null ? DB.Users.Where(r => r.UserID == CSRRate.Createdby).FirstOrDefault() != null ? DB.Users.Where(r => r.UserID == CSRRate.Createdby).FirstOrDefault().FirstName + " " + DB.Users.Where(r => r.UserID == CSRRate.Createdby).FirstOrDefault().LastName : "" : "";
                        string ModifiedByName = CSRRate.ModifiedBy != null ? DB.Users.Where(r => r.UserID == CSRRate.ModifiedBy).FirstOrDefault() != null ? DB.Users.Where(r => r.UserID == CSRRate.ModifiedBy).FirstOrDefault().FirstName + " " + DB.Users.Where(r => r.UserID == CSRRate.ModifiedBy).FirstOrDefault().LastName : "" : "";
                        var objPlan = DB.PlanAttributeMsts.Where(r => r.StandardComponentId == CSRRate.PlanID && r.BusinessYear == CSRRate.BusinessYear).FirstOrDefault();
                        string PlanAttributeId = objPlan.Id.ToString();
                        int ApprovalStatus = objPlan.ApprovalStatus;

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("CSRRates", CSRRate);
                        res.Add("CreatedBy", CreateByName);
                        res.Add("ModifiedBy", ModifiedByName);
                        res.Add("PlanAttributeId", PlanAttributeId);
                        res.Add("ApprovalStatus", ApprovalStatus);
                        res.Add("NextRecord", nextRecord != null ? nextRecord.Id.ToString() : "");
                        res.Add("PreviousRecord", preRecord != null ? preRecord.Id.ToString() : "");
                        res.Add("PlanMarketingName", DB.PlanAttributeMsts.Where(x => x.StandardComponentId == CSRRate.PlanID && x.BusinessYear == CSRRate.BusinessYear).Select(s => s.PlanMarketingName).FirstOrDefault());
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "CSR Rate does not exist or Issue is not active.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetCSRRate" + Environment.NewLine;
                    ExceptionString += "Request :  " + " Id " + Id + " ,EventType " + EventType + " ,sortby " + sortby + " ,desc " + desc + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetCSRRate - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/UpdateCSRRate")]
        public HttpResponseMessage UpdateCSRRate(CSR_Rate_Mst objCSR_Rate)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    if (DB.CSR_Rate_Mst.Any(o => o.Id == objCSR_Rate.Id))
                    {
                        objCSR_Rate.ModifiedDateTime = System.DateTime.Now;
                        DB.CSR_Rate_Mst.Attach(objCSR_Rate);
                        DB.Entry(objCSR_Rate).State = EntityState.Modified;
                        DB.SaveChanges();

                        //Remove CSR Rate from Cache
                        var CSRRateListFromCache = MHMCache.GetMyCachedItem("CSR_Rate_Mst");
                        if (CSRRateListFromCache != null)
                        {
                            MHMCache.RemoveMyCachedItem("CSR_Rate_Mst");
                        }

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "CSR Rate does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UpdateCSRRate" + Environment.NewLine;
                    ExceptionString += "Request :  " + " UpdateCSRRate " + JsonConvert.SerializeObject(objCSR_Rate) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdateCSRRate - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/AddCSRRate")]
        public HttpResponseMessage AddCSRRate(CSR_Rate_Mst objCSR_Rate)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (DB.CSR_Rate_Mst.Any(o => o.PlanID == objCSR_Rate.PlanID && o.Age == objCSR_Rate.Age && o.RatingAreaId == objCSR_Rate.RatingAreaId))
                        {
                            res.Add("Status", "false");
                            res.Add("Message", "CSR Rate with same PlanId, Age and RatingAreaId already exists.");
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                        else
                        {
                            objCSR_Rate.CreatedDateTime = System.DateTime.Now;
                            objCSR_Rate.ModifiedDateTime = null;

                            DB.CSR_Rate_Mst.Add(objCSR_Rate);
                            DB.SaveChanges();

                            //var planAttribute = DB.PlanAttributeMsts.Where(x => x.StandardComponentId == objCSR_Rate.PlanID && x.BusinessYear == objCSR_Rate.BusinessYear && x.OpenForEnrollment == false).FirstOrDefault();
                            //if (planAttribute != null)
                            //{
                            //    planAttribute.OpenForEnrollment = true;
                            //    DB.PlanAttributeMsts.Add(planAttribute);
                            //    DB.SaveChanges();
                            //}

                            var CSRRateListFromCache = MHMCache.GetMyCachedItem("CSR_Rate_Mst");
                            if (CSRRateListFromCache != null)
                            {
                                MHMCache.RemoveMyCachedItem("CSR_Rate_Mst");
                            }

                            oResponse.Status = true;
                            oResponse.Message = "CSR Rate added successfully";
                            HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return objResponse;
                        }
                    }
                    else
                    {
                        var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                        oResponse.Status = false;
                        oResponse.Message = Messages;
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return objResponse;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : AddCSRRate" + Environment.NewLine;
                    ExceptionString += "Request :  " + " UpdateCSRRate " + JsonConvert.SerializeObject(objCSR_Rate) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddCSRRate - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetPlanBenefit")]
        public HttpResponseMessage GetPlanBenefit(long Id)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                var planBenefits = objPlanBenefitMst.GetPlanBenefitMaster();
                var PlanBenefit = planBenefits.Where(r => r.Id == Id).FirstOrDefault();
                if (PlanBenefit != null)
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("PlanBenefits", PlanBenefit);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "Plan Benefit does not exist.");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetPlanBenefit" + Environment.NewLine;
                ExceptionString += "Request :  " + " Id " + Id + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPlanBenefit - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetPlanBenefitByOrderEvent")]
        public HttpResponseMessage GetPlanBenefitByOrderEvent(long Id, string EventType = null, string sortby = null, bool desc = true)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                //objConn.Open();
                // SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ToString());
                objcmd = new SqlCommand("get_PlanBenefitMs", objConn);
                objcmd.CommandType = CommandType.StoredProcedure;


                objcmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = Id;
                objcmd.Parameters.Add("@SortColumn", SqlDbType.VarChar).Value = sortby;
                if (desc) { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "desc"; }
                else { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "asc"; }

                objConn.Open();
                SqlDataReader objReader = objcmd.ExecuteReader();

                PlanBenefitMst PlanBenefit = new PlanBenefitMst();
                string CreateByName = "";
                string ModifiedByName = "";
                string PlanAttributeId = "";
                int? ApprovalStatus = null;
                long? nextRecord = null, preRecord = null;

                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        switch (Convert.ToInt64(objReader["Rank"]))
                        {
                            case 0:
                                preRecord = Convert.ToInt64(objReader["Id"]);
                                break;
                            case 1:

                                PlanBenefit.Id = Convert.ToInt64(objReader["Id"]);
                                PlanBenefit.PlanId = objReader["PlanId"].ToString();
                                PlanBenefit.BusinessYear = objReader["BusinessYear"] != DBNull.Value ? objReader["BusinessYear"].ToString() : "";
                                PlanBenefit.BenefitName = objReader["BenefitName"] != DBNull.Value ? objReader["BenefitName"].ToString() : "";
                                PlanBenefit.CopayInnTier1Desc = objReader["CopayInnTier1Desc"] != DBNull.Value ? objReader["CopayInnTier1Desc"].ToString() : "";
                                PlanBenefit.CoinsInnTier1Desc = objReader["CoinsInnTier1Desc"] != DBNull.Value ? objReader["CoinsInnTier1Desc"].ToString() : "";
                                PlanBenefit.CopayInnTier1 = objReader["CopayInnTier1"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CopayInnTier1"]) : null;
                                PlanBenefit.CoinsInnTier1 = objReader["CoinsInnTier1"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CoinsInnTier1"]) : null;
                                PlanBenefit.IsEHB = objReader["IsEHB"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["IsEHB"]) : null;
                                PlanBenefit.IsCovered = objReader["IsCovered"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["IsCovered"]) : null;
                                PlanBenefit.IsSubjToDedTier1 = objReader["IsSubjToDedTier1"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["IsSubjToDedTier1"]) : null;
                                PlanBenefit.IsExclFromInnMOOP = objReader["IsExclFromInnMOOP"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["IsExclFromInnMOOP"]) : null;
                                PlanBenefit.SourceName = objReader["SourceName"] != DBNull.Value ? objReader["SourceName"].ToString() : "";
                                PlanBenefit.CreatedDateTime = objReader["CreatedDateTime"] != DBNull.Value ? (DateTime?)objReader["CreatedDateTime"] : null;
                                PlanBenefit.ModifiedDateTime = objReader["ModifiedDateTime"] != DBNull.Value ? (DateTime?)objReader["ModifiedDateTime"] : null;
                                PlanBenefit.BenefitNum = objReader["BenefitNum"] != DBNull.Value ? (Int32?)Convert.ToInt32(objReader["BenefitNum"]) : null;
                                PlanBenefit.MHMBenefitId = objReader["MHMBenefitId"] != DBNull.Value ? (Int64?)Convert.ToInt64(objReader["MHMBenefitId"]) : null;
                                PlanBenefit.CostSharingType1 = objReader["CostSharingType1"] != DBNull.Value ? objReader["CostSharingType1"].ToString() : "";
                                PlanBenefit.CostSharingType2 = objReader["CostSharingType2"] != DBNull.Value ? objReader["CostSharingType2"].ToString() : "";
                                PlanBenefit.MaxCoinsInnTier1Amt = objReader["MaxCoinsInnTier1Amt"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["MaxCoinsInnTier1Amt"]) : null;
                                PlanBenefit.CopayInnTier2 = objReader["CopayInnTier2"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CopayInnTier2"]) : null;
                                PlanBenefit.CoinsInnTier2 = objReader["CoinsInnTier2"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CoinsInnTier2"]) : null;
                                PlanBenefit.MaxCoinsInnTier2Amt = objReader["MaxCoinsInnTier2Amt"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["MaxCoinsInnTier2Amt"]) : null;
                                PlanBenefit.CopayOutofNet = objReader["CopayOutofNet"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CopayOutofNet"]) : null;
                                PlanBenefit.CoinsOutofNet = objReader["CoinsOutofNet"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CoinsOutofNet"]) : null;
                                PlanBenefit.QuantLimitOnSvc = objReader["QuantLimitOnSvc"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["QuantLimitOnSvc"]) : null;
                                PlanBenefit.LimitQty = objReader["LimitQty"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["LimitQty"]) : null;
                                PlanBenefit.LimitUnit = objReader["LimitUnit"] != DBNull.Value ? objReader["LimitUnit"].ToString() : null;
                                PlanBenefit.IsExclFromOonMOOP = objReader["IsExclFromOonMOOP"] != DBNull.Value ? (bool?)Convert.ToBoolean(objReader["IsExclFromOonMOOP"]) : null;
                                PlanBenefit.Exclusions = objReader["Exclusions"] != DBNull.Value ? objReader["Exclusions"].ToString() : null;
                                PlanBenefit.BenefitDeductible = objReader["BenefitDeductible"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["BenefitDeductible"]) : null;
                                PlanBenefit.CoinsMaxAmt = objReader["CoinsMaxAmt"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["CoinsMaxAmt"]) : null;
                                PlanBenefit.MaxQtyBeforeCoPay = objReader["MaxQtyBeforeCoPay"] != DBNull.Value ? (decimal?)Convert.ToDecimal(objReader["MaxQtyBeforeCoPay"]) : null;
                                // PlanBenefit.Unassign = objReader["Unassign"] != DBNull.Value ? (Int32?)Convert.ToInt32(objReader["Unassign"]) : null;
                                PlanBenefit.IssuerId = objReader["IssuerId"] != DBNull.Value ? (Int64?)Convert.ToInt64(objReader["IssuerId"]) : null;
                                PlanBenefit.IssuerName = objReader["IssuerName"] != DBNull.Value ? objReader["IssuerName"].ToString() : "";
                                CreateByName = objReader["CreateByName"] != DBNull.Value ? objReader["CreateByName"].ToString() : "";
                                ModifiedByName = objReader["ModifiedByName"] != DBNull.Value ? objReader["ModifiedByName"].ToString() : "";
                                PlanBenefit.PlanMarketingName = objReader["PlanMarketingName"] != DBNull.Value ? objReader["PlanMarketingName"].ToString() : "";
                                PlanBenefit.Createdby = objReader["Createdby"] != DBNull.Value ? (Int64?)Convert.ToInt64(objReader["Createdby"]) : null;
                                PlanBenefit.ModifiedBy = objReader["ModifiedBy"] != DBNull.Value ? (Int64?)Convert.ToInt64(objReader["ModifiedBy"]) : null;

                                PlanBenefit.BenefitKey_Old = objReader["BenefitKey_Old"] != DBNull.Value ? objReader["BenefitKey_Old"].ToString() : "";
                                PlanBenefit.MarketConverage_Old = objReader["MarketConverage_Old"] != DBNull.Value ? objReader["MarketConverage_Old"].ToString() : "";
                                PlanBenefit.VersionNum_Old = objReader["VersionNum_Old"] != DBNull.Value ? (Int32?)Convert.ToInt32(objReader["VersionNum_Old"]) : null;
                                PlanBenefit.StandardComponentId_Old = objReader["StandardComponentId_Old"] != DBNull.Value ? objReader["StandardComponentId_Old"].ToString() : "";
                                PlanBenefit.StateCode_Old = objReader["StateCode_Old"] != DBNull.Value ? objReader["StateCode_Old"].ToString() : "";
                                PlanBenefit.PlanBenNotes = objReader["PlanBenNotes"] != DBNull.Value ? objReader["PlanBenNotes"].ToString() : "";
                                PlanBenefit.SBCText = objReader["SBCText"] != DBNull.Value ? objReader["SBCText"].ToString() : "";

                                PlanAttributeId = objReader["PlanAttributeId"] != DBNull.Value ? objReader["PlanAttributeId"].ToString() : "";
                                ApprovalStatus = objReader["ApprovalStatus"] != DBNull.Value ? (Int32?)Convert.ToInt32(objReader["ApprovalStatus"]) : null;
                                break;

                            case 2:
                                nextRecord = Convert.ToInt64(objReader["Id"]);
                                break;
                        }
                    }
                }
                objReader.Close();
                if (PlanBenefit != null)
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("PlanBenefits", PlanBenefit);
                    res.Add("CreatedBy", CreateByName);
                    res.Add("ModifiedBy", ModifiedByName);
                    res.Add("NextRecord", nextRecord != null ? nextRecord.ToString() : "");
                    res.Add("PreviousRecord", preRecord != null ? preRecord.ToString() : "");
                    res.Add("PlanMarketingName", PlanBenefit.PlanMarketingName);
                    res.Add("PlanAttributeId", PlanAttributeId);
                    res.Add("ApprovalStatus", ApprovalStatus);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "Plan Benefit does not exist.");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetPlanBenefitByOrderEvent" + Environment.NewLine;
                ExceptionString += "Request :  " + " Id " + Id + " ,EventType " + EventType + " ,sortby " + sortby + " ,desc " + desc + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetPlanBenefitByOrderEvent - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
            finally
            {
                objcmd.Dispose();
                objConn.Close();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetPlanBenefitByOrderEvent/{IssuerId}/{BenefitNum}")]
        public HttpResponseMessage GetDefaultMHMMappingId(long IssuerId, int BenefitNum)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {

                var lstMHMBenefitMapping = objMHMBenefitMappingMst.GetMHMBenefitMappingMaster();

                var objMapping = lstMHMBenefitMapping.Where(r => r.IssuerID == IssuerId && r.IssuerBenefitID == BenefitNum).FirstOrDefault();

                if (objMapping != null)
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("MHMBenefitId", objMapping.MHMCommonBenefitID);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("MHMBenefitId", 0);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetDefaultMHMMappingId" + Environment.NewLine;
                ExceptionString += "Request :  " + " IssuerId " + IssuerId + " ,BenefitNum " + BenefitNum + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetDefaultMHMMappingId - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/UpdatePlanBenefit")]
        public HttpResponseMessage UpdatePlanBenefit(PlanBenefitMst objPlanBenefit)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    if (DB.PlanBenefitMsts.Any(o => o.Id == objPlanBenefit.Id))
                    {
                        if (DB.PlanBenefitMsts.Any(o => o.PlanId == objPlanBenefit.PlanId && o.BusinessYear == objPlanBenefit.BusinessYear && o.MHMBenefitId == objPlanBenefit.MHMBenefitId && o.Id != objPlanBenefit.Id))
                        {
                            res.Add("Status", "false");
                            res.Add("Message", "Plan Benefit with same PlanId, BusinessYear and MHMBenefitId already exists.");
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                        else if (DB.PlanBenefitMsts.Any(o => o.PlanId == objPlanBenefit.PlanId && o.BusinessYear == objPlanBenefit.BusinessYear && o.BenefitName == objPlanBenefit.BenefitName && o.Id != objPlanBenefit.Id))
                        {
                            res.Add("Status", "false");
                            res.Add("Message", "Plan Benefit with same PlanId, BusinessYear and BenefitName already exists.");
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                        else
                        {
                            objPlanBenefit.MaxCoinsInnTier1Amt = objPlanBenefit.CoinsMaxAmt;
                            objPlanBenefit.MaxCoinsInnTier2Amt = objPlanBenefit.CoinsMaxAmt;
                            objPlanBenefit.ModifiedDateTime = System.DateTime.Now;
                            DB.PlanBenefitMsts.Attach(objPlanBenefit);
                            DB.Entry(objPlanBenefit).State = EntityState.Modified;
                            DB.SaveChanges();

                            //Remove Cache of PlanBenefitMaster
                            var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanBenefitMaster");
                            if (PlanAttributeMasterFromCache != null)
                            {
                                MHMCache.RemoveMyCachedItem("PlanBenefitMaster");
                            }

                            res.Add("Status", "true");
                            res.Add("Message", "Success");
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Plan Benefit does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
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

                    string ExceptionString = "Api : UpdatePlanBenefit" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objPlanBenefit " + JsonConvert.SerializeObject(objPlanBenefit) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdatePlanBenefit - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UpdatePlanBenefit" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objPlanBenefit " + JsonConvert.SerializeObject(objPlanBenefit) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdatePlanBenefit - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/AddPlanBenefit")]
        public HttpResponseMessage AddPlanBenefit(PlanBenefitMst objPlanBenefit)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstobjPlanBenefitMst = objPlanBenefitMst.GetPlanBenefitMaster();

                    if (ModelState.IsValid)
                    {
                        if (lstobjPlanBenefitMst.Any(o => o.PlanId == objPlanBenefit.PlanId && o.BusinessYear == objPlanBenefit.BusinessYear && o.MHMBenefitId == objPlanBenefit.MHMBenefitId))
                        {
                            res.Add("Status", "false");
                            res.Add("Message", "Plan Benefit with same PlanId, BusinessYear and MHMBenefitId already exists.");
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                        else if (lstobjPlanBenefitMst.Any(o => o.PlanId == objPlanBenefit.PlanId && o.BusinessYear == objPlanBenefit.BusinessYear && o.BenefitName == objPlanBenefit.BenefitName))
                        {
                            res.Add("Status", "false");
                            res.Add("Message", "Plan Benefit with same PlanId, BusinessYear and BenefitName already exists.");
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                        else
                        {
                            objPlanBenefit.MaxCoinsInnTier1Amt = objPlanBenefit.CoinsMaxAmt;
                            objPlanBenefit.MaxCoinsInnTier2Amt = objPlanBenefit.CoinsMaxAmt;
                            objPlanBenefit.CreatedDateTime = System.DateTime.Now;
                            objPlanBenefit.ModifiedDateTime = null;
                            DB.PlanBenefitMsts.Add(objPlanBenefit);
                            DB.SaveChanges();

                            //Remove Cache of PlanBenefitMaster
                            var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanBenefitMaster");
                            if (PlanAttributeMasterFromCache != null)
                            {
                                MHMCache.RemoveMyCachedItem("PlanBenefitMaster");
                            }

                            oResponse.Status = true;
                            oResponse.Message = "Plan benefit added successfully";
                            HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return objResponse;
                        }
                    }
                    else
                    {
                        var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                        oResponse.Status = false;
                        oResponse.Message = Messages;
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return objResponse;
                    }

                }
                catch (DbUpdateException ex)
                {
                    SqlException innerException = ex.InnerException.InnerException as SqlException;
                    if (innerException != null && (innerException.Number == 547))
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Invalid PlanId";
                        HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return resp;
                    }
                    oResponse.Status = false;
                    oResponse.Message = !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message.ToString() : ex.Message;

                    string ExceptionString = "Api : AddPlanBenefit" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objPlanBenefit " + JsonConvert.SerializeObject(objPlanBenefit) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddPlanBenefit - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : AddPlanBenefit" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objPlanBenefit " + JsonConvert.SerializeObject(objPlanBenefit) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddPlanBenefit - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetMarketCoverage/{TableName}")]
        public HttpResponseMessage GetMarketCoverage(string TableName)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    List<string> lstMarketCoverage = new List<string>();

                    lstMarketCoverage = DB.PlanAttributeMsts.Select(r => r.MrktCover).Distinct().ToList();
                    if (lstMarketCoverage != null)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("lstMarketCoverage", lstMarketCoverage.OrderBy(r => r));
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Market Coverage does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetMarketCoverage" + Environment.NewLine;
                    ExceptionString += "Request :  " + " TableName " + TableName + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetMarketCoverage - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetMetalLevel/{TableName}")]
        public HttpResponseMessage GetMetalLevel(string TableName)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    List<string> lstMetalLevel = new List<string>();
                    if (TableName == "PlanAttributeMsts")
                    {
                        lstMetalLevel = DB.PlanAttributeMsts.Select(r => r.MetalLevel).Distinct().ToList();
                    }

                    if (lstMetalLevel != null)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("lstMarketCoverage", lstMetalLevel.OrderBy(r => r));
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Market Coverage does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetMetalLevel" + Environment.NewLine;
                    ExceptionString += "Request :  " + " TableName " + TableName + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetMetalLevel - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetEmployerMaster")]
        public HttpResponseMessage GetEmployerMaster()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstEmployerMaster = DB.EmployerMsts.OrderBy(x => x.EmployerName).ToList();

                    if (lstEmployerMaster != null)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("lstEmployerMaster", lstEmployerMaster);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Employer Master does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetEmployerMaster" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetEmployerMaster - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/ExportPlanAttributes")]
        public HttpResponseMessage ExportPlanAttributes([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                var lstPlanAttribute = objPlanAttributeMst.GetPlanAttributeMasterExcel();
                if (!string.IsNullOrEmpty(searchby))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.PlanId.ToLower().Contains(searchby.ToLower())).ToList();
                }

                if (BusinessYear != null)
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.BusinessYear == BusinessYear).ToList();
                }
                //Carrier, Plan, IsActive , StateCode MetalLevel and MarketCover 
                if (!string.IsNullOrEmpty(lstParameter[0]))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.ApprovalStatus == Convert.ToInt64(lstParameter[0])).ToList();
                }
                if (!string.IsNullOrEmpty(lstParameter[1]))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.CarrierId == Convert.ToInt64(lstParameter[1])).ToList();
                }
                if (!string.IsNullOrEmpty(lstParameter[2]))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.BusinessYear == lstParameter[2].ToString()).ToList();
                }
                if (!string.IsNullOrEmpty(lstParameter[3]))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.MrktCover == lstParameter[3].ToString()).ToList();
                }
                if (!string.IsNullOrEmpty(lstParameter[4]))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.GroupName == lstParameter[4].ToString()).ToList();
                }
                if (!string.IsNullOrEmpty(lstParameter[5]))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.OpenForEnrollment == Convert.ToBoolean(lstParameter[5])).ToList();
                }
                if (!string.IsNullOrEmpty(lstParameter[6]))
                {
                    lstPlanAttribute = lstPlanAttribute.Where(x => x.StateCode == lstParameter[5].ToString()).ToList();
                }

                //Vaibhav EmployerId
                //if (!string.IsNullOrEmpty(lstParameter[6]))
                //{
                //    lstPlanAttribute = lstPlanAttribute.Where(x => x.EmployerId == Convert.ToInt64(lstParameter[6])).ToList();
                //}

                int total = lstPlanAttribute.Count();

                if (total > 0)
                {
                    //lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList() : lstPlanAttribute.OrderBy(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList();
                    switch (sortby)
                    {
                        case "PlanId":
                            lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.PlanId).ToList() : lstPlanAttribute.OrderBy(x => x.PlanId).ToList();
                            break;

                        case "OpenForEnrollment":
                            lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.OpenForEnrollment).ToList() : lstPlanAttribute.OrderBy(x => x.OpenForEnrollment).ToList();
                            break;

                        case "Id":
                            lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.Id).ToList() : lstPlanAttribute.OrderBy(x => x.Id).ToList();
                            break;

                        case "Carrier":
                            lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.IssuerMst.IssuerName).ToList() : lstPlanAttribute.OrderBy(x => x.IssuerMst.IssuerName).ToList();
                            break;

                        case "PlanMarketingName":
                            lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.PlanMarketingName).ToList() : lstPlanAttribute.OrderBy(x => x.PlanMarketingName).ToList();
                            break;

                        case "GroupName":
                            lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.GroupName).ToList() : lstPlanAttribute.OrderBy(x => x.GroupName).ToList();
                            break;

                        default:
                            lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.CreatedDateTime).ToList() : lstPlanAttribute.OrderBy(x => x.CreatedDateTime).ToList();
                            break;
                    }


                    System.Data.DataTable dtPlanAttributes = ConvertToDataTable(lstPlanAttribute.ToList());

                    dtPlanAttributes.TableName = "PlanAttributes";

                    MemoryStream stream = new MemoryStream();
                    Stream fs = new MemoryStream();

                    //Create with ClosedXML
                    XLWorkbook xlWorkbook = new XLWorkbook();
                    xlWorkbook.Worksheets.Add(dtPlanAttributes);
                    xlWorkbook.SaveAs(fs);
                    fs.Position = 0;
                    fs.CopyTo(stream);

                    byte[] bytes = stream.ToArray();

                    System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes);

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StreamContent(memStream);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = "PlanAttributes.xlsx";
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/ms-excel");
                    response.Content.Headers.ContentLength = memStream.Length;
                    return response;
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "PlanAttributes does not exist.");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/ExportCSRRates")]
        public HttpResponseMessage ExportCSRRates([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstCSRRate = objCSRRateMst.GetCSRRateMaster();
                    if (!string.IsNullOrEmpty(searchby))
                    {
                        lstCSRRate = lstCSRRate.Where(x => x.PlanID.ToLower().Contains(searchby.ToLower())).ToList();
                    }

                    if (BusinessYear != null)
                    {
                        lstCSRRate = lstCSRRate.Where(x => x.BusinessYear == BusinessYear).ToList();
                    }

                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        lstCSRRate = lstCSRRate.Where(x => x.RatingAreaId == Convert.ToInt64(lstParameter[0])).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]))
                    {
                        var MarketCoverage = lstParameter[1].ToString();
                        var plans = DB.PlanAttributeMsts.Where(r => r.MrktCover == MarketCoverage)
                            .Select(t => t.StandardComponentId).ToList();
                        lstCSRRate = lstCSRRate.Where(x => plans.Contains(x.PlanID)).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[2]))
                    {
                        var IssuerId = Convert.ToInt64(lstParameter[2]);
                        var plans = DB.PlanAttributeMsts.Where(r => r.CarrierId == IssuerId)
                            .Select(t => t.StandardComponentId).ToList();
                        lstCSRRate = lstCSRRate.Where(x => plans.Contains(x.PlanID)).ToList();
                    }

                    int total = lstCSRRate.Count();

                    //SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ToString());
                    //SqlCommand objcmd = new SqlCommand("get_CSR_Rate_Mst_List", objConn);
                    //SqlDataAdapter da = new SqlDataAdapter();
                    //objcmd.CommandType = CommandType.StoredProcedure;

                    //if (!string.IsNullOrEmpty(searchby))
                    //{
                    //    objcmd.Parameters.Add("@Searchby", SqlDbType.VarChar).Value = searchby;
                    //}

                    //if (BusinessYear != null)
                    //{
                    //    objcmd.Parameters.Add("@BusinessYear", SqlDbType.VarChar).Value = BusinessYear;
                    //}

                    //if (!string.IsNullOrEmpty(lstParameter[0]))
                    //{
                    //    objcmd.Parameters.Add("@RatingAreaId", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[0]);
                    //}
                    //if (!string.IsNullOrEmpty(lstParameter[1]))
                    //{
                    //    objcmd.Parameters.Add("@MrktCover", SqlDbType.VarChar).Value = lstParameter[1].ToString();
                    //}
                    //if (!string.IsNullOrEmpty(lstParameter[2]))
                    //{
                    //    objcmd.Parameters.Add("@CarrierId", SqlDbType.BigInt).Value = Convert.ToInt64(lstParameter[2]);
                    //}

                    //objcmd.Parameters.Add("@SortColumn", SqlDbType.VarChar).Value = sortby;
                    //if (desc) { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "desc"; }
                    //else { objcmd.Parameters.Add("@SortOrder", SqlDbType.VarChar).Value = "asc"; }

                    //objcmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = 200000;
                    //da.SelectCommand = objcmd;
                    //DataTable dtCSRRates = new DataTable("CSRRates");
                    //da.Fill(dtCSRRates);

                    if (total > 0)
                    {
                        switch (sortby)
                        {
                            case "Id":
                                lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.Id).ToList() : lstCSRRate.OrderBy(x => x.Id).ToList();
                                break;

                            case "PlanID":
                                lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.PlanID).ToList() : lstCSRRate.OrderBy(x => x.PlanID).ToList();
                                break;

                            case "RatingAreaId":
                                lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.RatingAreaId).ToList() : lstCSRRate.OrderBy(x => x.RatingAreaId).ToList();
                                break;

                            case "Age":
                                lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.Age).ToList() : lstCSRRate.OrderBy(x => x.Age).ToList();
                                break;

                            default:
                                lstCSRRate = desc ? lstCSRRate.OrderByDescending(x => x.CreatedDateTime).ToList() : lstCSRRate.OrderBy(x => x.CreatedDateTime).ToList();
                                break;
                        }

                        System.Data.DataTable dtCSRRates = ConvertToDataTable(lstCSRRate.ToList());

                        dtCSRRates.TableName = "CSRRates";

                        MemoryStream stream = new MemoryStream();
                        Stream fs = new MemoryStream();

                        //Create with ClosedXML
                        XLWorkbook xlWorkbook = new XLWorkbook();
                        xlWorkbook.Worksheets.Add(dtCSRRates);
                        xlWorkbook.SaveAs(fs);
                        fs.Position = 0;
                        fs.CopyTo(stream);

                        byte[] bytes = stream.ToArray();

                        System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes);

                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StreamContent(memStream);
                        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = "CSRRates.xlsx";
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/ms-excel");
                        response.Content.Headers.ContentLength = memStream.Length;
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "CSR Rates does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/ExportPlanBenefits")]
        public HttpResponseMessage ExportPlanBenefits([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                var lstobjPlanBenefitMst = objPlanBenefitMst.GetPlanBenefitMaster();
                //  var test = objPlanBenefitMst.GetPlanBenefitMaster().Where(r => r.MarketConverage == lstParameter[0].ToString()).ToList();
                if (!string.IsNullOrEmpty(searchby))
                {
                    lstobjPlanBenefitMst = lstobjPlanBenefitMst.Where(x => x.PlanId.ToLower().Contains(searchby.ToLower())).ToList();
                }

                if (BusinessYear != null)
                {
                    lstobjPlanBenefitMst = lstobjPlanBenefitMst.Where(x => x.BusinessYear == BusinessYear).ToList();
                }


                if (!string.IsNullOrEmpty(lstParameter[0]))
                {
                    lstobjPlanBenefitMst = lstobjPlanBenefitMst.Where(x => x.PlanId == lstParameter[0].ToString()).ToList();
                }
                if (!string.IsNullOrEmpty(lstParameter[1]))
                {
                    lstobjPlanBenefitMst = lstobjPlanBenefitMst.Where(x => x.MHMBenefitId == Convert.ToInt64(lstParameter[1]));
                }
                if (!string.IsNullOrEmpty(lstParameter[2]))
                {
                    lstobjPlanBenefitMst = lstobjPlanBenefitMst.Where(x => x.PlanAttributeMst.CarrierId == Convert.ToInt64(lstParameter[2]));
                    //oModellst = oModellst.Where(x => x.CreatedBy.ToLower().Contains(lstParameter[2].ToLower())).ToList();
                }
                //if (!string.IsNullOrEmpty(lstParameter[3]))
                //{
                //    oModellst = oModellst.Where(x => x.MobileNo.Contains(lstParameter[3])).ToList();
                //}
                res.Add("PlanIds", lstobjPlanBenefitMst.Select(x => x.PlanId).ToList());
                int total = lstobjPlanBenefitMst.Count();

                if (total > 0)
                {
                    //lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.BenefitKey).Skip(skipRows).Take(pageSize).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.BenefitKey).Skip(skipRows).Take(pageSize).ToList();

                    switch (sortby)
                    {
                        //case "Id":
                        //    lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.Id).Skip(skipRows).Take(pageSize).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.Id).Skip(skipRows).Take(pageSize).ToList();
                        //    break;

                        //case "BenefitKey":
                        //    lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.BenefitKey).Skip(skipRows).Take(pageSize).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.BenefitKey).Skip(skipRows).Take(pageSize).ToList();
                        //    break;

                        //case "BenefitName":
                        //    lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.BenefitName).Skip(skipRows).Take(pageSize).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.BenefitName).Skip(skipRows).Take(pageSize).ToList();
                        //    break;

                        //case "CaseTitle":
                        //    oModellst = desc ? oModellst.OrderByDescending(x => x.CaseTitle).Skip(skipRows).Take(pageSize).ToList() : oModellst.OrderBy(x => x.CaseTitle).Skip(skipRows).Take(pageSize).ToList();
                        //    break;

                        case "PlanId":
                            lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.PlanId).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.PlanId).ToList();
                            break;

                        case "MHMBenefitId":
                            lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.MHMBenefitId).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.MHMBenefitId).ToList();
                            break;

                        case "BenefitName":
                            lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.BenefitName).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.BenefitName).ToList();
                            break;

                        case "IsCovered":
                            lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.IsCovered).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.IsCovered).ToList();
                            break;

                        case "CostShareType1":
                            lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.CostSharingType1).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.CostSharingType1).ToList();
                            break;


                        case "CostShareType2":
                            lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.CostSharingType2).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.CostSharingType2).ToList();
                            break;

                        default:
                            lstobjPlanBenefitMst = desc ? lstobjPlanBenefitMst.OrderByDescending(x => x.CreatedDateTime).ToList() : lstobjPlanBenefitMst.OrderBy(x => x.CreatedDateTime).ToList();
                            break;
                    }

                    var lstItems = lstobjPlanBenefitMst.Select(r => new
                    {
                        r.Id,
                        r.PlanId,
                        r.BenefitName,
                        r.MHMBenefitId,
                        r.CostSharingType1,
                        r.CopayInnTier1,
                        r.CoinsInnTier1,
                        r.CostSharingType2,
                        r.CopayInnTier2,
                        r.CoinsInnTier2,
                        r.QuantLimitOnSvc,
                        r.LimitQty,
                        r.LimitUnit,
                        r.IsCovered,
                        r.IsExclFromInnMOOP,
                        r.CoinsMaxAmt,
                        r.MaxQtyBeforeCoPay,
                        r.MaxCoinsInnTier1Amt,
                        r.MaxCoinsInnTier2Amt,
                        r.CopayInnTier1Desc,
                        r.CoinsInnTier1Desc,
                        r.CopayOutofNet,
                        r.CoinsOutofNet,
                        r.IsExclFromOonMOOP,
                        r.BusinessYear,
                        r.BenefitNum,
                        r.SourceName,
                        r.CreatedDateTime,
                        r.ModifiedDateTime,
                        r.Createdby,
                        r.ModifiedBy,
                        r.Exclusions,
                        r.BenefitDeductible,
                        r.IssuerName,
                        r.IssuerId,
                        r.PlanMarketingName
                    }).ToList();
                    System.Data.DataTable dtPlanBenefits = ConvertToDataTable(lstItems);

                    dtPlanBenefits.TableName = "PlanBenefits";

                    MemoryStream stream = new MemoryStream();
                    Stream fs = new MemoryStream();

                    //Create with ClosedXML
                    XLWorkbook xlWorkbook = new XLWorkbook();
                    xlWorkbook.Worksheets.Add(dtPlanBenefits);
                    xlWorkbook.SaveAs(fs);
                    fs.Position = 0;
                    fs.CopyTo(stream);

                    byte[] bytes = stream.ToArray();

                    System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes);

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StreamContent(memStream);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = "PlanBenefits.xlsx";
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/ms-excel");
                    response.Content.Headers.ContentLength = memStream.Length;
                    return response;
                }
                else
                {
                    res.Add("Status", "false");
                    res.Add("Message", "Plan Benefits does not exist.");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        private System.Data.DataTable ConvertToDataTable<T>(IList<T> data)
        {
            System.ComponentModel.PropertyDescriptorCollection properties =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
                //break;
            }
            return table;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetJobMasters")]
        public HttpResponseMessage GetJobMasters([FromUri]  List<string> lstParameter, string JobYear, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    int skipRows = (page - 1) * pageSize;

                    var lstJobMasters = DB.JobMasters.ToList();
                    if (!string.IsNullOrEmpty(searchby))
                    {
                        lstJobMasters = lstJobMasters.Where(x => x.JobNumber.ToLower().Contains(searchby.ToLower())).ToList();
                    }

                    if (JobYear != null)
                    {
                        lstJobMasters = lstJobMasters.Where(x => x.JobYear == JobYear).ToList();
                    }
                    //Carrier, Plan, IsActive , StateCode MetalLevel and MarketCover 
                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        lstJobMasters = lstJobMasters.Where(x => x.EmployerId == Convert.ToInt64(lstParameter[0])).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]))
                    {
                        lstJobMasters = lstJobMasters.Where(x => x.JobStatus == lstParameter[1].ToString()).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[2]))
                    {
                        lstJobMasters = lstJobMasters.Where(x => x.JobDateStart >= Convert.ToDateTime(lstParameter[2])).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[3]))
                    {
                        lstJobMasters = lstJobMasters.Where(x => x.JobDateEnd <= Convert.ToDateTime(lstParameter[3])).ToList();
                    }

                    int total = lstJobMasters.Count();

                    if (total > 0)
                    {
                        //lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList() : lstPlanAttribute.OrderBy(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList();
                        switch (sortby)
                        {
                            case "JobNumber":
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.JobNumber).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.JobNumber).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "EmployerName":
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.EmployerMst.EmployerName).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.EmployerMst.EmployerName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "JobStatus":
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.JobStatus).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.JobStatus).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "JobYear":
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.JobYear).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.JobYear).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "JobDateStart":
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.JobDateStart).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.JobDateStart).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "JobDateEnd":
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.JobDateEnd).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.JobDateEnd).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "CreatedDateTime":
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            default:
                                lstJobMasters = desc ? lstJobMasters.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lstJobMasters.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }

                        lstJobMasters.ForEach(r => r.EmployerMst = r.EmployerMst);

                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("lstJobMasters", lstJobMasters.Select(r =>
                            new
                            {
                                r.EmployerId,
                                EmployerName = r.EmployerId != null ? r.EmployerMst.EmployerName : null,
                                r.JobDesc,
                                r.JobStatus,
                                r.JobDateStart,
                                r.JobDateEnd,
                                r.JobYear,
                                r.CreatedDateTime,
                                r.ModifiedDateTime,
                                r.JobNumber,
                                r.EmailBodyText,
                                r.EmailSubjText,
                                r.EmailSignText
                            }));
                        res.Add("TotalCount", total);
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", "failed");
                        res.Add("Message", "JobMaster does not exist");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.ToString();

                    string ExceptionString = "Api : GetJobMasters" + Environment.NewLine;
                    ExceptionString += "Request :  " + " lstParameter " + lstParameter + " ,JobYear " + JobYear + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetJobMasters - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetJobMastersDetail/{JobNumber}")]
        public HttpResponseMessage GetJobMastersDetail(string JobNumber)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var JobMasters = DB.JobMasters.Where(r => r.JobNumber == JobNumber).FirstOrDefault();
                    JobMasters.JobRunStatus = JobMasters.JobRunStatus == null ? "NULL" : JobMasters.JobRunStatus;
                    string CreateByName = DB.Users.Where(r => r.UserID == JobMasters.Createdby).FirstOrDefault() != null ? DB.Users.Where(r => r.UserID == JobMasters.Createdby).FirstOrDefault().FirstName + " " + DB.Users.Where(r => r.UserID == JobMasters.Createdby).FirstOrDefault().LastName : "";
                    string ModifiedByName = DB.Users.Where(r => r.UserID == JobMasters.ModifiedBy).FirstOrDefault() != null ? DB.Users.Where(r => r.UserID == JobMasters.ModifiedBy).FirstOrDefault().FirstName + " " + DB.Users.Where(r => r.UserID == JobMasters.ModifiedBy).FirstOrDefault().LastName : "";

                    if (JobMasters != null)
                    {
                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("JobMasters", JobMasters);
                        res.Add("CreatedByName", CreateByName);
                        res.Add("ModifiedByName", ModifiedByName);
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", "failed");
                        res.Add("Message", "JobMaster does not exist");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetJobMastersDetail" + Environment.NewLine;
                    ExceptionString += "Request :  " + " JobNumber " + JobNumber + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetJobMastersDetail - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetJobMasters")]
        public HttpResponseMessage GetJobMasters()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var JobMasters = DB.JobMasters.OrderByDescending(r => r.CreatedDateTime).Select(t => new { t.JobNumber, t.JobDesc, t.JobYear, t.EmployerMst.EmployerName, t.EmployerId, t.CaseZipCode, t.PlanYearStartDt, t.PlanYearEndDt, t.WellnessOffered, t.PayPeriodsPerYear }).ToList();
                    if (JobMasters.Count > 0)
                    {
                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("JobMasters", JobMasters);
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", "failed");
                        res.Add("Message", "JobMaster does not exist");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetJobMasters" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetJobMasters - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/AddJobMaster")]
        public HttpResponseMessage AddJobMaster(JobMaster objJobMaster)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        objJobMaster.CreatedDateTime = System.DateTime.Now;
                        if (objJobMaster.OldJobNumber != null && objJobMaster.OldJobNumber != "")
                        {
                            using (var dbContextTransaction = DB.Database.BeginTransaction())
                            {
                                try
                                {
                                    objJobMaster.Createdby = objJobMaster.Createdby;
                                    objJobMaster.ModifiedBy = objJobMaster.ModifiedBy;
                                    objJobMaster.ModifiedDateTime = null;
                                    objJobMaster.JobCopiedFrom = objJobMaster.OldJobNumber;
                                    DB.JobMasters.Add(objJobMaster);
                                    DB.SaveChanges();

                                    var lstSelectedPlans = DB.JobPlansMsts.Where(r => r.JobNumber == objJobMaster.OldJobNumber).ToList();
                                    lstSelectedPlans.ForEach(r => r.JobNumber = objJobMaster.JobNumber);
                                    DB.JobPlansMsts.AddRange(lstSelectedPlans);
                                    DB.SaveChanges();

                                    var lstCases = DB.Cases.Where(r => r.JobNumber == objJobMaster.OldJobNumber).Include(r => r.Families).Include(r => r.CasePlanResults).ToList();
                                    List<Case> lstNewCases = new List<Case>();
                                    foreach (var oCase in lstCases)
                                    {
                                        Applicant oApplicant = new Applicant();
                                        oApplicant.EmployerId = oCase.Applicant.EmployerId;
                                        oApplicant.FirstName = oCase.Applicant.FirstName;
                                        oApplicant.LastName = oCase.Applicant.LastName;
                                        oApplicant.CurrentPlan = oCase.Applicant.CurrentPlan;
                                        oApplicant.CurrentPremium = oCase.Applicant.CurrentPremium;
                                        oApplicant.Origin = oCase.Applicant.Origin;
                                        oApplicant.City = oCase.Applicant.City;
                                        oApplicant.State = oCase.Applicant.State;
                                        oApplicant.Street = oCase.Applicant.Street;
                                        oApplicant.Zip = oCase.Applicant.Zip;
                                        oApplicant.Email = oCase.Applicant.Email;
                                        oApplicant.Mobile = oCase.Applicant.Mobile;
                                        oApplicant.InsuranceTypeId = oCase.Applicant.InsuranceTypeId;
                                        oApplicant.Createdby = oCase.Createdby;
                                        oApplicant.CreatedDateTime = oCase.CreatedDateTime;
                                        oApplicant.ModifiedBy = objJobMaster.ModifiedBy;
                                        oApplicant.ModifiedDateTime = DateTime.Now;

                                        Case OCaseModel = new Case();
                                        OCaseModel.Applicant = oApplicant;
                                        OCaseModel.CaseTitle = oCase.CaseTitle;
                                        OCaseModel.Createdby = oCase.Createdby;
                                        OCaseModel.CreatedDateTime = oCase.CreatedDateTime;
                                        OCaseModel.FPL = oCase.FPL;
                                        OCaseModel.HSAAmount = oCase.HSAAmount;
                                        OCaseModel.HSAFunding = oCase.HSAFunding;
                                        OCaseModel.HSALimit = oCase.HSALimit;
                                        OCaseModel.IssuerID = oCase.IssuerID;
                                        OCaseModel.MAGIncome = oCase.MAGIncome;
                                        OCaseModel.MonthlySubsidy = oCase.MonthlySubsidy;
                                        OCaseModel.Notes = oCase.Notes;
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
                                        OCaseModel.IsSubsidy = oCase.IsSubsidy;
                                        OCaseModel.StatusId = oCase.StatusId;
                                        OCaseModel.JobNumber = objJobMaster.JobNumber;
                                        OCaseModel.CaseSource = "Case copied from job " + objJobMaster.OldJobNumber;
                                        OCaseModel.CaseJobRunStatus = oCase.CaseJobRunStatus;
                                        OCaseModel.DedBalAvailDate = oCase.DedBalAvailDate;
                                        OCaseModel.DedBalAvailToRollOver = oCase.DedBalAvailToRollOver;
                                        OCaseModel.TierIntention = oCase.TierIntention;
                                        OCaseModel.CaseReferenceId = oCase.CaseID;
                                        OCaseModel.ApplicantID = oApplicant.ApplicantID;
                                        OCaseModel.ModifiedBy = objJobMaster.ModifiedBy;
                                        OCaseModel.ModifiedDateTime = DateTime.Now;

                                        List<Family> lstFamily = new List<Family>();
                                        foreach (var itemFM in oCase.Families)
                                        {

                                            Family oFamily = new Family()
                                            {
                                                Gender = itemFM.Gender,
                                                DOB = itemFM.DOB,
                                                Age = CalculateAge(Convert.ToDateTime(GenerateEncryptedString.GetDecryptedString(itemFM.DOB))),
                                                Createdby = itemFM.Createdby,
                                                CreatedDateTime = itemFM.CreatedDateTime,
                                                IsPrimary = itemFM.IsPrimary,
                                                Smoking = itemFM.Smoking,
                                                TotalMedicalUsage = itemFM.TotalMedicalUsage,
                                                CaseNumId = OCaseModel.CaseID,
                                                ModifiedBy = objJobMaster.ModifiedBy,
                                                ModifiedDateTime = DateTime.Now
                                            };

                                            List<BenefitUserDetail> lstBenefitUserDetail = new List<BenefitUserDetail>();
                                            foreach (var itemBUD in itemFM.BenefitUserDetails)
                                            {
                                                BenefitUserDetail benefitUserDetail = new BenefitUserDetail();
                                                benefitUserDetail.UsageCost = itemBUD.UsageCost;
                                                benefitUserDetail.UsageQty = itemBUD.UsageQty;
                                                benefitUserDetail.UsageNotes = itemBUD.UsageNotes;
                                                benefitUserDetail.Createdby = itemBUD.Createdby;
                                                benefitUserDetail.CreatedDateTime = itemBUD.CreatedDateTime;
                                                benefitUserDetail.FamilyID = oFamily.FamilyID;
                                                benefitUserDetail.MHMMappingBenefitId = itemBUD.MHMMappingBenefitId;
                                                benefitUserDetail.ModifiedBy = objJobMaster.ModifiedBy;
                                                benefitUserDetail.ModifiedDateTime = DateTime.Now;
                                                lstBenefitUserDetail.Add(benefitUserDetail);
                                            }

                                            List<Criticalillness> lstCriticalillness = new List<Criticalillness>();
                                            foreach (var itemCI in itemFM.Criticalillnesses)
                                            {
                                                Criticalillness criticalillness = new Criticalillness();
                                                criticalillness.IllnessId = itemCI.IllnessId;
                                                criticalillness.Createdby = itemCI.Createdby;
                                                criticalillness.CreatedDateTime = itemCI.CreatedDateTime;
                                                criticalillness.FamilyID = oFamily.FamilyID;
                                                criticalillness.ModifiedBy = objJobMaster.ModifiedBy;
                                                criticalillness.ModifiedDateTime = DateTime.Now;
                                                lstCriticalillness.Add(criticalillness);
                                            }

                                            oFamily.BenefitUserDetails = lstBenefitUserDetail;
                                            oFamily.Criticalillnesses = lstCriticalillness;

                                            lstFamily.Add(oFamily);
                                        }
                                        OCaseModel.Families = lstFamily;

                                        List<CasePlanResult> CasePlanResults = new List<CasePlanResult>();
                                        foreach (var itemCPR in oCase.CasePlanResults)
                                        {
                                            CasePlanResult casePlanResult = new CasePlanResult();
                                            casePlanResult.CaseId = OCaseModel.CaseID;
                                            casePlanResult.GrossAnnualPremium = itemCPR.GrossAnnualPremium;
                                            casePlanResult.FederalSubsidy = itemCPR.FederalSubsidy;
                                            casePlanResult.NetAnnualPremium = itemCPR.NetAnnualPremium;
                                            casePlanResult.MonthlyPremium = itemCPR.MonthlyPremium;
                                            casePlanResult.Copays = itemCPR.Copays;
                                            casePlanResult.PaymentsToDeductibleLimit = itemCPR.PaymentsToDeductibleLimit;
                                            casePlanResult.CoinsuranceToOutOfPocketLimit = itemCPR.CoinsuranceToOutOfPocketLimit;
                                            casePlanResult.ContributedToYourHSAAccount = itemCPR.ContributedToYourHSAAccount;
                                            casePlanResult.TaxSavingFromHSAAccount = itemCPR.TaxSavingFromHSAAccount;
                                            casePlanResult.Medical = itemCPR.Medical;
                                            casePlanResult.TotalPaid = itemCPR.TotalPaid;
                                            casePlanResult.PaymentsByInsuranceCo = itemCPR.PaymentsByInsuranceCo;
                                            casePlanResult.DeductibleSingle = itemCPR.DeductibleSingle;
                                            casePlanResult.DeductibleFamilyPerPerson = itemCPR.DeductibleFamilyPerPerson;
                                            casePlanResult.DeductibleFamilyPerGroup = itemCPR.DeductibleFamilyPerGroup;
                                            casePlanResult.OPLSingle = itemCPR.OPLSingle;
                                            casePlanResult.OPLFamilyPerPerson = itemCPR.OPLFamilyPerPerson;
                                            casePlanResult.OPLFamilyPerGroup = itemCPR.OPLFamilyPerGroup;
                                            casePlanResult.Coinsurance = itemCPR.Coinsurance;
                                            casePlanResult.WorstCase = itemCPR.WorstCase;
                                            casePlanResult.HRAReimbursedAmt = itemCPR.HRAReimbursedAmt;
                                            casePlanResult.Createdby = casePlanResult.Createdby;
                                            casePlanResult.CreatedDateTime = casePlanResult.CreatedDateTime;
                                            casePlanResult.ModifiedBy = objJobMaster.ModifiedBy;
                                            casePlanResult.ModifiedDateTime = DateTime.Now;
                                            CasePlanResults.Add(casePlanResult);
                                        }
                                        OCaseModel.CasePlanResults = CasePlanResults;

                                        lstNewCases.Add(OCaseModel);
                                    }

                                    DB.Cases.AddRange(lstNewCases);
                                    DB.SaveChanges();

                                    dbContextTransaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    throw ex;
                                }
                            }
                        }
                        else
                        {
                            DB.JobMasters.Add(objJobMaster);
                            DB.SaveChanges();
                        }

                        oResponse.Status = true;
                        oResponse.Message = "JobMaster added successfully";
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return objResponse;
                    }
                    else
                    {
                        var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                        oResponse.Status = false;
                        oResponse.Message = Messages;
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return objResponse;
                    }
                }
                catch (DbUpdateException ex)
                {
                    SqlException innerException = ex.InnerException.InnerException as SqlException;
                    if (innerException != null && (innerException.Number == 2627 || innerException.Number == 2601))
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Job Number already exist";
                        HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return resp;
                    }
                    oResponse.Status = false;
                    oResponse.Message = ex.InnerException != null ? ex.InnerException.Message.ToString() : ex.Message;

                    string ExceptionString = "Api : AddJobMaster" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objJobMaster " + objJobMaster + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddJobMaster - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.InnerException != null ? ex.InnerException.Message.ToString() : ex.Message;

                    string ExceptionString = "Api : AddJobMaster" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objJobMaster " + objJobMaster + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddJobMaster - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        // [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetJobPlans")]
        public HttpResponseMessage GetJobPlans([FromUri] List<string> filters, string JobNumber)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    List<JobPlanData> lstPlans;
                    List<JobPlanData> lstPlansNotSelected;
                    List<PlanAttributeMst> lstPlans1 = new List<PlanAttributeMst>();
                    string JobPlansData = string.Empty;


                    List<PlanAttributeMst> lstJobPlans = DB.JobPlansMsts.Where(r => r.JobNumber == JobNumber).Select(r => r.PlanAttributeMst).ToList();
                    lstPlans1 = DB.PlanAttributeMsts.ToList().Where(r => !lstJobPlans.Contains(r)).ToList();
                    //lstPlans1 = DB.PlanAttributeMsts.ToList().Where(r => !lstJobPlans.Contains(r) && r.ApprovalStatus == 5).ToList();
                    //lstPlans1 = DB.PlanAttributeMsts.ToList().Where(r => !lstJobPlans.Any(t => t.Id == r.Id)).ToList();
                    if (filters != null && filters.Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(filters[0]))
                        {
                            lstPlans1 = lstPlans1.Where(r => r.CarrierId.ToString() == filters[0]).ToList();
                        }
                        if (!string.IsNullOrEmpty(filters[1]))
                        {
                            lstPlans1 = lstPlans1.Where(r => r.StateCode.ToString() == filters[1].ToString()).ToList();
                        }
                        if (!string.IsNullOrEmpty(filters[2]))
                        {
                            lstPlans1 = lstPlans1.Where(r => r.PlanType.ToString() == filters[2]).ToList();
                        }
                        if (!string.IsNullOrEmpty(filters[3]))
                        {
                            lstPlans1 = lstPlans1.Where(r => r.BusinessYear == filters[3]).ToList();
                        }
                        if (!string.IsNullOrEmpty(filters[4]))
                        {
                            lstPlans1 = lstPlans1.Where(r => r.GroupName == filters[4]).ToList();
                        }
                        if (!string.IsNullOrEmpty(filters[5]))
                        {
                            lstPlans1 = lstPlans1.Where(r => r.MrktCover == filters[5]).ToList();
                        }
                    }
                    lstPlans = lstJobPlans.Select(s => new JobPlanData { Id = s.Id, SelectedPlanTitle = (s.PlanId + ":" + s.PlanMarketingName + "-" + s.BusinessYear) }).ToList();
                    lstPlansNotSelected = lstPlans1.Select(s => new JobPlanData { Id = s.Id, SelectedPlanTitle = (s.PlanId + ":" + s.PlanMarketingName + "-" + s.BusinessYear) }).ToList();

                    if (lstPlansNotSelected.Count > 0 || lstPlans.Count > 0)
                    {
                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("SelectedPlans", lstPlans);
                        res.Add("NotSelectedPlans", lstPlansNotSelected);
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", "failed");
                        res.Add("Message", "JobPlans does not exist");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetJobPlans" + Environment.NewLine;
                    ExceptionString += "Request :  " + " filters " + filters + " ,JobNumber " + JobNumber + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetJobPlans - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/AddJobPlans")]
        public HttpResponseMessage AddJobPlans(JobPlanDetails JobPlanDetails)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    List<JobPlansMst> selectedJobPlans;
                    List<JobPlansMst> jobPlans = DB.JobPlansMsts.Where(x => x.JobNumber == JobPlanDetails.JobNumber).ToList();
                    if (jobPlans != null && jobPlans.Count() > 0)
                    {
                        DB.JobPlansMsts.RemoveRange(jobPlans);
                        DB.SaveChanges();
                    }
                    selectedJobPlans = DB.PlanAttributeMsts.Where(x => JobPlanDetails.SelectedPlanIds.Contains(x.Id)).ToList().Select(s => new JobPlansMst { JobNumber = JobPlanDetails.JobNumber, PlanId = s.PlanId, BusinessYear = s.BusinessYear }).ToList();

                    if (selectedJobPlans != null && selectedJobPlans.Count > 0)
                    {
                        DB.JobPlansMsts.AddRange(selectedJobPlans);
                        DB.SaveChanges();
                    }

                    oResponse.Status = true;
                    oResponse.Message = "JobPlansMaster added successfully";
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return objResponse;

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message.ToString() : ex.Message;

                    string ExceptionString = "Api : AddJobPlans" + Environment.NewLine;
                    ExceptionString += "Request :  " + " JobPlanDetails " + JsonConvert.SerializeObject(JobPlanDetails) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddJobPlans - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/UpdateJobMaster")]
        public HttpResponseMessage UpdateJobMaster(JobMaster objJobMaster)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        JobMaster objJob = DB.JobMasters.Where(r => r.JobNumber == objJobMaster.JobNumber).FirstOrDefault();
                        if (objJob != null)
                        {
                            objJob.JobNumber = objJobMaster.JobNumber;
                            objJob.EmployerId = objJobMaster.EmployerId;
                            objJob.JobDesc = objJobMaster.JobDesc;
                            objJob.JobStatus = objJobMaster.JobStatus;
                            objJob.JobDateStart = objJobMaster.JobDateStart;
                            objJob.JobDateEnd = objJobMaster.JobDateEnd;
                            objJob.JobYear = objJobMaster.JobYear;
                            objJob.JobType = objJobMaster.JobType;

                            objJob.ComparisonJobNum = objJobMaster.ComparisonJobNum;
                            objJob.JobCopiedFrom = objJobMaster.JobCopiedFrom;
                            objJob.AcctMgr = objJobMaster.AcctMgr;
                            objJob.ExpectedCompletionDt = objJobMaster.ExpectedCompletionDt;
                            objJob.PlanYearStartDt = objJobMaster.PlanYearStartDt;
                            objJob.PlanYearEndDt = objJobMaster.PlanYearEndDt;
                            objJob.JobPlansSelectionLockedDt = objJobMaster.JobPlansSelectionLockedDt;
                            objJob.JobPlansSelectionLocked = objJobMaster.JobPlansSelectionLocked;
                            objJob.JobCensusImportDt = objJobMaster.JobCensusImportDt;
                            objJob.HRAMaxReimbursePrimary = objJobMaster.HRAMaxReimbursePrimary;
                            objJob.HRAMaxReimburseDependent = objJobMaster.HRAMaxReimburseDependent;
                            objJob.HRADedLimitPrimary = objJobMaster.HRADedLimitPrimary;
                            objJob.HRADedLimitDependent = objJobMaster.HRADedLimitDependent;
                            objJob.HRACanCoverPremium = objJobMaster.HRACanCoverPremium;
                            objJob.HRAReimburseRatePrimary = objJobMaster.HRAReimburseRatePrimary;
                            objJob.HRAReimburseRateDependent = objJobMaster.HRAReimburseRateDependent;

                            objJob.IsHSAMatch = objJobMaster.IsHSAMatch;
                            objJob.HSAMatchLimit1 = objJobMaster.HSAMatchLimit1;
                            objJob.HSAMatchRate1 = objJobMaster.HSAMatchRate1;
                            objJob.HSAMatchLimit2 = objJobMaster.HSAMatchLimit2;
                            objJob.HSAMatchRate2 = objJobMaster.HSAMatchRate2;
                            objJob.HSAMatchLimit3 = objJobMaster.HSAMatchLimit3;
                            objJob.HSAMatchRate3 = objJobMaster.HSAMatchRate3;
                            objJob.HSAMatchLimit4 = objJobMaster.HSAMatchLimit4;
                            objJob.HSAMatchRate4 = objJobMaster.HSAMatchRate4;

                            objJob.PayPeriodsPerYear = objJobMaster.PayPeriodsPerYear;
                            objJob.WellnessOffered = objJobMaster.WellnessOffered;

                            objJob.ModifiedDateTime = System.DateTime.Now;
                            objJob.ModifiedBy = objJobMaster.ModifiedBy;
                            objJob.EmailSubjText = objJobMaster.EmailSubjText;
                            objJob.EmailBodyText = objJobMaster.EmailBodyText;
                            objJob.EmailSignText = objJobMaster.EmailSignText;
                            objJob.CaseZipCode = objJobMaster.CaseZipCode;
                            DB.SaveChanges();

                            oResponse.Status = true;
                            oResponse.Message = "Success";
                            HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return objResponse;
                        }
                        else
                        {
                            oResponse.Status = true;
                            oResponse.Message = "JobMaster does not exist";
                            HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return objResponse;
                        }
                    }
                    else
                    {
                        var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                        oResponse.Status = false;
                        oResponse.Message = Messages;
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.InnerException.ToString();

                    string ExceptionString = "Api : UpdateJobMaster" + Environment.NewLine;
                    ExceptionString += "Request :  " + " objJobMaster " + JsonConvert.SerializeObject(objJobMaster) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdateJobMaster - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetJobStatus")]
        public HttpResponseMessage GetJobStatus()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                List<string> lstJobStatus = Enum.GetValues(typeof(MHM.Api.Models.EnumStatusModel.JobStatus)).Cast<MHM.Api.Models.EnumStatusModel.JobStatus>().Select(r => r.ToString()).ToList();
                if (lstJobStatus.Count() > 0)
                {
                    res.Add("Status", true);
                    res.Add("Message", "success");
                    res.Add("JobStatus", lstJobStatus);
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    res.Add("Status", "failed");
                    res.Add("Message", "Job Status does not exist");
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetJobStatus" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetJobStatus - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetApprovalStatus")]
        public HttpResponseMessage GetApprovalStatus()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                List<CaseStatus> lstApprovalStatus = new List<CaseStatus>();
                //int i = 1;
                //foreach (var val in Enum.GetValues(typeof(MHMBLL.EnumStatusModel.CaseApprovalStatus)))
                //{
                //    lstJobStatus.Add(new CaseStatus() { Id = i, Value = val });
                //    i++;
                //}


                lstApprovalStatus.Add(new CaseStatus() { Id = 1, Value = "Not Reviewed" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 2, Value = "Reviewed, not Tested" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 3, Value = "Tested, Errors to Address" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 4, Value = "Tested, Approved" });
                lstApprovalStatus.Add(new CaseStatus() { Id = 5, Value = "In Production" });

                if (lstApprovalStatus.Count() > 0)
                {
                    res.Add("Status", true);
                    res.Add("Message", "success");
                    res.Add("ApprovalStatus", lstApprovalStatus.OrderBy(r => r.Value).ToList());
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    res.Add("Status", "failed");
                    res.Add("Message", "Approval status does not exist");
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetApprovalStatus" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetApprovalStatus - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetGroupName")]
        public HttpResponseMessage GetGroupName()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                List<string> lstGroupName;
                using (var DB = new MHMDal.Models.MHM())
                {
                    lstGroupName = DB.Database.SqlQuery<string>(@"select distinct GroupName from PlanAttributeMst").ToList();
                }
                //lstGroupName = objPlanAttributeMst.GetPlanAttributeMaster().OrderBy(x => x.GroupName).GroupBy(r => r.GroupName).Select(x => x.First().GroupName).Distinct().ToList();
                if (lstGroupName.Count() > 0)
                {
                    res.Add("Status", true);
                    res.Add("Message", "success");
                    res.Add("GroupName", lstGroupName);
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    res.Add("Status", "failed");
                    res.Add("Message", "Group name does not exist");
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetGroupName" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetGroupName - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetCaseJobRunStatus")]
        public HttpResponseMessage GetCaseJobRunStatus()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                List<string> lstCaseJobRunStatus = Enum.GetValues(typeof(MHM.Api.Models.EnumStatusModel.CaseJobRunStatus)).Cast<MHM.Api.Models.EnumStatusModel.CaseJobRunStatus>().Select(r => r.ToString()).ToList();
                if (lstCaseJobRunStatus.Count() > 0)
                {
                    res.Add("Status", true);
                    res.Add("Message", "success");
                    res.Add("CaseJobRunStatus", lstCaseJobRunStatus.OrderBy(r => r));
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    res.Add("Status", "failed");
                    res.Add("Message", "Case Job Run Status does not exist");
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetCaseJobRunStatus" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetCaseJobRunStatus - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetNewJobNumber")]
        public HttpResponseMessage GetNewJobNumber()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstJobMaster = DB.JobMasters.ToList();
                    //long NewJobNum = lstJobMaster.Max(r => Convert.ToInt64(new string(r.JobNumber.Where(c => Char.IsDigit(c)).ToArray()))) + 1;
                    if (lstJobMaster.Count() > 0)
                    {
                        long NewJobNum = Convert.ToInt64(lstJobMaster.Max(r => Convert.ToInt64(new string(r.JobNumber.Where(c => Char.IsDigit(c)).ToArray())))) + Convert.ToInt64(1);
                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("NewJobNum", NewJobNum);
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("NewJobNum", "Job1000000");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetNewJobNumber" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetNewJobNumber - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/PlanMasters/GetInsuranceTypeMaster")]
        public HttpResponseMessage GetInsuranceTypeMaster()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var lstInsuranceTypesMaster = DB.InsuranceTypes.ToList();

                    if (lstInsuranceTypesMaster != null)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("lstInsuranceTypesMaster", lstInsuranceTypesMaster.OrderBy(r => r.InsuranceType1));
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Employer Master does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetInsuranceTypeMaster" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetInsuranceTypeMaster - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetCostSharingTypes")]
        public HttpResponseMessage GetCostSharingTypes()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    //List<string> lstCostSharingTypes = Enum.GetValues(typeof(MHM.Api.Models.EnumStatusModel.CostSharingType)).Cast<MHM.Api.Models.EnumStatusModel.CostSharingType>().OrderBy(r => r.ToString()).Select(r => r.ToString().Replace("_", " ")).ToList();
                    var lstCostSharingTypes = DB.CSTMsts.OrderBy(x => x.Description).ToList();
                    if (lstCostSharingTypes.Count() > 0)
                    {
                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("CostSharingTypes", lstCostSharingTypes.OrderBy(r => r.Description));
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", "failed");
                        res.Add("Message", "Cost Sharing Types does not exist");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetCostSharingTypes" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetCostSharingTypes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [Route("api/PlanMaster/GetLimitUnits")]
        public HttpResponseMessage GetLimitUnits()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                List<string> lstLimitUnits = Enum.GetValues(typeof(MHM.Api.Models.EnumStatusModel.LimitUnit)).Cast<MHM.Api.Models.EnumStatusModel.LimitUnit>().OrderBy(o => o.ToString()).Select(r => r.ToString().Replace("_", "$")).ToList();
                if (lstLimitUnits.Count() > 0)
                {
                    res.Add("Status", true);
                    res.Add("Message", "success");
                    res.Add("LimitUnits", lstLimitUnits);
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    res.Add("Status", "failed");
                    res.Add("Message", "Limit Units does not exist");
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetLimitUnits" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetLimitUnits - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [Route("api/PlanMaster/UploadPlanAttribute")]
        [HttpPost]
        //[ResponseType(typeof(void))]
        public HttpResponseMessage UploadPlanAttribute(List<PlanAttributeMst> planAttributeList)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            if (!ModelState.IsValid)
            {
                var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.Exception.Message));
                oResponse.Status = false;
                oResponse.Message = Messages;
                HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return objResponse;
            }
            Dictionary<string, string> lstNotSuccessPlanAttribute = new Dictionary<string, string>();
            using (var DB = new MHMDal.Models.MHM())
            {
                foreach (var data in planAttributeList)
                {
                    var IsExist = DB.PlanAttributeMsts.Any(r => r.PlanId == data.PlanId && r.BusinessYear == data.BusinessYear);
                    if (!IsExist)
                    {
                        try
                        {
                            DB.PlanAttributeMsts.Add(data);
                            DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            lstNotSuccessPlanAttribute.Add(data.PlanId + ' ' + data.BusinessYear, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                        }
                    }
                }

                var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("PlanAttributeMaster");
                if (PlanAttributeMasterFromCache != null)
                {
                    MHMCache.RemoveMyCachedItem("PlanAttributeMaster");
                }

                if (lstNotSuccessPlanAttribute.Count > 0)
                {
                    res.Add("Status", false);
                    res.Add("Message", lstNotSuccessPlanAttribute);
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    oResponse.Status = true;
                    oResponse.Message = "File Imported Successfully";
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return objResponse;
                }
            }
        }

        [Route("api/PlanMaster/UploadCsrRateMst")]
        [HttpPost]
        //[ResponseType(typeof(void))]
        public HttpResponseMessage UploadCsrRateMst(List<CSR_Rate_Mst> csrRateList)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            if (!ModelState.IsValid)
            {
                var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                oResponse.Status = false;
                oResponse.Message = Messages;
                HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return objResponse;
            }

            Dictionary<string, string> lstNotSuccessCsrRate = new Dictionary<string, string>();

            using (var DB = new MHMDal.Models.MHM())
            {
                foreach (var data in csrRateList)
                {
                    var IsExist = DB.CSR_Rate_Mst.Any(r => r.PlanID == data.PlanID && r.BusinessYear == data.BusinessYear && r.RatingAreaId == data.RatingAreaId && r.Age == data.Age);
                    if (!IsExist)
                    {
                        try
                        {
                            DB.CSR_Rate_Mst.Add(data);
                            DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            lstNotSuccessCsrRate.Add(data.PlanID + ' ' + data.BusinessYear, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                        }
                    }
                }

                var CSRRateListFromCache = MHMCache.GetMyCachedItem("CSR_Rate_Mst");
                if (CSRRateListFromCache != null)
                {
                    MHMCache.RemoveMyCachedItem("CSR_Rate_Mst");
                }

                if (lstNotSuccessCsrRate.Count > 0)
                {
                    res.Add("Status", false);
                    res.Add("Message", lstNotSuccessCsrRate);
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    oResponse.Status = true;
                    oResponse.Message = "File Imported Successfully";
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return objResponse;
                }
            }
        }

        [Route("api/PlanMaster/UploadPlanBenefitMst")]
        [HttpPost]
        //[ResponseType(typeof(void))]
        public HttpResponseMessage UploadPlanBenefitMst(List<PlanBenefitMst> planBenefitList)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            if (!ModelState.IsValid)
            {
                var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                oResponse.Status = false;
                oResponse.Message = Messages;
                HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return objResponse;
            }


            Dictionary<string, string> lstNotSuccessPlanBenefit = new Dictionary<string, string>();

            using (var DB = new MHMDal.Models.MHM())
            {
                foreach (var data in planBenefitList)
                {
                    var IsExist = DB.PlanBenefitMsts.Any(o => o.PlanId == data.PlanId && o.BusinessYear == data.BusinessYear && o.MHMBenefitId == data.MHMBenefitId);
                    IsExist = DB.PlanBenefitMsts.Any(o => o.PlanId == data.PlanId && o.BusinessYear == data.BusinessYear && o.BenefitName == data.BenefitName);
                    if (!IsExist)
                    {
                        try
                        {
                            DB.PlanBenefitMsts.Add(data);
                            DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            lstNotSuccessPlanBenefit.Add(data.PlanId + ' ' + data.BusinessYear + ' ' + data.BenefitName, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                        }
                    }
                }

                var PlanBenefitMasterFromCache = MHMCache.GetMyCachedItem("PlanBenefitMaster");
                if (PlanBenefitMasterFromCache != null)
                {
                    MHMCache.RemoveMyCachedItem("PlanBenefitMaster");
                }

                if (lstNotSuccessPlanBenefit.Count > 0)
                {
                    res.Add("Status", false);
                    res.Add("Message", lstNotSuccessPlanBenefit);
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    oResponse.Status = true;
                    oResponse.Message = "File Imported Successfully";
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return objResponse;
                }
            }
        }

        [HttpPost]
        [Route("api/PlanMaster/ImportCensus")]
        public HttpResponseMessage ImportCensus(string JobNumber, long EmployerId, string FileName, List<CensusImportData> Censusdata)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            if (!ModelState.IsValid)
            {
                var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.Exception.Message));
                oResponse.Status = false;
                oResponse.Message = Messages;
                HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return objResponse;
            }

            Dictionary<string, string> lstNotSuccessPlanAttribute = new Dictionary<string, string>();

            using (var DB = new MHMDal.Models.MHM())
            {

                long RowNumber = 1;
                foreach (var record in Censusdata)
                {
                    using (var dbContextTransaction = DB.Database.BeginTransaction())
                    {
                        RowNumber = RowNumber + 1;
                        try
                        {
                            var JobMst = DB.JobMasters.Where(r => r.JobNumber == JobNumber).FirstOrDefault();

                            var objApplication = new Applicant();
                            objApplication.FirstName = record.FirstName.Trim().Encrypt();
                            objApplication.LastName = record.LastName.Trim().Encrypt();
                            objApplication.Street = string.IsNullOrEmpty(record.Street) ? "".Encrypt() : record.Street.Trim().Encrypt();
                            objApplication.City = record.City.Trim().Encrypt();
                            objApplication.State = record.State.Trim().Encrypt();
                            objApplication.Zip = record.Zip.Trim().Encrypt();
                            objApplication.Email = record.Email.Trim().Encrypt();
                            objApplication.Mobile = record.Mobile.Replace("-", "").Trim().Encrypt();
                            objApplication.CurrentPlan = string.IsNullOrEmpty(record.Street) ? "" : record.CurrentPlan.Trim();
                            objApplication.CreatedDateTime = DateTime.Now;
                            objApplication.HireDate = string.IsNullOrEmpty(record.HireDate) ? (DateTime?)null : Convert.ToDateTime(record.HireDate);
                            objApplication.EREmpId = string.IsNullOrEmpty(record.EREmpId) ? "" : record.EREmpId.Trim();
                            objApplication.JobTitle = string.IsNullOrEmpty(record.JobTitle) ? "" : record.JobTitle.Trim();
                            objApplication.EmployerId = EmployerId;
                            DB.Applicants.Add(objApplication);
                            DB.SaveChanges();

                            var objCase = new Case();
                            objCase.ApplicantID = objApplication.ApplicantID;
                            objCase.MAGIncome = Convert.ToDecimal(record.MAGI.Replace("$", "").Replace(",", "").Trim());
                            objCase.UsageID = Convert.ToInt32(record.UsageID);
                            objCase.PreviousYrHSA = false;
                            objCase.Welness = record.Wellness == "Y" ? true : false;
                            objCase.CreatedDateTime = System.DateTime.Now;
                            objCase.JobNumber = JobNumber;
                            objCase.ZipCode = JobMst.CaseZipCode;
                            var Countylst = DB.qryZipCodeToRatingAreas.Where(m => m.Zip == JobMst.CaseZipCode).Select(r => new { r.CountyName, r.RatingAreaID, r.StateCode, r.StateId, r.StateName, r.City }).FirstOrDefault();

                            if (Countylst != null)
                            {
                                objCase.CountyName = Countylst.CountyName;
                                objCase.RatingAreaId = Countylst.RatingAreaID;
                            }
                            objCase.Year = JobMst.JobYear;
                            objCase.StatusId = 3;
                            if (!String.IsNullOrEmpty(record.CaseTitle))
                                objCase.CaseTitle = record.CaseTitle;
                            else
                                objCase.CaseTitle = !String.IsNullOrEmpty(record.LastName) ? record.LastName.Substring(0, 2) : "";

                            objCase.CaseReferenceId = RowNumber;
                            objCase.CaseSource = "Imported: " + FileName + " Row:  ##" + RowNumber;
                            objCase.TierIntention = 2;
                            objCase.TaxRate = 27;
                            objCase.HSAFunding = 0;
                            objCase.DedBalAvailToRollOver = 0;

                            DB.Cases.Add(objCase);
                            DB.SaveChanges();

                            var objcenimp = new CensusImport();
                            objcenimp.JobNumber = JobNumber;
                            objcenimp.EmployeeFirstName = record.FirstName.Trim();
                            objcenimp.EmployeeLastName = record.LastName.Trim();
                            objcenimp.Email = record.Email.Trim();
                            objcenimp.CreateDatetime = DateTime.Now;
                            DB.CensusImports.Add(objcenimp);
                            DB.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            lstNotSuccessPlanAttribute.Add(record.FirstName + ' ' + record.LastName + ' ' + record.Email, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                        }
                    }
                }


                JobMaster objJob = DB.JobMasters.Where(r => r.JobNumber == JobNumber).FirstOrDefault();
                objJob.JobCensusImportDt = System.DateTime.Now;
                DB.SaveChanges();

                if (lstNotSuccessPlanAttribute.Count > 0)
                {
                    res.Add("Status", false);
                    res.Add("Message", lstNotSuccessPlanAttribute);
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                    return objResponse;
                }
                else
                {
                    oResponse.Status = true;
                    oResponse.Message = "Census Imported Successfully";
                    HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return objResponse;
                }
            }
        }

        public static int CalculateAge(DateTime BirthDate)
        {
            int YearsPassed = DateTime.Now.Year - BirthDate.Year;
            // Are we before the birth date this year? If so subtract one year from the mix
            if (DateTime.Now.Month < BirthDate.Month || (DateTime.Now.Month == BirthDate.Month && DateTime.Now.Day < BirthDate.Day))
            {
                YearsPassed--;
            }
            return YearsPassed;
        }

    }

    internal class CaseStatus
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

}
