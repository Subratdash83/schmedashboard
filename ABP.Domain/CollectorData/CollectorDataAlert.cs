using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.CollectorData
{
   public  class CollectorDataAlert
    {
        public int YEAR { get; set; }
        public int FREQUENCYID { get; set; }
        public int FREQUENCYNO { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public string FREQUENCY { get; set; }
        public int BLOCKID { get; set; }
        public string BLOCKNAME { get; set; }
        public int DISTRICTID { get; set; }
        public string DISTNAME { get; set; }
        public int DPCOUNT { get; set; }
        public int DPENTERED { get; set; }
        public int NOTENTERED { get; set; }
        public string REMARKS { get; set; }
    }
}
