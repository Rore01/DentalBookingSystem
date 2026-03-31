namespace DentalBookingSystemApi.Endpoints.OpeningHours;

public class GetOpeningHours : IEndpointMapper
{
    public record HoursDto(
        int Id,
        DayOfWeek DayOfWeek,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        bool IsClosed
    );

    public async Task<IResult> Handle(
        int clinicId,
        AppDbContext db,
        CancellationToken ct)
    {
        var hours = await db.OpeningHours
            .Where(o => o.ClinicId == clinicId)
            .OrderBy(o => o.DayOfWeek)
            .Select(o => new HoursDto(o.Id, o.DayOfWeek, o.OpenTime, o.CloseTime, o.IsClosed))
            .ToListAsync(ct);

        return Results.Ok(hours);
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/opening-hours", Handle)
           .WithName("GetOpeningHours")
           .WithTags("OpeningHours");
    }
}