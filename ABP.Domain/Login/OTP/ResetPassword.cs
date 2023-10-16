using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ABP.Domain.Login.OTP
{
    public class ResetPassword
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "Confirm password must match the new password.")]
        public string ConfirmPassword { get; set; }
        public string OTP { get; set; }
        public string Token { get; set; }
    }
}
