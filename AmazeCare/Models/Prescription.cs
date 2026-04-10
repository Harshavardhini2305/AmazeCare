namespace AmazeCare.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }

        public int AppointmentId { get; set; }

        public string? MedicineName { get; set; }
        public string? Dosage { get; set; }

        // Navigation
        public Appointment? Appointment { get; set; }
    }
}
