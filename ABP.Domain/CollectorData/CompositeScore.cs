using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.CollectorData
{
    public class CompositeScore
    {
        public int COMPSCID { get; set; }
        public int DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public int SECTORID { get; set; }
        public string SECTORNAME { get; set; }
        public decimal COMPSCORE { get; set; }
    }
}
