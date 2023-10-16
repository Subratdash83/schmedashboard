using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.BlockData
{
   public class BlockDataAlert
    {
        public int YEAR { get; set; }
        public int FREQUENCYNO { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public int FREQUENCYID { get; set; }
        public string FREQUENCY { get; set; }
        public int BLOCKID { get; set; }
        public int DPCOUNT { get; set; }
        public int NONZERODPCOUNT { get; set; }
        public int HNDPCOUNT { get; set; }
        public string Remarks { get; set; }
       
    }
}
