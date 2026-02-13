using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Certifications;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
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

    public async Task<IActionResult> OnPostAsync()
    {
        // Remove validation for navigation properties
        ModelState.Remove("Certification.Tools");
        ModelState.Remove("Certification.MemberCertifications");

        // Server-side validation for training hours
        if (Certification.TrainingHoursRequired < 0)
        {
            ModelState.AddModelError("Certification.TrainingHoursRequired", "Training hours cannot be negative.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Certification).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Certifications.AnyAsync(e => e.Id == Certification.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("./Index");
    }
}
