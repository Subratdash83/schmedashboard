using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Communication
{
   public class InboxReport
    {
        public int EMAILID { get; set; }
        /// <summary>
        /// MAIL DATE
        /// </summary>
        public string MAILDATE { get; set; }
        
        /// <summary>
        /// COMPONENT
        /// </summary>
        public string COMPONENT { get; set; }

        public string deptname { get; set; }
        public string tdeptname { get; set; }
        /// <summary>
        /// MESSAGE
        /// </summary>
        public string MESSAGE { get; set; }
        public string REPLY_MESSAGE { get; set; }
        public string MESSAGEDATE { get; set; }
        public string ACTIONDATE { get; set; }
        /// <summary>
        /// READ STATUS
        /// </summary>
        public string READSTATUS { get; set; }
        public string FROMDATE { get; set; }
        public string TODATE { get; set; }
        public int DEPTID { get; set; }
    }
  
}
