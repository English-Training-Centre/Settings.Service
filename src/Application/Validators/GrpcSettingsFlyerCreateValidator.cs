using FluentValidation;
using Libs.Core.Public.Protos.Settings.Service;

namespace Settings.Service.src.Application.Validators;

public sealed class GrpcSettingsFlyerCreateValidator : AbstractValidator<GrpcSettingsFlyerCreateRequest>
{
    public GrpcSettingsFlyerCreateValidator()
    {
        RuleFor(v => v.ImageUrl).NotEmpty();
        RuleFor(v => v.EnrolmentFee).NotEmpty().GreaterThan(0);

        RuleFor(v => v.OnSiteIntensiveA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteIntensiveAA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteIntensiveB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteIntensiveBB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteIntensiveC).NotEmpty().GreaterThan(0);

        RuleFor(v => v.OnSitePrivateA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSitePrivateAA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSitePrivateB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSitePrivateBB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSitePrivateC).NotEmpty().GreaterThan(0);

        RuleFor(v => v.OnSiteRegularA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteRegularAA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteRegularB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteRegularBB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnSiteRegularC).NotEmpty().GreaterThan(0);

        RuleFor(v => v.OnlineIntensiveA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineIntensiveAA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineIntensiveB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineIntensiveBB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineIntensiveC).NotEmpty().GreaterThan(0);

        RuleFor(v => v.OnlinePrivateA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlinePrivateAA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlinePrivateB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlinePrivateBB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlinePrivateC).NotEmpty().GreaterThan(0);
        
        RuleFor(v => v.OnlineRegularA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineRegularAA).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineRegularB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineRegularBB).NotEmpty().GreaterThan(0);
        RuleFor(v => v.OnlineRegularC).NotEmpty().GreaterThan(0);
    }
}