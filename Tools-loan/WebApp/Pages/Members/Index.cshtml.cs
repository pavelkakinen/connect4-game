using DAL;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Members;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Member> Members { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public MemberStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchFilter { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Members
            .Include(m => m.Loans)
            .AsQueryable();

        if (StatusFilter.HasValue)
        {
            query = query.Where(m => m.Status == StatusFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(SearchFilter))
        {
            query = query.Where(m =>
                m.FirstName.Contains(SearchFilter) ||
                m.LastName.Contains(SearchFilter) ||
                m.Email.Contains(SearchFilter));
        }

        Members = await query.OrderBy(m => m.LastName).ThenBy(m => m.FirstName).ToListAsync();
    }
}
