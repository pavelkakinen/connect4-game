using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Student
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Package> Packages { get; set; } = new List<Package>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<TrialUsage> TrialUsages { get; set; } = new List<TrialUsage>();
    public ICollection<Booking> InvitedBookings { get; set; } = new List<Booking>();
}