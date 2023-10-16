using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Models
{
    public class DataPointDTO
    {
        public int BDODATAID { get; set; }
        public int DATAPOINTID { get; set; }
        public decimal DATAPOINTVALUE { get; set; }
        public string MONTHNAME { get; set; }
        public int FREQUENCYNO { get; set; }
        public int INDICATORID { get; set; }
    }
}
