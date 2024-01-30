using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class OtpVerification : DefaultModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Value { get; set; }
        public string OTP { get; set; }
    }
}
