using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Euphoric.FluentValidation.AspNetCore;

// ReSharper disable once ClassNeverInstantiated.Global
public class TestServerFixture : WebApplicationFactory<Program>, ITestOutputHelperAccessor
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureLogging(x =>
        {
            x.ClearProviders();
            x.AddXUnit(this);
        });
    }

    public ITestOutputHelper? OutputHelper { get; set; }
}