using DAL;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Tools;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Tool> Tools { get; set; } = default!;
    public SelectList CategoryList { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public ToolStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchFilter { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Tools
            .Include(t => t.Category)
            .Include(t => t.RequiredCertification)
            .AsQueryable();

        if (CategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == CategoryId.Value);
        }

        if (StatusFilter.HasValue)
        {
            query = query.Where(t => t.Status == StatusFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(SearchFilter))
        {
            query = query.Where(t => t.Name.Contains(SearchFilter) ||
                (t.Description != null && t.Description.Contains(SearchFilter)) ||
                (t.SerialNumber != null && t.SerialNumber.Contains(SearchFilter)));
        }

        Tools = await query.OrderBy(t => t.Name).ToListAsync();
        CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
    }
}
