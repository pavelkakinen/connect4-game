using Domain.Enums;

namespace Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public int QueuePosition { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public DateTime? NotifiedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public int ToolId { get; set; }
    public Tool Tool { get; set; } = default!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = default!;
}
