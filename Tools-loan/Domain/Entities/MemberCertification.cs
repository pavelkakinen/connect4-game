namespace Domain.Entities;

public class MemberCertification
{
    public int Id { get; set; }
    public DateTime EarnedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = default!;

    public int CertificationId { get; set; }
    public Certification Certification { get; set; } = default!;

    public bool IsValid => !ExpirationDate.HasValue || ExpirationDate.Value > DateTime.UtcNow;
}
