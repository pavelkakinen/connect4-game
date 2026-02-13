using DAL;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Category> Categories { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Categories = await _context.Categories
            .Include(c => c.Tools)
            .ToListAsync();
    }
}
