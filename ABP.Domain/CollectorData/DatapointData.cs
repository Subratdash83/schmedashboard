using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.CollectorData
{
    public class DatapointData
    {
        public string APPLICATIONNO { get; set; }
        public int  DISTRICTID { get; set; }
        public int DISTNAME { get; set; }
        public int BLOCKID { get; set; }
        public string BLOCKNAME { get; set; }
        public int YEAR { get; set; }
        public string DATAPOINT { get; set; }
        public string VALUE { get; set; }
        public string UNIT { get; set; }
    }
}
