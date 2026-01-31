using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Libs.Core.Internal.Protos.SettingService;
using Libs.Core.Internal.src.Interfaces;

namespace Settings.Service.Services;

public sealed class SettingsFlyerIdGrpcService (ISettingFlyerIdGrpcService flyserIdHandler, ILogger<SettingsFlyerIdGrpcService> logger) : SettingFlyerIdGrpc.SettingFlyerIdGrpcBase
{
    private readonly ISettingFlyerIdGrpcService _flyserIdHandler = flyserIdHandler;
    private readonly ILogger<SettingsFlyerIdGrpcService> _logger = logger;

    public async override Task<GrpcSettingsFlyerIdResponse> GetFlyerId(Empty request, ServerCallContext context)
    {
        try
        {
            var flyerId = await _flyserIdHandler.GetFlyerId(context.CancellationToken);

            var protoResponse = new GrpcSettingsFlyerIdResponse
            {
                Id = flyerId.ToString() ?? string.Empty
            };

            return protoResponse;            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SettingsFlyerIdGrpcService.GetFlyerId()");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to retrieve flyer settings"));
        }
    }
}