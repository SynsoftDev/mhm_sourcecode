using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MHMDal.Models;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;

namespace MHM.Api.Controllers
{
    public class EmployerMasterController : ApiController
    {
        //MHMDal.Models.MHM DB = new MHMDal.Models.MHM();

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/EmployerMaster/GetEmployers")]
        public HttpResponseMessage GetEmployers(long? employerid, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    int skipRows = (page - 1) * pageSize;

                    var lstEmployerMsts = DB.EmployerMsts.ToList();
                    if (!string.IsNullOrEmpty(searchby))
                    {
                        lstEmployerMsts = lstEmployerMsts.Where(r => r.EmployerName.ToLower().Contains(searchby.ToLower())).ToList();
                    }
                    if (employerid != null)
                    {
                        lstEmployerMsts = lstEmployerMsts.Where(r => r.EmployerId == employerid).ToList();
                    }

                    int total = lstEmployerMsts.Count();

                    if (total > 0)
                    {
                        switch (sortby)
                        {
                            case "EmployerId":
                                lstEmployerMsts = desc ? lstEmployerMsts.OrderByDescending(r => r.EmployerId).Skip(skipRows).Take(pageSize).ToList() : lstEmployerMsts.OrderBy(r => r.EmployerId).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "EmployerName":
                                lstEmployerMsts = desc ? lstEmployerMsts.OrderByDescending(r => r.EmployerName).Skip(skipRows).Take(pageSize).ToList() : lstEmployerMsts.OrderBy(r => r.EmployerName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            default:
                                lstEmployerMsts = desc ? lstEmployerMsts.OrderByDescending(r => r.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lstEmployerMsts.OrderBy(r => r.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("EmployerMsts", lstEmployerMsts.Select(r => new
                        {
                            r.EmployerId,
                            r.EmployerName,
                            r.CreatedDateTime
                        })
                        );
                        res.Add("TotalCount", total);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "EmployerMsts does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetEmployers" + Environment.NewLine;
                    ExceptionString += "Request : " + " employerid " + employerid + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetEmployers - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/EmployerMaster/GetEmployer/{EmployerId}")]
        public HttpResponseMessage GetEmployer(long EmployerId)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    EmployerMst employermast = DB.EmployerMsts.Find(EmployerId);
                    if (employermast == null)
                    {
                        data.Add("Status", false);
                        data.Add("Message", "Employer cannot be found. ");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;
                    }
                    else
                    {
                        data.Add("Status", true);
                        data.Add("Message", "Success");
                        data.Add("Employer", employermast);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    data.Add("Status", false);
                    data.Add("Message", ex.Message);

                    string ExceptionString = "Api : GetEmployer" + Environment.NewLine;
                    ExceptionString += "Request : " + " EmployerId " + EmployerId + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetEmployer - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, data);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/EmployerMster/AddEmployer")]
        public HttpResponseMessage AddEmployer(EmployerMst employermst)
        {
            Response oResponse = new Response();

            if (!ModelState.IsValid)
            {
                string messages = string.Join(Environment.NewLine, ModelState.Values
                                         .SelectMany(r => r.Errors)
                                         .Select(r => r.ErrorMessage));

                oResponse.Status = false;
                oResponse.Message = messages;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return response;
            }
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    HttpResponseMessage response;
                    if (DB.EmployerMsts.Any(r => r.EmployerId == employermst.EmployerId))
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Employer Id already exist";
                        response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }

                    employermst.CreatedDateTime = System.DateTime.Now;
                    DB.EmployerMsts.Add(employermst);
                    DB.SaveChanges();

                    oResponse.Status = true;
                    oResponse.Message = "Success";
                    response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.InnerException.Message;

                    string ExceptionString = "Api : AddEmployer" + Environment.NewLine;
                    ExceptionString += "Request : " + " employermst " + JsonConvert.SerializeObject(employermst) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "AddEmployer - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/EmployerMaster/UpdateEmployer")]
        public HttpResponseMessage UpdateEmployer(EmployerMst employermst)
        {
            Response oResponse = new Response();
            if (!ModelState.IsValid)
            {
                string messages = string.Join(Environment.NewLine, ModelState.Values
                                         .SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage));

                oResponse.Status = false;
                oResponse.Message = messages;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return response;
            }

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var employer = DB.EmployerMsts.Find(employermst.EmployerId);
                    HttpResponseMessage response;
                    if (employer == null)
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Employer cannot be found.";
                        response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }

                    employer.EmployerName = employermst.EmployerName;
                    employer.ModifiedDateTime = DateTime.Now;
                    DB.SaveChanges();

                    oResponse.Status = true;
                    oResponse.Message = "Success";
                    response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UpdateEmployer" + Environment.NewLine;
                    ExceptionString += "Request : " + " employermst " + JsonConvert.SerializeObject(employermst) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdateEmployer - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UpdateEmployer" + Environment.NewLine;
                    ExceptionString += "Request : " + " employermst " + JsonConvert.SerializeObject(employermst) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UpdateEmployer - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

    }
}

