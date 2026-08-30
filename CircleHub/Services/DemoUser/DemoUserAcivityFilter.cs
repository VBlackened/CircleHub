using CircleHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CircleHub.Services.DemoUser;

public class DemoUserActivityFilter(IDemoUserActivityService demoUserActivityService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Exception is null)
        {
            await demoUserActivityService.UpdateActivityAsync(
                context.HttpContext.User);
        }
    }
}