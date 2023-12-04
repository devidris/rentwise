using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class ChatModel:DefaultModel
    {
        [Key]
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
    }
}
