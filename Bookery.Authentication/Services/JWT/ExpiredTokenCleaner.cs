namespace Bookery.Authentication.Services.JWT;

public class ExpiredTokenCleaner : IHostedService, IDisposable
{
    private readonly IJwtService _jwtService;
    private Timer? _timer;

    public ExpiredTokenCleaner(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(o => { _jwtService.ClearExpiredRefreshTokens(DateTime.UtcNow); }, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}