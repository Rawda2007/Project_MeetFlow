using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Repositories;
using MeetFlow.BLL.Interfaces;
using MeetFlow.BLL.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database (connection string lives in appsettings.json) ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection in appsettings.json.");

builder.Services.AddDbContext<MeetFlowDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------- Authentication (JWT) ----------
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Missing Jwt:Key in appsettings.json.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Keep claim types exactly as issued ("sub", "email"...) instead of ASP.NET Core's
    // legacy auto-remapping to long XML-schema URIs — makes reading claims predictable.
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// ---------- Authorization ----------
// Secure-by-default: every endpoint requires a valid JWT unless explicitly marked [AllowAnonymous].
// This means new controllers (Meetings, Tasks, Workspaces...) are protected automatically,
// even if someone forgets to add [Authorize] on them.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ---------- CORS ----------
// Restrict browser access to the actual frontend origin instead of leaving it wide open.
// Set Frontend:Origin in appsettings.json once you know where the frontend is hosted.
var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:3000";
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- Rate limiting ----------
// Basic brute-force protection on the auth endpoints (login/register/forgot-password are the
// most attractive targets for credential stuffing / spam). 10 requests per minute per client IP.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

// ---------- App services (DI) ----------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IDecisionService, DecisionService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddHttpClient<IAiTaskExtractionService, GeminiTaskExtractionService>();
builder.Services.AddHttpClient<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


// ---------- Controllers + Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Lets Swagger UI send "Authorization: Bearer {token}" so [Authorize] endpoints can be tested directly.
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste only the token (no 'Bearer ' prefix needed here)."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            System.Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------- Global exception handling ----------
// Converts known exceptions into clean, safe HTTP responses instead of leaking
// stack traces or internal details to the client.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        // Always log the real exception — the HTTP response stays generic/safe,
        // but you can see exactly what broke in the terminal / VS Output window.
        app.Logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);

        var (statusCode, message) = ex switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ex.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
            InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
            HttpRequestException => (StatusCodes.Status502BadGateway,
                "Failed to reach an external service (Gemini or WhatsApp). Check the API keys/tokens in appsettings.json and see the server console for details."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message });
    }
});

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
