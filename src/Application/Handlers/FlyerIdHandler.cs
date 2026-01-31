using Libs.Core.Internal.src.Interfaces;
using Settings.Service.src.Application.Interfaces;

namespace Settings.Service.src.Application.Handlers;

public sealed class FlyerIdHandler (IFlyerRepository flyerRep, ILogger<SettingsHandler> logger) : ISettingFlyerIdGrpcService
{
    private readonly IFlyerRepository _flyerRep = flyerRep;
    private readonly ILogger<SettingsHandler> _logger = logger;

    public async Task<Guid> GetFlyerId(CancellationToken ct)
    {
        try
        {
            return await _flyerRep.GetFlyerId(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
            return Guid.Empty;
        }
    }
}