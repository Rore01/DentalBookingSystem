namespace DentalBookingSystemApi.Endpoints.Patients;

public class LoginPatient : IEndpointMapper
{
    public record Request(string Email, string Password);

    public record Response(
        int Id,
        string FirstName,
        string LastName,
        string Email
    );

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return Results.BadRequest("Email and password are required.");

        var patient = await db.Patients
            .FirstOrDefaultAsync(p => p.Email == req.Email, ct);

        if (patient is null)
            return Results.Unauthorized();

        var passwordValid = BCrypt.Net.BCrypt.Verify(req.Password, patient.PasswordHash);

        if (!passwordValid)
            return Results.Unauthorized();

        return Results.Ok(new Response(
            patient.Id,
            patient.FirstName,
            patient.LastName,
            patient.Email
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/patients/login", Handle)
           .WithName("LoginPatient")
           .WithTags("Patients")
           .AllowAnonymous();
    }
}