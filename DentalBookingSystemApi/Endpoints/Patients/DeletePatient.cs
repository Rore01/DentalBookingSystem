namespace DentalBookingSystemApi.Endpoints.Patients;

public class DeletePatient : IEndpointMapper
{
    public async Task<IResult> Handle(
    int id,
    AppDbContext db,
    CancellationToken ct)
    {
        var patient = await db.Patients.FindAsync([id], ct);
        if (patient is null)
            return Results.NotFound("Patient not found.");

        var bookings = await db.Bookings
            .Where(b => b.PatientId == id)
            .ToListAsync(ct);

        db.Bookings.RemoveRange(bookings);
        db.Patients.Remove(patient);
        await db.SaveChangesAsync(ct);

        return Results.Ok("Account deleted.");
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/api/patients/{id}", Handle)
           .WithName("DeletePatient")
           .WithTags("Patients")
           .AllowAnonymous();
    }
}