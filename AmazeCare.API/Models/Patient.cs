namespace AmazeCare.API.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Patient";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property (One-to-Many: Patient has many Appointments)
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();


        // One Patient → Many MedicalRecords  ← NEW
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();


    }
}
