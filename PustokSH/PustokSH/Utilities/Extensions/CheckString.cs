using System.Text.RegularExpressions;

namespace PustokSH.Utilities.Extensions
{
    public static class CheckString
    {
        public static string CapitalizeName(string name)
        {
            return name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
        }
        public static bool IsDigit(string name)
        {
            return name.Any(char.IsDigit);
        }
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
    }
}
