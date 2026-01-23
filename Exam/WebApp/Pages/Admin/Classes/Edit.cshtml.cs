using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Enums;
using Domain.Models;

namespace WebApp.Pages.Admin.Classes;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public DanceClass? DanceClass { get; set; }

    public SelectList StyleOptions { get; set; } = default!;
    public SelectList StudioOptions { get; set; } = default!;
    public SelectList LevelOptions { get; set; } = default!;
    public SelectList DayOptions { get; set; } = default!;
    public List<Studio> Studios { get; set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a dance style")]
        [Display(Name = "Dance Style")]
        public int DanceStyleId { get; set; }

        [Required(ErrorMessage = "Please select a studio")]
        [Display(Name = "Studio")]
        public int StudioId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Level")]
        public ClassLevel Level { get; set; }

        [Required]
        [Display(Name = "Day of Week")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Range(1, 50)]
        [Display(Name = "Max Students")]
        public int MaxStudents { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        DanceClass = await _context.DanceClasses.FindAsync(id);

        if (DanceClass == null)
        {
            return NotFound();
        }

        // Populate input from existing class
        Input = new InputModel
        {
            Id = DanceClass.Id,
            DanceStyleId = DanceClass.DanceStyleId,
            StudioId = DanceClass.StudioId,
            InstructorName = DanceClass.InstructorName,
            Level = DanceClass.Level,
            DayOfWeek = DanceClass.DayOfWeek,
            StartTime = DanceClass.StartTime,
            EndTime = DanceClass.EndTime,
            MaxStudents = DanceClass.MaxStudents
        };

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await LoadDataAsync();

        if (!ModelState.IsValid)
        {
            DanceClass = await _context.DanceClasses.FindAsync(id);
            return Page();
        }

        var studio = Studios.FirstOrDefault(s => s.Id == Input.StudioId);
        var style = await _context.DanceStyles.FindAsync(Input.DanceStyleId);

        if (studio == null || style == null)
        {
            ModelState.AddModelError("", "Invalid studio or style selected.");
            DanceClass = await _context.DanceClasses.FindAsync(id);
            return Page();
        }

        // Studio constraints
        if (studio.HasPoles && style.Name != "Pole Fitness")
        {
            ModelState.AddModelError("Input.StudioId", "Studio C can only be used for Pole Fitness classes.");
            DanceClass = await _context.DanceClasses.FindAsync(id);
            return Page();
        }

        if (studio.HasAerialRigging && style.Name != "Aerial Silks")
        {
            ModelState.AddModelError("Input.StudioId", "Studio D can only be used for Aerial Silks classes.");
            DanceClass = await _context.DanceClasses.FindAsync(id);
            return Page();
        }

        if (Input.MaxStudents > studio.MaxCapacity)
        {
            ModelState.AddModelError("Input.MaxStudents", $"Max students cannot exceed studio capacity ({studio.MaxCapacity}).");
            DanceClass = await _context.DanceClasses.FindAsync(id);
            return Page();
        }

        // Schedule validation
        var validationError = ValidateScheduleHours(Input.DayOfWeek, Input.StartTime, Input.EndTime);
        if (validationError != null)
        {
            ModelState.AddModelError("", validationError);
            DanceClass = await _context.DanceClasses.FindAsync(id);
            return Page();
        }

        // Check overlaps (excluding current class) - load candidates and check in memory due to SQLite TimeSpan limitations
        var potentialOverlaps = await _context.DanceClasses
            .Where(c => c.Id != id &&
                       c.StudioId == Input.StudioId &&
                       c.DayOfWeek == Input.DayOfWeek)
            .ToListAsync();

        var hasOverlap = potentialOverlaps.Any(c =>
            c.StartTime < Input.EndTime && c.EndTime > Input.StartTime);

        if (hasOverlap)
        {
            ModelState.AddModelError("", "This time slot overlaps with an existing class in the same studio.");
            DanceClass = await _context.DanceClasses.FindAsync(id);
            return Page();
        }

        // Load and update the existing class
        var danceClass = await _context.DanceClasses.FindAsync(id);
        if (danceClass == null)
        {
            return NotFound();
        }

        danceClass.DanceStyleId = Input.DanceStyleId;
        danceClass.StudioId = Input.StudioId;
        danceClass.InstructorName = Input.InstructorName;
        danceClass.Level = Input.Level;
        danceClass.DayOfWeek = Input.DayOfWeek;
        danceClass.StartTime = Input.StartTime;
        danceClass.EndTime = Input.EndTime;
        danceClass.MaxStudents = Input.MaxStudents;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Class updated successfully!";
        return RedirectToPage("Index");
    }

    private string? ValidateScheduleHours(DayOfWeek day, TimeSpan start, TimeSpan end)
    {
        if (end <= start)
            return "End time must be after start time.";

        if (day == System.DayOfWeek.Sunday)
        {
            if (start < new TimeSpan(10, 0, 0) || end > new TimeSpan(14, 0, 0))
                return "Sunday classes must be between 10:00 and 14:00.";
        }
        else
        {
            if (start < new TimeSpan(18, 0, 0) || end > new TimeSpan(21, 0, 0))
                return "Monday-Saturday classes must be between 18:00 and 21:00.";
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
