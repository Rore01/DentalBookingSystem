namespace DentalBookingSystemApi.Endpoints.BlockedSlots;

public class DeleteBlockedSlot : IEndpointMapper
{
    public async Task<IResult> Handle(
        int id,
        AppDbContext db,
        CancellationToken ct)
    {
        var slot = await db.BlockedSlots.FindAsync([id], ct);

        if (slot is null)
            return Results.NotFound("Blocked slot not found.");

        db.BlockedSlots.Remove(slot);
        await db.SaveChangesAsync(ct);

        return Results.NoContent();
    }

    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/api/blocked-slots/{id}", Handle)
           .WithName("DeleteBlockedSlot")
           .WithTags("BlockedSlots")
           .AllowAnonymous();
    }
}