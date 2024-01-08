using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class WithdrawalHistoryModel:DefaultModel
    {
        [Key]
        public int Id { get; set; }
        public string AgentId { get; set; }
        public double WithdrawalAmount { get; set; }
        public string AccountDetails { get; set; }
        public int LkpBankName { get; set; }
        public string FullName { get; set; }

        public bool Pending { get; set; } = true;
    }
}
