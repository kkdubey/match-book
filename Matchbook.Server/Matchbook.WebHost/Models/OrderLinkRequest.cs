using System.Collections.Generic;

namespace Matchbook.WebHost.Models
{
    public class OrderLinkRequest
    {

        public string LinkName { get; set; }
        public List<long> OrderIds { get; set; }

    }
}
