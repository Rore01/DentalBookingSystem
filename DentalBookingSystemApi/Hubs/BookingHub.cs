using Microsoft.AspNetCore.SignalR;

namespace DentalBookingSystemApi.Hubs;

public class BookingHub : Hub
{
    public async Task JoinClinicGroup(int clinicId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"clinic-{clinicId}");
    }

    public async Task LeaveClinicGroup(int clinicId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"clinic-{clinicId}");
    }
}

public static class BookingHubEvents
{
    public const string SlotTaken = "SlotTaken";
    public const string SlotFreed = "SlotFreed";
}