using FastEndpoints;


namespace APIService.Endpoints;

using FastEndpoints;
using Microsoft.Extensions.Logging;

public class TestEndpoint : EndpointWithoutRequest
{
    private readonly ILogger<TestEndpoint> _logger;

    public TestEndpoint(ILogger<TestEndpoint> logger)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/test"); // يحدد المسار للـ Endpoint
        AllowAnonymous(); // يتيح الوصول بدون مصادقة
    }

    public override async Task HandleAsync(CancellationToken ct)
    {

        _logger.LogInformation("تم استدعاء /test في {Time}", DateTime.UtcNow);
        _logger.LogWarning("تم استدعاء /test في {Time}", DateTime.UtcNow);
        _logger.LogError("تم استدعاء /test في {Time}", DateTime.UtcNow);
        await SendAsync(new { message = "Hello, FastEndpoints!" }, cancellation: ct);
    }
}
