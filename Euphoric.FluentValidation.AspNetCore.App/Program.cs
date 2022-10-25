using System.Reflection;
using FluentValidation;

namespace Euphoric.FluentValidation.AspNetCore;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        
        builder.Services.AddAutoFluentValidations();
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.MapHealthChecks("/health");
        
        await app.RunAsync();
    }
}