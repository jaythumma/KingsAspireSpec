using System.Net.Http.Json;
using System.Text.Json;
using KingsAspireSpec.Web;

namespace KingsAspireSpec.Tests.Integration;

public class CustomerServiceTests(AspireHostFixture fixture) : IClassFixture<AspireHostFixture>
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    private static readonly JsonSerializerOptions WebOptions = new(JsonSerializerDefaults.Web);

    private HttpClient CreateCustomerServiceClient() =>
        fixture.App.CreateHttpClient("customerservice");

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetRoot_ReturnsOk_WithApiRunningMessage()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var httpClient = CreateCustomerServiceClient();

        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("customerservice", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("API service is running", body);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetWeatherForecast_ReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var httpClient = CreateCustomerServiceClient();

        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("customerservice", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/weatherforecast", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetWeatherForecast_ReturnsNonEmptyArray()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var httpClient = CreateCustomerServiceClient();

        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("customerservice", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/weatherforecast", cancellationToken);
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>(WebOptions, cancellationToken);

        Assert.NotNull(forecasts);
        Assert.NotEmpty(forecasts);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetWeatherForecast_EachElementHasValidStructure()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var httpClient = CreateCustomerServiceClient();

        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("customerservice", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/weatherforecast", cancellationToken);
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>(WebOptions, cancellationToken);

        Assert.NotNull(forecasts);
        foreach (var f in forecasts)
        {
            Assert.NotEqual(default, f.Date);
            Assert.False(string.IsNullOrEmpty(f.Summary));
            Assert.InRange(f.TemperatureC, -20, 55);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetWeatherForecast_TemperatureFIsConsistentWithTemperatureC()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var httpClient = CreateCustomerServiceClient();

        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("customerservice", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/weatherforecast", cancellationToken);
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>(WebOptions, cancellationToken);

        Assert.NotNull(forecasts);
        foreach (var f in forecasts)
        {
            var expected = 32 + (int)(f.TemperatureC / 0.5556);
            Assert.Equal(expected, f.TemperatureF);
        }
    }
}
