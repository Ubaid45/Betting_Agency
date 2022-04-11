using System.Net;
using System.Text.Json;
using BettingAgency.Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace BettingAgency.Application.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "application/json";
            if (e.GetType() == typeof(HttpStatusException))
            {
                var result = JsonSerializer.Serialize(new
                {
                    errorCode = (e as HttpStatusException).ErrorCode
                });
                context.Response.StatusCode = (int)(e as HttpStatusException).StatusCode;
                await context.Response.WriteAsync(result);
            }
            else
            {
                var result = JsonSerializer.Serialize(new
                {
                    message = e.Message,
                    errorCode = nameof(HttpStatusCode.InternalServerError)
                });
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(result);
            }
        }
    }
}