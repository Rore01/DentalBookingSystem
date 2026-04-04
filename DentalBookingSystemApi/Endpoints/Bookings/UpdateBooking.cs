namespace DentalBookingSystemApi.Endpoints.Bookings;

public class UpdateBooking : IEndpointMapper
{
    public record StatusRequest(string Status);
    public record StatusResponse(int Id, BookingStatus Status);

    public record RescheduleRequest(DateOnly NewDate, TimeOnly NewStartTime);
    public record RescheduleResponse(
        int Id,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        BookingStatus Status
    );

    public async Task<IResult> HandleStatus(
        int id,
        StatusRequest req,
        AppDbContext db,
        CancellationToken ct)
    {
        var booking = await db.Bookings.FindAsync([id], ct);
        if (booking is null)
            return Results.NotFound("Booking not found.");

        if (!Enum.TryParse<BookingStatus>(req.Status, ignoreCase: true, out var newStatus))
            return Results.BadRequest($"Invalid status: {req.Status}");

        booking.Status = newStatus;
        await db.SaveChangesAsync(ct);

        return Results.Ok(new StatusResponse(booking.Id, booking.Status));
    }

    public async Task<IResult> HandleReschedule(
        int id,
        RescheduleRequest req,
        AppDbContext db,
        EmailService email,
        CancellationToken ct)
    {
        var booking = await db.Bookings
            .Include(b => b.Treatment)
            .Include(b => b.Patient)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (booking is null)
            return Results.NotFound("Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return Results.BadRequest("Cannot reschedule a cancelled booking.");

        var newEndTime = req.NewStartTime.AddMinutes(booking.Treatment.DurationMinutes);

        var conflict = await db.Bookings.AnyAsync(b =>
            b.Id != id &&
            b.ClinicId == booking.ClinicId &&
            b.Date == req.NewDate &&
            b.Status != BookingStatus.Cancelled &&
            b.StartTime < newEndTime &&
            b.EndTime > req.NewStartTime, ct);

        if (conflict)
            return Results.Conflict("The new time slot is already booked.");

        var isBlocked = await db.BlockedSlots.AnyAsync(bs =>
            bs.ClinicId == booking.ClinicId &&
            bs.StartDate <= req.NewDate &&
            bs.EndDate >= req.NewDate &&
            bs.StartTime == null, ct);

        if (isBlocked)
            return Results.BadRequest("The clinic is closed on that date.");

        booking.Date = req.NewDate;
        booking.StartTime = req.NewStartTime;
        booking.EndTime = newEndTime;
        booking.ReminderSent = false;
        await db.SaveChangesAsync(ct);

        try
        {
            await email.SendRescheduleConfirmationAsync(booking.Patient, booking, booking.Treatment);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RESCHEDULE EMAIL ERROR: {ex.Message}");
        }

        return Results.Ok(new RescheduleResponse(
            booking.Id,
            booking.Date,
            booking.StartTime,
            booking.EndTime,
            booking.Status
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPatch("/api/bookings/{id}/status", HandleStatus)
           .WithName("UpdateBookingStatus")
           .WithTags("Bookings")
           .AllowAnonymous();

        app.MapPut("/api/bookings/{id}", HandleReschedule)
           .WithName("RescheduleBooking")
           .WithTags("Bookings")
           .AllowAnonymous();
    }
}