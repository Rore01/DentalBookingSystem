namespace DentalBookingSystemApi.Endpoints.Treatments;

public class CreateTreatment : IEndpointMapper
{
    public record Request(
        int ClinicId,
        string Name,
        string? Description,
        decimal Price,
        int DurationMinutes
    );

    public record Response(int Id, string Name, decimal Price, int DurationMinutes);

    public async Task<IResult> Handle(
        Request req,
        AppDbContext db,
        CancellationToken ct)
    {
        var treatment = new Treatment
        {
            ClinicId = req.ClinicId,
            Name = req.Name,
            Description = req.Description,
            Price = req.Price,
            DurationMinutes = req.DurationMinutes
        };

        db.Treatments.Add(treatment);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/treatments/{treatment.Id}",
            new Response(treatment.Id, treatment.Name, treatment.Price, treatment.DurationMinutes));
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/api/treatments", Handle)
           .WithName("CreateTreatment")
           .WithTags("Treatments")
           .AllowAnonymous();
    }
}