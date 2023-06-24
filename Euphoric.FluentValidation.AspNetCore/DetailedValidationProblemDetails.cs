using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Euphoric.FluentValidation.AspNetCore;

/// <summary>
/// A machine-readable format for specifying errors in HTTP API responses based on <see href="https://tools.ietf.org/html/rfc7807"/>.
/// Includes Asp.Net core model validation errors and detailed errors from FluentValidations.
/// </summary>
public class DetailedValidationProblemDetails : ValidationProblemDetails
{
    public DetailedValidationProblemDetails()
    { }
    
    public DetailedValidationProblemDetails(ModelStateDictionary modelState)
        :base(modelState)
    { }
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public List<DetailedError>? DetailedErrors { get; set; }
}