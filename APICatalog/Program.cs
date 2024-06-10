using System.Text.Json.Serialization;
using APICatalog.Context;
using APICatalog.Filters;
using APICatalog.Handler;
using APICatalog.Logging;
using APICatalog.Repositories.Async;
using APICatalog.Repositories.Generic;
using APICatalog.Repositories.Sync;
using APICatalog.Repositories.UnitOfWork.Async;
using APICatalog.Repositories.UnitOfWork.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
builder.Services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync>();
builder.Services.AddScoped<IUnitOfWorkSync, UnitOfWorkSync>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "APICatalog",
        Version = "v1.0.0",
        Description = "API de Catálogo de Produtos e Categorias",
        TermsOfService = new Uri("https://github.com/viniciusdsandrade/ASP-.NET-Core-WEB-API"),
        Contact = new OpenApiContact
        {
            Name = "Vinícius Andrade",
            Email = "vinicius_andrade2010@hotmail.com",
            Url = new Uri("https://www.linkedin.com/in/viniciusdsandrade/"),
        },
        License = new OpenApiLicense
        {
            Name = "Github",
            Url = new Uri("https://github.com/viniciusdsandrade")
        }
    });
});

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalog v1")
    );
}

app.ConfigureExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Use(async (context, next) =>
{
    // adicionar o código antes do request
    await next();
    // adicionar o código depois do request
});

app.Run();