using Microsoft.OpenApi;
using Metro.Domain.Users.Entities;
using Metro.Infrastructure.PostgreSQL.Extensions;
using Metro.Infrastructure.PostgreSQL.Dapper.Extensions;
using Microsoft.Extensions.FileProviders;
using Metro.Infrastructure.PostgreSQL.Initializers;
using Metro.Infrastructure.PostgreSQL.Contexts;
using Metro.Domain.Services;
using Metro.Infrastructure.File.Extensions;
using Metro.Infrastructure.Email.Services;
using Metro.Domain.Users.Authentication.Services;
using Metro.Infrastructure.Auth.Services;
using Metro.Application.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IPasswordRecoverySettings, PasswordRecoverySettings>();

IConfiguration configuration = builder.Configuration;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var isDeveloperEnvironment = bool.TryParse(configuration["DeveloperEnvironments"], out var developerEnvironments)
            && developerEnvironments;
        var jwtKey = configuration["Authentication:JWT:Key"]
            ?? throw new InvalidOperationException("Authentication:JWT:Key is not configured.");
        var issuer = configuration["Authentication:JWT:Issuer"]
            ?? throw new InvalidOperationException("Authentication:JWT:Issuer is not configured.");
        var audience = configuration["Authentication:JWT:Audience"]
            ?? throw new InvalidOperationException("Authentication:JWT:Audience is not configured.");

        options.RequireHttpsMetadata = !isDeveloperEnvironment;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("master", policy =>
        policy.RequireRole("master"));
    options.AddPolicy("client", policy =>
        policy.RequireRole("client"));
    options.AddPolicy("user", policy =>
        policy.RequireRole("master", "client"));
});

ConfigureServices(builder.Services);

var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        if (corsOrigins.Length > 0)
        {
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.SetIsOriginAllowed(_ => false);
        }
    });
});

var createUserPermitLimit = int.Parse(configuration["RateLimit:CreateUser:PermitLimit"] ?? "5");
var createUserWindowMinutes = int.Parse(configuration["RateLimit:CreateUser:WindowMinutes"] ?? "15");
var loginPermitLimit = int.Parse(configuration["RateLimit:Login:PermitLimit"] ?? "10");
var loginWindowMinutes = int.Parse(configuration["RateLimit:Login:WindowMinutes"] ?? "15");
var passwordRecoveryPermitLimit = int.Parse(configuration["RateLimit:PasswordRecovery:PermitLimit"] ?? "5");
var passwordRecoveryWindowMinutes = int.Parse(configuration["RateLimit:PasswordRecovery:WindowMinutes"] ?? "15");

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("create-user", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = createUserPermitLimit,
                Window = TimeSpan.FromMinutes(createUserWindowMinutes),
                QueueLimit = 0
            }));
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = loginPermitLimit,
                Window = TimeSpan.FromMinutes(loginWindowMinutes),
                QueueLimit = 0
            }));
    options.AddPolicy("password-recovery", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = passwordRecoveryPermitLimit,
                Window = TimeSpan.FromMinutes(passwordRecoveryWindowMinutes),
                QueueLimit = 0
            }));
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(User).Assembly);
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("ApiV1", new OpenApiInfo { Title = "Metro API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT no header. Exemplo: Bearer {token}"
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

builder.Services.AddPostgreSQL(builder.Configuration);
builder.Services.AddPostgreSQLDapper(builder.Configuration);
builder.Services.AddFileStorage();

var postgresConnection = configuration["PostgreSQL:Connection"]
    ?? throw new InvalidOperationException("PostgreSQL:Connection is not configured.");

builder.Services.AddHealthChecks()
    .AddNpgSql(postgresConnection, name: "postgresql");

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PostgreSQLContext>();
    context.MigrateDatabase();

    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    var hashedPassword = passwordService.HashPassword("131102");
    context.SeedMasterUser(hashedPassword);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/ApiV1/swagger.json", "Metro API v1");
    });
}

app.UseExceptionHandler();
app.UseCors("Default");
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

var storageRoot = Path.IsPathRooted(configuration["Storage:RootPath"])
    ? configuration["Storage:RootPath"]!
    : Path.Combine(Directory.GetCurrentDirectory(), configuration["Storage:RootPath"] ?? Path.Combine("Public", "Storage"));
Directory.CreateDirectory(storageRoot);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storageRoot),
    RequestPath = configuration["Storage:RequestPath"] ?? "/Storage"
});

void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IEmailService, EmailService>();
}

app.MapControllers();

app.Run();
