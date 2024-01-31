using System.Text.Json;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Options;

namespace FawxGameDigBot;

public sealed class GameDig {
    
    private readonly ILogger<GameDig> _logger;
    private IOptions<AppSettings> _settings;

    public GameDig(ILogger<GameDig> logger, IOptions<AppSettings> settings) {
        _logger = logger;
        _settings = settings;
    }

    public async Task<DigResponse> Dig(GameServer server) {
        _logger.LogDebug($"Digging server: {server.Name} at {server.ConnectionString()}");

        DigResponse? response;

        var shellResult = await Cli.Wrap("gamedig")
            .WithArguments($"--type {server.GameType} {server.ConnectionString()}")
            .ExecuteBufferedAsync();
        
        _logger.LogDebug(($"Dig output: '{shellResult.StandardOutput.Trim()}'"));

        if (shellResult.ExitCode != 0 || shellResult.StandardOutput.Contains("{\"error\":")) {
            _logger.LogWarning($"Failed to contact server '{server.Name}'!");
            _logger.LogDebug(shellResult.StandardError.Trim());
            response = new DigResponse { online = false };
        } else {
            response = server.GameType switch {
                GameTypes.Palworld => JsonSerializer.Deserialize<PalworldDigResponse>(shellResult.StandardOutput),
                _ => JsonSerializer.Deserialize<DigResponse>(shellResult.StandardOutput)
            };

            if (response != null) {
                response.ParseRaw(_logger);
                response.online = true;
            }
        }
        return response ?? new DigResponse { online = false };
    }
}