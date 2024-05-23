using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalog.Filters;

public class ApiLoggingFilterAsync : IAsyncActionFilter
{
    private readonly ILogger<ApiLoggingFilterAsync> _logger;

    public ApiLoggingFilterAsync(ILogger<ApiLoggingFilterAsync> logger) => _logger = logger;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Log antes da execução da action
        _logger.LogInformation("### Executando -> OnActionExecutionAsync ###");
        _logger.LogInformation("############################################");
        _logger.LogInformation(
            $"{DateTime.Now.ToLongTimeString()} - Iniciando execução: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");

        // Executa a action e obtém o contexto após a execução
        var actionExecutedContext = await next();

        // Log após a execução da action
        _logger.LogInformation("### Executando -> OnActionExecutedAsync ###");
        _logger.LogInformation("############################################");

        if (actionExecutedContext.Exception != null)
        {
            _logger.LogError(actionExecutedContext.Exception, "Erro durante a execução da ação.");

            // Criar uma resposta customizada em caso de erro (opcional)
            actionExecutedContext.Result = new ObjectResult(new ErrorResponse
            {
                StatusCode = 500,
                Message = "Ocorreu um erro interno no servidor."
            });

            actionExecutedContext.ExceptionHandled = true;
        }
        else
        {
            _logger.LogInformation(
                $"{DateTime.Now.ToLongTimeString()} - Status Code: {actionExecutedContext.HttpContext.Response.StatusCode}");
        }
    }
}

// Classe para a resposta customizada de erro (opcional)
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
}