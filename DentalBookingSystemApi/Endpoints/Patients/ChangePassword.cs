namespace DentalBookingSystemApi.Endpoints.Patients;

public class ChangePassword : IEndpointMapper
{
    public record Request(string CurrentPassword, string NewPassword);

    public async Task<IResult> Handle(
        int id,
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        var patient = await db.Patients.FindAsync([id], ct);
        if (patient is null)
            return Results.NotFound("Patient not found.");

        if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, patient.PasswordHash))
            return Results.BadRequest("Current password is incorrect.");

        if (req.NewPassword.Length < 6)
            return Results.BadRequest("New password must be at least 6 characters.");

        patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await db.SaveChangesAsync(ct);

        return Results.Ok("Password updated successfully.");
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPatch("/api/patients/{id}/password", Handle)
           .WithName("ChangePassword")
           .WithTags("Patients")
           .AllowAnonymous();
    }
}