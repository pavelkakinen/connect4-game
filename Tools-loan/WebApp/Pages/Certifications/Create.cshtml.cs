using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Certifications;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Certification Certification { get; set; } = default!;

    public IActionResult OnGet()
    {
        Certification = new Certification();
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

        _context.Certifications.Add(Certification);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
