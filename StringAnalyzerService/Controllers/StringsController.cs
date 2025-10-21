using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StringAnalyzerService.Data;
using StringAnalyzerService.Models;
using StringAnalyzerService.Services;
using System.Text.Json;

namespace StringAnalyzerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StringsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StringsController(AppDbContext context)
        {
            _context = context;
        }

        // POST /strings
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateString([FromBody] StringRequest request)
        {
            // Validate request
            if (string.IsNullOrEmpty(request?.Value))
                return BadRequest(new { error = "Missing value field" });

            // Compute properties
            var sha256 = StringAnalyzer.ComputeSha256(request.Value);

            // Check if exists
            if (await _context.Strings.AnyAsync(s => s.Id == sha256))
                return Conflict(new { error = "String already exists" });

            var charFreq = StringAnalyzer.GetCharacterFrequency(request.Value);

            var analysis = new StringAnalysis
            {
                Id = sha256,
                Value = request.Value,
                Length = request.Value.Length,
                IsPalindrome = StringAnalyzer.IsPalindrome(request.Value),
                UniqueCharacters = StringAnalyzer.CountUniqueCharacters(request.Value),
                WordCount = StringAnalyzer.CountWords(request.Value),
                Sha256Hash = sha256,
                CharacterFrequencyMap = JsonSerializer.Serialize(charFreq),
                CreatedAt = DateTime.UtcNow
            };

            _context.Strings.Add(analysis);
            await _context.SaveChangesAsync();

            return StatusCode(201, FormatResponse(analysis));
        }

        // GET /strings/{string_value}
        [HttpGet("{stringValue}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetString(string stringValue)
        {
            var analysis = await _context.Strings
                .FirstOrDefaultAsync(s => s.Value == stringValue);

            if (analysis == null)
                return NotFound(new { error = "String not found" });

            return Ok(FormatResponse(analysis));
        }

        // GET /strings (with filters)
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllStrings(
            [FromQuery] bool? is_palindrome,
            [FromQuery] int? min_length,
            [FromQuery] int? max_length,
            [FromQuery] int? word_count,
            [FromQuery] string? contains_character)
        {
            var query = _context.Strings.AsQueryable();

            if (is_palindrome.HasValue)
                query = query.Where(s => s.IsPalindrome == is_palindrome.Value);

            if (min_length.HasValue)
                query = query.Where(s => s.Length >= min_length.Value);

            if (max_length.HasValue)
                query = query.Where(s => s.Length <= max_length.Value);

            if (word_count.HasValue)
                query = query.Where(s => s.WordCount == word_count.Value);

            if (!string.IsNullOrEmpty(contains_character))
                query = query.Where(s => s.Value.Contains(contains_character));

            var results = await query.ToListAsync();

            return Ok(new
            {
                data = results.Select(FormatResponse),
                count = results.Count,
                filters_applied = new
                {
                    is_palindrome,
                    min_length,
                    max_length,
                    word_count,
                    contains_character
                }
            });
        }

        // DELETE /strings/{string_value}
        [HttpDelete("{stringValue}")]
        public async Task<IActionResult> DeleteString(string stringValue)
        {
            var analysis = await _context.Strings
                .FirstOrDefaultAsync(s => s.Value == stringValue);

            if (analysis == null)
                return NotFound(new { error = "String not found" });

            _context.Strings.Remove(analysis);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private object FormatResponse(StringAnalysis analysis)
        {
            var charFreq = JsonSerializer.Deserialize<Dictionary<char, int>>(
                analysis.CharacterFrequencyMap);

            return new
            {
                id = analysis.Id,
                value = analysis.Value,
                properties = new
                {
                    length = analysis.Length,
                    is_palindrome = analysis.IsPalindrome,
                    unique_characters = analysis.UniqueCharacters,
                    word_count = analysis.WordCount,
                    sha256_hash = analysis.Sha256Hash,
                    character_frequency_map = charFreq
                },
                created_at = analysis.CreatedAt.ToString("o")
            };
        }
    

        // GET /strings/filter-by-natural-language
        [HttpGet("filter-by-natural-language")]
        [Produces("application/json")]
        public async Task<IActionResult> FilterByNaturalLanguage([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest(new { error = "Query parameter required" });

            var filters = ParseNaturalLanguageQuery(query);

            var dbQuery = _context.Strings.AsQueryable();

            if (filters.ContainsKey("is_palindrome"))
                dbQuery = dbQuery.Where(s => s.IsPalindrome == (bool)filters["is_palindrome"]);

            if (filters.ContainsKey("word_count"))
                dbQuery = dbQuery.Where(s => s.WordCount == (int)filters["word_count"]);

            if (filters.ContainsKey("min_length"))
                dbQuery = dbQuery.Where(s => s.Length >= (int)filters["min_length"]);

            if (filters.ContainsKey("contains_character"))
                dbQuery = dbQuery.Where(s => s.Value.Contains((string)filters["contains_character"]));

            var results = await dbQuery.ToListAsync();

            return Ok(new
            {
                data = results.Select(FormatResponse),
                count = results.Count,
                interpreted_query = new
                {
                    original = query,
                    parsed_filters = filters
                }
            });
        }

        private Dictionary<string, object> ParseNaturalLanguageQuery(string query)
        {
            var filters = new Dictionary<string, object>();
            var lower = query.ToLower();

            if (lower.Contains("palindrom"))
                filters["is_palindrome"] = true;

            if (lower.Contains("single word"))
                filters["word_count"] = 1;

            if (lower.Contains("longer than"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(lower, @"longer than (\d+)");
                if (match.Success)
                    filters["min_length"] = int.Parse(match.Groups[1].Value) + 1;
            }

            if (lower.Contains("containing") || lower.Contains("contain"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(lower, @"letter ([a-z])");
                if (match.Success)
                    filters["contains_character"] = match.Groups[1].Value;
            }

            if (lower.Contains("first vowel"))
                filters["contains_character"] = "a";

            return filters;
        } 
    } 
}

    public class StringRequest
    {
        public string? Value { get; set; }
    }
