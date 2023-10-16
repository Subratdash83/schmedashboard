using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ABP.Domain.Indicator
{
    public class IndicatorValueScore
    {
        public int SCORECOUNT { get; set; }
        public string APPLICATIONNO { get; set; }
        public int BLOCKID { get; set; }
        public int DISTID { get; set; }
        public int INDICATORID { get; set; }
        public decimal INDICATORSCORE { get; set; }
        public int YEAR { get; set; }
        public int FREQUENCYNO { get; set; }
        public int FREQUENCYID { get; set; }
        public int PANELID { get; set; }
        public string BlockName { get; set; }

        public string YEARLY_APPLICATION_NO { get; set; }

    }
    public class IndicatorScoreXML
    {
        public XDocument indicatorvaluexml { get; set; }
    }

}
