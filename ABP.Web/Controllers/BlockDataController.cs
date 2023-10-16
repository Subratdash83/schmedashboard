using ABP.Domain.BlockData;
using ABP.Domain.Indicator;
using ABP.Domain.Login;
using ABP.Domain.Panel;
using ABP.Domain.Report;
using ABP.Infrastructure;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Login;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.SMS;
using ABP.Repository.Contract.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Web.Controllers
{
    public class BlockDataController : Controller
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IControlPanelRepository _cpanelRepository;
        private readonly IBlockDataRepository _blockDataRepository;
        private readonly IDistrictDataRepository _districtDataRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IConfiguration _config;
        private readonly string _constr;
        private readonly ISMSRepository _iSMS;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IDistrictRepository _DistrictRepository;
        public IConfiguration Configuration { get; }
        private readonly ISendSMSRepository _sms;
        private readonly ILoginRepository _loginRepository;
        private readonly IReportRepository _reportRepository;

        public BlockDataController(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ISendSMSRepository sms, IPanelRepository panelRepository, IControlPanelRepository cpanelRepository, IBlockDataRepository blockDataRepository, IDistrictDataRepository districtDataRepository, IIndicatorRepository indicatorRepository, IConfiguration config, ILoginRepository loginRepository, ISMSRepository smsRepository, IReportRepository reportRepository, IDistrictRepository districtRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
            _panelRepository = panelRepository;
            _cpanelRepository = cpanelRepository;
            _blockDataRepository = blockDataRepository;
            _districtDataRepository = districtDataRepository;
            _indicatorRepository = indicatorRepository;
            _iSMS = smsRepository;
            _loginRepository = loginRepository;
            _reportRepository = reportRepository;
            _DistrictRepository = districtRepository;
            _sms = sms;
            _config = config;
            _constr = _config.GetConnectionString("ABPConnectionString");
            Log.Information("BlockDataController initialized");
        }
        public IActionResult BlockDataEntry(string AppNo, int? FreqId = 2, int? Status = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.FREQUENCYID = (int)FreqId;
                bd.STATUS = (int)Status;
                bd.APPLICATIONNO = AppNo;
                bd.FREQUENCYNO = DateTime.Now.Month - 1;
                bd.YEAR = DateTime.Now.Year;
                bd.BLOCKID = (int)LeveDetailId;
                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                ViewBag.Sector = _panelRepository.ViewPanel();
                ControlPanel cp = new ControlPanel();
                cp.FREQUENCYID = (int)FreqId;
                cp.FREQUENCYNO = DateTime.Now.Month;
                List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                {

                    dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, bdByAppNo.APPLICATIONNO));
                    // dps.ForEach((c) => c.PROVIDERNAME = _blockDataRepository.GetContolValue(c, bdByAppNo.PROVIDERNAME));
                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                    dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                }
                else
                {
                    if (FreqId == 2)
                    {
                        var today = DateTime.Today;
                        var month = new DateTime(today.Year, today.Month, 1);
                        var first = month.AddMonths(-1);
                        var thisMonthStart = Convert.ToDateTime(first).AddDays(1 - Convert.ToDateTime(first).Day);
                        var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                        dps.ForEach((c) => c.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy"));
                        if (GetEligibiltiy((int)FreqId, Convert.ToDateTime(thisMonthStart), (int)LeveDetailId))
                        {
                            dps.ForEach((c) => c.DataEntryEligibility = true);
                        }
                        else
                        {
                            dps.ForEach((c) => c.DataEntryEligibility = false);
                        }
                    }
                    else
                    {
                        var today = DateTime.Today;
                        int year = Convert.ToDateTime(today).Year - 1;
                        var thisMonthStart = new DateTime(year, 1, 1);
                        var thisMonthEnd = new DateTime(year, 12, 31);
                        dps.ForEach((c) => c.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        if (GetEligibiltiy((int)FreqId, Convert.ToDateTime(thisMonthStart), (int)LeveDetailId))
                        {
                            dps.ForEach((c) => c.DataEntryEligibility = true);
                        }
                        else
                        {
                            dps.ForEach((c) => c.DataEntryEligibility = false);
                        }
                    }

                    //string date = GetDataEntryDate(0, (int)FreqId);
                    //if (GetEligibiltiy((int)FreqId, Convert.ToDateTime(date), (int)LeveDetailId))
                    //{
                    //    if (FreqId == 2)
                    //    {
                    //        var thisMonthStart = Convert.ToDateTime(date).AddDays(1 - Convert.ToDateTime(date).Day);
                    //        var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                    //        dps.ForEach((c) => c.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy"));
                    //        dps.ForEach((c) => c.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy"));
                    //    }
                    //    else if (FreqId == 3)
                    //    {
                    //        var currentQuater = (Convert.ToDateTime(date).Month + 1) / 3;
                    //        var daysInLastMonthOfQuarter = DateTime.DaysInMonth(Convert.ToDateTime(date).Year, 3 * currentQuater);
                    //        dps.ForEach((c) => c.TODATE = new DateTime(Convert.ToDateTime(date).Year, 3 * currentQuater, daysInLastMonthOfQuarter).ToString("dd-MMM-yyyy"));
                    //        dps.ForEach((c) => c.FROMDATE = new DateTime(Convert.ToDateTime(date).Year, 3 * currentQuater - 2, 1).ToString("dd-MMM-yyyy"));
                    //    }
                    //    else if (FreqId == 4)
                    //    {
                    //        var date1 = Convert.ToDateTime(date).AddYears(-1);
                    //        var month = date1.Month;
                    //        var year = date1.Year;
                    //        if (month <= 6)
                    //        {
                    //            var thisMonthStart = new DateTime(year, 1, 1);
                    //            var thisMonthEnd = new DateTime(year, 6, 30);
                    //            dps.ForEach((c) => c.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy"));
                    //            dps.ForEach((c) => c.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy"));
                    //        }
                    //        else
                    //        {
                    //            var thisMonthStart = new DateTime(year, 7, 1);
                    //            var thisMonthEnd = new DateTime(year, 12, 31);
                    //            dps.ForEach((c) => c.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy"));
                    //            dps.ForEach((c) => c.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy"));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        int year = Convert.ToDateTime(date).Year - 1;
                    //        var thisMonthStart = new DateTime(year, 1, 1);
                    //        var thisMonthEnd = new DateTime(year, 12, 31);
                    //        dps.ForEach((c) => c.TODATE = thisMonthEnd.ToString("dd-MMM-yyyy"));
                    //        dps.ForEach((c) => c.FROMDATE = thisMonthStart.ToString("dd-MMM-yyyy"));
                    //    }
                    //    dps.ForEach((c) => c.DataEntryEligibility = true);
                    //}
                    //else
                    //{
                    //    dps.ForEach((c) => c.DataEntryEligibility = false);
                    //}
                }
                dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                ViewBag.Result = dps;

                StringBuilder sbScript = new StringBuilder();
                List<ABP.Domain.DataPointExpression.DatapointExpression> panelWiseScript = new List<ABP.Domain.DataPointExpression.DatapointExpression>();
                ABP.Domain.DataPointExpression.DatapointExpression EE = new ABP.Domain.DataPointExpression.DatapointExpression();

                List<ABP.Domain.DataPointExpression.DatapointExpression> ExList = (List<ABP.Domain.DataPointExpression.DatapointExpression>)_cpanelRepository.GetAllExpressionData().Result;
                int result = 0;
                foreach (var E in ExList)
                {

                    List<string> DPList = E.ExpressionNAMEWithID.Split(',').ToList();
                    foreach (var i in DPList)
                    {
                        foreach (var D in dps)
                        {
                            if (Convert.ToString(D.CONTROLID) == i)
                            {

                                result = result + 1;
                            }

                        }

                    }

                    if (result == 2)
                    {
                        if (sbScript.ToString() == "")
                        {
                            sbScript.Append(E.Script);
                        }
                        else
                        {
                            if (sbScript.ToString().Contains("[Next]"))
                            {
                                string CurrentExpression = sbScript.ToString();
                                string NextExpression = CurrentExpression.Replace("[Next]", E.Script);
                                sbScript.Replace(CurrentExpression, NextExpression);
                            }
                        }
                    }
                    result = 0;
                }

                if (sbScript.ToString().Contains("[Next]"))
                {
                    string CurrentExpression = sbScript.ToString();
                    string NextExpression = CurrentExpression.Replace("[Next]", "if(1==1){ return true;}");
                    sbScript.Replace(CurrentExpression, NextExpression);

                }
                if (sbScript.ToString() == "")
                {
                    sbScript.Append("if(1==1){ return true;}");
                }
                ViewBag.Script = sbScript.ToString();




                ViewBag.AppNo = AppNo;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult RejectedData(string AppNo,int FreqId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.BLOCKID = (int)LeveDetailId;
                bd.APPLICATIONNO = AppNo;
                bd.FREQUENCYID = FreqId;
                bd.STATUS = 3;
               
                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                if (bdByAppNo == null)
                {
                    bd.STATUS = 2;
                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                }
                if (bdByAppNo != null)
                {
                    //string applicno = bdByAppNo.APPLICATIONNO
                 
                    ViewBag.Sector = _panelRepository.ViewPanel();

                    List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetRejectedControls(bdByAppNo).Result;
                    foreach (ControlPanel dpss in dps)
                    {
                        dpss.CONTROLVALUE = _blockDataRepository.GetContolValue(dpss, bdByAppNo.APPLICATIONNO);
                        dpss.DataEntryEligibility = true;
                        dpss.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy");
                        dpss.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy");
                        dpss.FREQUENCYVALUE = GetFrequencyValueByFreqId(bdByAppNo.FREQUENCYID, Convert.ToDateTime(dpss.FROMDATE));
                        //dpss.STATUS = 3;
                        //dpss.REMARK = bdByAppNo.REMARK;

                    }
                    ViewBag.Result = dps;
                    ViewBag.AppNo = bdByAppNo.APPLICATIONNO;
                    ViewBag.Remark = bdByAppNo.REMARK;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult GetProjectInstallationDate(int SectorId, int FreqId)
        {
            return Ok(JsonConvert.SerializeObject(GetDataEntryDate(SectorId, FreqId)));
        }
        public string GetDataEntryDate(int SectorId, int FreqId)
        {
            string InstallationDate = string.Empty;
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            string LastUpdatedFreq = _blockDataRepository.GetLastFreq((int)LeveDetailId, SectorId, FreqId);
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
        public string GetFrequencyValueByFreqId(int? FreqId, DateTime date)
        {
            string Freq = string.Empty;
            if (FreqId == 2)
            {
                Freq = date.ToString("MMM");
            }
            else if (FreqId == 3)
            {
                if (date.Month >= 1 && date.Month <= 3)
                    Freq = "Q1";
                else if (date.Month >= 4 && date.Month <= 6)
                    Freq = "Q2";
                else if (date.Month >= 7 && date.Month <= 9)
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
        public bool GetEligibiltiyNew(List<BlockData> blockData)
        {
            bool isEligible = true;
            DateTime installationDate = Convert.ToDateTime(Configuration.GetValue<string>("MySettings:InstallationDate")).AddMonths(-1);
            foreach (BlockData block in blockData)
            {
                var thisMonthStart = Convert.ToDateTime(block.TODATE.Replace("YYYY", installationDate.Year.ToString())).AddDays(1 - Convert.ToDateTime(block.TODATE.Replace("YYYY", installationDate.Year.ToString())).Day);
                var freqdatacount = _blockDataRepository.FREQDATACOUNT(block.FREQUENCYID).Result;
                var checkcount = _blockDataRepository.FRESHDATACOUNT(block.FREQUENCYID, GetFrequencyNoByFreqValue(GetFrequencyValueByFreqId(block.FREQUENCYID, thisMonthStart)), block.BLOCKID, thisMonthStart.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(block.FREQUENCYID, GetFrequencyNoByFreqValue(GetFrequencyValueByFreqId(block.FREQUENCYID, thisMonthStart)), block.BLOCKID, thisMonthStart.Year).Result;
                if (installationDate.AddMonths(-1).ToString("dd-MMM-yyyy") == thisMonthStart.ToString("dd-MMM-yyyy") && checkcount == 0)
                {
                    isEligible = true;
                }
                else if (freqdatacount > 0 && checkcount > 0 && ApproveDataCount > 0)
                {
                    isEligible = true;
                }
                else
                {
                    isEligible = false;
                }
            }
            return isEligible;
        }
        public bool GetEligibiltiy(int FreqId, DateTime idate, int BlockId)
        {
            var freqdatacount2 = _blockDataRepository.FREQDATACOUNT(2).Result;
            var freqdatacount3 = _blockDataRepository.FREQDATACOUNT(3).Result;
            var freqdatacount4 = _blockDataRepository.FREQDATACOUNT(4).Result;
            var freqdatacount5 = _blockDataRepository.FREQDATACOUNT(5).Result;
            DateTime installationDate = Convert.ToDateTime(Configuration.GetValue<string>("MySettings:InstallationDate")).AddMonths(-1);
            if (idate.Month == 1)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 12, BlockId, idate.Year).Result;
                var checkcount1 = _blockDataRepository.FRESHDATACOUNT(3, 4, BlockId, idate.Year).Result;
                var checkcount2 = _blockDataRepository.FRESHDATACOUNT(4, 2, BlockId, idate.Year).Result;
                var checkcount3 = _blockDataRepository.FRESHDATACOUNT(5, idate.Year - 1, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 12, BlockId, idate.Year).Result;
                var ApproveDataCount1 = _blockDataRepository.MONTHDATACOUNT(3, 4, BlockId, idate.Year).Result;
                var ApproveDataCount2 = _blockDataRepository.MONTHDATACOUNT(4, 2, BlockId, idate.Year).Result;
                var ApproveDataCount3 = _blockDataRepository.MONTHDATACOUNT(5, idate.Year - 1, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0) && (checkcount2 > 0 && ApproveDataCount2 > 0) && (checkcount3 > 0 && ApproveDataCount3 > 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FreqId == 3)
                {
                    if (freqdatacount3 == 0 || checkcount1 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0) && (checkcount2 > 0 && ApproveDataCount2 > 0) && (checkcount3 > 0 && ApproveDataCount3 > 0))
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
                    if (freqdatacount4 == 0 || checkcount2 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0) && (checkcount2 > 0 && ApproveDataCount2 > 0) && (checkcount3 > 0 && ApproveDataCount3 > 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FreqId == 5)
                {
                    if (freqdatacount5 == 0 || checkcount3 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0) && (checkcount2 > 0 && ApproveDataCount2 > 0) && (checkcount3 > 0 && ApproveDataCount3 > 0))
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
                    return false;
                }
            }
            else if (idate.Month == 2)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 2, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 2, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if (checkcount > 0 && ApproveDataCount > 0)
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
                    return false;
                }
            }
            else if (idate.Month == 3)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 3, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 3, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if (checkcount > 0 && ApproveDataCount > 0)
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
                    return false;
                }
            }
            else if (idate.Month == 4)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 4, BlockId, idate.Year).Result;
                var checkcount1 = _blockDataRepository.FRESHDATACOUNT(3, 1, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 4, BlockId, idate.Year).Result;
                var ApproveDataCount1 = _blockDataRepository.MONTHDATACOUNT(3, 1, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FreqId == 3)
                {
                    if (freqdatacount3 == 0 || checkcount1 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0))
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
                    return false;
                }
            }
            else if (idate.Month == 5)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 5, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 5, BlockId, idate.Year).Result;
                var checkcount1 = _blockDataRepository.FRESHDATACOUNT(3, 1, BlockId, idate.Year).Result;
                var ApproveDataCount1 = _blockDataRepository.MONTHDATACOUNT(3, 1, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FreqId == 3)
                {
                    if (freqdatacount3 == 0 || checkcount1 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0))
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
                    return false;
                }
            }
            else if (idate.Month == 6)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 6, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 6, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if (checkcount > 0 && ApproveDataCount > 0)
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
                    return false;
                }
            }
            else if (idate.Month == 7)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 7, BlockId, idate.Year).Result;
                var checkcount1 = _blockDataRepository.FRESHDATACOUNT(3, 2, BlockId, idate.Year).Result;
                var checkcount2 = _blockDataRepository.FRESHDATACOUNT(4, 1, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 7, BlockId, idate.Year).Result;
                var ApproveDataCount1 = _blockDataRepository.MONTHDATACOUNT(3, 2, BlockId, idate.Year).Result;
                var ApproveDataCount2 = _blockDataRepository.MONTHDATACOUNT(4, 1, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0) && (checkcount2 > 0 && ApproveDataCount2 > 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FreqId == 3)
                {
                    if (freqdatacount3 == 0 || checkcount1 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0) && (checkcount2 > 0 && ApproveDataCount2 > 0))
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
                    if (freqdatacount4 == 0 || checkcount2 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0) && (checkcount2 > 0 && ApproveDataCount2 > 0))
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
                    return false;
                }
            }
            else if (idate.Month == 8)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 8, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 8, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 && checkcount == 0)
                    {
                        return true;
                    }
                    else if (checkcount > 0 && ApproveDataCount > 0)
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
                    return false;
                }
            }
            else if (idate.Month == 9)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 9, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 9, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if (checkcount > 0 && ApproveDataCount > 0)
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
                    return false;
                }
            }
            else if (idate.Month == 10)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 10, BlockId, idate.Year).Result;
                var checkcount1 = _blockDataRepository.FRESHDATACOUNT(3, 3, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 10, BlockId, idate.Year).Result;
                var ApproveDataCount1 = _blockDataRepository.MONTHDATACOUNT(3, 3, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FreqId == 3)
                {
                    if (freqdatacount3 == 0 || checkcount1 == 0)
                    {
                        return true;
                    }
                    else if ((checkcount > 0 && ApproveDataCount > 0) && (checkcount1 > 0 && ApproveDataCount1 > 0))
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
                    return false;
                }
            }
            else if (idate.Month == 11)
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 11, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 11, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if (checkcount > 0 && ApproveDataCount > 0)
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
                    return false;
                }
            }
            else
            {
                var checkcount = _blockDataRepository.FRESHDATACOUNT(2, 12, BlockId, idate.Year).Result;
                var ApproveDataCount = _blockDataRepository.MONTHDATACOUNT(2, 12, BlockId, idate.Year).Result;
                if (FreqId == 2)
                {
                    if (freqdatacount2 == 0 || checkcount == 0)
                    {
                        return true;
                    }
                    else if (checkcount > 0 && ApproveDataCount > 0)
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
                    return false;
                }
            }
        }
        [HttpPost]
        public IActionResult BlockDataEntry([FromBody] List<ControlPanel> cpListObj)
        {
            OracleTransaction objTrans = null;
            using (OracleConnection objConn = new OracleConnection(_constr))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    Log.Information("BlockDataEntry started");
                    var UserId = HttpContext.Session.GetInt32("_UserId");
                    if (!string.IsNullOrEmpty(UserId.ToString()))
                    {
                        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                        bool isvalid = true;
                        if (cpListObj != null && cpListObj.Count > 0)
                        {
                            if (isvalid == false)
                            {
                                return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                            }
                            else
                            {
                                bool IsInsert = true;
                                string dateString = string.Empty;
                                int Year = 0;
                                if (cpListObj[0].FREQUENCYID == 5)
                                {
                                    Year = Convert.ToInt32(cpListObj[0].FROMDATE.Split('-')[2]);
                                }
                                else
                                {
                                    Year = Convert.ToInt32(cpListObj[0].TODATE.Split('-')[2]);
                                }
                                BlockData bd = new BlockData();
                                bd.BLOCKID = (int)LeveDetailId;
                                bd.FREQUENCYID = cpListObj[0].FREQUENCYID;
                                if (cpListObj[0].FREQUENCYID == 2)
                                {
                                    bd.FREQUENCYNO = Convert.ToDateTime(cpListObj[0].FROMDATE).Month;
                                }
                                else
                                {
                                    bd.FREQUENCYNO = Convert.ToDateTime(cpListObj[0].FROMDATE).Year;
                                }
                                bd.APPLICATIONNO = cpListObj[0].APPLICATIONO;
                                bd.YEAR = Year;
                                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                if (bdByAppNo == null)
                                {
                                    bd.STATUS = 3;
                                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                    if (bdByAppNo == null)
                                    {
                                        bd.STATUS = 2;
                                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        if (bdByAppNo == null)
                                        {
                                            bd.STATUS = 1;
                                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        }
                                    }
                                }
                                //timestamp for the unique application no
                                if (bdByAppNo == null)
                                {
                                    IsInsert = true;
                                    dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                else
                                {
                                    IsInsert = false;
                                    dateString = bdByAppNo.APPLICATIONNO;
                                }
                                Log.Information("Is Insert?" + IsInsert + "& Application No?" + dateString);
                                //for each panel there will be different application no for the main table 
                                BlockData blockData = (from e in cpListObj
                                                       select new BlockData
                                                       {
                                                           PANELID = e.PANELID,
                                                           PANELNAME = e.PANELNAME,
                                                           APPLICATIONNO = dateString,
                                                           FREQUENCYID = e.FREQUENCYID,
                                                           FREQUENCYNO = GetFrequencyNoByFreqValue(e.FREQUENCYVALUE),
                                                           FREQUENCYVALUE = e.FREQUENCYVALUE,
                                                           YEAR = Year,
                                                           FROMDATE = e.FROMDATE,
                                                           TODATE = e.TODATE,
                                                           DISTRICTID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           BLOCKID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId")),
                                                           STATUS = e.STATUS,
                                                           HNDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_1").Where(u => u.CONTROLVALUE != null).Count(),
                                                           AGDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_2").Where(u => u.CONTROLVALUE != null).Count(),
                                                           BIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_4").Where(u => u.CONTROLVALUE != null).Count(),
                                                           FIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_5").Where(u => u.CONTROLVALUE != null).Count(),
                                                           EDDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_3").Where(u => u.CONTROLVALUE != null).Count(),
                                                           DPCOUNT = cpListObj.Count(),
                                                           NONZERODPCOUNT = cpListObj.Where(u => u.CONTROLVALUE != null).Count()
                                                       }).FirstOrDefault();
                                string RetValMain = string.Empty;
                                string RetValR = string.Empty;
                                string RetMsg = string.Empty;
                                if (IsInsert)
                                {
                                    //inserting the main data 
                                    Log.Information("Data inserting to block data entry table");
                                    RetValMain = _blockDataRepository.AddBlockDataMain(blockData);
                                }
                                else
                                {
                                    //Updating the status on the basis of applicationno here
                                    Log.Information("Data updating to block data entry table");
                                    RetValMain = _blockDataRepository.UpdateBlockDataMain(blockData);
                                }
                                //making query for insertion and updation dynamically
                                List<StringBuilder> sbList = new List<StringBuilder>();
                                StringBuilder sb = null;
                                foreach (ControlPanel cp in cpListObj.GroupBy(x => x.PANELID).Select(x => x.FirstOrDefault()))
                                {
                                    List<string> columns = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLNAME).ToList();
                                    List<string> values = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLVALUE == null ? "null" : u.CONTROLVALUE.ToString()).ToList();
                                    //for panel wise data entry
                                    BlockData bds = new BlockData();
                                    bds.APPLICATIONNO = dateString;
                                    bds.PANELID = cp.PANELID;
                                    bds.STATUS = cpListObj[0].STATUS;
                                    bds.REMARK = string.Empty;
                                    RetValR = _districtDataRepository.RejectASectorData(bds);
                                    var logtablename = cp.PANELNAME + "_LOG";
                                    if (IsInsert)
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + cp.PANELNAME + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + cp.PANELNAME + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                    }
                                    else
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("update T_ABP_" + cp.PANELNAME + " set ");
                                        int count = 0;
                                        foreach (string col in columns)
                                        {
                                            if (count == columns.Count - 1)
                                            {
                                                sb.Append(col + "=" + values[count]);
                                            }
                                            else
                                            {
                                                sb.Append(col + "=" + values[count] + ",");
                                            }
                                            count++;
                                        }
                                        sb.Append(" where APPLICATIONNO='" + dateString + "'");
                                        sbList.Add(sb);
                                    }
                                }
                                RetMsg = _blockDataRepository.SubmitBlockData(sbList);
                                //Getting Block Details to send the message
                                Login objUserDetails = new Login();
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_UserId");
                                var Result = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                //Getting Corresponding Block's District Details to send the message to collector
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_PARENTID");
                                var DistResult = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                if ((RetValMain == "1" || RetValMain == "2") && RetMsg == "1" && cpListObj[0].STATUS == 0 && RetValR == "1")
                                {
                                    return Json(new { status = 1, freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved Successfully!" });

                                }
                                else if ((RetValMain == "2" && RetMsg == "1") || (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 2) && RetValR == "1")
                                {
                                    if (DistResult != null)
                                    {
                                        #region Text Message
                                        var mobno = DistResult.MOBILENO;
                                        var blockname = DistResult.BlockName;
                                        var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                        var templateid = "1007168066714211943";
                                        var smsresult = _sms.Sendsms(mobno, template, templateid);
                                        #endregion
                                    }
                                    return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Submited Successfully!" });
                                }
                                else
                                {
                                    Log.Information("Data couldn't entered to the specific table");
                                    return Json(new { msg = "Something went wrong!" });
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
                    objTrans.Commit();
                    Log.Information("BlockDataEntry ended");
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }
            }
        }
        public int GetFrequencyNoByFreqValue(string Freq)
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
        public IActionResult ViewBlockData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.BLOCKID = (int)LeveDetailId;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _blockDataRepository.ViewFrequency().Result;
                List<BlockData> blockDatas = _blockDataRepository.GetAllBlockData(bd).Result.ToList();
                //blockDatas.ForEach((c) => c.DPCOUNT = _blockDataRepository.GetEnteredDatapoints(c.APPLICATIONNO, "T_ABP_" + c.TABLENAME).Result.ToList().Count);
                ViewBag.Result = blockDatas;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewBlockData(BlockData blockdata)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //var ACTION = "BDO";
                blockdata.BLOCKID = (int)HttpContext.Session.GetInt32("_LeveDetailId");
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _blockDataRepository.ViewFrequency().Result;
                ViewBag.year = blockdata.YEAR;
                List<BlockData> blockDatas = _blockDataRepository.GetAllBlockData(blockdata).Result.ToList();
                //blockDatas.ForEach((c) => c.DPCOUNT = _blockDataRepository.GetEnteredDatapoints(c.APPLICATIONNO, "T_ABP_" + c.TABLENAME).Result.ToList().Count);
                ViewBag.Result = blockDatas;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }
        [HttpGet]
        public ActionResult GetAllBlockDataEntered(string AppNo, string PanelName,int FreqId,int FreqNo)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                List<BlockData> dps = _blockDataRepository.GetEnteredDatapoints(AppNo, "T_ABP_" + PanelName).Result.ToList();
                List<BlockData> ALLDPS = _blockDataRepository.GetAllControlPanelDP(PanelName, FreqId, FreqNo).Result.ToList();
                var mergedList = (
                                    from d in dps
                                    join a in ALLDPS on new { d.PANELNAME, d.CONTROLVALUE } equals new { a.PANELNAME, a.CONTROLVALUE } into matchingDPS
                                    from a in matchingDPS.DefaultIfEmpty()
                                    select a ?? d // Use the element from ALLDPS if it exists, otherwise use the element from dps
                                ).ToList();
                var notMatchedDPS = ALLDPS.Where(a => !dps.Any(d => d.PANELNAME == a.PANELNAME));
                mergedList.AddRange(notMatchedDPS);
                ViewBag.BlockData = mergedList;
                return PartialView("_DisplayBlockData");
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public ActionResult GetDatapoints(string AppNo, string PanelName, int Status)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.APPLICATIONNO = AppNo;
                bd.STATUS = Status;
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
        [HttpGet]
        public ActionResult GetAllIndicatorAndDatapoints(string AppNo, string PanelName, int Status)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.APPLICATIONNO = AppNo;
                bd.STATUS = Status;
                BlockData blockData = _blockDataRepository.GetBlockDataByAppNo(bd).Result.FirstOrDefault();
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
        [HttpGet]
        public ActionResult TestGetAllIndicatorAndDatapoints(int BlockId,int Year, int FreqId,int Month, int Status)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                bd.BLOCKID = BlockId;
                bd.YEAR = Year;
                bd.FREQUENCYID = FreqId;
                bd.FREQUENCYNO = Month;
                bd.STATUS = Status;
                BlockData blockData = _blockDataRepository.GetBlockDataByAppNo(bd).Result.FirstOrDefault();
                if (blockData != null)
                {
                    List<ControlPanel> DataPoints = new List<ControlPanel>();
                    List<ControlPanel> DataPointsNew = null;
                    List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(blockData).Result.ToList();
                    foreach (var cv in Indicators)
                    {
                        //DataPointsNew = new List<ControlPanel>();
                        //cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno='" + blockData.APPLICATIONNO + "'"));
                        //DataPointsNew = _districtDataRepository.GetAllDPByIndicator(cv);
                        //DataPoints.AddRange(DataPointsNew);
                        if (cv.INDICATORID == 271)
                        {
                            int indcatorid = cv.INDICATORID;
                        }
                        DataPointsNew = new List<ControlPanel>();
                        if (blockData.FREQUENCYID == 5)
                        {
                            if (cv.IndicatorFormulaName != null && cv.IndicatorFormulaName.Contains("[condition1]"))
                            {

                                string numeratorCondition = " WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND YEAR = " + blockData.YEAR + " AND DPMONTH <= " + blockData.FREQUENCYNO + " AND BLOCKID = " + blockData.BLOCKID + " and status in(1,2)) ORDER BY ID DESC";
                                // Condition for the denominator
                                //string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + mnth + "-" + year + "', 'mm-yyyy') AND BLOCKID = " + block.BLOCK_CODE + " and status in(1, 2)) ORDER BY ID DESC";
                                string denominatorCondition = "  WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + blockData.FREQUENCYNO + "-" + blockData.YEAR + "', 'mm-yyyy') AND BLOCKID = " + blockData.BLOCKID + " and status in (1,2)) ORDER BY ID DESC";
                                // Replace [condition] with the condition for the numerator
                                string queryWithNumeratorCondition = cv.IndicatorFormulaName.Replace("[condition]", numeratorCondition);

                                // Replace [condition1] with the condition for the denominator
                                string finalQuery = queryWithNumeratorCondition.Replace("[condition1]", denominatorCondition);
                              
                                cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(finalQuery);
                            }
                            else
                            {
                                cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno in (select applicationno from t_abp_block_dataentry where frequencyid=5 and year=" + blockData.YEAR + "  and blockid=" + blockData.BLOCKID + " and status in(1,2) and dpmonth <=" + blockData.FREQUENCYNO + ")"));
                                //cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", "  WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + blockData.FREQUENCYNO + "-" + blockData.YEAR + "', 'mm-yyyy') AND BLOCKID = " + blockData.BLOCKID + " and status=1) ORDER BY ID DESC"));
                            }

                            DataPointsNew = _districtDataRepository.GetAllDPByIndicator(cv);
                            DataPoints.AddRange(DataPointsNew);
                        }
                        else
                        {
                            if (cv.INDICATORID == 249)
                            {
                                int indcatorid = cv.INDICATORID;
                            }
                            if (cv.IndicatorFormulaName != null && cv.IndicatorFormulaName.Contains("[condition1]"))
                            {
                                string yrdp = cv.SelectedDataPoints;
                                string resyrdp = yrdp.Substring(0, yrdp.Length - 1);
                                string numeratorCondition = " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=2 and year=" + blockData.YEAR + " and frequencyno=" + blockData.FREQUENCYNO + " and blockid=" + blockData.BLOCKID + " and status=1)";
                                //Condition for the denominator
                                string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + blockData.FREQUENCYNO + "-" + blockData.YEAR + "', 'mm-yyyy') AND BLOCKID = " + blockData.BLOCKID + " and status=1) ORDER BY ID DESC";
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
                        //cv.CONTROLVALUE = _blockDataRepository.GetContolValue(cv, blockData.APPLICATIONNO);
                        if (cv.FREQUENCYID == 2)
                        {
                            cv.CONTROLVALUE = _blockDataRepository.GetContolValuecol(cv, blockData.APPLICATIONNO, blockData.BLOCKID, blockData.YEAR, cv.FREQUENCYID, blockData.FREQUENCYNO);
                        }
                        else
                        {
                            cv.CONTROLVALUE = _blockDataRepository.GetContolValuecolyear(cv, blockData.APPLICATIONNO, blockData.BLOCKID, blockData.YEAR, cv.FREQUENCYID, blockData.FREQUENCYNO);
                        }
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
        public IActionResult BlockDataEntryTemp(string AppNo, int Year,int MonthNo, int? FreqId = 2, int? Status = 0, int? FreqNo = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                // ViewBag.Status = 1;
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData data = new BlockData();
                if (AppNo == null)
                {
                    data.APPLICATIONNO = "0";
                }
                else
                {
                    data.APPLICATIONNO = AppNo;
                }
                //for (var i = 2021; i <= 2023; i++)
                //{
                //    var SelDtNtYear = _blockDataRepository.BlockDatapointYearlyCheck(Year, (int)LeveDetailId, 5);
                //    if (SelDtNtYear != "52" && Status == 0)
                //    {
                //        return RedirectToAction("BlockDataEntryTempYearly", "BlockData");

                //    }

                //    //int x = _DistrictRepository.INSERTAnalyticDataYearly(distcode, block.BLOCK_CODE, i, freqid).Result;
                //}
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                //if(FreqId==5)
                //{
                //    ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.Where(u=>Convert.ToInt32(u.FREQUENCYNO)<DateTime.Today.Year).ToList();
                //}
                //else
                //{
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                //}
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.SelectedYear = Year;
                if(FreqId == 5)
                {
                    ViewBag.SelectedMonth =MonthNo;
                }
                else {
                    ViewBag.SelectedMonth = (int)FreqNo;
                }
                
                if (FreqNo != 0)
                {
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = (int)FreqId;
                    bd.FREQUENCYNO = (int)FreqNo;
                    bd.MONTHNO = MonthNo;
                    bd.STATUS = (int)Status;
                    bd.APPLICATIONNO = AppNo;
                    bd.BLOCKID = (int)LeveDetailId;
                    bd.YEAR = Year;
                    BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    if (bdByAppNo != null)
                    {
                        ViewBag.Status = (int)Status;
                    }
                    else
                    {
                        bd.STATUS = 2;
                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                        if (bdByAppNo != null)
                        {
                            ViewBag.Status = 2;
                        }
                        else
                        {
                            bd.STATUS = 1;
                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                            if (bdByAppNo != null)
                            {
                                ViewBag.Status = 1;
                            }                           
                            else
                            {
                                bd.STATUS = 3;
                                bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                if (bdByAppNo != null)
                                {
                                    ViewBag.Status = 3;
                                }
                                else
                                {
                                    ViewBag.Status = 0;
                                }
                               
                            }
                        }
                    }
                    ViewBag.Sector = _panelRepository.ViewPanel();
                    ControlPanel cp = new ControlPanel();
                    cp.FREQUENCYID = (int)FreqId;
                    if (FreqId == 2)
                    {
                        cp.FREQUENCYNO = (int)FreqNo;
                    }
                    else if (FreqId == 5)
                    {
                        cp.FREQUENCYNO = MonthNo;
                    }
                    else
                    {
                        cp.FREQUENCYNO = DateTime.Now.Month;
                    }
                    cp.YEARTYPE = (int)Year;
                    List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                    {
                        dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, bdByAppNo.APPLICATIONNO));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                    }
                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                    ViewBag.Result = dps;
                    int result = 0;
                    StringBuilder sbScript = new StringBuilder();
                    List<ABP.Domain.DataPointExpression.DatapointExpression> ExList = (List<ABP.Domain.DataPointExpression.DatapointExpression>)_cpanelRepository.GetAllExpressionData().Result;
                    foreach (var E in ExList)
                    {

                        List<string> DPList = E.ExpressionNAMEWithID.Split(',').ToList();
                        foreach (var i in DPList)
                        {
                            foreach (var D in dps)
                            {
                                if (Convert.ToString(D.CONTROLID) == i)
                                {

                                    result = result + 1;
                                }

                            }

                        }

                        if (result == 2)
                        {
                            if (sbScript.ToString() == "")
                            {
                                sbScript.Append(E.Script);
                            }
                            else
                            {
                                if (sbScript.ToString().Contains("[Next]"))
                                {
                                    string CurrentExpression = sbScript.ToString();
                                    string NextExpression = CurrentExpression.Replace("[Next]", E.Script);
                                    sbScript.Replace(CurrentExpression, NextExpression);
                                }
                            }
                        }
                        result = 0;
                    }

                    if (sbScript.ToString().Contains("[Next]"))
                    {
                        string CurrentExpression = sbScript.ToString();
                        string NextExpression = CurrentExpression.Replace("[Next]", "if(1==1){ return true;}");
                        sbScript.Replace(CurrentExpression, NextExpression);

                    }
                    if (sbScript.ToString() == "")
                    {
                        sbScript.Append("if(1==1){ return true;}");
                    }
                    ViewBag.Script = sbScript.ToString();
                }
                else
                {

                }

                ViewBag.AppNo = AppNo;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult BlockDataEntryTemp([FromBody] List<ControlPanel> cpListObj)
        {
            OracleTransaction objTrans = null;

            using (OracleConnection objConn = new OracleConnection(_constr))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    Log.Information("BlockDataEntry started");
                    var UserId = HttpContext.Session.GetInt32("_UserId");
                    if (!string.IsNullOrEmpty(UserId.ToString()))
                    {
                        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                        bool isvalid = true;
                        if (cpListObj != null && cpListObj.Count > 0)
                        {
                            if (isvalid == false)
                            {
                                return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                            }
                            else
                            {
                                bool IsInsert = true;
                                string dateString = string.Empty;
                                int Year = 0;
                                if (cpListObj[0].FREQUENCYID == 5)
                                {
                                    Year = Convert.ToInt32(cpListObj[0].FROMDATE.Split('-')[2]);
                                }
                                else
                                {
                                    Year = Convert.ToInt32(cpListObj[0].TODATE.Split('-')[2]);
                                }
                                BlockData bd = new BlockData();
                                bd.BLOCKID = (int)LeveDetailId;
                                bd.FREQUENCYID = cpListObj[0].FREQUENCYID;
                                bd.FREQUENCYNO = cpListObj[0].FREQUENCYNO;
                                bd.APPLICATIONNO = cpListObj[0].APPLICATIONO;
                                bd.YEAR = Year;
                                bd.MONTHNO = cpListObj[0].MONTHNO;
                                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                if (bdByAppNo == null)
                                {
                                    bd.STATUS = 3;
                                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                    if (bdByAppNo == null)
                                    {
                                        bd.STATUS = 2;
                                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        if (bdByAppNo == null)
                                        {
                                            bd.STATUS = 1;
                                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        }
                                    }
                                }
                                //timestamp for the unique application no
                                if (bdByAppNo == null)
                                {
                                    IsInsert = true;
                                    dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                else
                                {
                                    IsInsert = false;
                                    dateString = bdByAppNo.APPLICATIONNO;
                                }
                                Log.Information("Is Insert?" + IsInsert + "& Application No?" + dateString);
                                //for each panel there will be different application no for the main table 
                                BlockData blockData = (from e in cpListObj
                                                       select new BlockData
                                                       {
                                                           MONTHNO = e.MONTHNO,
                                                           PANELID = e.PANELID,
                                                           PANELNAME = e.PANELNAME,
                                                           APPLICATIONNO = dateString,
                                                           FREQUENCYID = e.FREQUENCYID,
                                                           FREQUENCYNO = GetFrequencyNoByFreqValue(e.FREQUENCYVALUE),
                                                           FREQUENCYVALUE = e.FREQUENCYVALUE,
                                                           YEAR = Year,
                                                           FROMDATE = e.FROMDATE,
                                                           TODATE = e.TODATE,
                                                           DISTRICTID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           BLOCKID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId")),
                                                           STATUS = 0,
                                                           HNDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_1").Where(u => u.CONTROLVALUE != null).Count(),
                                                           AGDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_2").Where(u => u.CONTROLVALUE != null).Count(),
                                                           BIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_4").Where(u => u.CONTROLVALUE != null).Count(),
                                                           FIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_5").Where(u => u.CONTROLVALUE != null).Count(),
                                                           EDDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_3").Where(u => u.CONTROLVALUE != null).Count(),
                                                           DPCOUNT = cpListObj.Count(),
                                                           NONZERODPCOUNT = cpListObj.Where(u => u.CONTROLVALUE != null).Count()
                                                       }).FirstOrDefault();
                                string RetValMain = string.Empty;
                                string RetValR = string.Empty;
                                string RetMsg = string.Empty;
                                if (IsInsert)
                                {
                                    //inserting the main data 
                                    Log.Information("Data inserting to block data entry table");
                                    RetValMain = _blockDataRepository.AddBlockDataMain(blockData);
                                }
                                else
                                {
                                    //Updating the status on the basis of applicationno here
                                    Log.Information("Data updating to block data entry table");
                                    RetValMain = _blockDataRepository.UpdateBlockDataMain(blockData);
                                }
                                //making query for insertion and updation dynamically
                                List<StringBuilder> sbList = new List<StringBuilder>();
                                StringBuilder sb = null;
                                foreach (ControlPanel cp in cpListObj.GroupBy(x => x.PANELID).Select(x => x.FirstOrDefault()))
                                {
                                    List<string> columns = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLNAME).ToList();
                                    List<string> values = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLVALUE == null ? "null" : u.CONTROLVALUE.ToString()).ToList();
                                    //for panel wise data entry
                                    BlockData bds = new BlockData();
                                    bds.APPLICATIONNO = dateString;
                                    bds.PANELID = cp.PANELID;
                                    bds.STATUS = cpListObj[0].STATUS;
                                    bds.REMARK = string.Empty;
                                    RetValR = _districtDataRepository.RejectASectorData(bds);
                                    var logtablename = cp.PANELNAME + "_LOG";
                                    if (IsInsert)
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + cp.PANELNAME + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + cp.PANELNAME + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                    }
                                    else
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("update T_ABP_" + cp.PANELNAME + " set ");
                                        int count = 0;
                                        foreach (string col in columns)
                                        {
                                            if (count == columns.Count - 1)
                                            {
                                                sb.Append(col + "=" + values[count]);
                                            }
                                            else
                                            {
                                                sb.Append(col + "=" + values[count] + ",");
                                            }
                                            count++;
                                        }
                                        sb.Append(" where APPLICATIONNO='" + dateString + "'");
                                        sbList.Add(sb);
                                    }
                                }
                                RetMsg = _blockDataRepository.SubmitBlockData(sbList);
                                //Getting Block Details to send the message
                                Login objUserDetails = new Login();
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_UserId");
                                var Result = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                //Getting Corresponding Block's District Details to send the message to collector
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_PARENTID");
                                var DistResult = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                if (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 0 && RetValR == "1")
                                {
                                    #region Text Message
                                    var mobno = DistResult.MOBILENO;
                                    var blockname = DistResult.BlockName;
                                    var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    var templateid = "1007168066714211943";
                                    var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    #endregion
                                    objTrans.Commit();
                                    return Json(new { status = 1, freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved As Draft Successfully!" });
                                    //return Redirect("/BlockData/PreviewBlockDataEntry?AppNo=" + dateString + "&Year=" + Year + "&FreqId=" + cpListObj[0].FREQUENCYID + "&Status=0&FreqNo=" + cpListObj[0].FREQUENCYNO);
                                }
                                else if ((RetValMain == "2" && RetMsg == "1") || (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 2) && RetValR == "1")
                                {
                                    objTrans.Commit();
                                    if (cpListObj[0].STATUS == 0)
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved As Draft Successfully!" });
                                    }
                                    else
                                    {
                                        return Json(new { status = 0, appno = dateString, freqid = cpListObj[0].FREQUENCYID, year = Year, freqno = cpListObj[0].FREQUENCYNO, msg = "Records Submited Successfully!" });
                                    }
                                    //return Redirect("/BlockData/PreviewBlockDataEntry?AppNo=" + dateString + "&Year=" + Year + "&FreqId=" + cpListObj[0].FREQUENCYID + "&Status=0&FreqNo=" + cpListObj[0].FREQUENCYNO);
                                    //#region Text Message
                                    //var mobno = DistResult.MOBILENO;
                                    //var blockname = DistResult.BlockName;
                                    //var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    //var templateid = "1007168066714211943";
                                    //var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    //#endregion
                                }
                                else
                                {
                                    Log.Information("Data couldn't entered to the specific table");
                                    return Json(new { msg = "Something went wrong!" });
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
                    objTrans.Commit();
                    Log.Information("BlockDataEntry ended");
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }
            }
        }
        [HttpPost]
        public IActionResult BlockDataEntryRej([FromBody] List<ControlPanel> cpListObj)
        {
            OracleTransaction objTrans = null;

            using (OracleConnection objConn = new OracleConnection(_constr))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    Log.Information("BlockDataEntry started");
                    var UserId = HttpContext.Session.GetInt32("_UserId");
                    if (!string.IsNullOrEmpty(UserId.ToString()))
                    {
                        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                        bool isvalid = true;
                        if (cpListObj != null && cpListObj.Count > 0)
                        {
                            if (isvalid == false)
                            {
                                return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                            }
                            else
                            {
                                bool IsInsert = true;
                                string dateString = string.Empty;
                                int Year = 0;
                                if (cpListObj[0].FREQUENCYID == 5)
                                {
                                    Year = Convert.ToInt32(cpListObj[0].FROMDATE.Split('-')[2]);
                                }
                                else
                                {
                                    Year = Convert.ToInt32(cpListObj[0].TODATE.Split('-')[2]);
                                }
                                BlockData bd = new BlockData();
                                bd.BLOCKID = (int)LeveDetailId;
                                bd.FREQUENCYID = cpListObj[0].FREQUENCYID;
                                bd.FREQUENCYNO = cpListObj[0].FREQUENCYNO;
                                bd.APPLICATIONNO = cpListObj[0].APPLICATIONO;
                               
                                bd.YEAR = Year;
                                bd.MONTHNO = cpListObj[0].MONTHNO;
                                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                string sMonth = Convert.ToDateTime(cpListObj[0].FROMDATE).ToString("MM");
                                if (bd.FREQUENCYID == 2)
                                {
                                    bd.FREQUENCYNO = Convert.ToInt32(sMonth);
                                }
                                //string ApplicationNo= _blockDataRepository.GetRejectAppno(Year, bd.FREQUENCYNO,bd.BLOCKID, bd.FREQUENCYID);
                                string ApplicationNo = bd.APPLICATIONNO;
                                if (bdByAppNo == null)
                                {

                                    bd.STATUS = 3;
                                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                    if (bdByAppNo == null)
                                    {
                                        bd.STATUS = 2;
                                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        if (bdByAppNo == null)
                                        {
                                            bd.STATUS = 1;
                                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        }
                                    }
                                }
                                //timestamp for the unique application no
                                if (bdByAppNo == null)
                                {
                                    IsInsert = true;
                                    dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                else
                                {
                                    IsInsert = false;
                                    // dateString = bdByAppNo.APPLICATIONNO;
                                    dateString = ApplicationNo;
                                }
                                Log.Information("Is Insert?" + IsInsert + "& Application No?" + dateString);
                                //for each panel there will be different application no for the main table 
                                BlockData blockData = (from e in cpListObj
                                                       select new BlockData
                                                       {
                                                           MONTHNO = e.MONTHNO,
                                                           PANELID = e.PANELID,
                                                           PANELNAME = e.PANELNAME,
                                                           APPLICATIONNO = dateString,
                                                           FREQUENCYID = e.FREQUENCYID,
                                                           FREQUENCYNO = GetFrequencyNoByFreqValue(e.FREQUENCYVALUE),
                                                           FREQUENCYVALUE = e.FREQUENCYVALUE,
                                                           YEAR = Year,
                                                           FROMDATE = e.FROMDATE,
                                                           TODATE = e.TODATE,
                                                           DISTRICTID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           BLOCKID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId")),
                                                           STATUS = 0,
                                                           HNDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_1").Where(u => u.CONTROLVALUE != null).Count(),
                                                           AGDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_2").Where(u => u.CONTROLVALUE != null).Count(),
                                                           BIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_4").Where(u => u.CONTROLVALUE != null).Count(),
                                                           FIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_5").Where(u => u.CONTROLVALUE != null).Count(),
                                                           EDDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_3").Where(u => u.CONTROLVALUE != null).Count(),
                                                           DPCOUNT = cpListObj.Count(),
                                                           NONZERODPCOUNT = cpListObj.Where(u => u.CONTROLVALUE != null).Count()
                                                       }).FirstOrDefault();
                                string RetValMain = string.Empty;
                                string RetValR = string.Empty;
                                string RetMsg = string.Empty;
                                if (IsInsert)
                                {
                                    //inserting the main data 
                                    Log.Information("Data inserting to block data entry table");
                                    RetValMain = _blockDataRepository.AddBlockDataMain(blockData);
                                }
                                else
                                {
                                    //Updating the status on the basis of applicationno here
                                    Log.Information("Data updating to block data entry table");
                                    RetValMain = _blockDataRepository.UpdateBlockDataMain(blockData);
                                }
                                //making query for insertion and updation dynamically
                                List<StringBuilder> sbList = new List<StringBuilder>();
                                StringBuilder sb = null;
                                foreach (ControlPanel cp in cpListObj.GroupBy(x => x.PANELID).Select(x => x.FirstOrDefault()))
                                {
                                    List<string> columns = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLNAME).ToList();
                                    List<string> values = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLVALUE == null ? "null" : u.CONTROLVALUE.ToString()).ToList();
                                    //for panel wise data entry
                                    BlockData bds = new BlockData();
                                    bds.APPLICATIONNO = dateString;
                                    string Applno = dateString;
                                    bds.PANELID = cp.PANELID;
                                    bds.STATUS = cpListObj[0].STATUS;
                                    bds.REMARK = string.Empty;
                                    RetValR = _districtDataRepository.RejectASectorData(bds);
                                    var logtablename = cp.PANELNAME + "_LOG";
                                    if (IsInsert)
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + cp.PANELNAME + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + cp.PANELNAME + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                    }
                                    else
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("update T_ABP_" + cp.PANELNAME + " set ");
                                        int count = 0;
                                        foreach (string col in columns)
                                        {
                                            if (count == columns.Count - 1)
                                            {
                                                sb.Append(col + "=" + values[count]);
                                            }
                                            else
                                            {
                                                sb.Append(col + "=" + values[count] + ",");
                                            }
                                            count++;
                                        }
                                        sb.Append(" where APPLICATIONNO='" + dateString + "'");
                                        sbList.Add(sb);
                                    }
                                }
                                RetMsg = _blockDataRepository.SubmitBlockData(sbList);
                                //Getting Block Details to send the message
                                Login objUserDetails = new Login();
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_UserId");
                                var Result = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                //Getting Corresponding Block's District Details to send the message to collector
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_PARENTID");
                                var DistResult = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                if (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 0 && RetValR == "1")
                                {
                                    #region Text Message
                                    var mobno = DistResult.MOBILENO;
                                    var blockname = DistResult.BlockName;
                                    var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    var templateid = "1007168066714211943";
                                    var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    #endregion
                                    objTrans.Commit();
                                    return Json(new { status = 1, freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved As Draft Successfully!" });
                                    //return Redirect("/BlockData/PreviewBlockDataEntry?AppNo=" + dateString + "&Year=" + Year + "&FreqId=" + cpListObj[0].FREQUENCYID + "&Status=0&FreqNo=" + cpListObj[0].FREQUENCYNO);
                                }
                                else if ((RetValMain == "2" && RetMsg == "1") || (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 2) && RetValR == "1")
                                {
                                    objTrans.Commit();
                                    if (cpListObj[0].STATUS == 0)
                                    {

                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved As Draft Successfully!" });
                                    }
                                    else
                                    {
                                        string retMsg = _blockDataRepository.UpdateRejData(dateString);
                                        return Json(new { status = 0, appno = dateString, freqid = cpListObj[0].FREQUENCYID, year = Year, freqno = cpListObj[0].FREQUENCYNO, msg = "Records Submited Successfully!" });
                                    }
                              
                                }
                                else
                                {
                                    Log.Information("Data couldn't entered to the specific table");
                                    return Json(new { msg = "Something went wrong!" });
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
                    objTrans.Commit();
                    Log.Information("BlockDataEntry ended");
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }
            }
        }
        [HttpPost]
        public IActionResult AutoSaveBlockDataEntry([FromBody] List<ControlPanel> cpListObj)
        {
            OracleTransaction objTrans = null;

            using (OracleConnection objConn = new OracleConnection(_constr))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    Log.Information("BlockDataEntry started");
                    var UserId = HttpContext.Session.GetInt32("_UserId");
                    if (!string.IsNullOrEmpty(UserId.ToString()))
                    {
                        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                        bool isvalid = true;
                        if (cpListObj != null && cpListObj.Count > 0)
                        {
                            if (isvalid == false)
                            {
                                return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                            }
                            else
                            {
                                bool IsInsert = true;
                                string dateString = string.Empty;
                                int Year = 0;
                                if (cpListObj[0].FREQUENCYID == 5)
                                {
                                    Year = Convert.ToInt32(cpListObj[0].FROMDATE.Split('-')[2]);
                                }
                                else
                                {
                                    Year = Convert.ToInt32(cpListObj[0].TODATE.Split('-')[2]);
                                }
                                BlockData bd = new BlockData();
                                bd.BLOCKID = (int)LeveDetailId;
                                bd.FREQUENCYID = cpListObj[0].FREQUENCYID;
                                bd.FREQUENCYNO = cpListObj[0].FREQUENCYNO;
                                bd.APPLICATIONNO = cpListObj[0].APPLICATIONO;
                                bd.YEAR = Year;
                                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                if (bdByAppNo == null)
                                {
                                    bd.STATUS = 3;
                                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                    if (bdByAppNo == null)
                                    {
                                        bd.STATUS = 2;
                                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        if (bdByAppNo == null)
                                        {
                                            bd.STATUS = 1;
                                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        }
                                    }
                                }
                                //timestamp for the unique application no
                                if (bdByAppNo == null)
                                {
                                    IsInsert = true;
                                    dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                else
                                {
                                    IsInsert = false;
                                    dateString = bdByAppNo.APPLICATIONNO;
                                }
                                Log.Information("Is Insert?" + IsInsert + "& Application No?" + dateString);
                                //for each panel there will be different application no for the main table 
                                BlockData blockData = (from e in cpListObj
                                                       select new BlockData
                                                       {
                                                           MONTHNO = e.MONTHNO,
                                                           PANELID = e.PANELID,
                                                           PANELNAME = e.PANELNAME,
                                                           APPLICATIONNO = dateString,
                                                           FREQUENCYID = e.FREQUENCYID,
                                                           FREQUENCYNO = GetFrequencyNoByFreqValue(e.FREQUENCYVALUE),
                                                           FREQUENCYVALUE = e.FREQUENCYVALUE,
                                                           YEAR = Year,
                                                           FROMDATE = e.FROMDATE,
                                                           TODATE = e.TODATE,
                                                           DISTRICTID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           BLOCKID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId")),
                                                           STATUS = e.STATUS,
                                                           HNDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_1").Where(u => u.CONTROLVALUE != null).Count(),
                                                           AGDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_2").Where(u => u.CONTROLVALUE != null).Count(),
                                                           BIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_4").Where(u => u.CONTROLVALUE != null).Count(),
                                                           FIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_5").Where(u => u.CONTROLVALUE != null).Count(),
                                                           EDDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_3").Where(u => u.CONTROLVALUE != null).Count(),
                                                           DPCOUNT = cpListObj.Count(),
                                                           NONZERODPCOUNT = cpListObj.Where(u => u.CONTROLVALUE != null).Count()
                                                       }).FirstOrDefault();
                                string RetValMain = string.Empty;
                                string RetValR = string.Empty;
                                string RetMsg = string.Empty;
                                if (IsInsert)
                                {
                                    //inserting the main data 
                                    Log.Information("Data inserting to block data entry table");
                                    RetValMain = _blockDataRepository.AddBlockDataMain(blockData);
                                }
                                else
                                {
                                    //Updating the status on the basis of applicationno here
                                    Log.Information("Data updating to block data entry table");
                                    RetValMain = _blockDataRepository.UpdateBlockDataMain(blockData);
                                }
                                //making query for insertion and updation dynamically
                                List<StringBuilder> sbList = new List<StringBuilder>();
                                StringBuilder sb = null;
                                foreach (ControlPanel cp in cpListObj.GroupBy(x => x.PANELID).Select(x => x.FirstOrDefault()))
                                {
                                    List<string> columns = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLNAME).ToList();
                                    List<string> values = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLVALUE == null ? "null" : u.CONTROLVALUE.ToString()).ToList();
                                    //for panel wise data entry
                                    BlockData bds = new BlockData();
                                    bds.APPLICATIONNO = dateString;
                                    bds.PANELID = cp.PANELID;
                                    bds.STATUS = cpListObj[0].STATUS;
                                    bds.REMARK = string.Empty;
                                    RetValR = _districtDataRepository.RejectASectorData(bds);
                                    var logtablename = cp.PANELNAME + "_LOG";
                                    if (IsInsert)
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + cp.PANELNAME + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + cp.PANELNAME + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                    }
                                    else
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("update T_ABP_" + cp.PANELNAME + " set ");
                                        int count = 0;
                                        foreach (string col in columns)
                                        {
                                            if (count == columns.Count - 1)
                                            {
                                                sb.Append(col + "=" + values[count]);
                                            }
                                            else
                                            {
                                                sb.Append(col + "=" + values[count] + ",");
                                            }
                                            count++;
                                        }
                                        sb.Append(" where APPLICATIONNO='" + dateString + "'");
                                        sbList.Add(sb);
                                    }
                                }
                                RetMsg = _blockDataRepository.SubmitBlockData(sbList);
                                //Getting Block Details to send the message
                                Login objUserDetails = new Login();
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_UserId");
                                var Result = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                //Getting Corresponding Block's District Details to send the message to collector
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_PARENTID");
                                var DistResult = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                if (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 0 && RetValR == "1")
                                {
                                    #region Text Message
                                    var mobno = DistResult.MOBILENO;
                                    var blockname = DistResult.BlockName;
                                    var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    var templateid = "1007168066714211943";
                                    var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    #endregion
                                    return Json(new { status = 1, freqid = cpListObj[0].FREQUENCYID, msg = "Record Saved As Draft Successfully!" });

                                }
                                else if ((RetValMain == "2" && RetMsg == "1") || (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 2) && RetValR == "1")
                                {
                                    if (cpListObj[0].STATUS == 0)
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Record Saved As Draft Successfully!" });
                                    }
                                    else
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Record Submited Successfully!" });
                                    }
                                    //#region Text Message
                                    //var mobno = DistResult.MOBILENO;
                                    //var blockname = DistResult.BlockName;
                                    //var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    //var templateid = "1007168066714211943";
                                    //var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    //#endregion
                                }
                                else
                                {
                                    Log.Information("Data couldn't entered to the specific table");
                                    return Json(new { msg = "Something went wrong!" });
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
                    objTrans.Commit();
                    Log.Information("BlockDataEntry ended");
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }
            }
        }
        public IActionResult PreviewBlockDataEntry(string AppNo, int Year, int MonthNo, int? FreqId = 2, int? Status = 0, int? FreqNo = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                // ViewBag.Status = 1;
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData data = new BlockData();
                if (AppNo == null)
                {
                    data.APPLICATIONNO = "0";
                }
                else
                {
                    data.APPLICATIONNO = AppNo;
                }
                //for (var i = 2021; i <= 2023; i++)
                //{
                //    var SelDtNtYear = _blockDataRepository.BlockDatapointYearlyCheck(Year, (int)LeveDetailId, 5);
                //    if (SelDtNtYear != "52" && Status == 0)
                //    {
                //        return RedirectToAction("BlockDataEntryTempYearly", "BlockData");

                //    }

                //    //int x = _DistrictRepository.INSERTAnalyticDataYearly(distcode, block.BLOCK_CODE, i, freqid).Result;
                //}
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Frequency = FreqId;
                //if(FreqId==5)
                //{
                //    ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.Where(u=>Convert.ToInt32(u.FREQUENCYNO)<DateTime.Today.Year).ToList();
                //}
                //else
                //{
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                //}
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                ViewBag.SelectedYear = Year;
                if (FreqId == 5)
                {
                    ViewBag.SelectedMonth = MonthNo;
                }
                else
                {
                    ViewBag.SelectedMonth = (int)FreqNo;
                }
                //ViewBag.SelectedMonth = (int)FreqNo;
                if (FreqNo != 0)
                {
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = (int)FreqId;
                    bd.FREQUENCYNO = (int)FreqNo;
                    bd.STATUS = (int)Status;
                    bd.APPLICATIONNO = AppNo;
                    bd.BLOCKID = (int)LeveDetailId;
                    bd.YEAR = Year;
                    BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    if (bdByAppNo != null)
                    {
                        ViewBag.Status = (int)Status;
                    }
                    else
                    {
                        bd.STATUS = 2;
                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                        if (bdByAppNo != null)
                        {
                            ViewBag.Status = 2;
                        }
                        else
                        {
                            bd.STATUS = 1;
                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                            if (bdByAppNo != null)
                            {
                               
                                ViewBag.Status = 1;
                            }
                            else
                            {
                                ViewBag.Status = 0;
                            }
                        }
                    }
                    ViewBag.Sector = _panelRepository.ViewPanel();
                    ControlPanel cp = new ControlPanel();
                    cp.FREQUENCYID = (int)FreqId;
                    if (FreqId == 2)
                    {
                        cp.FREQUENCYNO = (int)FreqNo;
                    }
                    else if (FreqId == 5)
                    {
                        cp.FREQUENCYNO = MonthNo;
                    }
                    else
                    {
                        cp.FREQUENCYNO = DateTime.Now.Month;
                    }
                    cp.YEARTYPE = (int)Year;
                    List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                    {
                        dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, bdByAppNo.APPLICATIONNO));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                    }
                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                    ViewBag.Result = dps;
                    int result = 0;
                    StringBuilder sbScript = new StringBuilder();
                    List<ABP.Domain.DataPointExpression.DatapointExpression> ExList = (List<ABP.Domain.DataPointExpression.DatapointExpression>)_cpanelRepository.GetAllExpressionData().Result;
                    foreach (var E in ExList)
                    {

                        List<string> DPList = E.ExpressionNAMEWithID.Split(',').ToList();
                        foreach (var i in DPList)
                        {
                            foreach (var D in dps)
                            {
                                if (Convert.ToString(D.CONTROLID) == i)
                                {

                                    result = result + 1;
                                }

                            }

                        }

                        if (result == 2)
                        {
                            if (sbScript.ToString() == "")
                            {
                                sbScript.Append(E.Script);
                            }
                            else
                            {
                                if (sbScript.ToString().Contains("[Next]"))
                                {
                                    string CurrentExpression = sbScript.ToString();
                                    string NextExpression = CurrentExpression.Replace("[Next]", E.Script);
                                    sbScript.Replace(CurrentExpression, NextExpression);
                                }
                            }
                        }
                        result = 0;
                    }

                    if (sbScript.ToString().Contains("[Next]"))
                    {
                        string CurrentExpression = sbScript.ToString();
                        string NextExpression = CurrentExpression.Replace("[Next]", "if(1==1){ return true;}");
                        sbScript.Replace(CurrentExpression, NextExpression);

                    }
                    if (sbScript.ToString() == "")
                    {
                        sbScript.Append("if(1==1){ return true;}");
                    }
                    ViewBag.Script = sbScript.ToString();
                }
                else
                {

                }

                ViewBag.AppNo = AppNo;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public IActionResult PreviewBlockDataEntry([FromBody] List<ControlPanel> cpListObj)
        {
            OracleTransaction objTrans = null;

            using (OracleConnection objConn = new OracleConnection(_constr))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    Log.Information("BlockDataEntry started");
                    var UserId = HttpContext.Session.GetInt32("_UserId");
                    if (!string.IsNullOrEmpty(UserId.ToString()))
                    {
                        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                        bool isvalid = true;
                        if (cpListObj != null && cpListObj.Count > 0)
                        {
                            if (isvalid == false)
                            {
                                return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                            }
                            else
                            {
                                bool IsInsert = true;
                                string dateString = string.Empty;
                                int Year = 0;
                                if (cpListObj[0].FREQUENCYID == 5)
                                {
                                    Year = Convert.ToInt32(cpListObj[0].FROMDATE.Split('-')[2]);
                                }
                                else
                                {
                                    Year = Convert.ToInt32(cpListObj[0].TODATE.Split('-')[2]);
                                }
                                BlockData bd = new BlockData();
                                bd.BLOCKID = (int)LeveDetailId;
                                bd.FREQUENCYID = cpListObj[0].FREQUENCYID;
                                bd.FREQUENCYNO = cpListObj[0].FREQUENCYNO;
                                bd.APPLICATIONNO = cpListObj[0].APPLICATIONO;
                                bd.YEAR = Year;
                                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                if (bdByAppNo == null)
                                {
                                    bd.STATUS = 3;
                                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                    if (bdByAppNo == null)
                                    {
                                        bd.STATUS = 2;
                                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        if (bdByAppNo == null)
                                        {
                                            bd.STATUS = 1;
                                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        }
                                    }
                                }
                                //timestamp for the unique application no
                                if (bdByAppNo == null)
                                {
                                    IsInsert = true;
                                    dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                else
                                {
                                    IsInsert = false;
                                    dateString = bdByAppNo.APPLICATIONNO;
                                }
                                Log.Information("Is Insert?" + IsInsert + "& Application No?" + dateString);
                                //for each panel there will be different application no for the main table 
                                BlockData blockData = (from e in cpListObj
                                                       select new BlockData
                                                       {
                                                           MONTHNO = e.MONTHNO,
                                                           PANELID = e.PANELID,
                                                           PANELNAME = e.PANELNAME,
                                                           APPLICATIONNO = dateString,
                                                           FREQUENCYID = e.FREQUENCYID,
                                                           FREQUENCYNO = GetFrequencyNoByFreqValue(e.FREQUENCYVALUE),
                                                           FREQUENCYVALUE = e.FREQUENCYVALUE,
                                                           YEAR = Year,
                                                           FROMDATE = e.FROMDATE,
                                                           TODATE = e.TODATE,
                                                           DISTRICTID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           BLOCKID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId")),
                                                           STATUS = e.STATUS,
                                                           HNDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_1").Where(u => u.CONTROLVALUE != null).Count(),
                                                           AGDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_2").Where(u => u.CONTROLVALUE != null).Count(),
                                                           BIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_4").Where(u => u.CONTROLVALUE != null).Count(),
                                                           FIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_5").Where(u => u.CONTROLVALUE != null).Count(),
                                                           EDDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_3").Where(u => u.CONTROLVALUE != null).Count(),
                                                           DPCOUNT = cpListObj.Count(),
                                                           NONZERODPCOUNT = cpListObj.Where(u => u.CONTROLVALUE != null).Count()
                                                       }).FirstOrDefault();
                                string RetValMain = string.Empty;
                                string RetValR = string.Empty;
                                string RetMsg = string.Empty;
                                if (IsInsert)
                                {
                                    //inserting the main data 
                                    Log.Information("Data inserting to block data entry table");
                                    RetValMain = _blockDataRepository.AddBlockDataMain(blockData);
                                }
                                else
                                {
                                    //Updating the status on the basis of applicationno here
                                    Log.Information("Data updating to block data entry table");
                                    RetValMain = _blockDataRepository.UpdateBlockDataMain(blockData);
                                }
                                //making query for insertion and updation dynamically
                                List<StringBuilder> sbList = new List<StringBuilder>();
                                StringBuilder sb = null;
                                foreach (ControlPanel cp in cpListObj.GroupBy(x => x.PANELID).Select(x => x.FirstOrDefault()))
                                {
                                    List<string> columns = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLNAME).ToList();
                                    List<string> values = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLVALUE == null ? "null" : u.CONTROLVALUE.ToString()).ToList();
                                    //for panel wise data entry
                                    BlockData bds = new BlockData();
                                    bds.APPLICATIONNO = dateString;
                                    bds.PANELID = cp.PANELID;
                                    bds.STATUS = cpListObj[0].STATUS;
                                    bds.REMARK = string.Empty;
                                    RetValR = _districtDataRepository.RejectASectorData(bds);
                                    var logtablename = cp.PANELNAME + "_LOG";
                                    if (IsInsert)
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + cp.PANELNAME + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + cp.PANELNAME + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                    }
                                    else
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("update T_ABP_" + cp.PANELNAME + " set ");
                                        int count = 0;
                                        foreach (string col in columns)
                                        {
                                            if (count == columns.Count - 1)
                                            {
                                                sb.Append(col + "=" + values[count]);
                                            }
                                            else
                                            {
                                                sb.Append(col + "=" + values[count] + ",");
                                            }
                                            count++;
                                        }
                                        sb.Append(" where APPLICATIONNO='" + dateString + "'");
                                        sbList.Add(sb);
                                    }
                                }
                                RetMsg = _blockDataRepository.SubmitBlockData(sbList);
                                //Getting Block Details to send the message
                                Login objUserDetails = new Login();
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_UserId");
                                var Result = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                //Getting Corresponding Block's District Details to send the message to collector
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_PARENTID");
                                var DistResult = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                if (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 0 && RetValR == "1")
                                {
                                    #region Text Message
                                    var mobno = DistResult.MOBILENO;
                                    var blockname = DistResult.BlockName;
                                    var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    var templateid = "1007168066714211943";
                                    var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    #endregion
                                    return Json(new { status = 1, freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved As Draft Successfully!" });

                                }
                                else if ((RetValMain == "2" && RetMsg == "1") || (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 2) && RetValR == "1")
                                {
                                    if (cpListObj[0].STATUS == 0)
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved As Draft Successfully!" });
                                    }
                                    else
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Record Submitted Successfully !" });
                                    }
                                    //#region Text Message
                                    //var mobno = DistResult.MOBILENO;
                                    //var blockname = DistResult.BlockName;
                                    //var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    //var templateid = "1007168066714211943";
                                    //var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    //#endregion
                                }
                                else
                                {
                                    Log.Information("Data couldn't entered to the specific table");
                                    return Json(new { msg = "Something went wrong!" });
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
                    objTrans.Commit();
                    Log.Information("BlockDataEntry ended");
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }
            }
        }
        public IActionResult QuaterlyBlockDataEntry(string AppNo, int Year, int? FreqId = 3, int? Status = 0, int? FreqNo = 0, int? blockid = 0, int? Distid = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = blockid;
                BlockData data = new BlockData();
                if (AppNo == null)
                {
                    data.APPLICATIONNO = "0";
                }
                else
                {
                    data.APPLICATIONNO = AppNo;
                }
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 3;
                ViewBag.Quater = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                //***ViewBag.DistrictData = _DistrictRepository.GetBlockByDist(0).Result;***//


                ViewBag.DistrictData = _DistrictRepository.GetDistData(0).Result;
                ViewBag.bid = blockid;
                ViewBag.District = Distid;
                if (FreqNo != 0)
                {
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = (int)FreqId;
                    bd.FREQUENCYNO = (int)FreqNo;
                    bd.STATUS = (int)Status;
                    bd.APPLICATIONNO = AppNo;
                    bd.BLOCKID = (int)LeveDetailId;
                    bd.YEAR = Year;
                    BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    if (bdByAppNo != null)
                    {
                        ViewBag.bid = bdByAppNo.BLOCKID;
                        ViewBag.District = bdByAppNo.DISTRICTID;
                    }
                    else
                    {
                        ViewBag.bid = blockid;
                        ViewBag.District = Distid;
                    }
                    ViewBag.Sector = _panelRepository.ViewPanel();
                    ControlPanel cp = new ControlPanel();
                    cp.FREQUENCYID = (int)FreqId;
                    cp.FREQUENCYNO = (int)FreqNo;
                    cp.YEARTYPE = (int)Year;
                    List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                    {
                        dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, bdByAppNo.APPLICATIONNO));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                    }
                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                    ViewBag.Result = dps;
                    int result = 0;
                    StringBuilder sbScript = new StringBuilder();
                    List<ABP.Domain.DataPointExpression.DatapointExpression> ExList = (List<ABP.Domain.DataPointExpression.DatapointExpression>)_cpanelRepository.GetAllExpressionData().Result;
                    foreach (var E in ExList)
                    {

                        List<string> DPList = E.ExpressionNAMEWithID.Split(',').ToList();
                        foreach (var i in DPList)
                        {
                            foreach (var D in dps)
                            {
                                if (Convert.ToString(D.CONTROLID) == i)
                                {

                                    result = result + 1;
                                }

                            }

                        }

                        if (result == 2)
                        {
                            if (sbScript.ToString() == "")
                            {
                                sbScript.Append(E.Script);
                            }
                            else
                            {
                                if (sbScript.ToString().Contains("[Next]"))
                                {
                                    string CurrentExpression = sbScript.ToString();
                                    string NextExpression = CurrentExpression.Replace("[Next]", E.Script);
                                    sbScript.Replace(CurrentExpression, NextExpression);
                                }
                            }
                        }
                        result = 0;
                    }

                    if (sbScript.ToString().Contains("[Next]"))
                    {
                        string CurrentExpression = sbScript.ToString();
                        string NextExpression = CurrentExpression.Replace("[Next]", "if(1==1){ return true;}");
                        sbScript.Replace(CurrentExpression, NextExpression);

                    }
                    if (sbScript.ToString() == "")
                    {
                        sbScript.Append("if(1==1){ return true;}");
                    }
                    ViewBag.Script = sbScript.ToString();
                }
                else
                {

                }

                ViewBag.AppNo = AppNo;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult QuaterlyBlockDataEntryTemp([FromBody] List<ControlPanel> cpListObj)
        {
            OracleTransaction objTrans = null;

            using (OracleConnection objConn = new OracleConnection(_constr))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    Log.Information("QuaterlyBlockDataEntry started");
                    var UserId = HttpContext.Session.GetInt32("_UserId");
                    if (!string.IsNullOrEmpty(UserId.ToString()))
                    {
                        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                        bool isvalid = true;
                        if (cpListObj != null && cpListObj.Count > 0)
                        {
                            if (isvalid == false)
                            {
                                return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                            }
                            else
                            {
                                bool IsInsert = true;
                                string dateString = string.Empty;
                                int Year = 0;
                                if (cpListObj[0].FREQUENCYID == 5)
                                {
                                    Year = Convert.ToInt32(cpListObj[0].FROMDATE.Split('-')[2]);
                                }
                                else
                                {
                                    Year = Convert.ToInt32(cpListObj[0].TODATE.Split('-')[2]);
                                }
                                BlockData bd = new BlockData();
                                bd.BLOCKID = cpListObj[0].BLOCKID;
                                bd.FREQUENCYID = cpListObj[0].FREQUENCYID;
                                bd.FREQUENCYNO = cpListObj[0].FREQUENCYNO;
                                bd.APPLICATIONNO = cpListObj[0].APPLICATIONO;
                                bd.YEAR = Year;
                                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                if (bdByAppNo == null)
                                {
                                    bd.STATUS = 3;
                                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                    if (bdByAppNo == null)
                                    {
                                        bd.STATUS = 2;
                                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        if (bdByAppNo == null)
                                        {
                                            bd.STATUS = 1;
                                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        }
                                    }
                                }
                                //timestamp for the unique application no
                                if (bdByAppNo == null)
                                {
                                    IsInsert = true;
                                    dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                else
                                {
                                    IsInsert = false;
                                    dateString = bdByAppNo.APPLICATIONNO;
                                }
                                //for each panel there will be different application no for the main table 
                                BlockData blockData = (from e in cpListObj
                                                       select new BlockData
                                                       {
                                                           PANELID = e.PANELID,
                                                           PANELNAME = e.PANELNAME,
                                                           APPLICATIONNO = dateString,
                                                           FREQUENCYID = e.FREQUENCYID,
                                                           FREQUENCYNO = GetFrequencyNoByFreqValue(e.FREQUENCYVALUE),
                                                           FREQUENCYVALUE = e.FREQUENCYVALUE,
                                                           YEAR = Year,
                                                           FROMDATE = e.FROMDATE,
                                                           TODATE = e.TODATE,
                                                           DISTRICTID = Convert.ToInt32(e.DISTID),
                                                           BLOCKID = Convert.ToInt32(e.BLOCKID),
                                                           CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId")),
                                                           STATUS = e.STATUS,
                                                           DPCOUNT = cpListObj.Count(),
                                                           NONZERODPCOUNT = cpListObj.Where(u => u.CONTROLVALUE != 0).Count()
                                                       }).FirstOrDefault();
                                string RetValMain = string.Empty;
                                string RetValR = string.Empty;
                                string RetMsg = string.Empty;
                                if (IsInsert)
                                {
                                    //inserting the main data 
                                    RetValMain = _blockDataRepository.AddBlockDataMain(blockData);
                                }
                                else
                                {
                                    //Updating the status on the basis of applicationno here
                                    RetValMain = _blockDataRepository.UpdateBlockDataMain(blockData);
                                }
                                //making query for insertion and updation dynamically
                                List<StringBuilder> sbList = new List<StringBuilder>();
                                StringBuilder sb = null;
                                foreach (ControlPanel cp in cpListObj.GroupBy(x => x.PANELID).Select(x => x.FirstOrDefault()))
                                {
                                    List<string> columns = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLNAME).ToList();
                                    List<string> values = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLVALUE == null ? "null" : u.CONTROLVALUE.ToString()).ToList();
                                    //for panel wise data entry
                                    BlockData bds = new BlockData();
                                    bds.APPLICATIONNO = dateString;
                                    bds.PANELID = cp.PANELID;
                                    bds.STATUS = cpListObj[0].STATUS;
                                    bds.REMARK = string.Empty;
                                    RetValR = _districtDataRepository.RejectASectorData(bds);
                                    var logtablename = cp.PANELNAME + "_LOG";
                                    if (IsInsert)
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + cp.PANELNAME + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + cp.PANELNAME + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                    }
                                    else
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("update T_ABP_" + cp.PANELNAME + " set ");
                                        int count = 0;
                                        foreach (string col in columns)
                                        {
                                            if (count == columns.Count - 1)
                                            {
                                                sb.Append(col + "=" + values[count]);
                                            }
                                            else
                                            {
                                                sb.Append(col + "=" + values[count] + ",");
                                            }
                                            count++;
                                        }
                                        sb.Append(" where APPLICATIONNO='" + dateString + "'");
                                        sbList.Add(sb);
                                    }
                                }
                                RetMsg = _blockDataRepository.SubmitBlockData(sbList);
                                if (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 0 && RetValR == "1")
                                {
                                    return Json(new { status = 1, freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved Successfully!" });
                                }
                                else if ((RetValMain == "2" && RetMsg == "1") || (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 2) && RetValR == "1")
                                {
                                    if (cpListObj[0].STATUS == 0)
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved Successfully!" });
                                    }
                                    else
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Submited Successfully!" });
                                    }
                                }
                                else
                                {
                                    Log.Information("Data couldn't entered to the specific table");
                                    return Json(new { msg = "Something went wrong!" });
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
                    objTrans.Commit();
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }
            }
        }
        public IActionResult QuaterlyViewBlockData()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData bd = new BlockData();
                //bd.BLOCKID = (int)LeveDetailId;
                bd.FREQUENCYID = 3;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _blockDataRepository.ViewFrequency().Result;
                List<BlockData> blockDatas = _blockDataRepository.GetAllBlockData(bd).Result.ToList();
                blockDatas.ForEach((c) => c.DPCOUNT = _blockDataRepository.GetEnteredDatapoints(c.APPLICATIONNO, "T_ABP_" + c.TABLENAME).Result.ToList().Count);
                ViewBag.Result = blockDatas;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuaterlyViewBlockData(BlockData blockdata)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //var ACTION = "BDO";
                ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
                ViewBag.Frequency = _blockDataRepository.ViewFrequency().Result;
                ViewBag.year = blockdata.YEAR;
                List<BlockData> blockDatas = _blockDataRepository.GetAllBlockData(blockdata).Result.ToList();
                blockDatas.ForEach((c) => c.DPCOUNT = _blockDataRepository.GetEnteredDatapoints(c.APPLICATIONNO, "T_ABP_" + c.TABLENAME).Result.ToList().Count);
                ViewBag.Result = blockDatas;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }

        }
        public IActionResult BlockDataViewTempYearly(string AppNo, int Year, int FreqId = 5, int Status = 0, int FreqNo = 0, int blockid = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                Report rpt = _reportRepository.GetApplicationNOyearly(blockid, Year, FreqId).Result;
                ViewBag.block = rpt.BLOCKNAME;
                ViewBag.dist = rpt.Districtname;
                BlockData data = new BlockData();
                data.APPLICATIONNO = rpt.APPLICATIONNO;
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                if (FreqId == 5)
                {
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = (int)FreqId;
                    bd.FREQUENCYNO = (int)FreqNo;
                    bd.STATUS = (int)Status;
                    bd.APPLICATIONNO = rpt.APPLICATIONNO;
                    bd.BLOCKID = (int)rpt.BlockId;
                    bd.YEAR = Year;
                    BlockData bdByAppNo = _blockDataRepository.GetBlockDatas(bd).Result.FirstOrDefault();
                    ViewBag.Sector = _panelRepository.ViewPanel();
                    ControlPanel cp = new ControlPanel();
                    cp.FREQUENCYID = (int)FreqId;
                    cp.FREQUENCYNO = (int)FreqNo;
                    cp.YEARTYPE = (int)Year;
                    List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                    {
                        dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, bdByAppNo.APPLICATIONNO));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                    }
                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                    ViewBag.Result = dps;
                }
                else
                {

                }

                ViewBag.AppNo = AppNo;
                return View(rpt);
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult BlockDataViewTemp(string method, string AppNo, int Year, int FreqId = 2, int Status = 2, int FreqNo = 0, int blockid = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                string applicationnoyr = "";
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                Report rpt = _reportRepository.GetApplicationNO(blockid, Year, FreqNo, FreqId).Result;
                if (rpt == null)
                {
                    rpt= _reportRepository.GetApplicationNO(blockid, Year, FreqNo, 5).Result;
                }

                ViewBag.method = method;
                ViewBag.block = rpt.BLOCKNAME;
                ViewBag.dist = rpt.Districtname;
                BlockData data = new BlockData();
                data.APPLICATIONNO = rpt.APPLICATIONNO;
                //data.APPLICATIONNO = rpt1.APPLICATIONNO;
                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                if (FreqNo != 0)
                {
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = (int)FreqId;
                    bd.FREQUENCYNO = (int)FreqNo;
                    bd.STATUS = (int)Status;
                    bd.APPLICATIONNO = rpt.APPLICATIONNO;
                    bd.BLOCKID = (int)rpt.BlockId;
                    bd.YEAR = Year;
                    BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    if(bdByAppNo==null)
                    {
                        bd.FREQUENCYID = 5;
                        bd.FREQUENCYNO = Year;
                        bd.MONTHNO = (int)FreqNo;
                        bd.APPLICATIONNO = "0";
                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    }

                    ViewBag.Sector = _panelRepository.ViewPanel();
                    ControlPanel cp = new ControlPanel();
                    cp.FREQUENCYID = (int)FreqId;
                    cp.FREQUENCYNO = (int)FreqNo;
                    cp.YEARTYPE = (int)Year;
                    if (cp.APPLICATIONO == null)
                    {
                        cp.APPLICATIONO = rpt.APPLICATIONNO;
                        cp.BLOCKID = rpt.BlockId;
                        //bdByAppNo.APPLICATIONNO = rpt.APPLICATIONNO;
                        //bdByAppNo.BLOCKID = rpt.BlockId;
                    }
                    List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    cp.FREQUENCYID = 5;
                    List<ControlPanel> ydps = ViewBag.Results1 = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    dps.AddRange(ydps);

                    //  List<ControlPanel> YearlyAppNo = ViewBag.Results2 = _blockDataRepository.GetYearlyAppNo(cp).Result;
                    string YearlyAppNo = _blockDataRepository.GetYearlyAppNo((int)blockid, FreqNo, Year);

                    if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                    {

                        dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, c.FREQUENCYID == 5 ? YearlyAppNo : bdByAppNo.APPLICATIONNO));
                        //dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, YearlyAppNo[0].APPLICATIONO));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                    }

                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                    ViewBag.Result = dps;
                }
                else
                {

                }

                ViewBag.AppNo = AppNo;
                return View(rpt);
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult BlockDataViewTempV2(string method, string AppNo, int Year, int FreqId = 0, int Status = 0, int FreqNo = 0, int blockid = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                Report rpt = new Report();
                if (FreqId == 5)
                {
                    rpt = _reportRepository.GetApplicationNOyearly(blockid, Year, FreqId).Result;
                }
                else
                {
                    rpt = _reportRepository.GetApplicationNO(blockid, Year, FreqNo, FreqId).Result;
                }
                List<BlockDpData> Dpdt = _reportRepository.GetBlockDatapoint(blockid, rpt.APPLICATIONNO).Result.ToList();
                ViewBag.Sector = _panelRepository.ViewPanel();
                ViewBag.method = method;
                ViewBag.block = rpt.BLOCKNAME;
                ViewBag.dist = rpt.Districtname;
                ViewBag.AppNo = AppNo;
                ViewBag.Result = Dpdt;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }


        [HttpGet]
        public IActionResult GetBlockByDistID(int id)
        {
            var result = _DistrictRepository.GetBlockByDist(id).Result;
            return Json(result);
        }
        public IActionResult BlockDataEntryTempYearly(string AppNo, int Year, int? FreqId = 5, int? Status = 0, int? FreqNo = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                FreqId = 5;
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                BlockData data = new BlockData();
                if (AppNo == null)
                {
                    data.APPLICATIONNO = "0";
                }
                else
                {
                    data.APPLICATIONNO = AppNo;
                }

                data.BLOCKID = (int)LeveDetailId;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)Year;
                ViewBag.Year = Year;
                FreqNo = Year;
                if (FreqNo != 0)
                {
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = (int)FreqId;
                    bd.FREQUENCYNO = (int)Year;
                    bd.STATUS = (int)Status;
                    bd.APPLICATIONNO = AppNo;
                    bd.BLOCKID = (int)LeveDetailId;
                    bd.YEAR = Year;
                    BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    ViewBag.Sector = _panelRepository.ViewPanel();
                    ControlPanel cp = new ControlPanel();
                    cp.FREQUENCYID = (int)FreqId;
                    cp.FREQUENCYNO = (int)FreqNo;
                    cp.YEARTYPE = (int)Year;
                    List<ControlPanel> dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                    {
                        dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, bdByAppNo.APPLICATIONNO));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                    }
                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                    ViewBag.Result = dps;
                    int result = 0;
                    StringBuilder sbScript = new StringBuilder();
                    List<ABP.Domain.DataPointExpression.DatapointExpression> ExList = (List<ABP.Domain.DataPointExpression.DatapointExpression>)_cpanelRepository.GetAllExpressionData().Result;
                    foreach (var E in ExList)
                    {

                        List<string> DPList = E.ExpressionNAMEWithID.Split(',').ToList();
                        foreach (var i in DPList)
                        {
                            foreach (var D in dps)
                            {
                                if (Convert.ToString(D.CONTROLID) == i)
                                {

                                    result = result + 1;
                                }

                            }

                        }

                        if (result == 2)
                        {
                            if (sbScript.ToString() == "")
                            {
                                sbScript.Append(E.Script);
                            }
                            else
                            {
                                if (sbScript.ToString().Contains("[Next]"))
                                {
                                    string CurrentExpression = sbScript.ToString();
                                    string NextExpression = CurrentExpression.Replace("[Next]", E.Script);
                                    sbScript.Replace(CurrentExpression, NextExpression);
                                }
                            }
                        }
                        result = 0;
                    }

                    if (sbScript.ToString().Contains("[Next]"))
                    {
                        string CurrentExpression = sbScript.ToString();
                        string NextExpression = CurrentExpression.Replace("[Next]", "if(1==1){ return true;}");
                        sbScript.Replace(CurrentExpression, NextExpression);

                    }
                    if (sbScript.ToString() == "")
                    {
                        sbScript.Append("if(1==1){ return true;}");
                    }
                    ViewBag.Script = sbScript.ToString();
                }
                else
                {

                }

                ViewBag.AppNo = AppNo;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult BlockDataEntryTempYearly([FromBody] List<ControlPanel> cpListObj, int? year = 0)
        {
            OracleTransaction objTrans = null;

            using (OracleConnection objConn = new OracleConnection(_constr))
            {
                objConn.Open();
                objTrans = objConn.BeginTransaction();
                try
                {
                    Log.Information("BlockDataEntry started");
                    var UserId = HttpContext.Session.GetInt32("_UserId");
                    if (!string.IsNullOrEmpty(UserId.ToString()))
                    {
                        var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                        bool isvalid = true;
                        if (cpListObj != null && cpListObj.Count > 0)
                        {
                            if (isvalid == false)
                            {
                                return Json(new { status = 2, msg = "Numerator cannot be Greater Then Denominator " });
                            }
                            else
                            {
                                bool IsInsert = true;
                                string dateString = string.Empty;
                                int Year = 0;
                                if (cpListObj[0].FREQUENCYID == 5)
                                {
                                    //Year = Convert.ToInt32(cpListObj[0].FROMDATE.Split('-')[2]);
                                    Year = cpListObj[0].FREQUENCYNO;
                                }
                                else
                                {
                                    Year = Convert.ToInt32(cpListObj[0].TODATE.Split('-')[2]);
                                }
                                BlockData bd = new BlockData();
                                bd.BLOCKID = (int)LeveDetailId;
                                bd.FREQUENCYID = cpListObj[0].FREQUENCYID;
                                bd.FREQUENCYNO = cpListObj[0].FREQUENCYNO;
                                bd.APPLICATIONNO = cpListObj[0].APPLICATIONO;
                                bd.YEAR = Year;
                                BlockData bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                if (bdByAppNo == null)
                                {
                                    bd.STATUS = 3;
                                    bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                    if (bdByAppNo == null)
                                    {
                                        bd.STATUS = 2;
                                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        if (bdByAppNo == null)
                                        {
                                            bd.STATUS = 1;
                                            bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                                        }
                                    }
                                }
                                //timestamp for the unique application no
                                if (bdByAppNo == null)
                                {
                                    IsInsert = true;
                                    dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
                                }
                                else
                                {
                                    IsInsert = false;
                                    dateString = bdByAppNo.APPLICATIONNO;
                                }
                                Log.Information("Is Insert?" + IsInsert + "& Application No?" + dateString);
                                //for each panel there will be different application no for the main table 
                                BlockData blockData = (from e in cpListObj
                                                       select new BlockData
                                                       {
                                                           PANELID = e.PANELID,
                                                           PANELNAME = e.PANELNAME,
                                                           APPLICATIONNO = dateString,
                                                           FREQUENCYID = e.FREQUENCYID,
                                                           FREQUENCYNO = GetFrequencyNoByFreqValue(e.FREQUENCYVALUE),
                                                           FREQUENCYVALUE = e.FREQUENCYVALUE,
                                                           YEAR = Year,
                                                           FROMDATE = e.FROMDATE,
                                                           TODATE = e.TODATE,
                                                           DISTRICTID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           BLOCKID = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId")),
                                                           CREATEDBY = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId")),
                                                           STATUS = e.STATUS,
                                                           HNDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_1").Where(u => u.CONTROLVALUE != null).Count(),
                                                           AGDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_2").Where(u => u.CONTROLVALUE != null).Count(),
                                                           BIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_4").Where(u => u.CONTROLVALUE != null).Count(),
                                                           FIDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_5").Where(u => u.CONTROLVALUE != null).Count(),
                                                           EDDPCOUNT = cpListObj.Where(x => x.PANELNAME == "PANEL_3").Where(u => u.CONTROLVALUE != null).Count(),
                                                           DPCOUNT = cpListObj.Count(),
                                                           NONZERODPCOUNT = cpListObj.Where(u => u.CONTROLVALUE != null).Count()
                                                       }).FirstOrDefault();
                                string RetValMain = string.Empty;
                                string RetValR = string.Empty;
                                string RetMsg = string.Empty;
                                if (IsInsert)
                                {
                                    //inserting the main data 
                                    Log.Information("Data inserting to block data entry table");
                                    RetValMain = _blockDataRepository.AddBlockDataMain(blockData);
                                }
                                else
                                {
                                    //Updating the status on the basis of applicationno here
                                    Log.Information("Data updating to block data entry table");
                                    RetValMain = _blockDataRepository.UpdateBlockDataMain(blockData);
                                }
                                //making query for insertion and updation dynamically
                                List<StringBuilder> sbList = new List<StringBuilder>();
                                StringBuilder sb = null;
                                foreach (ControlPanel cp in cpListObj.GroupBy(x => x.PANELID).Select(x => x.FirstOrDefault()))
                                {
                                    List<string> columns = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLNAME).ToList();
                                    List<string> values = cpListObj.Where(u => u.PANELID == cp.PANELID).OrderBy(u => u.CONTROLID).Select(u => u.CONTROLVALUE == null ? "null" : u.CONTROLVALUE.ToString()).ToList();
                                    //for panel wise data entry
                                    BlockData bds = new BlockData();
                                    bds.APPLICATIONNO = dateString;
                                    bds.PANELID = cp.PANELID;
                                    bds.STATUS = cpListObj[0].STATUS;
                                    bds.REMARK = string.Empty;
                                    RetValR = _districtDataRepository.RejectASectorData(bds);
                                    var logtablename = cp.PANELNAME + "_LOG";
                                    if (IsInsert)
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + cp.PANELNAME + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + cp.PANELNAME + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                    }
                                    else
                                    {
                                        sb = new StringBuilder();
                                        sb.Append("insert into T_ABP_" + logtablename + " (ID,APPLICATIONNO," + string.Join(",", columns) + ") values (T_ABP_" + logtablename + "_SEQ.nextval,'" + dateString + "'," + string.Join(",", values) + ")");
                                        sbList.Add(sb);
                                        sb = new StringBuilder();
                                        sb.Append("update T_ABP_" + cp.PANELNAME + " set ");
                                        int count = 0;
                                        foreach (string col in columns)
                                        {
                                            if (count == columns.Count - 1)
                                            {
                                                sb.Append(col + "=" + values[count]);
                                            }
                                            else
                                            {
                                                sb.Append(col + "=" + values[count] + ",");
                                            }
                                            count++;
                                        }
                                        sb.Append(" where APPLICATIONNO='" + dateString + "'");
                                        sbList.Add(sb);
                                    }
                                }
                                RetMsg = _blockDataRepository.SubmitBlockData(sbList);
                                //Getting Block Details to send the message
                                Login objUserDetails = new Login();
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_UserId");
                                var Result = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                //Getting Corresponding Block's District Details to send the message to collector
                                objUserDetails.INTUSERID = (int)HttpContext.Session.GetInt32("_PARENTID");
                                var DistResult = _loginRepository.GetLoginDetailsByUserId(objUserDetails).Result.FirstOrDefault();
                                if (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 0 && RetValR == "1")
                                {
                                    #region Text Message
                                    var mobno = DistResult.MOBILENO;
                                    var blockname = DistResult.BlockName;
                                    var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    var templateid = "1007168066714211943";
                                    var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    #endregion
                                    return Json(new { status = 1, freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved Successfully!" });

                                }
                                else if ((RetValMain == "2" && RetMsg == "1") || (RetValMain == "1" && RetMsg == "1" && cpListObj[0].STATUS == 2) && RetValR == "1")
                                {
                                    if (cpListObj[0].STATUS == 0)
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Saved Successfully!" });
                                    }
                                    else
                                    {
                                        return Json(new { freqid = cpListObj[0].FREQUENCYID, msg = "Records Submited Successfully!" });
                                    }
                                    //#region Text Message
                                    //var mobno = DistResult.MOBILENO;
                                    //var blockname = DistResult.BlockName;
                                    //var template = "Dear user, abp data for " + blockname + " block has been submitted for the month " + cpListObj[0].FREQUENCYVALUE + " for your kind approval by 15th. odisha state dashboard.";
                                    //var templateid = "1007168066714211943";
                                    //var smsresult = _sms.Sendsms(mobno, template, templateid);
                                    //#endregion
                                }
                                else
                                {
                                    Log.Information("Data couldn't entered to the specific table");
                                    return Json(new { msg = "Something went wrong!" });
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
                    objTrans.Commit();
                    Log.Information("BlockDataEntry ended");
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
                finally
                {
                    objConn.Close();
                }
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetBdoDtaAlert()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {

                    var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                    var Blockdatalist = await _blockDataRepository.GetBDOAlertData((int)LeveDetailId);
                    return Json(Blockdatalist);


                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
                return null;
            }

        }
        public IActionResult BlockWiseDPView(int Year,int sector,int indicatorid=0, int FreqId = 2, int Status = 2, int FreqNo = 0, int blockid = 0)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                string applicationnoyr = "";
                var LeveDetailId = HttpContext.Session.GetInt32("_LeveDetailId");
                if (blockid == 0)
                {
                    blockid = Convert.ToInt32(LeveDetailId);
                }
                Report rpt = new Report();
                if (FreqId == 5)
                {
                    rpt = _reportRepository.ApplicationNOyearly(blockid, Year, FreqId, FreqNo).Result;
                }
                else
                {
                    rpt = _reportRepository.GetApplicationNO(blockid, Year, FreqNo, FreqId).Result;
                }
                ViewBag.block = rpt.BLOCKNAME;
                ViewBag.dist = rpt.Districtname;
                BlockData data = new BlockData();
                data.APPLICATIONNO = rpt.APPLICATIONNO;
                data.BLOCKID = blockid;
                data.FREQUENCYID = 5;
                data.YEAR = Year;
                data.FREQUENCYNO = (int)FreqNo;
                ViewBag.Year = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                data.FREQUENCYID = 2;
                ViewBag.Month = _blockDataRepository.GetFrequencyValue(data).Result.ToList();
                if (FreqNo != 0)
                {
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = (int)FreqId;
                    bd.FREQUENCYNO = (int)FreqNo;
                    bd.STATUS = (int)Status;
                    bd.APPLICATIONNO = rpt.APPLICATIONNO;
                    bd.BLOCKID = (int)rpt.BlockId;
                    bd.YEAR = Year;
                    BlockData bdByAppNo = new BlockData();
                    if (FreqId == 5)
                    {
                        bdByAppNo = _blockDataRepository.GetBlockEnteredDataByYear(bd).Result.FirstOrDefault();
                    }
                    else
                    {
                        bdByAppNo = _blockDataRepository.GetBlockData(bd).Result.FirstOrDefault();
                    }
                    ViewBag.Sector = _panelRepository.ViewPanel().Result.ToList().Where(x => x.PANELID == sector).ToList();
                    ControlPanel cp = new ControlPanel();
                    cp.FREQUENCYID = (int)FreqId;
                    cp.FREQUENCYNO = (int)FreqNo;
                    cp.YEARTYPE = (int)Year;
                    if (cp.APPLICATIONO == null)
                    {
                        cp.APPLICATIONO = rpt.APPLICATIONNO;
                        cp.BLOCKID = rpt.BlockId;
                    }
                    List<ControlPanel> dps = new List<ControlPanel>();
                    if (indicatorid != 0)
                    {
                        cp.INDICATORID = indicatorid;
                        dps = ViewBag.Results = _blockDataRepository.GetControlsByIndicators(cp).Result;
                    }
                    else
                    {
                        dps = ViewBag.Results = _blockDataRepository.GetControlsByFrequency(cp).Result;
                    }

                    if (bdByAppNo != null && bdByAppNo.APPLICATIONNO != null)
                    {
                        dps.ForEach((c) => c.CONTROLVALUE = _blockDataRepository.GetContolValue(c, bdByAppNo.APPLICATIONNO));
                        dps.ForEach((c) => c.DataEntryEligibility = true);
                        dps.ForEach((c) => c.TODATE = Convert.ToDateTime(bdByAppNo.TODATE).ToString("dd-MMM-yyyy"));
                        dps.ForEach((c) => c.FROMDATE = Convert.ToDateTime(bdByAppNo.FROMDATE).ToString("dd-MMM-yyyy"));
                    }

                    dps.ForEach((c) => c.DataEntryEligibility = true);
                    dps.ForEach((c) => c.FREQUENCYVALUE = GetFrequencyValueByFreqId(FreqId, Convert.ToDateTime(c.TODATE)));
                    ViewBag.Result = dps;
                }
                else
                {

                }
                return View(rpt);
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult RejectedDetails()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                int leveldetailid = Convert.ToInt32(HttpContext.Session.GetInt32("_LeveDetailId"));
                ViewBag.RejectedResult = _blockDataRepository.ViewRejectedBlockDetails(leveldetailid).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
