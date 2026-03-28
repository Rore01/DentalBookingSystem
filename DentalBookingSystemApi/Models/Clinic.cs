namespace DentalBookingSystemApi.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation
        public ICollection<Treatment> Treatments { get; set; } = [];
        public ICollection<OpeningHours> OpeningHours { get; set; } = [];
        public ICollection<BlockedSlot> BlockedSlots { get; set; } = [];
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
