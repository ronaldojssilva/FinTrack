using System.Text;
using FinTrack.Auth.Application.Interfaces;
using FinTrack.Auth.Application.UseCases.Login;
using FinTrack.Auth.Application.UseCases.Register;
using FinTrack.Auth.Domain.Repositories;
using FinTrack.Auth.Infrastructure.Persistence;
using FinTrack.Auth.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ──────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, services, cfg) => cfg
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Service", "auth-service")
        .WriteTo.Console(new Serilog.Formatting.Compact.CompactJsonFormatter())
        .WriteTo.Seq(ctx.Configuration["Seq__ServerUrl"] ?? "http://localhost:5341"));

    // ── OpenTelemetry ────────────────────────────────────────────────────────
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService("auth-service"))
        .WithTracing(t => t
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddOtlpExporter(o => o.Endpoint = new Uri(
                builder.Configuration["Otlp__Endpoint"] ?? "http://localhost:4317")))
        .WithMetrics(m => m
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter());

    // ── Database ─────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<AuthDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

    // ── JWT ──────────────────────────────────────────────────────────────────
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt => opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt__Issuer"],
            ValidAudience            = builder.Configuration["Jwt__Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt__Secret"]!))
        });

    builder.Services.AddAuthorization();

    // ── Application ──────────────────────────────────────────────────────────
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<RegisterHandler>();
    builder.Services.AddScoped<LoginHandler>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // ── Migrations ───────────────────────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await db.Database.MigrateAsync();
    }

    // ── Middleware ────────────────────────────────────────────────────────────
    app.UseSerilogRequestLogging(opt =>
    {
        opt.EnrichDiagnosticContext = (diag, ctx) =>
        {
            diag.Set("ClientIp", ctx.Connection.RemoteIpAddress?.ToString());
            diag.Set("UserAgent", ctx.Request.Headers.UserAgent.ToString());
        };
    });

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapPrometheusScrapingEndpoint();

    // ── Endpoints ─────────────────────────────────────────────────────────────
    app.MapPost("/auth/register", async (RegisterCommand cmd, RegisterHandler handler, CancellationToken ct) =>
    {
        var result = await handler.HandleAsync(cmd, ct);
        return Results.Created($"/auth/users/{result.UserId}", result);
    });

    app.MapPost("/auth/login", async (LoginCommand cmd, LoginHandler handler, CancellationToken ct) =>
    {
        var result = await handler.HandleAsync(cmd, ct);
        return Results.Ok(result);
    });

    app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "auth-service" }));

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Auth service failed to start");
}
finally
{
    await Log.CloseAndFlushAsync();
}
