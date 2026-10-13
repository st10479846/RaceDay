using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. Register the database context
// ============================================================
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// 2. Register controllers + OpenAPI
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "RaceDay API",
        Version = "v1",
        Description = "RaceDay event management API (PROG6212 PoE Part 2)"
    });
});

// ============================================================
// 3. Session support (feedback fix: session-based auth)
// ============================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".RaceDay.Session";
});

// ============================================================
// 4. Build the app
// ============================================================
var app = builder.Build();

// ============================================================
// 5. Configure the HTTP request pipeline
// ============================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession();      // <-- MUST come before MapControllers

app.UseAuthorization();
app.MapControllers();

app.Run();

// Required so RaceDay.Api.Tests can use WebApplicationFactory<Program>
public partial class Program { }