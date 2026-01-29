using Libs.Core.Public.src.DTOs.Responses;
using Settings.Service.src.Application.DTOs.Requests;

namespace Settings.Service.src.Application.Interfaces;

public interface IFlyerRepository
{
    Task<Guid> SaveFlyerAsync(FlyerRequest request, CancellationToken ct);
    Task<List<Guid>> SaveLevelFeeAsync(LevelFeeRequest request, CancellationToken ct);
    Task SaveMonthlyTuitionAsync(MonthlyTuitionRequest request, CancellationToken ct);
    Task<IReadOnlyList<SettingsFlyerCreateResponse>> GetAllFlyerAsync(CancellationToken ct);
}