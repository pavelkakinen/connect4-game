namespace Domain.Entities;

public class DamageReport
{
    public int Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal? RepairCost { get; set; }
    public bool IsResolved { get; set; }
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedDate { get; set; }

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = default!;
}
