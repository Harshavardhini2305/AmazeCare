// ─────────────────────────────────────────────────────────────
// AmazeCare.MVC/Models/ViewModels.cs
// All ViewModels used in Views
// These match the DTOs from AmazeCare.API
// ─────────────────────────────────────────────────────────────

using System.ComponentModel.DataAnnotations;

namespace AmazeCare.MVC.Models
{
    // ══════════════════════════════════════════════════════
    // AUTH VIEW MODELS
    // ══════════════════════════════════════════════════════
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        // Which role is logging in
        public string Role { get; set; } = "Patient";
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Mobile number must be 10 digits.")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginResponseViewModel
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }


    // ══════════════════════════════════════════════════════
    // DOCTOR VIEW MODELS
    // ══════════════════════════════════════════════════════
    public class DoctorViewModel
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Qualification { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class CreateDoctorViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specialty is required.")]
        public string Specialty { get; set; } = string.Empty;

        [Required]
        [Range(0, 50)]
        [Display(Name = "Experience (Years)")]
        public int ExperienceYears { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        public string Designation { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Mobile number must be 10 digits.")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class UpdateDoctorViewModel
    {
        public int DoctorId { get; set; }
        public string? FullName { get; set; }
        public string? Specialty { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Qualification { get; set; }
        public string? Designation { get; set; }
        public string? MobileNumber { get; set; }
    }


    // ══════════════════════════════════════════════════════
    // PATIENT VIEW MODELS
    // ══════════════════════════════════════════════════════
    public class PatientViewModel
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdatePatientViewModel
    {
        public string? FullName { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }


    // ══════════════════════════════════════════════════════
    // APPOINTMENT VIEW MODELS
    // ══════════════════════════════════════════════════════
    public class CreateAppointmentViewModel
    {
        [Required(ErrorMessage = "Please select a doctor.")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Time slot is required.")]
        [Display(Name = "Time Slot")]
        public string TimeSlot { get; set; } = string.Empty;

        [Required(ErrorMessage = "Symptoms are required.")]
        [StringLength(500)]
        [Display(Name = "Symptoms / Health Concerns")]
        public string Symptoms { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nature of visit is required.")]
        [Display(Name = "Nature of Visit")]
        public string NatureOfVisit { get; set; } = string.Empty;
    }

    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientMobile { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialty { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public string NatureOfVisit { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public ConsultationViewModel? Consultation { get; set; }
    }

    public class RescheduleViewModel
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "New date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "New Date")]
        public DateTime NewDate { get; set; }

        [Required(ErrorMessage = "New time slot is required.")]
        [Display(Name = "New Time Slot")]
        public string NewTimeSlot { get; set; } = string.Empty;
    }

    public class CancelViewModel
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [Display(Name = "Cancellation Reason")]
        public string Reason { get; set; } = string.Empty;
    }


    // ══════════════════════════════════════════════════════
    // CONSULTATION VIEW MODELS
    // ══════════════════════════════════════════════════════
    public class CreateConsultationViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Current symptoms are required.")]
        [Display(Name = "Current Symptoms")]
        public string CurrentSymptoms { get; set; } = string.Empty;

        [Required(ErrorMessage = "Physical examination is required.")]
        [Display(Name = "Physical Examination")]
        public string PhysicalExamination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Diagnosis is required.")]
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Treatment plan is required.")]
        [Display(Name = "Treatment Plan")]
        public string TreatmentPlan { get; set; } = string.Empty;

        [Display(Name = "Recommended Tests")]
        public string RecommendedTests { get; set; } = string.Empty;
    }

    public class ConsultationViewModel
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public string CurrentSymptoms { get; set; } = string.Empty;
        public string PhysicalExamination { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string TreatmentPlan { get; set; } = string.Empty;
        public string RecommendedTests { get; set; } = string.Empty;
        public DateTime ConsultedAt { get; set; }
    }


    // ══════════════════════════════════════════════════════
    // PRESCRIPTION VIEW MODELS
    // ══════════════════════════════════════════════════════
    public class PrescriptionViewModel
    {
        public int PrescriptionId { get; set; }
        public int ConsultationId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Timing { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class CreatePrescriptionViewModel
    {
        public int ConsultationId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;

        // List of medicines to add
        public List<PrescriptionItemViewModel> Prescriptions { get; set; }
            = new List<PrescriptionItemViewModel>
            {
                new PrescriptionItemViewModel() // start with one empty row
            };
    }

    public class PrescriptionItemViewModel
    {
        [Required(ErrorMessage = "Medicine name is required.")]
        [Display(Name = "Medicine Name")]
        public string MedicineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dosage is required.")]
        [Display(Name = "Dosage (e.g. 1-0-1)")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Timing is required.")]
        [Display(Name = "Timing")]
        public string Timing { get; set; } = "AF";

        [Required(ErrorMessage = "Duration is required.")]
        [Display(Name = "Duration")]
        public string Duration { get; set; } = string.Empty;

        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }


    // ══════════════════════════════════════════════════════
    // MEDICAL RECORD VIEW MODELS
    // ══════════════════════════════════════════════════════
    public class MedicalRecordViewModel
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string RecordedBy { get; set; } = string.Empty;
        public string RecordType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? AttachmentFileName { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMedicalRecordViewModel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recorded by is required.")]
        [Display(Name = "Recorded By")]
        public string RecordedBy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Record type is required.")]
        [Display(Name = "Record Type")]
        public string RecordType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description / Findings")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Attachment File Name")]
        public string? AttachmentFileName { get; set; }

        [Required(ErrorMessage = "Record date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Record Date")]
        public DateTime RecordDate { get; set; } = DateTime.Today;
    }


    // ══════════════════════════════════════════════════════
    // API RESPONSE WRAPPER
    // Matches ApiResponse<T> from backend
    // ══════════════════════════════════════════════════════
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    // Paged response from backend
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
