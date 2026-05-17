namespace AmazeCare.API.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        // Foreign Key to Patient
        public int PatientId { get; set; }

        // Foreign Key to Doctor
        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string NatureOfVisit { get; set; } = string.Empty;

        // Status: Pending / Confirmed / Completed / Cancelled
        public string Status { get; set; } = "Pending";
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

        // One-to-One: Appointment can have one Consultation
        public Consultation? Consultation { get; set; }

    }
}
