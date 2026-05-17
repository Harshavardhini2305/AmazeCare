namespace AmazeCare.API.Models
{
    public class Consultation
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }  // FK → Appointment (One-to-One)
        public string CurrentSymptoms { get; set; } = string.Empty;
        public string PhysicalExamination { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string TreatmentPlan { get; set; } = string.Empty;

        // Recommended tests as a comma-separated string
        // e.g. "Blood Test (CBC), Urine Test, X-Ray Chest"
        public string RecommendedTests { get; set; } = string.Empty;

        public DateTime ConsultedAt { get; set; } = DateTime.Now;

        // Navigation Property
        public Appointment Appointment { get; set; } = null!;

        // One Consultation → Many Prescriptions  ← NEW
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
