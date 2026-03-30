namespace DentalBookingSystemApi.Endpoints.Treatments;

public class GetAllTreatments : IEndpointMapper
{
    public record TreatmentDto(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        int DurationMinutes
    );

    public async Task<IResult> Handle(
        int clinicId,
        AppDbContext db,
        CancellationToken ct)
    {
        var treatments = await db.Treatments
            .Where(t => t.ClinicId == clinicId)
            .OrderBy(t => t.Name)
            .Select(t => new TreatmentDto(t.Id, t.Name, t.Description, t.Price, t.DurationMinutes))
            .ToListAsync(ct);

        return Results.Ok(treatments);
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/api/treatments", Handle)
           .WithName("GetAllTreatments")
           .WithTags("Treatments");
    }
}