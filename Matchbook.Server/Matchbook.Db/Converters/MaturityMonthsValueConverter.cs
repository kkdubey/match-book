using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Matchbook.Db.Converters
{
    public static class MaturityMonthsValueConverter
    {

        public static readonly ValueConverter<int[], string> Instance =
            new ValueConverter<int[], string>(
                array => string.Join(";", array),
                str => str.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
    }
}
