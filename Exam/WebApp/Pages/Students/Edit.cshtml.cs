using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Students;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Student Student { get; set; } = default!;

    public class InputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        Student = student;
        Input = new InputModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Phone = student.Phone
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var student = await _context.Students.FindAsync(Input.Id);
            if (student != null) Student = student;
            return Page();
        }

        var studentToUpdate = await _context.Students.FindAsync(Input.Id);
        if (studentToUpdate == null)
        {
            return NotFound();
        }

        // Check if email is being changed to one that already exists
        if (studentToUpdate.Email != Input.Email)
        {
            if (await _context.Students.AnyAsync(s => s.Email == Input.Email && s.Id != Input.Id))
            {
                ModelState.AddModelError("Input.Email", "This email is already registered to another student.");
                Student = studentToUpdate;
                return Page();
            }
        }

        studentToUpdate.FirstName = Input.FirstName;
        studentToUpdate.LastName = Input.LastName;
        studentToUpdate.Email = Input.Email;
        studentToUpdate.Phone = Input.Phone;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Student updated successfully!";
        return RedirectToPage("Details", new { id = Input.Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        // Check if student has any bookings
        var hasBookings = await _context.Bookings.AnyAsync(b => b.StudentId == id);
        if (hasBookings)
        {
            TempData["Error"] = "Cannot delete student with existing bookings. Please delete their bookings first.";
            return RedirectToPage("Edit", new { id });
        }

        // Delete related data
        var packages = _context.Packages.Where(p => p.StudentId == id);
        _context.Packages.RemoveRange(packages);

        var trialUsages = _context.TrialUsages.Where(t => t.StudentId == id);
        _context.TrialUsages.RemoveRange(trialUsages);

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Student {student.FullName} deleted successfully.";
        return RedirectToPage("Index");
    }
}
