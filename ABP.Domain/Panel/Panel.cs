using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Panel
{
    public class Panel
    {
        public string DEPTNAME { get; set; }
        public int DEPTID { get; set; }
        public int PANELID { get; set; }
        public string PANELNAME { get; set; }
        public string DISPLAYNAME { get; set; }
        public string IMAGE { get; set; }
        public string EFFECTIVADATE { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int DELETEDFLAG { get; set; }
        public int FREQUENCY { get; set; }

        public int Indicatorid { get; set; }
        public string INDICATORNAME { get; set; }

        public string IndicatorTrend{ get; set; }

        public string CONTROLNAME { get; set; }

        public string GRAPHDATA { get; set; }

        public int PROVIDERID { get; set; }

        public string PROVIDERNAME { get; set; }



    }
}
