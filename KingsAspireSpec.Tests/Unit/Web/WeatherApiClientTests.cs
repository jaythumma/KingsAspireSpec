using System.Text;
using System.Text.Json;
using KingsAspireSpec.Tests.Unit.Helpers;
using KingsAspireSpec.Web;

namespace KingsAspireSpec.Tests.Unit.Web;

public class WeatherApiClientTests
{
    private static WeatherApiClient BuildClient(string json)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        return new WeatherApiClient(httpClient);
    }

    private static string Serialize(WeatherForecast[] forecasts) =>
        JsonSerializer.Serialize(forecasts);

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetWeatherAsync_ReturnsAllForecasts_WhenCountBelowMaxItems()
    {
        var forecasts = new[]
        {
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), 15, "Cool"),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 20, "Mild"),
        };
        var client = BuildClient(Serialize(forecasts));

        var result = await client.GetWeatherAsync(maxItems: 10, CancellationToken.None);

        Assert.Equal(2, result.Length);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetWeatherAsync_RespectsMaxItemsLimit()
    {
        var forecasts = Enumerable.Range(1, 10)
            .Select(i => new WeatherForecast(DateOnly.FromDateTime(DateTime.Today.AddDays(i)), i * 2, "Warm"))
            .ToArray();
        var client = BuildClient(Serialize(forecasts));

        var result = await client.GetWeatherAsync(maxItems: 3, CancellationToken.None);

        Assert.Equal(3, result.Length);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetWeatherAsync_ReturnsEmptyArray_WhenApiReturnsEmptyArray()
    {
        var client = BuildClient("[]");

        var result = await client.GetWeatherAsync(cancellationToken: CancellationToken.None);

        Assert.Empty(result);
        Assert.NotNull(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetWeatherAsync_ReturnsCorrectForecastValues()
    {
        var expected = new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), 25, "Warm");
        var client = BuildClient(Serialize([expected]));

        var result = await client.GetWeatherAsync(cancellationToken: CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(expected.Date, result[0].Date);
        Assert.Equal(expected.TemperatureC, result[0].TemperatureC);
        Assert.Equal(expected.Summary, result[0].Summary);
        Assert.Equal(expected.TemperatureF, result[0].TemperatureF);
    }
}
