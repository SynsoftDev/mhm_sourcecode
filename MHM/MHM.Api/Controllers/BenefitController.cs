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
    public class BenefitController : ApiController
    {
        //MHMDal.Models.MHM DB = new MHMDal.Models.MHM();
        SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ConnectionString);
        SqlCommand cmd;
        MHMCache MHMCache = new MHMCache();

        // GET: api/Benefit
        [Authorize(Roles = "Admin")]
        public IQueryable<MHMCommonBenefitsMst> GetMHMCommonBenefitsMsts()
        {
            using (var DB = new MHMDal.Models.MHM())
            {
                return DB.MHMCommonBenefitsMsts;
            }
        }

        /// <summary>
        /// This api is use to get MHM Common Benefit Details
        /// </summary>
        /// <param name="id">MHM Common Benefit Id</param>
        /// <returns>MHM Common Benefit Details</returns>
        [ResponseType(typeof(MHMCommonBenefitsMst))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage GetMHMCommonBenefitsMst(long id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Response oResponse = new Response();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    MHMCommonBenefitsMst mHMCommonBenefitsMst = DB.MHMCommonBenefitsMsts.Find(id);
                    mHMCommonBenefitsMst.MHMBenefitMappingMsts = mHMCommonBenefitsMst.MHMBenefitMappingMsts;

                    if (mHMCommonBenefitsMst == null)
                    {
                        data.Add("Status", false);
                        data.Add("Message", "Benefit cannot be found. ");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;
                    }
                    else
                    {
                        data.Add("Status", true);
                        data.Add("Message", "Success");
                        data.Add("MHMCommonBenefit", mHMCommonBenefitsMst);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;

                    }
                }
            }
            catch (Exception ex)
            {
                data.Add("Status", false);
                data.Add("Message", ex.Message);

                string ExceptionString = "Api : GetMHMCommonBenefitsMst" + Environment.NewLine;
                ExceptionString += "Request :  " + " id " + id + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "GetMHMCommonBenefitsMst - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, data);
                return response;
            }
        }

        /// <summary>
        /// This api is used to Update Common Benefit
        /// </summary>
        /// <param name="mHMCommonBenefitsMst">MHM Common Benefit Object</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Route("api/Updatebenefit")]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage PutMHMCommonBenefitsMst(MHMCommonBenefitsMst mHMCommonBenefitsMst)
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
                List<MHMBenefitMappingMst> newBenefits = mHMCommonBenefitsMst.MHMBenefitMappingMsts.ToList();
                using (var dbContextTransaction = DB.Database.BeginTransaction())
                {
                    try
                    {
                        var mhmCommonBenefit = DB.MHMCommonBenefitsMsts.Find(mHMCommonBenefitsMst.MHMBenefitID);
                        if (mhmCommonBenefit == null)
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Benefit cannot be found.";
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }

                        mhmCommonBenefit.MHMBenefitName = mHMCommonBenefitsMst.MHMBenefitName;
                        mhmCommonBenefit.CategoryId = mHMCommonBenefitsMst.CategoryId;
                        mhmCommonBenefit.IsDefault = mHMCommonBenefitsMst.IsDefault;
                        mhmCommonBenefit.ModifiedDateTime = DateTime.Now;
                        DB.SaveChanges();
                        newBenefits.ForEach(r => r.MHMCommonBenefitID = mhmCommonBenefit.MHMBenefitID);
                    }
                    catch (Exception ex)
                    {
                        var sqlException = ex.InnerException.InnerException as System.Data.SqlClient.SqlException;

                        if (sqlException.Number == 2601 || sqlException.Number == 2627)
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Benefit name already exists.";

                            string ExceptionString = "Api : PutMHMCommonBenefitsMst" + Environment.NewLine;
                            ExceptionString += "Request :  " + JsonConvert.SerializeObject(mHMCommonBenefitsMst) + Environment.NewLine;
                            ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                            var fileName = "PutMHMCommonBenefitsMst - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                            Helpers.Service.LogError(fileName, ExceptionString);

                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                        else
                        {
                            oResponse.Status = false;
                            oResponse.Message = ex.Message;

                            string ExceptionString = "Api : PutMHMCommonBenefitsMst" + Environment.NewLine;
                            ExceptionString += "Request :  " + JsonConvert.SerializeObject(mHMCommonBenefitsMst) + Environment.NewLine;
                            ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                            var fileName = "PutMHMCommonBenefitsMst - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                            Helpers.Service.LogError(fileName, ExceptionString);

                            dbContextTransaction.Rollback();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                            return response;
                        }

                    }

                    //DB.Entry(mHMCommonBenefitsMst).State = EntityState.Modified;

                    try
                    {
                        foreach (var item in mHMCommonBenefitsMst.MHMBenefitMappingMsts)
                        {
                            var temp = DB.MHMBenefitMappingMsts.Where(r => r.Id == item.Id).FirstOrDefault();
                            if (temp != null)
                            {
                                temp.IssuerBenefitID = item.IssuerBenefitID;
                                temp.ModifiedBy = mHMCommonBenefitsMst.Createdby;
                                temp.ModifiedDateTime = System.DateTime.Now;
                            }
                            else
                            {
                                MHMBenefitMappingMst obj = new MHMBenefitMappingMst();
                                obj.IssuerID = item.IssuerID;
                                obj.MHMCommonBenefitID = item.MHMCommonBenefitID;
                                obj.IssuerBenefitID = item.IssuerBenefitID;
                                obj.Createdby = mHMCommonBenefitsMst.Createdby;
                                obj.CreatedDateTime = System.DateTime.Now;
                                DB.MHMBenefitMappingMsts.Add(obj);
                            }
                            DB.SaveChanges();
                        }

                        dbContextTransaction.Commit();

                        //Remove Cache of PlanBenefitMaster
                        var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("MHMBenefitMappingMaster");
                        if (PlanAttributeMasterFromCache != null)
                        {
                            MHMCache.RemoveMyCachedItem("MHMBenefitMappingMaster");
                        }

                        oResponse.Status = true;
                        oResponse.Message = "Success";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!MHMCommonBenefitsMstExists(mHMCommonBenefitsMst.MHMBenefitID))
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Benefit cannot be found. ";

                            string ExceptionString = "Api : PutMHMCommonBenefitsMst" + Environment.NewLine;
                            ExceptionString += "Request :  " + JsonConvert.SerializeObject(mHMCommonBenefitsMst) + Environment.NewLine;
                            ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                            var fileName = "PutMHMCommonBenefitsMst - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                            Helpers.Service.LogError(fileName, ExceptionString);

                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                        else
                        {
                            oResponse.Status = false;
                            oResponse.Message = ex.Message;

                            string ExceptionString = "Api : PutMHMCommonBenefitsMst" + Environment.NewLine;
                            ExceptionString += "Request :  " + JsonConvert.SerializeObject(mHMCommonBenefitsMst) + Environment.NewLine;
                            ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                            var fileName = "PutMHMCommonBenefitsMst - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                            Helpers.Service.LogError(fileName, ExceptionString);

                            dbContextTransaction.Rollback();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                            return response;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This api is used to Add new common Benefit
        /// </summary>
        /// <param name="mHMCommonBenefitsMst">MHM Common Benefit Object</param>
        /// <returns>Status</returns>
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage PostMHMCommonBenefitsMst(MHMCommonBenefitsMst mHMCommonBenefitsMst)
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
                using (var dbContextTransaction = DB.Database.BeginTransaction())
                {
                    try
                    {
                        var Benefit = DB.MHMCommonBenefitsMsts.Where(r => r.MHMBenefitName == mHMCommonBenefitsMst.MHMBenefitName).FirstOrDefault();
                        if (Benefit == null)
                        {
                            mHMCommonBenefitsMst.CreatedDateTime = DateTime.Now;
                            DB.MHMCommonBenefitsMsts.Add(mHMCommonBenefitsMst);
                            //mHMCommonBenefitsMst.MHMBenefitMappingMsts.ToList().ForEach(r => r.MHMCommonBenefitID = mHMCommonBenefitsMst.MHMBenefitID);
                            DB.SaveChanges();

                            oResponse.Status = true;
                            oResponse.Message = "Success";
                            dbContextTransaction.Commit();

                            //Remove Cache of PlanBenefitMaster
                            var PlanAttributeMasterFromCache = MHMCache.GetMyCachedItem("MHMBenefitMappingMaster");
                            if (PlanAttributeMasterFromCache != null)
                            {
                                MHMCache.RemoveMyCachedItem("MHMBenefitMappingMaster");
                            }

                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                        else
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Benefit name already exists.";
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        oResponse.Status = false;
                        oResponse.Message = ex.Message;

                        string ExceptionString = "Api : PostMHMCommonBenefitsMst" + Environment.NewLine;
                        ExceptionString += "Request :  " + JsonConvert.SerializeObject(mHMCommonBenefitsMst) + Environment.NewLine;
                        ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                        var fileName = "PostMHMCommonBenefitsMst - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                        Helpers.Service.LogError(fileName, ExceptionString);

                        dbContextTransaction.Rollback();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return response;
                    }
                }
            }
        }

        /// <summary>
        /// This api is used to deleter MHM Common Benefit if it's not used
        /// </summary>
        /// <param name="id">MHM Common Benefit id</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/Benefit/DeleteBenefit/{id}")]
        public HttpResponseMessage DeleteBenefit(long id)
        {
            Response oResponse = new Response();
            using (var DB = new MHMDal.Models.MHM())
            {
                MHMCommonBenefitsMst mHMCommonBenefitsMst = DB.MHMCommonBenefitsMsts.Find(id);
                if (mHMCommonBenefitsMst == null)
                {
                    oResponse.Status = false;
                    oResponse.Message = "Benefit cannot be found. ";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent, oResponse);
                    return response;
                }

                using (var dbContextTransaction = DB.Database.BeginTransaction())
                {
                    try
                    {
                        var status = DB.BenefitUserDetails.Where(r => r.MHMMappingBenefitId == mHMCommonBenefitsMst.MHMBenefitID).FirstOrDefault();
                        if (status == null)
                        {
                            var MHMBenefitMappingMsts = DB.MHMBenefitMappingMsts.Where(r => r.MHMCommonBenefitID == mHMCommonBenefitsMst.MHMBenefitID);
                            var MHMBenefitCostByAreaMsts = DB.MHMBenefitCostByAreaMsts.Where(r => r.MHMBenefitId == mHMCommonBenefitsMst.MHMBenefitID);
                            DB.MHMBenefitMappingMsts.RemoveRange(MHMBenefitMappingMsts);
                            DB.MHMBenefitCostByAreaMsts.RemoveRange(MHMBenefitCostByAreaMsts);
                            DB.MHMCommonBenefitsMsts.Remove(mHMCommonBenefitsMst);
                            DB.SaveChanges();
                            oResponse.Status = true;
                            oResponse.Message = "Success";
                            dbContextTransaction.Commit();

                            //Remove Cache of MHMBenefitMappingMaster
                            var MHMBenefitMappingMasterFromCache = MHMCache.GetMyCachedItem("MHMBenefitMappingMaster");
                            if (MHMBenefitMappingMasterFromCache != null)
                            {
                                MHMCache.RemoveMyCachedItem("MHMBenefitMappingMaster");
                            }

                            //Remove Cache of MHMBenefitCostByAreaMaster
                            var MHMBenefitCostByAreaMasterFromCache = MHMCache.GetMyCachedItem("MHMBenefitCostByAreaMaster");
                            if (MHMBenefitCostByAreaMasterFromCache != null)
                            {
                                MHMCache.RemoveMyCachedItem("MHMBenefitCostByAreaMaster");
                            }

                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                        else
                        {
                            oResponse.Status = false;
                            oResponse.Message = "This benefit is used in a case hence can't be deleted";
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        oResponse.Status = false;
                        oResponse.Message = ex.Message;

                        string ExceptionString = "Api : DeleteBenefit" + Environment.NewLine;
                        ExceptionString += "Request :  " + " id " + id + Environment.NewLine;
                        ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                        var fileName = "DeleteBenefit - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                        Helpers.Service.LogError(fileName, ExceptionString);

                        dbContextTransaction.Rollback();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return response;
                    }
                }
            }
        }

        /// <summary>
        /// This api is used to get all rating area
        /// </summary>
        /// <returns>list of rating areas</returns>
        [HttpGet]
        [Route("api/Benefit/GetRatingArea")]
        [Authorize]
        public HttpResponseMessage GetRatingArea()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var RatingList = DB.tblRatingAreaMsts.ToList();
                    if (RatingList.Count > 0)
                    {
                        data.Add("Status", true);
                        data.Add("Message", "Success");
                        data.Add("List", RatingList.Select(r => new { r.RatingAreaID, r.RatingAreaName }));
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;
                    }
                    else
                    {
                        data.Add("Status", true);
                        data.Add("Message", "Rating Area cannot be found. ");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                data.Add("Status", false);
                data.Add("Message", ex.Message);

                string ExceptionString = "Api : GetRatingArea" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(data) + Environment.NewLine;
                var fileName = "GetRatingArea - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, data);
                return response;

            }
        }

        /// <summary>
        /// This is api is used to get All the benefits 
        /// </summary>
        /// <param name="searchBy">Search Value</param>
        /// <param name="sortby">Sort By Value</param>
        /// <param name="desc">Ascending or Descending</param>
        /// <param name="page">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="filterByCategory">Category Id</param>
        /// <returns>List of benefits</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage AllBenefits(string searchBy, string sortby, bool desc, int page, int pageSize, long filterByCategory = 0)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    List<MHMCommonBenefitsMst> oBenefitlist = new List<MHMCommonBenefitsMst>();
                    oBenefitlist = DB.MHMCommonBenefitsMsts.ToList();
                    int skipRows = (page - 1) * pageSize;

                    if (!string.IsNullOrEmpty(searchBy))
                    {
                        //oBenefitlist = oBenefitlist.Where(x => x.MHMBenefitName.ToLower().StartsWith(searchBy.ToLower()) || x.CategoryId == (int.TryParse(searchBy, out num1) ? num1 : 0)).ToList();
                        oBenefitlist = oBenefitlist.Where(x => x.MHMBenefitName.ToLower().Contains(searchBy.ToLower())).ToList();
                    }
                    if (filterByCategory > 0)
                    {
                        oBenefitlist = oBenefitlist.Where(x => x.CategoryId == filterByCategory).ToList();
                    }

                    int totalRecords = oBenefitlist.Count();

                    if (totalRecords > 0)
                    {
                        switch (sortby)
                        {
                            case "MHMBenefitID":
                                oBenefitlist = desc ? oBenefitlist.OrderByDescending(x => x.MHMBenefitID).Skip(skipRows).Take(pageSize).ToList() : oBenefitlist.OrderBy(x => x.MHMBenefitID).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "MHMBenefitName":
                                oBenefitlist = desc ? oBenefitlist.OrderByDescending(x => x.MHMBenefitName).Skip(skipRows).Take(pageSize).ToList() : oBenefitlist.OrderBy(x => x.MHMBenefitName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "CategoryName":
                                oBenefitlist = desc ? oBenefitlist.OrderByDescending(x => x.CategoryMst.CategoryName).Skip(skipRows).Take(pageSize).ToList() : oBenefitlist.OrderBy(x => x.CategoryMst.CategoryName).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "CreatedDateTime":
                                oBenefitlist = desc ? oBenefitlist.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : oBenefitlist.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            default:
                                oBenefitlist = desc ? oBenefitlist.OrderByDescending(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList() : oBenefitlist.OrderBy(x => x.CreatedDateTime).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }

                        #region // For dynamic getting sortby

                        //ParameterExpression param = Expression.Parameter(typeof(MHMCommonBenefitsMst), "model");
                        //Expression propertyAccess = Expression.Property(param, sortby);
                        //var funcType = typeof(Func<object,object>).MakeGenericType(typeof(MHMCommonBenefitsMst), propertyAccess.Type);
                        //LambdaExpression Sortby = Expression.Lambda(funcType, propertyAccess, param);

                        // var param = Expression.Parameter(typeof(MHMCommonBenefitsMst), "model");
                        // var Sortby =Expression.Lambda<Func<MHMCommonBenefitsMst, object>>(Expression.Property(param, "MHMBenefitID"), param);    


                        //if (desc)
                        //    oBenefitlist = DB.MHMCommonBenefitsMsts.OrderByDescending(Sortby).Skip(skipRows).Take(pageSize).ToList();
                        //else
                        //    oBenefitlist = DB.MHMCommonBenefitsMsts.OrderBy(Sortby).Skip(skipRows).Take(pageSize).ToList();

                        #endregion

                        var lstUsers = DB.Users.ToList();
                        oBenefitlist.ForEach(r => r.CategoryMst = r.CategoryMst);

                        data.Add("Status", true);
                        data.Add("Message", "Success");
                        data.Add("Benefits", oBenefitlist.Select(r => new { r.MHMBenefitID, r.MHMBenefitName, r.CreatedDateTime, r.Createdby, r.CategoryId, r.CategoryMst.CategoryName, r.IsDefault, CreatedByName = r.Createdby != null ? lstUsers.Where(t => t.UserID == r.Createdby).FirstOrDefault().FirstName + " " + lstUsers.Where(t => t.UserID == r.Createdby).FirstOrDefault().LastName : "" }));
                        data.Add("TotalRecords", totalRecords);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;
                    }
                    else
                    {
                        data.Add("Status", "false");
                        data.Add("Message", "Benefit cannot be found. ");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                        return response;
                    }

                }
                catch (Exception ex)
                {
                    data.Add("Status", false);
                    data.Add("Message", ex.Message);

                    string ExceptionString = "Api : AllBenefits" + Environment.NewLine;
                    ExceptionString += "Request :  " + " filterByCategory " + filterByCategory + " ,searchBy " + searchBy + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(data) + Environment.NewLine;
                    var fileName = "AllBenefits - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, data);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to get all the Issuer/Carrier 
        /// </summary>
        /// <returns>list of Carrier / Inssuer</returns>
        [HttpGet]
        [Route("api/Benefit/Getcarrier")]
        //[Authorize]
        public CarrierViewModel Carrier()
        {
            CarrierViewModel oCarrierViewModel = new CarrierViewModel();
            List<IssuerMst> oModellst = new List<IssuerMst>();

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    //BenefitIdsViewModel class inside the IssuerMst class
                    List<BenefitIdsViewModel> lstIssuerBenefitIds = DB.Database.SqlQuery<BenefitIdsViewModel>("exec sp_GetBenefitId").ToList<BenefitIdsViewModel>();
                    var result = DB.IssuerMsts.Where(r => r.Status == true).ToList();
                    foreach (var item in result)
                    {
                        oModellst.Add(new IssuerMst { Id = item.Id, IssuerCode = item.IssuerCode, IssuerName = item.IssuerName + " (" + item.IssuerCode + ")", Abbreviations = item.Abbreviations, BenefitIds = lstIssuerBenefitIds.Where(r => r.CarrierId == item.Id).ToList() });
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
                }
            }
            catch (Exception ex)
            {
                oCarrierViewModel.Status = false;
                oCarrierViewModel.Message = ex.Message;

                string ExceptionString = "Api : Carrier" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oCarrierViewModel) + Environment.NewLine;
                var fileName = "Carrier - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

            }
            return oCarrierViewModel;
        }

        #region NewChanges(17-Feb)

        /// <summary>
        /// This api is used to get all the States
        /// </summary>
        /// <returns>list of states</returns>
        [HttpGet]
        [Route("api/benefit/GetAllStates")]
        [Authorize]
        public HttpResponseMessage GetAllStates()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var States = DB.tblStateAbrevs.ToList();

                    if (States.Count() > 0)
                    {
                        res.Add("Status", "true");
                        res.Add("Message", "States List");
                        res.Add("States", States.Select(r => new { StateID = r.Id, r.StateName, r.StateCode }).OrderBy(r => r.StateName));
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "False");
                        res.Add("Message", "State cannot be found. ");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Add("Status", "False");
                res.Add("Message", ex.Message);

                string ExceptionString = "Api : GetAllStates" + Environment.NewLine;
                ExceptionString += "Request :  " + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(res) + Environment.NewLine;
                var fileName = "GetAllStates - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, res);
                return response;
            }
        }

        /// <summary>
        /// This api is used to get all the benftis based on Rating Area Id and StateCode
        /// </summary>
        /// <param name="RatingAreaId">Rating Area Id</param>
        /// <param name="StateCode">State Code</param>
        /// <returns>list of benefits</returns>
        [HttpGet]
        [Authorize]
        public HttpResponseMessage AllBenefits(long RatingAreaId, string StateCode)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();

            try
            {
                DataTable dtusage = new DataTable();
                objCon.Open();
                cmd = new SqlCommand("get_AllBenefitCost_List", objCon);
                cmd.Parameters.AddWithValue("@RatingAreaId", RatingAreaId);
                cmd.Parameters.AddWithValue("@StateCode", StateCode);

                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtusage);

                foreach (DataRow dr in dtusage.Rows)
                {
                    Dictionary<string, object> odata = new Dictionary<string, object>();

                    // odata.Add("CategoryId", Convert.ToInt16(dr["CategoryId"]));
                    // odata.Add("CategoryName", dr["CategoryName"].ToString());
                    odata.Add("MHMBenefitID", Convert.ToInt64(dr["MHMBenefitID"]));
                    odata.Add("MHMBenefitName", dr["MHMBenefitName"].ToString());
                    odata.Add("IsDefault", Convert.ToBoolean(dr["IsDefault"]));
                    odata.Add("MHMBenefitCost", Convert.ToDecimal(dr["MHMBenefitCost"]));

                    res.Add(odata);
                }
                if (res.Count() == 0)
                {
                    data.Add("Status", false);
                    data.Add("Message", "Benefit cannot be found. ");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                    return response;
                }
                else
                {
                    data.Add("Status", true);
                    data.Add("Message", "Success");
                    data.Add("Benefits", res);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                    return response;
                }
            }
            catch (Exception ex)
            {
                data.Add("Status", false);
                data.Add("Message", ex.Message);

                string ExceptionString = "Api : AllBenefits" + Environment.NewLine;
                ExceptionString += "Request :  " + " RatingAreaId " + RatingAreaId + " ,StateCode " + StateCode + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(res) + Environment.NewLine;
                var fileName = "AllBenefits - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, data);
                return response;
            }
            finally
            {
                cmd.Dispose();
                objCon.Close();
            }
        }

        /// <summary>
        /// This api is used to Save the cost of Benefits
        /// </summary>
        /// <param name="oModel">Json Object of Benefit Cost</param>
        /// <returns>Status</returns>
        [ActionName("SaveBenefitCost")]
        [HttpPost]
        [Route("api/benefit/SaveBenefitCost")]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage SaveBenefitCost(BenefitCostViewModel oModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (!ModelState.IsValid)
            {
                string messages = string.Join(Environment.NewLine, ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                data.Add("Status", false);
                data.Add("Message", messages);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                return response;
            }

            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    var result = DB.MHMBenefitCostByAreaMsts.Where(m => m.RatingAreaID == oModel.RatingAreaID && m.StateCode == oModel.StateCode).ToList();
                    DB.MHMBenefitCostByAreaMsts.RemoveRange(result);
                    //foreach (var item in result)
                    //{
                    //    MHMBenefitCostByAreaMst oMHMBenefitCostByAreaMst = new MHMBenefitCostByAreaMst();
                    //    oMHMBenefitCostByAreaMst = item;
                    //    DB.MHMBenefitCostByAreaMsts.Remove(oMHMBenefitCostByAreaMst);
                    //}

                    // DB.MHMBenefitCostByAreaMsts.AddRange(oModel.Benefits);
                    bool FinalStatus = false;

                    foreach (var item in oModel.Benefits)
                    {
                        var Status = DB.MHMCommonBenefitsMsts.Select(r => r.MHMBenefitID).Contains(item.MHMBenefitId);
                        if (Status)
                        {
                            MHMBenefitCostByAreaMst oMHMBenefitCost = new MHMBenefitCostByAreaMst();
                            oMHMBenefitCost.MHMBenefitId = item.MHMBenefitId;
                            oMHMBenefitCost.RatingAreaID = oModel.RatingAreaID;
                            oMHMBenefitCost.MHMBenefitCost = item.MHMBenefitCost;
                            oMHMBenefitCost.StateCode = oModel.StateCode;
                            oMHMBenefitCost.Createdby = oModel.Createdby;
                            oMHMBenefitCost.CreatedDateTime = DateTime.Now;
                            DB.MHMBenefitCostByAreaMsts.Add(oMHMBenefitCost);
                            DB.SaveChanges();
                        }
                        else
                        {
                            FinalStatus = true;
                        }
                    }

                    //Remove Cache of PlanBenefitMaster
                    var MHMBenefitCostByAreaMasterFromCache = MHMCache.GetMyCachedItem("MHMBenefitCostByAreaMaster");
                    if (MHMBenefitCostByAreaMasterFromCache != null)
                    {
                        MHMCache.RemoveMyCachedItem("MHMBenefitCostByAreaMaster");
                    }

                    data.Add("Status", true);
                    data.Add("Message", FinalStatus ? "Success ! but some benefit deleted" : "Success");
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
                    return response;
                }
            }
            catch (Exception ex)
            {
                data.Add("Status", false);
                data.Add("Message", ex.Message);

                string ExceptionString = "Api : SaveBenefitCost" + Environment.NewLine;
                ExceptionString += "Request :  " + JsonConvert.SerializeObject(oModel) + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(data) + Environment.NewLine;
                var fileName = "SaveBenefitCost - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, data);
                return response;
            }
        }


        #endregion

        /// <summary>
        /// This api is used to Check benefit is mapped for that issuer or not
        /// </summary>
        /// <param name="CarrierId">Carrier Id</param>
        /// <param name="IssuerBenefitId">Issuer Benefit Id</param>
        /// <returns>Status</returns>
        [ActionName("CheckBenefitMapping")]
        [HttpGet]
        [Route("api/benefit/CheckBenefitMapping/{CarrierId}/{IssuerBenefitId}/{CommonBenefitId}")]
        //[Authorize(Roles = "Admin")]
        public HttpResponseMessage CheckBenefitMapping(long CarrierId, long IssuerBenefitId, long CommonBenefitId)
        {
            Response oResponse = new Response();
            try
            {
                using (var DB = new MHMDal.Models.MHM())
                {
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    var MHMBenefitMapping = DB.MHMBenefitMappingMsts.Where(e => e.IssuerID == CarrierId && e.IssuerBenefitID == IssuerBenefitId).ToList();
                    res.Add("Status", "true");
                    if (MHMBenefitMapping.Count > 0 && MHMBenefitMapping.Count(r => r.MHMCommonBenefitID != CommonBenefitId) > 0)
                    {
                        res.Add("Message", "This Issuer Benefit already assign to another Issuer.");
                        res.Add("BenefitMappingStatus", false);
                    }
                    else
                    {
                        res.Add("Message", "Success");
                        res.Add("BenefitMappingStatus", true);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, res);
                }
            }
            catch (Exception ex)
            {
                oResponse.Status = false;
                oResponse.Message = ex.Message;

                string ExceptionString = "Api : CheckBenefitMapping" + Environment.NewLine;
                ExceptionString += "Request :  " + " CarrierId " + CarrierId + " ,IssuerBenefitId " + IssuerBenefitId + " ,CommonBenefitId " + CommonBenefitId + Environment.NewLine;
                ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                var fileName = "CheckBenefitMapping - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                Helpers.Service.LogError(fileName, ExceptionString);

                return Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        DB.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private bool MHMCommonBenefitsMstExists(long id)
        {
            using (var DB = new MHMDal.Models.MHM())
            {
                return DB.MHMCommonBenefitsMsts.Count(e => e.MHMBenefitID == id) > 0;
            }
        }
    }
}