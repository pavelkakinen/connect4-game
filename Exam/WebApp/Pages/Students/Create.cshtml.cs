using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Models;

namespace WebApp.Pages.Students;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check if email already exists
        if (await _context.Students.AnyAsync(s => s.Email == Input.Email))
        {
            ModelState.AddModelError("Input.Email", "This email is already registered.");
            return Page();
        }

        var student = new Student
        {
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Email = Input.Email,
            Phone = Input.Phone,
            RegistrationDate = DateTime.UtcNow
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Student {student.FullName} created successfully!";
        return RedirectToPage("Details", new { id = student.Id });
    }
}
