using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.DashboardModel
{
   public class DashboardModel
    {
        public string SECTORNAME { get; set; }
        public string CSSCLASS { get; set; }
        public string MAXDATAPOINTVALUE { get; set; }
        public string MINDATAPOINTVALUE { get; set; }
        public string BLOCKNAME { get; set; }
        public string MXBLOCKNAME { get; set; }
        public string MNBLOCKNAME { get; set; }
        public string DATAPOINTNAME { get; set; }
        public int SECTORID { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public decimal DATAPOINTAVG { get; set; }
        public string DATAPOINTVALUE { get; set; }
        public int MAXVAL { get; set; }
        public int MINVAL { get; set; }
    }
}
