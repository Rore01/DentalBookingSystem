namespace DentalBookingSystemApi.Endpoints.Patients;

public class GetPatient : IEndpointMapper
{
    public record Response(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        string? Phone
    );

    public async Task<IResult> Handle(
        int id,
        AppDbContext db,
        CancellationToken ct)
    {
        var patient = await db.Patients.FindAsync([id], ct);

        if (patient is null)
            return Results.NotFound("Patient not found.");

        return Results.Ok(new Response(
            patient.Id,
            patient.FirstName,
            patient.LastName,
            patient.Email,
            patient.Phone
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/patients/{id}", Handle)
           .WithName("GetPatient")
           .WithTags("Patients")
           .AllowAnonymous();
    }
}