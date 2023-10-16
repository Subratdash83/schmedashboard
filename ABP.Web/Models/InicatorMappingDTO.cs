using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Models
{
    public class InicatorMappingDTO
    {
        public string IndicatorMappingId { get; set; }
        public string IndicatorId { get; set; }
        public string SectorId { get; set; }
        public string Formula { get; set; }
        public string CheckedValuesD { get; set; }
        public string CheckedValuesN { get; set; }
        public string CheckedValuesIncenus { get; set; }
        public int iscensus { get; set; }
    }
}
