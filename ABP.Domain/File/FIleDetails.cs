using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Domain.File
{
    public class FIleDetails
    {
        public int TrackerId { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        //=== XMlFIleName
        public string FileName { get; set; }
        //=== XMlFIleName
        public bool IsExistsInSFTP { get; set; }
        public bool IsDownloaded { get; set; }
        public string SFTPFileName { get; set; }
        public string CreationTime { get; set; }
        public string FilePath { get; set; }
        public string TotalBenf { get; set; }
        public string ProcessedFileCount { get; set; }
        public string ACKFilePath { get; set; }
        public string NCKFilePath { get; set; }
        public string OBJFilePath { get; set; }
        public string RETFilePath { get; set; }
        public string STSFilePath { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}