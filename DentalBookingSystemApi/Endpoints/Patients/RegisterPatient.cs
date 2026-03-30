namespace DentalBookingSystemApi.Endpoints.Patients;

public class RegisterPatient : IEndpointMapper
{
    public record Request(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string? Phone
    );

    public record Response(int Id, string FirstName, string LastName, string Email);

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {

        var exists = await db.Patients.AnyAsync(p => p.Email == req.Email, ct);
        if (exists)
            return Results.Conflict("An account with this email already exists.");

        var patient = new Patient
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Phone = req.Phone
        };

        db.Patients.Add(patient);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/patients/{patient.Id}",
            new Response(patient.Id, patient.FirstName, patient.LastName, patient.Email));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/patients/register", Handle)
           .WithName("RegisterPatient")
           .WithTags("Patients")
           .AllowAnonymous();
    }
}