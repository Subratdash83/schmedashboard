using ABP.Domain.DataPoint;
using ABP.Domain.Indicator;
using ABP.Repository.Contract.Contract.DataPoint;
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
using System.Xml.Linq;

namespace ABP.Web.Controllers
{
    public class IndicatorFormulaController : Controller
    {
        private readonly ILogger<IndicatorRepository> Logger;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IPanelRepository _sectorRepository;
        private readonly IControlPanelRepository _datapointRepository;
        private readonly IDataPointRepository _datapointrepository;

        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public IndicatorFormulaController(IControlPanelRepository datapointRepository, ILogger<IndicatorRepository> logger, IHostingEnvironment hostingEnvironment, IIndicatorRepository indicatorRepository, IPanelRepository sectorRepository, IConfiguration configuration, IDataPointRepository datapointrepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _indicatorRepository = indicatorRepository;
            _sectorRepository = sectorRepository;
            _datapointRepository = datapointRepository;
            _datapointrepository= datapointrepository;
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("IndicatorFormulaController initialized");
        }
        public IActionResult AddFormula()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _sectorRepository.ViewPanel().Result;
                ViewBag.Frequency = _datapointrepository.ViewFrequency().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult AddFormulav2()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _sectorRepository.ViewPanel().Result;
                ViewBag.Frequency = _datapointrepository.ViewFrequency().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult ViewFormula()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                //ViewBag.Sector = _sectorRepository.ViewPanel().Result;
                //ViewBag.Sector = _sectorRepository.ViewSector().Result;//Binding Initials
                ViewBag.Result = _sectorRepository.ViewIndicatorFormula();//getting all data points to show in the grid

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult BindIndicatorAutoFill(int Sectorid, int IndicatorFormulaID)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var result = _datapointRepository.ControlPanelGateByPanelId(Sectorid, IndicatorFormulaID).Result;
                    var commonres = _datapointRepository.CommonControlPanelData().Result.ToList();
                    List<Domain.Panel.ControlPanel> mlist=result.ToList();
                    foreach (var item in commonres)
                    {
                        mlist.Add(item);
                    }
                    //string jsonnresult = JsonConvert.SerializeObject(result);
                    string jsonnresult = JsonConvert.SerializeObject(mlist);
                    //  ViewBag.Childlist = _ApprovalRepository.GetFETCH(DeptId, 0);
                    return Json(jsonnresult);
                    // return Json(searchlist);
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch { }
            return null;
        }
        public IActionResult BindIndicators(int Sectorid,int frequencyid)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var result = _indicatorRepository.IndicatorGateBySectorId(Sectorid, frequencyid).Result;
                    string jsonnresult = JsonConvert.SerializeObject(result);

                    return Json(jsonnresult);
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch { }
            return null;
        }
        [HttpPost]
        public IActionResult AddFormulaData()
        {

            Indicator IND = new Indicator();

            try
            {
                var data = HttpContext.Request.Form["Elements"];
                List<Indicator> DATAResult = JsonConvert.DeserializeObject<List<Indicator>>(data);
                foreach (var Dresult in DATAResult)
                {
                    string multiCharString = Dresult.IndicatorFormulaName;


                    List<ABP.Domain.Panel.ControlPanel> Datapointdata = (List<ABP.Domain.Panel.ControlPanel>)_datapointRepository.ControlPanelGateByPanelId(Convert.ToInt16(Dresult.SECTORID),0).Result;
                    var commonres = _datapointRepository.CommonControlPanelData().Result.ToList();
                    foreach (var item in commonres)
                    {
                        Datapointdata.Add(item);
                    }
                    string CommaSepDPIds = string.Empty;


                    foreach (var item in Datapointdata)
                    {
                        if (multiCharString.Contains(item.DISPLAYNAME))
                        {
                            CommaSepDPIds = Convert.ToString(item.CONTROLID) + ","+ CommaSepDPIds;
                        }
                        multiCharString = multiCharString.Replace(item.DISPLAYNAME, Convert.ToString("("+item.CONTROLNAME+")"));
                        

                    }
                    Dresult.IndicatorFormulaID ="select ROUND("+ multiCharString+ ",2) as INDICATORVALUE from dual";
                    Dresult.SelectedDataPoints = CommaSepDPIds;
                }


                string result = string.Empty;
                var value = HttpContext.Session.GetInt32("_UserId");
                var errors = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();


                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    try
                    {
                        //  DP.CREATEDBY = Convert.ToInt32(value);
                        //   var ResultDtls = JsonConvert.DeserializeObject<List<KPIFormula>>(data);
                        var ResultDtls = DATAResult;
                        //Convert to xml/text                
                        var xEle = new XElement("INDFormula",
                                  from emp in ResultDtls
                                  select new XElement("row",

                                                 new XElement("SECTORID", emp.SECTORID),
                                                 new XElement("INDICATORID", emp.INDICATORID),
                                                    new XElement("IndicatorFormulaID", emp.IndicatorFormulaID),
                                                      new XElement("IndicatorFormulaName", emp.IndicatorFormulaName),
                                                       new XElement("SelectedDataPoints", emp.SelectedDataPoints)

                                             ));


                        IND.ApprovalConfing = xEle;
                        string retMsg = _indicatorRepository.AddUpdateIndicatorsFormula(IND);
                        // string retMsg = "1";
                        if (retMsg == "1" || retMsg == "2")
                        {
                            return Json("Formula Added Successfuly");
                        }
                        else
                        {
                            return Json("Something went wrong");
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
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }

        }

        [HttpPost]
        public IActionResult DeleteIndicatorFormulaId(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    Indicator model = new Indicator();
                    model.IndicatorFormulaID = (Convert.ToString(id));
                    //model.CREATEDBY = int.Parse(value.ToString());
                    string Result = _sectorRepository.DeleteIndicatorFormulaId(model); //deleting the IndicatorFormulaId by the id from form
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
        public IActionResult BindFormulaById(string IndicatorFormulaID)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //Getting DataPoint By Id To Bind Inputs For Update Purpose
                var datapoint = _sectorRepository.BindFormulaById(Convert.ToInt32(IndicatorFormulaID)).Result;
                return Ok(JsonConvert.SerializeObject(datapoint));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
