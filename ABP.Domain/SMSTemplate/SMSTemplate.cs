using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.SMSTemplate
{
   public class SMSTemplate
    {
        
        public int SMSFOR { get; set; }

        public string SMSCONTENT { get; set; }
        
        public string DAY { get; set; }
        public string TIME { get; set; }

        public string TIMEPERIOD { get; set; }

        public int SMSID { get; set; }

        public string TEMPLATENAME { get; set; }
        public string TEMPLATEDLTID { get; set; }

        public string HEADER { get; set; }

        public string COMMUNICATIONTYPE { get; set; }

    }
}
