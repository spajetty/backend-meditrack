using backend_meditrack.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ✅ Correct and use only one CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://meditrack-web-app.vercel.app" // ✅ Add Vercel URL here
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// ✅ EF Core
builder.Services.AddDbContext<ClinicDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ✅ JSON loop fix
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5002);  // Optional: HTTP local
    serverOptions.ListenLocalhost(7015, listenOptions => listenOptions.UseHttps()); // Optional: HTTPS local
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend"); // ✅ Only this one

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
