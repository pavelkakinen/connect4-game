using System.ComponentModel.DataAnnotations;

namespace ConsoleAppDbTest.Domain;

public class Book : BaseEntity
{
    // Id - bad idea beacause all staff have Id (a.id == b.id  VS a.BookId == b.PersonId )
    // BookId

    [MaxLength(255)]
    public string Title { get; set; } = default;

    // nullable FK - this relationship is optional
    public int? PersonId { get; set; }
    public Person? Person { get; set; }

    public ICollection<BookAuthor> BookAuthors { get; set; }

}