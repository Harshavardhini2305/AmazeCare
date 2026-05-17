// Stores patient's complete medical history.
// Can be added by doctor or admin.
//
// Examples of RecordType:
//   "Lab Report", "X-Ray", "MRI Scan", "Diagnosis",
//   "Allergy", "Vaccination", "Surgical History"



namespace AmazeCare.API.Models
{
    public class MedicalRecord
    {
        public int RecordId { get; set; }

        // FK → Patient (Many records per patient)
        public int PatientId { get; set; }

        // Who created this record (Doctor name or "Admin")
        public string RecordedBy { get; set; } = string.Empty;

        // Type: "Lab Report" / "X-Ray" / "Diagnosis" / "Allergy" etc.
        public string RecordType { get; set; } = string.Empty;

        // e.g. "Blood Test Report - June 2024"
        public string Title { get; set; } = string.Empty;

        // Detailed description / findings
        public string Description { get; set; } = string.Empty;

        // Optional file name if any document uploaded
        public string? AttachmentFileName { get; set; }

        // When this record/event happened
        public DateTime RecordDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property → Patient
        public Patient Patient { get; set; } = null!;

    }
}
