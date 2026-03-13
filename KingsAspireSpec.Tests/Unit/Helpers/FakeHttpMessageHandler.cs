namespace KingsAspireSpec.Tests.Unit.Helpers;

internal sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    private int _callCount;

    public int CallCount => _callCount;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _callCount);
        return Task.FromResult(response);
    }
}
