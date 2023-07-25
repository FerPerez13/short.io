using Microsoft.EntityFrameworkCore;

namespace Short.IO.API;
public class ShortIoContext : DbContext
{
    public DbSet<UrlRedirect> UrlRedirects { get; set; } = null!;
    public ShortIoContext(DbContextOptions<ShortIoContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlRedirect>().ToTable("UrlRedirects");
    }
}
