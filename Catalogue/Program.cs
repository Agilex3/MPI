using Catalogue.Components;
using Microsoft.EntityFrameworkCore;  // Adaug? acest using pentru DbContext
using Catalogue.Data;  // Add this using directive for MyDbContext
using Catalogue.Models;  // Add this using directive for User

// Modific? semn?tura metodei Main s? fie async
var builder = WebApplication.CreateBuilder(args);

//Configurare DbContext
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Testeaz? conexiunea la baza de date înainte de a porni aplica?ia
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    try
    {
        // Execut? o simpl? interogare pentru a verifica conexiunea
        var usersCount = await dbContext.Users.CountAsync();
        Console.WriteLine($"Conexiunea a fost realizat? cu succes! Num?rul de utilizatori: {usersCount}");

        // Adaug? un utilizator de test indiferent de num?rul de utilizatori
        var user = new User
        {
            first_name = "Test",
            last_name = "User",
            email = "test.user@example.com",
            password = "password123", // Într-o aplica?ie real?, ar trebui s? criptezi parola
            role = "student",
            created_at = DateTime.Now // Adaug? data cre?rii pentru a respecta structura entit??ii
        };

        // Adaug? utilizatorul în baza de date
        dbContext.Users.Add(user);
        try
        {
            // Salveaz? modific?rile în baza de date
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Utilizator ad?ugat cu succes!");
        }
        catch (Exception ex)
        {
            // Captur?m orice eroare care apare la salvarea modific?rilor
            Console.WriteLine($"A ap?rut o eroare la salvarea utilizatorului: {ex.Message}");
        }
    }
    catch (Exception ex)
    {
        // Afi?eaz? eroarea dac? nu se poate conecta
        Console.WriteLine($"Eroare la conectarea la baza de date: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
