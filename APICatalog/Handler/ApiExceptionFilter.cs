using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalog.Handler;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception has occurred: status code 500.");
        context.Result = new ObjectResult(new { Message = "An error occurred. Try again later." })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}