using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using MHM.Api.Models;
using MHM.Api.Providers;
using MHM.Api.Results;
using MHMDal.Models;
using System.Data.Entity.Validation;
using MHM.Api.ViewModel;
using System.Data.Entity;
using System.Web.Http.Cors;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using MHM.Api.Helpers;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.IO;

namespace MHM.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        //SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MHM"].ConnectionString);
        //MHMDal.Models.MHM DB = new MHMDal.Models.MHM();

        public UsersController()
        {

        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Users/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]

        /// <summary>
        /// This api is used to Register new user
        /// </summary>
        /// <param name="model">Json Object</param>
        /// <returns>Status</returns>
        [Authorize(Roles = "Admin")]
        [Route("Register")]
        public async Task<HttpResponseMessage> Register(RegisterViewModel model)
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

                    var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, PhoneNumber = model.Phone, LockoutEnabled = false };

                    IdentityResult result = await UserManager.CreateAsync(user, GeneratePassword.GetPassword(2, 2, 2));

                    if (!result.Succeeded)
                    {
                        oResponse.Status = false;
                        oResponse.Message = GetErrorResult(result).ToString();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return response;
                        //return GetErrorResult(result).ToString();
                    }
                    else
                    {
                        try
                        {
                            if (result.Succeeded)
                            {
                                // Add User Role
                                if (user != null) UserManager.AddToRole(user.Id, model.Role);

                                // Add New User details in User table 
                                User oUsers = new User();
                                oUsers.FirstName = model.FirstName;
                                oUsers.LastName = model.LastName;
                                oUsers.Email = model.Email;
                                oUsers.CreatedBy = model.CreatedBy;
                                oUsers.ClientCompanyId = model.ClientCompanyId;
                                oUsers.IsActive = true;
                                oUsers.CreateDate = DateTime.Now;
                                DB.Users.Add(oUsers);
                                DB.SaveChanges();
                                oResponse.Status = true;
                                oResponse.Message = "Success";

                                string body = string.Empty;
                                //changes given by ashish sir -- '/#/'
                                var url = model.BaseUrl + "/set-password/" + user.Id;
                                body = "Dear : " + oUsers.FirstName + " " + oUsers.LastName + "<br/><br/> Thank you for registering." + Environment.NewLine + " <br/> Please click the Link Below to confirm your account and set up your password." + Environment.NewLine + " <br/>  <a href='" + url + "'>" + url + "</a>";

                                try
                                {
                                    // Send new password to user
                                    Service.SendMail(model.Email.Trim(), "Email Confirmation", body);
                                }
                                catch { Service.SendMail(model.Email.Trim(), "Email Confirmation", body); }


                            }
                        }
                        catch (Exception ex)
                        {
                            UserManager.RemoveFromRole(user.Id, model.Role);
                            UserManager.Delete(user);

                            oResponse.Status = false;
                            oResponse.Message = ex.Message;

                        }

                        string ExceptionString = "Api : Register" + Environment.NewLine;
                        ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                        ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                        var fileName = "Register - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                        Helpers.Service.LogError(fileName, ExceptionString);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : Register" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "Register - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to update user
        /// </summary>
        /// <param name="model">Json Object</param>
        /// <returns>Status</returns>
        [Route("Update")]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Update(UpdateViewModel model)
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
                AspNetUser oAspNetUser = DB.AspNetUsers.Where(m => m.Email == model.Email).FirstOrDefault();

                if (oAspNetUser == null)
                {
                    oResponse.Status = false;
                    oResponse.Message = "User does not exist.";
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
                else
                {
                    try
                    {
                        oAspNetUser.PhoneNumber = model.Phone;
                        var userRole = oAspNetUser.AspNetRoles.FirstOrDefault().Name;
                        DB.Entry(oAspNetUser).State = EntityState.Modified;
                        UserManager.RemoveFromRole(oAspNetUser.Id, userRole);
                        UserManager.AddToRole(oAspNetUser.Id, model.Role);
                        DB.SaveChanges();

                        User oUser = DB.Users.Where(m => m.UserID == model.UserID).FirstOrDefault();
                        if (oUser != null)
                        {
                            // Add New User details in User table 
                            oUser.FirstName = model.FirstName;
                            oUser.LastName = model.LastName;
                            oUser.ClientCompanyId = model.ClientCompanyId;
                            //oUser.IsActive = true;
                            oUser.ModifiedDate = DateTime.Now;
                            oUser.ModifiedBy = model.ModifiedBy;
                            DB.Entry(oUser).State = EntityState.Modified;
                            DB.SaveChanges();

                            Dictionary<string, object> data = new Dictionary<string, object>();
                            Dictionary<string, object> res = new Dictionary<string, object>();

                            data.Add("FirstName", oUser.FirstName);
                            data.Add("LastName", oUser.LastName);
                            data.Add("Email", oUser.Email);
                            data.Add("Phone", oAspNetUser.PhoneNumber);
                            data.Add("id", oUser.UserID);
                            res.Add("Status", "true");
                            res.Add("Message", "Success");
                            res.Add("Customer", data);
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                        else
                        {
                            oResponse.Status = false;
                            oResponse.Message = "User not found.";
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        oResponse.Status = false;
                        oResponse.Message = ex.Message;

                        string ExceptionString = "Api : Update" + Environment.NewLine;
                        ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                        ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                        var fileName = "Update - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                        Helpers.Service.LogError(fileName, ExceptionString);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                        return response;
                    }
                }
            }
        }

        /// <summary>
        /// This is api is used to login
        /// </summary>
        /// <param name="omodel">Json Object</param>
        /// <returns>Status with authentication token</returns>
        [Route("Login")]
        public async Task<HttpResponseMessage> Login(LoginViewModel omodel)
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
                    var users = await UserManager.FindByNameAsync(omodel.Email);

                    if (users != null)
                    {
                        var validCredentials = await UserManager.FindAsync(omodel.Email, omodel.Password);
                        int accessFailedCount = await UserManager.GetAccessFailedCountAsync(users.Id);
                        // When a user is lockedout, this check is done to ensure that even if the credentials are valid
                        // the user can not login until the lockout duration has passed

                        //fetch user here to get UserId of that user
                        User oUser = DB.Users.Where(m => m.Email == omodel.Email).FirstOrDefault();
                        if (!oUser.IsActive)
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Your Account is inactive. Please contact your system administrator.";
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                        AuditLog oAuditLog = new AuditLog() { LoginDate = DateTime.Now.Date, LoginTime = DateTime.Now.TimeOfDay, UserId = oUser.UserID };

                        if (!await UserManager.IsEmailConfirmedAsync(users.Id))
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Your user account has not been verified. Please check your email and setup your password.";
                            oAuditLog.LoginStatus = Convert.ToInt16(LoginStatus.EmailNotVerified);
                            DB.AuditLogs.Add(oAuditLog);
                            DB.SaveChanges();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }

                        if (await UserManager.IsLockedOutAsync(users.Id))
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Your acccount has been locked due to multiple failed login attempts. Please contact your system administrator.";
                            oAuditLog.LoginStatus = Convert.ToInt16(LoginStatus.AccountLockedDueToMultipleWrongAttempt);
                            DB.AuditLogs.Add(oAuditLog);
                            DB.SaveChanges();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                        else if (validCredentials == null)
                        {
                            string message;

                            users.AccessFailedCount = accessFailedCount;
                            await UserManager.AccessFailedAsync(users.Id);

                            if (accessFailedCount >= 2)
                            {
                                await UserManager.SetLockoutEnabledAsync(users.Id, true);
                                await UserManager.SetLockoutEndDateAsync(users.Id, DateTimeOffset.MaxValue);
                                message = "Your acccount has been locked due to multiple failed login attempts. Please contact your system administrator.";
                            }
                            else
                            {

                                int attemptsLeft = 3 - accessFailedCount - 1;
                                message = string.Format(
                                    "Invalid login credentials. You have ({0}) more attempt(s) before your account is locked.", attemptsLeft);
                            }
                            oResponse.Status = false;
                            oResponse.Message = message;
                            oAuditLog.LoginStatus = Convert.ToInt16(LoginStatus.InvalidCredentials);
                            DB.AuditLogs.Add(oAuditLog);
                            DB.SaveChanges();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                            return response;
                        }
                        else
                        {
                            // When token is verified correctly, clear the access failed count used for lockout
                            await UserManager.ResetAccessFailedCountAsync(users.Id);
                            await UserManager.SetLockoutEnabledAsync(users.Id, false);

                            string Role = "";

                            Boolean roles = await UserManager.IsInRoleAsync(users.Id, "Agent");
                            if (roles)
                                Role = "Agent";
                            else
                                Role = "Admin";

                            string Token = GetToken(omodel);//  UserManager.GenerateUserToken("Token",user.Id);

                            //User oUser = DB.Users.Where(m => m.Email == omodel.Email).FirstOrDefault();

                            Dictionary<string, object> data = new Dictionary<string, object>();
                            Dictionary<string, object> res = new Dictionary<string, object>();

                            data.Add("FirstName", oUser.FirstName);
                            data.Add("LastName", oUser.LastName);
                            data.Add("Email", oUser.Email);
                            data.Add("Phone", users.PhoneNumber);
                            data.Add("id", oUser.UserID);
                            data.Add("Role", Role);
                            data.Add("LastLogin", oUser.LastLogin);
                            res.Add("Status", "true");
                            res.Add("Token", Token.ToString());
                            res.Add("Message", "Success");
                            res.Add("Customer", data);
                            oUser.LastLogin = DateTime.Now;

                            oAuditLog.LoginStatus = Convert.ToInt16(LoginStatus.LoginSuccess);
                            DB.AuditLogs.Add(oAuditLog);
                            DB.SaveChanges();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                            return response;
                        }
                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Invalid login credentials. Please try again.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : Login" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(omodel) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "Login - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to get all the users
        /// </summary>
        /// <param name="searchby">Search By value</param>
        /// <param name="sortby">Sort By value</param>
        /// <param name="desc">Ascending or Descending</param>
        /// <param name="page">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="lstParameter">list of parameters</param>
        /// <returns>list of users</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage GetUsers2(string searchby, string sortby, bool desc, int page, int pageSize, [FromUri]  List<string> lstParameter)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    int skipRows = (page - 1) * pageSize;

                    var user = DB.AspNetUsers.OrderByDescending(r => r.Id);

                    Dictionary<string, object> res = new Dictionary<string, object>();
                    List<UsersViewModel> UserList = new List<UsersViewModel>();

                    //var userrole = UserManager.role
                    //ApplicationUser currentUser = DB.Users.FirstOrDefault(x => x.UserID == User.Identity.GetUserId());



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
                                Email = oUser.Email,
                                IsActive = oUser.IsActive,
                                Phone = item.PhoneNumber,
                                UserLockedStatus = item.LockoutEnabled,
                                Role = UserManager.GetRoles(item.Id).First(),
                                LastLogin = oUser.LastLogin,
                                CreatedBy = oUser.CreatedBy,
                                CreateDate = oUser.CreateDate,
                                ModifiedDate = Convert.ToDateTime(oUser.ModifiedDate)
                            });
                        }
                    }

                    if (!string.IsNullOrEmpty(searchby))
                    {
                        UserList = UserList.Where(x => x.Name.ToLower().Contains(searchby.ToLower()) || x.Email.ToLower().Contains(searchby.ToLower()) || x.Phone.Contains(searchby)).ToList();
                    }

                    if (!string.IsNullOrEmpty(lstParameter[0]))
                    {
                        UserList = UserList.Where(x => x.Name.ToLower().Contains(lstParameter[0].ToLower())).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[1]))
                    {
                        UserList = UserList.Where(x => x.Email.ToLower().Contains(lstParameter[1].ToLower())).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[2]))
                    {
                        UserList = UserList.Where(x => x.Role.ToLower().StartsWith(lstParameter[2].ToLower())).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[3]))
                    {
                        UserList = UserList.Where(x => x.IsActive == Convert.ToBoolean(lstParameter[3])).ToList();
                    }
                    if (!string.IsNullOrEmpty(lstParameter[4]))
                    {
                        UserList = UserList.Where(x => x.UserLockedStatus == Convert.ToBoolean(lstParameter[4])).ToList();
                    }

                    int total = UserList.Count();

                    if (total > 0)
                    {

                        switch (sortby)
                        {
                            case "Name":
                                UserList = desc ? UserList.OrderByDescending(x => x.Name).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.Name).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "Email":
                                UserList = desc ? UserList.OrderByDescending(x => x.Email).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.Email).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "CreateDate":
                                UserList = desc ? UserList.OrderByDescending(x => x.CreateDate).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.CreateDate).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "Role":
                                UserList = desc ? UserList.OrderByDescending(x => x.Role).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.Role).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "IsActive":
                                UserList = desc ? UserList.OrderByDescending(x => x.IsActive).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.IsActive).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "UserLockedStatus":
                                UserList = desc ? UserList.OrderByDescending(x => x.UserLockedStatus).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.UserLockedStatus).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            case "LastLogin":
                                UserList = desc ? UserList.OrderByDescending(x => x.LastLogin).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.LastLogin).Skip(skipRows).Take(pageSize).ToList();
                                break;

                            default:
                                UserList = desc ? UserList.OrderByDescending(x => x.CreateDate).Skip(skipRows).Take(pageSize).ToList() : UserList.OrderBy(x => x.CreateDate).Skip(skipRows).Take(pageSize).ToList();
                                break;
                        }




                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("Users", UserList);
                        res.Add("TotalCount", total);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "false");
                        res.Add("Message", "User does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetUsers2" + Environment.NewLine;
                    ExceptionString += "Request : " + " searchby " + searchby + " ,sortby " + sortby + " ,desc " + desc + " ,page " + page + " ,pageSize " + pageSize + " ,lstParameter " + lstParameter + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetUsers2 - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to get user details
        /// </summary>
        /// <param name="UserId">User id</param>
        /// <returns>User Details</returns>
        [Route("Detail/{UserId}")]
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public HttpResponseMessage Detail(long UserId)
        {
            Response oResponse = new Response();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    User oUser = DB.Users.Where(m => m.UserID == UserId).FirstOrDefault();
                    if (oUser != null)
                    {
                        var user = UserManager.FindByEmail(oUser.Email);
                        Dictionary<string, object> res = new Dictionary<string, object>();
                        Dictionary<string, object> data = new Dictionary<string, object>();

                        if (user != null)
                        {
                            data.Add("id", oUser.UserID);
                            data.Add("Phone", user.PhoneNumber);
                            data.Add("FirstName", oUser.FirstName);
                            data.Add("LastName", oUser.LastName);
                            data.Add("Email", oUser.Email);
                            data.Add("Role", UserManager.GetRoles(user.Id).First());
                            data.Add("ClientCompanyId", oUser.ClientCompanyId);
                            data.Add("CreateDate", oUser.CreateDate);
                            data.Add("ModifiedDate", oUser.ModifiedDate);
                        }

                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        res.Add("Users", data);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "User does not exist.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : Detail" + Environment.NewLine;
                    ExceptionString += "Request : UserId " + UserId + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "Detail - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        // POST api/Users/Delete
        //[Route("Delete/{UserId}")]
        //[Authorize(Roles = "Admin")]
        //public HttpResponseMessage Delete(long UserId)
        //{
        //    Response oResponse = new Response();
        //    try
        //    {
        //        if (UserId == 0)
        //        {
        //            oResponse.Status = false;
        //            oResponse.Message = "UserId can't be 0";
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
        //            return response;
        //        }

        //        User oUser = DB.Users.Where(m => m.UserID == UserId).FirstOrDefault();
        //        if (oUser != null)
        //        {
        //            var user = UserManager.FindByEmail(oUser.Email);
        //            UserManager.RemoveFromRole(user.Id, "Agent");
        //            UserManager.Delete(user);

        //            DB.Users.Remove(oUser);

        //            oResponse.Status = true;
        //            oResponse.Message = "Success";
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
        //            return response;
        //        }
        //        else
        //        {
        //            oResponse.Status = false;
        //            oResponse.Message = "User not found.";
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oResponse.Status = false;
        //        oResponse.Message = ex.Message;
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
        //        return response;
        //    }
        //}

        /// <summary>
        /// This api is used to change the password
        /// </summary>
        /// <param name="model">Json Object</param>
        /// <returns>Status</returns>
        [Route("ChangePassword")]
        [Authorize]
        public async Task<HttpResponseMessage> ChangePassword(ChangePasswordBindingModel model)
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
                    var user = await UserManager.FindByEmailAsync(model.Email);
                    if (model.OldPassword == model.NewPassword)
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Old password and new password can't be same.";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }

                    IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword,
                        model.NewPassword);

                    if (!result.Succeeded)
                    {
                        oResponse.Status = false;
                        oResponse.Message = GetErrorResult(result).ToString();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                    else
                    {
                        oResponse.Status = true;
                        oResponse.Message = "Success";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : ChangePassword" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "ChangePassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to Reset Password
        /// </summary>
        /// <param name="model">Json Object</param>
        /// <returns>Status</returns>
        [Route("ResetPassword")]
        public async Task<HttpResponseMessage> ResetPassword(SetPasswordBindingModel model)
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
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(DB);
                    var user = await UserManager.FindByEmailAsync(model.Email);
                    var oUser = DB.Users.Where(r => r.Email == model.Email).FirstOrDefault();
                    if (oUser != null && user != null)
                    {
                        String userId = user.Id;
                        String newPassword = GeneratePassword.GetPassword(2, 2, 2);
                        // System.Web.Security.Membership.GeneratePassword(8, 2);
                        String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
                        ApplicationUser cUser = await UserManager.FindByIdAsync(userId);
                        await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                        await UserManager.UpdateAsync(cUser);

                        string Token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        var result = await UserManager.ConfirmEmailAsync(user.Id, Token);

                        string body = string.Empty;

                        body = "Dear: " + oUser.FirstName + " " + oUser.LastName + "<br/> Here is your new password : " + newPassword + "<br/> Please login using the password above.<br/><br/>" + Environment.NewLine + "Thank you,<br/>" + Environment.NewLine + " MyHealthMath";

                        // Send new password to user
                        Service.SendMail(model.Email.Trim(), "Your Account Password", body);

                        oResponse.Status = true;
                        oResponse.Message = "Success";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Email address not found. ";
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : ResetPassword" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "ResetPassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is use to Set user password and verify user
        /// </summary>
        /// <param name="model">Jsob Object</param>
        /// <returns>Status</returns>
        [Route("SetPassword")]
        public async Task<HttpResponseMessage> SetPassword(SetPasswordModel model)
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
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(DB);
                    var user = await UserManager.FindByIdAsync(model.Id);

                    String userId = user.Id;
                    String newPassword = model.ConfirmPassword;
                    String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
                    ApplicationUser cUser = await UserManager.FindByIdAsync(userId);
                    if (!cUser.EmailConfirmed)
                    {
                        await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                        await UserManager.UpdateAsync(cUser);

                        string Token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        var result = await UserManager.ConfirmEmailAsync(user.Id, Token);
                        if (result.Succeeded)
                        {
                            oResponse.Status = true;
                            oResponse.Message = "Success";
                        }
                        else
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Please try again after a few minutes.";
                        }

                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Your account is already verified.";
                    }

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : SetPassword" + Environment.NewLine;
                    ExceptionString += "Request : " + JsonConvert.SerializeObject(model) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "SetPassword - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to cofirm the user email id
        /// </summary>
        /// <param name="Id">Email Id</param>
        /// <returns>Status</returns>
        [HttpGet]
        [Route("CheckEmailStatus/{Id}")]
        public async Task<HttpResponseMessage> CheckEmailStatus(string Id)
        {
            Response oResponse = new Response();
            if (Id == string.Empty)
            {

                oResponse.Status = false;
                oResponse.Message = "Id is necessory";
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return response;
            }
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {

                    ApplicationUser cUser = await UserManager.FindByIdAsync(Id);
                    if (cUser == null)
                    {
                        oResponse.Status = false;
                        oResponse.Message = "Invalid Request";
                    }
                    else
                    {
                        if (!cUser.EmailConfirmed)
                        {
                            oResponse.Status = true;
                            oResponse.Message = "Email not confirmed";
                        }
                        else
                        {
                            oResponse.Status = false;
                            oResponse.Message = "Email already confirmed";
                        }
                    }

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : CheckEmailStatus" + Environment.NewLine;
                    ExceptionString += "Request : Id " + Id + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "CheckEmailStatus - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to Unlock the user
        /// </summary>
        /// <param name="emailId">Email Id</param>
        /// <returns>Status</returns>
        [HttpGet]
        [Route("UnlockUser")]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> UnlockUser(string emailId)
        {
            Response oResponse = new Response();
            if (emailId == string.Empty)
            {

                oResponse.Status = false;
                oResponse.Message = "Email address cannot be blank.";
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                return response;
            }
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var users = await UserManager.FindByNameAsync(emailId);
                    if (users != null)
                    {
                        await UserManager.SetLockoutEnabledAsync(users.Id, false);

                        oResponse.Status = true;
                        oResponse.Message = "User account has been unlocked.";
                    }
                    else
                    {
                        oResponse.Status = false;
                        oResponse.Message = "User does not exist.";
                    }

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, oResponse);
                    return response;
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : UnlockUser" + Environment.NewLine;
                    ExceptionString += "Request : emailId " + emailId + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "UnlockUser - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to get all client companies
        /// </summary>
        /// <returns>List of client companies</returns>
        [HttpGet]
        [Route("GetClientCompanies")]
        [Authorize]
        public HttpResponseMessage GetClientCompanies()
        {
            Response oResponse = new Response();
            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var companies = DB.ClientCompanies.ToList();

                    Dictionary<string, object> res = new Dictionary<string, object>();

                    res.Add("Status", "true");
                    res.Add("Message", "Success");
                    res.Add("Companies", companies.Select(r => new { r.CompanyId, r.CompanyName }));

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;

                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : GetClientCompanies" + Environment.NewLine;
                    ExceptionString += "Request :  " + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "GetClientCompanies - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This api is used to active or de-active user
        /// </summary>
        /// <param name="objUserStatus">Json</param>
        /// <returns>Status</returns>
        [HttpPost]
        [Route("ChangeUserStatus")]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage ChangeUserStatus(ChangeUserStatus objUserStatus)
        {
            Response oResponse = new Response();
            Dictionary<string, object> res = new Dictionary<string, object>();

            using (var DB = new MHMDal.Models.MHM())
            {
                try
                {
                    var user = DB.Users.Find(objUserStatus.UserId);
                    if (user != null)
                    {
                        user.IsActive = objUserStatus.Status;
                        DB.SaveChanges();
                        res.Add("Status", "true");
                        res.Add("Message", "Success");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                    else
                    {
                        res.Add("Status", "False");
                        res.Add("Message", "User does not exist.");
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    oResponse.Status = false;
                    oResponse.Message = ex.Message;

                    string ExceptionString = "Api : ChangeUserStatus" + Environment.NewLine;
                    ExceptionString += "Request :  " + JsonConvert.SerializeObject(objUserStatus) + Environment.NewLine;
                    ExceptionString += "Exception : " + JsonConvert.SerializeObject(oResponse) + Environment.NewLine;
                    var fileName = "ChangeUserStatus - " + System.DateTime.Now.ToString("MM-dd-yyyy hh-mm-ss");
                    Helpers.Service.LogError(fileName, ExceptionString);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, oResponse);
                    return response;
                }
            }
        }

        /// <summary>
        /// This method is used to get access token for authentication
        /// </summary>
        /// <param name="omodel"></param>
        /// <returns>Token</returns>
        [NonAction]
        private string GetToken(LoginViewModel omodel)
        {
            string baseAddress = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>    
               {    
                   {"grant_type", "password"},    
                   {"username", omodel.Email},    
                   {"password", omodel.Password},    
               };
                var tokenResponse = client.PostAsync(baseAddress + "/token", new FormUrlEncodedContent(form)).Result;
                //var token = tokenResponse.Content.ReadAsStringAsync().Result;  
                var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

                if (string.IsNullOrEmpty(token.Error))
                {
                    return token.AccessToken; ;//  Console.WriteLine("Token issued is: {0}", token.AccessToken);
                }
                else
                {
                    return token.Error; // Console.WriteLine("Error : {0}", token.Error);
                }
            }
        }

        public class Token
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
            [JsonProperty("error")]
            public string Error { get; set; }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _userManager != null)
        //    {
        //        _userManager.Dispose();
        //        _userManager = null;
        //    }

        //    base.Dispose(disposing);
        //}

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private string GetErrorResult(IdentityResult result)
        {
            string errormsg = string.Empty;

            if (result == null)
            {
                // return InternalServerError();
                return "InternalServerError";
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                        errormsg = error;
                    }
                }
                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    //return BadRequest();
                    return "BadRequest";
                }
                return errormsg;
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}

