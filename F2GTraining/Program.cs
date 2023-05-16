using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using F2GTraining.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret keyVaultSecret = await secretClient.GetSecretAsync("StorageAzure");

string azureKeys = keyVaultSecret.Value;

BlobServiceClient blobServiceClient = new BlobServiceClient(azureKeys);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);
builder.Services.AddTransient<ServiceStorageBlobs>();

builder.Services.AddTransient<ServiceAPIF2GTraining>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
});

//BASE DE DATOS
builder.Services.AddHttpContextAccessor();

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
