using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Report
{
    public class Report
    {
        public string MAPPEDBLOCK { get; set; }
        public string action { get; set; }
        public int SCORE { get; set; }
        public int code { get; set; }

        public int MAPID { get; set; }
        public int DistrictId { get; set; }
        public int BlockId { get; set; }
        public int Category { get; set; } 
        public int FREQUENCYNO { get; set; }
        public string PanelName { get; set; }
        public string Districtname { get; set; }
        public string APPLICATIONNO { get; set; }

        //public int Jan { get; set; }
        //public int Feb { get; set; }
        //public int Mar { get; set; }
        //public int Apr { get; set; }
        //public int May { get; set; }
        //public int Jun { get; set; }
        //public int Jul { get; set; }
        //public int Aug { get; set; }
        //public int Sep { get; set; }
        //public int Oct { get; set; }
        //public int Nov { get; set; }
        //public int Dec { get; set; }
        //public int BLOCKID { get; set; }
        public string DATSUBMITTED { get; set; } 
       // public string DataSubmitted }{ get; set; }
        public string Month { get; set; }
        public string status { get; set; }
        public string status1 { get; set; }
        public string dataformonthof { get; set; }
        public int Year { get; set; }
        public int DPCOUNT { get; set; }
        public int NONZERODPCOUNT { get; set; }
        public string BLOCKNAME { get; set; }
        public int Id { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int TotalBlocks { get; set; }
        public int BlocksEntered { get; set; }
        public int DataPending { get; set; }
        public int DataApproved { get; set; }
        public int DataRejected { get; set; }
        public int filterid { get; set; }

        public int ZERODPCOUNT { get; set; }

        public string SectorName { get; set; }
        public int Freqid { get; set; }
        public int ControlID { get; set; }
        public int BlocksNotEntered { get; set; }
        public string ControlName { get; set; }
        public string DisplayName { get; set; }
        public int SectorId { get; set; }
        public int ChangeCount { get; set; }
        public int FreqNo { get; set; }
        public int DISTPERFOMANCE { get; set; }

        public string DISTRICT_NAME { get; set; }

        public string BLOCK_NAME { get; set; }

        public string INDICATORNAME { get; set; }
        public string ALLMONTHS { get; set; }

        public int IndicatorId { get; set; }

        public string INDICATORSCORE { get; set; }
        public string INDICATORSCORECAL { get; set; }
        //****Monthly Dashboard test***//
        public string filterno { get; set; }
        public int filternoto { get; set; }
        public int condition { get; set; }
        public int MONTHNO { get; set; }
        public int Aspirationblock { get; set; }

        public int ASPIRATIONAL_BLOCKS { get; set; }
        public int BlockType { get; set; }

        public int FREEZESTATUS { get; set; }
    }
}
