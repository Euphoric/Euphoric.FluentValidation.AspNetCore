using System.Reflection;
using FluentValidation;

namespace Euphoric.FluentValidation.AspNetCore;

public class Startup
{
    public void ConfigureServices(IServiceCollection service)
    {
        service.AddControllers();
        service.AddSwaggerGen();
        service.AddHealthChecks();
        
        service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Tested addon
        service.AddAutoFluentValidations();
    }
    
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        
        app.UseEndpoints(cfg =>
        {
            cfg.MapControllers();
            cfg.MapHealthChecks("/health");
        });
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHost(web =>
            {
                web.UseStartup<Startup>();
            });
    }
}