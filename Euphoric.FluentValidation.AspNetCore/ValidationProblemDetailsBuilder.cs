using System.Diagnostics;
using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Euphoric.FluentValidation.AspNetCore;

public interface IValidationProblemDetailsBuilder
{
    public ProblemDetails Build(ValidationResult validationResult, HttpContext httpContext);
}

public class ValidationProblemDetailsBuilder : IValidationProblemDetailsBuilder
{
    private readonly ApiBehaviorOptions _options;
    
    public ValidationProblemDetailsBuilder(IOptions<ApiBehaviorOptions>? options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }
    
    public virtual ProblemDetails Build(ValidationResult validationResult, HttpContext httpContext)
    {
        var problemDetails = new DetailedValidationProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Detail = validationResult.ToString(" "),
            DetailedErrors = validationResult.Errors.Select(BuildErrorDetail).ToList(),
            Extensions =
            {
                ["traceId"] = GetTraceId(httpContext)
            }
        };

        return problemDetails;
    }

    protected virtual DetailedError BuildErrorDetail(ValidationFailure failure)
    {
        var ignoredKeys = new HashSet<string> { "PropertyName", "PropertyValue", "CollectionIndex" };
        var extensionValue = 
            failure.FormattedMessagePlaceholderValues?
                .Where(x=>!ignoredKeys.Contains(x.Key))
                .ToDictionary(x=>x.Key, x=>(object?)x.Value) 
            ?? new Dictionary<string, object?>();
        var modelValidationError = new DetailedError
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