using System.Diagnostics;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Euphoric.FluentValidation.AspNetCore;

static class ValidationProblemDetailsBuilder
{
    public static ProblemDetails Build(ValidationResult validationResult, HttpContext httpContext)
    {
        return ModelValidationProblemDetails.Build(validationResult, GetTraceId(httpContext));
    }
    
    private static string GetTraceId(HttpContext httpContext)
    {
        return Activity.Current?.Id ?? httpContext.TraceIdentifier;
    }
}