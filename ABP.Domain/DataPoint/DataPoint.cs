using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.DataPoint
{
    public class DataPoint
    {
       
        public int DEPTID { get; set; }
        public decimal INDICATORVALUE { get; set; }
        public int TYPE { get; set; }
        public int YEARTYPE { get; set; }
        public int leveldetailedid { get; set; }
        public string BLOCKNAME { get; set; }
        public string APPLICATIONNO { get; set; }
        public int CONTROLID { get; set; }
        public int SERIALNO { get; set; }

        public int MONTHNO { get; set; }
        public string PANELNAME { get; set; }
        public string ControlNO { get; set; }
        public int iscensus { get; set; }
        public int DATAPOINTID { get; set; }
        public int BDODATAID { get; set; }
        public int SECTORID { get; set; }
        public int INDICATORID { get; set; }
        public int FREQUENCYID { get; set; }
        public string SECTORNAME { get; set; }
        public string DATAPOINTNAME { get; set; }
        public string COLLECTORREMARK { get; set; }   
        public string DATAPOINTDATE { get; set; }
        public string FROMDATE { get; set; }
        public string TODATE { get; set; }
        public int YEAR { get; set; }
        public int BLOCKID { get; set; }
        public string FREQUENCY { get; set; }
        public string FREQUENCYVALUE { get; set; }
        public string YRFREQUENCYVALUE { get; set; }
        public int FREQUENCYNO { get; set; }
        public decimal DATAPOINTVALUE { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int DELETEDFLAG { get; set; }
        public int STATUS { get; set; }
        public int EDITSTATUS { get; set; }
        public string MONTHNAME { get; set; }
        public int EDITCOUNT { get; set; }
        public int INDICATORMAPPINGID { get; set; }
        public int DATAPOINTCOUNT { get; set; }

        public string DistrictName { get; set; }
        public string BLOCK { get; set; }
        public string BlockName { get; set; }
        public string DepartmentName { get; set; }


        public int DATAID { get; set; }
        public bool DataEntryEligibility { get; set; }
        public string DType { get; set; }
        public string DESCRIPTION { get; set; }
        public string UNIT { get; set; }

        public int DPCOUNT { get; set; }
        public string DPTREND { get; set; }

        public int PROVIDERID { get; set; }
        public string PROVIDERNAME { get; set; }

    }
}
