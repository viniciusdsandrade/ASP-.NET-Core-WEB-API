using System.Text.Json.Serialization;
using APICatalog.Context;
using APICatalog.Filters;
using APICatalog.Handler;
using APICatalog.Logging;
using APICatalog.Repositories.Async;
using APICatalog.Repositories.Sync; // Adicione este using
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiLoggingFilterSync>();
    options.Filters.Add<ApiLoggingFilterAsync>();
    options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionStr,
        ServerVersion.AutoDetect(mySqlConnectionStr)));

// Registro da dependência do ICategoryRepositoryAsync
builder.Services.AddScoped<ICategoryRepositoryAsync, CategoryRepositoryAsync>();

// Registro da dependência do IProductRepositoryAsync
builder.Services.AddScoped<IProductRepositoryAsync, ProductRepositoryAsync>();

// Registro da dependência do IcategoryRepositorySync
builder.Services.AddScoped<ICategoryRepositorySync, CategoryRepositorySync>();

// Registro de dependência do IProductRepositorySync
builder.Services.AddScoped<IProductRepositorySync, ProductRepositorySync>();


builder.Services.AddScoped<ApiLoggingFilterSync>();
builder.Services.AddScoped<ApiLoggingFilterAsync>();

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Configurar o ExceptionHandler CORRETAMENTE
app.ConfigureExceptionHandler();

app.Use(async (context, next) =>
{
    // adicionar o código antes do request
    await next();
    // adicionar o código depois do request
});

app.MapControllers();

app.Run();