namespace DentalBookingSystemApi.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Phone { get; set; }

        // Navigation
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
