using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using WinMovers.Data;
using WinMovers.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WinMoversContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicio de env�o de correo (versi�n de desarrollo: imprime en consola).
builder.Services.AddTransient<WinMovers.Services.IEmailSender, WinMovers.Services.EmailSenderConsola>();

// Servicios del m�dulo de Cotizaciones.
builder.Services.AddScoped<WinMovers.Services.IQuoteService, WinMovers.Services.QuoteService>();
builder.Services.AddScoped<WinMovers.Services.ICotizacionEmailService, WinMovers.Services.CotizacionEmailService>();
builder.Services.AddScoped<WinMovers.Services.IViewRenderService, WinMovers.Services.ViewRenderService>();

// =========================================================
// ASP.NET Identity
// =========================================================
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // --- Pol�tica de contrase�as ---
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;

    // --- Bloqueo por intentos fallidos (HU-AUT-001 Escenario 3) ---
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // --- Correo �nico obligatorio ---
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<WinMoversContext>()
    .AddDefaultTokenProviders();

// =========================================================
// POL�TICA DE ACCESO GLOBAL
// Toda la aplicaci�n exige sesi�n iniciada, incluidos los controladores
// que se agreguen m�s adelante. Las �nicas excepciones son las pantallas
// de login y recuperaci�n de contrase�a, marcadas con [AllowAnonymous].
// =========================================================
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Cookie de autenticaci�n: rutas de login/acceso denegado y expiraci�n de sesi�n.
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
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Los roles base deben existir antes de asignarlos: AddToRoleAsync lanza
    // excepci�n si el rol no est�, y eso tumbar�a el arranque.
    foreach (var (nombre, descripcion) in new[]
             {
                 (Roles.Administrador, "Acceso total al sistema."),
                 (Roles.Empleado, "Asesor: registra clientes, cotizaciones y �rdenes."),
                 (Roles.SinRol, "Usuario sin permisos asignados.")
             })
    {
        if (!await roleManager.RoleExistsAsync(nombre))
        {
            await roleManager.CreateAsync(new ApplicationRole(nombre) { Descripcion = descripcion });
        }
    }

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