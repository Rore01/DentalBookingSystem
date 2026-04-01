namespace DentalBookingSystemApi.Endpoints.OpeningHours;

public class SetOpeningHours : IEndpointMapper
{
    public record DayRequest(
        DayOfWeek DayOfWeek,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        bool IsClosed
    );

    public record Request(int ClinicId, List<DayRequest> Days);

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        var existing = await db.OpeningHours
            .Where(o => o.ClinicId == req.ClinicId)
            .ToListAsync(ct);

        db.OpeningHours.RemoveRange(existing);

        var newHours = req.Days.Select(d => new Models.OpeningHours
        {
            ClinicId = req.ClinicId,
            DayOfWeek = d.DayOfWeek,
            OpenTime = d.OpenTime,
            CloseTime = d.CloseTime,
            IsClosed = d.IsClosed
        }).ToList();

        db.OpeningHours.AddRange(newHours);
        await db.SaveChangesAsync(ct);

        return Results.Ok("Opening hours updated.");
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/api/opening-hours", Handle)
           .WithName("SetOpeningHours")
           .WithTags("OpeningHours")
           .AllowAnonymous();
    }
}