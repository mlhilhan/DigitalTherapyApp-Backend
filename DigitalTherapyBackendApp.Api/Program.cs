using DigitalTherapyBackendApp.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using MediatR;
using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DigitalTherapyBackendApp.Api.Middleware;
using StackExchange.Redis;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using DigitalTherapyBackendApp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Repositories;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// MediatR
builder.Services.AddMediatR(typeof(Program).Assembly);

// AutoMapper konfigürasyonu
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(DigitalTherapyBackendApp.Infrastructure.Mapping.MappingProfile).Assembly);

// Token Black List
builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

// CORS Settings
var allowedOrigins = builder.Environment.IsDevelopment()
    ? new[] { "http://localhost:3000", "http://localhost:5173" } // Development
    : new[] { "https://example.com", "https://api.example.com" }; // Prod

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithOrigins(allowedOrigins); // Sadece belirlenen originler
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

// Swagger UI Settings
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Digital Therapy API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Identity Settings
builder.Services.AddIdentity<User, DigitalTherapyBackendApp.Domain.Entities.Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;

    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
});

// Authentication Settings
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidIssuer = "DigitalTherapyApp",
        ValidateAudience = false,
        ValidAudience = "DigitalTherapyUsers",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration["JWTSecurity:SecretKey"]!)),
        ValidateLifetime = true
    };
});

// Authorization Settings
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("PsychologistOnly", policy => policy.RequireRole("Psychologist"));
    options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
});

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddHttpClient();

// Redis
var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
var redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = false;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Therapy Api v1");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "storage", "avatars")),
    RequestPath = "/storage/avatars"
});

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

app.UseMiddleware<BlacklistTokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
