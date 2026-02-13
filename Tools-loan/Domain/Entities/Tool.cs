using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class Tool
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    public DateTime? PurchaseDate { get; set; }

    [Range(0, 1000000, ErrorMessage = "Replacement cost must be 0 or greater")]
    public decimal? ReplacementCost { get; set; }

    [Range(1, 365, ErrorMessage = "Loan period must be between 1 and 365 days")]
    public int LoanPeriodDays { get; set; } = 7;

    public ToolStatus Status { get; set; } = ToolStatus.Available;

    // Maintenance tracking
    [Range(0, 10000, ErrorMessage = "Maintenance interval must be 0 or greater")]
    public decimal? MaintenanceIntervalHours { get; set; }

    [Range(0, 1000000, ErrorMessage = "Total usage hours must be 0 or greater")]
    public decimal TotalUsageHours { get; set; }

    [Range(0, 1000000, ErrorMessage = "Hours at last service must be 0 or greater")]
    public decimal? HoursAtLastService { get; set; }

    // Out of service info
    public string? OutOfServiceReason { get; set; }
    public DateTime? ExpectedReturnToService { get; set; }

    // Relationships
    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public int? RequiredCertificationId { get; set; }
    public Certification? RequiredCertification { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    // Computed properties
    public bool IsMaintenanceDue => MaintenanceIntervalHours.HasValue &&
        TotalUsageHours - (HoursAtLastService ?? 0) >= MaintenanceIntervalHours.Value;
}
