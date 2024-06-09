using System.Text.Json.Serialization;
using APICatalog.Context;
using APICatalog.Filters;
using APICatalog.Handler;
using APICatalog.Logging;
using APICatalog.Repositories.Async;
using APICatalog.Repositories.Generic;
using APICatalog.Repositories.Sync;
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

var mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionStr,
        ServerVersion.AutoDetect(mySqlConnectionStr)));

builder.Services.AddScoped<ICategoryRepositoryAsync, CategoryRepositoryAsync>();
builder.Services.AddScoped<IProductRepositoryAsync, ProductRepositoryAsync>();
builder.Services.AddScoped<ICategoryRepositorySync, CategoryRepositorySync>();
builder.Services.AddScoped<IProductRepositorySync, ProductRepositorySync>();
builder.Services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
builder.Services.AddScoped(typeof(IRepositorySync<>), typeof(RepositorySync<>));
builder.Services.AddScoped<ApiExceptionFilter>();
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