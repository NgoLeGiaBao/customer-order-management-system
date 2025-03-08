var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(); // OpenAPI for documentation
builder.Services.AddReverseProxy() // Add Reverse Proxy services
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));  // Load config from appsettings.json

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();  // Enable detailed errors in development

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Map Reverse Proxy routes
app.MapReverseProxy();

app.Run();
