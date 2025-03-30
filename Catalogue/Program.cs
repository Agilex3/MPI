using Catalogue.Components;
using Catalogue.Data;
using Catalogue.Models;
using Catalogue.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// 1. CONFIGURARE SERVICII
// ------------------------------

// 1.1 DbContext pentru SQL Server
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1.2 Autentificare cu cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// 1.3 Servicii personalizate
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CourseService>();
//builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<GradeService>();

// 1.4 Suport pentru API + MVC + Razor Components
builder.Services.AddControllers();               // pentru [ApiController]
builder.Services.AddControllersWithViews();     // pentru MVC clasic
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// ------------------------------
// 2. TEST CONEXIUNE LA BAZĂ DE DATE
// ------------------------------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

    try
    {
        var usersCount = await dbContext.Users.CountAsync();
        Console.WriteLine($"✅ Conexiune DB reușită! Utilizatori existenți: {usersCount}");

        // Dacă vrei să adaugi un utilizator de test:
        /*
        var user = new User
        {
            first_name = "Test",
            last_name = "User",
            email = "test@example.com",
            password = "1234",
            role = "student",
            created_at = DateTime.Now
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        */
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Eroare conectare DB: {ex.Message}");
    }
}

// ------------------------------
// 3. PIPELINE HTTP
// ------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts(); // securitate pentru HTTPS
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// Mapare controller-e API (cele cu [ApiController])
app.MapControllers();

// Mapare controller-e MVC (ex: UserController cu views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor components (Blazor)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Rulăm aplicația
await app.RunAsync();
