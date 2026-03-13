using Aspire.Hosting;
using Microsoft.Extensions.Logging;

namespace KingsAspireSpec.Tests.Integration;

public sealed class AspireHostFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2);
    private DistributedApplication? _app;

    public DistributedApplication App => _app ?? throw new InvalidOperationException("Aspire app not initialized.");

    public async ValueTask InitializeAsync()
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.KingsAspireSpec_AppHost>(cts.Token);
        appHost.Services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        appHost.Services.ConfigureHttpClientDefaults(c => c.AddStandardResilienceHandler());

        _app = await appHost.BuildAsync(cts.Token);
        await _app.StartAsync(cts.Token);

        await _app.ResourceNotifications
            .WaitForResourceHealthyAsync("customerservice", cts.Token)
            .WaitAsync(DefaultTimeout, cts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
            await _app.DisposeAsync();
    }
}
