using System.Net;
using System.Text.Json;

namespace FinancialAccountManagementApi.Common.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        IWebHostEnvironment env = (IWebHostEnvironment)context.RequestServices.GetService(typeof(IWebHostEnvironment))!;
        
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, env, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, IWebHostEnvironment env, Exception exception)
    {
        string result = "";
        var code = HttpStatusCode.InternalServerError;

        if (env.IsDevelopment())
        {
            var errorMessage = new
            {
                error = exception.Message,
                stack = exception.StackTrace,
                innerException = exception.InnerException
            };

            result = JsonSerializer.Serialize(errorMessage);
        }
        else
        {
            var errorMessage = new
            {
                error = exception.Message
            };

            result = JsonSerializer.Serialize(errorMessage);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;
        
        return context.Response.WriteAsync(result);
    }
}