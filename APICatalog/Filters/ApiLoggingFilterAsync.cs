using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalog.Filters;

public class ApiLoggingFilterAsync(ILogger<ApiLoggingFilterAsync> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Log antes da execução da action
        logger.LogInformation("### Executando -> OnActionExecutionAsync ###");
        logger.LogInformation("############################################");
        logger.LogInformation(
            $"{DateTime.Now.ToLongTimeString()} - Iniciando execução: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");

        // Executa a action e obtém o contexto após a execução
        var actionExecutedContext = await next();

        // Log após a execução da action
        logger.LogInformation("### Executando -> OnActionExecutedAsync ###");
        logger.LogInformation("############################################");

        if (actionExecutedContext.Exception != null)
        {
            logger.LogError(actionExecutedContext.Exception, "Erro durante a execução da ação.");

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
            logger.LogInformation(
                $"{DateTime.Now.ToLongTimeString()} - Status Code: {actionExecutedContext.HttpContext.Response.StatusCode}");
        }
    }
}

// Classe para a resposta customizada de erro (opcional)
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}