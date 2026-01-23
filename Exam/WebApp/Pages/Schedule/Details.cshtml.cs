using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Schedule;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public DanceClass DanceClass { get; set; } = default!;
    public List<Booking> Bookings { get; set; } = new();
    public List<DateTime> AvailableDates { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public DateTime? SelectedDate { get; set; }

    // Add student booking
    [BindProperty]
    public AddStudentInput Input { get; set; } = new();
    public SelectList StudentOptions { get; set; } = default!;

    public class AddStudentInput
    {
        public int StudentId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var danceClass = await _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (danceClass == null)
        {
            return NotFound();
        }

        DanceClass = danceClass;

        // Set default date to next occurrence of this class
        if (!SelectedDate.HasValue)
        {
            SelectedDate = GetNextDateForDayOfWeek(danceClass.DayOfWeek);
        }

        await LoadBookingsAsync();
        await LoadStudentOptionsAsync();
        LoadAvailableDates();

        return Page();
    }

    public async Task<IActionResult> OnPostAddStudentAsync(int id)
    {
        var danceClass = await _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (danceClass == null)
        {
            return NotFound();
        }

        DanceClass = danceClass;

        if (!SelectedDate.HasValue)
        {
            TempData["Error"] = "Please select a date.";
            return RedirectToPage(new { id });
        }

        // Validate date matches day of week
        if (SelectedDate.Value.DayOfWeek != danceClass.DayOfWeek)
        {
            TempData["Error"] = $"Selected date must be a {danceClass.DayOfWeek}.";
            return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
        }

        // Load student
        var student = await _context.Students.FindAsync(Input.StudentId);
        if (student == null)
        {
            TempData["Error"] = "Student not found.";
            return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
        }

        // Check if already booked
        var alreadyBooked = await _context.Bookings
            .AnyAsync(b => b.StudentId == Input.StudentId &&
                          b.DanceClassId == id &&
                          b.BookingDate.Date == SelectedDate.Value.Date);

        if (alreadyBooked)
        {
            TempData["Error"] = $"{student.FullName} is already booked for this class on this date.";
            return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
        }

        // Check capacity
        var currentBookings = await _context.Bookings
            .CountAsync(b => b.DanceClassId == id && b.BookingDate.Date == SelectedDate.Value.Date);

        if (currentBookings >= danceClass.MaxStudents)
        {
            TempData["Error"] = "Class is full for this date.";
            return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
        }

        // Create booking
        var booking = new Booking
        {
            StudentId = Input.StudentId,
            DanceClassId = id,
            BookingDate = SelectedDate.Value.Date,
            CreatedAt = DateTime.UtcNow,
            Attended = null
        };

        if (Input.PaymentMethod == "trial")
        {
            // Check trial eligibility
            var hasUsedTrial = await _context.TrialUsages
                .AnyAsync(t => t.StudentId == Input.StudentId && t.DanceStyleId == danceClass.DanceStyleId);

            if (hasUsedTrial)
            {
                TempData["Error"] = $"{student.FullName} has already used their trial for {danceClass.DanceStyle.Name}.";
                return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
            }

            booking.IsTrial = true;
            booking.PackageId = null;

            _context.TrialUsages.Add(new TrialUsage
            {
                StudentId = Input.StudentId,
                DanceStyleId = danceClass.DanceStyleId,
                TrialDate = DateTime.UtcNow
            });
        }
        else if (Input.PaymentMethod.StartsWith("package_"))
        {
            if (!int.TryParse(Input.PaymentMethod.Replace("package_", ""), out var packageId))
            {
                TempData["Error"] = "Invalid package.";
                return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
            }

            var package = await _context.Packages
                .Include(p => p.DanceStyle)
                .FirstOrDefaultAsync(p => p.Id == packageId && p.StudentId == Input.StudentId);

            if (package == null)
            {
                TempData["Error"] = "Package not found.";
                return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
            }

            if (package.IsExpired)
            {
                TempData["Error"] = "Package has expired.";
                return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
            }

            if (package.PackageType == PackageType.TenClassCard &&
                (!package.RemainingClasses.HasValue || package.RemainingClasses.Value <= 0))
            {
                TempData["Error"] = "Package has no classes remaining.";
                return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
            }

            if (package.PackageType == PackageType.MonthlyUnlimitedSingle &&
                package.DanceStyleId != danceClass.DanceStyleId)
            {
                TempData["Error"] = $"Package is for {package.DanceStyle?.Name} only.";
                return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
            }

            booking.IsTrial = false;
            booking.PackageId = packageId;

            if (package.PackageType == PackageType.TenClassCard && package.RemainingClasses.HasValue)
            {
                package.RemainingClasses--;
            }
        }
        else
        {
            TempData["Error"] = "Please select a payment method.";
            return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
        }

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"{student.FullName} booked successfully!";
        return RedirectToPage(new { id, selectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> OnPostCancelBookingAsync(int id, int bookingId)
    {
        var danceClass = await _context.DanceClasses
            .Include(c => c.DanceStyle)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (danceClass == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Package)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.DanceClassId == id);

        if (booking == null)
        {
            TempData["Error"] = "Booking not found.";
            return RedirectToPage(new { id, selectedDate = SelectedDate?.ToString("yyyy-MM-dd") });
        }

        // Only allow canceling future bookings
        if (booking.BookingDate.Date < DateTime.Today)
        {
            TempData["Error"] = "Cannot cancel past bookings.";
            return RedirectToPage(new { id, selectedDate = booking.BookingDate.ToString("yyyy-MM-dd") });
        }

        // Refund logic
        if (booking.IsTrial)
        {
            // Remove trial usage so they can use trial again for this style
            var trialUsage = await _context.TrialUsages
                .FirstOrDefaultAsync(t => t.StudentId == booking.StudentId && t.DanceStyleId == danceClass.DanceStyleId);

            if (trialUsage != null)
            {
                _context.TrialUsages.Remove(trialUsage);
            }
        }
        else if (booking.PackageId.HasValue && booking.Package != null)
        {
            // Refund class to 10-class card
            if (booking.Package.PackageType == PackageType.TenClassCard)
            {
                booking.Package.RemainingClasses = (booking.Package.RemainingClasses ?? 0) + 1;
            }
        }

        var studentName = booking.Student.FullName;
        var bookingDate = booking.BookingDate;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Booking for {studentName} has been cancelled.";
        return RedirectToPage(new { id, selectedDate = bookingDate.ToString("yyyy-MM-dd") });
    }

    private async Task LoadBookingsAsync()
    {
        if (!SelectedDate.HasValue) return;

        Bookings = await _context.Bookings
            .Include(b => b.Student)
            .Include(b => b.Package)
            .Where(b => b.DanceClassId == DanceClass.Id && b.BookingDate.Date == SelectedDate.Value.Date)
            .OrderBy(b => b.Student.LastName)
            .ThenBy(b => b.Student.FirstName)
            .ToListAsync();
    }

    private async Task LoadStudentOptionsAsync()
    {
        var students = await _context.Students
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        StudentOptions = new SelectList(
            students.Select(s => new { s.Id, Name = $"{s.FullName} ({s.Email})" }),
            "Id", "Name");
    }

    private void LoadAvailableDates()
    {
        var today = DateTime.Today;
        var dates = new List<DateTime>();

        for (int i = 0; i < 28; i++)
        {
            var date = today.AddDays(i);
            if (date.DayOfWeek == DanceClass.DayOfWeek)
            {
                dates.Add(date);
                if (dates.Count >= 4) break;
            }
        }

        AvailableDates = dates;
    }

    private DateTime GetNextDateForDayOfWeek(DayOfWeek dayOfWeek)
    {
        var today = DateTime.Today;
        var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7;
        return today.AddDays(daysUntil);
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

    public async Task<List<PaymentOption>> GetPaymentOptionsForStudent(int studentId)
    {
        var options = new List<PaymentOption>();

        // Check trial eligibility
        var hasUsedTrial = await _context.TrialUsages
            .AnyAsync(t => t.StudentId == studentId && t.DanceStyleId == DanceClass.DanceStyleId);

        if (!hasUsedTrial)
        {
            options.Add(new PaymentOption { Value = "trial", Name = "Free Trial" });
        }

        // Get valid packages
        var packages = await _context.Packages
            .Include(p => p.DanceStyle)
            .Where(p => p.StudentId == studentId)
            .ToListAsync();

        foreach (var pkg in packages.Where(p => p.IsValid))
        {
            if (pkg.PackageType == PackageType.MonthlyUnlimitedSingle &&
                pkg.DanceStyleId != DanceClass.DanceStyleId)
            {
                continue;
            }

            var name = pkg.PackageType switch
            {
                PackageType.TenClassCard => $"10-Class Card ({pkg.RemainingClasses} left)",
                PackageType.MonthlyUnlimitedSingle => $"Monthly ({pkg.DanceStyle?.Name})",
                PackageType.MonthlyUnlimitedAll => "Monthly (All Styles)",
                _ => pkg.PackageType.ToString()
            };

            options.Add(new PaymentOption { Value = $"package_{pkg.Id}", Name = name });
        }

        return options;
    }

    public class PaymentOption
    {
        public string Value { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
