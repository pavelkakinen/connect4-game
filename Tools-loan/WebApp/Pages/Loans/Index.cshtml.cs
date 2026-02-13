using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Loans;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Loan> ActiveLoans { get; set; } = new();
    public List<Loan> OverdueLoans { get; set; } = new();

    public async Task OnGetAsync()
    {
        var allActive = await _context.Loans
            .Include(l => l.Tool)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();

        OverdueLoans = allActive.Where(l => l.IsOverdue).ToList();
        ActiveLoans = allActive.Where(l => !l.IsOverdue).ToList();
    }
}
