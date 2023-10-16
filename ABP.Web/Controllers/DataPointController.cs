using ABP.Domain.DataPoint;
using ABP.Domain.Panel;
using ABP.Domain.Sector;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Web.Controllers
{
    public class DataPointController : Controller
    {
        private readonly ILogger<IDataPointRepository> Logger;
        private readonly IDataPointRepository _datapointRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IControlPanelRepository _controlPanelRepository; 
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public DataPointController(ILogger<IDataPointRepository> logger, IHostingEnvironment hostingEnvironment, IDataPointRepository datapointRepository, ISectorRepository sectorRepository, IPanelRepository panelRepository,IControlPanelRepository controlPanelRepository,IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _datapointRepository = datapointRepository;
            _sectorRepository = sectorRepository;
            Configuration = configuration;
            Logger = logger;
            _panelRepository = panelRepository;
            _controlPanelRepository = controlPanelRepository;
            Log.Information("DataPointController initialized");
        }
        [HttpGet]
        public IActionResult AddDataPoint()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Department = _panelRepository.ViewLevels().Result;
                ViewBag.Provider = _panelRepository.ViewProviders().Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Frequency1 = _datapointRepository.ViewFrequencyWiseMonth().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult ViewDataPoint()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<Panel> panels = ViewBag.Sector = _panelRepository.ViewPanel().Result.ToList();
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Frequency1 = _datapointRepository.ViewFrequencyWiseMonth().Result;
                sector sector = new sector();
                List<sector> sectors = (from e in panels
                                        select new sector
                                        {
                                            SECTORID = e.PANELID,
                                            SECTORNAME = e.DISPLAYNAME                                            
                                        }).ToList();
                sectors.Add(sector);
                ViewBag.SectorResult = sectors;
                ViewBag.Result = _controlPanelRepository.ViewControlPanel(0,0).Result.ToList();
                List<ControlPanel> controlpanels = ViewBag.Result;
                DataPoint datapoint = new DataPoint();
                List<DataPoint> datapoints = (from e in controlpanels
                                              select new DataPoint
                                              {
                                                  SECTORID = e.PANELID,
                                                  DATAPOINTID = e.CONTROLID,
                                                  DESCRIPTION=e.DESCRIPTION,
                                                  SECTORNAME = e.DISPLAYNAME,
                                                  DATAPOINTNAME = e.DISPLAYNAME1,
                                                  DATAPOINTDATE = Convert.ToDateTime(e.EFFECTIVEDATE).ToString("dd-MMM-yy"),
                                                  FREQUENCY = e.FREQUENCY,
                                                  MONTHNAME =e.MONTHNAME,
                                                  FREQUENCYID = e.FREQUENCYID,
                                                  SERIALNO =e.SERIALNO,
                                                  UNIT = e.UNIT,
                                                  TYPE = e.TYPE,
                                                  YEARTYPE = e.YEARTYPE,
                                                  DepartmentName=e.DepartmentName,
                                                  PROVIDERNAME = e.PROVIDERNAME
                                              }).ToList();
                
                ViewBag.DatapointResult = datapoints;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult ViewDataPoint(sector sector)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.sectorid = sector.SECTORID;
                ViewBag.datapointid = sector.DATAPOINTID;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _controlPanelRepository.ViewControlPanel(sector.SECTORID,sector.FREQUENCY).Result.ToList();//getting all data points to show in the grid
                List<ControlPanel> controlpanels = ViewBag.Result;
                DataPoint datapoint = new DataPoint();
                List<DataPoint> datapoints = (from e in controlpanels
                                              select new DataPoint
                                              {
                                                  SECTORID = e.PANELID,
                                                  DATAPOINTID = e.CONTROLID,
                                                  DESCRIPTION = e.DESCRIPTION,
                                                  SECTORNAME = e.DISPLAYNAME,
                                                  DATAPOINTNAME = e.DISPLAYNAME1,
                                                  DATAPOINTDATE = Convert.ToDateTime(e.EFFECTIVEDATE).ToString("dd-MMM-yy"),
                                                  FREQUENCY = e.FREQUENCY,
                                                  MONTHNAME = e.MONTHNAME,
                                                  FREQUENCYID =e.FREQUENCYID,
                                                  SERIALNO = e.SERIALNO,
                                                  UNIT=e.UNIT,
                                                  TYPE=e.TYPE,
                                                  YEARTYPE=e.YEARTYPE,
                                                  DepartmentName = e.DepartmentName,
                                                  PROVIDERNAME = e.PROVIDERNAME
                                              }).ToList();

                ViewBag.DatapointResult = datapoints;
                ViewBag.frequencyid = sector.FREQUENCY;

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        // check box submission Start

        [HttpPost]
        public IActionResult UpdateSerialNo()
        {
            try
            {
                DataPoint datapoint = new DataPoint();
                var data = HttpContext.Request.Form["Elements"];
                var ResultDtls = JsonConvert.DeserializeObject<List<DataPoint>>(data);
                foreach (var dp in ResultDtls)
                {
                    string output = _controlPanelRepository.ModifySLNO(dp.CONTROLID, dp.SERIALNO);
                }
                return Json(new { Response = 1, Id = 1 });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        // check box submission End
        [HttpPost]
        public async Task<JsonResult> AddDataPoint(DataPoint dataPoint)
        {
            try
            {
                ViewBag.Department = _panelRepository.ViewLevels().Result;
                ViewBag.Provider = _panelRepository.ViewProviders().Result;
                ControlPanel controlp = new ControlPanel();
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
                        dataPoint = new DataPoint();
                        try
                        {
                            //Getting values from form to Insert/Update into DB
                            string SeqVal = _panelRepository.GetSeqValue("CONTROLNAME_SEQ");
                            controlp.CONTROLNAME = "CONTROL_" + SeqVal;
                            controlp.CONTROLID = Convert.ToInt32(HttpContext.Request.Form["DATAPOINTID"]);
                            controlp.PANELID = Convert.ToInt32(HttpContext.Request.Form["SECTORID"]);
                            controlp.DISPLAYNAME1 = HttpContext.Request.Form["DATAPOINTNAME"].ToString();
                            controlp.EFFECTIVEDATE = HttpContext.Request.Form["DATAPOINTDATE"].ToString();
                            controlp.DESCRIPTION = HttpContext.Request.Form["DESCRIPTION"].ToString();
                            controlp.FREQUENCYID = Convert.ToInt32(HttpContext.Request.Form["FREQUENCYID"]);
                            controlp.MONTHNO = Convert.ToInt32(HttpContext.Request.Form["MONTHNO"]);
                            controlp.UNIT = Convert.ToString(HttpContext.Request.Form["UNIT"]);
                            controlp.DEPTID= Convert.ToInt32(HttpContext.Request.Form["DEPTID"]);
                            controlp.PROVIDERID = Convert.ToInt32(HttpContext.Request.Form["PROVIDERID"]);
                            controlp.TYPE = Convert.ToInt32(HttpContext.Request.Form["TYPE"]);
                            controlp.YEARTYPE = Convert.ToInt32(HttpContext.Request.Form["YEARTYPE"]);
                            controlp.CREATEDBY = Convert.ToInt32(value);
                            string retMsg = _controlPanelRepository.InsertControlPanel(controlp);
                            if (retMsg == "1")
                            {
                                string ret = _controlPanelRepository.AlterTable("T_ABP_" + _panelRepository.PanelGateById(controlp.PANELID).Result.ToList()[0].PANELNAME, controlp.CONTROLNAME);
                                if (ret == "1")
                                {
                                    return Json("Record Saved Successfully");
                                }
                                else
                                {
                                    return Json("Failled To Alter Table");
                                }
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
        public IActionResult DeleteDataPoint(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    ControlPanel model = new ControlPanel();
                    model.CONTROLID = id;
                    //model.CREATEDBY = int.Parse(value.ToString());
                    string Result = _controlPanelRepository.DeleteControlPanel(model); 
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
        public IActionResult DataPointGateById(string DATAPOINTID)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {                
                ControlPanel controlpanel = ViewBag.ControlPanels = _controlPanelRepository.ControlPanelGateById(Convert.ToInt32(DATAPOINTID)).Result.FirstOrDefault();
                DataPoint dataPoint = new DataPoint();
                List<DataPoint> dataPoints = new List<DataPoint>();
                dataPoint.SECTORNAME = controlpanel.DISPLAYNAME;
                dataPoint.DATAPOINTDATE = Convert.ToDateTime(controlpanel.EFFECTIVEDATE).ToString("dd-MMM-yy");
                dataPoint.SECTORID = controlpanel.PANELID;
                dataPoint.DATAPOINTID = controlpanel.CONTROLID;
                dataPoint.DATAPOINTNAME = controlpanel.DISPLAYNAME1;
                dataPoint.FREQUENCYID = controlpanel.FREQUENCYID;
                dataPoint.DESCRIPTION = controlpanel.DESCRIPTION;
                dataPoint.DEPTID = controlpanel.DEPTID;
                dataPoint.PROVIDERID = controlpanel.PROVIDERID;
                dataPoint.UNIT = controlpanel.UNIT;
                dataPoint.TYPE = controlpanel.TYPE;
                dataPoint.YEARTYPE = controlpanel.YEARTYPE;
                dataPoint.MONTHNO = controlpanel.MONTHNO;
                dataPoints.Add(dataPoint);
                return Ok(JsonConvert.SerializeObject(dataPoints));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
