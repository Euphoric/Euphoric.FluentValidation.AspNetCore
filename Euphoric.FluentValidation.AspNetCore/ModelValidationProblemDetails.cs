using Microsoft.AspNetCore.Mvc;

namespace Euphoric.FluentValidation.AspNetCore;

public class ModelValidationProblemDetails : ProblemDetails
{
#pragma warning disable CS8618
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    
    public IReadOnlyList<ModelValidationError> Errors { get; set; }
    
#pragma warning restore CS8618
}