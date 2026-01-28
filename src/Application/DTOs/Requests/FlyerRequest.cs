namespace Settings.Service.src.Application.DTOs.Requests;

public sealed record FlyerRequest
(
    string ImageUrl,
    long EnrolmentFee
);