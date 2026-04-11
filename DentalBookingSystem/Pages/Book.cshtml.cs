using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBookingSystem.Pages;

public class BookModel : PageModel
{
    public int ClinicId { get; private set; }
    public int? RescheduleId { get; private set; }

    public void OnGet(int? rescheduleId)
    {
        ClinicId = int.Parse(
            HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["ClinicId"] ?? "1");

        RescheduleId = rescheduleId;
    }
}