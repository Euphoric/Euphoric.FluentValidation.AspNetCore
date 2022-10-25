using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Euphoric.FluentValidation.AspNetCore;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddAutoFluentValidations(this IServiceCollection services)
    {
        // disables model validation using null validator
        services.AddSingleton<IObjectModelValidator, NullObjectModelValidator>();

        // register MVC filters in MVC
        Action<MvcOptions> setupAction = options => options.Filters.Add<ValidationActionFilter>();
        services.Configure(setupAction);

        return services;
    }
}