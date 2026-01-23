using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Admin.Classes;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList StyleOptions { get; set; } = default!;
    public SelectList StudioOptions { get; set; } = default!;
    public SelectList LevelOptions { get; set; } = default!;
    public SelectList DayOptions { get; set; } = default!;
    public List<Studio> Studios { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Please select a dance style")]
        [Display(Name = "Dance Style")]
        public int? DanceStyleId { get; set; }

        [Required(ErrorMessage = "Please select a studio")]
        [Display(Name = "Studio")]
        public int? StudioId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a level")]
        [Display(Name = "Level")]
        public ClassLevel? Level { get; set; }

        [Required(ErrorMessage = "Please select a day")]
        [Display(Name = "Day of Week")]
        public DayOfWeek? DayOfWeek { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(18, 0, 0);

        [Required]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(19, 0, 0);

        [Required]
        [Range(1, 50)]
        [Display(Name = "Max Students")]
        public int MaxStudents { get; set; } = 15;
    }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadDataAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Validate studio constraints
        var studio = Studios.FirstOrDefault(s => s.Id == Input.StudioId);
        var style = await _context.DanceStyles.FindAsync(Input.DanceStyleId);

        if (studio == null || style == null)
        {
            ModelState.AddModelError("", "Invalid studio or style selected.");
            return Page();
        }

        // Studio C: Pole Fitness only
        if (studio.HasPoles && style.Name != "Pole Fitness")
        {
            ModelState.AddModelError("Input.StudioId", "Studio C can only be used for Pole Fitness classes.");
            return Page();
        }

        // Studio D: Aerial Silks only
        if (studio.HasAerialRigging && style.Name != "Aerial Silks")
        {
            ModelState.AddModelError("Input.StudioId", "Studio D can only be used for Aerial Silks classes.");
            return Page();
        }

        // Validate capacity
        if (Input.MaxStudents > studio.MaxCapacity)
        {
            ModelState.AddModelError("Input.MaxStudents", $"Max students cannot exceed studio capacity ({studio.MaxCapacity}).");
            return Page();
        }

        // Validate schedule hours
        var validationError = ValidateScheduleHours(Input.DayOfWeek!.Value, Input.StartTime, Input.EndTime);
        if (validationError != null)
        {
            ModelState.AddModelError("", validationError);
            return Page();
        }

        // Check for overlapping classes in same studio - load candidates and check in memory due to SQLite TimeSpan limitations
        var potentialOverlaps = await _context.DanceClasses
            .Where(c => c.StudioId == Input.StudioId &&
                       c.DayOfWeek == Input.DayOfWeek)
            .ToListAsync();

        var hasOverlap = potentialOverlaps.Any(c =>
            c.StartTime < Input.EndTime && c.EndTime > Input.StartTime);

        if (hasOverlap)
        {
            ModelState.AddModelError("", "This time slot overlaps with an existing class in the same studio.");
            return Page();
        }

        var danceClass = new DanceClass
        {
            DanceStyleId = Input.DanceStyleId!.Value,
            StudioId = Input.StudioId!.Value,
            InstructorName = Input.InstructorName,
            Level = Input.Level!.Value,
            DayOfWeek = Input.DayOfWeek!.Value,
            StartTime = Input.StartTime,
            EndTime = Input.EndTime,
            MaxStudents = Input.MaxStudents
        };

        _context.DanceClasses.Add(danceClass);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Class created successfully!";
        return RedirectToPage("Index");
    }

    private string? ValidateScheduleHours(DayOfWeek day, TimeSpan start, TimeSpan end)
    {
        if (end <= start)
        {
            return "End time must be after start time.";
        }

        if (day == System.DayOfWeek.Sunday)
        {
            // Sunday: 10:00-14:00
            if (start < new TimeSpan(10, 0, 0) || end > new TimeSpan(14, 0, 0))
            {
                return "Sunday classes must be between 10:00 and 14:00.";
            }
        }
        else
        {
            // Mon-Sat: 18:00-21:00
            if (start < new TimeSpan(18, 0, 0) || end > new TimeSpan(21, 0, 0))
            {
                return "Monday-Saturday classes must be between 18:00 and 21:00.";
            }
        }

        return null;
    }

    private async Task LoadDataAsync()
    {
        var styles = await _context.DanceStyles.OrderBy(s => s.Name).ToListAsync();
        StyleOptions = new SelectList(styles, "Id", "Name");

        Studios = await _context.Studios.OrderBy(s => s.Name).ToListAsync();
        StudioOptions = new SelectList(Studios, "Id", "Name");

        LevelOptions = new SelectList(
            Enum.GetValues<ClassLevel>().Select(l => new { Value = (int)l, Text = l.ToString() }),
            "Value", "Text");

        var days = new[]
        {
            new { Value = (int)System.DayOfWeek.Monday, Text = "Monday" },
            new { Value = (int)System.DayOfWeek.Tuesday, Text = "Tuesday" },
            new { Value = (int)System.DayOfWeek.Wednesday, Text = "Wednesday" },
            new { Value = (int)System.DayOfWeek.Thursday, Text = "Thursday" },
            new { Value = (int)System.DayOfWeek.Friday, Text = "Friday" },
            new { Value = (int)System.DayOfWeek.Saturday, Text = "Saturday" },
            new { Value = (int)System.DayOfWeek.Sunday, Text = "Sunday" }
        };
        DayOptions = new SelectList(days, "Value", "Text");
    }
}
