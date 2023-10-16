using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Report
{
    public class SectorPRFData
    {
        public int FrequencyNo { get; set; }

        public string Month { get; set; }

        public int SectorId { get; set; }
        public string SectorName { get; set; }

        public int DpEntered { get; set; }

        public int HN { get; set; }
        public int EDU { get; set; }

        public int AGRI { get; set; }

        public int BASINFRA { get; set; }
         
        public int FINACIAL { get; set; }
        public int TOTAL { get; set; }
        public int BLOCKID { get; set; }
        public int TOTALBLOCKASP { get; set; }
        public int DistrictId { get; set; }
        public int Year { get; set; }
        public int TOTALDBLOCK { get; set; }
        public int DISTRICTID { get; set; }

    }
}
