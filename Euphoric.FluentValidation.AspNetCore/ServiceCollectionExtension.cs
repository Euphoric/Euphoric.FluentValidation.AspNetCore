using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Euphoric.FluentValidation.AspNetCore;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddAutoFluentValidations(this IServiceCollection services)
    {
        // disables model validation using null validator
        services.AddSingleton<IObjectModelValidator, NullObjectModelValidator>();

        return services;
    }
}