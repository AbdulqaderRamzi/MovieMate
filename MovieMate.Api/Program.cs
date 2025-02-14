using MovieMate.Api;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddApplicationServices(builder.Configuration);
}

var app = builder.Build();
{
    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    /*app
        .MapGroup("/auth")
        .MapIdentityApi<ApplicationUser>();*/
    app.Run();
}

