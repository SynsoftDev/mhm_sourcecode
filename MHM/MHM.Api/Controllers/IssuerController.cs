using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MHMDal.Models;
using System.Linq.Expressions;
using MHM.Api.ViewModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.Entity.Core;
using MHMBLL;
using Newtonsoft.Json;

namespace MHM.Api.Controllers
{
    public class IssuerController : ApiController
    {
        //MHMDal.Models.MHM DB = new MHMDal.Models.MHM();
        MHMCache MHMCache = new MHMCache();

        /// <summary>
        /// This api is used to get all Carrier / Inssuer
        /// </summary>
        /// <returns>List of Carrier / Inssuer</returns>
        [HttpGet]
        [Route("api/Issuer/GetAllCarrier")]
        [Authorize]
        public HttpResponseMessage GetAllCarrier(string issuerCode, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    int skipRows = (page - 1) * pageSize;

                    var lstIssuerMasters = DB.IssuerMsts.ToList();
                    if (!string.IsNullOrEmpty(searchby))
                    {
                        lstIssuerMasters = lstIssuerMasters.Where(x => x.IssuerName.ToLower().Contains(searchby.ToLower())).ToList();
                    }

                    if (issuerCode != null)
                    {
                        lstIssuerMasters = lstIssuerMasters.Where(x => x.IssuerCode.ToLower() == issuerCode.ToLower()).ToList();
                    }
                    //Carrier, Plan, IsActive , StateCode MetalLevel and MarketCover 
                    //if (!string.IsNullOrEmpty(lstParameter[0]))
                    //{
                    //    lstJobMasters = lstJobMasters.Where(x => x.EmployerId == Convert.ToInt64(lstParameter[0])).ToList();
                    //}


                    int total = lstIssuerMasters.Count();

                    if (total > 0)
                    {
                        //lstPlanAttribute = desc ? lstPlanAttribute.OrderByDescending(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList() : lstPlanAttribute.OrderBy(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList();
                        switch (sortby)
                        {
                            case "IssuerName":
                                lstIssuerMasters = desc ? lstIssuerMasters.OrderByDescending(x => x.IssuerName).Skip(skipRows).Take(pageSize).ToList() : lstIssuerMasters.OrderBy(x => x.IssuerName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "EmployerName":
                                lstIssuerMasters = desc ? lstIssuerMasters.OrderByDescending(x => x.IssuerCode).Skip(skipRows).Take(pageSize).ToList() : lstIssuerMasters.OrderBy(x => x.IssuerCode).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "CreatedDateTime":
                                lstIssuerMasters = desc ? lstIssuerMasters.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lstIssuerMasters.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            default:
                                lstIssuerMasters = desc ? lstIssuerMasters.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lstIssuerMasters.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }

                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("lstIssuerMasters", lstIssuerMasters);
                        res.Add("TotalCount", total);
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", "failed");
                        res.Add("Message", "IssuerMasters does not exist");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.ToString();

                    string ExceptionString = "Api : GetAllCarrier" + Environment.NewLine;
                    ExceptionString += "Request : " + " issuerCode " + issuerCode + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetAllCarrier - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to get all Carrier / Inssuer
        /// </summary>
        /// <returns>List of Carrier / Inssuer</returns>
        [HttpGet]
        [Route("api/Issuer/GetCarrier/{IssuerCode}")]
        [Authorize]
        public HttpResponseMessage GetCarrier(string IssuerCode)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    IssuerMst objIssuer = DB.IssuerMsts.Where(r => r.IssuerCode == IssuerCode).FirstOrDefault();

                    if (objIssuer != null)
                    {
                        res.Add("Status", true);
                        res.Add("Message", "success");
                        res.Add("IssuerMaster", objIssuer);
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }
                    else
                    {
                        res.Add("Status", "failed");
                        res.Add("Message", "IssuerMaster does not exist");
                        HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, res);
                        return objResponse;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetCarrier" + Environment.NewLine;
                    ExceptionString += "Request : " + " IssuerCode " + IssuerCode + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetCarrier - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to Update Issuer
        /// </summary>
        /// <param name="objIssuer">Json Objec of Issuer</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Route("api/Issuer/SaveCarrier")]
        [Authorize]
        public HttpResponseMessage SaveCarrier(IssuerMst objIssuer)
        {
            Response oResponse = new Response();

            if (!ModelState.IsValid)
            {
                var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                oResponse.Status = false;
                oResponse.Message = Messages;
                HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return objResponse;
            }

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var Issuer = DB.IssuerMsts.Add(objIssuer);

                    oResponse.Status = true;
                    oResponse.Message = "Success";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : SaveCarrier" + Environment.NewLine;
                    ExceptionString += "Request : " + " objIssuer " + JsonConvert.SerializeObject(objIssuer) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "SaveCarrier - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to Update Issuer
        /// </summary>
        /// <param name="objIssuer">Json Objec of Issuer</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Route("api/Issuer/UpdateCarrier")]
        [Authorize]
        public HttpResponseMessage UpdateCarrier(IssuerMst objIssuer)
        {
            Response oResponse = new Response();

            if (!ModelState.IsValid)
            {
                var Messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(r => r.Errors).Select(r => r.ErrorMessage));
                oResponse.Status = false;
                oResponse.Message = Messages;
                HttpResponseMessage objResponse = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return objResponse;
            }

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var Issuer = DB.IssuerMsts.Where(r => r.Id == objIssuer.Id).FirstOrDefault();
                    if (Issuer != null)
                    {
                        Issuer.IssuerCode = objIssuer.IssuerCode;
                        Issuer.IssuerName = objIssuer.IssuerName;
                        Issuer.Abbreviations = objIssuer.Abbreviations;
                        Issuer.StateCode = objIssuer.StateCode;
                        Issuer.Status = objIssuer.Status;
                        Issuer.ModifiedBy = objIssuer.ModifiedBy;
                        Issuer.ModifiedDateTime = System.DateTime.Now;
                        DB.SaveChanges();

                        var IssuerMasterFromCache = MHMCache.GetMyCachedItem("IssuerMaster");
                        if (IssuerMasterFromCache != null)
                        {
                            MHMCache.RemoveMyCachedItem("IssuerMaster");
                        }

                        oResponse.Status = true;
                        oResponse.Message = "Success";

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                    else
                    {
                        oResponse.Status = true;
                        oResponse.Message = "Issuer not found";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }

                }
                catch (Exception ex)
                {

                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UpdateCarrier" + Environment.NewLine;
                    ExceptionString += "Request : " + " objIssuer " + JsonConvert.SerializeObject(objIssuer) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdateCarrier - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

    }
}
