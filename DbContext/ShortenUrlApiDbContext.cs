using Microsoft.EntityFrameworkCore;
using ShortenUrlApi.Models;

namespace ShortenUrlApi.Data;

public class ShortenUrlApiDbContext : DbContext {
    public ShortenUrlApiDbContext(DbContextOptions<ShortenUrlApiDbContext> options) : base(options) {

    }
    public DbSet<ShortUrlModel> ShortUrlTable {get; set;}
}