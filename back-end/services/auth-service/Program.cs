using System.Text;
using auth_service.Data;
using auth_service.Models;
using auth_service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure services in the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity services for managing users and roles
builder.Services.AddIdentity<Employee, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure Redis cache (for storing tokens or user authentication data if needed)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
});

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add controllers for API endpoints
builder.Services.AddControllers();

// Configure Swagger (OpenAPI) for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register SeedData as a transient service
builder.Services.AddTransient<SeedData>();

var app = builder.Build();

// Seed the database with initial data (Admin user and roles)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<Employee>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    // Get the SeedData service
    // var seedData = services.GetRequiredService<SeedData>();

    // Call the non-static method Initialize
    // await seedData.Initialize(userManager, roleManager);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthentication(); // Ensure authentication for API requests
app.UseAuthorization(); // Ensure authorization for API requests

app.MapControllers(); // Map all controllers for API routes

app.Run(); // Run the application