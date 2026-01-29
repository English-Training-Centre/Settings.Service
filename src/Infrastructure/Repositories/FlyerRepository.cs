using Libs.Core.Public.src.DTOs.Responses;
using Npgsql;
using Settings.Service.src.Application.DTOs.Requests;
using Settings.Service.src.Application.Interfaces;

namespace Settings.Service.src.Infrastructure.Repositories;

public sealed class FlyerRepository(IPostgresDB db, ILogger<FlyerRepository> logger) : IFlyerRepository
{
    private readonly IPostgresDB _db = db;    
    private readonly ILogger<FlyerRepository> _logger = logger;

    public async Task<Guid> SaveFlyerAsync(FlyerRequest request, CancellationToken ct = default)
    {
        const string sqlDeactivate = @"
            UPDATE tbFlyer
            SET is_active = false;
        ";

        const string sqlInsert = @"
            INSERT INTO tbFlyer (image_url, enrolment_fee, is_active)
            VALUES (@ImageUrl, @EnrolmentFee, true)
            RETURNING id;
        ";

        try
        {            
            return await _db.ExecuteInTransactionAsync(
                async (conn, tx) =>
                {
                    // Step 1: Deactivate all existing flyers
                    await using var deactivateCmd = new NpgsqlCommand(sqlDeactivate, conn, tx);
                    await deactivateCmd.ExecuteNonQueryAsync(ct);

                    // Step 2: Insert the new active flyer
                    await using var insertCmd = new NpgsqlCommand(sqlInsert, conn, tx);
                    
                    insertCmd.Parameters.AddWithValue("@ImageUrl", request.ImageUrl);
                    
                    insertCmd.Parameters.AddWithValue("@EnrolmentFee", request.EnrolmentFee);

                    var scalarResult = await insertCmd.ExecuteScalarAsync(ct);

                    // Safely handle possible null from RETURNING (shouldn't happen on successful insert)
                    return scalarResult is Guid id ? id : Guid.Empty;
                },
                ct);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error.");
            return Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
            return Guid.Empty;
        }
    }

    public async Task<List<Guid>> SaveLevelFeeAsync(LevelFeeRequest request, CancellationToken ct)
    {
        const string sqlInsert = @"
            INSERT INTO tbFlyerLevelFee (level_a, level_aa, level_b, level_bb, level_c)
            VALUES (@LevelA, @LevelAA, @LevelB, @LevelBB, @LevelC)
            RETURNING id;
        ";

        try
        {
            return await _db.ExecuteInTransactionAsync(async (conn, tx) =>
            {
                var insertedIds = new List<Guid>();

                var feeSets = new[]
                {
                    (request.OnSiteIntensiveA, request.OnSiteIntensiveAA, request.OnSiteIntensiveB, request.OnSiteIntensiveBB, request.OnSiteIntensiveC),

                    (request.OnSitePrivateA, request.OnSitePrivateAA, request.OnSitePrivateB, request.OnSitePrivateBB, request.OnSitePrivateC),

                    (request.OnSiteRegularA, request.OnSiteRegularAA, request.OnSiteRegularB, request.OnSiteRegularBB, request.OnSiteRegularC),

                    (request.OnlineIntensiveA, request.OnlineIntensiveAA, request.OnlineIntensiveB, request.OnlineIntensiveBB, request.OnlineIntensiveC),

                    (request.OnlinePrivateA, request.OnlinePrivateAA, request.OnlinePrivateB, request.OnlinePrivateBB, request.OnlinePrivateC),

                    (request.OnlineRegularA, request.OnlineRegularAA, request.OnlineRegularB, request.OnlineRegularBB, request.OnlineRegularC)
                };

                foreach (var fees in feeSets)
                {
                    await using var cmd = new NpgsqlCommand(sqlInsert, conn, tx);
                    cmd.Parameters.AddWithValue("@LevelA", fees.Item1);
                    cmd.Parameters.AddWithValue("@LevelAA", fees.Item2);
                    cmd.Parameters.AddWithValue("@LevelB", fees.Item3);
                    cmd.Parameters.AddWithValue("@LevelBB", fees.Item4);
                    cmd.Parameters.AddWithValue("@LevelC", fees.Item5);

                    var result = await cmd.ExecuteScalarAsync(ct);

                    if (result is Guid id)
                    {
                        insertedIds.Add(id);
                    }
                    else
                    {
                        throw new InvalidOperationException("Falha ao obter o ID da inserção.");
                    }
                }

                return insertedIds;
            }, ct);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error.");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - An unexpected error occurred...");
            return [];
        }
    }
    
    public async Task SaveMonthlyTuitionAsync(MonthlyTuitionRequest request, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO tbFlyerMonthlyTuition (flyer_id, level_fee_id, package, modality)
            VALUES (@FlyerId, @LevelFeeId, @Package::package_enum, @Modality::modality_enum)";

        try
        {
            await _db.ExecuteAsync(sql, request, ct);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
        }
    }

    public async Task<IReadOnlyList<SettingsFlyerCreateResponse>> GetAllFlyerAsync(CancellationToken ct)
    {
        const string sql = @"
            SELECT
                f.id AS Id,
                f.image_url AS ImageUrl,
                f.enrolment_fee AS EnrolmentFee,
                f.is_active AS IsActive,
                f.created_at AS CreatedAt,

                jsonb_agg(
                    jsonb_build_object(
                        'package', mt.package,
                        'modality', mt.modality,
                        'levelA', COALESCE(lf.level_a, 0)::bigint,
                        'levelAA', COALESCE(lf.level_aa, 0)::bigint,
                        'levelB', COALESCE(lf.level_b, 0)::bigint,
                        'levelBB', COALESCE(lf.level_bb, 0)::bigint,
                        'levelC', COALESCE(lf.level_c, 0)::bigint
                    )
                ) AS MonthlyTuitions

            FROM tbFlyer f
            JOIN tbFlyerMonthlyTuition mt
                ON mt.flyer_id = f.id
            JOIN tbFlyerLevelFee lf
                ON lf.id = mt.level_fee_id
            GROUP BY
                f.id,
                f.image_url,
                f.enrolment_fee,
                f.is_active,
                f.created_at
            ORDER BY f.created_at DESC;
            ";

        try
        {
            var flyers = await _db.QueryWithJsonListAsync<SettingsFlyerCreateResponse>(
                sql,
                (flyer, json) =>
                {
                    flyer.MonthlyTuitions = string.IsNullOrWhiteSpace(json)
                        ? []
                        : System.Text.Json.JsonSerializer.Deserialize<List<MonthlyTuitionResponse>>(json)!;

                    return flyer;
                },
                splitOnColumn: "MonthlyTuitions",
                cancellationToken: ct
            );

            return flyers;
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, " - Unexpected PostgreSQL Error");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " - Unexpected error during transaction operation.");
            return [];
        }
    }
}