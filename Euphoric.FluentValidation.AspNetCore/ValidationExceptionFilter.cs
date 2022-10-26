using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Euphoric.FluentValidation.AspNetCore;

/// <summary>
/// Converts <see cref="ValidationException"/> thrown by controllers into 400 response with problem details.
/// </summary>
public class ValidationExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is ValidationException ex)
        {
            context.ExceptionHandled = true;
            context.Result = new BadRequestObjectResult(ValidationProblemDetailsBuilder.Build(new ValidationResult(ex.Errors), context.HttpContext));
        }

        return Task.CompletedTask;
    }
}