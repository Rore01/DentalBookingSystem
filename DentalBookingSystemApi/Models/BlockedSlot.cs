namespace DentalBookingSystemApi.Models
{
    public class BlockedSlot
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Reason { get; set; } = string.Empty;

        // Navigation
        public Clinic Clinic { get; set; } = null!;
    }
}