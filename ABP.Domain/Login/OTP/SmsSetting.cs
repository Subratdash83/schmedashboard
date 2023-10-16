using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Login.OTP
{
    public class SmsSetting
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string PhoneNumber { get; set; }
    }
}
