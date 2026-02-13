using DAL;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Tools;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Tool Tool { get; set; } = default!;
    public Loan? CurrentLoan { get; set; }
    public List<Loan> RecentLoans { get; set; } = new();
    public int WaitingCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var tool = await _context.Tools
            .Include(t => t.Category)
            .Include(t => t.RequiredCertification)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tool == null)
        {
            return NotFound();
        }

        Tool = tool;

        CurrentLoan = await _context.Loans
            .Include(l => l.Member)
            .Where(l => l.ToolId == id && l.ReturnDate == null)
            .FirstOrDefaultAsync();

        RecentLoans = await _context.Loans
            .Include(l => l.Member)
            .Where(l => l.ToolId == id)
            .OrderByDescending(l => l.CheckoutDate)
            .Take(10)
            .ToListAsync();

        WaitingCount = await _context.Reservations
            .Where(r => r.ToolId == id && r.Status == ReservationStatus.Pending)
            .CountAsync();

        return Page();
    }
}
