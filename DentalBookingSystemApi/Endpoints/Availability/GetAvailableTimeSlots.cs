namespace DentalBookingSystemApi.Endpoints.Availability;

public class GetAvailableTimeSlots : IEndpointMapper
{
    public record Request(int ClinicId, int TreatmentId, DateOnly Date);
    public record Response(List<TimeOnly> AvailableSlots);

    private static readonly TimeOnly DefaultOpen = new(8, 0);
    private static readonly TimeOnly DefaultClose = new(17, 0);

    public async Task<IResult> Handle(
        [AsParameters] Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        if (req.Date.DayOfWeek == DayOfWeek.Saturday ||
            req.Date.DayOfWeek == DayOfWeek.Sunday)
            return Results.Ok(new Response([]));

        var hours = await db.OpeningHours.FirstOrDefaultAsync(oh =>
            oh.ClinicId == req.ClinicId &&
            oh.DayOfWeek == req.Date.DayOfWeek, ct);

        var openTime = hours?.OpenTime ?? DefaultOpen;
        var closeTime = hours?.CloseTime ?? DefaultClose;

        var isFullyBlocked = await db.BlockedSlots.AnyAsync(bs =>
            bs.ClinicId == req.ClinicId &&
            bs.StartDate <= req.Date &&
            bs.EndDate >= req.Date &&
            bs.StartTime == null, ct);

        if (isFullyBlocked)
            return Results.Ok(new Response([]));

        var partialBlocks = await db.BlockedSlots
            .Where(bs =>
                bs.ClinicId == req.ClinicId &&
                bs.StartDate <= req.Date &&
                bs.EndDate >= req.Date &&
                bs.StartTime != null)
            .Select(bs => new { StartTime = bs.StartTime!.Value, EndTime = bs.EndTime!.Value })
            .ToListAsync(ct);

        var treatment = await db.Treatments.FindAsync([req.TreatmentId], ct);
        if (treatment is null)
            return Results.NotFound();

        var existingBookings = await db.Bookings
            .Where(b =>
                b.ClinicId == req.ClinicId &&
                b.Date == req.Date &&
                b.Status != BookingStatus.Cancelled)
            .Select(b => new { b.StartTime, b.EndTime })
            .ToListAsync(ct);

        var slots = new List<TimeOnly>();
        var current = openTime;

        while (current.AddMinutes(treatment.DurationMinutes) <= closeTime)
        {
            var slotEnd = current.AddMinutes(treatment.DurationMinutes);

            var bookedTaken = existingBookings.Any(b =>
                b.StartTime < slotEnd && b.EndTime > current);

            var blockTaken = partialBlocks.Any(p =>
                p.StartTime < slotEnd && p.EndTime > current);

            if (!bookedTaken && !blockTaken)
                slots.Add(current);

            current = current.AddMinutes(30);
        }

        return Results.Ok(new Response(slots));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/availability/slots", Handle)
           .WithName("GetAvailableTimeSlots")
           .WithTags("Availability");
    }
}