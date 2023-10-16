using ABP.Domain.BlockData;
using ABP.Domain.Panel;
using ABP.Domain.Indicator;
using ABP.Infrastructure;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Panel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.CollectorData;
using ABP.Domain.DataPoint;
using ABP.Repository.Contract.Contract.Dashboard;
using Newtonsoft.Json;
using System.Xml.Linq;
using ABP.Domain.Login;
using ABP.Repository.Contract.Contract.Login;
using ABP.Domain.Report;
using ABP.Repository.District;
using ABP.Repository.Contract.Report;
using System.Text.RegularExpressions;

namespace ABP.Web.Controllers
{
    public class DistrictDataController : Controller
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IDataPointRepository _datapointRepository;
        private readonly IControlPanelRepository _controlpanelRepository;
        private readonly ICollectorDataRepository _CollectorDataRepository;
        private readonly IBlockDataRepository _bdodataRepository;
        private readonly IDistrictDataRepository _districtDataRepository;
        private readonly IBlockDataRepository _blockDataRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private IHostingEnvironment _hostingEnvironment;
        private readonly ISendSMSRepository _sms;
        private readonly ILoginRepository _loginRepository;
        private readonly IReportRepository _reportRepository;
        public IConfiguration Configuration { get; }
        public DistrictDataController(IHostingEnvironment hostingEnvironment, ICollectorDataRepository CollectorDataRepository, IDistrictRepository distrcitRepository, IBlockRepository blockRepository, IIndicatorRepository indicatorRepository,
        IPanelRepository panelRepository, IDataPointRepository datapointRepository, IControlPanelRepository controlpanelRepository, IBlockDataRepository blockdataRepository, IConfiguration configuration, ISendSMSRepository sms, IDistrictDataRepository districtDataRepository, IBlockDataRepository blockDataRepository, IDashboardRepository dashboardRepository, ILoginRepository loginRepository, IReportRepository reportRepository)
        {
            _CollectorDataRepository = CollectorDataRepository;
            _hostingEnvironment = hostingEnvironment;
            _indicatorRepository = indicatorRepository;
            _districtRepository = distrcitRepository;
            _datapointRepository = datapointRepository;
            _blockRepository = blockRepository;
            _bdodataRepository = blockdataRepository;
            Configuration = configuration;
            _panelRepository = panelRepository;
            _loginRepository = loginRepository;
            _panelRepository = panelRepository;
            _reportRepository = reportRepository;
            _districtDataRepository = districtDataRepository;
            _controlpanelRepository = controlpanelRepository;
            _blockDataRepository = blockDataRepository;
            _dashboardRepository = dashboardRepository;

            _sms = sms;
            Log.Information("DistrictDataController initialized");
        }
      
        public IActionResult Pending(int BlockId, string AppNo)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                if (BlockId == 0)
                {
                    var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                    var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                    ViewBag.Block = blocks;
                    ViewBag.Sector = null;
                    ViewBag.BlockId = BlockId;
                    ViewBag.IndicatorResult = null;
                    ViewBag.DataPointResult = null;
                    ViewBag.Year = null;
                }
                else
                {
                    var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                    BlockData bd = new BlockData();
                    bd.BLOCKID = Convert.ToInt32(BlockId);
                    bd.STATUS = 2;
                    bd.APPLICATIONNO = AppNo;
                    string ApplicNo = AppNo;
                    ViewBag.RejectedSts = _districtDataRepository.GetRejectSectorCount(ApplicNo);
                    BlockData blockData = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    if (blockData != null)
                    {
                        List<ControlPanel> DataPoints = new List<ControlPanel>();
                        List<ControlPanel> DataPointsNew = null;
                        List<Indicator> Indicators = _districtDataRepository.GetAllIndicatorsForApproval(blockData).Result.ToList();
                        int monthno = (DateTime.Now.Month) - 1;
                        foreach (var cv in Indicators)
                        {

                            DataPointsNew = new List<ControlPanel>();
                            if (blockData.FREQUENCYID==5)
                            {
                                if (cv.IndicatorFormulaName != null && cv.IndicatorFormulaName.Contains("[condition1]"))
                                {
                                    if (cv.INDICATORID == 267)
                                    {
                                        int valtest = cv.INDICATORID;
                                    }
                                    //string formuladp = cv.IndicatorFormulaName;
                                    //string pattern = @"\bCONTROL_\w+\b"; // Regular expression pattern to match 'CONTROL_' followed by word characters

                                    //MatchCollection matches = Regex.Matches(formuladp, pattern);
                                    //string yrdp = cv.SelectedDataPoints;
                                    //string resyrdp = yrdp.Substring(0, yrdp.Length - 1);
                                    // Condition for the numerator
                                    string numeratorCondition = " WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND YEAR = " + blockData.YEAR + " AND DPMONTH = " + blockData.DPMONTH + " AND BLOCKID = " + blockData.BLOCKID + " and status in(1,2)) ORDER BY ID DESC)";
                                    // Condition for the denominator
                                    string denominatorCondition = "  WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + blockData.DPMONTH + "-" + blockData.YEAR + "', 'mm-yyyy') AND BLOCKID = " + blockData.BLOCKID + " and status in(1,2)) ORDER BY ID DESC)";
                                    // Replace [condition] with the condition for the numerator
                                    string queryWithNumeratorCondition = cv.IndicatorFormulaName.Replace("[condition]", numeratorCondition);

                                    // Replace [condition1] with the condition for the denominator
                                    string finalQuery = queryWithNumeratorCondition.Replace("[condition1]", denominatorCondition);
                                    cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(finalQuery);
                                }
                                else
                                {
                                    if (cv.INDICATORID == 267)
                                    {
                                        int valtest = cv.INDICATORID;
                                    }

                                    cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", "  WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + blockData.DPMONTH + "-" + blockData.YEAR + "', 'mm-yyyy') AND BLOCKID = " + blockData.BLOCKID + " and status in(1,2)) ORDER BY ID DESC"));
                                    //cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=" + blockData.FREQUENCYID + " and year=" + blockData.YEAR + " and frequencyno=" + blockData.FREQUENCYNO + " and blockid=" + blockData.BLOCKID + ")"));
                                }

                                DataPointsNew = _districtDataRepository.GetAllDPByIndicator(cv);
                                DataPoints.AddRange(DataPointsNew);
                            }
                            else
                            {
                            
                                if (cv.IndicatorFormulaName != null && cv.IndicatorFormulaName.Contains("[condition1]"))
                            {
                                    if (cv.INDICATORID == 267)
                                    {
                                        int valtest = cv.INDICATORID;
                                    }
                                    string yrdp = cv.SelectedDataPoints;
                                    string resyrdp = yrdp.Substring(0, yrdp.Length - 1);                                  
                                    string numeratorCondition = " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=2 and year=" + blockData.YEAR + " and frequencyno=" + blockData.FREQUENCYNO + " and blockid=" + blockData.BLOCKID + " and status=2)";
                                    //Condition for the denominator
                                    //string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND YEAR <= "+ blockData.YEAR + " AND DPMONTH <= " + monthno + " AND BLOCKID = " + blockData.BLOCKID + ") ORDER BY ID DESC";
                                    string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + blockData.FREQUENCYNO + "-" + blockData.YEAR + "', 'mm-yyyy') AND BLOCKID = " + blockData.BLOCKID + " and status in(1,2)) ORDER BY ID DESC";
                                    // Replace [condition] with the condition for the numerator
                                    string queryWithNumeratorCondition = cv.IndicatorFormulaName.Replace("[condition]", numeratorCondition);
                                // Replace [condition1] with the condition for the denominator
                                string finalQuery = queryWithNumeratorCondition.Replace("[condition1]", denominatorCondition);
                                  cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(finalQuery);
                            }
                               else
                               {
                                    cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=" + blockData.FREQUENCYID + " and year=" + blockData.YEAR + " and frequencyno=" + blockData.FREQUENCYNO + " and blockid=" + blockData.BLOCKID + ")"));
                                }
                            DataPointsNew = _districtDataRepository.GetAllDPByIndicator(cv);
                            DataPoints.AddRange(DataPointsNew);
                            }

                        }
                        foreach (var cv in DataPoints)
                        {
                            if(cv.FREQUENCYID==2)
                            {
                                cv.CONTROLVALUE = _blockDataRepository.GetContolValuecol(cv, blockData.APPLICATIONNO, blockData.BLOCKID, blockData.YEAR, cv.FREQUENCYID, blockData.FREQUENCYNO);
                            }
                            else
                            {
                                //if(cv.CONTROLNAME== "CONTROL_92")
                                //{
                                //    string cnrlname = cv.CONTROLNAME;
                                //}
                                cv.CONTROLVALUE = _blockDataRepository.GetContolValueyear(cv, blockData.APPLICATIONNO, blockData.BLOCKID, blockData.YEAR, cv.FREQUENCYID, blockData.FREQUENCYNO);
                            }
                           
                        }
                        ViewBag.IndicatorResult = Indicators.Where(u => u.INDICATORVALUE != 0).ToList();
                        ViewBag.DPResult = DataPoints;
                        ViewBag.FROMDATE = blockData.FROMDATE;
                        ViewBag.TODATE = blockData.TODATE;
                        ViewBag.AppNo = blockData.APPLICATIONNO;
                        ViewBag.Year = blockData.YEAR;
                        if (blockData.FREQUENCYID == 5)
                        {
                            ViewBag.FrequencyNo = blockData.DPMONTH;
                        }
                        else
                        {
                            ViewBag.FrequencyNo = blockData.FREQUENCYNO;
                        }
                        ViewBag.FrequencyId = blockData.FREQUENCYID;
                        var blockid = blockData.BLOCKID;
                    }
                    var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                    ViewBag.Block = blocks;
                    ViewBag.BlockId = Convert.ToInt32(BlockId);
                }
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
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.BLOCKID = Convert.ToInt32(BlockId);
                bd.STATUS = 2;
                BlockData blockData = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                if (blockData != null)
                {
                    List<ControlPanel> DataPoints = new List<ControlPanel>();
                    List<ControlPanel> DataPointsNew = null;
                    List<Indicator> Indicators = _districtDataRepository.GetAllIndicatorsForApproval(blockData).Result.ToList();
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
                    ViewBag.FROMDATE = blockData.FROMDATE;
                    ViewBag.TODATE = blockData.TODATE;
                    ViewBag.AppNo = blockData.APPLICATIONNO;
                }
                var blocks = _blockRepository.GetBlockByDistId(Convert.ToInt32(LeveDetailId));
                ViewBag.Block = blocks;
                ViewBag.BlockId = Convert.ToInt32(BlockId);
                return View("Pending");
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
        [HttpPost]
        public IActionResult Accept(string ApplicationNo, string Remark, List<DataPoint> datap)
        {
            try
            {
                string InstallationDate = string.Empty;
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                var code = HttpContext.Session.GetInt32("CODE");
                BlockData bd = new BlockData();
                var data = HttpContext.Request.Form["Elements"];
                var result = JsonConvert.DeserializeObject<List<DataPoint>>(data);
                var xEle = new XElement("person",
                                                   from DataPoint in result
                                                   select new XElement("row",
                                                                 new XElement("SECTORID", DataPoint.SECTORID),
                                                                 new XElement("INDICATORID", DataPoint.INDICATORID),
                                                                 new XElement("INDICATORVALUE", DataPoint.INDICATORVALUE),
                                                                  new XElement("FREQUENCYID", DataPoint.FREQUENCYID),
                                                                  new XElement("YEAR", DataPoint.YEAR),
                                                                  new XElement("BLOCKID", DataPoint.BLOCKID),
                                                                  new XElement("MONTHNAME", DataPoint.MONTHNAME),
                                                                  new XElement("FREQUENCYNO", DataPoint.FREQUENCYNO)
                                                              ));
                //var value = _datapointRepository.Datadashboard(xEle, Convert.ToInt32(LeveDetailId), Convert.ToInt32(code));
                bd.APPLICATIONNO = ApplicationNo;
                bd.BLOCKID = result[0].BLOCKID;
                bd.FREQUENCYID = result[0].FREQUENCYID;
                bd.FREQUENCYNO = result[0].FREQUENCYNO;
                bd.YEAR = result[0].YEAR;
                string RetVal = _districtDataRepository.AcceptPendingIndicatorData(bd);

                if (RetVal == "1")
                {

                    Login objUserDetails = new Login();
                    objUserDetails.INTLEVELDETAILID = result[0].BLOCKID;
                    objUserDetails = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                    var mobno = objUserDetails.MOBILENO;
                    //string mobno = "9348301952";
                    if(mobno!= "Not Available")
                    {
                        var mobilenumber = Convert.ToInt64(mobno);
                        var template = "Dear User, ABP Data approved for the month " + result[0].MONTHNAME + " by dist. collector. Access portal https://abp.odisha.gov.in/ for further details. Odisha State Dashboard.";
                        var templateid = "1007168066714211943";
                        var smsresult = _sms.Sendsms(mobno, template, templateid);
                        TempData["done"] = smsresult.Result;
                    }
                  
                    //"Reminder Sent Successfully";
                    //return Json(smsresult.Result);
                    return Json("Data Accepted Successfully!");
                }
                else
                {
                    return Json("Some Error Occuered!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult Reject(string ApplicationNo, string Remark, int? SectorId, int? Year, int? BlockId, int? FrequencyId, int? FrequencyNo)
        {
            string RetVal = string.Empty;
            BlockData bd = new BlockData();
            bd.APPLICATIONNO = ApplicationNo;
            bd.YEAR = (int)Year;
            bd.BLOCKID = (int)BlockId;
            bd.FREQUENCYID = (int)FrequencyId;
            bd.FREQUENCYNO = (int)FrequencyNo;
            bd.REMARK = Remark;
           
            if (SectorId != null)
            {
                bd.PANELID = (int)SectorId;
                bd.STATUS = 3;
                RetVal = _districtDataRepository.RejectASectorData(bd);
                
               
            }
            else
            {
                RetVal = _districtDataRepository.RejectPendingIndicatorData(bd);
            }
            if (RetVal == "1")
            {
                ViewBag.RejectedSts = _districtDataRepository.GetRejectSectorCount(ApplicationNo);
                return Json("Data Rejected Successfully!");
            }
            else
            {
                return Json("Some Error Occuered!");
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
        #region collector view
        public IActionResult ApprovedData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                DataPoint dp = new DataPoint();
                dp.leveldetailedid = (int)LeveDetailId;
                dp.STATUS = 1;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _districtDataRepository.GetAllBlocksApprovedData(dp, "DP");
                ViewBag.Block = _blockRepository.GetBlockByDistId(dp.leveldetailedid).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        #endregion       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApprovedData(Indicator bDOData)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //var ACTION = "BDO";

                DataPoint dp = new DataPoint();
                dp.FREQUENCYID = bDOData.FREQUENCYID;
                dp.YEAR = bDOData.YEAR;
                dp.leveldetailedid = (int)LeveDetailId;
                dp.BLOCKID = bDOData.BLOCKID;
                dp.STATUS = 1;
                ViewBag.year = bDOData.YEAR;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _districtDataRepository.GetAllBlocksApprovedData(dp, "DP");
                ViewBag.Block = _blockRepository.GetBlockByDistId((int)LeveDetailId).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        //report collector data
        public IActionResult ReportCollectorData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                DataPoint dp = new DataPoint();
                dp.leveldetailedid = (int)LeveDetailId;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _districtDataRepository.GetAllCollectorData(dp);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult PendingDetails()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                
                
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult DataSync()
        {
            List<Indicator> indDataToBeInsert = new List<Indicator>();
            try
            {
                var blocks = _blockRepository.GetBlockAsync().Result.ToList();
                foreach (var block in blocks)
                {
                    BlockData bd = new BlockData();
                    bd.BLOCKID = Convert.ToInt32(block.BLOCKID);
                    bd.STATUS = 0;
                    BlockData blockData = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    if (blockData != null)
                    {
                        List<Indicator> Indicators = _districtDataRepository.GetAllIndicatorsForApproval(blockData).Result.ToList();
                        foreach (var cv in Indicators)
                        {
                            cv.DISTRICTID = block.DISTRICTID;
                            cv.BLOCKID = block.BLOCKID;
                            cv.DISTRICTCODE = block.DISTRICTCODE;
                            cv.BLOCKCODE = block.BLOCKCODE;
                            cv.FREQUENCYID = blockData.FREQUENCYID;
                            cv.FREQUENCYNO = blockData.FREQUENCYNO;
                            cv.FREQUENCYVALUE = blockData.FREQUENCYVALUE;
                            cv.YEAR = blockData.YEAR;
                            cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno='" + blockData.APPLICATIONNO + "'"));
                        }
                        indDataToBeInsert.AddRange(Indicators);
                    }
                }
                var xEle = new XElement("person",
                                                  from Indicator in indDataToBeInsert
                                                  select new XElement("row",
                                                                new XElement("SECTORID", Indicator.SECTORID),
                                                                new XElement("INDICATORID", Indicator.INDICATORID),
                                                                new XElement("INDICATORVALUE", Indicator.INDICATORVALUE),
                                                                new XElement("FREQUENCYID", Indicator.FREQUENCYID),
                                                                new XElement("FREQUENCYNO", Indicator.FREQUENCYNO),
                                                                new XElement("YEAR", Indicator.YEAR),
                                                                new XElement("BLOCKID", Indicator.BLOCKID),
                                                                new XElement("DISTRICTID", Indicator.DISTRICTID),
                                                                new XElement("BLOCKCODE", Indicator.BLOCKCODE),
                                                                new XElement("DISTRICTCODE", Indicator.DISTRICTCODE),
                                                                new XElement("FREQUENCYVALUE", Indicator.FREQUENCYVALUE)
                                                             ));
                var value = _datapointRepository.IndicatorDataInsertAutoApp(xEle);
                return Ok(1);
            }
            catch (Exception EX)
            {
                Log.Error("ERROR" + EX.Message);
                return Ok("2");
            }
        }


        #region Indicator Value Display
        public IActionResult ViewIndicatorValue()
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    ViewBag.Result = _districtDataRepository.Viewindicator();
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
        #endregion
        #region collector view
        public IActionResult CltrDashboard()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _districtRepository.GetBlockByDist(0).Result;
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
        public IActionResult CltrDashboard(Report Rpt)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                //ViewBag.RejectedResult = _dashboardRepository.GetRejectedDataDetails(leveldetailid, 2);
                ViewBag.DistrictData = _districtRepository.GetBlockByDist(0).Result;
                ViewBag.year = Rpt.Year;
                ViewBag.DatapointData = _reportRepository.GetDashboardDPData(Rpt);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        #endregion

        [HttpGet]
        public IActionResult ManageAspirationalBlock()

        {

            ViewBag.DistrictData = _districtDataRepository.GetDistrict(0).Result;

            return View();
        }

        [HttpPost]
        public IActionResult ManageAspirationalBlock(Report lr)
        {
            try
            {
                ViewBag.DistrictData = _districtDataRepository.GetDistrict(0).Result;
                var retMsg = _districtRepository.INSERTAspirationalDetail(lr).Result;
                if (retMsg == -1)
                {

                    return Json("Saved Successfully");

                }
                else
                {
                    return Json("Some Error in Save");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        public IActionResult ViewAspirationalBlock(Report rpt)
        {
            ViewBag.Result = _districtRepository.ViewAspirationalBlock(rpt);
            ViewBag.DistrictData = _districtDataRepository.GetDistrict(0).Result;
            return View();
        }
        [HttpGet]
        public IActionResult bindBlockById(int id)
        {
            int UserId = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId"));
            if (UserId != 0)
            {
                  var  dash1 =  _districtRepository.GetAspirationalid(id).Result;
                return Json(new { data = dash1 });
            }
            else
            {
                return RedirectToAction("SimpleLogout", "Login");
            }
        }

        [HttpGet]
        public IActionResult GetMappedBlocksByDist(string DistCode)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");//Getting Block By DistCode
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Blocks1 = _districtRepository.GetMappedeBlockByDist(Convert.ToInt32(DistCode)).Result;
                return Ok(JsonConvert.SerializeObject(Blocks1));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult MapBlock(string BlockData, int DistIdD,string BlockDataN)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            int retMsg = 0;
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Blocks1 = _districtRepository.GetMappedeBlockByDist(Convert.ToInt32(DistIdD)).Result;
                foreach(var b in Blocks1)
                {
                    if(BlockData.Split(',').ToList().Contains(b.BlockId.ToString()))
                    {
                        b.MAPPEDBLOCK = "true";
                    }
                    else
                    {
                        b.MAPPEDBLOCK = "false";
                    }
                     retMsg = _districtRepository.INSERTAspirationalDetail(b).Result;
                    
                }
                if (retMsg == -1)
                {

                    return Json("Saved Successfully");

                }
                else
                {
                    return Json("Some Error in Save");
                }

            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
