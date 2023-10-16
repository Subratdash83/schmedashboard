using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Models
{
    public class BlocksDTO
    {
        public string DistCode { get; set; }
        public List<string> CheckedValues { get; set; }
        public List<string> UnCheckedValues { get; set; }

    }
}
