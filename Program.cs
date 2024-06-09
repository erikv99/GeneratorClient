using GeneratorClient.Models;
using GeneratorClient.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

_configureServices(builder);

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

void _configureServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllersWithViews();

    builder.Services.Configure<GenerationSettings>(builder.Configuration.GetSection("GenerationSettings"));

    var dataSourcePath = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MainDbContext.db");

    builder.Services.AddDbContext<MainDbContext>(options =>
        options.UseSqlite($"Data Source={dataSourcePath}"),
        ServiceLifetime.Scoped);

    builder.Services.AddScoped<GeneratorUplink>();

    builder.Services.AddHttpClient();
}