using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using order_service.Data;
using order_service.EventHandlers;
using System.Text;
using RabbitMQ.Client;  

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = builder.Configuration;

// Register the ApplicationDbContext with the DI container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Build the controller
builder.Services.AddControllers(); 

// Register the OrderEventHandler as a background service
builder.Services.AddHostedService<OrderEventHandler>();

// Configure rabbitMQ 
// builder.Services.AddSingleton<IConnectionFactory>(sp =>
// {
//     var configuration = sp.GetRequiredService<IConfiguration>();
//     var factory = new ConnectionFactory()
//     {
//         HostName = configuration["RabbitMQ:Host"], 
//         Port = int.Parse(configuration["RabbitMQ:Port"])  
//     };
//     return factory;
// });

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with JWT Bearer Authentication support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service API", Version = "v1" });

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

// Configure Kestrel to listen on port 82
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(83);  // Ensure the app listens on port 82
});

var app = builder.Build();

// Enable Swagger UI in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API v1");
        c.RoutePrefix = string.Empty;  // This makes Swagger UI available at the root
    });
}

//
// Enable map controllers, authentication, authorization, HTTPS redirection and run the app
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

// Enable HTTPS Redirection
app.UseHttpsRedirection();

app.Run();
