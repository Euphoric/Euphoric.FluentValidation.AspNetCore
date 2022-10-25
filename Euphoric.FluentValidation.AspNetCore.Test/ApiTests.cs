using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

namespace Euphoric.FluentValidation.AspNetCore;

[UseReporter(typeof(DiffReporter))]
public class ApiTests : IClassFixture<TestServerFixture>
{
    private TestServerFixture Fixture { get; }

    public ApiTests(TestServerFixture fixture)
    {
        Fixture = fixture;
    }
    
    [Fact]
    public async Task Is_fixture_healthy()
    {
        var healthResponse = await Fixture.CreateClient().GetStringAsync("health");
        Assert.Equal("Healthy", healthResponse);
    }
    
    [Fact]
    public async Task Can_roundtrip_request()
    {
        var order = new Order("Description", "valid-name", 13, new []{new OrderItem("Order-name", 1, 3000)}, new OrderDetail("order-address", "order-name"));
        var response = await Fixture.CreateClient().PostAsJsonAsync("order", order);
        var responseValue = await response.Content.ReadAsStringAsync();
        Assert.Equal("OK", responseValue);
    }
    
    [Fact]
    public async Task Validates_empty_value()
    {
        var emptyOrder = new Order(null, null!, 0, null, null!);
        var httpClient = Fixture.CreateClient();
        httpClient.DefaultRequestHeaders.Add("traceparent","00-d25ce4e91664e7ee080019c7b58f1545-48624c21ae7c93ac-00");
        var response = await httpClient.PostAsJsonAsync("order", emptyOrder);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Approvals.VerifyJson(FixTraceId(await response.Content.ReadAsStringAsync()));
    }

    private string FixTraceId(string responseJson)
    {
        var regex = @"(""traceId"":\s*""[\d\w]+-[\d\w]+-)([\d\w]+)(-[\d\w]+"")";
        return Regex.Replace(responseJson, regex, match => match.Groups[1].Value + "0000000000000000" + match.Groups[3].Value);
    }

    [Fact]
    public async Task Validates_invalid_values()
    {
        var longDescription =
            "01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";
        var invalidOrder = new Order(longDescription, "invalid-name", 25, new []{new OrderItem("", 1, 1000000m)},new OrderDetail("", null!));
        var httpClient = Fixture.CreateClient();
        httpClient.DefaultRequestHeaders.Add("traceparent","00-37be1758609afda059cc901e1ba893ec-15476cceff3adf10-00");
        var response = await httpClient.PostAsJsonAsync("order", invalidOrder);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        Approvals.VerifyJson(FixTraceId(await response.Content.ReadAsStringAsync()));
    }
}