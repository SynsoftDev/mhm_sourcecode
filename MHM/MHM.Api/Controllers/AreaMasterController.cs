using MHMDal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MHM.Api.Controllers
{
    public class AreaMasterController : ApiController
    {
        // MHMDal.Models.MHM DB = new MHMDal.Models.MHM();

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/AreaMaster/GetStateAbrev")]
        public HttpResponseMessage GetStateAbrev([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    int skipRows = (page - 1) * pageSize;

                    var lstStateAbrevs = DB.tblStateAbrevs.ToList();
                    if (!string.IsNullOrEmpty(searchby))
                    {
                        lstStateAbrevs = lstStateAbrevs.Where(x => x.StateName.ToLower().Contains(searchby.ToLower())).ToList();
                    }

                    if (BusinessYear != null)
                    {
                        lstStateAbrevs = lstStateAbrevs.Where(x => x.Businessyear == BusinessYear).ToList();
                    }
                    //Carrier, Plan, IsActive , StateCode MetalLevel and MarketCover 
                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        lstStateAbrevs = lstStateAbrevs.Where(x => x.StateCode.ToLower() == lstParameter[0].ToString().ToLower()).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]))
                    {
                        lstStateAbrevs = lstStateAbrevs.Where(x => x.FipsState == lstParameter[1].ToString()).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[2]))
                    {
                        lstStateAbrevs = lstStateAbrevs.Where(x => x.EntityType == lstParameter[2].ToString()).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[3]))
                    {
                        lstStateAbrevs = lstStateAbrevs.Where(x => x.IsoCode == lstParameter[3].ToString()).ToList();
                    }

                    int total = lstStateAbrevs.Count();

                    if (total > 0)
                    {
                        //lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList();
                        switch (sortby)
                        {
                            case "Id":
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.Id).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.Id).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "StateName":
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.StateName).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.StateName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "StateCode":
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.StateCode).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.StateCode).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "FipsState":
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.FipsState).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.FipsState).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "EntityType":
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.EntityType).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.EntityType).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "IsoCode":
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.IsoCode).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.IsoCode).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "Businessyear":
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.Businessyear).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.Businessyear).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            default:
                                lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("StateAbrevs", lstStateAbrevs.Select(r => new
                        {
                            r.Id,
                            r.StateName,
                            r.StateCode,
                            r.FipsState,
                            r.EntityType,
                            r.IsoCode,
                            r.Businessyear
                        })
                        );
                        res.Add("TotalCount", total);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "StateAbrevs does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetStateAbrev" + Environment.NewLine;
                ExceptionString += "Request :  " + "lstParameter " + lstParameter + " ,BusinessYear " + BusinessYear + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetStateAbrev - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/AreaMaster/GetEntityTypes")]
        public HttpResponseMessage GetEntityTypes()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var lstEntityTypes = DB.tblStateAbrevs.Select(r => r.EntityType).Distinct().ToList();
                    if (lstEntityTypes.Count > 0)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("EntityTypes", lstEntityTypes);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "EntityTypes does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetEntityTypes" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetEntityTypes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/AreaMaster/GetZipCodes")]
        public HttpResponseMessage GetZipCodes([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    int skipRows = (page - 1) * pageSize;

                    var lsttblZipCodes = DB.tblZipCodes.Join(DB.tblStateAbrevs, t1 => t1.State, t2 => t2.StateCode, (t1, t2) => new { t1.Zip, t1.State, t1.County, t1.CreatedDateTime, t2.StateName }).ToList();

                    if (!string.IsNullOrEmpty(searchby))
                    {
                        lsttblZipCodes = lsttblZipCodes.Where(x => x.Zip.Contains(searchby)).ToList();
                    }


                    //Carrier, Plan, IsActive , StateCode MetalLevel and MarketCover 
                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        lsttblZipCodes = lsttblZipCodes.Where(x => x.State.ToLower().Contains(lstParameter[0].ToLower())).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]))
                    {
                        lsttblZipCodes = lsttblZipCodes.Where(x => x.County.ToLower().Contains(lstParameter[1].ToLower())).ToList();
                    }

                    int total = lsttblZipCodes.Count();

                    if (total > 0)
                    {
                        //lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList();
                        switch (sortby)
                        {
                            case "Zip":
                                lsttblZipCodes = desc ? lsttblZipCodes.OrderByDescending(x => x.Zip).Skip(skipRows).Take(pageSize).ToList() : lsttblZipCodes.OrderBy(x => x.Zip).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "StateName":
                                lsttblZipCodes = desc ? lsttblZipCodes.OrderByDescending(x => x.StateName).Skip(skipRows).Take(pageSize).ToList() : lsttblZipCodes.OrderBy(x => x.StateName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "State":
                                lsttblZipCodes = desc ? lsttblZipCodes.OrderByDescending(x => x.State).Skip(skipRows).Take(pageSize).ToList() : lsttblZipCodes.OrderBy(x => x.State).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "County":
                                lsttblZipCodes = desc ? lsttblZipCodes.OrderByDescending(x => x.County).Skip(skipRows).Take(pageSize).ToList() : lsttblZipCodes.OrderBy(x => x.County).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            //case "Businessyear":
                            //    lsttblZipCodes = desc ? lsttblZipCodes.OrderByDescending(x => x.Businessyear).Skip(skipRows).Take(pageSize).ToList() : lsttblZipCodes.OrderBy(x => x.Businessyear).Skip(skipRows).Take(pageSize).ToList();
                            //    break;

                            default:
                                lsttblZipCodes = desc ? lsttblZipCodes.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lsttblZipCodes.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("ZipCodes", lsttblZipCodes.Select(r => new
                        {
                            r.Zip,
                            r.State,
                            r.StateName,
                            r.County,
                            Businessyear = ""
                        })
                        );
                        res.Add("TotalCount", total);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "Zip Codes does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetZipCodes" + Environment.NewLine;
                ExceptionString += "Request :  " + " lstParameter " + lstParameter + " ,BusinessYear " + BusinessYear + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetZipCodes - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/AreaMaster/GetRatingAreas")]
        public HttpResponseMessage GetRatingAreas([FromUri]  List<string> lstParameter, string BusinessYear, string searchby = null, string sortby = null, bool desc = true, int page = 0, int pageSize = 10)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    int skipRows = (page - 1) * pageSize;

                    var lstRatingAreas = DB.tblRatingAreas.ToList();
                    if (!string.IsNullOrEmpty(searchby))
                    {
                        //oCase.Applicant.City != null ? oCase.Applicant.City.Encrypt() : null;
                        //if (searchby != null) { lstRatingAreas = lstRatingAreas.Where(r => r.CountyName != null || r.ThreeDigitZipCode != null).ToList(); }
                        lstRatingAreas.ForEach(
                            x => x.CountyName = x.CountyName != null ? x.CountyName : ""
                            );
                        lstRatingAreas.ForEach(
                           x => x.ThreeDigitZipCode = x.ThreeDigitZipCode != null ? x.ThreeDigitZipCode : ""
                           );
                        lstRatingAreas = lstRatingAreas.Where(x => x.CountyName.ToLower().Contains(searchby.ToLower()) || x.ThreeDigitZipCode.ToString().Contains(searchby)).ToList();
                        //lstRatingAreas = lstRatingAreas.Where(x => x.CountyName.ToLower() == searchby.ToLower()).ToList();
                    }

                    if (BusinessYear != null)
                    {
                        lstRatingAreas = lstRatingAreas.Where(x => x.Businessyear == BusinessYear).ToList();
                    }
                    //Carrier, Plan, IsActive , StateCode MetalLevel and MarketCover 
                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        lstRatingAreas = lstRatingAreas.Where(x => x.StateCode == lstParameter[0]).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]))
                    {
                        lstRatingAreas = lstRatingAreas.Where(x => x.RatingAreaID == Convert.ToInt64(lstParameter[1])).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[2]))
                    {
                        lstRatingAreas = lstRatingAreas.Where(x => x.MarketCoverage == lstParameter[2].ToString()).ToList();
                    }



                    int total = lstRatingAreas.Count();

                    if (total > 0)
                    {
                        //lstStateAbrevs = desc ? lstStateAbrevs.OrderByDescending(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList() : lstStateAbrevs.OrderBy(x => x.PlanId).Skip(skipRows).Take(pageSize).ToList();
                        switch (sortby)
                        {
                            case "Id":
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.Id).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.Id).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "CountyName":
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.CountyName).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.CountyName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "ThreeDigitZipCode":
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.ThreeDigitZipCode).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.ThreeDigitZipCode).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "StateCode":
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.StateCode).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.StateCode).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "RatingAreaName":
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.tblRatingAreaMst.RatingAreaName).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.tblRatingAreaMst.RatingAreaName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "FipsCounty":
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.MarketCoverage).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.MarketCoverage).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "Businessyear":
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.Businessyear).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.Businessyear).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            default:
                                lstRatingAreas = desc ? lstRatingAreas.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : lstRatingAreas.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }

                        lstRatingAreas.ForEach(r => r.tblRatingAreaMst = r.tblRatingAreaMst);
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("RatingAreas", lstRatingAreas.Select(r => new
                        {
                            r.Id,
                            r.CountyName,
                            r.ThreeDigitZipCode,
                            r.StateCode,
                            r.RatingAreaID,
                            r.tblRatingAreaMst.RatingAreaName,
                            r.MarketCoverage,
                            r.Businessyear
                        })
                        );
                        res.Add("TotalCount", total);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "RatingAreas does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetRatingAreas" + Environment.NewLine;
                ExceptionString += "Request :  " + " lstParameter " + lstParameter + " ,BusinessYear " + BusinessYear + " ,searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetRatingAreas - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/AreaMaster/GetRatingAreas")]
        public HttpResponseMessage GetRatingAreas()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var lstRatingAreas = DB.tblRatingAreaMsts.Select(r => new { r.RatingAreaID, r.RatingAreaName }).Distinct().OrderBy(t => t.RatingAreaID).ToList();
                    if (lstRatingAreas.Count > 0)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("RatingAreas", lstRatingAreas);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "RatingAreas does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetRatingAreas" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetRatingAreas - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/AreaMaster/GetRatingAreas/{StateCode}")]
        public HttpResponseMessage GetRatingAreas(string StateCode)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    List<long> lst = DB.tblRatingAreas.Where(r => r.StateCode == StateCode).Select(r => r.RatingAreaID).Distinct().ToList();
                    var lstRatingAreas = DB.tblRatingAreaMsts.Where(t => lst.Contains(t.RatingAreaID)).Select(r => new { r.RatingAreaID, r.RatingAreaName }).Distinct().ToList();
                    if (lstRatingAreas.Count > 0)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("RatingAreas", lstRatingAreas);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "RatingAreas does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetRatingAreas" + Environment.NewLine;
                ExceptionString += "Request :  " + " StateCode " + StateCode + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetRatingAreas - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/AreaMaster/GetMarketCoverages")]
        public HttpResponseMessage GetMarketCoverages()
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var lstMarketCoverages = DB.tblRatingAreas.Select(r => r.MarketCoverage).Distinct().ToList();
                    if (lstMarketCoverages.Count > 0)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("MarketCoverages", lstMarketCoverages);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "MarketCoverages does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : GetMarketCoverages" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetMarketCoverages - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                return response;
            }
        }

    }
}
