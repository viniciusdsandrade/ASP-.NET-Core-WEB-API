using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalog.Handler;

public abstract class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An unhandled exception has occurred: status code 500.");
        context.Result = new ObjectResult(new { Message = "An error occurred. Try again later." })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}