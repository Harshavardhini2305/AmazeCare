// Stores appointment data
// Includes PatientId, DoctorId
// Links patient and doctor
// Used to track bookings




namespace AmazeCare.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }
        public string? Symptoms { get; set; }

        public string VisitType { get; set; } // checkup / issue

        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
