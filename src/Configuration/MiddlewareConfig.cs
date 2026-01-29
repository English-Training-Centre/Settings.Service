using FluentValidation;
using FluentValidation.AspNetCore;
using Libs.Core.Public.src.Interfaces;
using Npgsql;
using Settings.Service.Services;
using Settings.Service.src.Application.Handlers;
using Settings.Service.src.Application.Interfaces;
using Settings.Service.src.Infrastructure.Persistence;
using Settings.Service.src.Infrastructure.Repositories;

namespace Settings.Service.src.Configuration;

public static class MiddlewareConfig
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        var logger = LoggerFactory.Create(s => s.AddConsole()).CreateLogger<Program>();
        try
        {
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            services.AddFluentValidationAutoValidation();

            services.AddSingleton(nd =>
            {
                var builder = new NpgsqlDataSourceBuilder(
                    PostgresDB.BuildConnectionStringFromEnvironment()
                );
                builder.UseLoggerFactory(nd.GetRequiredService<ILoggerFactory>());
                return builder.Build();
            });

            services.AddHealthChecks()
                    .AddNpgSql(nd => nd.GetRequiredService<NpgsqlDataSource>());

            services.AddScoped<IPostgresDB, PostgresDB>();
            services.AddScoped<IFlyerRepository, FlyerRepository>();
            services.AddScoped<ISettingGrpcService, SettingsHandler>();

            services.AddGrpc();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while configuring Middleware");
        }
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.MapGrpcService<SettingsGrpcService>();

        return app;
    }
}