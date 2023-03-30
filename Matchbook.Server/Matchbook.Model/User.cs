using System.ComponentModel.DataAnnotations;

namespace Matchbook.Model
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        public string DsId { get; set; }
    }
}
