using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Text;

namespace ABP.Domain.Report
{
    public  class MapData
    {
        
        public int INTDISTRICTID { get; set; }
        public string DistrictName { get; set; }
        public string VARMATRIX { get; set; }
        public string VARCLASS { get; set; }
        public int SVGID { get; set; }

        public int TotalBlocks { get; set; }
        public int BlocksEntered { get; set; }
        public int BlocksNotEntered { get; set; }
        public int Allblocks { get; set; }
        public int TBlocksEntered { get; set; }
        public int TBlocksNotEntered { get; set; }

        public string DISTRICTSVGMAP { get; set; }
        public int SCORE { get; set; }
        public int DISTPERFOMANCE { get; set; }

        public int ASPIRATIONAL_DISTRICT { get; set; }

        public int ASPIRATIONAL_BLOCKS { get; set; }
        public int NON_ASPIRATIONAL_BLOCKS { get; set; }
        public int ASPBLOCKSENTERED { get; set; }
        public int ASPBLOCKSNOTENTERED { get; set; }
        public int NASPBLOCKSENTERED { get; set; }
        public int NASPBLOCKSNOTENTERED { get; set; }
    }
}
