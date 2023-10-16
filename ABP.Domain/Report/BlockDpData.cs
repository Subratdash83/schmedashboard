using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Report
{
     public class BlockDpData
    {

        public int SLNO { get; set; }
        public string APPLICATIONNO { get; set; }
        public int DISTRICTID { get; set; }
        public string DISTNAME { get; set; }
        public int BLOCKID { get; set; }
        public string BLOCKNAME { get; set; }
        public int YEAR { get; set; }
        public string DATAPOINT { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public string VALUE { get; set; }
        public int SECTORID { get; set; }
        public string SECTORNAME { get; set; }
        public string UNIT { get; set; }
        public int PANELID { get; set; }
        public int FREQUENCYNO { get; set; }
        public string PANELNAME { get; set; }
        public string DISPLAYNAME { get; set; }
    }
}
