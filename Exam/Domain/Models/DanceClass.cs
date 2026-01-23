using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models;

public class DanceClass
{
    public int Id { get; set; }

    public int DanceStyleId { get; set; }
    public DanceStyle DanceStyle { get; set; } = default!;

    public int StudioId { get; set; }
    public Studio Studio { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string InstructorName { get; set; } = default!;

    public ClassLevel Level { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int MaxStudents { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}