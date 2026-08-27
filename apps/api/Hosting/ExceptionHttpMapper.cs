using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BangaloreTaxi.Api.Hosting;

public readonly record struct MappedError(int Status, string Title, string Detail, int? RetryAfterSeconds = null);

/// <summary>
/// Maps exceptions to safe Problem Details fields. Never includes stack traces or connection strings.
/// </summary>
public static class ExceptionHttpMapper
{
    public static MappedError Map(Exception exception)
    {
        if (exception is Application.InvalidRequestException invalid)
        {
            return new MappedError(StatusCodes.Status400BadRequest, "Invalid request", invalid.Message);
        }

        if (exception is Application.UnauthorizedException unauthorized)
        {
            return new MappedError(StatusCodes.Status401Unauthorized, "Unauthorized", unauthorized.Message);
        }

        if (exception is Application.TooManyRequestsException tooMany)
        {
            return new MappedError(
                StatusCodes.Status429TooManyRequests,
                "Too Many Requests",
                tooMany.Message,
                tooMany.RetryAfterSeconds);
        }

        if (exception is Application.ServiceUnavailableException unavailable)
        {
            return new MappedError(StatusCodes.Status503ServiceUnavailable, "Service Unavailable", unavailable.Message);
        }

        if (exception is Application.ConflictException conflict)
        {
            return new MappedError(StatusCodes.Status409Conflict, "Conflict", conflict.Message);
        }

        if (exception is Application.NotFoundException notFound)
        {
            return new MappedError(StatusCodes.Status404NotFound, "Not Found", notFound.Message);
        }

        if (exception is DbUpdateException dbUpdate && dbUpdate.InnerException is PostgresException postgres)
        {
            return MapPostgres(postgres);
        }

        if (exception is PostgresException postgresDirect)
        {
            return MapPostgres(postgresDirect);
        }

        return new MappedError(
            StatusCodes.Status500InternalServerError,
            "An error occurred.",
            "An unexpected error occurred. Use the trace identifier when contacting support.");
    }

    private static MappedError MapPostgres(PostgresException postgres)
    {
        return postgres.SqlState switch
        {
            PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ExclusionViolation =>
                new MappedError(
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    "The request conflicts with existing data."),
            PostgresErrorCodes.ForeignKeyViolation =>
                new MappedError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request",
                    "The request referenced data that does not exist."),
            PostgresErrorCodes.CheckViolation =>
                new MappedError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request",
                    "The request failed a data integrity check."),
            _ => new MappedError(
                StatusCodes.Status500InternalServerError,
                "An error occurred.",
                "An unexpected error occurred. Use the trace identifier when contacting support.")
        };
    }
}
