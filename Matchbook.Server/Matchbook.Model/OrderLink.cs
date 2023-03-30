using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Matchbook.Model
{
    public class OrderLink
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<Order> LinkedOrders { get; set; }
    }
}
