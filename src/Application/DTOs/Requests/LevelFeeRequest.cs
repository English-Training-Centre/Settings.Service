namespace Settings.Service.src.Application.DTOs.Requests;

public sealed record LevelFeeRequest
(
    // On-Site
    long OnSiteIntensiveA,
    long OnSiteIntensiveAA,
    long OnSiteIntensiveB,
    long OnSiteIntensiveBB,
    long OnSiteIntensiveC,

    long OnSitePrivateA,
    long OnSitePrivateAA,
    long OnSitePrivateB,
    long OnSitePrivateBB,
    long OnSitePrivateC,

    long OnSiteRegularA,
    long OnSiteRegularAA,
    long OnSiteRegularB,
    long OnSiteRegularBB,
    long OnSiteRegularC,

    // Online   
    long OnlineIntensiveA,
    long OnlineIntensiveAA,
    long OnlineIntensiveB,
    long OnlineIntensiveBB,
    long OnlineIntensiveC,

    long OnlinePrivateA,
    long OnlinePrivateAA,
    long OnlinePrivateB,
    long OnlinePrivateBB,
    long OnlinePrivateC,

    long OnlineRegularA,
    long OnlineRegularAA,
    long OnlineRegularB,
    long OnlineRegularBB,
    long OnlineRegularC
);