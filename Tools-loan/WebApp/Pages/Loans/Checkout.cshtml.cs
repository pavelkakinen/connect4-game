using DAL;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Loans;

public class CheckoutModel : PageModel
{
    private readonly AppDbContext _context;

    public CheckoutModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int MemberId { get; set; }

    [BindProperty]
    public int ToolId { get; set; }

    [BindProperty]
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);

    [BindProperty]
    public string? Notes { get; set; }

    public SelectList MemberList { get; set; } = default!;
    public SelectList ToolList { get; set; } = default!;
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? memberId, int? toolId)
    {
        await LoadSelectListsAsync();

        if (memberId.HasValue) MemberId = memberId.Value;
        if (toolId.HasValue)
        {
            ToolId = toolId.Value;
            var tool = await _context.Tools.FindAsync(toolId.Value);
            if (tool != null)
            {
                DueDate = DateTime.UtcNow.AddDays(tool.LoanPeriodDays);
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        var member = await _context.Members
            .Include(m => m.Loans)
            .Include(m => m.MemberCertifications)
            .FirstOrDefaultAsync(m => m.Id == MemberId);

        var tool = await _context.Tools
            .Include(t => t.RequiredCertification)
            .FirstOrDefaultAsync(t => t.Id == ToolId);

        if (member == null || tool == null)
        {
            ErrorMessage = "Invalid member or tool selection.";
            await LoadSelectListsAsync();
            return Page();
        }

        // Check member can borrow
        if (!member.CanBorrow)
        {
            ErrorMessage = "This member cannot borrow. They may be suspended or expired.";
            await LoadSelectListsAsync();
            return Page();
        }

        // Check for 3 late returns
        var oneYearAgo = DateTime.UtcNow.AddYears(-1);
        var lateCount = member.Loans.Count(l => l.WasLate && l.ReturnDate > oneYearAgo);
        if (lateCount >= 3)
        {
            ErrorMessage = $"This member has {lateCount} late returns in the past year and should be suspended.";
            await LoadSelectListsAsync();
            return Page();
        }

        // Check tool availability
        if (tool.Status != ToolStatus.Available)
        {
            ErrorMessage = $"This tool is not available. Current status: {tool.Status}";
            await LoadSelectListsAsync();
            return Page();
        }

        // Check certification requirement
        if (tool.RequiredCertificationId.HasValue)
        {
            var hasCert = member.MemberCertifications
                .Any(mc => mc.CertificationId == tool.RequiredCertificationId && mc.IsValid);
            if (!hasCert)
            {
                ErrorMessage = $"This tool requires {tool.RequiredCertification!.Name} certification.";
                await LoadSelectListsAsync();
                return Page();
            }
        }

        // Create loan
        var loan = new Loan
        {
            MemberId = MemberId,
            ToolId = ToolId,
            CheckoutDate = DateTime.UtcNow,
            DueDate = DueDate,
            Notes = Notes
        };

        tool.Status = ToolStatus.OnLoan;

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        var activeMembers = await _context.Members
            .Where(m => m.Status == MemberStatus.Active)
            .Select(m => new { m.Id, Name = m.FirstName + " " + m.LastName })
            .ToListAsync();

        var availableTools = await _context.Tools
            .Where(t => t.Status == ToolStatus.Available)
            .Select(t => new { t.Id, t.Name })
            .ToListAsync();

        MemberList = new SelectList(activeMembers, "Id", "Name");
        ToolList = new SelectList(availableTools, "Id", "Name");
    }
}
