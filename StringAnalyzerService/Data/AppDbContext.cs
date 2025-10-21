using Microsoft.EntityFrameworkCore;
using StringAnalyzerService.Models;

namespace StringAnalyzerService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<StringAnalysis> Strings { get; set; }
    }
}