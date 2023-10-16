using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.QueryBuilder
{
    public class QueryBuilderModel
    {
        public string DMLCode { get; set; }
        public string QueryText { get; set; }
        public string ReturnMsg { get; set; }
    }
}
