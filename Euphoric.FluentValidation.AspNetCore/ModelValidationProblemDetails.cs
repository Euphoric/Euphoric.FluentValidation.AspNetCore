using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Euphoric.FluentValidation.AspNetCore;

public class ModelValidationProblemDetails : ProblemDetails
{
    public IReadOnlyList<ModelValidationError> Errors { get; private set; }

    public static ModelValidationProblemDetails Build(ValidationResult validationResult, string traceId)
    {
        var problemDetails = new ModelValidationProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Detail = validationResult.ToString(),
            Errors = validationResult.Errors.Select(err=>new ModelValidationError(err.PropertyName, err.ErrorCode, err.AttemptedValue, err.ErrorMessage)).ToList()
        };
        
        problemDetails.Extensions["traceId"] = traceId;
        
        return problemDetails;
    }
}