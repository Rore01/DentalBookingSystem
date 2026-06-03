namespace DentalBookingSystemApi.Endpoints.Patients;

public class GetAllPatients : IEndpointMapper
{
    public record Response(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        string? Phone,
        int BookingCount
    );

    public async Task<IResult> Handle(
    AppDbContext db,
    CancellationToken ct)
    {
        var patients = await db.Patients
            .Include(p => p.Bookings)
            .ToListAsync(ct);

        var response = patients
            .Select(p => new Response(
                p.Id,
                p.FirstName,
                p.LastName,
                p.Email,
                p.Phone,
                p.Bookings.Count
            ))
            .OrderBy(p => p.FirstName)
            .ToList();

        return Results.Ok(response);
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/patients", Handle)
           .WithName("GetAllPatients")
           .WithTags("Patients")
           .AllowAnonymous();
    }
}