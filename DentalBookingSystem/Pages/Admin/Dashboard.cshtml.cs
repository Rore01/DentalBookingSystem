using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace DentalBookingSystem.Pages.Admin;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly HttpClient _http;

    public DashboardModel(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("DentaCareApi");
    }

    public int ClinicId { get; set; }
    public List<BookingRow> TodayBookings { get; set; } = [];
    public List<BookingRow> UpcomingBookings { get; set; } = [];
    public int TodayCount { get; set; }
    public int WeekCount { get; set; }
    public int CancelCount { get; set; }
    public decimal MonthRevenue { get; set; }

    public async Task OnGetAsync()
    {
        var clinicId = User.FindFirst("ClinicId")?.Value ?? "1";
        ClinicId = int.Parse(clinicId);

        try
        {
            var response = await _http.GetAsync($"/api/bookings?clinicId={clinicId}");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var all = JsonSerializer.Deserialize<List<BookingRow>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

            var today = DateOnly.FromDateTime(DateTime.Today);
            var weekStart = today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            var monthStart = new DateOnly(today.Year, today.Month, 1);

            TodayBookings = all
                .Where(b => b.Date == today)
                .OrderBy(b => b.StartTime)
                .ToList();

            UpcomingBookings = all
                .Where(b => b.Date >= today && b.Status != "Cancelled")
                .OrderBy(b => b.Date).ThenBy(b => b.StartTime)
                .ToList();

            TodayCount = TodayBookings.Count(b => b.Status is "Confirmed" or "Pending");
            WeekCount = all.Count(b => b.Date >= weekStart && b.Date <= weekStart.AddDays(6));
            CancelCount = all.Count(b => b.Date >= monthStart && b.Status == "Cancelled");
            MonthRevenue = all
                .Where(b => b.Date >= monthStart && b.Status == "Completed")
                .Sum(b => b.Price);
        }
        catch
        {
            // API unavailable — show empty dashboard
        }
    }

    public class BookingRow
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string TreatmentName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DurationMinutes =>
            (int)(EndTime.ToTimeSpan() - StartTime.ToTimeSpan()).TotalMinutes;
    }
}