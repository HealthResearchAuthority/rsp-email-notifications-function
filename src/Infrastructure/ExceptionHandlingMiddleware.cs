using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.NotifyFunction.Application.Constants;

namespace Rsp.NotifyFunction.Infrastructure;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogAsError(NotifyServiceExceptions.FunctionError, "Error processing invocation", ex); // Constant for error here
        }
    }
}