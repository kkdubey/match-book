using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public enum ClearingAccountType
    {
        Clearing,
        Execution
    }

    public class ClearingAccount
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        public ClearingAccountType Type { get; set; }
        [Required]
        [MaxLength(50)]
        public string BrokerCode { get; set; }
        [Required]
        [MaxLength(50)]
        public string BrokerTerm { get; set; }
    }
}
