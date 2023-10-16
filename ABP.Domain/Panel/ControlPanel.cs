using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Panel 
{
    public class ControlPanel
    {
        public int DPCOUNT { get; set; }
        public int ZERODPCOUNT { get; set; }
        public int TYPE { get; set; }
        public int YEARTYPE { get; set; }
        public int YEAR { get; set; }
        public int DPMONTH { get; set; }
        public int MONTHNO { get; set; }
        public int DEPTID { get; set; }
        public string DepartmentName { get; set; }
        public int CONTROLID { get; set; }
        public int PANELID { get; set; }
        public string PANELNAME { get; set; } 
        public string CONTROLNAME { get; set; }
        public string DISPLAYNAME { get; set; }
        public string DISPLAYNAME1 { get; set; }
        public int FREQUENCYID { get; set; }
        public string FREQUENCY { get; set; }

        public string MONTHNAME { get; set; }
        public string EFFECTIVEDATE { get; set; }
        public int FREQUENCYNO { get; set; } 
        public int CREATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime UPDATEDBYON { get; set; }
        public int DELETEDFLAG { get; set; }

        //======ExtraProperties To Bind All Controlls with proper Frequency=======
        public int INDICATORID { get; set; }
        public decimal? CONTROLVALUE { get; set; } 
        public string FROMDATE { get; set; }
        public string TODATE { get; set; }
        public string FREQUENCYVALUE { get; set; } 
        public bool DataEntryEligibility { get; set; }
        public string APPLICATIONO { get; set; }

        public string UNIT { get; set; }

        public string DESCRIPTION { get; set; }
        public int SERIALNO { get; set; }
        public int STATUS { get; set; }
        public string REMARK { get; set; }
        public int BLOCKID { get; set; }
        public int DISTID { get; set; }

        public int PROVIDERID { get; set; }
        public string PROVIDERNAME { get; set; }

    }
}
