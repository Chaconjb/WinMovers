using Microsoft.EntityFrameworkCore;
using AvanceWinMovers.Data;
using AvanceWinMovers.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar conexión a SQL Server
var connectionString = Environment.GetEnvironmentVariable("WINMOVERS_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("WinMoversDb");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No se encontró una cadena de conexión. Define WINMOVERS_DB_CONNECTION o ConnectionStrings:WinMoversDb.");
}

builder.Services.AddDbContext<WinMoversContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar servicios
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IModulosService, ModulosService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
