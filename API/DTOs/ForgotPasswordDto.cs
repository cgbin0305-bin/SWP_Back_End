using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string NewPassword { get; set; }
    }
}