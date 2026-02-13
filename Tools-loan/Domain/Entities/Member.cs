using Domain.Enums;

namespace Domain.Entities;

public class Member
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public MemberStatus Status { get; set; } = MemberStatus.Active;
    public DateTime? SuspensionEndDate { get; set; }
    public string? SuspensionReason { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<MemberCertification> MemberCertifications { get; set; } = new List<MemberCertification>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public string FullName => $"{FirstName} {LastName}";

    public bool CanBorrow => Status == MemberStatus.Active &&
        (!SuspensionEndDate.HasValue || SuspensionEndDate.Value < DateTime.UtcNow);
}
