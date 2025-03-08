using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using payment_service.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = builder.Configuration;

// Register the ApplicationDbContext with the DI container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Configure Kestrel to listen on port 84
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(84); 
});


// Register the OrderEventHandler as a background service (if applicable)
// builder.Services.AddHostedService<OrderEventHandler>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing")))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin")); 
});

builder.Services.AddControllers(); 

// Add authorization services
builder.Services.AddAuthorization();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with JWT Bearer Authentication support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment Service API", Version = "v1" });

    // Add Bearer Token authentication in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}' below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
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


var app = builder.Build();

// Enable Swagger UI in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Menu Service API v1");
        c.RoutePrefix = string.Empty; // This makes Swagger UI available at the root
    });
}

// Enable Authentication & Authorization middleware
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

// Enable HTTPS Redirection
app.UseHttpsRedirection();

// **Apply Migrations & Seed Data**
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<ApplicationDbContext>();

//     try
//     {
//         context.Database.Migrate(); // Apply migrations
//         DataSeeder.SeedData(context); // Seed data
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Database migration failed: {ex.Message}");
//     }
// }

app.Run();
