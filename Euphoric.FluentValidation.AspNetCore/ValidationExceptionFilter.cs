using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

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
            var problemBuilder = context.HttpContext.RequestServices.GetRequiredService<IValidationProblemDetailsBuilder>();
            
            context.ExceptionHandled = true;
            context.Result = new BadRequestObjectResult(problemBuilder.Build(new ValidationResult(ex.Errors), context.HttpContext));
        }

        return Task.CompletedTask;
    }
}