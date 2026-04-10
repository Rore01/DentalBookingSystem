using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentalBookingSystem.Pages.Account;

public class AdminLoginModel : PageModel
{
    private readonly HttpClient _http;

    public AdminLoginModel(IHttpClientFactory httpClientFactory)
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
            var payload = JsonSerializer.Serialize(new
            {
                email = Input.Email,
                password = Input.Password
            });

            var response = await _http.PostAsync("/api/clinics/login",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            var clinic = JsonSerializer.Deserialize<ClinicLoginResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (clinic is null)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                return Page();
            }

            // Create cookie with admin claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, clinic.Id.ToString()),
                new Claim(ClaimTypes.Name, clinic.Name),
                new Claim(ClaimTypes.Email, clinic.Email),
                new Claim("Role", "Admin"),
                new Claim("ClinicId", clinic.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToPage("/Admin/Dashboard");
        }
        catch
        {
            ErrorMessage = "Unable to connect. Please try again later.";
            return Page();
        }
    }

    private class ClinicLoginResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
