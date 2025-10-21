using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StringAnalyzerService.Services
{
    public class StringAnalyzer
    {
        public static string ComputeSha256(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        public static bool IsPalindrome(string input)
        {
            var cleaned = input.ToLower().Replace(" ", "");
            return cleaned.SequenceEqual(cleaned.Reverse());
        }

        public static int CountUniqueCharacters(string input)
        {
            return input.Distinct().Count();
        }

        public static int CountWords(string input)
        {
            return input.Split(new[] { ' ', '\t', '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static Dictionary<char, int> GetCharacterFrequency(string input)
        {
            return input.GroupBy(c => c)
                       .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}