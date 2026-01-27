using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.FileProviders;
using Npgsql;
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

            services.AddGrpc();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while configuring Middleware");
        }
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        //app.MapGrpcService<HResourceGrpcService>();
        
        app.UseDefaultFiles();
        app.UseStaticFiles();
        
        string[] folders = ["images"];
        foreach (var folder in folders)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(path),
                RequestPath = "/" + folder
            });
        }

        return app;
    }
}