using System;
using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public class EfrpOrderDetails
    {
        public DateTime AgreementDate { get; set; }
        [MaxLength(100)]
        public string OppositeFirmName { get; set; }
        [MaxLength(100)]
        public string AccountHolder { get; set; }
        [MaxLength(100)]
        public string CustomerName { get; set; }
        [MaxLength(100)]
        public string CustomerClearingAccount { get; set; }
    }
}
