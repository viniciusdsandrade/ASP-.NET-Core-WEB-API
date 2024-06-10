using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalog.Filters;

public class ApiLoggingFilterSync : IActionFilter
{
    private readonly ILogger<ApiLoggingFilterSync> _logger;
    private readonly Stopwatch _stopwatch;

    public ApiLoggingFilterSync(ILogger<ApiLoggingFilterSync> logger)
    {
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch.Start();

        var request = context.HttpContext.Request;
        _logger.LogInformation("### Iniciando solicitação ###");
        _logger.LogInformation($"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        _logger.LogInformation($"Método: {request.Method}, URL: {request.Path}");
        _logger.LogInformation($"Model State válido? {context.ModelState.IsValid}");

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
            _logger.LogDebug(
                $"Query Strings: {string.Join(", ", context.HttpContext.Request.Query.Select(q => $"{q.Key}={q.Value}"))}");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();

        _logger.LogInformation("### Solicitação finalizada ###");
        _logger.LogInformation($"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        _logger.LogInformation(
            $"StatusCode: {context.HttpContext.Response.StatusCode} ({context.HttpContext.Response.StatusCode})");
        _logger.LogInformation($"Tempo total de execução: {_stopwatch.ElapsedMilliseconds}ms");
        _logger.LogInformation("--------------------------------------------------------");

        if (context.Exception != null)
        {
            _logger.LogError(context.Exception, "Exceção não tratada durante a execução da ação.");
            context.ExceptionHandled = true; // Impede a propagação da exceção (opcional)

            // Aqui você pode adicionar lógica para retornar uma resposta customizada de erro ao cliente
            // context.Result = new ObjectResult(...)
        }
    }
}