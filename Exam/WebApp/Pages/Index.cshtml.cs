using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<DanceStyle> DanceStyles { get; set; } = new();
    public List<Studio> Studios { get; set; } = new();

    public async Task OnGetAsync()
    {
        DanceStyles = await _context.DanceStyles.OrderBy(s => s.Name).ToListAsync();
        Studios = await _context.Studios.OrderBy(s => s.Name).ToListAsync();
    }
}