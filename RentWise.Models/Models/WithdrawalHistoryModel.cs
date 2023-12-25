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
        public int AgentId { get; set; }
        public int WithdrawalAmount { get; set; }
        public DateTime WithdrawalDate { get; set; }

        public string AccountDetails { get; set; }
        public string BankName { get; set; }
    }
}
