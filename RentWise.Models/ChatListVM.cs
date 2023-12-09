using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class ChatListVM
    {
        public string Image { get; set; }
        public IdentityUser User { get; set; }
    }
}
