using Microsoft.Extensions.Options;

namespace FawxGameDigBot;

public class Worker : BackgroundService {
    
    private readonly ILogger<Worker> _logger;
    private IOptions<AppSettings> _settings;

    public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings) {
        _logger = logger;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await Task.Delay(1000, stoppingToken);
        }
    }
}