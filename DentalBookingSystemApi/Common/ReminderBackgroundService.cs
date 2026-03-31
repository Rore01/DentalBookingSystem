namespace DentalBookingSystemApi.Common;

public class ReminderBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<ReminderBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SendPendingReminders();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task SendPendingReminders()
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var email = scope.ServiceProvider.GetRequiredService<EmailService>();

        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var bookings = await db.Bookings
            .Include(b => b.Patient)
            .Include(b => b.Treatment)
            .Where(b =>
                b.Date == tomorrow &&
                b.Status == BookingStatus.Confirmed &&
                b.ReminderSent == false)
            .ToListAsync();

        foreach (var booking in bookings)
        {
            try
            {
                await email.SendReminderAsync(booking.Patient, booking, booking.Treatment);
                booking.ReminderSent = true;
                logger.LogInformation(
                    "Reminder sent to {Email} for booking {Id}",
                    booking.Patient.Email, booking.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send reminder for booking {Id}", booking.Id);
            }
        }

        await db.SaveChangesAsync();
    }
}