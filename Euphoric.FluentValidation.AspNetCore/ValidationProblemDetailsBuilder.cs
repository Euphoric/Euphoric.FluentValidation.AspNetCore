using System.Diagnostics;
using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Euphoric.FluentValidation.AspNetCore;

public interface IValidationProblemDetailsBuilder
{
    public ProblemDetails Build(ValidationResult validationResult, HttpContext httpContext);
}

public class ValidationProblemDetailsBuilder : IValidationProblemDetailsBuilder
{
    public virtual ProblemDetails Build(ValidationResult validationResult, HttpContext httpContext)
    {
        var problemDetails = new ModelValidationProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Detail = validationResult.ToString(" "),
            Errors = validationResult.Errors.Select(BuildErrorDetail).ToList(),
            Extensions =
            {
                ["traceId"] = GetTraceId(httpContext)
            }
        };

        return problemDetails;
    }

    protected virtual ModelValidationError BuildErrorDetail(ValidationFailure failure)
    {
        var ignoredKeys = new HashSet<string> { "PropertyName", "PropertyValue", "CollectionIndex" };
        var extensionValue = 
            failure.FormattedMessagePlaceholderValues?
                .Where(x=>!ignoredKeys.Contains(x.Key))
                .ToDictionary(x=>x.Key, x=>(object?)x.Value) 
            ?? new Dictionary<string, object?>();
        var modelValidationError = new ModelValidationError
        {
            PropertyName = failure.PropertyName,
            ErrorCode = failure.ErrorCode,
            ErrorMessage = failure.ErrorMessage,
            AttemptedValue = failure.AttemptedValue,
            Extensions = extensionValue
        };
        return modelValidationError;
    }

    private static string GetTraceId(HttpContext httpContext)
    {
        return Activity.Current?.Id ?? httpContext.TraceIdentifier;
    }
}