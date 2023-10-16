using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.BDOData
{
    public class BDOData
    {

        public int BDODATAID { get; set; }
        public int DATAPOINTID { get; set; }
        public decimal VALUE { get; set; }
        public string DATAPOINTNAME { get; set; }
        public string FROMDATE { get; set; }
        public string TODATE { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public int FREQUENCY { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int YEAR { get; set; }
        public int DELETEDFLAG { get; set; }
        public int SECTORID { get; set; }
       
    }
}
