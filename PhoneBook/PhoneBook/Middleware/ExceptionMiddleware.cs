using PhoneBook.Contracts.Exceptions;
using System.Text.Json;

namespace PhoneBook.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Исключение: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new { error = exception.Message };

        switch (exception)
        {
            case ContactNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                break;
            case ContactsLimitException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            case DuplicatePhoneException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new { error = "Внутренняя ошибка сервера" };
                break;
        }

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
