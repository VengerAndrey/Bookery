using Bookery.Authentication.Services.Interfaces;

namespace Bookery.Authentication.Services.Hosted;

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
        _timer = new Timer(o =>
            {
                _jwtService.ClearExpiredRefreshTokens(DateTime.UtcNow);
            }, 
            state: null, 
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(dueTime: Timeout.Infinite, period: 0);
        return Task.CompletedTask;
    }
}