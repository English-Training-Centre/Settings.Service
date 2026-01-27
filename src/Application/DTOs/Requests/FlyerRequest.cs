namespace Settings.Service.src.Application.DTOs.Requests;

public sealed record FlyerRequest
(
    IFormFile Image,
    long EnrolmentFee
);