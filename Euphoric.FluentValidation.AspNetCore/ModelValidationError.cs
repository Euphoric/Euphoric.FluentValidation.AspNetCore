namespace Euphoric.FluentValidation.AspNetCore;

public record ModelValidationError(string PropertyName, string ErrorCode, object? AttemptedValue, string ErrorMessage);