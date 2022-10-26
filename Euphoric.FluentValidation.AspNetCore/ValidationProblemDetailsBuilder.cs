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
    public ProblemDetails Build(ValidationResult validationResult, HttpContext httpContext)
    {
        var problemDetails = new ModelValidationProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Detail = validationResult.ToString(),
            Errors = validationResult.Errors.Select(err=>new ModelValidationError(err.PropertyName, err.ErrorCode, err.AttemptedValue, err.ErrorMessage)).ToList(),
            Extensions =
            {
                ["traceId"] = GetTraceId(httpContext)
            }
        };

        return problemDetails;
    }

    private static string GetTraceId(HttpContext httpContext)
    {
        return Activity.Current?.Id ?? httpContext.TraceIdentifier;
    }
}