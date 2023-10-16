using ABP.Domain.BlockData;
using ABP.Domain.DataPoint;
using ABP.Domain.Department;
using ABP.Domain.Indicator;
using ABP.Domain.Panel;
using ABP.Domain.Sector;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.Department;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Repository.Department;
using ABP.Repository.District;
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
    public class DepartmentController : Controller
    {
        private readonly ILogger<DepartmentRepository> Logger;
        private readonly IDepartmentRepository _dpartmentRepository;
        private readonly IDistrictRepository _DistrictRepository;
        private readonly IDistrictDataRepository _districtDataRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IDataPointRepository _datapointRepository;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IBlockDataRepository _blockDataRepository;
       
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IPanelRepository _panelRepository;
        public IConfiguration Configuration { get; }
        public DepartmentController(ILogger<DepartmentRepository> logger, IDistrictDataRepository districtDataRepository, IHostingEnvironment hostingEnvironment, IDataPointRepository datapointRepository,IDepartmentRepository departmentRepository, ISectorRepository sectorRepository, IBlockDataRepository blockdataRepository,IIndicatorRepository indicatorRepository, IConfiguration configuration, IPanelRepository panelRepository, IDistrictRepository districtRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _dpartmentRepository = departmentRepository;
            _sectorRepository = sectorRepository;
            _indicatorRepository = indicatorRepository;
            _districtDataRepository = districtDataRepository;
            _panelRepository = panelRepository;
            _datapointRepository = datapointRepository;
            _DistrictRepository = districtRepository;
            _blockDataRepository= blockdataRepository;
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("DepartmentController initialized");
        }
        [HttpGet]
        public IActionResult AddDepartment()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }
        public IActionResult ViewDepartment()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Result = _dpartmentRepository.ViewDepartment(0,0);//getting all department data to bind in grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }

        [HttpPost]
        public IActionResult ViewDepartment(sector sector)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Department = _dpartmentRepository.ViewLevels().Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Result = _dpartmentRepository.ViewDepartment(sector.SECTORID,sector.Deptid);//getting all department data to bind in grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public async Task<JsonResult> AddDepartment(Department department)
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
                        department = new Department();
                        try
                        {
                            department.ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);
                            department.SECTORID = Convert.ToInt32(HttpContext.Request.Form["SECTORID"]);
                            department.DEPTID = Convert.ToInt32(HttpContext.Request.Form["DEPTID"]);
                            department.DESCRIPTION = HttpContext.Request.Form["DESCRIPTION"].ToString();
                            department.CREATEDBY = Convert.ToInt32(value);
                            string retMsg = _dpartmentRepository.AddDepartment(department);
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
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    Department model = new Department();
                    model.ID = id;
                    model.CREATEDBY = int.Parse(value.ToString());
                    string Result = _dpartmentRepository.DeleteDepartment(model);
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
        public IActionResult DepartmentGateById(string DeptId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var department = _dpartmentRepository.DepartmentGateById(Convert.ToInt32(DeptId)).Result;
                return Ok(JsonConvert.SerializeObject(department));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpGet]
        public ActionResult GetDatapoints(string AppNo, int Status)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.APPLICATIONNO = AppNo;
                bd.STATUS = Status;
                bd.BLOCKID = (int)LeveDetailId;
                BlockData blockData = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                if (blockData != null)
                {
                    List<ControlPanel> DataPoints = new List<ControlPanel>();
                    List<ControlPanel> DataPointsNew = null;
                    List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(blockData).Result.ToList();
                    foreach (var cv in Indicators)
                    {
                        DataPointsNew = new List<ControlPanel>();
                        cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno='" + blockData.APPLICATIONNO + "'"));
                        DataPointsNew = _districtDataRepository.GetAllDPByIndicator(cv);
                        DataPoints.AddRange(DataPointsNew);
                    }
                    foreach (var cv in DataPoints)
                    {
                        cv.CONTROLVALUE = _blockDataRepository.GetContolValue(cv, blockData.APPLICATIONNO);
                    }
                    ViewBag.IndicatorResult = Indicators;
                    ViewBag.DPResult = DataPoints;
                }
                return PartialView("_DisplayDataNamesPartial");
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult ReportCollectorData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {             
                             
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _districtDataRepository.GetAllDepartmentData(new BlockData());
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        #region Department view District and Block wise
        [HttpGet]
        public IActionResult DepartmentReport()
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                data.FREQUENCYID = 5;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.DistrictData = _districtDataRepository.GetDistrict(0).Result;
                ViewBag.bid = '0';
                       DataPoint dp = new DataPoint();
                ViewBag.Result = null;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DepartmentReport(BlockData bd)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                data.FREQUENCYID = 5;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                //ViewBag.DistrictData = _districtDataRepository.GetBlockByDist(0).Result;
                ViewBag.DistrictData = _districtDataRepository.GetDistrict(0).Result;
                ViewBag.Blockdata = _DistrictRepository.GetBlockByDist(0).Result;
                DataPoint dp = new DataPoint();
                dp.leveldetailedid = bd.DISTRICTID;
                dp.SECTORID = bd.PANELID;
                dp.YEAR = bd.YEAR;
                dp.FREQUENCYID= bd.FREQUENCYID;
                dp.BLOCKID = bd.BLOCKID;
                ViewBag.bid= bd.BLOCKID;
                ViewBag.Result = _districtDataRepository.GetAllBlocksApprovedData(dp, "COLLECTOR");
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        #endregion
        [HttpGet]
        public IActionResult GetBlockByDistID(int id)
        {
            var result = _DistrictRepository.GetBlockByDist(id).Result;
            return Json(result);
        }
    }
}
