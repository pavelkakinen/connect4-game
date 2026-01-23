using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Students;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<StudentViewModel> Students { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PackageStatusFilter { get; set; }

    public SelectList PackageStatusOptions { get; set; } = default!;

    public async Task OnGetAsync()
    {
        PackageStatusOptions = new SelectList(new[]
        {
            new { Value = "", Text = "All Students" },
            new { Value = "active", Text = "Active Package" },
            new { Value = "expired", Text = "Expired Package" },
            new { Value = "none", Text = "No Package" }
        }, "Value", "Text");

        var query = _context.Students.AsQueryable();

        // Search by name
        if (!string.IsNullOrWhiteSpace(SearchName))
        {
            var searchLower = SearchName.ToLower();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(searchLower) ||
                s.LastName.ToLower().Contains(searchLower) ||
                s.Email.ToLower().Contains(searchLower));
        }

        var students = await query
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        // Load packages and bookings for each student
        foreach (var student in students)
        {
            var packages = await _context.Packages
                .Include(p => p.DanceStyle)
                .Where(p => p.StudentId == student.Id)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            var bookings = await _context.Bookings
                .Where(b => b.StudentId == student.Id && b.Attended.HasValue)
                .ToListAsync();

            var activePackage = packages.FirstOrDefault(p => p.IsValid);
            var hasExpiredPackage = packages.Any(p => p.IsExpired);

            var attendedCount = bookings.Count(b => b.Attended == true);
            var attendanceRate = bookings.Count > 0
                ? (decimal)attendedCount / bookings.Count * 100
                : 0;

            var vm = new StudentViewModel
            {
                Student = student,
                CurrentPackage = activePackage,
                HasActivePackage = activePackage != null,
                HasExpiredPackage = hasExpiredPackage && activePackage == null,
                HasNoPackage = !packages.Any(),
                AttendanceRate = attendanceRate
            };

            // Apply package status filter
            if (string.IsNullOrEmpty(PackageStatusFilter) ||
                (PackageStatusFilter == "active" && vm.HasActivePackage) ||
                (PackageStatusFilter == "expired" && vm.HasExpiredPackage) ||
                (PackageStatusFilter == "none" && vm.HasNoPackage))
            {
                Students.Add(vm);
            }
        }
    }

    public string GetPackageStatusBadge(StudentViewModel vm)
    {
        if (vm.HasActivePackage && vm.CurrentPackage != null)
        {
            if (vm.CurrentPackage.HasLowBalance)
                return "<span class=\"badge bg-warning text-dark\">Low Balance</span>";
            return "<span class=\"badge bg-success\">Active</span>";
        }
        if (vm.HasExpiredPackage)
            return "<span class=\"badge bg-danger\">Expired</span>";
        return "<span class=\"badge bg-secondary\">No Package</span>";
    }

    public string GetPackageInfo(StudentViewModel vm)
    {
        if (vm.CurrentPackage == null)
            return "-";

        var pkg = vm.CurrentPackage;
        var typeName = pkg.PackageType switch
        {
            PackageType.TenClassCard => "10-Class Card",
            PackageType.MonthlyUnlimitedSingle => $"Monthly ({pkg.DanceStyle?.Name ?? "Single"})",
            PackageType.MonthlyUnlimitedAll => "Monthly (All Styles)",
            _ => pkg.PackageType.ToString()
        };

        if (pkg.RemainingClasses.HasValue)
            return $"{typeName} ({pkg.RemainingClasses} left)";

        return $"{typeName} (exp: {pkg.ExpiryDate:MMM dd})";
    }

    public class StudentViewModel
    {
        public Student Student { get; set; } = default!;
        public Package? CurrentPackage { get; set; }
        public bool HasActivePackage { get; set; }
        public bool HasExpiredPackage { get; set; }
        public bool HasNoPackage { get; set; }
        public decimal AttendanceRate { get; set; }
    }
}
