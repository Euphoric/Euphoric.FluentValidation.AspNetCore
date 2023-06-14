using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Euphoric.FluentValidation.AspNetCore;

public class NullObjectModelValidator : IObjectModelValidator
{
    public void Validate(ActionContext actionContext, ValidationStateDictionary? validationState, string prefix, object? model)
    {
        foreach (var (_, value) in actionContext.ModelState)
        {
            value.ValidationState = ModelValidationState.Skipped;
        }
    }
}