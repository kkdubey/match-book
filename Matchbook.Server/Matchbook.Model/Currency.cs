using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public class Currency
    {
        [Required]
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
