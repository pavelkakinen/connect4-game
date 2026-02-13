using DAL;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Loans;

public class ReturnModel : PageModel
{
    private readonly AppDbContext _context;

    public ReturnModel(AppDbContext context)
    {
        _context = context;
    }

    public Loan Loan { get; set; } = default!;

    [BindProperty]
    public LoanCondition ReturnCondition { get; set; } = LoanCondition.Good;

    [BindProperty]
    public decimal? UsageHours { get; set; }

    [BindProperty]
    public string? Notes { get; set; }

    [BindProperty]
    public string? DamageDescription { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Tool)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null || loan.ReturnDate != null)
        {
            return NotFound();
        }

        Loan = loan;
        Notes = loan.Notes;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Tool)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null || loan.ReturnDate != null)
        {
            return NotFound();
        }

        Loan = loan;

        loan.ReturnDate = DateTime.UtcNow;
        loan.ReturnCondition = ReturnCondition;
        loan.UsageHours = UsageHours;
        loan.Notes = Notes;

        // Update tool usage hours
        if (UsageHours.HasValue)
        {
            loan.Tool.TotalUsageHours += UsageHours.Value;
        }

        // Set tool status
        if (ReturnCondition == LoanCondition.Damaged || ReturnCondition == LoanCondition.Lost)
        {
            loan.Tool.Status = ToolStatus.OutOfService;
            loan.Tool.OutOfServiceReason = ReturnCondition == LoanCondition.Lost
                ? "Lost"
                : "Returned damaged - needs repair";

            if (!string.IsNullOrWhiteSpace(DamageDescription))
            {
                var damageReport = new DamageReport
                {
                    LoanId = loan.Id,
                    Description = DamageDescription,
                    ReportDate = DateTime.UtcNow
                };
                _context.DamageReports.Add(damageReport);
            }
        }
        else
        {
            // Check for pending reservations
            var nextReservation = await _context.Reservations
                .Where(r => r.ToolId == loan.ToolId && r.Status == ReservationStatus.Pending)
                .OrderBy(r => r.QueuePosition)
                .FirstOrDefaultAsync();

            if (nextReservation != null)
            {
                loan.Tool.Status = ToolStatus.Reserved;
                nextReservation.Status = ReservationStatus.Ready;
                nextReservation.NotifiedDate = DateTime.UtcNow;
                nextReservation.ExpirationDate = DateTime.UtcNow.AddDays(3);
            }
            else
            {
                loan.Tool.Status = ToolStatus.Available;
            }
        }

        // Check if member should be suspended (3 late returns in a year)
        if (loan.WasLate)
        {
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);
            var lateCount = await _context.Loans
                .Where(l => l.MemberId == loan.MemberId &&
                            l.ReturnDate != null &&
                            l.ReturnDate > l.DueDate &&
                            l.ReturnDate > oneYearAgo)
                .CountAsync();

            if (lateCount >= 3)
            {
                loan.Member.Status = MemberStatus.Suspended;
                loan.Member.SuspensionEndDate = DateTime.UtcNow.AddMonths(3);
                loan.Member.SuspensionReason = $"3 or more late returns in the past year ({lateCount} total)";
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
