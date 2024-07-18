using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class DisplayPreview
    {
        public string Image { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public double Rating { get; set; }
        public string Location { get; set; } = string.Empty;

        public string AgentImage { get; set; } = "/img/profile.png";

        public string AgentName { get; set; } = "Lois";

        public string ProductId { get; set; } = string.Empty;

        public bool Premium { get; set; } = false;

    }
}
