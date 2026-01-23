using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin.DanceStyles;

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
        [Display(Name = "Style Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
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
        var exists = await _context.DanceStyles
            .AnyAsync(s => s.Name.ToLower() == Input.Name.ToLower());

        if (exists)
        {
            ModelState.AddModelError("Input.Name", "A dance style with this name already exists.");
            return Page();
        }

        var style = new DanceStyle
        {
            Name = Input.Name,
            Description = Input.Description
        };

        _context.DanceStyles.Add(style);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"'{style.Name}' has been created.";
        return RedirectToPage("Index");
    }
}
