using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Loans;

public class HistoryModel : PageModel
{
    private readonly AppDbContext _context;

    public HistoryModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Loan> Loans { get; set; } = new();

    public async Task OnGetAsync()
    {
        Loans = await _context.Loans
            .Include(l => l.Tool)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate != null)
            .OrderByDescending(l => l.ReturnDate)
            .Take(100)
            .ToListAsync();
    }
}
