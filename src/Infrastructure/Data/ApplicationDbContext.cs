using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add DbSet for Applications
    public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var stringListConverter = new ValueConverter<List<string>, string>(
            list => JsonSerializer.Serialize(list, (JsonSerializerOptions?)null),
            json => string.IsNullOrWhiteSpace(json)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null) ?? new List<string>()
        );

        // Configure Application entity
        builder.Entity<Domain.Entities.Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RoleTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SourceName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SourceUrl).HasMaxLength(500);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.JobDescription).HasMaxLength(4000);
            entity.Property(e => e.RequiredSkills).HasConversion(stringListConverter);
            entity.Property(e => e.NiceToHaveSkills).HasConversion(stringListConverter);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Applications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
