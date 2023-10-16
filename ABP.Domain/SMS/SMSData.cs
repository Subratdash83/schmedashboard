using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.SMS
{
    public class SMSData
    {
        public int ID { get; set; }
        public int EVENTID { get; set; }
        public int TEMPLATEID { get; set; }
        public int DISTRICTID { get; set; }

        public string DISTRICTNAME { get; set; }
        public int BLOCKID { get; set; }

        public string MOBNO { get; set; }
        public string BLOCKNAME{get; set;}

        public string SENDSTATUS { get; set; }

        public string SMSSTATUS { get; set; }
        public string SMSTEMPLATE { get; set; }
        public string EVENTNAME { get; set; }
        public string TEMPLATECODE{get; set;}
        public string TEMPLATETITLE{get; set;}
        public string TEMPLATEMESSAGE { get; set;}
        public int TEMPLATETYPE{get; set;}
        public DateTime TEMPDATE{get; set;}
        public DateTime SMSDATE { get; set; }
        public string JOBTITLE{get; set;}
        public string USERTYPE{get; set;}
        public string USERNAME{get; set;}
        public string USEREMAIL{get; set;}
        public string USERMOBILE{get; set;}
        public string DISTNAME { get; set; }

        //Use for Indicator Job Schedular Table
        public int JOBTYPEID { get; set; }
        public int INDJOBTYPEID { get; set; }
        public DateTime JOBDATE { get; set; }
    }
}
