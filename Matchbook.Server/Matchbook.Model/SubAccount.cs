using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public class SubAccount
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string AccountOwner { get; set; }
        public ICollection<SubAccountProductSpecs> TradedProducts { get; set; }
        [Required]
        public int ClearingAccountId { get; set; }
        public ClearingAccount ClearingAccount { get; set; }
        [Required]
        public int InternalClearingAccountId { get; set; }
        public ClearingAccount InternalClearingAccount { get; set; }
    }
}
