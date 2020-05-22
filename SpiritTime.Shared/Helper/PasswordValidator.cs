using System.Linq;
using System.Text.RegularExpressions;

namespace SpiritTime.Shared.Helper
{
    public static class PasswordValidator
    {
        private static readonly Regex LowerChars   = new Regex("[a-z]+");
        private static readonly Regex UpperChars   = new Regex("[A-Z]+");
        private static readonly Regex Digits      = new Regex("[0-9]+");
        private static readonly Regex SpecialChars = new Regex("[^a-zA-Z\\d\\s:]+");

        public static (bool, string) ByIdentityStandard(string password)
        {
            return ByOptions(password, 6, true, true, true, true);
        }
        
        public static (bool, string) ByComplexitiyLevel(string password, int length, int complexity)
        {
            if (length > password.Length)
                return (false, "The password is too short. The required length would be: " + length);

            var givenComplexity = 0;
            if (LowerChars.IsMatch(password))
                givenComplexity++;
            if (UpperChars.IsMatch(password))
                givenComplexity++;
            if (Digits.IsMatch(password))
                givenComplexity++;
            if (SpecialChars.IsMatch(password))
                givenComplexity++;

            return givenComplexity >= complexity 
                ? (true, "") 
                : (false, "The password complexity of " + complexity + " is not reached.");

        }

        public static (bool, string) ByOptions(string password, int length,  bool requireLowercase, bool requireUppercase , bool requireDigit, bool requireNonAlphanumeric)
        {
            if (length > password.Length)
                return (false, "The password is too short. The required length would be: " + length);
            
            if(requireLowercase && !LowerChars.IsMatch(password))
                return (false, "The password should contain lower chars but does not.");
            if(requireUppercase && !UpperChars.IsMatch(password))
                return (false, "The password should contain upper chars but does not.");
            if(requireDigit && !Digits.IsMatch(password))
                return (false, "The password should contain digits but does not.");
            if(requireNonAlphanumeric && !SpecialChars.IsMatch(password))
                return (false, "The password should contain non alphanumeric chars but does not.");

            return (true, "");
        }

    }
}