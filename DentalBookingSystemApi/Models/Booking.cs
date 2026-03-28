namespace DentalBookingSystemApi.Models
{
    public enum BookingStatus { Pending, Confirmed, Cancelled, Completed }

    public class Booking
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public int PatientId { get; set; }
        public int TreatmentId { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
        public bool ReminderSent { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Clinic Clinic { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public Treatment Treatment { get; set; } = null!;
    }
}
