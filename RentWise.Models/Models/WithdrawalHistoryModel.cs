using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models.Models
{
    public class WithdrawalHistoryModel
    {
        [Key]
        public int Id { get; set; }
        public string AgentId { get; set; }
        public double WithdrawalAmount { get; set; }
        public string AccountDetails { get; set; }
        public string LkpBankName { get; set; }
        public string FullName { get; set; }
    }
}
