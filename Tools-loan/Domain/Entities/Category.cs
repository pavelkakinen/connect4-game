namespace Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<Tool> Tools { get; set; } = new List<Tool>();
}
