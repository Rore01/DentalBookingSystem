namespace DentalBookingSystemApi.Endpoints.BlockedSlots;

public class GetBlockedSlots : IEndpointMapper
{
    public record SlotDto(
        int Id,
        DateOnly StartDate,
        DateOnly EndDate,
        TimeOnly? StartTime,
        TimeOnly? EndTime,
        string Reason
    );

    public async Task<IResult> Handle(
        int clinicId,
        AppDbContext db,
        CancellationToken ct)
    {
        var slots = await db.BlockedSlots
            .Where(bs => bs.ClinicId == clinicId)
            .OrderBy(bs => bs.StartDate)
            .Select(bs => new SlotDto(bs.Id, bs.StartDate, bs.EndDate, bs.StartTime, bs.EndTime, bs.Reason))
            .ToListAsync(ct);

        return Results.Ok(slots);
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/blocked-slots", Handle)
           .WithName("GetBlockedSlots")
           .WithTags("BlockedSlots")
           .AllowAnonymous();
    }
}