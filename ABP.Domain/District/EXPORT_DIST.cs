using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Districts
{
   public class EXPORT_DIST
    {
        public int SL_NO { get; set; }
        public string LGD_DIST_CODE { get; set; }
        public string LGD_DIST_NAME { get; set; }
        public string SOURCE_DIST_CODE { get; set; }
        public string SOURCE_DIST_NAME { get; set; }
        public string CENSUS_2001_CODE { get; set; }
        public string CENSUS_2011_CODE { get; set; }
    }
}
