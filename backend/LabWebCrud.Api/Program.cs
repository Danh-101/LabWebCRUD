using LabWebCrud.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Doc chuoi ket noi trong appsettings.json va dang ky Entity Framework Core.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Thieu ConnectionStrings:DefaultConnection.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// Neu sau nay chay frontend rieng (Vite 5173), CORS da duoc cho phep.
// Trong bai nay frontend duoc ASP.NET phuc vu cung domain, nen khong can CORS.
builder.Services.AddCors(options => options.AddPolicy("DevelopmentFrontend", policy =>
    policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
          .AllowAnyHeader()
          .AllowAnyMethod()));

var app = builder.Build();

app.UseDefaultFiles();  // Truy cap / se tra ve wwwroot/index.html.
app.UseStaticFiles();   // Phuc vu CSS, JS va HTML Frontend.
app.UseCors("DevelopmentFrontend");
app.MapControllers();   // Kich hoat cac endpoint trong ProductsController.

app.Run();
