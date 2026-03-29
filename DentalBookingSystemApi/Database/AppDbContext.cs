using DentalBookingSystemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalBookingSystemApi.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<OpeningHours> OpeningHours => Set<OpeningHours>();
    public DbSet<BlockedSlot> BlockedSlots => Set<BlockedSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        // ── Clinic ────────────────────────────────────────────
        model.Entity<Clinic>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(100);
            e.Property(c => c.Email).IsRequired().HasMaxLength(200);
            e.Property(c => c.PasswordHash).IsRequired();
        });

        // ── Treatment ─────────────────────────────────────────
        model.Entity<Treatment>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Name).IsRequired().HasMaxLength(100);
            e.Property(t => t.Price).HasColumnType("decimal(10,2)");
            e.HasOne(t => t.Clinic)
             .WithMany(c => c.Treatments)
             .HasForeignKey(t => t.ClinicId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── OpeningHours ──────────────────────────────────────
        model.Entity<OpeningHours>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasOne(o => o.Clinic)
             .WithMany(c => c.OpeningHours)
             .HasForeignKey(o => o.ClinicId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── BlockedSlot ───────────────────────────────────────
        model.Entity<BlockedSlot>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Reason).HasMaxLength(200);
            e.HasOne(b => b.Clinic)
             .WithMany(c => c.BlockedSlots)
             .HasForeignKey(b => b.ClinicId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Patient ───────────────────────────────────────────
        model.Entity<Patient>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            e.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            e.Property(p => p.Email).IsRequired().HasMaxLength(200);
            e.Property(p => p.PasswordHash).IsRequired();
            e.Property(p => p.Phone).HasMaxLength(20);
        });

        // ── Booking ───────────────────────────────────────────
        model.Entity<Booking>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Status)
             .HasConversion<string>();   // stores "Confirmed" not 0 in Postgres
            e.HasOne(b => b.Clinic)
             .WithMany(c => c.Bookings)
             .HasForeignKey(b => b.ClinicId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(b => b.Patient)
             .WithMany(p => p.Bookings)
             .HasForeignKey(b => b.PatientId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(b => b.Treatment)
             .WithMany(t => t.Bookings)
             .HasForeignKey(b => b.TreatmentId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}