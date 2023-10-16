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

namespace ABP.Web.Controllers
{
    public class EmailController : Controller
    {
        private readonly ILogger<EmailController> _logger;
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
        private readonly IDistrictRepository _districtRepository;
        private readonly ILoginRepository _loginRepository;
        public EmailController(ILogger<EmailController> logger, ILoginRepository loginRepository, IDashboardRepository dashboardRepository, IBDODataRepository bdodataRepository, IDataPointRepository datapointRepository, IBlockRepository blockRepository, ISectorRepository sectorRepository, IHostingEnvironment hostingEnvironment, IPanelRepository panelRepository, IBlockDataRepository blockdataRepository, IDistrictDataRepository districtDataRepository, IIndicatorRepository indicatorRepository, IReportRepository reportRepository, IDistrictRepository districtRepository)
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

        public IActionResult MailForBlockUser()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.DistrictData = _DistrictRepository.GetDistrictAsync().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult MailForBlockUser(Report rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.DistrictData = _DistrictRepository.GetDistrictAsync().Result;
                ViewBag.BlockData = _DistrictRepository.GetByBlockDetails(rpt.DistrictId);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult MailForDistrictUser()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //ViewBag.DistrictData = _DistrictRepository.GetDistrictAsync().Result;

                ViewBag.BlockData = _DistrictRepository.GetByDistrictDetails();
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]

        public IActionResult MailForDistrictUser(Report rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //ViewBag.DistrictData = _DistrictRepository.GetDistrictAsync().Result;
                ViewBag.BlockData = _DistrictRepository.GetByDistrictDetails();
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

    }
}

