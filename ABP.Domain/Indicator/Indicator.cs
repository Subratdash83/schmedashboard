using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ABP.Domain.Indicator
{
    public class Indicator
    {
        public string DISTRICT { get; set; }
        public string BLOCK { get; set; }
        public string FREQUENCY_TYPE { get; set; }
        public int BLOCKCODE { get; set; }
        public int INDICATORID { get; set; }
        public int SECTORID { get; set; }
        public int NBDODATAID { get; set; }
        public int DBDODATAID { get; set; }
        public string SECTORNAME { get; set; }
        public string PANELNAME { get; set; } 
        public string INDICATORNAME { get; set; }
        public decimal INDICATORVALUE { get; set; }
        public decimal NDATAPOINTVALUE { get; set; }
        public int NDATAPOINTID { get; set; }
        public int DDATAPOINTID { get; set; }
        public string UNIT { get; set; }
        public string COLLECTORREMARK { get; set; }
        public decimal DDATAPOINTVALUE { get; set; }                 
        public string NDDATAPOINTNAME { get; set; }
        public string DDDATAPOINTNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string INDICATORDATE { get; set; }
        public int INDICATORTYPE { get; set; }
        public decimal LASTVALUE { get; set; }
        public int BLOCKID { get; set; }
        public int STATUS { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public int IndType { get; set; }
        public string CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int DELETEDFLAG { get; set; }
        public int TARGETVALUE { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public int FREQUENCYNO { get; set; }
        public string FROMDATE { get; set; }
        public string TODATE { get; set; }
        public int YEAR { get; set; }
        public decimal NDPVALUE { get; set; }
        public decimal DDPVALUE { get; set; }
        public string MONTHNAME { get; set; }
        public int ID { get; set; }
        public int DEPTID { get; set; }
        public string DEPTNAME { get; set; }
        public XElement ApprovalConfing { get; set; }
        public string IndicatorFormulaName { get; set; }
        public string IndicatorFormulaID { get; set; }
        public string SelectedDataPoints { get; set; }
        public string  DPQuery { get; set; }
        public string ApplicationNo { get; set; }
        public int FREQUENCYID { get; set; }
        public string FREQUENCY { get; set; }
        public int DISTRICTID { get; set; }
        public int DISTRICTCODE { get; set; }
        public string IndicatorTrend { get; set; }

        public int Count { get; set; }
    }
   
}
