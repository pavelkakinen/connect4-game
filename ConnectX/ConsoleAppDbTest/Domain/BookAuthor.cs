namespace ConsoleAppDbTest.Domain;

public class BookAuthor : BaseEntity
{
    // FK is mendatory = so this is mendatory relationship 
    public int BookId { get; set; }
    
    // Entity is nullable because maybe we did not do sql join
    public Book? Book { get; set; }
    
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}