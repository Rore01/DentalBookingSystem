namespace DentalBookingSystemApi.Endpoints.Bookings;

public class GetBooking : IEndpointMapper
{
    public record Response(
        int Id,
        string PatientName,
        string PatientEmail,
        string TreatmentName,
        decimal Price,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        BookingStatus Status,
        bool ReminderSent,
        DateTime CreatedAt
    );

    public async Task<IResult> Handle(
        int id,
        AppDbContext db,
        CancellationToken ct)
    {
        var booking = await db.Bookings
            .Include(b => b.Patient)
            .Include(b => b.Treatment)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (booking is null)
            return Results.NotFound("Booking not found.");

        return Results.Ok(new Response(
            booking.Id,
            $"{booking.Patient.FirstName} {booking.Patient.LastName}",
            booking.Patient.Email,
            booking.Treatment.Name,
            booking.Treatment.Price,
            booking.Date,
            booking.StartTime,
            booking.EndTime,
            booking.Status,
            booking.ReminderSent,
            booking.CreatedAt
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/bookings/{id}", Handle)
           .WithName("GetBooking")
           .WithTags("Bookings")
           .AllowAnonymous();
    }
}