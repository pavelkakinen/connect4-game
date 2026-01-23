using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Admin.Classes;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DanceClass? DanceClass { get; set; }

    public int BookingCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        DanceClass = await _context.DanceClasses
            .Include(c => c.DanceStyle)
            .Include(c => c.Studio)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (DanceClass != null)
        {
            BookingCount = await _context.Bookings.CountAsync(b => b.DanceClassId == id);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var danceClass = await _context.DanceClasses.FindAsync(DanceClass?.Id);

        if (danceClass != null)
        {
            _context.DanceClasses.Remove(danceClass);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Class deleted successfully!";
        }

        return RedirectToPage("Index");
    }
}
