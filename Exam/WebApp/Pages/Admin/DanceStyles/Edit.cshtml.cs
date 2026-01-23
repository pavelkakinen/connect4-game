using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin.DanceStyles;

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
        [Display(Name = "Style Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var style = await _context.DanceStyles
            .Include(s => s.DanceClasses)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (style == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = style.Id,
            Name = style.Name,
            Description = style.Description
        };

        ClassCount = style.DanceClasses.Count;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadCountsAsync(id);
            return Page();
        }

        // Check for duplicate name (excluding current)
        var exists = await _context.DanceStyles
            .AnyAsync(s => s.Id != id && s.Name.ToLower() == Input.Name.ToLower());

        if (exists)
        {
            ModelState.AddModelError("Input.Name", "A dance style with this name already exists.");
            await LoadCountsAsync(id);
            return Page();
        }

        var style = await _context.DanceStyles.FindAsync(id);
        if (style == null)
        {
            return NotFound();
        }

        style.Name = Input.Name;
        style.Description = Input.Description;

        await _context.SaveChangesAsync();

        TempData["Success"] = $"'{style.Name}' has been updated.";
        return RedirectToPage("Index");
    }

    private async Task LoadCountsAsync(int id)
    {
        var style = await _context.DanceStyles
            .Include(s => s.DanceClasses)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (style != null)
        {
            ClassCount = style.DanceClasses.Count;
        }
    }
}
