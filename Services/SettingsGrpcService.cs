using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Libs.Core.Public.Protos.Settings.Service;
using Libs.Core.Public.src.DTOs.Requests;
using Libs.Core.Public.src.Interfaces;

namespace Settings.Service.Services;

public sealed class SettingsGrpcService (ISettingGrpcService settingHandler, ILogger<SettingsGrpcService> logger) : SettingsGrpc.SettingsGrpcBase
{
    private readonly ISettingGrpcService _settingHandler = settingHandler;
    private readonly ILogger<SettingsGrpcService> _logger = logger;

    public async override Task<GrpcSettingsResponseDTO> CreateFlyer(GrpcSettingsFlyerCreateRequest request, ServerCallContext context)
    {
        var parameter = new SettingsFlyerCreateRequest
        (
            request.ImageUrl,
            request.EnrolmentFee,

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
            var result = await _settingHandler.CreateFlyerAsync(parameter, context.CancellationToken);

            var protoResponse = new GrpcSettingsResponseDTO
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message
            };

            return protoResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: SettingsGrpcService -> CreateFlyer(....)");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to create flyer"));
        }
    }

    public async override Task<GrpcSettingsFlyerGetAllResponse> GetAllFlyer(Empty request, ServerCallContext context)
    {
        try
        {
            var flyers = await _settingHandler.GetAllFlyerAsync(context.CancellationToken);

            var protoResponse = new GrpcSettingsFlyerGetAllResponse();

        // Map each flyer to the gRPC response format
        foreach (var flyer in flyers)
        {
            var flyerResponse = new GrpcSettingsFlyerGetResponse
            {
                Id = flyer.Id.ToString(),
                ImageUrl = flyer.ImageUrl ?? "",
                EnrolmentFee = flyer.EnrolmentFee,
                IsActive = flyer.IsActive,
                CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(flyer.CreatedAt.ToUniversalTime()),
            };

            flyerResponse.MonthlyTuitions.AddRange(
                flyer.MonthlyTuitions.Select(m => new GrpcSettingsFlyerMonthlyTuitionResponse
                {
                    Package = m.Package ?? "",
                    Modality = m.Modality ?? "",
                    LevelA = m.LevelA,
                    LevelAA = m.LevelAA,
                    LevelB = m.LevelB,
                    LevelBB = m.LevelBB,
                    LevelC = m.LevelC
                })
            );

            protoResponse.Flyers.Add(flyerResponse);
        }

        return protoResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SettingsGrpcService.GetAllFlyer()");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to retrieve flyer settings"));
        }
    }
}