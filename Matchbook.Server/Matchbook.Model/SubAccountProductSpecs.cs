namespace Matchbook.Model
{
    public class SubAccountProductSpecs
    {
        public int SubAccountId { get; set; }
        public SubAccount SubAccount { get; set; }
        public int ProductSpecId { get; set; }
        public ProductSpecification ProductSpec { get; set; }
    }
}
