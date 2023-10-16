using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Indicator
{
    public class IndicatorValue
    {
        public string APPLICATIONNO { get; set; }
        public string UPDATEDON { get; set; } 
        public int DISTRICTCODE { get; set; }
        public string BLOCKCODE { get; set; }
        public string DISTRICTNAME { get; set; }
        public string BLOCKNAME { get; set; }
        public string SECTORNAME { get; set; }
        public string INDICATORNAME { get; set; }
        public string UNIT { get; set; }
        public decimal INDICATORVALUE { get; set; }
        public int YEAR { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public string CREATEDON { get; set; }
        public decimal TARGETVALUE { get; set; }
        public string INDICATORTYPE { get; set; }
        public string FREQUENCYTYPE { get; set; }
    }
}
