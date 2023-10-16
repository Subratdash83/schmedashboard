using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Frequency
{
    public class FrequencyDP
    {
        public int FREQUENCYID { get; set; }
        public string FREQUENCY { get; set; }
        public string FREQUENCYNO { get; set; } 
        public string FREQUENCYVALUE { get; set; } 
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int DELETEDFLAG { get; set; }
    }
}
