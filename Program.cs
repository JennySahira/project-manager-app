using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Kopplar in databascontexten och läser anslutningssträng från appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Aktiverar ASP.NET Core Identity och kopplar till databasen via EF Core
// SignIn.RequireConfirmedAccount = false tillåter inloggning utan e-. Skapad med hjälp av Chat gpt.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Registrerar MVC-komponenter (controllers + views)
builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    
    app.UseExceptionHandler("/Home/Error");

   
    app.UseHsts();
}


app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting(); 

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapRazorPages();

// 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Projects}/{action=Index}/{id?}");

app.Run();
