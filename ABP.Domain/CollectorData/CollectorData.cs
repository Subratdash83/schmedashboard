using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.CollectorData
{
    public class CollectorData
    {
        public int COLLECTORDATAID { get; set; }
        public int DATAPOINTID { get; set; }
        public decimal VALUE { get; set; }
        public string DATAPOINTNAME { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int DELETEDFLAG { get; set; }
    }
}
