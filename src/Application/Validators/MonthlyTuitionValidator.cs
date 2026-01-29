using System.Data;
using FluentValidation;
using Settings.Service.src.Application.DTOs.Requests;

namespace Settings.Service.src.Application.Validators;

public sealed class MonthlyTuitionValidator : AbstractValidator<MonthlyTuitionRequest>
{
    public MonthlyTuitionValidator()
    {
        RuleFor(v => v.FlyerId).NotEmpty();
        RuleFor(v => v.LevelFeeId).NotEmpty();
        RuleFor(v => v.Package).NotEmpty();
        RuleFor(v => v.Modality).NotEmpty();
    }
}