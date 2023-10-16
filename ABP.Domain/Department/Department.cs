using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Department
{
    public class Department
    {
        public int ID { get; set; }
        public int DEPTID { get; set; }
        public string DEPTNAME { get; set; }
        public int SECTORID { get; set; }
        public int INDICATORID { get; set; }
        public string SECTORNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime CREATEDON { get; set; }
        public DateTime UPDATEDON { get; set; }
        public int DELETEDFLAG { get; set; }
        public int Frequency { get; set; }
        public int Year { get; set; }
    }
}
