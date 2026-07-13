using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WinMovers.Data;
using WinMovers.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WinMoversContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicio de envío de correo (versión de desarrollo: imprime en consola).
builder.Services.AddTransient<WinMovers.Services.IEmailSender, WinMovers.Services.EmailSenderConsola>();

// =========================================================
// ASP.NET Identity
// =========================================================
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // --- Política de contraseñas ---
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;

    // --- Bloqueo por intentos fallidos (HU-AUT-001 Escenario 3) ---
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // --- Correo único obligatorio ---
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<WinMoversContext>()
    .AddDefaultTokenProviders();

// Cookie de autenticación: rutas de login/acceso denegado y expiración de sesión.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// IMPORTANTE: UseAuthentication() SIEMPRE antes de UseAuthorization().
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}");

// =========================================================
// SEED: crea el usuario Administrador inicial si no existe ninguno.
// Esto solo corre una vez; en corridas futuras encuentra que ya
// existe y no hace nada.
// =========================================================
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var admin = await userManager.FindByEmailAsync("admin@winmovers.com");
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = "admin@winmovers.com",
            Email = "admin@winmovers.com",
            NombreCompleto = "Administrador WinMovers",
            EmailConfirmed = true,
            Activo = true
        };

        var resultado = await userManager.CreateAsync(admin, "Admin123!");
        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, Roles.Administrador);
        }
    }
}

app.Run();