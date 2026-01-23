using Microsoft.EntityFrameworkCore;
using DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure SQLite database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

// API endpoint for getting payment options for a student/class combination
app.MapGet("/api/student-payments", async (int studentId, int classId, int styleId, ApplicationDbContext db) =>
{
    var options = new List<object>();

    // Check trial eligibility
    var hasUsedTrial = await db.TrialUsages
        .AnyAsync(t => t.StudentId == studentId && t.DanceStyleId == styleId);

    if (!hasUsedTrial)
    {
        options.Add(new { value = "trial", name = "Free Trial" });
    }

    // Get valid packages
    var packages = await db.Packages
        .Include(p => p.DanceStyle)
        .Where(p => p.StudentId == studentId)
        .ToListAsync();

    foreach (var pkg in packages.Where(p => p.IsValid))
    {
        // Skip single-style packages for different styles
        if (pkg.PackageType == Domain.Enums.PackageType.MonthlyUnlimitedSingle &&
            pkg.DanceStyleId != styleId)
        {
            continue;
        }

        var name = pkg.PackageType switch
        {
            Domain.Enums.PackageType.TenClassCard => $"10-Class Card ({pkg.RemainingClasses} left)",
            Domain.Enums.PackageType.MonthlyUnlimitedSingle => $"Monthly ({pkg.DanceStyle?.Name})",
            Domain.Enums.PackageType.MonthlyUnlimitedAll => "Monthly (All Styles)",
            _ => pkg.PackageType.ToString()
        };

        options.Add(new { value = $"package_{pkg.Id}", name });
    }

    return Results.Ok(options);
});

app.Run();