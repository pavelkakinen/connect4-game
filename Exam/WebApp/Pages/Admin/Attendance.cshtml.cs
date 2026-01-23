using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin;

public class AttendanceModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public AttendanceModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int? SelectedClassId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? SelectedDate { get; set; }

    [BindProperty]
    public List<int> BookingIds { get; set; } = new();

    [BindProperty]
    public List<string?> AttendanceStatus { get; set; } = new();

    public List<DanceClass> Classes { get; set; } = new();
    public DanceClass? SelectedClass { get; set; }
    public List<Booking> Bookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        // SQLite doesn't support TimeSpan ordering, so we load then sort in memory
        var loadedClasses = await _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .OrderBy(c => c.DayOfWeek)
            .ToListAsync();

        Classes = loadedClasses
            .OrderBy(c => c.DayOfWeek)
            .ThenBy(c => c.StartTime)
            .ToList();

        if (SelectedClassId.HasValue && SelectedDate.HasValue)
        {
            SelectedClass = Classes.FirstOrDefault(c => c.Id == SelectedClassId.Value);

            Bookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.InvitedByStudent)
                .Where(b => b.DanceClassId == SelectedClassId.Value &&
                           b.BookingDate.Date == SelectedDate.Value.Date)
                .OrderBy(b => b.Student.LastName)
                .ThenBy(b => b.Student.FirstName)
                .ToListAsync();
        }

        if (!SelectedDate.HasValue)
        {
            SelectedDate = DateTime.Today;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!SelectedClassId.HasValue || !SelectedDate.HasValue)
        {
            return RedirectToPage();
        }

        for (int i = 0; i < BookingIds.Count; i++)
        {
            var bookingId = BookingIds[i];
            var status = i < AttendanceStatus.Count ? AttendanceStatus[i] : null;

            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Attended = string.IsNullOrEmpty(status) ? null : bool.Parse(status);
            }
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Attendance saved successfully!";

        return RedirectToPage(new { SelectedClassId, SelectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") });
    }
}
