namespace Domain.Models;

public class TrialUsage
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;

    public int DanceStyleId { get; set; }
    public DanceStyle DanceStyle { get; set; } = default!;

    public DateTime TrialDate { get; set; }
}