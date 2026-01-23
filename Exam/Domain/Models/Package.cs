using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models;

public class Package
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;

    public PackageType PackageType { get; set; }

    // Only for MonthlyUnlimitedSingle - specifies which style
    public int? DanceStyleId { get; set; }
    public DanceStyle? DanceStyle { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    // Only for TenClassCard
    public int? RemainingClasses { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    // Computed properties for validation
    public bool IsExpired => ExpiryDate.Date < DateTime.Today;
    public bool IsValid => !IsExpired && (PackageType != PackageType.TenClassCard || RemainingClasses > 0);
    public bool HasLowBalance => PackageType == PackageType.TenClassCard && RemainingClasses <= 2;
}