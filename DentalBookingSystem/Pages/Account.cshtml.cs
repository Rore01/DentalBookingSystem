using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace DentalBookingSystem.Pages;

[Authorize]
public class AccountModel : PageModel
{
    private readonly HttpClient _http;

    public AccountModel(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("DentaCareApi");
    }

    public int PatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var role = User.FindFirst("Role")?.Value;
        if (role == "Admin")
            return RedirectToPage("/Admin/Dashboard");

        PatientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        try
        {
            var res = await _http.GetAsync($"/api/patients/{PatientId}");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var patient = JsonSerializer.Deserialize<PatientDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (patient != null)
                {
                    FirstName = patient.FirstName;
                    LastName = patient.LastName;
                    Phone = patient.Phone;
                }
            }
        }
        catch { }

        return Page();
    }

    private record PatientDto(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        string? Phone
    );
}