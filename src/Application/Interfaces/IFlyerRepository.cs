using Settings.Service.src.Application.DTOs.Requests;

namespace Settings.Service.src.Application.Interfaces;

public interface IFlyerRepository
{
    Task<Guid> SaveFlyer(FlyerRequest request, CancellationToken ct);
    Task<List<Guid>> SaveLevelFee(LevelFeeRequest request, CancellationToken ct);
    Task SaveMonthlyTuition(MonthlyTuitionRequest request, CancellationToken ct);
}