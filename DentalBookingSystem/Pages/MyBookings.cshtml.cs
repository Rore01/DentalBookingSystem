using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace DentalBookingSystem.Pages;

[Authorize]
public class MyBookingsModel : PageModel
{
    private readonly HttpClient _http;

    public MyBookingsModel(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("DentaCareApi");
    }

    public List<BookingViewModel> Bookings { get; set; } = [];

    public async Task OnGetAsync()
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (patientId is null) return;

        try
        {
            var response = await _http.GetAsync($"/api/bookings/patient/{patientId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bookings = JsonSerializer.Deserialize<List<BookingViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (bookings != null)
                    Bookings = bookings;
            }
        }
        catch
        {
            Bookings = [];
        }
    }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        try
        {
            var response = await _http.PutAsync($"/api/bookings/{bookingId}/cancel", null);
            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Could not cancel booking. Please try again.";
            else
                TempData["Success"] = "Your booking has been cancelled.";
        }
        catch
        {
            TempData["Error"] = "Network error. Please try again.";
        }
        return RedirectToPage();
    }
}

public class BookingViewModel
{
    public int Id { get; set; }
    public string TreatmentName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = string.Empty;

    public int DurationMinutes =>
        (int)(EndTime.ToTimeSpan() - StartTime.ToTimeSpan()).TotalMinutes;
}