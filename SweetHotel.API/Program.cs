using Microsoft.EntityFrameworkCore;
using SweetHotel.API.Data;
using SweetHotel.API.Repositories;
using SweetHotel.API.Mappings;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "SweetHotel API", 
        Version = "v1",
        Description = "API for SweetHotel Management System"
    });
    
    // Custom schema IDs to avoid conflicts
    c.CustomSchemaIds(type => type.FullName);
});

// ??ng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ??ng ký AutoMapper v?i assembly ch?a MappingProfile
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// ??ng ký Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    try
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SweetHotel API V1");
            c.DisplayRequestDuration();
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Swagger Error: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
