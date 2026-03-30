namespace DentalBookingSystemApi.Endpoints.Treatments;

public class DeleteTreatment : IEndpointMapper
{
    public async Task<IResult> Handle(
        int id,
        AppDbContext db,
        CancellationToken ct)
    {
        var treatment = await db.Treatments.FindAsync([id], ct);

        if (treatment is null)
            return Results.NotFound("Treatment not found.");

        db.Treatments.Remove(treatment);
        await db.SaveChangesAsync(ct);

        return Results.NoContent();
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/api/treatments/{id}", Handle)
           .WithName("DeleteTreatment")
           .WithTags("Treatments")
           .AllowAnonymous();
    }
}