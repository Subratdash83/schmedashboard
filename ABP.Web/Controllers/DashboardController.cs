using ABP.Domain.BlockData;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.Dashboard;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Dashboard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ABP.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardRepository> Logger;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IBlockDataRepository _blockDataRepository;
        private readonly IDistrictRepository _DistrictRepository;
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public DashboardController(ILogger<DashboardRepository> logger, IHostingEnvironment hostingEnvironment, IDashboardRepository dashboardRepository,IBlockDataRepository blockdataRepository, IDistrictRepository districtRepository,IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _dashboardRepository = dashboardRepository;
            _blockDataRepository = blockdataRepository;
            _DistrictRepository = districtRepository;
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("DashboardController initialized");
        }
        public IActionResult DistrictDashboard(int year, int month, int? BlockId = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData data = new BlockData();
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result;
                if (year == 0)
                {
                    ViewBag.Year1 = DateTime.Now.Year.ToString();
                    year = Convert.ToInt32(ViewBag.Year1);
                }
                else
                {
                    ViewBag.Year1 = year;
                    year = Convert.ToInt32(ViewBag.Year1);
                }
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result;
                if (month == 0)
                {
                    ViewBag.Month1 = DateTime.Now.Month - 1;
                    month = Convert.ToInt32(ViewBag.Month1);
                }
                else
                {
                    ViewBag.Month1 = month;
                    month = Convert.ToInt32(ViewBag.Month1);
                }
                ViewBag.District = _DistrictRepository.GetBlockByDist(0).Result.ToList().Where(x => x.BLOCK_CODE == LeveDetailId);
                ViewBag.BlockWiseData = _dashboardRepository.GetBlockWiseDataByDistID(year,month, Convert.ToInt32(LeveDetailId)).Result;
                
                if (BlockId == 0)
                {
                    BlockId = ViewBag.TopBlock = _dashboardRepository.GetBlockWiseDataByDistID(year, month, Convert.ToInt32(LeveDetailId)).Result.FirstOrDefault().BLOCKID;
                }
                else
                {
                    ViewBag.TopBlock = BlockId;
                }
                ViewBag.SectorDataBlockWise = _dashboardRepository.GetSectorDataByBlockID(year, month, Convert.ToInt32(BlockId)).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult BlockDashboard(int year, int month,int? SectorId=2)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData data = new BlockData();
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result;
                if (year==0)
                {
                    ViewBag.Year1 = DateTime.Now.Year.ToString();
                    year =Convert.ToInt32(ViewBag.Year1);
                }
                else
                {
                    ViewBag.Year1 = year;
                    year = Convert.ToInt32(ViewBag.Year1);
                }
                data.FREQUENCYID = 2;
                ViewBag.Month= _blockDataRepository.GetFrequencyValue(data).Result;
                if (month == 0)
                {
                    ViewBag.Month1 = DateTime.Now.Month - 1;
                    month = Convert.ToInt32(ViewBag.Month1);
                }
                else
                {
                    ViewBag.Month1 = month;
                    month = Convert.ToInt32(ViewBag.Month1);
                }
                
                ViewBag.SectorwiseIcounts = _dashboardRepository.AllSectorIndicatorcounts().Result;
                ViewBag.SectorwiseDPcounts = _dashboardRepository.AllSectorDatapointcounts(month).Result;
                ViewBag.MonthlyDPcounts = _dashboardRepository.MonthlyDatapointcounts(year,month, Convert.ToInt32(LeveDetailId)).Result;
                ViewBag.YearlyDPcounts = _dashboardRepository.YearlyDatapointcounts(year,month,Convert.ToInt32(LeveDetailId)).Result;
                //Sectorwise Indicators and its Datapoint Entered or Not Entered
                ViewBag.IndicatorWiseDP=_dashboardRepository.GetIndicatorDPBySector(year, month, Convert.ToInt32(LeveDetailId), Convert.ToInt32(SectorId)).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult DepartmentDashboard(int year, int month, int? BlockId = 0,int? DistId=0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData data = new BlockData();
                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result;
                if (year == 0)
                {
                    ViewBag.Year1 = DateTime.Now.Year.ToString();
                    year = Convert.ToInt32(ViewBag.Year1);
                }
                else
                {
                    ViewBag.Year1 = year;
                    year = Convert.ToInt32(ViewBag.Year1);
                }
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result;
                if (month == 0)
                {
                    ViewBag.Month1 = DateTime.Now.Month - 1;
                    month = Convert.ToInt32(ViewBag.Month1);
                }
                else
                {
                    ViewBag.Month1 = month;
                    month = Convert.ToInt32(ViewBag.Month1);
                }
                ViewBag.District = _DistrictRepository.GetBlockByDist(0).Result;
                if (DistId == 0)
                {
                    DistId = _DistrictRepository.GetBlockByDist(0).Result.ToList().FirstOrDefault().BLOCK_CODE;  
                }
                ViewBag.BlockWiseData = _dashboardRepository.GetBlockWiseDataByDistID(year, month, Convert.ToInt32(DistId)).Result;
                ViewBag.District1 = DistId;
                if (BlockId == 0)
                {
                    BlockId = ViewBag.TopBlock = _dashboardRepository.GetBlockWiseDataByDistID(year, month, Convert.ToInt32(DistId)).Result.FirstOrDefault().BLOCKID;
                }
                else
                {
                    ViewBag.TopBlock = BlockId;
                }
                ViewBag.SectorDataBlockWise = _dashboardRepository.GetSectorDataByBlockID(year, month, Convert.ToInt32(BlockId)).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult SectorWiseIndicators(int id)
        {
            var result = _dashboardRepository.GetIndicatorBySector(id).Result;
            return Json(result);
        }
    }
}
