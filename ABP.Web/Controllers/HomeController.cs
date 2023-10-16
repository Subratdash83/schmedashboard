using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ABP.Web.Models;
using System.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RestSharp;
using Microsoft.Extensions.Configuration;
using ABP.Repository.Contract.Contract.Dashboard;
using Microsoft.AspNetCore.Http;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Domain.DataPoint;
using Newtonsoft.Json;
using ABP.Repository.Contract.Contract.BDOData;
using ABP.Domain.Department;
using Microsoft.AspNetCore.Hosting;
using ABP.Domain.File;
using System.IO;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Domain.BlockData;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Domain.Panel;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Report;
using ABP.Domain.Report;
using ABP.Repository.Contract.Contract.Login;
using System.Text;
using ABP.Domain.BlockMap;
using ABP.Domain.Login;
using ABP.Domain.Indicator;
using ABP.Domain.District;
using Hangfire;

namespace ABP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfiguration Configuration { get; }
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IDataPointRepository _datapointRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IBDODataRepository _bdodataRepository;
        private readonly IBlockDataRepository _blockDataRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IDistrictDataRepository _districtDataRepository;
        private readonly IReportRepository _reportRepository;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IDistrictRepository _DistrictRepository;
        private readonly ILoginRepository _loginRepository;
        public HomeController(ILogger<HomeController> logger, ILoginRepository loginRepository, IDashboardRepository dashboardRepository, IBDODataRepository bdodataRepository, IDataPointRepository datapointRepository, IBlockRepository blockRepository, ISectorRepository sectorRepository, IHostingEnvironment hostingEnvironment, IPanelRepository panelRepository, IBlockDataRepository blockdataRepository, IDistrictDataRepository districtDataRepository, IIndicatorRepository indicatorRepository, IReportRepository reportRepository, IDistrictRepository districtRepository)
        {
            _logger = logger;
            _dashboardRepository = dashboardRepository;
            _blockRepository = blockRepository;
            _sectorRepository = sectorRepository;
            _datapointRepository = datapointRepository;
            _bdodataRepository = bdodataRepository;
            _hostingEnvironment = hostingEnvironment;
            _panelRepository = panelRepository;
            _blockDataRepository = blockdataRepository;
            _districtDataRepository = districtDataRepository;
            _reportRepository = reportRepository;
            _indicatorRepository = indicatorRepository;
            _DistrictRepository = districtRepository;
            _loginRepository = loginRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Dashboard()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = "";
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult Dashboard(Report Rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.DatapointData = _reportRepository.GetDashboardData(Rpt);

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult DistDashboardData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                if (ViewBag.RejectedResult != null)
                {
                    return PartialView("_AlertData");
                }
                else
                {
                    return View("Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        //public IActionResult DistDashboard()
        //{
        //    var UserId = HttpContext.Session.GetInt32("_UserId");
        //    if (!string.IsNullOrEmpty(UserId.ToString()))
        //    {
        //        //int Distid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
        //        //ViewBag.Result = _dashboardRepository.GetDashboardData(Distid, 0);
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Logout", "Home");
        //    }
        //}
        public IActionResult DistDashboard()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.Blockdata = _DistrictRepository.GetBlockByDist(leveldetailid).Result;
                ViewBag.DatapointData = "";

                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DistDashboard(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                Rpt.DistrictId = leveldetailid;
                ViewBag.year = Rpt.Year;
                ViewBag.Blockdata = _DistrictRepository.GetBlockByDist(leveldetailid).Result;
                ViewBag.DatapointData = _reportRepository.GetDashboardData(Rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult DeptDashboard()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                int Deptid = Convert.ToInt32(HttpContext.Session.GetInt32("_DeptId"));
                ViewBag.Sector = _panelRepository.ViewPanel().Result; //Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.yr = 0;
                ViewBag.Result = _bdodataRepository.GetAllDepartmentDashboard((int)Deptid, 0, 0, 0); //getting all data points to show in the grid   
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DeptDashboard(Department department)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                int Deptid = Convert.ToInt32(HttpContext.Session.GetInt32("_DeptId"));
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.yr = department.Year;
                ViewBag.Result = _bdodataRepository.GetAllDepartmentDashboard((int)Deptid, Convert.ToInt32(department.SECTORID), Convert.ToInt32(department.Frequency), Convert.ToInt32(department.Year)); //getting all data points to show in the grid   
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        //[HttpGet]
        //public IActionResult AllDeptDashboard()
        //{
        //    var UserId = HttpContext.Session.GetInt32("_UserId");
        //    if (!string.IsNullOrEmpty(UserId.ToString()))
        //    {

        //        int Deptid = Convert.ToInt32(HttpContext.Session.GetInt32("_DeptId"));

        //        List<DataPoint> BDODataPoints = ViewBag.Result = _bdodataRepository.GetAllDepartmentDashboard((int)Deptid).Result;//getting all data points to show in the grid                
        //        return Ok(JsonConvert.SerializeObject(BDODataPoints));
        //    }
        //    else
        //    {
        //        return RedirectToAction("Logout", "Home");
        //    }
        //}
        public IActionResult BlockDashboard()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;

                data.FREQUENCYID = 5;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 3);
                ViewBag.DatapointData = "";
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult BlockDashboard(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                data.FREQUENCYID = 5;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                int parentid = Convert.ToInt32(HttpContext.Session.GetInt32("_PARENTID"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                Rpt.BlockId = leveldetailid;
                Rpt.DistrictId = parentid;
                ViewBag.DatapointData = _reportRepository.GetDashboardData(Rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult BlockDashboardData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 3);
                if (ViewBag.RejectedResult != null)
                {
                    return PartialView("_AlertData");
                }
                else
                {
                    return View("BlockDashboard");
                }
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult DashboardDetails(int SectorId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Result = _dashboardRepository.GetDashboardDetails(SectorId);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult MonthwiseReport()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                Report rpt = new Report();
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = null;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult MonthwiseReport(Report rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = _reportRepository.GetMonthwiseReportData(rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult AnnualWiseReport()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = "";
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult AnnualWiseReport(Report rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = _reportRepository.GetAnnualWiseReportData(rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult DistrictAnnualWiseReport()
        {
            Report rpt = new Report();
            rpt.DistrictId = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.DatapointData = _reportRepository.GetAnnualWiseReportData(rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult LoginReport()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                Report lr = new Report();
                if (lr.FromDate == "" || lr.FromDate == null)
                {
                    lr.FromDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }

                if (lr.ToDate == "" || lr.ToDate == null)
                {
                    lr.ToDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                ViewBag.Loginreport = _loginRepository.GetLoginLogDetails(lr.FromDate, lr.ToDate);
                ViewBag.FromDate = lr.FromDate;
                ViewBag.TODATE = lr.ToDate;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult LoginReport(Report lr)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                if (lr.FromDate == "" || lr.FromDate == null)
                {
                    lr.FromDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }

                if (lr.ToDate == "" || lr.ToDate == null)
                {
                    lr.ToDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                ViewBag.Loginreport = _loginRepository.GetLoginLogDetails(lr.FromDate, lr.ToDate);
                ViewBag.FromDate = lr.FromDate;
                ViewBag.TODATE = lr.ToDate;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult Logout()
        {
            int logid = Convert.ToInt32(HttpContext.Session.GetInt32("_logid"));
            var res = _loginRepository.UpdateLoginlogDetails(logid);
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }

        public IActionResult ViewAllLogs()
        {

            int UserId = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId"));
            if (UserId != 0)
            {
                //var roleid = HttpContext.Session.GetInt32("_CapsRoleId");
                string XMLFilePath = _hostingEnvironment.WebRootPath + "/logs/";
                List<FIleDetails> FileList = new List<FIleDetails>();
                FIleDetails filenew = null;
                DirectoryInfo d = new DirectoryInfo(XMLFilePath);
                FileInfo[] allfiles = d.GetFiles();
                foreach (FileInfo file in allfiles)
                {
                    filenew = new FIleDetails();
                    filenew.FileName = file.Name;
                    filenew.CreationTime = file.CreationTime.ToString();
                    FileList.Add(filenew);
                }
                ViewBag.FileDetails = FileList;
                return View();
            }
            else
            {
                return RedirectToAction("SessionExpired", "Home");
            }
        }

        [HttpGet]
        public IActionResult Unauthorizedaccess()
        {
            return View();
        }

        public IActionResult ReportDashboard()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = "";
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult ReportDashboard(Report Rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = _reportRepository.GetReportDashboardData(Rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult LogReport()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {

                    int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                    //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                    ViewBag.district = _DistrictRepository.GetDistrictAsync().Result;
                    ViewBag.Districtid = 0;
                    ViewBag.DatapointData = "";
                    //_reportRepository.GetDashboardData("");
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public IActionResult LogReport(Report Rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.blockid = Rpt.BlockId;
                ViewBag.Districtid = Rpt.DistrictId;
                ViewBag.district = _DistrictRepository.GetDistrictAsync().Result;
                List<Report> rpt = ViewBag.DatapointData = _reportRepository.GetLogReport(Rpt).Result;
                rpt.ForEach((c) => c.ChangeCount = (int)_blockDataRepository.GetCountValue(c.PanelName, c.APPLICATIONNO));
                ViewBag.DatapointData = rpt;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public JsonResult Blocklist(int did)
        {
            var Blocklist = _DistrictRepository.GetUserAndBlockByDist(did).Result;
            return Json(Blocklist);

        }
        [HttpGet]
        public IActionResult GetLogDetails(string APPLICATIONNO, int SectorId, string PanelName)
        {
            var value = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                //List<Report> rpt = ViewBag.Controls=_reportRepository.GetControlsByPanelid(Convert.ToInt32(SectorId)).Result;
                ViewBag.LogDetails = _reportRepository.GetLogData(APPLICATIONNO, PanelName, SectorId);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }
        [HttpGet]
        public IActionResult MapData(int Year, int? FreqId = 2, int? FreqNo = 0)
        {
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                Report rpt = new Report();
                if (Year.ToString() == "0")
                {
                    rpt.Year = DateTime.Now.Year;
                }
                else
                {
                    rpt.Year = Year;
                }
                if (FreqNo == 0)
                {
                    rpt.FreqNo = DateTime.Now.Month - 1;
                    if (rpt.FreqNo == 0)
                    {
                        rpt.Year = DateTime.Now.Year - 1;
                        rpt.FreqNo = 12;
                    }
                }
                else
                {
                    rpt.FreqNo = Convert.ToInt32(FreqNo);
                }
                ViewBag.Year1 = rpt.Year;
                ViewBag.Month1 = rpt.FreqNo;
                rpt.action = "MAP";
                ViewBag.tdata = _reportRepository.GetDistrictWiseReport(rpt);
                ViewBag.mapdata = _reportRepository.GetMapDatas(0, rpt.Year, rpt.FreqNo);
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewMapData()
        {
            BlockData data = new BlockData();
            ViewBag.Year1 = DateTime.Now.Year;
            ViewBag.Month1 = DateTime.Now.Month - 1;

            data.FREQUENCYID = 5; 
            ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
            data.FREQUENCYID = 2;
            ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
            return View();
        }


        //public IActionResult GetDistrictData(int id)
        //{
        //    MapData md=_reportRepository.GetMapDatas(id).Result.FirstOrDefault();
        //    return Json(md);
        //}
        public IActionResult GetDistrictSVG(int id)
        {
            Report rpt = new Report();
            rpt.Year = DateTime.Now.Year;
            rpt.FreqNo = DateTime.Now.Month;
            MapData md = _reportRepository.GetMapDatas(id, rpt.Year, rpt.FreqNo).Result.FirstOrDefault();
            var bm = _reportRepository.GetBlockMapData(md.INTDISTRICTID).Result;
            return Json(bm);
        }
        public IActionResult GetDistrictDSVG(int id)
        {
            Report rpt = new Report();
            rpt.Year = DateTime.Now.Year;
            rpt.FreqNo = DateTime.Now.Month;
            MapData md = _reportRepository.GetDistrictDatas(id, rpt.Year, rpt.FreqNo).Result.FirstOrDefault();
            var bm = _reportRepository.GetBlockMapData(md.INTDISTRICTID).Result;
            return Json(bm);
        }
        [HttpGet]
        public IActionResult DistrictMapData(int Year, int? FreqId = 2, int? FreqNo = 0)
        {
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                Report rpt = new Report();
                if (Year.ToString() == "0")
                {
                    rpt.Year = DateTime.Now.Year;
                }
                else
                {
                    rpt.Year = Year;
                }
                if (FreqNo == 0)
                {
                    rpt.FreqNo = DateTime.Now.Month - 1;
                    if (rpt.FreqNo == 0)
                    {
                        rpt.Year = DateTime.Now.Year - 1;
                        rpt.FreqNo = 12;
                    }
                }
                else
                {
                    rpt.FreqNo = Convert.ToInt32(FreqNo);
                }
                rpt.DistrictId= (int)LeveDetailId;
                ViewBag.tdata = _reportRepository.GetBlockByDistrictReport(rpt).Result.ToList();
                ViewBag.Year1 = rpt.Year;
                ViewBag.Month1 = rpt.FreqNo;
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.District = _DistrictRepository.GetBlockByDist(0).Result.ToList().Where(x => x.BLOCK_CODE == LeveDetailId);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult BlockMapData(int id, int freqno, int year)
        {
            MapData md = _reportRepository.GetMapDatas(id, year, freqno).Result.FirstOrDefault();
            var result = _reportRepository.GetBlockMapDatas(md.INTDISTRICTID, year, freqno).Result;
            return Json(result);
        }

        [HttpGet]
        public IActionResult NewBlockMapData(int id, int freqno, int year)
        {
            
            var result = _reportRepository.GetBlockMapDatas(id, year, freqno).Result;
            return Json(result);
        }
        [HttpGet]
        public IActionResult BlockMapDataC(int id, int freqno, int year)
        {
            MapData md = _reportRepository.GetDistrictDatas(id, year, freqno).Result.FirstOrDefault();
            var result = _reportRepository.GetBlockMapDatas(md.INTDISTRICTID, year, freqno).Result;
            return Json(result);
        }
        public IActionResult NewDashboard(int Year, int? FreqId = 2, int? FreqNo = 0)
        {

            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                BlockData data = new BlockData();
                Report rpt = new Report();
                if (Year.ToString() == "0")
                {
                    rpt.Year = DateTime.Now.Year;
                }
                else
                {
                    rpt.Year = Year;
                }
                if (FreqNo == 0)
                {
                    rpt.FreqNo = DateTime.Now.Month - 1;
                }
                else
                {
                    rpt.FreqNo = Convert.ToInt32(FreqNo);
                }
                ViewBag.Year1 = rpt.Year;
                ViewBag.Month1 = rpt.FreqNo;
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;


                rpt.action = "MAP";
                ViewBag.tdata = _reportRepository.GetDistrictWiseReport(rpt);
                ViewBag.mapdata = _reportRepository.GetMapDatas(0, rpt.Year, rpt.FreqNo);
                ViewBag.DatapointData = _reportRepository.GetDashboardData(rpt);
                ViewBag.DatapointDataAnnual = _reportRepository.GetAnnualWiseReportData(rpt);
                ViewBag.BlockDataPerformance = _DistrictRepository.getblockdata(0);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult GetMapDataAjax(int Year, int? FreqNo = 0)
        {
            Report rpt = new Report();
            if (Year.ToString() == "0")
            {
                rpt.Year = DateTime.Now.Year;
            }
            else
            {
                rpt.Year = Year;
            }
            if (FreqNo == 0)
            {
                rpt.FreqNo = DateTime.Now.Month - 1;
            }
            else
            {
                rpt.FreqNo = Convert.ToInt32(FreqNo);
            }
            var data = _reportRepository.GetMapDatas(0, rpt.Year, rpt.FreqNo).Result;
            return Json(data);
        }
        [HttpPost]
        public IActionResult GetDistPerformanceDataAjax(int Year, int? FreqNo = 0)
        {
            Report rpt = new Report();
            if (Year.ToString() == "0")
            {
                rpt.Year = DateTime.Now.Year;
            }
            else
            {
                rpt.Year = Year;
            }
            if (FreqNo == 0)
            {
                rpt.FreqNo = DateTime.Now.Month - 1;
            }
            else
            {
                rpt.FreqNo = Convert.ToInt32(FreqNo);
            }
            rpt.action = "MAPF";
            var data = _reportRepository.GetDistrictWiseReport(rpt).Result;
            return Json(data);
        }
        public IActionResult AnnualWiseReportcpy()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                //ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = "";
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult AnnualWiseReportcpy(Report rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                //ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.DatapointData = _reportRepository.GetAnnualWiseReportData(rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult Dashboardcpy()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);

                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.DatapointData = "";
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult Dashboardcpy(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.DatapointData = _reportRepository.GetDashboardData(Rpt);

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult getmapcolur(int Year, int? FreqId = 2, int? FreqNo = 0)
        {
            Report rpt = new Report();
            if (Year.ToString() == "0")
            {
                rpt.Year = DateTime.Now.Year;
            }
            else
            {
                rpt.Year = Year;
            }
            if (FreqNo == 0)
            {
                rpt.FreqNo = DateTime.Now.Month - 1;
            }
            else
            {
                rpt.FreqNo = Convert.ToInt32(FreqNo);
            }

            var result = _reportRepository.GetMapDatas(0, rpt.Year, rpt.FreqNo).Result;
            return Ok(result);
        }

        [HttpGet]
        public IActionResult TrendGraphDashboard(Report Rpt)
        {
            try
            {
                BlockData data = new BlockData();
                data.FREQUENCYID = 5;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                // ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                return View();
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        public IActionResult TrendGraphDashboardv2(Report Rpt)
        {
            try
            {
                BlockData data = new BlockData();
                data.FREQUENCYID = 5;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                // ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                return View();
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        public IActionResult GetBlockTrendGraphDashboardv2(int Distid, int indicatorid , int sectorid, string Blockid, int Year)
        {
            try
            {
                List<Indicator> indicator = new List<Indicator>();
                if (indicatorid == null || Blockid==null) { return Json(indicator = null); }
                else
                {
                    List<string> slist = Blockid.Split(',').Select(x => Convert.ToString(x)).ToList();
                    foreach (var item in slist)
                    {
                        Indicator ind = new Indicator();
                        ind.BLOCK = _panelRepository.GetBlockNameTrend(Distid, item);
                        ind.IndicatorTrend = _panelRepository.GetIndicatorTrendv2(Distid, indicatorid, sectorid, Convert.ToInt32(item), Year);
                        indicator.Add(ind);
                    }
                    return Json(indicator);

                }

            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
        [HttpGet]
        public IActionResult GetDistrictwiseMapInfo(int Year, int FreqNo)
        {
            try
            {
                var bm = _reportRepository.GetDistrictwiseMapData(Year,FreqNo).Result;//For Map Data
                var edata = _reportRepository.GetEnterednotenteredData(Year,FreqNo).Result;//For Districtwise Data Enter count details
                return Ok(new { BM= bm , EDATA= edata });
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
        [HttpGet]
        public IActionResult GetBlockDataEntryCnt(int Year, int FreqNo,int DistId)
        {
            try
            {
                var mapdata = _reportRepository.GetEnterednotenteredDatadistwise(Year, FreqNo, DistId).Result;//For Districtwise Data Enter count details
                return Json(mapdata);
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        public IActionResult GetIndicatorTrendGraphDashboard(int Distid, int Blockid, int sectorid, string indicatorid, int Year)
        {
            try
            {
                List<Indicator> indicator = new List<Indicator>();
                if (indicatorid == null) { return Json(indicator = null); }
                else
                {
                    List<string> slist = indicatorid.Split(',').Select(x => Convert.ToString(x)).ToList();
                    foreach (var item in slist)
                    {
                        Indicator ind = new Indicator();
                        ind.INDICATORNAME = _panelRepository.GetIndictorNameTrend(sectorid, item);
                        ind.IndicatorTrend = _panelRepository.GetIndicatorTrend(Distid, Blockid, sectorid, Convert.ToInt32(item), Year);
                        indicator.Add(ind);
                    }
                    return Json(indicator);

                }

            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
        [HttpGet]
        public IActionResult GetBlockByDistID(int id)
        {
            var result = _DistrictRepository.GetBlockByDist(id).Result;
            return Json(result);
        }
        // ViewBag.Months = _blockDataRepository.GetFrequencyValuemoth(2).Result.ToList();
        [HttpGet]
        public IActionResult GetMonth(int id)
        {
            var result = _blockDataRepository.GetFrequencyValuemoth(2).Result;
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetIndicatorBySector(int id)
        {
            var result = _panelRepository.GetIndicatorBySector(id).Result;
            return Json(result);
        }
        public IActionResult GetIndicatorBySectorYearly(int id)
        {
            var result = _panelRepository.GetIndicatorBySectorYearly(id).Result;
            return Json(result);
        }

        public IActionResult GetIndicatorBySectorMonthly(int id)
        {
            var result = _panelRepository.GetIndicatorBySectorMonthly(id).Result;
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetDataPointBySector(int id)
        {
            var result = _panelRepository.GetDatapointBySector(id).Result;
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetDatapointTrendGraphDashboard(int Blockid, int sectorid, int indicatorid, int Year, string ControlName)
        {
            try
            {
                List<DataPoint> dps = new List<DataPoint>();
                if (ControlName == null) { return Json(dps = null); }
                else
                {
                    List<string> slist = ControlName.Split(',').Select(x => Convert.ToString(x)).ToList();
                    foreach (var item in slist)
                    {
                        DataPoint dp = new DataPoint();
                        dp.DATAPOINTNAME = _panelRepository.GetDataPointName(sectorid, item);

                        dp.DPTREND = _panelRepository.GetDataPointTrend(Blockid, sectorid, indicatorid, Year, item);
                        dps.Add(dp);
                    }
                    return Json(dps);
                }
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        public IActionResult GetDatapointBlockTrendGraphDashboard(string Blockid, int sectorid, int indicatorid, int Year, string ControlName,int Distid)
        {
            try
            {
                List<DataPoint> dps = new List<DataPoint>();
                if (ControlName == null || Blockid == null) { return Json(dps = null); }
                else
                {
                    List<string> slist = Blockid.Split(',').Select(x => Convert.ToString(x)).ToList();
                    foreach (var item in slist)
                    {
                        DataPoint dp = new DataPoint();
                        dp.BLOCK = _panelRepository.GetBlockNameTrend(Distid, item);

                        dp.DPTREND = _panelRepository.GetDataPointTrendv2(ControlName, sectorid, indicatorid, Year, item);
                        dps.Add(dp);
                    }
                    return Json(dps);
                }
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        public IActionResult IndicatorValueReport()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);

                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.DatapointData = "";
                ViewBag.bid = "0";
                ViewBag.Iid = 0;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult IndicatorValueReport(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                data.FREQUENCYID = 5;               
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                // ViewBag.District = Rpt.DistrictId;
                ViewBag.bid = Rpt.BlockId;
                // ViewBag.District = Rpt.DistrictId;
                ViewBag.Iid = Rpt.IndicatorId;
                ViewBag.year = Rpt.Year;
                ViewBag.DatapointData = _reportRepository.GetIndicatorData(Rpt);

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult IndicatorYearlyValueReport()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);

                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.DatapointData = "";
                ViewBag.Iid = 0;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult IndicatorYearlyValueReport(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);

                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.Iid = Rpt.IndicatorId;
                ViewBag.year = Rpt.Year;
                ViewBag.DatapointData = _reportRepository.GetIndicatorYearlyData(Rpt);

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public ActionResult GetBlockwiseDataEntered()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");

                ViewBag.Result1 = _bdodataRepository.GetCollecterAlertData(Convert.ToInt32(LeveDetailId));
                //ViewBag.Result1 = ResultList;
                // var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                // ViewBag.Blocksdata = blocks;

                return PartialView("_CollectorAlertList");
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public ActionResult GetDistrictBlockData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");

                ViewBag.CollectorData = _bdodataRepository.GetCollecterAlertData(Convert.ToInt32(LeveDetailId)).Result;
                ViewBag.BlocksData = _blockRepository.GetBlockDistBlock(Convert.ToInt32(LeveDetailId));

                return PartialView("_DisplayBlockDataPoint");
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public ActionResult GetBdoDtaAlert()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {

                    var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                    ViewBag.BlocksData = _blockDataRepository.GetBDOAlertDataPoint((int)LeveDetailId);
                    return PartialView("_DisplayBdoDataPoint");


                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        [HttpGet]
        public ActionResult GetDistrictAlertData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                int Districtid = 0;

                ViewBag.CollectorData = _bdodataRepository.GetDeptAlertData(Districtid).Result;
                ViewBag.DistrictData = _districtDataRepository.GetAllDistrict(0);
                //ViewBag.BlocksData = _blockRepository.GetBlockDistBlock(Convert.ToInt32(Districtid));

                return PartialView("_DisplayDeptdataPoint");
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult MonthlyDashboard()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);

                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.DatapointData = "";
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult MonthlyDashboard(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                if (Rpt.Freqid == 2)
                {
                    ViewBag.DatapointDatamonthly = _reportRepository.GetMonthlyDashboardData(Rpt);
                }
                else
                {
                    ViewBag.DatapointDatayearly = _reportRepository.GetYearlyWiseReportData(Rpt);
                }
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult DashboardCopyV2()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);

                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().Where(r => r.FREQUENCYVALUE == "2023");
                //ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();

                ViewBag.Months = _blockDataRepository.GetFrequencyValuemoth(2).Result.ToList();
                ViewBag.DatapointData = "";
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DashboardCopyV2(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().Where(r => r.FREQUENCYVALUE == "2023");
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.DatapointData = _reportRepository.GetMonthlyYearlyDashboardData(Rpt);
                ViewBag.DatapointDatayearly = _reportRepository.GetYearlyWiseReportData(Rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult DashboardCopyV3()
        {
            Report Rpt = new Report();
            Rpt.DisplayName = "";
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
                //***Monthwise datapoint count***//
                ViewBag.Monthwisedp = _DistrictRepository.GetDPMonCount();
                //***Monthwise datapoint count***//
                ViewBag.DatapointData = "";
                ViewBag.bid = Rpt.BlockId;
                ViewBag.year = Rpt.Year;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.DPCOUNTMON = _reportRepository.GetDpcount();
                ViewBag.UserId = leveldetailid;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DashboardCopyV3(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                if (Rpt.DisplayName == null)
                {
                    Rpt.DisplayName = "Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec, ";
                }
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //***Monthwise datapoint count***//
                ViewBag.Monthwisedp = _DistrictRepository.GetDPMonCount();
                //***Monthwise datapoint count***//
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.Dpcount = Rpt.Year;
                ViewBag.bid = Rpt.BlockId;
                
                ViewBag.DPCOUNTMON = _reportRepository.GetDpcount();
                ViewBag.DatapointData = _reportRepository.GetMonthlyYearlyDashboardData2(Rpt);
                ViewBag.DatapointDatayearly = _reportRepository.GetYearlyWiseReportData(Rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }


        public IActionResult ManageFreezeStatus()
        {
           
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                BlockData data = new BlockData();
                string currentYear = DateTime.Now.Year.ToString();
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
                //ViewBag.Frequency1 = _datapointRepository.ViewFrequencyWiseMonth().Result;
                ViewBag.UserId = leveldetailid;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }


        [HttpPost]
        public IActionResult ManageFreezeStatus(BlockData data)
        {
           
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                BlockData data1 = new BlockData();
                string currentYear = DateTime.Now.Year.ToString();
                data1.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data1).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                
                ViewBag.Monthwisedp = _DistrictRepository.GetDPMonCount();
                ViewBag.bid = data.BLOCKID;
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = data.YEAR;
                ViewBag.Frequency1 = _datapointRepository.ViewFrequencyWiseMonth().Result;
                ViewBag.Month = data.MONTH;
                
                ViewBag.StatusRslt = _blockDataRepository.GetMonthlyFreezeDashboardData(data);
               
                
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public JsonResult UpdateFreezeStatus(string ApplicationNo,int Blockid, int Districtid,int Status,int year,int frequencyid, int frequencyno)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");

            string returnmessgae = _blockDataRepository.UpdateFreezeDashboardData(ApplicationNo, Blockid, Districtid, Status,year,frequencyid,frequencyno, Convert.ToInt32(UserId));
            return Json(returnmessgae);

        }


        public IActionResult ViewFreezedetails()
        {
            ViewBag.Result = _blockDataRepository.ViewFreezedetails(0,0);
            ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
            ViewBag.bid = "0";
            return View();
        }

        [HttpPost]
        public IActionResult ViewFreezedetails(BlockData data)
        {
            ViewBag.Result = _blockDataRepository.ViewFreezedetails(data.DISTRICTID,data.BLOCKID);
            ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
            ViewBag.bid = data.BLOCKID;
            return View();
        }

        public IActionResult DashboardAbstract(int? DISTRICTID, int? year)
        {

            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            string currentYear = DateTime.Now.Year.ToString();
            ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
            data.FREQUENCYID = 5;
            ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
            if (year != null)
            {
                AbstractData Rpt = new AbstractData();
                Rpt.DistrictId = Convert.ToInt32(DISTRICTID);
                Rpt.Year = Convert.ToInt32(year);
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.DatapointData = _reportRepository.GetDashboarAbstractdData(Rpt);
                ViewBag.DistrictId = Rpt.DistrictId;
                ViewBag.year = Rpt.Year;
                ViewBag.Aspirationblock = Rpt.Aspirationblock;
                ViewBag.month = DateTime.Now.Month - 1;
            }
            return View();
        }
        [HttpPost]
        public IActionResult DashboardAbstract(AbstractData Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.month = DateTime.Now.Month - 1;
                ViewBag.Aspirationblock = Rpt.Aspirationblock;
                ViewBag.DatapointData = _reportRepository.GetDashboarAbstractdData(Rpt);
                //ViewBag.JanData = _reportRepository.GetDashboarAbstractdData(Rpt).Result.Select(y=>y.JAN);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult DashboardAbstractc()
        {
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            string currentYear = DateTime.Now.Year.ToString();
            ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
            data.FREQUENCYID = 5;
            ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
            return View();
        }
        [HttpPost]
        public IActionResult DashboardAbstractc(string Distid, string Categoryid, string Yearid)
        {
            AbstractData Rpt = new AbstractData();
            Rpt.Year = int.Parse(Yearid);
            Rpt.Aspirationblock = int.Parse(Categoryid);
            Rpt.DistrictId = int.Parse(Distid);
            BlockData datab = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                datab.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(datab).Result.ToList();
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.DatapointData = _reportRepository.GetDashboarAbstractdData(Rpt);
                return Ok(JsonConvert.SerializeObject(ViewBag.DatapointData));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult DistDashboardV2()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;

                data.FREQUENCYID = 2;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().Where(r => r.FREQUENCYVALUE == "2023");

                // ViewBag.Year1 = DateTime.Now.Year.ToString();
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.Blockdata = _DistrictRepository.GetBlockByDist(leveldetailid).Result;
                ViewBag.DatapointData = "";

                //_reportRepository.GetDashboardData("");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DistDashboardV2(Report Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                Rpt.DistrictId = leveldetailid;
                ViewBag.year = Rpt.Year;
                ViewBag.Blockdata = _DistrictRepository.GetBlockByDist(leveldetailid).Result;
                //ViewBag.DatapointData = _reportRepository.GetDashboardData(Rpt);
                ViewBag.DatapointData = _reportRepository.GetMonthlyYearlyDashboardData(Rpt);
                ViewBag.DatapointDatayearly = _reportRepository.GetYearlyWiseReportData(Rpt);

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost("recurring")]
        public IActionResult Recurring(string mail)
        {
            RecurringJob.AddOrUpdate(() => SendMail(mail), Cron.Minutely);
            return Ok($"The recurring job has been scheduled for user with mail: {mail}.");
        }

        public void SendMail(string mail)
        {
            // Implement any logic you want - not in the controller but in some repository.
            Console.WriteLine($"This is a test - Hello {mail}");
        }
        [HttpGet]
        public IActionResult DashboardData(int Districtid, int Year, int Category, string DistrictName)
        {
            Report Rpt = new Report();
            Rpt.DistrictId = Districtid;
            Rpt.Year = Year;
            Rpt.Aspirationblock = Category;
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                if (Rpt.DisplayName == null)
                {
                    Rpt.DisplayName = "Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec, ";
                }
                string currentYear = DateTime.Now.Year.ToString();
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
                // ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                //***Monthwise datapoint count***//
                ViewBag.Monthwisedp = _DistrictRepository.GetDPMonCount();
                //***Monthwise datapoint count***//
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.DistrictName = DistrictName;
                ViewBag.DistrictId = Districtid;
                ViewBag.DatapointData = _reportRepository.GetMonthlyYearlyDashboardData2(Rpt);
                ViewBag.DatapointDatayearly = _reportRepository.GetYearlyWiseReportData(Rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult SectorEntryPerformance(int? DISTRICTID, int? year, int? Dist)
        {

            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            string currentYear = DateTime.Now.Year.ToString();
            ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
            data.FREQUENCYID = 5;
            ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
            if (DISTRICTID != null && year != null)
            {
                AbstractData Rpt = new AbstractData();
                Rpt.DistrictId = Convert.ToInt32(DISTRICTID);
                ViewBag.year = Rpt.Year;
                ViewBag.Aspirationblock = Rpt.Aspirationblock;
                ViewBag.month = DateTime.Now.Month - 1;
                if (Dist == 0)
                {
                    ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                    ViewBag.DistrictId = 0;
                    Rpt.DistrictId = 0;

                    ViewBag.DatapointData = _reportRepository.GetDashboarAbstractdData(Rpt);
                }
                else
                {
                    ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                    ViewBag.DistrictId = Rpt.DistrictId;
                    ViewBag.DatapointData = _reportRepository.GetDashboarAbstractdData(Rpt);
                }

            }

            return View();
        }
        [HttpPost]
        public IActionResult SectorEntryPerformance(AbstractData Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.month = DateTime.Now.Month - 1;
                ViewBag.Aspirationblock = Rpt.Aspirationblock;
                ViewBag.DatapointData = _reportRepository.GetDashboarAbstractdData(Rpt);
                //ViewBag.JanData = _reportRepository.GetDashboarAbstractdData(Rpt).Result.Select(y=>y.JAN);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
