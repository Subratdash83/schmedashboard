using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using ABP.Domain.Login;
using ABP.Persistence;
using ABP.Repository.Contract.Contract.Login;
using ClientsideEncryption;
using ABP.Repository.SendMessage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;

using System.IO;
using System.Net;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Block;
using Microsoft.AspNetCore.Authentication.Cookies;
using ABP.Repository.Contract.Contract.Department;
using Hangfire;
using Hangfire.MemoryStorage;
using ABP.Infrastructure;
using ABP.Domain.Login.OTP;
using ABP.Repository.Login;
using Twilio.Types;
using Oracle.ManagedDataAccess.Client;

namespace ABP.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly IDepartmentRepository _dpartmentRepository;
        private readonly ILogger<LoginRepository> _logger;
        public IConfiguration Configuration { get; }
        private readonly ISendSMSRepository _sms;
        public LoginController(ILoginRepository loginRepository, IConfiguration configuration, IDistrictRepository districtRepository,
            IDepartmentRepository departmentRepository, ISendSMSRepository sms, ILogger<LoginRepository> logger)
        {
            _loginRepository = loginRepository;
            _districtRepository = districtRepository;
            Configuration = configuration;
            _dpartmentRepository = departmentRepository;
            _sms = sms;
            _logger = logger;
            Log.Information("LoginController Initialized");
        }
        [HttpGet]
        public IActionResult UserLogin()
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            ViewBag.msg = null;
            return View();
        }
        [HttpGet]
        public IActionResult DeptLogin()
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");

            ViewBag.msg = null;
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
            ViewBag.msg = null;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login Model)
        {
            Log.Information("Login Initialized!");
            try
            {

                Model.VCHUSERNAME = AESEncrytDecry.DecryptStringAES(Model.encryptedtxtuname);
                Model.vchpassword = AESEncrytDecry.DecryptStringAES(Model.encryptedtxtpwd);
                if (Model.VCHUSERNAME == null)
                {
                    Model.Message = "Please enter user name.";
                    return View(Model);
                }
                else if (Model.vchpassword == null)
                {
                    Model.Message = "Please enter password.";
                    return View(Model);
                }
                else
                {
                    var Result = _loginRepository.GetLoginDetailsByUserId(Model).Result.FirstOrDefault();
                    if (Result != null && (Result.nvchaliasname == "Admin" || Result.HRMSSTATUS == 1)/*|| Configuration.GetValue<int>("MySettings:LoginType") == 2)*/)
                    {
                        if (Result.nvchdesigname == "BDO")
                        {

                            var NotificationCount = _loginRepository.CountNotification(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                            HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                        }
                        else if (Result.nvchdesigname == "COLLECTOR")
                        {

                            var NotificationCount = _loginRepository.CountNotificationCol(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                            HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                        }
                        else if (Result.nvchdesigname == "Planning Commissioner")
                        {

                            var NotificationCount = _loginRepository.CountNotificationDept().Result.FirstOrDefault();
                            HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                        }
                        Log.Information("Login alias by uid: " + Result.nvchaliasname);
                        var Login = _loginRepository.GetLoginDetails(Model).Result.FirstOrDefault();
                        if (Login != null)
                        {
                            var result1 = _loginRepository.InsertLoginlogDetails(Login.INTUSERID).Result.FirstOrDefault();
                            HttpContext.Session.SetInt32("_logid", Convert.ToInt32(result1.INTUSERID));
                            Log.Information("Login alias by uid and Pass: " + Result.vchfullname);
                            //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
                            SetSession(Login);
                            if (!string.IsNullOrWhiteSpace(Result.MOBILENO))
                            {
                                HttpContext.Session.SetString("_MobileNo", Result.MOBILENO);
                            }
                            if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 5)
                            {
                                return RedirectToAction("BlockDashboard", "Dashboard");
                            }
                            else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 4)
                            {
                                return RedirectToAction("DistrictDashboard", "Dashboard");
                            }
                            else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 6)
                            {
                                return RedirectToAction("DepartmentDashboard", "Dashboard");
                            }
                            else
                            {
                                return RedirectToAction("DepartmentDashboard", "Dashboard");
                            }
                        }
                        else
                        {

                            Model.Message = "Internal Server Error Something Went Wrong";
                            ViewBag.msg = Model.Message;
                            return View(Model);
                        }
                    }
                    #region hrms login
                    else
                    {
                        Log.Information("came to else for HRMS");
                        //converting model into  json string
                        var jstring = JsonConvert.SerializeObject(new LoginClass { username = Model.VCHUSERNAME, userpassword = Model.vchpassword });
                        ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                        //converting json into  base64 encoded string
                        string enstr = Base64Encode(jstring);
                        //-------------Api integration HRMS----------------------------
                        using (var httpClient = new HttpClient())
                        {
                            string url = Configuration.GetValue<string>("Apiurl1:url1");
                            httpClient.BaseAddress = new Uri(url);
                            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                            httpClient.DefaultRequestHeaders.Accept.Clear();
                            LoginResponse lrlist = new LoginResponse
                            {
                                usercredentials = enstr,
                            };
                            HttpResponseMessage response = await httpClient.PostAsJsonAsync<LoginResponse>(url + "?usercredentials=" + enstr, lrlist);
                            var res = await response.Content.ReadAsAsync<LoginResponse>();
                            if (res.linkid != null)
                            {
                                using (var Client = new HttpClient())
                                {
                                    CallbackResponse cblist = new CallbackResponse
                                    {
                                        linkid = Base64Encode(res.linkid),
                                        securitykey = res.securitykey
                                    };
                                    string url2 = Configuration.GetValue<string>("Apiurl1:url2");
                                    Client.BaseAddress = new Uri(url2);
                                    Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                                    Client.DefaultRequestHeaders.Accept.Clear();
                                    HttpResponseMessage response2 = await Client.PostAsJsonAsync<CallbackResponse>(url2, cblist);
                                    var calres = await response2.Content.ReadAsAsync<CallbackResponse>();
                                    if (calres.responseCode == 0)
                                    {
                                        string res2 = (Base64Decode(calres.responseData));

                                        //var res3 = JObject.Parse(res2);
                                        var rd = JsonConvert.DeserializeObject(res2);
                                        dynamic data = JObject.Parse(res2);
                                        Login spc = new Login();
                                        spc.vchspcid = data.curspc;
                                        spc.MOBILENO = data.MOBILENO;
                                        var Result1 = _loginRepository.GetLoginDetailsByUserId(spc).Result.FirstOrDefault();
                                        if (Result1 != null)
                                        {
                                            var result2 = _loginRepository.InsertLoginlogDetails(Result1.INTUSERID).Result.FirstOrDefault();
                                            HttpContext.Session.SetInt32("_logid", Convert.ToInt32(result2.INTUSERID));
                                            Result1.MOBILENO = data.mobileNumber;
                                            SetSession(Result1);
                                            if (!string.IsNullOrWhiteSpace(Result1.MOBILENO))
                                            {
                                                HttpContext.Session.SetString("_MobileNo", Result1.MOBILENO);
                                            }
                                            if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result1.nvchaliasname + "")) == 5)
                                            {
                                                return RedirectToAction("BlockDashboard", "Dashboard");
                                            }
                                            else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result1.nvchaliasname + "")) == 4)
                                            {
                                                return RedirectToAction("DistrictDashboard", "Dashboard");
                                            }
                                            else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 6)
                                            {
                                                return RedirectToAction("DepartmentDashboard", "Dashboard");
                                            }
                                            else
                                            {
                                                return RedirectToAction("DepartmentDashboard", "Dashboard");
                                            }
                                        }
                                        else
                                        {
                                            Model.Message = "Internal Server Error Something Went Wrong";
                                            ViewBag.msg = Model.Message;
                                            return View(Model);
                                        }
                                    }
                                    else
                                    {
                                        Model.Message = calres.responseMsg;
                                        ViewBag.msg = Model.Message;
                                        return View(Model);
                                    }

                                }
                            }
                            else
                            {
                                Model.Message = "Internal Server Error Something Went Wrong";
                                ViewBag.msg = Model.Message;
                                return View(Model);
                            }
                        }
                    }
                    #endregion
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Model.Message = "Something went wrong. Plese try again";
                return View(Model);
                //throw ex;

            }
        }

        [HttpGet]
        public IActionResult NewLogin()
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            ViewBag.district = _districtRepository.GetDistrictAsync().Result;

            ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
            ViewBag.msg = null;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewLogin(Login Model)
        {
            ViewBag.district = _districtRepository.GetDistrictAsync().Result;
            Log.Information("Login Initialized!");
            try
            {

                Model.VCHUSERNAME = AESEncrytDecry.DecryptStringAES(Model.VCHUSERNAME);
                Model.vchpassword = AESEncrytDecry.DecryptStringAES(Model.vchpassword);
                if (Model.VCHUSERNAME == null)
                {
                    Model.Message = "Please Select Block!";
                    return View(Model);
                }
                else if (Model.vchpassword == null)
                {
                    Model.Message = "Please Enter Password!";
                    return View(Model);
                }
                else
                {
                    var Result = _loginRepository.GetLoginDetails(Model).Result.FirstOrDefault();
                    if (Result != null)
                    {
                        // Result.nvchdesigname
                        if (Result.nvchdesigname == "BDO")
                        {

                            var NotificationCount = _loginRepository.CountNotification(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                            HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                        }
                        else if (Result.nvchdesigname == "COLLECTOR")
                        {

                            var NotificationCount = _loginRepository.CountNotificationCol(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                            HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                        }
                        else if (Result.nvchdesigname == "Planning Commissioner")
                        {

                            var NotificationCount = _loginRepository.CountNotificationDept().Result.FirstOrDefault();
                            HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                        }
                        //************Notification Count***********//
                        var result1 = _loginRepository.InsertLoginlogDetails(Result.INTUSERID).Result.FirstOrDefault();

                        HttpContext.Session.SetInt32("_logid", Convert.ToInt32(result1.INTUSERID));

                        Log.Information("Login alias by uid and Pass: " + Result.vchfullname);
                        //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
                        SetSession(Result);
                        if (!string.IsNullOrWhiteSpace(Result.MOBILENO))
                        {
                            HttpContext.Session.SetString("_MobileNo", Result.MOBILENO);
                        }
                        if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 5)
                        {
                            return RedirectToAction("BlockDashboard", "Dashboard");
                        }
                        else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 4)
                        {
                            return RedirectToAction("DistrictDashboard", "Dashboard");
                        }
                        else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 6)
                        {
                            return RedirectToAction("DepartmentDashboard", "Dashboard");
                        }
                        else
                        {
                            return RedirectToAction("DepartmentDashboard", "Dashboard");
                        }
                    }
                    else
                    {
                        ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                        Model.Message = "Internal Server Error Something Went Wrong";
                        ViewBag.msg = Model.Message;
                        return View(Model);
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Model.Message = "Something went wrong. Plese try again";
                ViewBag.msg = Model.Message;
                return View(Model);
                //throw ex;

            }
        }
        public IActionResult MasterLogin()
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            ViewBag.district = _districtRepository.GetDistrictAsync().Result;

            ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
            ViewBag.msg = null;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterLogin(Login Model)

        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.district = _districtRepository.GetDistrictAsync().Result;
                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                Log.Information("Login Initialized!");
                try
                {
                    if (Model.ROLEID == 0)
                    {
                        Model.Message = "Please Select User Type!";
                        ViewBag.msg = Model.Message;
                        ViewBag.Roleid = 0;
                        return View(Model);
                    }

                    else if (Model.ROLEID == 1)
                    {
                        if (Model.nvchlevelname == "0")
                        {
                            Model.Message = "Please Select Department!";
                            ViewBag.msg = Model.Message;
                            return View(Model);
                        }
                        else if (Model.VCHUSERNAME == null || Model.VCHUSERNAME == "0")
                        {
                            Model.Message = "Please Select Designation!";
                            ViewBag.msg = Model.Message;
                            ViewBag.Roleid = 1;
                            return View(Model);
                        }
                        else
                        {
                            var Result = _loginRepository.GetMLoginDetails(Model).Result.FirstOrDefault();
                            if (Result != null)
                            {
                                // Result.nvchdesigname
                                if (Result.nvchdesigname == "BDO")
                                {

                                    var NotificationCount = _loginRepository.CountNotification(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                else if (Result.nvchdesigname == "COLLECTOR")
                                {

                                    var NotificationCount = _loginRepository.CountNotificationCol(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                else if (Result.nvchdesigname == "Planning Commissioner")
                                {

                                    var NotificationCount = _loginRepository.CountNotificationDept().Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                //************Notification Count***********//
                                var result1 = _loginRepository.InsertLoginlogDetails(Result.INTUSERID).Result.FirstOrDefault();

                                HttpContext.Session.SetInt32("_logid", Convert.ToInt32(result1.INTUSERID));

                                Log.Information("Login alias by uid and Pass: " + Result.vchfullname);
                                //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
                                SetSession(Result);
                                if (!string.IsNullOrWhiteSpace(Result.MOBILENO))
                                {
                                    HttpContext.Session.SetString("_MobileNo", Result.MOBILENO);
                                }
                                if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 5)
                                {
                                    return RedirectToAction("BlockDashboard", "Dashboard");
                                }
                                else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 4)
                                {
                                    return RedirectToAction("DistrictDashboard", "Dashboard");
                                }
                                else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 6)
                                {
                                    return RedirectToAction("DepartmentDashboard", "Dashboard");
                                }
                                else
                                {
                                    return RedirectToAction("DepartmentDashboard", "Dashboard");
                                }
                            }
                            else
                            {
                                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                                // Model.Message = "Internal Server Error Something Went Wrong";
                                Model.Message = "Please Select Mandatory Field(s)";
                                ViewBag.msg = Model.Message;
                                return View(Model);
                            }
                        }
                    }
                    else if (Model.ROLEID == 2)
                    {
                        if (Model.dist == "0")
                        {
                            Model.Message = "Please Select District!";
                            ViewBag.msg = Model.Message;
                            return View(Model);
                        }
                        else if (Model.VCHUSERNAME == null || Model.VCHUSERNAME == "0")
                        {
                            Model.Message = "Please Select Block!";
                            ViewBag.msg = Model.Message;
                            ViewBag.Roleid = 2;
                            return View(Model);
                        }
                        else
                        {
                            var Result = _loginRepository.GetMLoginDetails(Model).Result.FirstOrDefault();
                            if (Result != null)
                            {
                                // Result.nvchdesigname
                                if (Result.nvchdesigname == "BDO")
                                {

                                    var NotificationCount = _loginRepository.CountNotification(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                else if (Result.nvchdesigname == "COLLECTOR")
                                {

                                    var NotificationCount = _loginRepository.CountNotificationCol(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                else if (Result.nvchdesigname == "Planning Commissioner")
                                {

                                    var NotificationCount = _loginRepository.CountNotificationDept().Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                //************Notification Count***********//
                                var result1 = _loginRepository.InsertLoginlogDetails(Result.INTUSERID).Result.FirstOrDefault();

                                HttpContext.Session.SetInt32("_logid", Convert.ToInt32(result1.INTUSERID));

                                Log.Information("Login alias by uid and Pass: " + Result.vchfullname);
                                //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
                                SetSession(Result);
                                if (!string.IsNullOrWhiteSpace(Result.MOBILENO))
                                {
                                    HttpContext.Session.SetString("_MobileNo", Result.MOBILENO);
                                }
                                if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 5)
                                {
                                    return RedirectToAction("BlockDashboard", "Dashboard");
                                }
                                else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 4)
                                {
                                    return RedirectToAction("DistrictDashboard", "Dashboard");
                                }
                                else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 6)
                                {
                                    return RedirectToAction("DepartmentDashboard", "Dashboard");
                                }
                                else
                                {
                                    return RedirectToAction("DepartmentDashboard", "Dashboard");
                                }
                            }
                            else
                            {
                                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                                // Model.Message = "Internal Server Error Something Went Wrong";
                                Model.Message = "Please Select Mandatory Field(s)";
                                ViewBag.msg = Model.Message;
                                return View(Model);
                            }
                        }
                    }

                    else if (Model.ROLEID == 3)
                    {
                        if (Model.VCHUSERNAME == null || Model.VCHUSERNAME == "0")
                        {
                            Model.Message = "Please Select District!";
                            ViewBag.msg = Model.Message;
                            ViewBag.Roleid = 3;
                            return View(Model);
                        }
                        else
                        {
                            var Result = _loginRepository.GetMLoginDetails(Model).Result.FirstOrDefault();
                            if (Result != null)
                            {
                                // Result.nvchdesigname
                                if (Result.nvchdesigname == "BDO")
                                {

                                    var NotificationCount = _loginRepository.CountNotification(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                else if (Result.nvchdesigname == "COLLECTOR")
                                {

                                    var NotificationCount = _loginRepository.CountNotificationCol(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                else if (Result.nvchdesigname == "Planning Commissioner")
                                {

                                    var NotificationCount = _loginRepository.CountNotificationDept().Result.FirstOrDefault();
                                    HttpContext.Session.SetString("_Notifcount", Convert.ToString(NotificationCount.NONZERODPCOUNT));
                                }
                                //************Notification Count***********//
                                var result1 = _loginRepository.InsertLoginlogDetails(Result.INTUSERID).Result.FirstOrDefault();

                                HttpContext.Session.SetInt32("_logid", Convert.ToInt32(result1.INTUSERID));

                                Log.Information("Login alias by uid and Pass: " + Result.vchfullname);
                                //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
                                SetSession(Result);
                                if (!string.IsNullOrWhiteSpace(Result.MOBILENO))
                                {
                                    HttpContext.Session.SetString("_MobileNo", Result.MOBILENO);
                                }
                                if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 5)
                                {
                                    return RedirectToAction("BlockDashboard", "Dashboard");
                                }
                                else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 4)
                                {
                                    return RedirectToAction("DistrictDashboard", "Dashboard");
                                }
                                else if (Convert.ToInt32(Configuration.GetValue<string>("Roles:" + Result.nvchaliasname + "")) == 6)
                                {
                                    return RedirectToAction("DepartmentDashboard", "Dashboard");
                                }
                                else
                                {
                                    return RedirectToAction("DepartmentDashboard", "Dashboard");
                                }
                            }
                            else
                            {
                                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                                // Model.Message = "Internal Server Error Something Went Wrong";
                                Model.Message = "Please Select Mandatory Field(s)";
                                ViewBag.msg = Model.Message;
                                return View(Model);
                            }
                        }
                        
                    }
                    

                    //Model.VCHUSERNAME = AESEncrytDecry.DecryptStringAES(Model.VCHUSERNAME);
                    //Model.vchpassword = AESEncrytDecry.DecryptStringAES(Model.vchpassword);



                    //else if (Model.vchpassword == null)
                    //{
                    //    Model.Message = "Please Enter Password!";
                    //    return View(Model);
                    //}

                }

                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                    Model.Message = "Something went wrong. Plese try again";
                    ViewBag.msg = Model.Message;
                    return View(Model);
                    //throw ex;

                }
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
            return View();
        }

        [HttpGet]
        public JsonResult Blocklist(int did)
        {
            var Blocklist = _districtRepository.GetUserAndBlockByDist(did).Result;
            return Json(Blocklist);

        }
        [HttpGet]
        public JsonResult Desglist(int deptid)
        {
            var Desglist = _districtRepository.GetUserAndDesgByDept(deptid).Result;
            return Json(Desglist);

        }
        [HttpGet]
        public JsonResult DistlistColwise(int did)
        {
            var Distlist = _districtRepository.GetUserAndColByDist(did).Result;
            return Json(Distlist);

        }
        public IActionResult HRMSLogin()
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HRMSLogin(Login Model)
        {
            Log.Information("Login Initialized!");
            try
            {
                Model.VCHUSERNAME = AESEncrytDecry.DecryptStringAES(Model.VCHUSERNAME);
                Model.vchpassword = AESEncrytDecry.DecryptStringAES(Model.vchpassword);
                if (Model.VCHUSERNAME == null)
                {
                    Model.Message = "Please enter user name.";
                    return View(Model);
                }
                else if (Model.vchpassword == null)
                {
                    Model.Message = "Please enter password.";
                    return View(Model);
                }
                else
                {
                    var Result = _loginRepository.GetLoginDetailsByUserId(Model).Result.FirstOrDefault();
                    Log.Information("came to else for HRMS");
                    //converting model into  json string
                    var jstring = JsonConvert.SerializeObject(new LoginClass { username = Model.VCHUSERNAME, userpassword = Model.vchpassword });
                    //converting json into  base64 encoded string
                    string enstr = Base64Encode(jstring);
                    //-------------Api integration HRMS----------------------------
                    using (var httpClient = new HttpClient())
                    {
                        string url = Configuration.GetValue<string>("Apiurl1:url1");
                        httpClient.BaseAddress = new Uri(url);
                        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        LoginResponse lrlist = new LoginResponse
                        {
                            usercredentials = enstr,
                        };
                        //************Notification Count***********//
                        // Result.nvchdesigname
                        if (Result.nvchdesigname == "BDO")
                        {

                            var NotificationCount = _loginRepository.CountNotification(Convert.ToInt32(Result.INTLEVELDETAILID)).Result.FirstOrDefault();
                        }
                        else if (Result.nvchdesigname == "COLLECTOR")
                        {

                        }
                        else if (Result.nvchdesigname == "Planning Commissioner")
                        {

                        }
                        //************Notification Count***********//
                        HttpResponseMessage response = await httpClient.PostAsJsonAsync<LoginResponse>(url + "?usercredentials=" + enstr, lrlist);
                        var res = await response.Content.ReadAsAsync<LoginResponse>();
                        if (res.linkid != null)
                        {
                            using (var Client = new HttpClient())
                            {
                                CallbackResponse cblist = new CallbackResponse
                                {
                                    linkid = Base64Encode(res.linkid),
                                    securitykey = res.securitykey
                                };
                                string url2 = Configuration.GetValue<string>("Apiurl1:url2");
                                Client.BaseAddress = new Uri(url2);
                                Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                                Client.DefaultRequestHeaders.Accept.Clear();

                                HttpResponseMessage response2 = await Client.PostAsJsonAsync<CallbackResponse>(url2, cblist);
                                var calres = await response2.Content.ReadAsAsync<CallbackResponse>();
                                if (calres.responseCode == 0)
                                {
                                    string res2 = (Base64Decode(calres.responseData));

                                    //var res3 = JObject.Parse(res2);
                                    var rd = JsonConvert.DeserializeObject(res2);
                                    dynamic data = JObject.Parse(res2);
                                    string spcid = data.curspc;
                                    string mobno = data.mobileNumber;
                                    if (Result.vchspcid == spcid)
                                    {
                                        //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
                                        HttpContext.Session.SetInt32("_UserId", Convert.ToInt32(Result.INTUSERID));
                                        HttpContext.Session.SetInt32("_RoleId", Convert.ToInt32(Result.ROLEID));
                                        HttpContext.Session.SetInt32("_DeptId", Convert.ToInt32(Result.INTLEVELDETAILID));
                                        HttpContext.Session.SetInt32("_DesignId", Convert.ToInt32(Result.intdesigid));
                                        HttpContext.Session.SetInt32("_LeveDetailId", Convert.ToInt32(Result.INTLEVELDETAILID));
                                        HttpContext.Session.SetString("_UserName", Result.vchfullname);

                                        HttpContext.Session.SetString("_DesigName", Result.nvchdesigname);
                                        HttpContext.Session.SetString("_DeptName", Result.nvchlevelname);
                                        //HttpContext.Session.SetString("_Aurl", Aurl);
                                        HttpContext.Session.SetString("_EncryptUserName", CommonFunction.EncryptData(Result.VCHUSERNAME));
                                        HttpContext.Session.SetString("_MobileNo", mobno);
                                        HttpContext.Session.SetInt32("_HRMSSTATUS", Result.HRMSSTATUS);
                                        return RedirectToAction("ChangePasswordReq", "Login");
                                    }
                                    else
                                    {
                                        Model.Message = "Internal Server Error Something Went Wrong";
                                        return View(Model);
                                    }
                                }
                                else
                                {
                                    Model.Message = calres.responseMsg;
                                    return View(Model);
                                }

                            }
                        }
                        else
                        {

                            Model.Message = "Internal Server Error Something Went Wrong";
                            return View(Model);
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Model.Message = "Something went wrong. Plese try again";
                return View(Model);
                //throw ex;

            }
        }
        public void SetSession(Login model)
        {
            //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
            HttpContext.Session.SetInt32("_UserId", Convert.ToInt32(model.INTUSERID));
            HttpContext.Session.SetInt32("_RoleId", Convert.ToInt32(model.ROLEID));
            HttpContext.Session.SetInt32("_DeptId", Convert.ToInt32(model.INTLEVELDETAILID));
            HttpContext.Session.SetInt32("_DesignId", Convert.ToInt32(model.intdesigid));
            HttpContext.Session.SetInt32("_LeveDetailId", Convert.ToInt32(model.INTLEVELDETAILID));
            HttpContext.Session.SetString("_nvchaliasname", model.nvchaliasname);
            HttpContext.Session.SetString("_UserName", model.vchfullname);
            HttpContext.Session.SetString("_DesigName", model.nvchdesigname);
            HttpContext.Session.SetString("_DeptName", model.nvchlevelname);
            //HttpContext.Session.SetString("_Aurl", Aurl);
            HttpContext.Session.SetString("_EncryptUserName", CommonFunction.EncryptData(model.VCHUSERNAME));
            HttpContext.Session.SetString("_MobileNo", model.MOBILENO);
            HttpContext.Session.SetInt32("_HRMSSTATUS", model.HRMSSTATUS);
            HttpContext.Session.SetInt32("_PARENTID", model.INTPARENTID);
            HttpContext.Session.SetInt32("CODE", model.CODE);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        [HttpGet]
        public IActionResult TestApi()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> TestApi(Login Model)
        {
            string linkid = "";
            string securityKey = "";
            if (Model.ROLEID == 1)
            {
                var jstring = JsonConvert.SerializeObject(new LoginClass { username = Model.VCHUSERNAME, userpassword = Model.vchpassword });
                //converting json into  base64 encoded string
                string enstr = Base64Encode(jstring);
                string url = Configuration.GetValue<string>("Apiurl1:url1");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 300000;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"usercredentials\":\"" + enstr + "\"}";


                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<LoginResponse>(result);

                    linkid = (Base64Encode(model.linkid));
                    securityKey = model.securitykey;


                    //var res3 = JObject.Parse(res2);
                    var rd = JsonConvert.SerializeObject(model);
                    ViewBag.Callback2 = rd;
                    return View();
                }
            }
            else
            {
                var jstring = JsonConvert.SerializeObject(new LoginClass { username = Model.VCHUSERNAME, userpassword = Model.vchpassword });
                //converting json into  base64 encoded string
                string enstr = Base64Encode(jstring);
                string url = Configuration.GetValue<string>("Apiurl1:url1");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 300000;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"usercredentials\":\"" + enstr + "\"}";


                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<LoginResponse>(result);

                    linkid = (Base64Encode(model.linkid));
                    securityKey = model.securitykey;
                }

                string url1 = Configuration.GetValue<string>("Apiurl1:url2");
                var httpWebRequest1 = (HttpWebRequest)WebRequest.Create(url1);
                httpWebRequest1.ContentType = "application/json";
                httpWebRequest1.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest1.GetRequestStream()))
                {
                    string json = "{\"linkid\":\"" + linkid + "\"," +
                                  "\"securitykey\":\"" + securityKey + "\"}";


                    // - { "linkid":"NDMwMDMzNjM=","securitykey": "f219fe9e91c96a5611a1b3154a1d33f9"}


                    streamWriter.Write(json);
                }

                var httpResponse1 = (HttpWebResponse)httpWebRequest1.GetResponse();
                using (var streamReader = new StreamReader(httpResponse1.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CallbackResponse>(result);

                    string res2 = (Base64Decode(model.responseData));

                    //var res3 = JObject.Parse(res2);
                    var rd = JsonConvert.DeserializeObject(res2);
                    dynamic data = JObject.Parse(res2);
                    ViewBag.Callback2 = rd;
                    return View();
                    //ViewBag.Callback2 = result;
                    //return View(Model);
                }
            }




            //-------------Api integration HRMS----------------------------
            //using (var httpClient = new HttpClient())
            //{
            //    string url = Configuration.GetValue<string>("Apiurl1:url1");
            //    httpClient.BaseAddress = new Uri(url);
            //    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //    httpClient.DefaultRequestHeaders.Accept.Clear();
            //    LoginResponse lrlist = new LoginResponse
            //    {
            //        usercredentials = enstr,
            //    };
            //    HttpResponseMessage response = await httpClient.PostAsJsonAsync<LoginResponse>(url + "?usercredentials=" + enstr, lrlist);
            //    var res = await response.Content.ReadAsAsync<LoginResponse>();
            //    ViewBag.Callback1 = res;
            //    if (res.linkid != null)
            //    {
            //        using (var Client = new HttpClient())
            //        {
            //            CallbackResponse cblist = new CallbackResponse
            //            {
            //                linkid = Base64Encode(res.linkid),
            //                securitykey = res.securitykey
            //            };
            //            string url2 = Configuration.GetValue<string>("Apiurl1:url2");
            //            Client.BaseAddress = new Uri(url2);
            //            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //            Client.DefaultRequestHeaders.Accept.Clear();
            //            HttpResponseMessage response2 = await Client.PostAsJsonAsync<CallbackResponse>(url2, cblist);
            //            var calres = await response2.Content.ReadAsAsync<CallbackResponse>();
            //            if (calres.responseCode == 0)
            //            {
            //                string res2 = (Base64Decode(calres.responseData));

            //                //var res3 = JObject.Parse(res2);
            //                var rd = JsonConvert.DeserializeObject(res2);
            //                dynamic data = JObject.Parse(res2);
            //                ViewBag.Callback2 = rd;
            //                return View();
            //            }
            //            else
            //            {
            //                ViewBag.Callback2 = calres.responseMsg;
            //                return View(Model);
            //            }


            //        }
            //    }
            //    else
            //    {
            //        return View();
            //    }
            //}
        }

        //[HttpGet]
        //public IActionResult ChangePasswordReq(/*string Hid*/)
        //{
        //    //string username = CommonFunction.DecryptData(Hid);
        //    var username = HttpContext.Session.GetString("_EncryptUserName");
        //    var username1 = CommonFunction.DecryptData(username);
        //    ViewBag.Username = username1;
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult ChangePasswordReq(string UserName, string Password)
        //{
        //    var password = AESEncrytDecry.DecryptStringAES(Password);
        //    var username = HttpContext.Session.GetString("_EncryptUserName");
        //    var username1 = CommonFunction.DecryptData(username);
        //    if (UserName == username1)
        //    {
        //        var updatepass = _loginRepository.ForgetPasswordNew(forgetPassword);
        //        HttpContext.Session.SetInt32("_HRMSSTATUS", 1);
        //        return Json(new { data = updatepass });
        //    }
        //    else
        //    {
        //        return Json(new { data = "1" });
        //    }

        //}

        public IActionResult SMSjobScheduler(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            //BackgroundJob.Enqueue(() => TestSMS1());
            //recurringJobManager.AddOrUpdate("Insert in Indicator Score Table Yearly", () => TestSMS1(), Cron.Monthly(22, 17, 33), TimeZoneInfo.Local);
            //RecurringJob.AddOrUpdate(() => TestSMS1("Recurring Job", DateTime.Now.ToLongTimeString(), Cron.Daily(16, 33), TimeZoneInfo.Local);
            //recurringJobManager.AddOrUpdate("Insert in Indicator Score Table Yearly", () => jobscheduler.JobIndScoreCalculationYearly(), Cron.Weekly(DayOfWeek.Wednesday, 16, 33), TimeZoneInfo.Local);
            // _backgroundJobClient.Enqueue(() => GetAge("Manual Scheduler", DateTime.Now.ToLongTimeString(), userName, Year));
            return Ok();
        }

        public IActionResult SendOtp(string MobileNumber, string otp)
        {
            try
            {
                string smsMessage = "Dear User,Your One-Time Password (OTP) is "+otp+" for updation of your password.";
                Log.Information("Sending OTP SMS");
                var smsResult = _sms.Sendsms(MobileNumber, smsMessage, "1007001830520872724");

                return Ok(smsResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
        }
        #region New Forget Password

        private IActionResult InvalidPhoneNumberView()
        {
            var invalidUserModel = new ForgetPassword { Message = "Invalid Phone Number" };
            return View(invalidUserModel);
        }

        private IActionResult FinalSuccessView(ForgetPassword forgetPassword)
        {
            HttpContext.Session.SetInt32("_HRMSSTATUS", 1);
            ViewBag.ShowOtpSection = true;
            return View(forgetPassword);
        }

        private void SetupSessionValues(ForgetPassword forgetPassword)
        {
            string otp = GenerateRandomOtp();
            string token = GenerateRandomToken();

            forgetPassword.OTP = otp;
            forgetPassword.Token = token;

            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetString("Username", forgetPassword.UserName);
            HttpContext.Session.SetString("MobileNumber", forgetPassword.MobileNumber);
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();

        }

        public async Task<IActionResult> ForgetPassword(ForgetPassword forgetPassword)
        {
            try
            {
                if (!ValidateCaptcha(forgetPassword.strCaptcha))
                {
                    ModelState.AddModelError("Captcha", "Captcha validation failed.");
                    return View(forgetPassword);
                }

                if (string.IsNullOrEmpty(forgetPassword.UserName) && string.IsNullOrEmpty(forgetPassword.MobileNumber))
                {
                    return InvalidPhoneNumberView();
                }

                // Generate OTP
                string otp = GenerateRandomOtp();

                // Pass OTP to SendOtp method
                SendOtp(forgetPassword.MobileNumber, otp);

                SetupSessionValues(forgetPassword);

                var updatepass = await _loginRepository.ForgetPasswordNew(forgetPassword);

                if (updatepass != null && updatepass.Message != null)
                {
                    return Json(new { success = false, message = "Invalid Phone Number" });
                }
                else
                {
                    return FinalSuccessView(forgetPassword);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ForgetPassword action.");
                return View("Error");
            }
        }


        private bool ValidateCaptcha(string captchaCode)
        {
            return GenerateCaptcha.ValidateCaptchaCode(captchaCode, HttpContext);
        }
        private string GenerateRandomOtp()
        {

            Random random = new Random();
            return random.Next(1000, 9999).ToString();
        }
        private string GenerateRandomToken()
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] token = new char[40];

            for (int i = 0; i < token.Length; i++)
            {
                token[i] = characters[random.Next(characters.Length)];
            }

            return new string(token);
        }

        [Route("get-captcha-image")]
        public FileStreamResult GetImage()
        {
            try
            {
                int width = 90;
                int height = 25;
                var captchaCode = GenerateCaptcha.Captcha();
                var result = GenerateCaptcha.GenerateCaptchaImage(width, height, captchaCode);
                HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
                Stream s = new MemoryStream(result.CaptchaByteData);
                return new FileStreamResult(s, "image/GIF");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        [HttpPost]
        [Route("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(string Username, string OTP, string PhoneNumber)
        {
            var storedUsername = HttpContext.Session.GetString("Username");
            var storedPhone = HttpContext.Session.GetString("MobileNumber");
            var tokenz = HttpContext.Session.GetString("Token");
            var data = await _loginRepository.GetOTP(Username, PhoneNumber);

            if (data == OTP)
            {
                 _loginRepository.UpdateOTPStatus(Username, OTP, storedPhone);
                return Json(new { success = true, username = storedUsername });

            }
            else
            {
                return Json(new { success = false, message = "Invalid OTP" });
            }
        }
        public IActionResult ChangeOtpPasswordReq()
        {
           
         return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeOtpPasswordReq(ResetPassword resetPassword)
        {
           
            try
            {
                resetPassword.NewPassword = AESEncrytDecry.DecryptStringAES(resetPassword.NewPassword);
                resetPassword.ConfirmPassword = AESEncrytDecry.DecryptStringAES(resetPassword.ConfirmPassword);
                if (resetPassword.NewPassword != resetPassword.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "New password and confirm password do not match.");
                    return View();
                }
                var tokens = HttpContext.Session.GetString("Token");
                if (tokens != null)
                {
                    var storedUsername = HttpContext.Session.GetString("Username");
                    var storePhone = HttpContext.Session.GetString("MobileNumber");
                    resetPassword.UserName = storedUsername;
                    resetPassword.MobileNumber = storePhone;

                    var result = await _loginRepository.ChangePassword(resetPassword);
                    if (result != null)
                    {

                        return RedirectToAction("Login", "Login");
                    }
                    else
                    {

                        return View();
                    }
                }

                return View();

            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion
    }
}