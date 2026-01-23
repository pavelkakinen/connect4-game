namespace Domain.Models;

public class Booking
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;

    public int DanceClassId { get; set; }
    public DanceClass DanceClass { get; set; } = default!;

    // Null for trial bookings
    public int? PackageId { get; set; }
    public Package? Package { get; set; }

    public DateTime BookingDate { get; set; }

    public bool IsTrial { get; set; }

    // For friend trial invitations - who invited this person
    public int? InvitedByStudentId { get; set; }
    public Student? InvitedByStudent { get; set; }

    // null = pending, true = attended, false = no-show
    public bool? Attended { get; set; }

    public DateTime CreatedAt { get; set; }
}