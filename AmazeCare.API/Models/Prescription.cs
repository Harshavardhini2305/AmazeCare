//Each medicine prescribed during a consultation
    // is stored as a separate row in this table.
    //
    // Example: For one consultation, 3 medicines = 3 rows:
    //   Row 1: Paracetamol 500mg | 1-1-1 | AF | 5 days
    //   Row 2: Cough Syrup       | 0-0-1 | BF | 3 days
    //   Row 3: Vitamin C         | 1-0-0 | AF | 30 days



namespace AmazeCare.API.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }

        // FK → Consultation (Many prescriptions per consultation)
        public int ConsultationId { get; set; }

        // e.g. "Paracetamol 500mg", "Amoxicillin 250mg"
        public string MedicineName { get; set; } = string.Empty;

        // Dosage pattern: Morning-Afternoon-Night
        // "1-0-1" = 1 tablet morning, 0 afternoon, 1 night
        // "0-0-1" = only at night
        // "1-1-1" = thrice a day
        public string Dosage { get; set; } = string.Empty;

        // AF = After Food, BF = Before Food
        public string Timing { get; set; } = string.Empty;

        // e.g. "5 days", "2 weeks", "1 month"
        public string Duration { get; set; } = string.Empty;

        // Optional extra note e.g. "Take with warm water", "Avoid dairy"
        public string? Notes { get; set; }

        // Navigation Property → Consultation
        public Consultation Consultation { get; set; } = null!;
    }
}
