using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Categories;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; } = default!;

    public int ToolCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Tools)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }
        Category = category;
        ToolCount = category.Tools.Count;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Tools)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        // Check if category has tools
        if (category.Tools.Any())
        {
            ModelState.AddModelError(string.Empty,
                $"Cannot delete category '{category.Name}' because it has {category.Tools.Count} tool(s) assigned to it.");
            Category = category;
            ToolCount = category.Tools.Count;
            return Page();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
