using System.ComponentModel.DataAnnotations;

namespace ConsoleAppDbTest.Domain;

public class Person : BaseEntity
{
    [MaxLength(128)]
    public string FirstName { get; set; } = default!;
    
    [MaxLength(128)]
    public string LastName { get; set; } = default!;

    // is's null when there was no query 
    public ICollection<Book>? Books { get; set; }
    
}