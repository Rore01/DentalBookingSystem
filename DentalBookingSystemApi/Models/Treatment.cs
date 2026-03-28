namespace DentalBookingSystemApi.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }

        public string Name { get; set; } = string.Empty; 
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }

        // Navigation
        public Clinic Clinic { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
