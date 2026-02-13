using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Certifications;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Certification Certification { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var certification = await _context.Certifications.FindAsync(id);
        if (certification == null)
        {
            return NotFound();
        }
        Certification = certification;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var certification = await _context.Certifications.FindAsync(id);
        if (certification != null)
        {
            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
