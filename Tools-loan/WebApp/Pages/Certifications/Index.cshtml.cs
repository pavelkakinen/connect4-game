using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Certifications;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Certification> Certifications { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Certifications = await _context.Certifications
            .Include(c => c.Tools)
            .Include(c => c.MemberCertifications)
            .ToListAsync();
    }
}
