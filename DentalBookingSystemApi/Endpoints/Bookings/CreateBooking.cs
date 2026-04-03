namespace DentalBookingSystemApi.Endpoints.Bookings;

public class CreateBooking : IEndpointMapper
{
    public record Request(
        int ClinicId,
        int PatientId,
        int TreatmentId,
        DateOnly Date,
        TimeOnly StartTime
    );

    public record Response(
        int BookingId,
        string TreatmentName,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        decimal Price
    );

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        EmailService email,
        IHubContext<BookingHub> hub,
        CancellationToken ct)
    {
        var treatment = await db.Treatments
            .FirstOrDefaultAsync(t => t.Id == req.TreatmentId && t.ClinicId == req.ClinicId, ct);

        if (treatment is null)
            return Results.NotFound("Treatment not found.");

        var endTime = req.StartTime.AddMinutes(treatment.DurationMinutes);

        var conflict = await db.Bookings.AnyAsync(b =>
            b.ClinicId == req.ClinicId &&
            b.Date == req.Date &&
            b.Status != BookingStatus.Cancelled &&
            b.StartTime < endTime &&
            b.EndTime > req.StartTime, ct);

        if (conflict)
            return Results.Conflict("This time slot is already booked.");

        var isBlocked = await db.BlockedSlots.AnyAsync(bs =>
            bs.ClinicId == req.ClinicId &&
            bs.StartDate <= req.Date &&
            bs.EndDate >= req.Date &&
            bs.StartTime == null, ct);

        if (isBlocked)
            return Results.BadRequest("The clinic is closed on this date.");

        var booking = new Booking
        {
            ClinicId = req.ClinicId,
            PatientId = req.PatientId,
            TreatmentId = req.TreatmentId,
            Date = req.Date,
            StartTime = req.StartTime,
            EndTime = endTime,
            Status = BookingStatus.Confirmed
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync(ct);

        try
        {
            await hub.Clients
                .Group($"clinic-{req.ClinicId}")
                .SendAsync(BookingHubEvents.SlotTaken, new
                {
                    date = req.Date,
                    startTime = req.StartTime,
                    endTime = endTime
                }, ct);
        }
        catch { /* SignalR failure should not block booking */ }

        var patient = await db.Patients.FindAsync([req.PatientId], ct);
        if (patient is not null)
        {
            try
            {
                await email.SendBookingConfirmationAsync(patient, booking, treatment);
            }
            catch { /* Email failure should not block booking */ }
        }

        return Results.Created($"/api/bookings/{booking.Id}", new Response(
            booking.Id,
            treatment.Name,
            booking.Date,
            booking.StartTime,
            booking.EndTime,
            treatment.Price
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/bookings", Handle)
           .WithName("CreateBooking")
           .WithTags("Bookings")
           .AllowAnonymous();
    }
}