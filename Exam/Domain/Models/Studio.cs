using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Studio
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int SizeSquareMeters { get; set; }

    public bool HasPoles { get; set; }

    public bool HasAerialRigging { get; set; }

    public int MaxCapacity { get; set; }

    public ICollection<DanceClass> DanceClasses { get; set; } = new List<DanceClass>();
}