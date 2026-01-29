using FluentValidation;
using Settings.Service.src.Application.DTOs.Requests;

namespace Settings.Service.src.Application.Validators;

public sealed class SaveFlyerValidator : AbstractValidator<FlyerRequest>
{
    public SaveFlyerValidator()
    {
        RuleFor(v => v.ImageUrl).NotEmpty();
        RuleFor(v => v.EnrolmentFee).NotEmpty().GreaterThan(0);
    }
}