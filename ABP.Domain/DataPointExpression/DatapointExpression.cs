using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.DataPointExpression
{
   public class DatapointExpression
    {
        public int SECTORID { get; set; }
       
        public string SECTORNAME { get; set; }
        public int ExpressionID { get; set; }

        public string ExpressionNAME { get; set; }
        public string ExpressionNAMEWithID { get; set; }
        public string Script { get; set; }
        public int CREATEDBY { get; set; }
    }
    public class Operatorlist
    {
       

        public string Operator { get; set; }
      

        public string Discription { get; set; }
        
    }

}
