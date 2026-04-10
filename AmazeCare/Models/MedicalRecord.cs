namespace AmazeCare.Models
{
    public class MedicalRecord
    {
        public int MedicalRecordId { get; set; }

        public int AppointmentId { get; set; }

        // FULL CONSULTATION DETAILS
        public string? Symptoms { get; set; }
        public string? PhysicalExamination { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? RecommendedTests { get; set; }


        public string?Diagnosis { get; set; }
       

        
        public Appointment? Appointment { get; set; }
    }
}
