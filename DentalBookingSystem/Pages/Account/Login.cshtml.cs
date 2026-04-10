using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentalBookingSystem.Pages.Account;

public class LoginModel : PageModel
{
    private readonly HttpClient _http;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("DentaCareApi");
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Call API to verify patient credentials
            var payload = JsonSerializer.Serialize(new
            {
                email = Input.Email,
                password = Input.Password
            });

            var response = await _http.PostAsync("/api/patients/login",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            var patient = JsonSerializer.Deserialize<PatientLoginResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (patient is null)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                return Page();
            }

            // Create cookie with patient claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{patient.FirstName} {patient.LastName}"),
                new Claim(ClaimTypes.Email, patient.Email),
                new Claim("Role", "Patient")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToPage("/MyBookings");
        }
        catch
        {
            ErrorMessage = "Unable to connect. Please try again later.";
            return Page();
        }
    }

    private class PatientLoginResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}