namespace DentalBookingSystemApi.Endpoints.Bookings;

public class GetAllBookings : IEndpointMapper
{
    public record BookingDto(
        int Id,
        int PatientId,
        string PatientName,
        string PatientEmail,
        string PatientPhone,
        string TreatmentName,
        decimal Price,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        BookingStatus Status,
        DateTime CreatedAt
    );

    public async Task<IResult> Handle(
        int clinicId,
        AppDbContext db,
        CancellationToken ct)
    {
        var bookings = await db.Bookings
            .Include(b => b.Patient)
            .Include(b => b.Treatment)
            .Where(b => b.ClinicId == clinicId)
            .OrderBy(b => b.Date)
            .ThenBy(b => b.StartTime)
            .Select(b => new BookingDto(
                b.Id,
                b.PatientId,
                $"{b.Patient.FirstName} {b.Patient.LastName}",
                b.Patient.Email,
                b.Patient.Phone ?? "—",
                b.Treatment.Name,
                b.Treatment.Price,
                b.Date,
                b.StartTime,
                b.EndTime,
                b.Status,
                b.CreatedAt
            ))
            .ToListAsync(ct);

        return Results.Ok(bookings);
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/bookings", Handle)
           .WithName("GetAllBookings")
           .WithTags("Bookings")
           .AllowAnonymous();
    }
}