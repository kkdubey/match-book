using System;
using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public class Order
    {
        public int Id { get; set; }
        public Side Side { get; set; }
        [Required]
        [MaxLength(20)]
        public string ProductSymbol { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public PriceInstruction? PriceInstruction { get; set; }
        [Required]
        public int SubAccountId { get; set; }
        public SubAccount SubAccount { get; set; }
        [Required]
        public int CounterpartySubAccountId { get; set; }
        public SubAccount CounterpartySubAccount { get; set; }
        public PhysicalContract PhysicalContract { get; set; }
        [MaxLength(100)]
        public string SourceOrderId { get; set; }
        [MaxLength(100)]
        public string SourceSystem { get; set; }
        [MaxLength(100)]
        public string ExternalReferenceId { get; set; }
        [MaxLength(5000)]
        public string AdditionalNotes { get; set; }
        [MaxLength(100)]
        public string CargillContact { get; set; }

        public OrderType OrderType { get; set; }
        public OrderStatus OrderStatus { get; set; }
        
        public EfrpOrderDetails EfrpOrderDetails { get; set; }

        public int? LinkId { get; set; }
        public OrderLink Link { get; set; }

        public DateTime Created { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; }
        public DateTime LastUpdated { get; set; }
        [MaxLength(50)]
        public string LastUpdatedBy { get; set; }
    }
}
