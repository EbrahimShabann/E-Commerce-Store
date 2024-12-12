using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace BulkyWeb_Api.Filters
{
    public class LogActivityfilter : IActionFilter
    {
        private readonly ILogger<LogActivityfilter> _logger;
        public LogActivityfilter( ILogger<LogActivityfilter> logger)
        {
            _logger=logger;
        }
        public void OnActionExecuting(ActionExecutingContext context )
        {
            _logger.LogInformation($"Action'{context.ActionDescriptor.DisplayName}' execution is on controller '{context.Controller}' with arguments '{JsonSerializer.Serialize(context.ActionArguments)}' ");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"Action'{context.ActionDescriptor.DisplayName}' is already executed on controller {context.Controller}  ");
        }


    }
}
