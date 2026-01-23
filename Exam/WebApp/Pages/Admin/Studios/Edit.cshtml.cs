using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin.Studios;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int ClassCount { get; set; }

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [Display(Name = "Studio Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Range(10, 1000, ErrorMessage = "Size must be between 10 and 1000 m²")]
        [Display(Name = "Size (m²)")]
        public int SizeSquareMeters { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100")]
        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; }

        [Display(Name = "Has Poles (for Pole Fitness)")]
        public bool HasPoles { get; set; }

        [Display(Name = "Has Aerial Rigging (for Aerial Silks)")]
        public bool HasAerialRigging { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var studio = await _context.Studios
            .Include(s => s.DanceClasses)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (studio == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = studio.Id,
            Name = studio.Name,
            Description = studio.Description,
            SizeSquareMeters = studio.SizeSquareMeters,
            MaxCapacity = studio.MaxCapacity,
            HasPoles = studio.HasPoles,
            HasAerialRigging = studio.HasAerialRigging
        };

        ClassCount = studio.DanceClasses.Count;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadClassCountAsync(id);
            return Page();
        }

        // Check for duplicate name (excluding current)
        var exists = await _context.Studios
            .AnyAsync(s => s.Id != id && s.Name.ToLower() == Input.Name.ToLower());

        if (exists)
        {
            ModelState.AddModelError("Input.Name", "A studio with this name already exists.");
            await LoadClassCountAsync(id);
            return Page();
        }

        var studio = await _context.Studios.FindAsync(id);
        if (studio == null)
        {
            return NotFound();
        }

        studio.Name = Input.Name;
        studio.Description = Input.Description;
        studio.SizeSquareMeters = Input.SizeSquareMeters;
        studio.MaxCapacity = Input.MaxCapacity;
        studio.HasPoles = Input.HasPoles;
        studio.HasAerialRigging = Input.HasAerialRigging;

        await _context.SaveChangesAsync();

        TempData["Success"] = $"'{studio.Name}' has been updated.";
        return RedirectToPage("Index");
    }

    private async Task LoadClassCountAsync(int id)
    {
        ClassCount = await _context.DanceClasses.CountAsync(c => c.StudioId == id);
    }
}
