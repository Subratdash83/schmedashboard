using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.SMS
{
    public class SMS
    {
        public int intdesignationid { get; set; }
        public int intleveldetailid { get; set; }
        public string vchfullname { get; set; }
        public string vchmobtel { get; set; }        
        public string vchemail { get; set; }

        public string FROMDATE { get; set; }

        public string TODATE { get; set; }

        public int intuserid { get; set; }

        public string nvchlevelname { get; set; }

        public int Count { get; set; }

        public int SMSTRACKERID { get; set; }
        public int USERID { get; set; }
        public int SMSID { get; set; }

        public string SMSRESPONSE { get; set; }
        public int DistrictId { get; set; }
        public int BLOCKID
        {
            get; set;
        }

    }
}
