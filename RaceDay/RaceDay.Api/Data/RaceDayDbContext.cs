using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
        : base(options)
    {
    }

    // ============================================================
    // DbSets — each becomes a table in SQL Server
    // ============================================================
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<EventRoute> EventRoutes => Set<EventRoute>();

    // ============================================================
    // Relationship configuration
    // ============================================================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Users ----------
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ---------- Events ----------
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Categories ----------
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Event)
            .WithMany(e => e.Categories)
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Enrolments ----------
        // Avoid multiple cascade paths by using NoAction on the FKs that
        // could otherwise create cycles.
        modelBuilder.Entity<Enrolment>()
            .HasOne(en => en.User)
            .WithMany(u => u.Enrolments)
            .HasForeignKey(en => en.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Enrolment>()
            .HasOne(en => en.Event)
            .WithMany(e => e.Enrolments)
            .HasForeignKey(en => en.EventId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Enrolment>()
            .HasOne(en => en.Category)
            .WithMany(c => c.Enrolments)
            .HasForeignKey(en => en.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // A participant can only enrol once per event
        modelBuilder.Entity<Enrolment>()
            .HasIndex(en => new { en.UserId, en.EventId })
            .IsUnique();

        // ---------- Results ----------
        // Result links ONLY to Enrolment (1:1) — feedback fix
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Enrolment)
            .WithOne(en => en.Result)
            .HasForeignKey<Result>(r => r.EnrolId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- EventRoutes ----------
        modelBuilder.Entity<EventRoute>()
            .HasOne(r => r.Event)
            .WithMany(e => e.Routes)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}