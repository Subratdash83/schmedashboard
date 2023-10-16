using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using ABP.Domain.BDOData;
using ABP.Domain.Block;
using ABP.Domain.DataPoint;
using ABP.Domain.Indicator;
using ABP.Domain.Login.OTPTracker;
using ABP.Infrastructure;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Contract.BDOData;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace ABP.Web.Controllers
{
    public class BDODataController : Controller
    {
        private readonly IDataPointRepository _datapointRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IBDODataRepository _bdodataRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IPanelRepository _IPanelRepository;
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        private readonly ISendSMSRepository _sms;
        public BDODataController(IHostingEnvironment hostingEnvironment, IDataPointRepository datapointRepository, ISectorRepository sectorRepository, IBDODataRepository bdodataRepository, IConfiguration configuration, IBlockRepository blockRepository, ISendSMSRepository sms,IPanelRepository PanelRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _datapointRepository = datapointRepository;
            _sectorRepository = sectorRepository;
            _bdodataRepository = bdodataRepository;
            Configuration = configuration;
            _blockRepository = blockRepository;
            _IPanelRepository = PanelRepository;
            _sms = sms;
            Log.Information("BDODataController initialized");
        }
        public IActionResult AddBDOData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _sectorRepository.ViewSector().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult BDODataEntry(int? FreqId = 2)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                ViewBag.Sector = _IPanelRepository.ViewPanel();
                DataPoint dp = new DataPoint();
                dp.FREQUENCYID = (int)FreqId;
                List<DataPoint> dps = ViewBag.Results = _datapointRepository.GetDataPointBySectorAndFrequency(dp).Result;
                foreach (DataPoint dpss in dps)
                {
                    string date = GetDataEntryDate(dpss.SECTORID, (int)FreqId);
                    if (FreqId == 2)
                    {
                        var thisMonthStart = Convert.ToDateTime(date).AddDays(1 - Convert.ToDateTime(date).Day);
                        var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        dpss.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy");
                        dpss.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy");
                    }
                    else if (FreqId == 3)
                    {
                        var currentQuater = (Convert.ToDateTime(date).Month - 1) / 3 + 1;
                        var daysInLastMonthOfQuarter = DateTime.DaysInMonth(Convert.ToDateTime(date).Year, 3 * currentQuater);
                        dpss.FROMDATE = new DateTime(Convert.ToDateTime(date).Year, 3 * currentQuater - 2, 1).ToString("dd-MMM-yyyy");
                        dpss.TODATE = new DateTime(Convert.ToDateTime(date).Year, 3 * currentQuater, daysInLastMonthOfQuarter).ToString("dd-MMM-yyyy");

                    }
                    else if (FreqId == 4)
                    {
                        var date1 = Convert.ToDateTime(date).AddYears(-1);
                        var month = date1.Month;
                        var year = date1.Year;
                        if (month <= 6)
                        {
                            var thisMonthStart = new DateTime(year, 1, 1);
                            var thisMonthEnd = new DateTime(year, 6, 30);
                            dpss.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy");
                            dpss.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            var thisMonthStart = new DateTime(year, 7, 1);
                            var thisMonthEnd = new DateTime(year, 12, 31);
                            dpss.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy");
                            dpss.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy");
                        }
                    }
                    else
                    {
                        int year = Convert.ToDateTime(date).Year - 1;
                        var thisMonthStart = new DateTime(year, 1, 1);
                        var thisMonthEnd = new DateTime(year, 12, 31);
                        dpss.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy");
                        dpss.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy");
                    }
                    dpss.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(dpss.FROMDATE));
                    dpss.DataEntryEligibility = GetEligibiltiy((int)FreqId, Convert.ToDateTime(date));
                }
                ViewBag.Result = dps;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public bool GetEligibiltiy(int FreqId, DateTime idate)
        {
            if (FreqId == 2)
            {
                var checkcount = _bdodataRepository.FRESHDATACOUNT(FreqId,idate.Month - 1);
                var data = checkcount.Result;
                if (data==0)
                {
                    return true;
                }
                else
                {
                    var ApproveDataCount = _bdodataRepository.MONTHDATACOUNT(FreqId, idate.Month - 1).Result; 
                    if(ApproveDataCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (FreqId == 3)
            {
                if (idate.Month == 4 || idate.Month == 7 || idate.Month == 10 || idate.Month == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (FreqId == 4)
            {
                if (idate.Month == 7 || idate.Month == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (idate.Month == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public string GetFrequencyValueByFreqId(int? FreqId, DateTime date)
        {
            string Freq = string.Empty;
            if (FreqId == 2)
            {
                Freq = date.ToString("MMM");
            }
            else if (FreqId == 3)
            {
                if (date.Month >= 4 && date.Month <= 6)
                    Freq = "Q1";
                else if (date.Month >= 7 && date.Month <= 9)
                    Freq = "Q2";
                else if (date.Month >= 10 && date.Month <= 12)
                    Freq = "Q3";
                else
                    Freq = "Q4";
            }
            else if (FreqId == 4)
            {
                if (date.Month >= 4 && date.Month <= 9)
                    Freq = "H1";
                else
                    Freq = "H2";
            }
            else
            {
                Freq = date.ToString("yyyy");
            }
            return Freq;
        }
        public IActionResult EditBDOData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Sector = _sectorRepository.ViewSector().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult ViewBDOData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");

                ViewBag.Sector = _sectorRepository.ViewSector().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _bdodataRepository.GetAllBDOData((int)LeveDetailId, (int)UserId);//getting all data points to show in the grid

                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewBDOData(BDOData bDOData)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //var ACTION = "BDO";
                ViewBag.Sector = _sectorRepository.ViewSector().Result;//Binding Initials
                ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.Result = _bdodataRepository.GetAllSearchbdodata(bDOData.SECTORID, bDOData.FREQUENCY, bDOData.YEAR, (int)LeveDetailId, (int)UserId);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }
        ////////////////for some checking purpose.
        //[HttpGet]
        //public IActionResult GetAllBDOData()
        //{
        //    var UserId = HttpContext.Session.GetInt32("_UserId");
        //    if (!string.IsNullOrEmpty(UserId.ToString()))
        //    {
        //        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
        //        List<DataPoint> BDODataPoints = ViewBag.Result = _bdodataRepository.GetAllBDOData((int)LeveDetailId).Result;//getting all data points to show in the grid
        //        return Ok(JsonConvert.SerializeObject(BDODataPoints));
        //    }
        //    else
        //    {
        //        return RedirectToAction("Logout", "Home");
        //    }
        //}
        [HttpGet]
        public IActionResult SearchBDOData(int SectorId, int Year, int FrequencyId, string FrequencyValue, string FromDate, string ToDate)
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
                List<DataPoint> BDODataPoints = ViewBag.Result = _bdodataRepository.SearchBDOData((int)LeveDetailId, SectorId, Year, FrequencyId, FrequencyValue, FromDate, ToDate).Result;//getting all data points to show in the grid
                return Ok(JsonConvert.SerializeObject(BDODataPoints));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetDataPointBySectorAndFrequency(string SECTORID, string FREQUENCYID)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                DataPoint dp = new DataPoint();
                dp.SECTORID = Convert.ToInt32(SECTORID);
                dp.FREQUENCYID = Convert.ToInt32(FREQUENCYID);
                var datapoint = _datapointRepository.GetDataPointBySectorAndFrequency(dp).Result;
                return Ok(JsonConvert.SerializeObject(datapoint));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult AddBDOData([FromBody] List<DataPoint> dpListObj)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            string retMsg = "";
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                bool isvalid = true;
                if (dpListObj != null && dpListObj.Count > 0)
                {
                    foreach (DataPoint dp in dpListObj.GroupBy(x => x.INDICATORID).Select(x => x.FirstOrDefault()))
                    {

                        IndicatorMapping im = _bdodataRepository.GetDataPointID(dp.INDICATORID).Result.FirstOrDefault();
                        decimal ndpvalue = 0;
                        decimal ddpvalue = 0;
                        ndpvalue = dpListObj.Where(u => u.DATAPOINTID == im.NDATAPOINTID).FirstOrDefault().DATAPOINTVALUE;
                        ddpvalue = dpListObj.Where(u => u.DATAPOINTID == im.DDATAPOINTID).FirstOrDefault().DATAPOINTVALUE;
                        if (ndpvalue > ddpvalue)
                        {
                            isvalid = false;
                        }
                    }
                    if (isvalid == false)
                    {
                        return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                    }
                    else
                    {
                        var results = dpListObj.GroupBy(
                        p => p.SECTORID,
                        (key, g) => new { SECTORID = key, data = g.ToList() });
                        var dataBySectorId = results.ToLookup(p => p.SECTORID, p => p.data);
                        //var carsForPerson = carsByPersonId[52];
                        List<DataPoint> Masterdplist = new List<DataPoint>();
                        foreach (var datas in dataBySectorId)
                        {
                            DataPoint dps = new DataPoint();
                            dps.SECTORID = datas.Key;
                            Masterdplist.Add(dps);
                        }
                        var xElem = new XElement("person",
                        from dp in Masterdplist
                        select new XElement("row",
                        new XElement("SECTORID", dp.SECTORID)
                       ));


                        var xEle = new XElement("person",
                                from dp in dpListObj
                                select new XElement("row",
                                               new XElement("DATAPOINTID", dp.DATAPOINTID),
                                               new XElement("DATAPOINTVALUE", dp.DATAPOINTVALUE),
                                               new XElement("INDICATORID", dp.INDICATORID),
                                                new XElement("BDODATAID", dp.BDODATAID),

                                           new XElement("SECTORID", dp.SECTORID)
                                           //new XElement("FROMDATE", dp.FROMDATE),
                                           //new XElement("TODATE", dp.TODATE),
                                           //new XElement("YEAR", dp.TODATE.Split('-')[2]),
                                           //new XElement("FREQUENCYVALUE", dp.FREQUENCYVALUE),
                                           //new XElement("FREQUENCYNO", GetFrequencyValueByFreq(dp.FREQUENCYVALUE))
                                           ));

                        retMsg = _bdodataRepository.AddBDOData(xElem, xEle, dpListObj[0], (int)LeveDetailId, GetFrequencyValueByFreq(dpListObj[0].FREQUENCYVALUE), (Int32)UserId);

                        if (retMsg == "1")
                        {
                            return Json(new { status = 1, msg = "Records Saved To Draft Successfully!" });
                        }
                        else if (retMsg == "2")
                        {
                            return Json(new { status = 2, msg = "Records Updated Successfully!" });
                        }
                        else if (retMsg == "3")
                        {
                            return Json(new { status = 2, msg = "Records Deleted Successfully!" });
                        }
                        else if (retMsg == "5")
                        {
                            return Json(new { status = 1, msg = "Please submit the records in draft!" });
                        }
                        else
                        {
                            return Json(new { status = 2, msg = "Records already exists for the same duration!" });
                        }
                    }
                }
                else
                {
                    return Json("Value can not be null!");
                }
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered!");
            }
        }
        [HttpPost]
        public IActionResult BDODataSubmission([FromBody] List<DataPoint> dpListObj)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //var prevmonth = DateTime.Now.AddMonths(-1);
                //var startDate = new DateTime(prevmonth.Year, prevmonth.Month, 1);
                //var endDate = startDate.AddMonths(1).AddDays(-1);
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                bool isvalid = true;
                foreach (DataPoint dp in dpListObj.GroupBy(x => x.INDICATORID).Select(x => x.FirstOrDefault()))
                {

                    IndicatorMapping im = _bdodataRepository.GetDataPointID(dp.INDICATORID).Result.FirstOrDefault();
                    decimal ndpvalue = 0;
                    decimal ddpvalue = 0;
                    ndpvalue = dpListObj.Where(u => u.DATAPOINTID == im.NDATAPOINTID).FirstOrDefault().DATAPOINTVALUE;
                    ddpvalue = dpListObj.Where(u => u.DATAPOINTID == im.DDATAPOINTID).FirstOrDefault().DATAPOINTVALUE;
                    if (ndpvalue > ddpvalue)
                    {
                        isvalid = false;
                    }
                }
                if (isvalid == false)
                {
                    return Json("Numerator cannot be Greater Then Denominator ");
                }
                else
                {
                    List<Domain.Block.Block> BlockDetails = ViewBag.BlockDetails = _blockRepository.GetBlockDetailsByBlockId(Convert.ToInt32(LeveDetailId)).Result;
                    var prevmonth = DateTime.Now.AddMonths(-1);
                    var startDate = new DateTime(prevmonth.Year, prevmonth.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var prevstartdate = startDate.ToString("dd-MMM-yy");
                    var prevenddate = endDate.ToString("dd-MMM-yy");
                    var xEle = new XElement("person",
                                          from dp in dpListObj
                                          select new XElement("row",
                                                       new XElement("BDODATAID", dp.BDODATAID),
                                                       new XElement("DATAPOINTID", dp.DATAPOINTID),
                                                       new XElement("DATAPOINTVALUE", dp.DATAPOINTVALUE),
                                                       new XElement("INDICATORID", dp.INDICATORID),
                                                       new XElement("SECTORID", dp.SECTORID),
                                                       new XElement("BLOCKID", dp.BLOCKID),
                                                       new XElement("FREQUENCYNO", dp.FREQUENCYNO),
                                                       new XElement("YEAR", dp.YEAR),
                                                       new XElement("FROMDATE", dp.FROMDATE),
                                                       new XElement("TODATE", dp.TODATE),
                                                       new XElement("FREQUENCYVALUE", dp.FREQUENCYVALUE),
                                                       new XElement("FREQUENCYID", dp.FREQUENCYID),
                                                        new XElement("INDICATORMAPPINGID", dp.INDICATORMAPPINGID),
                                                        new XElement("DATAID", dp.DATAID)
                                                     ));
                    var checkcount = _bdodataRepository.BDODATACOUNT(dpListObj[0].FREQUENCYNO);
                    var data = checkcount.Result;
                    string RetVal = string.Empty;
                    if (data == 0)
                    {
                        RetVal = _bdodataRepository.NEWBDODataSubmission(dpListObj[0].FREQUENCYNO, prevstartdate, prevenddate, xEle);
                    }
                    else
                    {
                        RetVal = _bdodataRepository.BDODataSubmission(xEle, dpListObj[0].FREQUENCYNO, dpListObj[0].FREQUENCYID, (Int32)UserId, prevstartdate, prevenddate);
                    }

                    if (RetVal == "1")
                    {
                        #region Text Message
                        string mobno = "9437307915";
                        var mobilenumber = Convert.ToInt64(mobno);
                        var template = "Dear User, ABP data for " + BlockDetails[0].BLOCK_NAME + " block has been submitted for the month " + dpListObj[0].MONTHNAME + " for your kind approval by 15th. Odisha State Dashboard.";
                        var templateid = "1007654211165182804";
                        var smsresult = _sms.Sendsms(mobno, template, templateid);
                        TempData["done"] = smsresult.Result;
                        //"Reminder Sent Successfully";
                        //return Json(smsresult.Result);
                        return Json("Record(s) Submitted Successfully");
                        #endregion
                    }
                    else
                    {
                        return Json("2");
                    }
                }
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered");
            }
        }
        public DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        [HttpGet]
        public IActionResult BDODataSubmission()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                List<DataPoint> DataPoints = ViewBag.Result = _datapointRepository.DPGroupBySector(0, Convert.ToInt32(LeveDetailId)).Result;
                ViewBag.Sector = _sectorRepository.ViewSector();
                List<int> Stat = DataPoints.Select(u => u.STATUS).Distinct().ToList();
                if (Stat.Contains(0) || Stat.Contains(3))
                {
                    ViewBag.Status = 0;
                }
                else
                {
                    ViewBag.Status = 1;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult BDODataIdGateById(string SECTORID, string frequencyvalue, string Year)
        {


            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //Getting DataPoint By Id To Bind Inputs For Update Purpose
                var datapoint = _bdodataRepository.BDODataIdGetById(Convert.ToInt32(SECTORID), frequencyvalue, Convert.ToInt32(Year)).Result;
                return Ok(JsonConvert.SerializeObject(datapoint));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public int GetFrequencyValueByFreq(string Freq)
        {
            try
            {
                return Convert.ToInt32(Freq);
            }
            catch
            {
                if (Freq == "Jan")
                {
                    return 1;
                }
                else if (Freq == "Feb")
                {
                    return 2;
                }
                else if (Freq == "Mar")
                {
                    return 3;
                }
                else if (Freq == "Apr")
                {
                    return 4;
                }
                else if (Freq == "May")
                {
                    return 5;
                }
                else if (Freq == "Jun")
                {
                    return 6;
                }
                else if (Freq == "Jul")
                {
                    return 7;
                }
                else if (Freq == "Aug")
                {
                    return 8;
                }
                else if (Freq == "Sep")
                {
                    return 9;
                }
                else if (Freq == "Oct")
                {
                    return 10;
                }
                else if (Freq == "Nov")
                {
                    return 11;
                }
                else if (Freq == "Dec")
                {
                    return 12;
                }
                else if (Freq == "Q1")
                {
                    return 1;
                }
                else if (Freq == "Q2")
                {
                    return 2;
                }
                else if (Freq == "Q3")
                {
                    return 3;
                }
                else if (Freq == "Q4")
                {
                    return 4;
                }
                else if (Freq == "H1")
                {
                    return 1;
                }
                else if (Freq == "H2")
                {
                    return 2;
                }
                else if (System.DateTime.Now.Year.ToString() == Freq)
                {
                    return 1;
                }
                else if ((System.DateTime.Now.Year - 1).ToString() == Freq)
                {
                    return 2;
                }
                else if ((System.DateTime.Now.Year - 2).ToString() == Freq)
                {
                    return 3;
                }
                else if ((System.DateTime.Now.Year - 3).ToString() == Freq)
                {
                    return 4;
                }
                else
                { return 0; }
            }
        }
        [HttpGet]
        public IActionResult BDODataGateById(int ID)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                List<DataPoint> BDODataPoints = ViewBag.Result = _bdodataRepository.GetBDODataById(ID).Result;//getting all data points to show in the grid
                return Ok(JsonConvert.SerializeObject(BDODataPoints));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetAllDatapoints(int sectorid, int frequency, int Status)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<DataPoint> BDODataPoints = ViewBag.Result = _bdodataRepository.GetAlldatapoints(sectorid, frequency, Status).Result;//getting all data points to show in the grid
                                                                                                                                         // return PartialView("_DisplayDataNamesPartial");

                return Ok(JsonConvert.SerializeObject(BDODataPoints));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
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
        public IActionResult SendOTPToLoggedInUser()
        {
            Log.Information("SendOTPToLoggedInUser Started");
            int UserId = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId"));
            if (UserId != 0)
            {
                try
                {
                    int OTP = 0;
                    string MobileNo = HttpContext.Session.GetString("_MobileNo");
                    string InstanceId = Configuration.GetValue<string>("MySettings:InstanceId");
                    if (InstanceId == "1")
                    {
                        OTP = 1234;
                    }
                    else
                    {
                        Random rnd = new Random();
                        OTP = rnd.Next(1000, 9999);
                    }
                    #region OTPTracker
                    OTPTracker tracker = new OTPTracker();
                    tracker.MOBILENO = MobileNo;
                    tracker.OTP = OTP;
                    tracker.OTPTYPE = 1;
                    tracker.CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId"));
                    var feature = HttpContext.Features.Get<IHttpConnectionFeature>();
                    string IP = feature?.LocalIpAddress?.ToString();
                    if (IP == "::1")
                    {
                        tracker.IPADDRESS = "127.0.0.1";
                    }
                    else
                    {
                        tracker.IPADDRESS = IP;
                    }
                    var RetVal = _bdodataRepository.TrackOTP(tracker);
                    #endregion
                    if (RetVal == "1")
                    {
                        if (InstanceId != "1")
                        {
                            #region Text Message
                            string mobno = "9437745159";
                            var mobilenumber = Convert.ToInt64(mobno);
                            var template = "Dear Applicant, One Time Password (OTP) is " + OTP + " To Submit The Data. Odisha State Dashboard.";
                            var templateid = "1007654211165182804";
                            var smsresult = _sms.Sendsms(mobno, template, templateid);
                            TempData["done"] = smsresult.Result;
                            //"Reminder Sent Successfully";
                            //return Json(smsresult.Result);
                            //return Json("Record(s) Submitted Successfully");
                            #endregion
                            return Ok(OTP);
                        }
                        else
                        {
                            return Ok(OTP);
                        }
                    }
                    else
                    {
                        return Ok("0");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                    return Ok("0");
                }
            }
            else
            {
                return RedirectToAction("SessionExpired", "Home");
            }
        }
        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ReportBDOData(BDOData bDOData)
        //{
        //    ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
        //    ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
        //    var UserId = HttpContext.Session.GetInt32("_UserId");
        //    var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
        //    if (!string.IsNullOrEmpty(UserId.ToString()))
        //    {
        //        //var ACTION = "BDO";
        //        ViewBag.Sector = _sectorRepository.ViewSector().Result;//Binding Initials
        //        ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
        //        ViewBag.Result = _bdodataRepository.GetAllSearchbdodata(bDOData.SECTORID, bDOData.FREQUENCY, bDOData.YEAR, (int)LeveDetailId, (int)UserId);
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Logout", "Home");
        //    }

        //}
        public IActionResult GetProjectInstallationDate(int SectorId, int FreqId)
        {
            return Ok(JsonConvert.SerializeObject(GetDataEntryDate(SectorId, FreqId)));
        }
        public string GetDataEntryDate(int SectorId, int FreqId)
        {
            string InstallationDate = string.Empty;
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            string LastUpdatedFreq = _bdodataRepository.GetLastFreq((int)LeveDetailId, SectorId, FreqId);
            if (LastUpdatedFreq != null)
            {
                InstallationDate = Convert.ToDateTime(LastUpdatedFreq).AddMonths(1).ToString("dd-MMM-yyyy");
            }
            else
            {
                InstallationDate = Convert.ToDateTime(Configuration.GetValue<string>("MySettings:InstallationDate")).AddMonths(-1).ToString("dd-MMM-yyyy");
            }
            return InstallationDate;
        }

        [HttpPost]
        public IActionResult UpdateBDOData([FromBody] List<DataPoint> dpListObj)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            string retMsg = "";
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                bool isvalid = true;
                if (dpListObj != null && dpListObj.Count > 0)
                {
                    foreach (DataPoint dp in dpListObj.GroupBy(x => x.INDICATORID).Select(x => x.FirstOrDefault()))
                    {

                        IndicatorMapping im = _bdodataRepository.GetDataPointID(dp.INDICATORID).Result.FirstOrDefault();
                        decimal ndpvalue = 0;
                        decimal ddpvalue = 0;
                        ndpvalue = dpListObj.Where(u => u.DATAPOINTID == im.NDATAPOINTID).FirstOrDefault().DATAPOINTVALUE;
                        ddpvalue = dpListObj.Where(u => u.DATAPOINTID == im.DDATAPOINTID).FirstOrDefault().DATAPOINTVALUE;
                        if (ndpvalue > ddpvalue)
                        {
                            isvalid = false;
                        }
                    }
                    if (isvalid == false)
                    {
                        return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                    }
                    else
                    {
                        var results = dpListObj.GroupBy(
                        p => p.SECTORID,
                        (key, g) => new { SECTORID = key, data = g.ToList() });
                        var dataBySectorId = results.ToLookup(p => p.SECTORID, p => p.data);
                        //var carsForPerson = carsByPersonId[52];
                        List<DataPoint> Masterdplist = new List<DataPoint>();
                        foreach (var datas in dataBySectorId)
                        {
                            DataPoint dps = new DataPoint();
                            dps.SECTORID = datas.Key;
                            Masterdplist.Add(dps);
                        }
                        var xElem = new XElement("person",
                        from dp in Masterdplist
                        select new XElement("row",
                        new XElement("SECTORID", dp.SECTORID)
                       ));


                        var xEle = new XElement("person",
                                from dp in dpListObj
                                select new XElement("row",
                                               new XElement("DATAPOINTID", dp.DATAPOINTID),
                                               new XElement("DATAPOINTVALUE", dp.DATAPOINTVALUE),
                                               new XElement("INDICATORID", dp.INDICATORID),
                                                new XElement("BDODATAID", dp.BDODATAID),

                                           new XElement("SECTORID", dp.SECTORID)
                                           //new XElement("FROMDATE", dp.FROMDATE),
                                           //new XElement("TODATE", dp.TODATE),
                                           //new XElement("YEAR", dp.TODATE.Split('-')[2]),
                                           //new XElement("FREQUENCYVALUE", dp.FREQUENCYVALUE),
                                           //new XElement("FREQUENCYNO", GetFrequencyValueByFreq(dp.FREQUENCYVALUE))
                                           ));

                        retMsg = _bdodataRepository.UpdateBDOData(xElem, xEle, dpListObj[0], (int)LeveDetailId, GetFrequencyValueByFreq(dpListObj[0].FREQUENCYVALUE), (Int32)UserId);

                        if (retMsg == "1")
                        {
                            return Json(new { status = 1, msg = "Records Saved To Draft Successfully!" });
                        }
                        else if (retMsg == "2")
                        {
                            return Json(new { status = 2, msg = "Records Updated Successfully!" });
                        }
                        else if (retMsg == "3")
                        {
                            return Json(new { status = 2, msg = "Records Deleted Successfully!" });
                        }
                        else if (retMsg == "5")
                        {
                            return Json(new { status = 1, msg = "Please submit the records in draft!" });
                        }
                        else
                        {
                            return Json(new { status = 2, msg = "Records already exists for the same duration!" });
                        }
                    }
                }
                else
                {
                    return Json("Value can not be null!");
                }
            }
            else
            {
                RedirectToAction("Logout", "Home");
                return Json("Some Error Occuered!");
            }
        }

    }
}