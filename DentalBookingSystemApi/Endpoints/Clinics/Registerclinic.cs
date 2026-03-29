using Microsoft.EntityFrameworkCore;

namespace DentalBookingSystemApi.Endpoints.Clinics;

public class RegisterClinic : IEndpointMapper
{
    public record Request(
        string Name,
        string Email,
        string Password
    );

    public record Response(int Id, string Name, string Email);

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        var exists = await db.Clinics.AnyAsync(c => c.Email == req.Email, ct);
        if (exists)
            return Results.Conflict("A clinic with this email already exists.");

        var clinic = new Clinic
        {
            Name = req.Name,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        db.Clinics.Add(clinic);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/clinics/{clinic.Id}",
            new Response(clinic.Id, clinic.Name, clinic.Email));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/clinics/register", Handle)
           .WithName("RegisterClinic")
           .WithTags("Clinics")
           .AllowAnonymous();
    }
}