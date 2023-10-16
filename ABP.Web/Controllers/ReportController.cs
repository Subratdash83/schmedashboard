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
using ABP.Domain.Login;
using ABP.Repository.Contract.DistDetailReport;
using ABP.Domain.Indicator;
using Serilog.Core;
using Serilog;
using System.Xml.Linq;
using ABP.Domain.Frequency;

namespace ABP.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
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

        private readonly ILoginRepository _loginRepository;
        private readonly IDistDetailRepository _distDetailRepository;

        public ReportController(ILogger<ReportController> logger, ILoginRepository loginRepository, IDashboardRepository dashboardRepository, IBDODataRepository bdodataRepository, IDataPointRepository datapointRepository, IBlockRepository blockRepository, ISectorRepository sectorRepository, IHostingEnvironment hostingEnvironment, IPanelRepository panelRepository, IBlockDataRepository blockdataRepository, IDistrictDataRepository districtDataRepository, IIndicatorRepository indicatorRepository, IReportRepository reportRepository, IDistrictRepository districtRepository, IDistDetailRepository distDetailRepository)
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
            _distDetailRepository = distDetailRepository;
        }
        public IActionResult DistrictDetailReport(int Year, int FREQUENCYNO)
        {
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            int DistCode = Convert.ToInt32(HttpContext.Session.GetInt32("_distCode"));
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                // ViewBag.DistrictData = _DistrictRepository.GetDistrictAsync();
                // ViewBag.Blockdata = _DistrictRepository.GetBlockByDist(DistCode).Result;
                if (Year.ToString() == "0")
                {
                    Year = DateTime.Now.Year;
                }
                else
                {
                    Year = Year;
                }
                if (FREQUENCYNO == 0)
                {
                    FREQUENCYNO = DateTime.Now.Month - 1;
                }
                else
                {
                    FREQUENCYNO = Convert.ToInt32(FREQUENCYNO);
                }
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FREQUENCYNO;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.Year1 = Year;
                ViewBag.Month1 = FREQUENCYNO;
                ViewBag.BlockData = _DistrictRepository.GetByDistrictDataDetails(Year, FREQUENCYNO);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        //[HttpPost]
        //public IActionResult DistrictDetailReport(int Year, int FREQUENCYNO)
        //{
        //    var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
        //    BlockData data = new BlockData();
        //    var UserId = HttpContext.Session.GetInt32("_UserId");
        //    if (!string.IsNullOrEmpty(UserId.ToString()))
        //    {
        //        if (Year.ToString() == "0")
        //        {
        //            Year = DateTime.Now.Year;
        //        }
        //        else
        //        {
        //            Year = Year;
        //        }
        //        if (FREQUENCYNO == 0)
        //        {
        //            FREQUENCYNO = DateTime.Now.Month;
        //        }
        //        else
        //        {
        //            FREQUENCYNO = Convert.ToInt32(FREQUENCYNO);
        //        }
        //        data.BLOCKID = (int)LeveDetailId;
        //        data.FREQUENCYID = 5;
        //        data.YEAR = Year;
        //        data.FREQUENCYNO = (int)FREQUENCYNO;
        //        ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
        //        data.FREQUENCYID = 2;
        //        ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
        //        // ViewBag.DistrictData = _DistrictRepository.GetDistrictAsync();
        //        ViewBag.BlockData = _DistrictRepository.GetByDistrictDataDetails();
        //        //ViewBag.BlockData = _distDetailRepository.GetByDistrictDataDetails(distReport.DistrictId);

        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Logout", "Home");
        //    }
        //}

        //[HttpGet]
        //public JsonResult Blocklist(int DistCode)
        //{
        //    var Blocklist = _distDetailRepository.GetBlockByDistrict(DistCode).Result;

        //    return Json(Blocklist);

        //}

        public IActionResult BlockPerformance(int distcode, int Year, int FREQUENCYNO, int Status)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            int DistCode = Convert.ToInt32(HttpContext.Session.GetInt32("_distCode"));
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                if (Year.ToString() == "0")
                {
                    Year = DateTime.Now.Year;
                }
                else
                {
                    Year = Year;
                }
                if (FREQUENCYNO == 0)
                {
                    FREQUENCYNO = DateTime.Now.Month;
                }
                else
                {
                    FREQUENCYNO = Convert.ToInt32(FREQUENCYNO);
                }
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                // ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList(); /*_DistrictRepository.IndicatorScore(distcode, 0, 0/*Year,FREQUENCYNO*/); */
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.BlockDataPerformance = _DistrictRepository.getblockdata(distcode);
                ViewBag.IndicatorResult = _DistrictRepository.GetAvgIndicatorScore(distcode);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult BlockIndicatorScore(int distcode)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            int DistCode = Convert.ToInt32(HttpContext.Session.GetInt32("_distCode"));
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //ViewBag.BlockDataPerformance = _DistrictRepository.getblockdata(distcode);
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        
        public int IndicatorScoreCalculation(int distcode,int year, int mnth)
        {
            try
            {
                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                BlockData bd = new BlockData();
                bd.FREQUENCYID = 2;
                //
                //var result = _DistrictRepository.GetAppnoForCalculation(Convert.ToInt32(distcode)).Result;
                //foreach (var item in result)
                //{
                //}
                //
                List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                foreach (var block in Blocks)
                {

                        var i = mnth;
                        foreach (var cv in Indicators)
                        {
                            IndicatorValueScore isc = new IndicatorValueScore();
                            if (cv.IndicatorFormulaName != null)
                            {
                                int monthno = (DateTime.Now.Month) - 1;
                                if (cv.IndicatorFormulaName != null && cv.IndicatorFormulaName.Contains("[condition1]"))

                                {

                                    string numeratorCondition = " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=2 and year=" + year + " and frequencyno=" + mnth + " and blockid=" + block.BLOCK_CODE + " and status in(1,2))";
                                //Condition for the denominator
                                //string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND YEAR <= " + year + " AND DPMONTH <= " + monthno + " AND BLOCKID = " + block.BLOCK_CODE + " and status in(1,2)) ORDER BY ID DESC";
                                string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('"+ mnth + "-" + year + "', 'mm-yyyy') AND BLOCKID = " + block.BLOCK_CODE + " and status in(1, 2)) ORDER BY ID DESC";
                                // Replace [condition] with the condition for the numerator
                                string queryWithNumeratorCondition = cv.IndicatorFormulaName.Replace("[condition]", numeratorCondition);
                                    // Replace [condition1] with the condition for the denominator
                                    string finalQuery = queryWithNumeratorCondition.Replace("[condition1]", denominatorCondition);
                                    cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(finalQuery);
                                }
                                else
                                {
                                    cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=2 and year=" + year + " and frequencyno=" + mnth + " and blockid=" + block.BLOCK_CODE + " and status in(1,2))"));
                                }
                            }

                            cv.ApplicationNo = _indicatorRepository.GetAppValueThroughQuery("select applicationno from(select applicationno from t_abp_block_dataentry where frequencyid=2 and year <=" + year + " and frequencyno <=" + i + " and blockid=" + block.BLOCK_CODE + " and status in(1,2) order by id desc)where rownum=1");
                            isc.BLOCKID = block.BLOCK_CODE;
                            isc.FREQUENCYID = 2;
                            isc.FREQUENCYNO = i;
                            isc.YEAR = year;
                            isc.INDICATORID = cv.INDICATORID;
                            isc.PANELID = cv.SECTORID;
                            isc.INDICATORSCORE = cv.INDICATORVALUE;
                            isc.DISTID = distcode;
                            isc.APPLICATIONNO = cv.ApplicationNo;
                            if (cv.ApplicationNo == null)
                            {
                                isc.APPLICATIONNO = "nv";
                            }
                            isclist.Add(isc);
                        }

                }
                xEle.Add(new XElement("person",
                             from lan in isclist
                             select new XElement("row",
                                            new XElement("APPLICATIONNO", lan.APPLICATIONNO),
                                            new XElement("BLOCKID", lan.BLOCKID),
                                            new XElement("FREQUENCYID", lan.FREQUENCYID),
                                            new XElement("FREQUENCYNO", lan.FREQUENCYNO),
                                            new XElement("YEAR", lan.YEAR),
                                            new XElement("INDICATORID", lan.INDICATORID),
                                            new XElement("PANELID", lan.PANELID),
                                            new XElement("INDICATORSCORE", lan.INDICATORSCORE),
                                            new XElement("DISTID", lan.DISTID)
                                        )));
                IndicatorScoreXML isx = new IndicatorScoreXML();
                isx.indicatorvaluexml = xEle;
                int x = _DistrictRepository.INSERTSCORE(isx).Result;
                return 1;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        //Yearly calculation
        public int IndicatorScoreCalculationyearly(int distcode, int year,int mnth)
        {
            try
            {
                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                BlockData bd = new BlockData();
                bd.FREQUENCYID = 5;
                int monthno = (DateTime.Now.Month) - 1;
                List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                foreach (var block in Blocks)
                {
                  
                        foreach (var cv in Indicators)
                        {
                            IndicatorValueScore isc = new IndicatorValueScore();
                            if (cv.IndicatorFormulaName != null)
                            {
                            if (cv.INDICATORID == 270)
                            {
                                int indcatorid = cv.INDICATORID;
                            }
                            if (cv.IndicatorFormulaName != null && cv.IndicatorFormulaName.Contains("[condition1]"))
                                {
                                //int indid = cv.INDICATORID;
                                string numeratorCondition = " WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND YEAR = " + year + " AND DPMONTH <= " + mnth + " AND BLOCKID = " + block.BLOCK_CODE + " and status in(1,2)) ORDER BY ID DESC";
                                // Condition for the denominator
                                string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + mnth + "-" + year + "', 'mm-yyyy') AND BLOCKID = " + block.BLOCK_CODE + " and status in(1, 2)) ORDER BY ID DESC";                                
                                // Replace [condition] with the condition for the numerator
                                string queryWithNumeratorCondition = cv.IndicatorFormulaName.Replace("[condition]", numeratorCondition);
                                // Replace [condition1] with the condition for the denominator
                                string finalQuery = queryWithNumeratorCondition.Replace("[condition1]", denominatorCondition);
                                cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(finalQuery);
                                cv.ApplicationNo = _indicatorRepository.GetAppValueThroughQuery("select applicationno from(select applicationno from t_abp_block_dataentry where frequencyid=5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + mnth + "-" + year + "', 'mm-yyyy') and blockid=" + block.BLOCK_CODE + " order by id desc)where rownum=1");

                            }
                                else
                                {
                                    cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno in (select applicationno from t_abp_block_dataentry where frequencyid=5 and year=" + year + "  and blockid=" + block.BLOCK_CODE + " and status in(1,2) and dpmonth <=" + mnth + ")"));
                                cv.ApplicationNo = _indicatorRepository.GetAppValueThroughQuery("select applicationno from(select applicationno from t_abp_block_dataentry where frequencyid=5 and year =" + year + " and DPMONTH =" + mnth + " and blockid=" + block.BLOCK_CODE + " order by id desc)where rownum=1");
                            }
                            }
                            //cv.ApplicationNo = _indicatorRepository.GetAppValueThroughQuery("select applicationno from t_abp_block_dataentry where frequencyid=5 and year <=" + i + " AND DPMONTH <="+ mnth + " and blockid=" + block.BLOCK_CODE + "");
                        


                            isc.BLOCKID = block.BLOCK_CODE;
                            isc.FREQUENCYID = 5;
                            isc.FREQUENCYNO = year;
                            isc.YEAR = year;
                            isc.INDICATORID = cv.INDICATORID;
                            isc.PANELID = cv.SECTORID;
                            isc.INDICATORSCORE = cv.INDICATORVALUE;
                            isc.DISTID = distcode;
                            isc.APPLICATIONNO = cv.ApplicationNo;
                            isclist.Add(isc);
                        }
                   


                }
                xEle.Add(new XElement("person",
                             from lan in isclist
                             select new XElement("row",
                                            new XElement("APPLICATIONNO", lan.APPLICATIONNO),
                                            new XElement("BLOCKID", lan.BLOCKID),
                                            new XElement("FREQUENCYID", lan.FREQUENCYID),
                                            new XElement("FREQUENCYNO", lan.FREQUENCYNO),
                                            new XElement("YEAR", lan.YEAR),
                                            new XElement("INDICATORID", lan.INDICATORID),
                                            new XElement("PANELID", lan.PANELID),
                                            new XElement("INDICATORSCORE", lan.INDICATORSCORE),
                                            new XElement("DISTID", lan.DISTID)
                                        )));
                IndicatorScoreXML isx = new IndicatorScoreXML();
                isx.indicatorvaluexml = xEle;
                int x = _DistrictRepository.INSERTSCORE(isx).Result;
                return 1;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        
        public int Analyticport(int distcode, int freqid, int year, int mnth)
        {
            try
            {
                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                BlockData bd = new BlockData();
                bd.FREQUENCYID = freqid;

                // List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                foreach (var block in Blocks)
                {
                    if (freqid == 2)
                    {
                        int x = _DistrictRepository.INSERTAnalyticData(distcode, block.BLOCK_CODE, year, mnth, freqid).Result;

                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        public int AnalyticportCopy(int distcode, int freqid, int year, int mnth)
        {
            try
            {
                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                BlockData bd = new BlockData();
                bd.FREQUENCYID = freqid;

                // List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                foreach (var block in Blocks)
                {
                    if (freqid == 2)
                    {
                        int x = _DistrictRepository.INSERTAnalyticDataCopy(distcode, block.BLOCK_CODE, year, mnth, freqid).Result;

                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        public int AnalyticportYearly(int distcode, int freqid)
        {
            try
            {
                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                BlockData bd = new BlockData();
                bd.FREQUENCYID = freqid;

                //  List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                foreach (var block in Blocks)
                {
                    if (freqid == 5)
                    {
                        for (var i = 2021; i <= 2023; i++)
                        {

                            int x = _DistrictRepository.INSERTAnalyticDataYearly(distcode, block.BLOCK_CODE, i, freqid).Result;
                        }

                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        public int AnalyticportCopyYearly(int distcode, int freqid, int year, int mnth)
        {
            try
            {
                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                BlockData bd = new BlockData();
                bd.FREQUENCYID = freqid;

                // List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                foreach (var block in Blocks)
                {
                    if (freqid == 5)
                    {
                        int x = _DistrictRepository.INSERTAnalyticDataCopyYearly(distcode, block.BLOCK_CODE, year, mnth, freqid).Result;

                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        public IActionResult EnteredBlockReport()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                Report lr = new Report();
                if (lr.FromDate == "" || lr.FromDate == null)
                {
                    lr.FromDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }

                if (lr.ToDate == "" || lr.ToDate == null)
                {
                    lr.ToDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                //ViewBag.Loginreport = _loginRepository.GetLoginLogDetails(lr.FromDate, lr.ToDate);
                //ViewBag.Frequency = _datapointRepository.ViewFrequency().Result;
                ViewBag.FromDate = lr.FromDate;
                ViewBag.TODATE = lr.ToDate;
                ViewBag.statusyes = 0;
                ViewBag.statusno = 1;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult EnteredBlockReport(Report lr)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                if (lr.FromDate == "" || lr.FromDate == null)
                {
                    lr.FromDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }

                if (lr.ToDate == "" || lr.ToDate == null)
                {
                    lr.ToDate = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                //ViewBag.Loginreport = _loginRepository.GetLoginLogDetails(lr.FromDate, lr.ToDate);
                ViewBag.Loginreport = _loginRepository.GetBlockEnteredDetails(lr.FromDate, lr.ToDate, lr.status);
                ViewBag.FromDate = lr.FromDate;
                ViewBag.TODATE = lr.ToDate;
                if (lr.status == "0")
                {
                    ViewBag.statusyes = lr.status;
                    ViewBag.statusno = 1;
                }
                else
                {
                    ViewBag.statusyes = 0;
                    ViewBag.statusno = lr.status;
                }

                return View(lr);
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        public IActionResult NotCalculatedIndicatorReport()
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
                // ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;
                DataPoint dp = new DataPoint();
                ViewBag.Result = null;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
     
        public IActionResult SectorWiseEntryPerformance() 
        {
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            string currentYear = DateTime.Now.Year.ToString();
            // ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
            data.FREQUENCYID = 5;
            ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList().OrderByDescending(r => r.FREQUENCYVALUE);
            SectorPRFData Rpt = new SectorPRFData();
            ViewBag.month = DateTime.Now.Month - 1;
            ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
            return View();
        }
        [HttpPost]
        public IActionResult SectorWiseEntryPerformance(SectorPRFData Rpt)
        {
            BlockData data = new BlockData();
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {

                data.FREQUENCYID = 5;
                ViewBag.Years = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.bid = Rpt.BLOCKID;
                ViewBag.District = Rpt.DISTRICTID;
                ViewBag.year = Rpt.Year;
                ViewBag.month = DateTime.Now.Month - 1;
                //ViewBag.bid = Rpt.BLOCKID;
                ViewBag.SectorDpCountData = _reportRepository.GetSectorEntryPrfData(Rpt);
                ViewBag.SectorTtlData = _reportRepository.GetSectorEntryTotalPrfData(Rpt);
                ViewBag.SectorTotlData = _reportRepository.GetSectorEntryYearTotal(Rpt);
                
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        
    }
}
