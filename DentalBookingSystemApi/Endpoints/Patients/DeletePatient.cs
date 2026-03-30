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

        var upcomingBookings = await db.Bookings
            .Where(b => b.PatientId == id && b.Status != BookingStatus.Cancelled)
            .ToListAsync(ct);

        foreach (var booking in upcomingBookings)
            booking.Status = BookingStatus.Cancelled;

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