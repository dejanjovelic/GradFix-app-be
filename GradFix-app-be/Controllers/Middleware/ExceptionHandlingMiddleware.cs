using System.Text.Json;
using GradFix_app_be.Services.Exceptions;

namespace GradFix_app_be.Controllers.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public ExceptionHandlingMiddleware() { }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }

        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };
            var response = new { error = exception.Message };
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }


    }
}
