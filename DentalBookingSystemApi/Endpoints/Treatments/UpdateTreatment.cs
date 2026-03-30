namespace DentalBookingSystemApi.Endpoints.Treatments;

public class UpdateTreatment : IEndpointMapper
{
    public record Request(
        string Name,
        string? Description,
        decimal Price,
        int DurationMinutes
    );

    public record Response(int Id, string Name, decimal Price, int DurationMinutes);

    public async Task<IResult> Handle(
        int id,
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        var treatment = await db.Treatments.FindAsync([id], ct);

        if (treatment is null)
            return Results.NotFound("Treatment not found.");

        treatment.Name = req.Name;
        treatment.Description = req.Description;
        treatment.Price = req.Price;
        treatment.DurationMinutes = req.DurationMinutes;

        await db.SaveChangesAsync(ct);

        return Results.Ok(new Response(
            treatment.Id,
            treatment.Name,
            treatment.Price,
            treatment.DurationMinutes
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/api/treatments/{id}", Handle)
           .WithName("UpdateTreatment")
           .WithTags("Treatments")
           .AllowAnonymous();
    }
}