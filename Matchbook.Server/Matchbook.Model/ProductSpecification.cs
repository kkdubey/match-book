using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Matchbook.Model
{
    public class ProductSpecification
    {
        public ProductSpecification()
        {
            MaturityMonths = Array.Empty<int>();
        }

        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        public string ExchangeName { get; set; }
        [Required]
        [MaxLength(20)]
        public string CommodityCode { get; set; }
        [Required]
        [MaxLength(20)]
        public string JPMCode { get; set; }
        public int ContractSize { get; set; }
        [Required]
        [MaxLength(10)]
        public string ContractUoM { get; set; }
        [Required]
        [MaxLength(10)]
        public string PriceQuoteCurrency { get; set; }

        public int[] MaturityMonths { get; set; }

        private static readonly Dictionary<int, char> MaturityMonthToLetterMapping =
            new Dictionary<int, char>
            {
                { 1, 'F' },
                { 2, 'G' },
                { 3, 'H' },
                { 4, 'J' },
                { 5, 'K' },
                { 6, 'M' },
                { 7, 'N' },
                { 8, 'Q' },
                { 9, 'U' },
                {10, 'V' },
                {11, 'X' },
                {12, 'Z' }
            };

        // Example how to get 'YYYYMM' periods
        public IEnumerable<string> GetProductMaturityPeriods(int startYear, int endYear)
        {
            return
                from year in Enumerable.Range(startYear, endYear - startYear + 1)
                from month in MaturityMonths
                select $"{year}{month:D2}";
        }

        // Example how to get product symbols
        public IEnumerable<string> GetProductSymbols(int startYear, int endYear)
        {
            return
                from year in Enumerable.Range(startYear, endYear - startYear + 1)
                from month in MaturityMonths
                select $"{CommodityCode}{MaturityMonthToLetterMapping[month]}{year % 10}";
        }

        public ICollection<SubAccountProductSpecs> SubAccounts { get; set; }
    }
}
