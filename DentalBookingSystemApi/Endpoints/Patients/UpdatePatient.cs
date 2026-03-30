namespace DentalBookingSystemApi.Endpoints.Patients;

public class UpdatePatient : IEndpointMapper
{
    public record Request(
        string FirstName,
        string LastName,
        string? Phone
    );

    public record Response(int Id, string FirstName, string LastName, string Email, string? Phone);

    public async Task<IResult> Handle(
        int id,
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        var patient = await db.Patients.FindAsync([id], ct);

        if (patient is null)
            return Results.NotFound("Patient not found.");

        patient.FirstName = req.FirstName;
        patient.LastName = req.LastName;
        patient.Phone = req.Phone;

        await db.SaveChangesAsync(ct);

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
        app.MapPut("/api/patients/{id}", Handle)
           .WithName("UpdatePatient")
           .WithTags("Patients")
           .AllowAnonymous();
    }
}