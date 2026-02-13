using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Certification
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    [Range(0, 1000, ErrorMessage = "Training hours must be between 0 and 1000")]
    public decimal TrainingHoursRequired { get; set; }

    public ICollection<Tool> Tools { get; set; } = new List<Tool>();
    public ICollection<MemberCertification> MemberCertifications { get; set; } = new List<MemberCertification>();
}
