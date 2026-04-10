using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DentalBookingSystem.Pages;

public class TreatmentViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsFeatured { get; set; }

    public string Icon => Name.ToLower() switch
    {
        var n when n.Contains("white") => "✨",
        var n when n.Contains("clean") => "🪥",
        var n when n.Contains("check") => "🔍",
        var n when n.Contains("fill") => "🛡️",
        var n when n.Contains("canal") => "🦷",
        var n when n.Contains("braces") => "😁",
        _ => "🦷"
    };
}

public class IndexModel : PageModel
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public List<TreatmentViewModel> Treatments { get; set; } = [];

    public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _http = httpClientFactory.CreateClient("DentaCareApi");
        _config = config;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var clinicId = _config.GetValue<int>("ClinicId", 1);
            var response = await _http.GetAsync($"/api/treatments?clinicId={clinicId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var treatments = JsonSerializer.Deserialize<List<TreatmentViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (treatments != null)
                {
                    if (treatments.Count > 0)
                        treatments[0].IsFeatured = true;

                    Treatments = treatments;
                }
            }
        }
        catch
        {
            Treatments = [];
        }
    }
}