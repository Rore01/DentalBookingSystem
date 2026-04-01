namespace DentalBookingSystemApi.Endpoints.Availability;

public class GetAvailableDates : IEndpointMapper
{
    public record Request(int ClinicId, int TreatmentId, int Year, int Month);
    public record Response(List<DateOnly> AvailableDates);

    private static readonly TimeOnly DefaultOpen = new(8, 0);
    private static readonly TimeOnly DefaultClose = new(17, 0);

    public async Task<IResult> Handle(
        [AsParameters] Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        var treatment = await db.Treatments.FindAsync([req.TreatmentId], ct);
        if (treatment is null)
            return Results.NotFound("Treatment not found.");

        var openingHours = await db.OpeningHours
            .Where(o => o.ClinicId == req.ClinicId)
            .ToListAsync(ct);

        var firstDay = new DateOnly(req.Year, req.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var blockedSlots = await db.BlockedSlots
            .Where(bs =>
                bs.ClinicId == req.ClinicId &&
                bs.StartDate <= lastDay &&
                bs.EndDate >= firstDay)
            .ToListAsync(ct);

        var bookings = await db.Bookings
            .Where(b =>
                b.ClinicId == req.ClinicId &&
                b.Date >= firstDay &&
                b.Date <= lastDay &&
                b.Status != BookingStatus.Cancelled)
            .Select(b => new { b.Date, b.StartTime, b.EndTime })
            .ToListAsync(ct);

        var availableDates = new List<DateOnly>();
        var today = DateOnly.FromDateTime(DateTime.Today);

        for (var date = firstDay; date <= lastDay; date = date.AddDays(1))
        {
            if (date < today) continue;

            if (date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday) continue;

            var hours = openingHours.FirstOrDefault(o => o.DayOfWeek == date.DayOfWeek);
            var openTime = hours?.OpenTime ?? DefaultOpen;
            var closeTime = hours?.CloseTime ?? DefaultClose;

            var isFullyBlocked = blockedSlots.Any(bs =>
                bs.StartDate <= date && bs.EndDate >= date && bs.StartTime == null);
            if (isFullyBlocked) continue;

            var partialBlocks = blockedSlots
                .Where(bs => bs.StartDate <= date && bs.EndDate >= date && bs.StartTime != null)
                .Select(bs => new { StartTime = bs.StartTime!.Value, EndTime = bs.EndTime!.Value })
                .ToList();

            var dayBookings = bookings.Where(b => b.Date == date).ToList();
            var current = openTime;
            var hasSlot = false;

            while (current.AddMinutes(treatment.DurationMinutes) <= closeTime)
            {
                var slotEnd = current.AddMinutes(treatment.DurationMinutes);

                var bookedTaken = dayBookings.Any(b =>
                    b.StartTime < slotEnd && b.EndTime > current);

                var blockTaken = partialBlocks.Any(p =>
                    p.StartTime < slotEnd && p.EndTime > current);

                if (!bookedTaken && !blockTaken) { hasSlot = true; break; }
                current = current.AddMinutes(30);
            }

            if (hasSlot)
                availableDates.Add(date);
        }

        return Results.Ok(new Response(availableDates));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/availability/dates", Handle)
           .WithName("GetAvailableDates")
           .WithTags("Availability");
    }
}