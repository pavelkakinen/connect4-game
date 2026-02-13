using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Members;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Member Member { get; set; } = default!;
    public List<Loan> ActiveLoans { get; set; } = new();
    public List<Loan> RecentLoans { get; set; } = new();
    public List<MemberCertification> Certifications { get; set; } = new();
    public int TotalLoans { get; set; }
    public int LateReturnsThisYear { get; set; }
    public int DamageCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        Member = member;

        ActiveLoans = await _context.Loans
            .Include(l => l.Tool)
            .Where(l => l.MemberId == id && l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();

        RecentLoans = await _context.Loans
            .Include(l => l.Tool)
            .Where(l => l.MemberId == id && l.ReturnDate != null)
            .OrderByDescending(l => l.ReturnDate)
            .Take(10)
            .ToListAsync();

        Certifications = await _context.MemberCertifications
            .Include(mc => mc.Certification)
            .Where(mc => mc.MemberId == id)
            .ToListAsync();

        TotalLoans = await _context.Loans.CountAsync(l => l.MemberId == id);

        var oneYearAgo = DateTime.UtcNow.AddYears(-1);
        LateReturnsThisYear = await _context.Loans
            .Where(l => l.MemberId == id &&
                        l.ReturnDate != null &&
                        l.ReturnDate > l.DueDate &&
                        l.ReturnDate > oneYearAgo)
            .CountAsync();

        DamageCount = await _context.DamageReports
            .Include(d => d.Loan)
            .Where(d => d.Loan.MemberId == id)
            .CountAsync();

        return Page();
    }
}
