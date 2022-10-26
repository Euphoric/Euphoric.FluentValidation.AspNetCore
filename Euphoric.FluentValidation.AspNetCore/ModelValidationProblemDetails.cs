using Microsoft.AspNetCore.Mvc;

namespace Euphoric.FluentValidation.AspNetCore;

public class ModelValidationProblemDetails : ProblemDetails
{
    public IReadOnlyList<ModelValidationError> Errors { get; set; }
}