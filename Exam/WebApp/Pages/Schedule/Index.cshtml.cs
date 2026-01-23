using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Schedule;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<DanceClass> Classes { get; set; } = new();
    public List<Studio> Studios { get; set; } = new();
    public Dictionary<int, int> BookingCounts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? StyleFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public ClassLevel? LevelFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DayOfWeek? DayFilter { get; set; }

    public SelectList StyleOptions { get; set; } = default!;
    public SelectList LevelOptions { get; set; } = default!;
    public SelectList DayOptions { get; set; } = default!;

    public async Task OnGetAsync()
    {
        // Load filter options
        var styles = await _context.DanceStyles.OrderBy(s => s.Name).ToListAsync();
        StyleOptions = new SelectList(styles, "Id", "Name");

        LevelOptions = new SelectList(
            Enum.GetValues<ClassLevel>().Select(l => new { Value = (int)l, Text = l.ToString() }),
            "Value", "Text");

        var days = new[]
        {
            new { Value = (int)DayOfWeek.Monday, Text = "Monday" },
            new { Value = (int)DayOfWeek.Tuesday, Text = "Tuesday" },
            new { Value = (int)DayOfWeek.Wednesday, Text = "Wednesday" },
            new { Value = (int)DayOfWeek.Thursday, Text = "Thursday" },
            new { Value = (int)DayOfWeek.Friday, Text = "Friday" },
            new { Value = (int)DayOfWeek.Saturday, Text = "Saturday" },
            new { Value = (int)DayOfWeek.Sunday, Text = "Sunday" }
        };
        DayOptions = new SelectList(days, "Value", "Text");

        // Build query with filters
        var query = _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .AsQueryable();

        if (StyleFilter.HasValue)
        {
            query = query.Where(c => c.DanceStyleId == StyleFilter.Value);
        }

        if (LevelFilter.HasValue)
        {
            query = query.Where(c => c.Level == LevelFilter.Value);
        }

        if (DayFilter.HasValue)
        {
            query = query.Where(c => c.DayOfWeek == DayFilter.Value);
        }

        // Load from DB without TimeSpan ordering (SQLite doesn't support it)
        // Then sort in memory
        var loadedClasses = await query
            .OrderBy(c => c.DayOfWeek)
            .ToListAsync();

        Classes = loadedClasses
            .OrderBy(c => c.DayOfWeek)
            .ThenBy(c => c.StartTime)
            .ToList();

        // Load booking counts for this week
        var today = DateTime.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
        var weekEnd = weekStart.AddDays(7);

        var bookings = await _context.Bookings
            .Where(b => b.BookingDate >= weekStart && b.BookingDate < weekEnd)
            .GroupBy(b => b.DanceClassId)
            .Select(g => new { ClassId = g.Key, Count = g.Count() })
            .ToListAsync();

        BookingCounts = bookings.ToDictionary(b => b.ClassId, b => b.Count);

        // Load studios
        Studios = await _context.Studios.OrderBy(s => s.Name).ToListAsync();
    }

    public int GetBookingCount(int classId)
    {
        return BookingCounts.GetValueOrDefault(classId, 0);
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
}