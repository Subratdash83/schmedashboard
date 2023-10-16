using ABP.Domain.Indicator;
using ABP.Domain.Sector;
using ABP.Repository.Contract.Contract.CollectorIndicatorData;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Repository.Indicator;
using ABP.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Controllers
{
    public class IndicatorMappingController : Controller
    {
        private readonly ILogger<IndicatorRepository> Logger;
        private readonly ISectorRepository _sectorRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IDataPointRepository _datapointRepository;
        private readonly ICollectorDataIndicatorRepository _collectordataindicatorRepository;
        private readonly IPanelRepository _panelRepository;
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public IndicatorMappingController(ILogger<IndicatorRepository> logger, IHostingEnvironment hostingEnvironment, ISectorRepository sectorRepository, IIndicatorRepository indicatorRepository, IDataPointRepository datapointRepository, ICollectorDataIndicatorRepository collectordataindicatorRepository, IConfiguration configuration, IPanelRepository panelRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _panelRepository = panelRepository;
            _indicatorRepository = indicatorRepository;
            _sectorRepository = sectorRepository;
            _datapointRepository = datapointRepository;
            _collectordataindicatorRepository = collectordataindicatorRepository;
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("IndicatorMappingController initialized");
        }

        [HttpGet]
        public IActionResult GetIsCensusBySectorId(string SectorId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var DataPoints = _datapointRepository.DataPointiscensus(Convert.ToInt32(SectorId)).Result;
                return Ok(JsonConvert.SerializeObject(new
                {
                    datapoints = DataPoints
                }));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpGet]
        public IActionResult AddIndicatorMapping()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Indicator = _indicatorRepository.IndicatorGateBySectorId(Convert.ToInt32(0), Convert.ToInt32(0)).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetIndicatorAndDataPointsBySectorId(string SectorId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Indicators = _indicatorRepository.IndicatorGateBySectorId(Convert.ToInt32(SectorId), 0).Result;
               // var DataPoints = _datapointRepository.DataPointGateBySectorId(Convert.ToInt32(SectorId)).Result;
                return Ok(JsonConvert.SerializeObject(new
                {
                    indicators = Indicators,
                    //datapoints = DataPoints
                }));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult AddIndicatorMapping([FromBody] InicatorMappingDTO InicatorMappingObj)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                IndicatorMapping New = new IndicatorMapping();
                New.INDICATORMAPPINGID = Convert.ToInt32(InicatorMappingObj.IndicatorMappingId);
                New.SECTORID = Convert.ToInt32(InicatorMappingObj.SectorId);
                New.INDICATORID = Convert.ToInt32(InicatorMappingObj.IndicatorId);
                New.FORMULA = InicatorMappingObj.Formula;
                if (InicatorMappingObj.iscensus == 1)
                {
                    New.DDATAPOINTID = Convert.ToInt32(InicatorMappingObj.CheckedValuesIncenus);
                   // New.NDATAPOINTID = Convert.ToInt32(InicatorMappingObj.CheckedValuesIncenus);


                }
                else
                {
                    New.DDATAPOINTID = Convert.ToInt32(InicatorMappingObj.CheckedValuesD);
                    New.NDATAPOINTID = Convert.ToInt32(InicatorMappingObj.CheckedValuesN);
                }
                New.CREATEDBY = (Int32)UserId;
                New.iscensus = Convert.ToInt32(InicatorMappingObj.iscensus);

                string retMsg = _indicatorRepository.AddIndicatorMapping(New);
                if (retMsg == "1")
                {
                    return Json("Record Saved Successfully");
                }
                else if (retMsg == "2")
                {
                    return Json("Record Updated Successfully");
                }
                else if (retMsg == "3")
                {
                    return Json("Record Deleted Successfully");
                }
                else
                {
                    return Json("Record Already Exist");
                }
            }
            else
            {
                 RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        public IActionResult ViewIndicatorMapping()
        {
            var value = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                var ACTION = "VIEW";
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Result = _indicatorRepository.ViewIndicatorMapping(ACTION,0,0);//getting all indicator mapping data to bind in the grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewIndicatorMapping(sector sector)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.sectorid = sector.SECTORID;
                ViewBag.indicatorid = sector.INDICATORID;
                var ACTION = "VIEW";
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Result = _indicatorRepository.ViewIndicatorMapping(ACTION,sector.SECTORID,sector.INDICATORID);//getting all indicator mapping data to bind in the grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DeleteIndicatorMapping(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    IndicatorMapping model = new IndicatorMapping();
                    model.INDICATORMAPPINGID = id;
                    model.CREATEDBY = int.Parse(value.ToString());
                    string Result = _indicatorRepository.DeleteIndicatorMapping(model);
                    return Json(Result);
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }
        [HttpGet]
        public IActionResult IndicatorMappingGateById(string IMId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var indicatormappings = _indicatorRepository.IndicatorMappingGateByIMId(Convert.ToInt32(IMId)).Result;
                return Ok(JsonConvert.SerializeObject(indicatormappings));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult IndicatorMappingGateBySectorAndImId(string SectorId, string IMId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var indicatormappings = _indicatorRepository.IndicatorMappingGateBySectorAndIMId(Convert.ToInt32(SectorId), Convert.ToInt32(IMId)).Result;
                return Ok(JsonConvert.SerializeObject(indicatormappings));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
