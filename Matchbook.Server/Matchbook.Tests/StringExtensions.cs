namespace Matchbook.Tests
{
    public static class StringExtensions
    {
        public static string JoinString(this string[] array, string delimiter)
        {
            return string.Join(delimiter, array);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
