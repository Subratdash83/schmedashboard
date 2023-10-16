using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Login.OTPTracker
{
    public class OTPTracker
    {
        public int OTPTRACKERID { get; set; }
        public string MOBILENO { get; set; }
        public int OTP { get; set; }
        public int OTPTYPE { get; set; }
        public string IPADDRESS { get; set; }
        public DateTime CREATEDON { get; set; }
        public int CREATEDBY { get; set; }
    }
}
