using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace DentalBookingSystem.Pages.Admin;

public class BookingsModel : PageModel
{
    public int ClinicId { get; private set; }

    public IActionResult OnGet()
    {
        var role = User.FindFirst("Role")?.Value;
        if (role != "Admin")
            return RedirectToPage("/Account/AdminLogin");

        ClinicId = int.Parse(User.FindFirst("ClinicId")?.Value ?? "1");
        return Page();
    }
}