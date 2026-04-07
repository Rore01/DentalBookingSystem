namespace DentalBookingSystemApi.Endpoints.Bookings;

public class CancelBooking : IEndpointMapper
{
    public record Response(int BookingId, BookingStatus Status);

    public async Task<IResult> Handle(
        int id,
        AppDbContext db,
        EmailService email,
        IHubContext<BookingHub> hub,
        CancellationToken ct)
    {
        var booking = await db.Bookings
            .Include(b => b.Patient)
            .Include(b => b.Treatment)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (booking is null)
            return Results.NotFound("Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return Results.BadRequest("Booking is already cancelled.");

        booking.Status = BookingStatus.Cancelled;
        await db.SaveChangesAsync(ct);

        try
        {
            await hub.Clients
                .Group($"clinic-{booking.ClinicId}")
                .SendAsync(BookingHubEvents.SlotFreed, new
                {
                    date = booking.Date,
                    startTime = booking.StartTime,
                    endTime = booking.EndTime
                }, ct);
        }
        catch { /* SignalR failure should not block cancellation */ }

        // Send cancellation email
        try
        {
            await email.SendCancellationAsync(booking.Patient, booking, booking.Treatment);
        }
        catch { /* Email failure should not block cancellation */ }

        return Results.Ok(new Response(booking.Id, booking.Status));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/api/bookings/{id}/cancel", Handle)
           .WithName("CancelBooking")
           .WithTags("Bookings")
           .AllowAnonymous();
    }
}