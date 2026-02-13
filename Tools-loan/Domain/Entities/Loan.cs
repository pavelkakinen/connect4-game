using Domain.Enums;

namespace Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanCondition? ReturnCondition { get; set; }
    public decimal? UsageHours { get; set; }
    public string? Notes { get; set; }

    public int ToolId { get; set; }
    public Tool Tool { get; set; } = default!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = default!;

    public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();

    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !IsReturned && DueDate < DateTime.UtcNow;
    public bool WasLate => IsReturned && ReturnDate!.Value > DueDate;
}
