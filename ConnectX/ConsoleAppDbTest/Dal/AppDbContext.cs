using Microsoft.EntityFrameworkCore;

namespace ConsoleAppDbTest.Dal;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
}