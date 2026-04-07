namespace DentalBookingSystemApi.Endpoints.Bookings;

public class GetBookingsByPatient : IEndpointMapper
{
    public record BookingDto(
        int Id,
        string TreatmentName,
        decimal Price,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string Status
    );

    public async Task<IResult> Handle(
        int patientId,
        AppDbContext db,
        CancellationToken ct)
    {
        var bookings = await db.Bookings
            .Include(b => b.Treatment)
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.Date)
            .Select(b => new BookingDto(
                b.Id,
                b.Treatment.Name,
                b.Treatment.Price,
                b.Date,
                b.StartTime,
                b.EndTime,
                b.Status.ToString()
            ))
            .ToListAsync(ct);

        return Results.Ok(bookings);
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/bookings/patient/{patientId}", Handle)
           .WithName("GetBookingsByPatient")
           .WithTags("Bookings")
           .AllowAnonymous();
    }
}