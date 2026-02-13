using Domain.Enums;

namespace Domain.Entities;

public class MaintenanceRecord
{
    public int Id { get; set; }
    public DateTime ServiceDate { get; set; }
    public MaintenanceType Type { get; set; }
    public string Description { get; set; } = default!;
    public decimal? HoursAtService { get; set; }
    public decimal? Cost { get; set; }
    public string? PerformedBy { get; set; }

    public int ToolId { get; set; }
    public Tool Tool { get; set; } = default!;
}
