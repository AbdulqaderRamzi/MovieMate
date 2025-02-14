using Microsoft.AspNetCore.Authentication.Cookies;
using MovieMate.Web.Services;
using MovieMate.Web.Services.IServices;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient("ApiClient", client => 
    {
        client.BaseAddress = new Uri("http://localhost:5199/");
    });
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IBaseApiClient, BaseApiClient>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IGenreService, GenreService>();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(opt =>
        {
            opt.Cookie.HttpOnly = true;
            opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            opt.SlidingExpiration = true;
            opt.LoginPath = "/Auth/Login";
            opt.AccessDeniedPath = "/Auth/AccessDenied";
            opt.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforce HTTPS
            opt.Cookie.SameSite = SameSiteMode.Strict; // Prevent CSRF
        });
}

var app = builder.Build();
{
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
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.Run();    
}

