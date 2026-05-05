using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Emart_DotNet.Utilities.Filters
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Log before the action executes
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var method = context.HttpContext.Request.Method;

            _logger.LogInformation($"Executing action: {controllerName}.{actionName} ({method})");

            var stopwatch = Stopwatch.StartNew();

            // Execute the action
            var resultContext = await next();

            stopwatch.Stop();

            // Log after the action executes
            if (resultContext.Exception != null)
            {
                _logger.LogError(resultContext.Exception, $"Action {controllerName}.{actionName} failed in {stopwatch.ElapsedMilliseconds}ms");
            }
            else
            {
                var statusCode = resultContext.HttpContext.Response.StatusCode;
                _logger.LogInformation($"Action {controllerName}.{actionName} executed in {stopwatch.ElapsedMilliseconds}ms. Status Code: {statusCode}");
            }
        }
    }
}
