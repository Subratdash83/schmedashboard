using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ABP.Domain.Login.OTP
{
    public class ForgetPassword
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
        public string MobileNumber { get; set; }
        public string OTP { get; set; }=string.Empty;
        public string strCaptcha { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string IsVerified { get; set; }
        public string Message { get; set; }
        public int msgstatus { get; set; }
        public string Token { get; set; }


    }
}
