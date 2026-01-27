using Libs.Core.Public.src.DTOs.Requests;
using Libs.Core.Public.src.DTOs.Responses;
using Libs.Core.Public.src.Interfaces;
using Libs.Core.Shared.src.DTOs.Responses;
using Settings.Service.src.Application.DTOs.Requests;
using Settings.Service.src.Application.Interfaces;

namespace Settings.Service.src.Application.Handlers;

public sealed class SettingsHandler(IFlyerRepository flyerRep, ILogger<SettingsHandler> logger) : ISettingGrpcService
{
    private readonly IFlyerRepository _flyerRep = flyerRep;
    private readonly ILogger<SettingsHandler> _logger = logger;

    public async Task<ResponseDTO> CreateFlyer(SettingsFlyerCreateRequest request, CancellationToken ct)
    {
        var flyerRequest = new FlyerRequest
        (
            request.Image,
            request.EnrolmentFee
        );

        var levelFeeRequest = new LevelFeeRequest
        (
            request.OnSiteIntensiveA,
            request.OnSiteIntensiveAA,
            request.OnSiteIntensiveB,
            request.OnSiteIntensiveBB,
            request.OnSiteIntensiveC,

            request.OnSitePrivateA,
            request.OnSitePrivateAA,
            request.OnSitePrivateB,
            request.OnSitePrivateBB,
            request.OnSitePrivateC,

            request.OnSiteRegularA,
            request.OnSiteRegularAA,
            request.OnSiteRegularB,
            request.OnSiteRegularBB,
            request.OnSiteRegularC,

            request.OnlineIntensiveA,
            request.OnlineIntensiveAA,
            request.OnlineIntensiveB,
            request.OnlineIntensiveBB,
            request.OnlineIntensiveC,

            request.OnlinePrivateA,
            request.OnlinePrivateAA,
            request.OnlinePrivateB,
            request.OnlinePrivateBB,
            request.OnlinePrivateC,

            request.OnlineRegularA,
            request.OnlineRegularAA,
            request.OnlineRegularB,
            request.OnlineRegularBB,
            request.OnlineRegularC
        );

        try
        {
            Guid flyerId = await _flyerRep.SaveFlyer(flyerRequest, ct);
            List<Guid> levelFeeIds = await _flyerRep.SaveLevelFee(levelFeeRequest, ct);

            if (flyerId.Equals(Guid.Empty))
            {
                return new ResponseDTO { IsSuccess = false, Message = "Failed to create flyer." };
            }
            else if (levelFeeIds.Count != 6)
            {
                return new ResponseDTO { IsSuccess = false, Message = "Failed to create level fees." };
            }
            else
            {
                var monthlyTuitionListRequest = new[]
                {
                    (LevelFeeId: 0, Package: "intensive", Modality: "on-site"),
                    (LevelFeeId: 1, Package: "private",   Modality: "on-site"),
                    (LevelFeeId: 2, Package: "regular",   Modality: "on-site"),
                    (LevelFeeId: 3, Package: "intensive", Modality: "online"),
                    (LevelFeeId: 4, Package: "private",   Modality: "online"),
                    (LevelFeeId: 5, Package: "regular",   Modality: "online")
                };

                foreach (var list in monthlyTuitionListRequest)
                {
                    var monthlyTuitionRequest = new MonthlyTuitionRequest
                    (
                        flyerId,
                        levelFeeIds[list.LevelFeeId],
                        list.Package,
                        list.Modality
                    );

                    await _flyerRep.SaveMonthlyTuition(monthlyTuitionRequest, ct);
                }

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Flyer created."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - An unexpected error occurred...");
            return new ResponseDTO { IsSuccess = false, Message = "An unexpected error occurred..." };
        }
    }

    public async Task<IReadOnlyList<SettingsFlyerCreateResponse>> GetAllFlyer(CancellationToken ct)
    {
        try
        {
            return await _flyerRep.GetAllFlyer(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - An unexpected error occurred...");
            return [];
        }
    }
}