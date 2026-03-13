using KingsAspireSpec.Web;

namespace KingsAspireSpec.Tests.Unit.CustomerService;

public class WeatherForecastTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void TemperatureF_Is32_WhenTemperatureCIs0()
    {
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), 0, "Clear");

        Assert.Equal(32, forecast.TemperatureF);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void TemperatureF_IsApproximately212_WhenTemperatureCIs100()
    {
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), 100, "Scorching");

        Assert.InRange(forecast.TemperatureF, 211, 213);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void TemperatureF_IsNegative40_WhenTemperatureCIsNegative40()
    {
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), -40, "Freezing");

        Assert.Equal(-39, forecast.TemperatureF);
    }
}
