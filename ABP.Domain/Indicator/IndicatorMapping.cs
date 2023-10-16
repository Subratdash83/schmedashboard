using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Indicator
{
    public class IndicatorMapping
    {
        public int iscensus { get; set; }
        public int INDICATORMAPPINGID { get; set; }
        public int SECTORID { get; set; }
        public string SECTORNAME { get; set; }
        public int INDICATORID { get; set; }
        public string INDICATORNAME { get; set; }
        public int DDATAPOINTID { get; set; }
        public int NDATAPOINTID { get; set; }
        public string DDATAPOINTNAME { get; set; }
        public string NDATAPOINTNAME { get; set; }
        public string FORMULA { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int DELETEDFLAG { get; set; }
    }
}
