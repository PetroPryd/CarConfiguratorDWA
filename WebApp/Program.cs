using CarConfigurator.Business.MappingProfiles;
using CarConfigurator.Business.Security;
using CarConfigurator.Business.Services;
using CarConfigurator.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Session holds the user's in-progress "build" (selected component ids).
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<CarConfiguratorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnStr")));

// AutoMapper — scans BOTH the Business assembly (DTO maps) and this WebApp
// assembly (ViewModel maps we add in the CRUD steps).
builder.Services.AddAutoMapper(cfg => { },
    typeof(CarComponentProfile).Assembly,   // Business
    typeof(Program).Assembly);              // WebApp

// JWT settings still needed because AuthService depends on TokenProvider,
// even though the website itself uses cookie auth (it never issues a token).
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<TokenProvider>();

// Business services (shared with the API).
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CarComponentService>();
builder.Services.AddScoped<ComponentTypeService>();
builder.Services.AddScoped<CarModelService>();
builder.Services.AddScoped<UserConfigurationService>();
builder.Services.AddScoped<UserService>();

// Cookie authentication for the website.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

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

var culture = new CultureInfo("hr-HR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
