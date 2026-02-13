using DAL;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public int TotalTools { get; set; }
    public int AvailableTools { get; set; }
    public int ActiveMembers { get; set; }
    public int OnLoan { get; set; }
    public int OverdueCount { get; set; }

    public List<Loan> OverdueLoans { get; set; } = new();
    public List<Tool> MaintenanceDue { get; set; } = new();
    public List<Tool> OutOfService { get; set; } = new();
    public List<Loan> RecentCheckouts { get; set; } = new();
    public List<Loan> RecentReturns { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalTools = await _context.Tools.CountAsync();
        AvailableTools = await _context.Tools.CountAsync(t => t.Status == ToolStatus.Available);
        ActiveMembers = await _context.Members.CountAsync(m => m.Status == MemberStatus.Active);
        OnLoan = await _context.Tools.CountAsync(t => t.Status == ToolStatus.OnLoan);

        OverdueLoans = await _context.Loans
            .Include(l => l.Tool)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null && l.DueDate < DateTime.UtcNow)
            .OrderBy(l => l.DueDate)
            .Take(10)
            .ToListAsync();

        OverdueCount = OverdueLoans.Count;

        var allTools = await _context.Tools.ToListAsync();
        MaintenanceDue = allTools.Where(t => t.IsMaintenanceDue && t.Status != ToolStatus.OutOfService).Take(5).ToList();
        OutOfService = allTools.Where(t => t.Status == ToolStatus.OutOfService).ToList();

        RecentCheckouts = await _context.Loans
            .Include(l => l.Tool)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)
            .OrderByDescending(l => l.CheckoutDate)
            .Take(5)
            .ToListAsync();

        RecentReturns = await _context.Loans
            .Include(l => l.Tool)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate != null)
            .OrderByDescending(l => l.ReturnDate)
            .Take(5)
            .ToListAsync();
    }
}
