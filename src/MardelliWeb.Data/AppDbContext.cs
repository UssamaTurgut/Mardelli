using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MardelliWeb.Core;

namespace MardelliWeb.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Region> Regions => Set<Region>();
    public DbSet<VocabularyEntry> VocabularyEntries => Set<VocabularyEntry>();
    public DbSet<MediaItem> MediaItems => Set<MediaItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<VocabularyEntry>(e =>
        {
            e.HasOne(x => x.Region).WithMany(r => r.VocabularyEntries).HasForeignKey(x => x.RegionId);
            e.HasIndex(x => new { x.SourceLanguage, x.SourceWord });
            e.HasIndex(x => x.MardelliWord);
            e.HasIndex(x => x.Status);
        });

        builder.Entity<MediaItem>(e =>
        {
            e.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionId).IsRequired(false);
            e.HasIndex(x => x.Status);
        });

        builder.Entity<ApplicationUser>(e =>
        {
            e.HasOne(x => x.HomeRegion).WithMany().HasForeignKey(x => x.HomeRegionId).IsRequired(false);
        });
    }
}
