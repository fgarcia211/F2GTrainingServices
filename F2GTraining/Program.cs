using F2GTraining.Data;
using F2GTraining.Helpers;
using F2GTraining.Repositories;
using F2GTraining.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ServiceAPIF2GTraining>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
});

//BASE DE DATOS
string connectionString = builder.Configuration.GetConnectionString("databaseAzure");
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<HelperRutasProvider>();
builder.Services.AddSingleton<HelperSubirFicheros>();
builder.Services.AddTransient<IRepositoryF2GTraining, RepositoryF2GTraining>();
builder.Services.AddDbContext<F2GDataBaseContext>(options => options.UseSqlServer(connectionString));

//SEGURIDAD
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

//RUTAS DE VALIDACION PROPIAS
builder.Services.AddControllersWithViews
    (options => options.EnableEndpointRouting = false);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Usuarios}/{action=InicioSesion}"
    );
});

Rotativa.AspNetCore.RotativaConfiguration.Setup(builder.Environment.WebRootPath, "Rotativa");
app.Run();
