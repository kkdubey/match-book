using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public class PhysicalContract
    {
        [MaxLength(50)]
        public string Number { get; set; }
        public decimal Quantity { get; set; }
        [MaxLength(10)]
        public string UnitOfMeasure { get; set; }
    }
}
