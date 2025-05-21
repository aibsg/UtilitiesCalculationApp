using Microsoft.EntityFrameworkCore;
using UtilitiesCalculationApp.Data;
using UtilitiesCalculationApp.Services;
using UtilitiesCalculationApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICalculationService, CalculationService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();