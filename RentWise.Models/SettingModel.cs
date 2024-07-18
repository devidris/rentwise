using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class SettingModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LookupId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
