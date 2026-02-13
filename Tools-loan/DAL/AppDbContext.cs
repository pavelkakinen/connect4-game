using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Tool> Tools => Set<Tool>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Certification> Certifications => Set<Certification>();
    public DbSet<MemberCertification> MemberCertifications => Set<MemberCertification>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<DamageReport> DamageReports => Set<DamageReport>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // Certification
        modelBuilder.Entity<Certification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // Tool
        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.ReplacementCost).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MaintenanceIntervalHours).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TotalUsageHours).HasColumnType("decimal(10,2)");
            entity.Property(e => e.HoursAtLastService).HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Tools)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RequiredCertification)
                .WithMany(c => c.Tools)
                .HasForeignKey(e => e.RequiredCertificationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Member
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // MemberCertification
        modelBuilder.Entity<MemberCertification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Member)
                .WithMany(m => m.MemberCertifications)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Certification)
                .WithMany(c => c.MemberCertifications)
                .HasForeignKey(e => e.CertificationId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.MemberId, e.CertificationId }).IsUnique();
        });

        // Loan
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UsageHours).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Tool)
                .WithMany(t => t.Loans)
                .HasForeignKey(e => e.ToolId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Member)
                .WithMany(m => m.Loans)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MaintenanceRecord
        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.HoursAtService).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Cost).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Tool)
                .WithMany(t => t.MaintenanceRecords)
                .HasForeignKey(e => e.ToolId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Reservation
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Tool)
                .WithMany(t => t.Reservations)
                .HasForeignKey(e => e.ToolId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Member)
                .WithMany(m => m.Reservations)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DamageReport
        modelBuilder.Entity<DamageReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.RepairCost).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Loan)
                .WithMany(l => l.DamageReports)
                .HasForeignKey(e => e.LoanId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
