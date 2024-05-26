using GeneratorClient.Models;
using GeneratorClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

var x = builder.Configuration.GetSection("GenerationSettings");

builder.Services.Configure<GenerationSettings>(builder.Configuration.GetSection("GenerationSettings"));

builder.Services.AddSingleton<GeneratorUplink>();
builder.Services.AddHttpClient();

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
