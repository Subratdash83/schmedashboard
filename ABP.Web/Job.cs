using ABP.Domain.BlockData;
using ABP.Domain.Indicator;
using ABP.Infrastructure;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.SMSTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Web
{
    public class Job
    {
        #region Property
        private readonly IDistrictRepository _DistrictRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IDistrictDataRepository _districtDataRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly ISendSMSRepository _sms;
        private readonly ISMSTemplateRepository _SMSTemplateRepository;
        private readonly IBlockDataRepository _blockDataRepository;
        //private readonly IEmployeeService _employeeService;
        #endregion

        #region Constructor
        public Job(IDistrictRepository isdtrictRepository, IBlockRepository blockRepository, IDistrictDataRepository districtDataRepository, IIndicatorRepository indicatorRepository, ISendSMSRepository sms, ISMSTemplateRepository SMSTemplateRepository, IBlockDataRepository blockDataRepository)
        {
            _sms = sms;
            _DistrictRepository = isdtrictRepository;
            _blockRepository = blockRepository;
            _districtDataRepository = districtDataRepository;
            _indicatorRepository = indicatorRepository;
            _SMSTemplateRepository = SMSTemplateRepository;
            _blockDataRepository = blockDataRepository;
        }
        #endregion

        #region Job Scheduler
        #region Indicator Score Calculation Monthwise
        public int JobIndScoreCalculation()
        {
            int distcode = 0; int freqid = 2;
            int BLOCK_CODE = 0;
            try
            {
                string JOBID = _SMSTemplateRepository.InsertJobRecord(0, freqid, 0, "INDMONTHLY");

                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                // Create an int array of district to store the DistrictId values
                int monthno = (DateTime.Now.Month) - 1;
                int[] distarr = _DistrictRepository.GetDistData(0).Result.Select(item => item.BLOCK_CODE).ToArray();
                for (int j = 0; j < distarr.Length; j++)
                {
                    distcode = distarr[j];
                    // List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                    int[] blockarr = _blockRepository.GetBlockByDistId(distcode).Result.Select(item => item.BLOCK_CODE).ToArray();
                    // distcode = distarr[j];
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = 5;
                    bd.FREQUENCYID = freqid;
                    if (freqid == 2)
                    {
                        List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                        // foreach (var block in Blocks)
                        for (int block = 0; block < blockarr.Length; block++)
                        {
                            BLOCK_CODE = blockarr[block];
                            int yr = DateTime.Now.Year;

                            for (var mon = 1; mon <= 12; mon++)
                            {

                                foreach (var cv in Indicators)
                                {
                                    IndicatorValueScore isc = new IndicatorValueScore();
                                    if (cv.IndicatorFormulaName != null)
                                    {
                                        if (cv.IndicatorFormulaName != null && cv.IndicatorFormulaName.Contains("[condition1]"))

                                        {
                                            string numeratorCondition = " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=2 and year=" + yr + " and frequencyno=" + mon + " and blockid=" + BLOCK_CODE + " and status in(1,2))";
                                            //Condition for the denominator

                                            string denominatorCondition = " WHERE Applicationno IN (SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH|| '-' || year,'mm-yyyy') <= to_date('" + mon + "-" + yr + "', 'mm-yyyy') AND BLOCKID = " + BLOCK_CODE + " and status in(1, 2)) ORDER BY ID DESC";
                                            // Replace [condition] with the condition for the numerator
                                            string queryWithNumeratorCondition = cv.IndicatorFormulaName.Replace("[condition]", numeratorCondition);
                                            // Replace [condition1] with the condition for the denominator
                                            string finalQuery = queryWithNumeratorCondition.Replace("[condition1]", denominatorCondition);
                                            cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(finalQuery);
                                        }
                                        else
                                        {
                                            cv.INDICATORVALUE = _indicatorRepository.GetContolValueThroughQuery(cv.IndicatorFormulaName.Replace("[condition]", " where Applicationno=(select applicationno from t_abp_block_dataentry where frequencyid=2 and year=" + yr + " and frequencyno=" + mon + " and blockid=" + BLOCK_CODE + " and status in(1,2))"));
                                        }
                                    }
                                    cv.ApplicationNo = _indicatorRepository.GetAppValueThroughQuery("select applicationno from(select applicationno from t_abp_block_dataentry where frequencyid=2 and year <=" + yr + " and frequencyno <=" + mon + " and blockid=" + BLOCK_CODE + " and status in(1,2) order by id desc)where rownum=1");

                                    isc.BLOCKID = BLOCK_CODE;
                                    isc.FREQUENCYID = 2;
                                    isc.FREQUENCYNO = mon;
                                    isc.YEAR = yr;
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
                            isclist.Clear();
                            xEle.RemoveNodes();



                        }




                    }

                }
                string successid = _SMSTemplateRepository.UpdateJobRecord(1, Convert.ToInt32(JOBID));
                return 1;
            }
            catch (Exception ex)
            {
                // Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        #endregion
        #region Indicator Score Calculation Yearwise
        public int JobIndScoreCalculationYearly()
        {
            int distcode; int freqid = 5;
            try
            {
                string JOBID = _SMSTemplateRepository.InsertJobRecord(0, freqid, 0, "INDYEARLY");
                List<IndicatorValueScore> isclist = new List<IndicatorValueScore>();
                var xEle = new XDocument();
                var dec = new XDeclaration("1.0", "UTF-8", "yes");
                xEle.Declaration = dec;
                // Create an int array of district to store the DistrictId values
                int mnth = (DateTime.Now.Month) - 1;
                int year = DateTime.Now.Year;
                int[] distarr = _DistrictRepository.GetDistData(0).Result.Select(item => item.BLOCK_CODE).ToArray();
                for (int j = 0; j < distarr.Length; j++)
                {

                    distcode = distarr[j];
                    List<Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = freqid;
                    List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                    foreach (var block in Blocks)
                    {
                        foreach (var cv in Indicators)
                        {
                            IndicatorValueScore isc = new IndicatorValueScore();
                            if (cv.IndicatorFormulaName != null)
                            {
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
                    xEle.RemoveNodes();
                }
                string successid = _SMSTemplateRepository.UpdateJobRecord(1, Convert.ToInt32(JOBID));
                return 1;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        #endregion
        public async Task<int> JobAsync()
        {
            try
            {
                int distcode;
                int freqid;
                freqid = 2;
                //int mnth = 4;

                int[] distarr = {40, 48, 49, 50, 51,
                        52, 53, 54, 55, 56,
                                57, 58, 59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75};
                for (int i = 0; i < distarr.Length; i++)
                {
                    distcode = distarr[i];
                    List<ABP.Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = freqid;

                    // List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                    foreach (var block in Blocks)
                    {
                        for (int yr = 2021; yr <= 2023; yr++)
                        {
                            for (int mnth = 1; mnth <= 12; mnth++)
                            {
                                if (freqid == 2)
                                {
                                    int x = _DistrictRepository.INSERTAnalyticDataCopy(distcode, block.BLOCK_CODE, yr, mnth, freqid).Result;
                                    // int x = _DistrictRepository.INSERTAnalyticDataCopy(49, 257, 2023, 4, 2).Result;
                                }
                            }
                        }
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
            //return 1;
        }
        public async Task<int> JobAsyncYearly()
        {
            try
            {
                int distcode;
                int freqid;
                freqid = 5;
                //int mnth = 4;

                int[] distarr = {40, 48, 49, 50, 51,
                        52, 53, 54, 55, 56,
                                57, 58, 59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75};
                for (int i = 0; i < distarr.Length; i++)
                {
                    distcode = distarr[i];
                    List<ABP.Domain.Block.Block> Blocks = _blockRepository.GetBlockByDistId(distcode).Result.ToList();
                    BlockData bd = new BlockData();
                    bd.FREQUENCYID = freqid;

                    // List<Indicator> Indicators = _districtDataRepository.GetIndicatorsWithFormula(bd).Result.ToList();
                    foreach (var block in Blocks)
                    {
                        for (int yr = 2021; yr <= 2023; yr++)
                        {
                            //for (int mnth = 1; mnth <= 12; mnth++)
                            //{
                            if (freqid == 5)
                            {
                                int x = _DistrictRepository.INSERTAnalyticDataCopyYearly(distcode, block.BLOCK_CODE, yr, yr, freqid).Result;
                                //int x = _DistrictRepository.INSERTAnalyticDataCopy(distcode, block.BLOCK_CODE, yr, mnth, freqid).Result;
                                // int x = _DistrictRepository.INSERTAnalyticDataCopy(49, 257, 2023, 4, 2).Result;
                            }
                            //}
                        }
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
            //return 1;
        }
        public async Task<string> JobAsyncsms(ABP.Domain.SMS.SMSData sms)
        {
            try
            {


                // Log.Information("TestSMS Started");
                if (sms.TEMPLATETYPE == 4)
                {

                    DateTime dt = DateTime.Now.AddMonths(-1);
                    var Monthstr = dt.ToString("MMMM");
                    if (sms.USERTYPE == "BDO")
                    {
                        List<Domain.SMS.SMSData> UserDetails = new List<Domain.SMS.SMSData>();
                        UserDetails = _SMSTemplateRepository.GetUserDetailsForSMS(sms).Result.ToList();
                        if (sms.BLOCKID == 0)
                        {
                            foreach (var user in UserDetails)
                            {
                                string TemplateMsg = sms.TEMPLATEMESSAGE.Replace("#user", user.BLOCKNAME).Replace("#month", Monthstr);
                                var smsresult = _sms.Sendsms(user.USERMOBILE, TemplateMsg, "1007857426954177953");
                            }
                        }
                        else
                        {
                            var Block = _DistrictRepository.GetBlockByDist(sms.DISTRICTID).Result.ToList().Find(x => x.BLOCK_CODE == sms.BLOCKID).BLOCK_NAME;
                            string TemplateMsg = sms.TEMPLATEMESSAGE.Replace("#user", Block).Replace("#month", Monthstr);
                            var smsresult = _sms.Sendsms(UserDetails[0].USERMOBILE, TemplateMsg, "1007857426954177953");
                        }
                        //var smsresult = _sms.Sendsms(SMS1[0].USERMOBILE, sms.TEMPLATEMESSAGE, "1007168066714211943");
                        //var smsresult = _sms.Sendsms(SMS1[0].USERMOBILE, "Dear User, " + Block + " blocks have not submitted ABP data for the month " + dt.ToString("MMMM") + ". Odisha State Dashboard.", "1007857426954177953");
                    }
                    else
                    {
                        List<Domain.SMS.SMSData> DistUserDetails = new List<Domain.SMS.SMSData>();
                        DistUserDetails = _SMSTemplateRepository.GetUserDetailsForSMS(sms).Result.ToList();
                        if (sms.DISTRICTID == 0)
                        {
                            foreach (var Distuser in DistUserDetails)
                            {
                                ABP.Domain.SMS.SMSData DistU = new ABP.Domain.SMS.SMSData();
                                DistU.DISTRICTID = Distuser.DISTRICTID;
                                DistU.BLOCKID = 0;
                                List<Domain.SMS.SMSData> BlockDetails = new List<Domain.SMS.SMSData>();
                                BlockDetails = _SMSTemplateRepository.GetUserDetailsForSMS(DistU).Result.ToList();
                                if (BlockDetails != null)
                                {
                                    string BlockNotEnteredList()
                                    {
                                        return string.Join(",", from item in BlockDetails select item.BLOCKNAME);
                                    }
                                    string NotEnteredBlocks = BlockNotEnteredList();
                                    string TemplateMsgAll = sms.TEMPLATEMESSAGE.Replace("#user", NotEnteredBlocks).Replace("#month", Monthstr);
                                    var smsresultAll = _sms.Sendsms(Distuser.USERMOBILE, TemplateMsgAll, "1007857426954177953");
                                }
                            }
                        }
                        else
                        {
                            string BlockNotEnteredDistwiselist()
                            {
                                return string.Join(", ", from item in DistUserDetails select item.BLOCKNAME);
                            }
                            string NotEnteredBlock = BlockNotEnteredDistwiselist();
                            //var District = _DistrictRepository.GetDistData(0).Result.ToList().Find(x => x.BLOCK_CODE == sms.DISTRICTID).BLOCK_NAME;
                            string TemplateMsgAll = sms.TEMPLATEMESSAGE.Replace("#user", NotEnteredBlock).Replace("#month", Monthstr);
                            var smsresultAll = _sms.Sendsms(DistUserDetails[0].USERMOBILE, TemplateMsgAll, "1007857426954177953");
                        }
                    }

                }

                //return Ok(smsresult);

                return "Ok";
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
            //return 1;
        }
        public async Task<string> JobSMSAsyncsms(ABP.Domain.SMS.SMSData sms)
        {
            try
            {

                // Log.Information("TestSMS Started");
                if (sms.TEMPLATETYPE == 4)
                {

                    DateTime dt = DateTime.Now.AddMonths(-1);
                    var Monthstr = dt.ToString("MMMM");
                    if (sms.USERTYPE == "BDO")
                    {
                        if (sms.BLOCKID == 0 && sms.DISTRICTID == 0)
                        {



                            List<Domain.SMS.SMSData> UserDetails = new List<Domain.SMS.SMSData>();

                            UserDetails = _SMSTemplateRepository.PostSendSMS().Result.ToList();
                            if (sms.BLOCKID == 0)
                            {

                                foreach (var user in UserDetails)
                                {
                                    string TemplateMsg = sms.TEMPLATEMESSAGE.Replace("#user", user.BLOCKNAME).Replace("#month", Monthstr);

                                    string retMsg = _SMSTemplateRepository.InsertSendSMSDetail(user.DISTRICTID, user.BLOCKID, user.USERMOBILE, TemplateMsg);
                                    var smsresult = _sms.Sendsms(user.USERMOBILE, TemplateMsg, "1007857426954177953");
                                }
                            }

                        }
                    }
                    else if (sms.USERTYPE == "Collector")
                    {
                        if (sms.BLOCKID == 0 && sms.DISTRICTID == 0)
                        {
                            List<Domain.SMS.SMSData> distarr = new List<Domain.SMS.SMSData>();

                            distarr = _SMSTemplateRepository.PostSMScol().Result.ToList();

                            foreach (var duser in distarr)
                            {
                                //distcode = distarr[i];
                                List<Domain.SMS.SMSData> UserDetails = new List<Domain.SMS.SMSData>();

                                UserDetails = _SMSTemplateRepository.PostSendSMSCol(duser.DISTRICTID).Result.ToList();
                                if (sms.BLOCKID == 0)
                                {
                                    string result = "";

                                    foreach (var user in UserDetails)
                                    {
                                        result += user.BLOCKNAME + ",";


                                    }
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        result = result.TrimEnd(',');
                                    }
                                    string TemplateMsg = sms.TEMPLATEMESSAGE.Replace("#user", result).Replace("#month", Monthstr);

                                    string retMsg = _SMSTemplateRepository.InsertSendSMSDetail(duser.DISTRICTID, 0, duser.USERMOBILE, TemplateMsg);
                                    //var smsresult = _sms.Sendsms(user.USERMOBILE, TemplateMsg, "1007857426954177953");
                                }
                            }
                        }
                    }


                }

                //return Ok(smsresult);

                return "Ok";
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
            //return 1;
        }


        #endregion
    }
}