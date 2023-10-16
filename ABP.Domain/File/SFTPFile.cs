using System;
using System.Collections.Generic;
using System.Text;

namespace CAPS.Domain.File
{
    public class SFTPFile
    {
        public string SFTPFileName { get; set; }
        public string SFTPFileType { get; set; }
        public bool DownloadStatus { get; set; }
        public bool SFTPFileStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
