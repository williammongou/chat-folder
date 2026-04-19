using TextLivingDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register our custom services
builder.Services.AddSingleton<ConversationAnalysisService>();

// Configure CORS for local development and Azure deployment
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",           // Local Angular dev
                "https://localhost:4200",          // Local Angular dev (HTTPS)
                "http://localhost:5173",           // Alternative local port
                "https://*.azurestaticapps.net",   // Azure Static Web Apps
                "https://*.z13.web.core.windows.net" // Azure Storage Static Website
            )
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost") ||
                origin.StartsWith("https://localhost") ||
                origin.Contains("azurestaticapps.net") ||
                origin.Contains("web.core.windows.net"))
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before other middleware
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
