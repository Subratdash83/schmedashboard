using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.BlockData
{
    public class BlockData
    {
        public string DISTRICTNAME { get; set; }
        public string BLOCKNAME { get; set; }
        public int ID { get; set; }
        public int PANELID { get; set; }
        public string PANELNAME { get; set; } 
        public string APPLICATIONNO { get; set; }
        public int FREQUENCYID { get; set; }
        public int FREQUENCYNO { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public string YRFREQUENCYVALUE { get; set; }
        public string FREQUENCY { get; set; }
        public int YEAR { get; set; }
        public string FROMDATE { get; set; }
        public string TODATE { get; set; }
        public int DISTRICTID { get; set; }
        public int BLOCKID { get; set; }
        public int STATUS { get; set; } 
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }

        public string FREEZEDON { get; set; }
        public DateTime UPDATEDON { get; set; } 
        public string REMARK { get; set; }
        public string ACTIONTAKENON { get; set; }
        public int status { get; set; }

        public int FREEZESTATUS { get; set; }
        public int DPCOUNT { get; set; }
        public int MONTH { get; set; }
        public int Code { get; set; }

        //=================
        public string TABLENAME { get; set; } 
        public string CONTROLNAME { get; set; }
        public decimal CONTROLVALUE { get; set; }
        public int NONZERODPCOUNT { get; set; }
        public int HNDPCOUNT { get; set; }
        public int AGDPCOUNT { get; set; }
        public int BIDPCOUNT { get; set; }
        public int FIDPCOUNT { get; set; }
        public int EDDPCOUNT { get; set; }

        public int MONTHNO { get; set; }
        public string YEARLY_APPLICATION_NO { get; set; }
        public string PROVIDERNAME { get; set; }
        public int DPMONTH { get; set; }
    }
}
