using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Euphoric.FluentValidation.AspNetCore;

[UsesVerify]
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

        await Verifier.Verify(response).AddScrubber(ScrubTraceId).AddScrubber(EscapeWhitespace).IgnoreMember("Request");
    }

    private void EscapeWhitespace(StringBuilder sb)
    {
        sb.Replace("\r", "\\r");
        sb.Replace("\n", "\\n");
        sb.Replace("\t", "\\t");
    }
    
    private void ScrubTraceId(StringBuilder sb)
    {
        var text = sb.ToString();
        var regex = @"([\d\w]{2}-[\d\w]{32}-)([\d\w]{16})(-[\d\w]{2})";
        var fix = Regex.Replace(text, regex, match => match.Groups[1].Value + "0000000000000000" + match.Groups[3].Value);
        sb.Clear();
        sb.Append(fix);
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
        
        await Verifier.Verify(response).AddScrubber(ScrubTraceId).AddScrubber(EscapeWhitespace).IgnoreMember("Request");
    }
    
    [Fact]
    public async Task Handles_validation_exception()
    {
        var httpClient = Fixture.CreateClient();
        httpClient.DefaultRequestHeaders.Add("traceparent","00-37be1758609afda059cc901e1ba893ec-15476cceff3adf10-00");
        var response = await httpClient.GetAsync("order/error");

        await Verifier.Verify(response).AddScrubber(ScrubTraceId).AddScrubber(EscapeWhitespace).IgnoreMember("Request");
    }
    
    [Fact]
    public async Task Get_with_route_parameter()
    {
        var httpClient = Fixture.CreateClient();
        
        var response = await httpClient.GetStringAsync("order/356c5592-efe7-4e4d-ac12-0d5329978439");
        Assert.Equal("356c5592-efe7-4e4d-ac12-0d5329978439", response);
    }
    
    [Fact]
    public async Task Get_with_empty_route_parameter()
    {
        var httpClient = Fixture.CreateClient();
        
        var response = await httpClient.GetStringAsync("order/abcd");
        Assert.Equal("356c5592-efe7-4e4d-ac12-0d5329978439", response);
    }
    
    [Fact]
    public async Task OpenApi_schema_specification()
    {
        var response = await Fixture.CreateClient().GetAsync("swagger/v1/swagger.json");

        await Verifier.Verify(response).IgnoreMember("Request").UniqueForTargetFrameworkAndVersion();
    }
}