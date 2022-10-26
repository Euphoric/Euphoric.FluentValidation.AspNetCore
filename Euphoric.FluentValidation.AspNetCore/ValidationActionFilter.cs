using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Euphoric.FluentValidation.AspNetCore;

/// <summary>
/// ActionFilter used for validation of Action parameters 
/// </summary>
public class ValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new List<ValidationFailure>();
        foreach (var argument in context.ActionArguments.Select(o => o.Value).Where(o => o != null).Select(o => o!)) 
        {
            errors.AddRange(await ValidateAsync(context.HttpContext.RequestServices, argument));
        }

        var validationResult = new ValidationResult(errors);
        
        if (!validationResult.IsValid)
        {
            context.Result = new BadRequestObjectResult( ValidationProblemDetailsBuilder.Build(validationResult, context.HttpContext));
        }
        else
        {
            await next.Invoke();
        }
    }

    private async Task<IEnumerable<ValidationFailure>> ValidateAsync(IServiceProvider sp, object parameter)
    {
        var validator = GetValidator(sp, parameter.GetType());
        if (validator != null)
        {
            var context = ValidationContext<object>.CreateWithOptions(parameter, _ => { });
            var validationResult = await validator.ValidateAsync(context);
            if (!validationResult.IsValid)
            {
                return validationResult.Errors;
            }
        }

        return Enumerable.Empty<ValidationFailure>();
    }
    
    private static IValidator? GetValidator(IServiceProvider serviceProvider, Type type)
    {
        if (type.IsValueType)
        {
            return null;
        }

        var validatorType = typeof(IValidator<>).MakeGenericType(type);
        return serviceProvider.GetService(validatorType) as IValidator;
    }
}