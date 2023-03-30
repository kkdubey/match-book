using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Symbol { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int SpecificationId { get; set; }
        public ProductSpecification Specification { get; set; }
    }
}
