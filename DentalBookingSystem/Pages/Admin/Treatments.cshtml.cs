using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace DentalBookingSystem.Pages.Admin;

public class TreatmentsModel : PageModel
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