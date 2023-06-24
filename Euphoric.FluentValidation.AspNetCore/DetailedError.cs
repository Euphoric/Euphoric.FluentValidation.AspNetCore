using System.Text.Json.Serialization;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Euphoric.FluentValidation.AspNetCore;

/// <summary>
/// Contains details about validation error.
/// </summary>
public class DetailedError
{
    public string PropertyName { get; set; } = null!;
    public string ErrorCode { get; set; } = null!;
    public object? AttemptedValue { get; set; }
    public string ErrorMessage { get; set; } = null!;
    
    [JsonExtensionData] 
    public IDictionary<string, object?> Extensions { get; set; } = null!;
}