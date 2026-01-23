using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DanceStyle> DanceStyles => Set<DanceStyle>();
    public DbSet<Studio> Studios => Set<Studio>();
    public DbSet<DanceClass> DanceClasses => Set<DanceClass>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<TrialUsage> TrialUsages => Set<TrialUsage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student - unique email
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();

        // TrialUsage - one trial per style per student
        modelBuilder.Entity<TrialUsage>()
            .HasIndex(t => new { t.StudentId, t.DanceStyleId })
            .IsUnique();

        // Booking - InvitedByStudent relationship
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.InvitedByStudent)
            .WithMany(s => s.InvitedBookings)
            .HasForeignKey(b => b.InvitedByStudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking - Student relationship
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Student)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Package - DanceStyle (optional, for single-style packages)
        modelBuilder.Entity<Package>()
            .HasOne(p => p.DanceStyle)
            .WithMany(d => d.Packages)
            .HasForeignKey(p => p.DanceStyleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed DanceStyles (12 styles)
        modelBuilder.Entity<DanceStyle>().HasData(
            new DanceStyle { Id = 1, Name = "Ballet" },
            new DanceStyle { Id = 2, Name = "Contemporary" },
            new DanceStyle { Id = 3, Name = "Jazz" },
            new DanceStyle { Id = 4, Name = "Hip-hop" },
            new DanceStyle { Id = 5, Name = "Salsa" },
            new DanceStyle { Id = 6, Name = "Bachata" },
            new DanceStyle { Id = 7, Name = "Tango" },
            new DanceStyle { Id = 8, Name = "Ballroom" },
            new DanceStyle { Id = 9, Name = "Pole Fitness" },
            new DanceStyle { Id = 10, Name = "Aerial Silks" },
            new DanceStyle { Id = 11, Name = "Breakdancing" },
            new DanceStyle { Id = 12, Name = "K-pop" }
        );

        // Seed Studios (4 studios)
        modelBuilder.Entity<Studio>().HasData(
            new Studio
            {
                Id = 1,
                Name = "Studio A",
                SizeSquareMeters = 120,
                HasPoles = false,
                HasAerialRigging = false,
                MaxCapacity = 25
            },
            new Studio
            {
                Id = 2,
                Name = "Studio B",
                SizeSquareMeters = 80,
                HasPoles = false,
                HasAerialRigging = false,
                MaxCapacity = 18
            },
            new Studio
            {
                Id = 3,
                Name = "Studio C",
                SizeSquareMeters = 60,
                HasPoles = true,
                HasAerialRigging = false,
                MaxCapacity = 12
            },
            new Studio
            {
                Id = 4,
                Name = "Studio D",
                SizeSquareMeters = 100,
                HasPoles = false,
                HasAerialRigging = true,
                MaxCapacity = 10
            }
        );

        // Seed sample DanceClasses
        modelBuilder.Entity<DanceClass>().HasData(
            // Monday classes (18:00-21:00)
            new DanceClass { Id = 1, DanceStyleId = 1, StudioId = 1, InstructorName = "Maria Santos", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 20 },
            new DanceClass { Id = 2, DanceStyleId = 5, StudioId = 2, InstructorName = "Carlos Rivera", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 15 },
            new DanceClass { Id = 3, DanceStyleId = 9, StudioId = 3, InstructorName = "Emma Wilson", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 0, 0), MaxStudents = 10 },
            new DanceClass { Id = 4, DanceStyleId = 4, StudioId = 1, InstructorName = "Jake Thompson", Level = Domain.Enums.ClassLevel.Advanced, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(20, 0, 0), EndTime = new TimeSpan(21, 0, 0), MaxStudents = 18 },

            // Tuesday classes
            new DanceClass { Id = 5, DanceStyleId = 2, StudioId = 1, InstructorName = "Sofia Chen", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 20 },
            new DanceClass { Id = 6, DanceStyleId = 6, StudioId = 2, InstructorName = "Carlos Rivera", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 0, 0), MaxStudents = 15 },
            new DanceClass { Id = 7, DanceStyleId = 10, StudioId = 4, InstructorName = "Luna Park", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 30, 0), MaxStudents = 8 },

            // Wednesday classes
            new DanceClass { Id = 8, DanceStyleId = 3, StudioId = 1, InstructorName = "Nina Johnson", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 22 },
            new DanceClass { Id = 9, DanceStyleId = 7, StudioId = 2, InstructorName = "Marco Rossi", Level = Domain.Enums.ClassLevel.Advanced, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 0, 0), MaxStudents = 14 },
            new DanceClass { Id = 10, DanceStyleId = 12, StudioId = 1, InstructorName = "Min-Jun Kim", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(20, 0, 0), EndTime = new TimeSpan(21, 0, 0), MaxStudents = 20 },

            // Thursday classes
            new DanceClass { Id = 11, DanceStyleId = 1, StudioId = 1, InstructorName = "Maria Santos", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 18 },
            new DanceClass { Id = 12, DanceStyleId = 8, StudioId = 2, InstructorName = "Victoria Adams", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 16 },
            new DanceClass { Id = 13, DanceStyleId = 9, StudioId = 3, InstructorName = "Emma Wilson", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 0, 0), MaxStudents = 10 },

            // Friday classes
            new DanceClass { Id = 14, DanceStyleId = 5, StudioId = 1, InstructorName = "Carlos Rivera", Level = Domain.Enums.ClassLevel.Advanced, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 16 },
            new DanceClass { Id = 15, DanceStyleId = 11, StudioId = 1, InstructorName = "DJ Marcus", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 0, 0), MaxStudents = 20 },
            new DanceClass { Id = 16, DanceStyleId = 4, StudioId = 2, InstructorName = "Jake Thompson", Level = Domain.Enums.ClassLevel.Professional, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeSpan(20, 0, 0), EndTime = new TimeSpan(21, 0, 0), MaxStudents = 12 },

            // Saturday classes
            new DanceClass { Id = 17, DanceStyleId = 6, StudioId = 1, InstructorName = "Carlos Rivera", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), MaxStudents = 20 },
            new DanceClass { Id = 18, DanceStyleId = 10, StudioId = 4, InstructorName = "Luna Park", Level = Domain.Enums.ClassLevel.Advanced, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 30, 0), MaxStudents = 8 },
            new DanceClass { Id = 19, DanceStyleId = 2, StudioId = 1, InstructorName = "Sofia Chen", Level = Domain.Enums.ClassLevel.Professional, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(19, 30, 0), EndTime = new TimeSpan(21, 0, 0), MaxStudents = 15 },

            // Sunday classes (10:00-14:00)
            new DanceClass { Id = 20, DanceStyleId = 1, StudioId = 1, InstructorName = "Maria Santos", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), MaxStudents = 25 },
            new DanceClass { Id = 21, DanceStyleId = 3, StudioId = 2, InstructorName = "Nina Johnson", Level = Domain.Enums.ClassLevel.Intermediate, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 0, 0), MaxStudents = 18 },
            new DanceClass { Id = 22, DanceStyleId = 12, StudioId = 1, InstructorName = "Min-Jun Kim", Level = Domain.Enums.ClassLevel.Beginner, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(13, 0, 0), MaxStudents = 22 },
            new DanceClass { Id = 23, DanceStyleId = 9, StudioId = 3, InstructorName = "Emma Wilson", Level = Domain.Enums.ClassLevel.Advanced, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(13, 30, 0), MaxStudents = 8 }
        );
    }
}