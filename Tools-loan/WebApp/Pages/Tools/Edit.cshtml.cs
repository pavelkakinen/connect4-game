using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Tools;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Tool Tool { get; set; } = default!;

    public SelectList CategoryList { get; set; } = default!;
    public SelectList CertificationList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var tool = await _context.Tools.FindAsync(id);
        if (tool == null)
        {
            return NotFound();
        }
        Tool = tool;
        await LoadSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Remove validation for navigation properties (they're not bound from form)
        ModelState.Remove("Tool.Category");
        ModelState.Remove("Tool.RequiredCertification");
        ModelState.Remove("Tool.Loans");
        ModelState.Remove("Tool.MaintenanceRecords");
        ModelState.Remove("Tool.Reservations");

        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        // Validate category exists
        if (Tool.CategoryId <= 0)
        {
            ModelState.AddModelError("Tool.CategoryId", "Please select a category.");
            await LoadSelectListsAsync();
            return Page();
        }

        var existingTool = await _context.Tools.AsNoTracking().FirstOrDefaultAsync(t => t.Id == Tool.Id);
        if (existingTool == null)
        {
            return NotFound();
        }

        _context.Attach(Tool).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Tools.AnyAsync(e => e.Id == Tool.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        CertificationList = new SelectList(await _context.Certifications.ToListAsync(), "Id", "Name");
    }
}
