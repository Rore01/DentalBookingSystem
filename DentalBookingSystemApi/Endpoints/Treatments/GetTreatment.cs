namespace DentalBookingSystemApi.Endpoints.Treatments;

public class GetTreatment : IEndpointMapper
{
    public record Response(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        int DurationMinutes
    );

    public async Task<IResult> Handle(
        int id,
        AppDbContext db,
        CancellationToken ct)
    {
        var treatment = await db.Treatments.FindAsync([id], ct);

        if (treatment is null)
            return Results.NotFound("Treatment not found.");

        return Results.Ok(new Response(
            treatment.Id,
            treatment.Name,
            treatment.Description,
            treatment.Price,
            treatment.DurationMinutes
        ));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/treatments/{id}", Handle)
           .WithName("GetTreatment")
           .WithTags("Treatments");
    }
}