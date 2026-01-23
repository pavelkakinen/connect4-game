using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Showcases;

public class EligibilityModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EligibilityModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int? SelectedStudentId { get; set; }

    public SelectList StudentOptions { get; set; } = default!;
    public Student? SelectedStudent { get; set; }
    public StudentStats SelectedStudentStats { get; set; } = new();
    public List<StudentStats> EligibleStudents { get; set; } = new();

    public async Task OnGetAsync()
    {
        var students = await _context.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToListAsync();
        StudentOptions = new SelectList(
            students.Select(s => new { s.Id, Name = $"{s.FullName} ({s.Email})" }),
            "Id", "Name");

        // Calculate stats for selected student
        if (SelectedStudentId.HasValue)
        {
            SelectedStudent = students.FirstOrDefault(s => s.Id == SelectedStudentId.Value);
            if (SelectedStudent != null)
            {
                SelectedStudentStats = await CalculateStudentStatsAsync(SelectedStudent);
            }
        }

        // Find all eligible students
        foreach (var student in students)
        {
            var stats = await CalculateStudentStatsAsync(student);
            if (stats.IsEligible)
            {
                EligibleStudents.Add(stats);
            }
        }
    }

    private async Task<StudentStats> CalculateStudentStatsAsync(Student student)
    {
        var bookings = await _context.Bookings
            .Include(b => b.DanceClass)
            .Where(b => b.StudentId == student.Id && b.Attended.HasValue)
            .ToListAsync();

        var stats = new StudentStats
        {
            Student = student,
            TotalBookings = bookings.Count,
            AttendedCount = bookings.Count(b => b.Attended == true)
        };

        if (stats.TotalBookings > 0)
        {
            stats.AttendanceRate = (decimal)stats.AttendedCount / stats.TotalBookings * 100;
        }

        // Find highest level attended
        var attendedLevels = bookings
            .Where(b => b.Attended == true)
            .Select(b => b.DanceClass.Level)
            .ToList();

        if (attendedLevels.Any())
        {
            stats.HighestLevel = attendedLevels.Max();
        }

        // Check eligibility: 80%+ attendance AND Intermediate+
        stats.IsEligible = stats.AttendanceRate >= 80 &&
                          stats.HighestLevel.HasValue &&
                          (int)stats.HighestLevel.Value >= (int)ClassLevel.Intermediate;

        return stats;
    }

    public string GetLevelBadgeClass(ClassLevel level)
    {
        return level switch
        {
            ClassLevel.Beginner => "bg-success",
            ClassLevel.Intermediate => "bg-primary",
            ClassLevel.Advanced => "bg-warning text-dark",
            ClassLevel.Professional => "bg-danger",
            _ => "bg-secondary"
        };
    }

    public class StudentStats
    {
        public Student Student { get; set; } = default!;
        public int TotalBookings { get; set; }
        public int AttendedCount { get; set; }
        public decimal AttendanceRate { get; set; }
        public ClassLevel? HighestLevel { get; set; }
        public bool IsEligible { get; set; }
    }
}
