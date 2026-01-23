using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class DanceStyle
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    public ICollection<DanceClass> DanceClasses { get; set; } = new List<DanceClass>();
    public ICollection<Package> Packages { get; set; } = new List<Package>();
    public ICollection<TrialUsage> TrialUsages { get; set; } = new List<TrialUsage>();
}