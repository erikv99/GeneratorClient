using Microsoft.EntityFrameworkCore;
using GeneratorClient.Models;

public class MainDbContext : DbContext
{
    public DbSet<GenerationSettings> GenerationSettings { get; set; }
    public DbSet<GenerationRequest> GenerationRequests { get; set; }
    public DbSet<Image> Images { get; set; } 
    public DbSet<GenerationResponse> GenerationResponses { get; set; }

    public string DbPath { get; }

    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "MainDbContext.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GenerationSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Prompt).HasMaxLength(500);
            entity.Property(e => e.NegativePrompt).HasMaxLength(500);
            entity.Property(e => e.GuidanceScale).IsRequired();
            entity.Property(e => e.StyleStrength).IsRequired();
            entity.Property(e => e.InferenceSteps).IsRequired();
            entity.Ignore(e => e.EndpointUrl);
        });

        modelBuilder.Entity<GenerationRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedOn).IsRequired();
            entity.HasOne(d => d.Settings)
                .WithMany(p => p.Requests)
                .HasForeignKey(d => d.SettingsId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Bytes).IsRequired();
        });

        modelBuilder.Entity<GenerationResponse>(entity =>
        {
            entity.HasKey(gr => gr.Id);
            entity.Property(gr => gr.CreatedOn).IsRequired();
            entity.HasOne(gr => gr.Image)
                  .WithMany()
                  .HasForeignKey(gr => gr.ImageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
