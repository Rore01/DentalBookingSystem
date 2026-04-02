namespace DentalBookingSystemApi.Endpoints.BlockedSlots;

public class CreateBlockedSlot : IEndpointMapper
{
    public record Request(
        int ClinicId,
        DateOnly Date,
        TimeOnly? StartTime,
        TimeOnly? EndTime,
        string? Reason
    );

    public record Response(
        int Id,
        DateOnly StartDate,
        DateOnly EndDate,
        TimeOnly? StartTime,
        TimeOnly? EndTime,
        string Reason
    );

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        if (req.StartTime == null)
        {
            var bookingCount = await db.Bookings.CountAsync(b =>
                b.ClinicId == req.ClinicId &&
                b.Date == req.Date &&
                b.Status != BookingStatus.Cancelled, ct);

            if (bookingCount > 0)
                return Results.Conflict(
                    $"There are {bookingCount} active booking(s) on this date. " +
                    $"Please reschedule or cancel them before blocking this day.");

            var allDayExists = await db.BlockedSlots.AnyAsync(bs =>
                bs.ClinicId == req.ClinicId &&
                bs.StartDate == req.Date &&
                bs.StartTime == null, ct);

            if (allDayExists)
                return Results.Conflict("This date is already fully blocked.");
        }
        else
        {
            var overlappingCount = await db.Bookings.CountAsync(b =>
                b.ClinicId == req.ClinicId &&
                b.Date == req.Date &&
                b.Status != BookingStatus.Cancelled &&
                b.StartTime < req.EndTime &&
                b.EndTime > req.StartTime, ct);

            if (overlappingCount > 0)
                return Results.Conflict(
                    $"There are {overlappingCount} active booking(s) during this time. " +
                    $"Please reschedule or cancel them before blocking this slot.");

            var fullDayExists = await db.BlockedSlots.AnyAsync(bs =>
                bs.ClinicId == req.ClinicId &&
                bs.StartDate == req.Date &&
                bs.StartTime == null, ct);

            if (fullDayExists)
                return Results.Conflict("This date is already fully blocked.");
        }

        var slot = new BlockedSlot
        {
            ClinicId = req.ClinicId,
            StartDate = req.Date,
            EndDate = req.Date,
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            Reason = req.Reason ?? string.Empty
        };

        db.BlockedSlots.Add(slot);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/blocked-slots/{slot.Id}",
            new Response(slot.Id, slot.StartDate, slot.EndDate, slot.StartTime, slot.EndTime, slot.Reason));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/blocked-slots", Handle)
           .WithName("CreateBlockedSlot")
           .WithTags("BlockedSlots")
           .AllowAnonymous();
    }
}