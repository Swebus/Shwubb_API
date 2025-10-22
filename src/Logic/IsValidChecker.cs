using System.Text.RegularExpressions;

namespace ShwubbApi.Logic
{
    public class IsValidChecker
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Require something@something.something (at least one dot in domain)
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}
