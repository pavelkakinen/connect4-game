using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Students;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Student Student { get; set; } = default!;
    public List<Package> Packages { get; set; } = new();
    public Package? ActivePackage { get; set; }
    public List<Booking> RecentBookings { get; set; } = new();
    public List<TrialUsage> TrialUsages { get; set; } = new();
    public List<DanceStyle> AvailableTrialStyles { get; set; } = new();

    // Stats
    public int TotalBookings { get; set; }
    public int AttendedCount { get; set; }
    public decimal AttendanceRate { get; set; }
    public ClassLevel? HighestLevel { get; set; }
    public bool IsShowcaseEligible { get; set; }

    // Package assignment
    [BindProperty]
    public PackageInputModel PackageInput { get; set; } = new();
    public SelectList PackageTypeOptions { get; set; } = default!;
    public SelectList DanceStyleOptions { get; set; } = default!;

    // Booking
    [BindProperty]
    public BookingInputModel BookingInput { get; set; } = new();
    public List<DanceClass> AvailableClasses { get; set; } = new();
    public SelectList BookingStyleOptions { get; set; } = default!;
    public SelectList BookingDayOptions { get; set; } = default!;

    public class PackageInputModel
    {
        public PackageType PackageType { get; set; }
        public int? DanceStyleId { get; set; }
    }

    public class BookingInputModel
    {
        public int ClassId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Today;
        public string PaymentMethod { get; set; } = string.Empty; // "trial" or "package_{id}"
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        Student = student;
        await LoadStudentDataAsync();
        await LoadBookingOptionsAsync();
        LoadPackageOptions();

        return Page();
    }

    public async Task<IActionResult> OnPostAssignPackageAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        Student = student;

        // Determine package properties based on type
        var now = DateTime.UtcNow;
        int? remainingClasses = null;
        DateTime expiryDate;
        decimal price;

        switch (PackageInput.PackageType)
        {
            case PackageType.TenClassCard:
                remainingClasses = 10;
                expiryDate = now.AddMonths(3);
                price = 120;
                PackageInput.DanceStyleId = null;
                break;
            case PackageType.MonthlyUnlimitedSingle:
                if (!PackageInput.DanceStyleId.HasValue)
                {
                    TempData["Error"] = "Please select a dance style for monthly single-style package.";
                    await LoadStudentDataAsync();
                    await LoadBookingOptionsAsync();
                    LoadPackageOptions();
                    return Page();
                }
                expiryDate = now.AddMonths(1);
                price = 75;
                break;
            case PackageType.MonthlyUnlimitedAll:
                expiryDate = now.AddMonths(1);
                price = 130;
                PackageInput.DanceStyleId = null;
                break;
            default:
                TempData["Error"] = "Invalid package type.";
                await LoadStudentDataAsync();
                await LoadBookingOptionsAsync();
                LoadPackageOptions();
                return Page();
        }

        var package = new Package
        {
            StudentId = id,
            PackageType = PackageInput.PackageType,
            DanceStyleId = PackageInput.DanceStyleId,
            PurchaseDate = now,
            ExpiryDate = expiryDate,
            RemainingClasses = remainingClasses,
            Price = price
        };

        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Package assigned successfully!";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostBookClassAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        Student = student;

        // Load the class
        var danceClass = await _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .FirstOrDefaultAsync(c => c.Id == BookingInput.ClassId);

        if (danceClass == null)
        {
            TempData["Error"] = "Class not found.";
            return RedirectToPage(new { id });
        }

        // Validate booking date matches day of week
        if (BookingInput.BookingDate.DayOfWeek != danceClass.DayOfWeek)
        {
            TempData["Error"] = $"Selected date must be a {danceClass.DayOfWeek}.";
            return RedirectToPage(new { id });
        }

        // Check if already booked
        var alreadyBooked = await _context.Bookings
            .AnyAsync(b => b.StudentId == id &&
                          b.DanceClassId == BookingInput.ClassId &&
                          b.BookingDate.Date == BookingInput.BookingDate.Date);

        if (alreadyBooked)
        {
            TempData["Error"] = "Student is already booked for this class on this date.";
            return RedirectToPage(new { id });
        }

        // Check class capacity
        var currentBookings = await _context.Bookings
            .CountAsync(b => b.DanceClassId == BookingInput.ClassId &&
                            b.BookingDate.Date == BookingInput.BookingDate.Date);

        if (currentBookings >= danceClass.MaxStudents)
        {
            TempData["Error"] = "Class is full for this date.";
            return RedirectToPage(new { id });
        }

        // Validate payment method
        var booking = new Booking
        {
            StudentId = id,
            DanceClassId = BookingInput.ClassId,
            BookingDate = BookingInput.BookingDate.Date,
            CreatedAt = DateTime.UtcNow,
            Attended = null
        };

        if (BookingInput.PaymentMethod == "trial")
        {
            // Check trial eligibility
            var hasUsedTrial = await _context.TrialUsages
                .AnyAsync(t => t.StudentId == id && t.DanceStyleId == danceClass.DanceStyleId);

            if (hasUsedTrial)
            {
                TempData["Error"] = $"Student has already used their free trial for {danceClass.DanceStyle.Name}.";
                return RedirectToPage(new { id });
            }

            booking.IsTrial = true;
            booking.PackageId = null;

            // Record trial usage
            _context.TrialUsages.Add(new TrialUsage
            {
                StudentId = id,
                DanceStyleId = danceClass.DanceStyleId,
                TrialDate = DateTime.UtcNow
            });
        }
        else if (BookingInput.PaymentMethod.StartsWith("package_"))
        {
            if (!int.TryParse(BookingInput.PaymentMethod.Replace("package_", ""), out var packageId))
            {
                TempData["Error"] = "Invalid package selected.";
                return RedirectToPage(new { id });
            }

            var package = await _context.Packages
                .Include(p => p.DanceStyle)
                .FirstOrDefaultAsync(p => p.Id == packageId && p.StudentId == id);

            if (package == null)
            {
                TempData["Error"] = "Package not found.";
                return RedirectToPage(new { id });
            }

            // Validate package
            if (package.IsExpired)
            {
                TempData["Error"] = "Package has expired.";
                return RedirectToPage(new { id });
            }

            if (package.PackageType == PackageType.TenClassCard &&
                (!package.RemainingClasses.HasValue || package.RemainingClasses.Value <= 0))
            {
                TempData["Error"] = "Package has no classes remaining.";
                return RedirectToPage(new { id });
            }

            if (package.PackageType == PackageType.MonthlyUnlimitedSingle &&
                package.DanceStyleId != danceClass.DanceStyleId)
            {
                TempData["Error"] = $"Package is for {package.DanceStyle?.Name} only, not {danceClass.DanceStyle.Name}.";
                return RedirectToPage(new { id });
            }

            booking.IsTrial = false;
            booking.PackageId = packageId;

            // Decrement class count for 10-class card
            if (package.PackageType == PackageType.TenClassCard && package.RemainingClasses.HasValue)
            {
                package.RemainingClasses--;
            }
        }
        else
        {
            TempData["Error"] = "Please select a payment method.";
            return RedirectToPage(new { id });
        }

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Booked {danceClass.DanceStyle.Name} on {BookingInput.BookingDate:dddd, MMM dd}!";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelBookingAsync(int id, int bookingId)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings
            .Include(b => b.DanceClass)
                .ThenInclude(c => c.DanceStyle)
            .Include(b => b.Package)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.StudentId == id);

        if (booking == null)
        {
            TempData["Error"] = "Booking not found.";
            return RedirectToPage(new { id });
        }

        // Only allow canceling future bookings
        if (booking.BookingDate.Date < DateTime.Today)
        {
            TempData["Error"] = "Cannot cancel past bookings.";
            return RedirectToPage(new { id });
        }

        // Refund logic
        if (booking.IsTrial)
        {
            // Remove trial usage so they can use trial again for this style
            var trialUsage = await _context.TrialUsages
                .FirstOrDefaultAsync(t => t.StudentId == id && t.DanceStyleId == booking.DanceClass.DanceStyleId);

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

        var className = booking.DanceClass.DanceStyle.Name;
        var bookingDate = booking.BookingDate;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Booking for {className} on {bookingDate:MMM dd} has been cancelled.";
        return RedirectToPage(new { id });
    }

    private async Task LoadStudentDataAsync()
    {
        // Load packages
        Packages = await _context.Packages
            .Include(p => p.DanceStyle)
            .Where(p => p.StudentId == Student.Id)
            .OrderByDescending(p => p.PurchaseDate)
            .ToListAsync();

        ActivePackage = Packages.FirstOrDefault(p => p.IsValid);

        // Load recent bookings
        var bookingsQuery = await _context.Bookings
            .Include(b => b.DanceClass)
                .ThenInclude(c => c.DanceStyle)
            .Include(b => b.DanceClass)
                .ThenInclude(c => c.Studio)
            .Where(b => b.StudentId == Student.Id)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        RecentBookings = bookingsQuery
            .OrderByDescending(b => b.BookingDate)
            .ThenBy(b => b.DanceClass.StartTime)
            .Take(20)
            .ToList();

        // Load trial usage
        TrialUsages = await _context.TrialUsages
            .Include(t => t.DanceStyle)
            .Where(t => t.StudentId == Student.Id)
            .ToListAsync();

        // Calculate available trial styles
        var usedStyleIds = TrialUsages.Select(t => t.DanceStyleId).ToHashSet();
        AvailableTrialStyles = await _context.DanceStyles
            .Where(s => !usedStyleIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .ToListAsync();

        // Calculate stats
        var allBookings = await _context.Bookings
            .Include(b => b.DanceClass)
            .Where(b => b.StudentId == Student.Id && b.Attended.HasValue)
            .ToListAsync();

        TotalBookings = allBookings.Count;
        AttendedCount = allBookings.Count(b => b.Attended == true);
        AttendanceRate = TotalBookings > 0
            ? (decimal)AttendedCount / TotalBookings * 100
            : 0;

        // Highest level attended
        var attendedLevels = allBookings
            .Where(b => b.Attended == true)
            .Select(b => b.DanceClass.Level)
            .ToList();

        if (attendedLevels.Any())
        {
            HighestLevel = attendedLevels.Max();
        }

        // Showcase eligibility
        IsShowcaseEligible = AttendanceRate >= 80 &&
                            HighestLevel.HasValue &&
                            (int)HighestLevel.Value >= (int)ClassLevel.Intermediate;
    }

    private async Task LoadBookingOptionsAsync()
    {
        // Load all classes for booking
        var classesQuery = await _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .OrderBy(c => c.DanceStyleId)
            .ThenBy(c => c.DayOfWeek)
            .ToListAsync();

        AvailableClasses = classesQuery
            .OrderBy(c => c.DanceStyle.Name)
            .ThenBy(c => c.DayOfWeek)
            .ThenBy(c => c.StartTime)
            .ToList();

        var styles = await _context.DanceStyles.OrderBy(s => s.Name).ToListAsync();
        BookingStyleOptions = new SelectList(styles, "Id", "Name");

        BookingDayOptions = new SelectList(new[]
        {
            new { Value = (int)DayOfWeek.Monday, Text = "Monday" },
            new { Value = (int)DayOfWeek.Tuesday, Text = "Tuesday" },
            new { Value = (int)DayOfWeek.Wednesday, Text = "Wednesday" },
            new { Value = (int)DayOfWeek.Thursday, Text = "Thursday" },
            new { Value = (int)DayOfWeek.Friday, Text = "Friday" },
            new { Value = (int)DayOfWeek.Saturday, Text = "Saturday" },
            new { Value = (int)DayOfWeek.Sunday, Text = "Sunday" }
        }, "Value", "Text");
    }

    private void LoadPackageOptions()
    {
        PackageTypeOptions = new SelectList(new[]
        {
            new { Value = (int)PackageType.TenClassCard, Text = "10-Class Card (120, 3 months)" },
            new { Value = (int)PackageType.MonthlyUnlimitedSingle, Text = "Monthly Unlimited Single Style (75)" },
            new { Value = (int)PackageType.MonthlyUnlimitedAll, Text = "Monthly Unlimited All Styles (130)" }
        }, "Value", "Text");

        var styles = _context.DanceStyles.OrderBy(s => s.Name).ToList();
        DanceStyleOptions = new SelectList(styles, "Id", "Name");
    }

    public string GetPackageTypeName(PackageType type)
    {
        return type switch
        {
            PackageType.TenClassCard => "10-Class Card",
            PackageType.MonthlyUnlimitedSingle => "Monthly Unlimited (Single Style)",
            PackageType.MonthlyUnlimitedAll => "Monthly Unlimited (All Styles)",
            _ => type.ToString()
        };
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

    public List<Package> GetValidPackagesForClass(DanceClass danceClass)
    {
        return Packages.Where(p =>
            p.IsValid &&
            (p.PackageType == PackageType.TenClassCard ||
             p.PackageType == PackageType.MonthlyUnlimitedAll ||
             (p.PackageType == PackageType.MonthlyUnlimitedSingle && p.DanceStyleId == danceClass.DanceStyleId))
        ).ToList();
    }

    public bool CanUseTrial(int danceStyleId)
    {
        return !TrialUsages.Any(t => t.DanceStyleId == danceStyleId);
    }
}
