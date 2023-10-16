using ABP.Domain.Department;
using ABP.Domain.Indicator;
using ABP.Domain.Sector;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.Department;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Repository.Indicator;
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
    public class IndicatorController : Controller
    {
        private readonly ILogger<IndicatorRepository> Logger;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IDepartmentRepository _dpartmentRepository;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IPanelRepository _panelRepository;
        private readonly IDataPointRepository _datapointRepository;
        public IConfiguration Configuration { get; }
        public IndicatorController(ILogger<IndicatorRepository> logger, IHostingEnvironment hostingEnvironment, IIndicatorRepository indicatorRepository, IDataPointRepository datapointRepository, ISectorRepository sectorRepository, IConfiguration configuration, IDepartmentRepository dpartmentRepository, IPanelRepository panelRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _indicatorRepository = indicatorRepository;
            _sectorRepository = sectorRepository;
            _dpartmentRepository = dpartmentRepository;
            Configuration = configuration;
            _panelRepository = panelRepository;
            _datapointRepository = datapointRepository;
            Logger = logger;
            Logger.LogInformation("IndicatorController initialized");
        }
        public IActionResult AddIndicator()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                //ViewBag.Sector = _sectorRepository.ViewSector().Result;
                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }
        public IActionResult ViewIndicator()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                //ViewBag.Sector = _sectorRepository.ViewSector().Result;
                ViewBag.Result = _indicatorRepository.ViewIndicator(0, 0,0);//getting all indicator data to bind in the grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult ViewIndicator(sector sector)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.sectorid = sector.SECTORID;
                ViewBag.indicatorid = sector.INDICATORID;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _indicatorRepository.ViewIndicator(sector.SECTORID, sector.INDICATORID,sector.FREQUENCYID);//getting all indicator data to bind in the grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public async Task<JsonResult> AddIndicator(Indicator indicator)
        {
            try
            {
                string result = string.Empty;
                var value = HttpContext.Session.GetInt32("_UserId");
                var errors = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                if (!ModelState.IsValid)
                {
                    return Json(errors.ErrorMessage);

                }
                else
                {
                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        indicator = new Indicator();
                        try
                        {
                            indicator.INDICATORID = Convert.ToInt32(HttpContext.Request.Form["INDICATORID"]);
                            indicator.SECTORID = Convert.ToInt32(HttpContext.Request.Form["SECTORID"]);
                            indicator.INDICATORNAME = HttpContext.Request.Form["INDICATORNAME"].ToString();
                            indicator.INDICATORTYPE = Convert.ToInt32(HttpContext.Request.Form["INDICATORTYPE"]);
                            indicator.DESCRIPTION = HttpContext.Request.Form["DESCRIPTION"].ToString();
                            indicator.INDICATORDATE = HttpContext.Request.Form["INDICATORDATE"].ToString();
                            indicator.TARGETVALUE = Convert.ToInt32(HttpContext.Request.Form["TARGETVALUE"].ToString());
                            indicator.UNIT = HttpContext.Request.Form["UNIT"].ToString();
                            indicator.FREQUENCYID=Convert.ToInt32(HttpContext.Request.Form["FREQUENCYID"]);
                            indicator.CREATEDBY = Convert.ToInt32(value);
                            string retMsg = _indicatorRepository.InsertIndicator(indicator);
                            if (retMsg != "0")
                            {
                                Department department = new Department();
                                department.ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);
                                department.SECTORID = Convert.ToInt32(HttpContext.Request.Form["SECTORID"]);
                                department.INDICATORID = Convert.ToInt32(retMsg);
                                department.DEPTID = Convert.ToInt32(HttpContext.Request.Form["DEPTID"]);
                                department.DESCRIPTION = HttpContext.Request.Form["DESCRIPTION"].ToString();
                                department.CREATEDBY = Convert.ToInt32(value);
                                string retMsg2 = _dpartmentRepository.AddDepartment(department);
                                
                                if (retMsg2 == "1")
                                {
                                    return Json("Record Saved Successfully");
                                }
                                else if (retMsg2 == "2")
                                {
                                    return Json("Record Updated Successfully");
                                }
                                else if (retMsg2 == "3")
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
                                return Json("Some Error Occured in Department Mapping");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, ex.Message);
                            result = ex.Message;
                            return Json(ex.Message);
                        }
                    }
                    else
                    {
                        RedirectToAction("Logout", "Home");
                        return Json(null);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        [HttpPost]
        public IActionResult DeleteIndicator(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    Indicator model = new Indicator();
                    model.INDICATORID = id;
                    model.CREATEDBY = int.Parse(value.ToString());
                    string Result = _indicatorRepository.DeleteIndicator(model);
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
        public IActionResult IndicatorGateById(string IndicatorId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var indicators = _indicatorRepository.IndicatorGateById(Convert.ToInt32(IndicatorId)).Result;
                return Ok(JsonConvert.SerializeObject(indicators));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
