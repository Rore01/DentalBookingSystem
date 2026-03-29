using Microsoft.EntityFrameworkCore;

namespace DentalBookingSystemApi.Endpoints.Clinics;

public class LoginClinic : IEndpointMapper
{
    public record Request(string Email, string Password);

    public record Response(
        int Id,
        string Name,
        string Email
    );

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return Results.BadRequest("Email and password are required.");

        var clinic = await db.Clinics
            .FirstOrDefaultAsync(c => c.Email == req.Email, ct);

        if (clinic is null)
            return Results.Unauthorized();

        var passwordValid = BCrypt.Net.BCrypt.Verify(req.Password, clinic.PasswordHash);

        if (!passwordValid)
            return Results.Unauthorized();

        return Results.Ok(new Response(
            clinic.Id,
            clinic.Name,
            clinic.Email
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/clinics/login", Handle)
           .WithName("LoginClinic")
           .WithTags("Clinics")
           .AllowAnonymous();
    }
}