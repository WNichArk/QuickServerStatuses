using Microsoft.AspNetCore.Mvc;
using System.Text;
using StatusWebApp;

var builder = WebApplication.CreateBuilder(args);

// Load ApiAuth settings
builder.Services.Configure<ApiAuthOptions>(builder.Configuration.GetSection("ApiAuth"));
builder.Services.AddSingleton<StatusService>();
builder.Services.AddRazorPages();

var app = builder.Build();

var config = app.Services.GetRequiredService<IConfiguration>();
var authOptions = config.GetSection("ApiAuth").Get<ApiAuthOptions>()!;

// Basic Auth Middleware
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader is null || !authHeader.StartsWith("Basic "))
        {
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        try
        {
            var encoded = authHeader["Basic ".Length..].Trim();
            var credentialBytes = Convert.FromBase64String(encoded);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

            if (credentials.Length != 2 || credentials[0] != authOptions.Username || credentials[1] != authOptions.Password)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden");
                return;
            }
        }
        catch
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Authorization Header");
            return;
        }
    }

    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// API Endpoints
app.MapGet("/api/status", (StatusService service) =>
{
    return Results.Json(service.GetAll());
});

app.MapPost("/api/status", async (HttpContext context, StatusService service) =>
{
    var data = await context.Request.ReadFromJsonAsync<Dictionary<string, string>>();
    if (data is null)
        return Results.BadRequest("Invalid JSON");

    foreach (var kvp in data)
        service.Set(kvp.Key, kvp.Value);

    return Results.Ok("Status updated.");
});

app.Run();
