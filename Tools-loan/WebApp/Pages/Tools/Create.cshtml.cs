using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Tools;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Tool Tool { get; set; } = default!;

    public SelectList CategoryList { get; set; } = default!;
    public SelectList CertificationList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        Tool = new Tool { LoanPeriodDays = 7 };
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

        // Validate that category exists
        if (Tool.CategoryId <= 0)
        {
            ModelState.AddModelError("Tool.CategoryId", "Please select a category.");
            await LoadSelectListsAsync();
            return Page();
        }

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == Tool.CategoryId);
        if (!categoryExists)
        {
            ModelState.AddModelError("Tool.CategoryId", "Selected category does not exist.");
            await LoadSelectListsAsync();
            return Page();
        }

        _context.Tools.Add(Tool);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        CertificationList = new SelectList(await _context.Certifications.ToListAsync(), "Id", "Name");
    }
}
