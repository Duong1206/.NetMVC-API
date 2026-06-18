using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace BanSach.Api.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        object response;
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new
            {
                status = StatusCodes.Status400BadRequest,
                title = "Validation failed.",
                errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())
            };
        }
        else if (exception is KeyNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            response = new
            {
                status = StatusCodes.Status404NotFound,
                title = "Resource not found.",
                detail = exception.Message
            };
        }
        else if (exception is UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            response = new
            {
                status = StatusCodes.Status401Unauthorized,
                title = "Unauthorized.",
                detail = exception.Message
            };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response = new
            {
                status = StatusCodes.Status500InternalServerError,
                title = "An unexpected error occurred.",
                detail = exception.Message
            };
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
