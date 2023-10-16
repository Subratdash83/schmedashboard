using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Dashboard
{
    public class DashboardNew
    {
        public string SECTORNAME { get; set; }
        public int INDICATORID { get; set; }
        public string INDICATORNAME { get; set; }
        public string BG_SECTOR_CLASS { get; set; }
        public string TEXT_SECTOR_CLASS { get; set; }
        public int INDICATORCOUNT { get; set; }
        public int SECTORID { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public int TOTALDPCOUNT { get; set; }
        public int TOTALMDP { get; set; }
        public int TOTALYDP { get; set; }
        public int FREQUENCYID { get; set; }
        public int DATAPOINTCOUNT { get; set; }
        public int MONTHLY_COUNT { get; set; }
        public int YEARLY_COUNT { get; set; }
        public int QUATERLY_COUNT { get; set; }
        public int MONTHLY_ENTERED { get; set; }
        public int YEARLY_ENTERED { get; set; }
        public int MONTHLY_NOTENTERED { get; set; }
        public int YEARLY_NOTENTERED { get; set; }
        public string SUM_ENTERED { get; set; }
        public int BLOCKID { get; set; }
        public string BLOCKNAME { get; set; }
        public string FREQUENCYVALUE { get; set; }
    }
}
