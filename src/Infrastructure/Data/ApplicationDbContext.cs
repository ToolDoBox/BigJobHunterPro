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

    // DbSets
    public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();
    public DbSet<TimelineEvent> TimelineEvents => Set<TimelineEvent>();
    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();
    public DbSet<HuntingParty> HuntingParties => Set<HuntingParty>();
    public DbSet<HuntingPartyMembership> HuntingPartyMemberships => Set<HuntingPartyMembership>();

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
            entity.HasMany(e => e.TimelineEvents)
                  .WithOne(te => te.Application)
                  .HasForeignKey(te => te.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TimelineEvent entity
        builder.Entity<TimelineEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasIndex(e => e.ApplicationId);
            entity.HasIndex(e => e.Timestamp);
        });

        // Configure ActivityEvent entity
        builder.Entity<ActivityEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.RoleTitle).HasMaxLength(200);
            entity.Property(e => e.MilestoneLabel).HasMaxLength(200);
            entity.HasIndex(e => e.PartyId);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasOne(e => e.Party)
                  .WithMany()
                  .HasForeignKey(e => e.PartyId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure HuntingParty entity
        builder.Entity<HuntingParty>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.InviteCode).IsRequired().HasMaxLength(8);
            entity.HasIndex(e => e.InviteCode).IsUnique();
            entity.HasOne(e => e.Creator)
                  .WithMany()
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure HuntingPartyMembership entity
        builder.Entity<HuntingPartyMembership>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.HuntingPartyId, e.UserId }).IsUnique();
            entity.HasOne(e => e.HuntingParty)
                  .WithMany(p => p.Memberships)
                  .HasForeignKey(e => e.HuntingPartyId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.PartyMemberships)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
