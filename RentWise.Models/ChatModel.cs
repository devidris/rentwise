using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RentWise.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public bool IsRead { get; set; } = false;

        public bool IsOrder { get; set; } = false;

    }

    public class ChatSummary
    {
        public string UserId { get; set; }
        public IdentityUser User { get; set; } // Assuming ApplicationUser is your IdentityUser
        public ChatModel LastMessage { get; set; }
        public string LastChat { get; set; }
        public int UnreadMessageCount { get; set; }

        public string ProfilePicture { get; set; }  
    }

}
