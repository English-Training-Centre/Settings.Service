namespace Settings.Service.src.Application.DTOs.Requests;

public sealed record MonthlyTuitionRequest
(
    Guid FlyerId,
    Guid LevelFeeId,
    string Package,
    string Modality
);