namespace DentalBookingSystemApi.Models
{
    public class OpeningHours
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public bool IsClosed { get; set; }

        // Navigation
        public Clinic Clinic { get; set; } = null!;
    }
}
