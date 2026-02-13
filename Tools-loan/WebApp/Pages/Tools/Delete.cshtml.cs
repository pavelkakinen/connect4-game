using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Tools;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Tool Tool { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var tool = await _context.Tools
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tool == null)
        {
            return NotFound();
        }
        Tool = tool;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var tool = await _context.Tools.FindAsync(id);
        if (tool != null)
        {
            _context.Tools.Remove(tool);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
