using AmazeCare.API.Data;
using AmazeCare.API.Filters;
using AmazeCare.API.Middleware;
using AmazeCare.API.Services;
using AmazeCare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════════════════
// STEP 1: DATABASE
// ══════════════════════════════════════════════════════════════
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ══════════════════════════════════════════════════════════════
// STEP 2: REGISTER SERVICES (Dependency Injection)
// ══════════════════════════════════════════════════════════════
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

// ══════════════════════════════════════════════════════════════
// STEP 3: JWT AUTHENTICATION
// ══════════════════════════════════════════════════════════════
var jwtKey = builder.Configuration["JwtSettings:SecretKey"]!;
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]!;
var jwtAudience = builder.Configuration["JwtSettings:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(jwtKey))
    };


options.Events = new JwtBearerEvents
{
    // When token is missing or invalid → 401
    OnChallenge = async context =>
    {
        context.HandleResponse();
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            "{\"success\":false,\"message\":\"Unauthorized. Please login first.\",\"data\":null}");
    },

    // When token is valid but role is wrong → 403
    OnForbidden = async context =>
    {
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            "{\"success\":false,\"message\":\"Forbidden. You do not have permission to perform this action.\",\"data\":null}");
    }
};

});

builder.Services.AddAuthorization();

// ══════════════════════════════════════════════════════════════
// STEP 4: CONTROLLERS + EXCEPTION FILTER
// AddControllers() called only ONCE
// ══════════════════════════════════════════════════════════════
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AmazeCareExceptionFilter>();
});

// ══════════════════════════════════════════════════════════════
// STEP 5: SWAGGER
// Swagger provides a nice browser UI to test all your APIs.
// Also configured with JWT support so you can test protected routes.
// ══════════════════════════════════════════════════════════════

builder.Services.AddControllers();
; builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AmazeCare API",
        Version = "v1",
        Description = "Healthcare Management System - AmazeCare REST API"
    });

    // ── ADD THESE 3 LINES ──────────────────────────────
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Only include if file exists - prevents error
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here. Format: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ══════════════════════════════════════════════════════════════
// STEP 6: CORS
// ══════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",  // React
                "http://localhost:3000"   // Angular
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ══════════════════════════════════════════════════════════════
// BUILD APP
// ══════════════════════════════════════════════════════════════
var app = builder.Build();

// ══════════════════════════════════════════════════════════════
// MIDDLEWARE PIPELINE — ORDER MATTERS!
// ══════════════════════════════════════════════════════════════

// 1. Global Exception Handler (catches ALL errors)
app.UseMiddleware<GlobalExceptionMiddleware>();

//// 2. Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AmazeCare API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle= "AmazeCare API";
    });
}

// 3. HTTPS
app.UseHttpsRedirection();

// 4. CORS
app.UseCors("AllowAll");

// 5. Authentication (JWT check)
app.UseAuthentication();

// 6. Authorization (Role check)
app.UseAuthorization();

// 7. Controllers
app.MapControllers();

// ══════════════════════════════════════════════════════════════
// AUTO-MIGRATE DATABASE ON STARTUP (only in Development)
// This automatically creates tables when you run the project.
// ══════════════════════════════════════════════════════════════
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();  // Creates DB + tables if they don't exist
    }
}


app.Run();