global using Microsoft.AspNetCore.Components.Server.Circuits;
global using System.Collections.Concurrent;
global using Microsoft.AspNetCore.Components;

using DxAimArb.Areas.Identity;
using DxAimArb.Data;
using DxAimArb.Helpers;
using DxAimArb.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static DxAimArb.Services.Core;
using DxAimArb;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

//var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
//builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
IServiceCollection services = builder.Services;

var connectionString = builder.Configuration.GetConnectionString("DevLJ");
//if (Environment.MachineName == "LJACOBS")
//   connectionString = builder.Configuration.GetConnectionString("DevLJ");

Core core = new Core();

services.AddDbContext<DxAimArbContext>(options =>
     options.UseSqlServer(connectionString));

services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DxAimArbContext>()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

// Add services to the container.

services.AddRazorPages();
services.AddServerSideBlazor();
services.AddDevExpressBlazor();
services.AddSingleton<WeatherForecastService>();
services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
services.AddScoped<TokenProvider>();
services.AddScoped<Core>();
services.AddScoped<CircuitHandler, CircuitHandlerService>();



//LJ - XPO
services.AddXpoDefaultUnitOfWork(true, options =>
              options.UseConnectionString(connectionString)
              .UseConnectionPool(false)
              .UseThreadSafeDataLayer(false)
              .UseEntityTypes(typeof(Customer), typeof(Order), typeof(Profile)));

builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error");
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseXpoData(connectionString);

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.MapControllers();

app.Run();