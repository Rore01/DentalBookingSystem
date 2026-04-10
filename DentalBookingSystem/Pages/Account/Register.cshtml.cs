using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DentalBookingSystem.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly HttpClient _http;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("DentaCareApi");
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Call API to register patient
            var payload = JsonSerializer.Serialize(new
            {
                firstName = Input.FirstName,
                lastName = Input.LastName,
                email = Input.Email,
                password = Input.Password,
                phone = Input.Phone
            });

            var response = await _http.PostAsync("/api/patients/register",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ErrorMessage = "An account with this email already exists.";
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Registration failed. Please try again.";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            var patient = JsonSerializer.Deserialize<RegisterResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (patient is null)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                return Page();
            }

            // Auto sign-in after registration
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

    private class RegisterResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
