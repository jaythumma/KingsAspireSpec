using Microsoft.Extensions.Logging;

namespace KingsAspireSpec.Tests.Integration;

public class WebTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.KingsAspireSpec_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("webfrontend");
        await app.ResourceNotifications
            .WaitForResourceHealthyAsync("webfrontend", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetWeatherPageReturnsOkStatusCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.KingsAspireSpec_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Warning);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("webfrontend");
        await app.ResourceNotifications
            .WaitForResourceHealthyAsync("webfrontend", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var response = await httpClient.GetAsync("/weather", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
