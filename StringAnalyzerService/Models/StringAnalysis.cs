using System.ComponentModel.DataAnnotations;

namespace StringAnalyzerService.Models
{
    public class StringAnalysis
    {
        [Key]
        public string Id { get; set; } // SHA256 hash

        public string Value { get; set; }

        public int Length { get; set; }

        public bool IsPalindrome { get; set; }

        public int UniqueCharacters { get; set; }

        public int WordCount { get; set; }

        public string Sha256Hash { get; set; }

        public string CharacterFrequencyMap { get; set; } // JSON string

        public DateTime CreatedAt { get; set; }
    }
}