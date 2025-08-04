using Livena.Identity.Application.Exceptions;
using Livena.Identity.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Livena.Identity.Api.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _env;

    public ApiGlobalExceptionFilter(IHostEnvironment env) => _env = env;

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var exception = context.Exception;

        if (exception is EntityValidationException entityValidationException)
        {
            details.Status = StatusCodes.Status422UnprocessableEntity;
            details.Type = "UnprocessableEntity";
            details.Extensions["errors"] = entityValidationException.Message.Split(" | ");
        }
        else if (exception is NotFoundException)
        {
            details.Status = StatusCodes.Status404NotFound;
            details.Type = "NotFound";
            details.Detail = exception!.Message;
        }
        else if (exception is ConflictException)
        {
            details.Status = StatusCodes.Status409Conflict;
            details.Type = "Conflict";
            details.Detail = exception!.Message;
        }
        else if (exception is WrongCredentialsException)
        {
            details.Status = StatusCodes.Status401Unauthorized;
            details.Type = "WrongCredentials";
            details.Detail = exception!.Message;
        }
        else if (exception is BadRequestException)
        {
            details.Status = StatusCodes.Status400BadRequest;
            details.Type = "BadRequest";
            details.Detail = exception!.Message;
        }
        else
        {
            details.Status = StatusCodes.Status422UnprocessableEntity;
            details.Type = "UnexpectedError";
            details.Detail = exception.Message;
        }

        context.HttpContext.Response.StatusCode = (int)details.Status;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}
