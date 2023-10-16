using ABP.Domain.Block;
using ABP.Domain.BlockData;
using ABP.Domain.CollectorData;
using ABP.Domain.DataPoint;
using ABP.Domain.Indicator;
using ABP.Domain.Panel;
using ABP.Infrastructure;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Contract.BDOData;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.CollectorData;
using ABP.Repository.Contract.Contract.CollectorIndicatorData;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Web.Controllers
{
    public class CollectorDataController : Controller
    {
        private readonly ILogger<IDataPointRepository> Logger;
        private readonly IBlockDataRepository _blockDataRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IDataPointRepository _datapointRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly ICollectorDataRepository _collectordataRepository;
        private readonly ICollectorDataIndicatorRepository _collectordataindicatorRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IBDODataRepository _bdodataRepository;
        private readonly IDistrictDataRepository _districtDataRepository;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IPanelRepository _panelRepository;
        private readonly ISendSMSRepository _sms;
        public IConfiguration Configuration { get; }
        public CollectorDataController(ILogger<IDataPointRepository> logger, IHostingEnvironment hostingEnvironment, IDistrictDataRepository districtDataRepository, IDataPointRepository datapointRepository, ISectorRepository sectorRepository, ICollectorDataRepository collectordataRepository, ICollectorDataIndicatorRepository collectordataindicatorRepository, IDistrictRepository distrcitRepository, IBlockRepository blockRepository, IIndicatorRepository indicatorRepository, IBDODataRepository bdodataRepository, IConfiguration configuration, ISendSMSRepository sms, IPanelRepository panelRepository, IBlockDataRepository blockDataRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _blockDataRepository = blockDataRepository;
            _sectorRepository = sectorRepository;
            _datapointRepository = datapointRepository;
            _indicatorRepository = indicatorRepository;
            _collectordataRepository = collectordataRepository;
            _collectordataindicatorRepository = collectordataindicatorRepository;
            _districtRepository = distrcitRepository;
            _blockRepository = blockRepository;
            _panelRepository = panelRepository;
            _bdodataRepository = bdodataRepository;
            Configuration = configuration;
            Logger = logger;
            _sms = sms;
            Logger.LogInformation("CollectorDataController initialized");
        }
        public IActionResult CollectorDataApproval()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                ViewBag.Result = null;
                ViewBag.Block = blocks;
                ViewBag.Sector = _panelRepository.ViewPanel();
                ViewBag.BlockId = 0;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetCollectorDataByBlockId(string BlockId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                List<DataPoint> dps = new List<DataPoint>();
                var datapoints = _datapointRepository.DPGroupBySector(2, Convert.ToInt32(BlockId)).Result;
                foreach (var dp in datapoints)
                {
                    dps.Add(dp);
                }
                ViewBag.Block = blocks;
                ViewBag.Result = dps;
                ViewBag.Sector = _panelRepository.ViewPanel();
                ViewBag.BlockId = Convert.ToInt32(BlockId);
                return View("CollectorDataApproval");
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult AcceptCollectorData([FromBody] List<DataPoint> dpListObjFromForm)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<DataPoint> dpListObj = ViewBag.dpListObj = _datapointRepository.DPGroupBySector(2, dpListObjFromForm[0].BLOCKID).Result;
                InserCollectorData(dpListObj, dpListObjFromForm, 1, (int)UserId);
                return Json("Record(s) Accepted Successfully");
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        [HttpPost]
        public IActionResult RejectCollectorData([FromBody] List<DataPoint> dpListObjFromForm)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<DataPoint> dpListObj = ViewBag.dpListObj = _datapointRepository.DPGroupBySector(2, dpListObjFromForm[0].BLOCKID).Result;
                InserCollectorData(dpListObj, dpListObjFromForm, 3, (int)UserId);
                return Json("Record(s) Accepted Successfully");
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        [HttpPost]
        public IActionResult SaveAsDraftCollectorData([FromBody] List<DataPoint> dpListObjFromForm)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<DataPoint> dpListObj = ViewBag.dpListObj = _datapointRepository.DPGroupBySector(2, dpListObjFromForm[0].BLOCKID).Result;
                InserCollectorData(dpListObj, dpListObjFromForm, 2, (int)UserId);
                return Json("Record(s) Submitted Successfully");
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        public IActionResult CollectorDataIndicator()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                ViewBag.Block = blocks;
                ViewBag.Sector = null;
                ViewBag.BlockId = 0;
                ViewBag.IndicatorResult = null;
                ViewBag.DataPointResult = null;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetIndicatorDataByBlockId(string BlockId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var starteddate = GetDataEntryDate(Convert.ToInt32(BlockId), 0, 0);
                var Frequency = Convert.ToDateTime(starteddate).Month;
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                List<Indicator> FinalInds = new List<Indicator>();
                List<DataPoint> DataPoints = new List<DataPoint>();
                ViewBag.Block = blocks;
                ViewBag.IndicatorResult = _indicatorRepository.IndicatorsGPBySector(Frequency, Convert.ToInt32(BlockId)).Result;
                //FinalInds;
                ViewBag.Sector = null;
                //_sectorRepository.ViewSector();
                ViewBag.BlockId = Convert.ToInt32(BlockId);
                ViewBag.DataPointResult = DataPoints;
                return View("CollectorDataIndicator");
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult SaveAsDraftCollectorDataIndicator([FromBody] List<Indicator> indicatorFromFormObj)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //converted list data to xml..
                var xEle = new XElement("person",
                                        from emp in indicatorFromFormObj
                                        select new XElement("row",
                                                       new XElement("NBDODATAID", emp.NBDODATAID),
                                                       new XElement("DBDODATAID", emp.DBDODATAID)
                                                   ));
                //collecting all bdodataid for updation purpose
                List<int> Bdodatalist = new List<int>();
                Bdodatalist.AddRange(indicatorFromFormObj.Select(u => u.NBDODATAID).ToList());
                Bdodatalist.AddRange(indicatorFromFormObj.Select(u => u.DBDODATAID).ToList());
                var xEle1 = new XElement("person",
                                       from emp in Bdodatalist
                                       select new XElement("row",
                                                      new XElement("BDODATAID", emp)
                                                  ));
                string RetVal = _collectordataindicatorRepository.AddCollectorDataIndicator(xEle, xEle1, 2, (Int32)UserId, indicatorFromFormObj[0].COLLECTORREMARK);
                if (RetVal == "1")
                {
                    ViewBag.dpListObj = _datapointRepository.DPGroupBySector(2, indicatorFromFormObj[0].BLOCKID).Result;
                    return Json("Indicator(s) Saved As Draft Successfully");
                }
                else
                {
                    return Json("Indicator(s) couldn't saved as draft successfully");
                }
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        [HttpPost]
        public IActionResult AcceptCollectorDataIndicator([FromBody] List<Indicator> indicatorFromFormObj)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //converted list data to xml..
                var xEle = new XElement("person",
                                        from emp in indicatorFromFormObj
                                        select new XElement("row",
                                                       new XElement("NBDODATAID", emp.NBDODATAID),
                                                       new XElement("DBDODATAID", emp.DBDODATAID)
                                                   ));
                //collecting all bdodataid for updation purpose
                List<int> Bdodatalist = new List<int>();
                Bdodatalist.AddRange(indicatorFromFormObj.Select(u => u.NBDODATAID).ToList());
                Bdodatalist.AddRange(indicatorFromFormObj.Select(u => u.DBDODATAID).ToList());
                var xEle1 = new XElement("person",
                                       from emp in Bdodatalist
                                       select new XElement("row",
                                                      new XElement("BDODATAID", emp)
                                                  ));
                string RetVal = _collectordataindicatorRepository.AddCollectorDataIndicator(xEle, xEle1, 1, (Int32)UserId, indicatorFromFormObj[0].COLLECTORREMARK);
                if (RetVal == "1")
                {
                    ViewBag.dpListObj = _datapointRepository.DPGroupBySector(2, indicatorFromFormObj[0].BLOCKID).Result;
                    #region Text Message
                    string mobno = "9437307915";
                    var mobilenumber = Convert.ToInt64(mobno);
                    var template = "Dear User, ABP Data approved for the month " + indicatorFromFormObj[0].MONTHNAME + " by dist. collector. Access portal https://sdbschmes.csmpl.com/ for further details. Odisha State Dashboard.";
                    var templateid = "1007881242922181891";
                    var smsresult = _sms.Sendsms(mobno, template, templateid);
                    TempData["done"] = smsresult.Result;
                    //"Reminder Sent Successfully";
                    //return Json(smsresult.Result);
                    #endregion
                    return Json("Indicator(s) Accepted Successfully");
                }
                else
                {
                    return Json("Indicator(s) couldn't accepted successfully");
                }
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        [HttpPost]
        public IActionResult RejectCollectorDataIndicator([FromBody] List<Indicator> indicatorFromFormObj)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //converted list data to xml..
                var xEle = new XElement("person",
                                        from emp in indicatorFromFormObj
                                        select new XElement("row",
                                                       new XElement("NBDODATAID", emp.NBDODATAID),
                                                       new XElement("DBDODATAID", emp.DBDODATAID)
                                                   ));
                //collecting all bdodataid for updation purpose
                List<int> Bdodatalist = new List<int>();
                Bdodatalist.AddRange(indicatorFromFormObj.Select(u => u.NBDODATAID).ToList());
                Bdodatalist.AddRange(indicatorFromFormObj.Select(u => u.DBDODATAID).ToList());
                var xEle1 = new XElement("person",
                                       from emp in Bdodatalist
                                       select new XElement("row",
                                                      new XElement("BDODATAID", emp)
                                                  ));
                string RetVal = _collectordataindicatorRepository.AddCollectorDataIndicator(xEle, xEle1, 3, (Int32)UserId, indicatorFromFormObj[0].COLLECTORREMARK);
                if (RetVal == "1")
                {
                    ViewBag.dpListObj = _datapointRepository.DPGroupBySector(2, indicatorFromFormObj[0].BLOCKID).Result;
                    #region Text Message
                    string mobno = "9437307915";
                    var mobilenumber = Convert.ToInt64(mobno);
                    var template = "Dear User, ABP Data Rejected for the month " + indicatorFromFormObj[0].MONTHNAME + " by dist. collector. Access portal https://sdbschmes.csmpl.com/ for further details. Odisha State Dashboard.";
                    var templateid = "1007881242922181891";
                    var smsresult = _sms.Sendsms(mobno, template, templateid);
                    TempData["done"] = smsresult.Result;
                    //"Reminder Sent Successfully";
                    //return Json(smsresult.Result);
                    #endregion
                    return Json("Indicator(s) Rejected Successfully");
                }
                else
                {
                    return Json("Indicator(s) couldn't rejected successfully");
                }
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        public void InserCollectorData(List<DataPoint> dpListObj, List<DataPoint> dpListObjFromForm, int Status, int UserId)
        {

            //converted for each loop to xml..
            var xEle = new XElement("person",
                                         from emp in dpListObj
                                         select new XElement("row",
                                                        new XElement("DATAPOINTID", emp.DATAPOINTID),
                                                        new XElement("BDODATAID", emp.BDODATAID),

                                                        new XElement("BLOCKID", emp.BLOCKID),
                                                        new XElement("SECTORID", emp.SECTORID),
                                                        new XElement("DATAPOINTVALUE", emp.DATAPOINTVALUE),
                                                        new XElement("YEAR", emp.YEAR),
                                                        new XElement("COLLECTORREMARK", emp.COLLECTORREMARK),
                                                        new XElement("FROMDATE", emp.FROMDATE),
                                                        new XElement("TODATE", emp.TODATE),

                                                        new XElement("FREQUENCYNO", emp.FREQUENCYNO),
                                                        new XElement("FREQUENCYVALUE", emp.FREQUENCYVALUE),
                                                        new XElement("CREATEDBY", emp.CREATEDBY)

                                                    ));
            _bdodataRepository.BDOTempDataSubmission(xEle, Status, (Int32)UserId);

            var xEle1 = new XElement("person",
                                         from emp in dpListObjFromForm
                                         select new XElement("row",

                                                        new XElement("BDODATAID", emp.BDODATAID),
                                                        new XElement("DATAPOINTVALUE", emp.DATAPOINTVALUE),
                                                        new XElement("COLLECTORREMARK", emp.COLLECTORREMARK),
                                                        new XElement("FROMDATE", emp.FROMDATE),
                                                        new XElement("TODATE", emp.TODATE),
                                                        new XElement("CREATEDBY", emp.CREATEDBY)

                                                    ));
            _collectordataRepository.AddCollectorData(xEle1, Status, (Int32)UserId, dpListObjFromForm[0].COLLECTORREMARK);

        }
        [HttpGet]
        public ActionResult GetDatapoints(int sectorid, int frequency, int Status)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<ABP.Domain.Indicator.Indicator> BDODataPoints = ViewBag.Result = _bdodataRepository.Getdatapoints(sectorid, frequency, Status).Result;//getting all data points to show in the grid
                return PartialView("_DisplayDataNamesPartial");

                //return Ok(JsonConvert.SerializeObject(BDODataPoints));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult ViewCollectorIndicator()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Indicator = _indicatorRepository.ViewIndicatorMapping("VIEW", 0, 0).Result;
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                ViewBag.yr = 0;
                ViewBag.Result = _collectordataindicatorRepository.GetAllCollectorIndicator((int)LeveDetailId);//getting all data points to show in the grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewCollectorIndicator(Indicator indicator)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.yr = indicator.YEAR;
                ViewBag.Indicator = _indicatorRepository.ViewIndicatorMapping("VIEW", 0, 0).Result;
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _collectordataindicatorRepository.GetAllSearchCollectordata(indicator.INDICATORID, indicator.FREQUENCYNO, indicator.YEAR, (int)LeveDetailId);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }
        [HttpGet]
        public IActionResult SearchCollectorIndicator(int IndicatorId, int Year, int FrequencyId, string FrequencyValue, string FromDate, string ToDate)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                if (FromDate == "0")
                {
                    FromDate = "1-JAN-1990";
                }
                else if (ToDate == "0")
                {
                    ToDate = "1-JAN-1990";
                }
                else { }
                List<Indicator> ColInd = ViewBag.Result = _collectordataindicatorRepository.SearchCollectorIndicator((int)LeveDetailId, IndicatorId, Year, FrequencyId, FrequencyValue, FromDate, ToDate).Result;//getting all data points to show in the grid
                return Ok(JsonConvert.SerializeObject(ColInd));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult ViewCompositeScore()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.District = _districtRepository.GetDistrictAsync().Result;//To bind drop down initial
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                ViewBag.Block = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId)).Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                CompositeScore score = new CompositeScore();
                score.DISTRICT_CODE = Convert.ToInt32(LeveDetailId);
                ViewBag.Result = _collectordataindicatorRepository.GetCompositeScore(score);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult ViewCompositeScore(CompositeScore score)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.District = _districtRepository.GetDistrictAsync().Result;//To bind drop down initial
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                ViewBag.Block = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId)).Result;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                score.DISTRICT_CODE = Convert.ToInt32(LeveDetailId);
                ViewBag.Result = _collectordataindicatorRepository.GetCompositeScore(score);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetBlockByDistId(string DistId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<Block> blocks = ViewBag.Block = _blockRepository.GetBlockByDistId(Convert.ToInt32(DistId)).Result;
                return Ok(JsonConvert.SerializeObject(blocks));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetLoggedInDist()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                return Ok(JsonConvert.SerializeObject(LeveDetailId));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public string GetDataEntryDate(int BlockId, int SectorId, int FreqId)
        {
            string InstallationDate = string.Empty;
            string LastUpdatedFreq = _bdodataRepository.GetLastFreq(BlockId, SectorId, FreqId);
            if (LastUpdatedFreq != null)
            {
                InstallationDate = Convert.ToDateTime(LastUpdatedFreq).ToString("dd-MMM-yyyy");
            }
            else
            {
                InstallationDate = Convert.ToDateTime(Configuration.GetValue<string>("MySettings:InstallationDate")).AddMonths(-1).ToString("dd-MMM-yyyy");
            }
            return InstallationDate;
        }
        [HttpGet]
        public async Task<JsonResult> GetCollectorDataAlert()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {

                    var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                    var Collectordatalist = await _bdodataRepository.GetCollecterAlertData((int)LeveDetailId);
                    return Json(Collectordatalist);


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

    }

}
