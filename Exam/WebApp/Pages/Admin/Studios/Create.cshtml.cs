using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin.Studios;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
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
        public int SizeSquareMeters { get; set; } = 50;

        [Required]
        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100")]
        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; } = 20;

        [Display(Name = "Has Poles (for Pole Fitness)")]
        public bool HasPoles { get; set; }

        [Display(Name = "Has Aerial Rigging (for Aerial Silks)")]
        public bool HasAerialRigging { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check for duplicate name
        var exists = await _context.Studios
            .AnyAsync(s => s.Name.ToLower() == Input.Name.ToLower());

        if (exists)
        {
            ModelState.AddModelError("Input.Name", "A studio with this name already exists.");
            return Page();
        }

        var studio = new Studio
        {
            Name = Input.Name,
            Description = Input.Description,
            SizeSquareMeters = Input.SizeSquareMeters,
            MaxCapacity = Input.MaxCapacity,
            HasPoles = Input.HasPoles,
            HasAerialRigging = Input.HasAerialRigging
        };

        _context.Studios.Add(studio);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"'{studio.Name}' has been created.";
        return RedirectToPage("Index");
    }
}
