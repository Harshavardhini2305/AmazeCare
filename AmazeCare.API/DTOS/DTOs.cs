

//namespace AmazeCare.API.DTOs
//{
//    // ── AUTH ──────────────────────────
//    public class RegisterPatientDto { }
//    public class LoginDto { }
//    public class LoginResponseDto { }

//    // ── PATIENT ───────────────────────
//    public class PatientDto { }
//    public class UpdatePatientDto { }

//    // ── DOCTOR ────────────────────────
//    public class DoctorDto { }
//    public class CreateDoctorDto { }
//    public class UpdateDoctorDto { }

//    // ── APPOINTMENT ───────────────────
//    public class CreateAppointmentDto { }
//    public class AppointmentDto { }
//    public class RescheduleAppointmentDto { }
//    public class CancelAppointmentDto { }

//    // ── CONSULTATION ──────────────────
//    public class CreateConsultationDto { }
//    public class ConsultationDto { }

//    // ── MEDICAL RECORD ────────────────
//    public class CreateMedicalRecordDto { }
//    public class MedicalRecordDto { }

//    // ── PRESCRIPTION ──────────────────
//    public class CreatePrescriptionDto { }
//    public class PrescriptionDto { }

//    // ── COMMON RESPONSE ───────────────
//    public class ApiResponse<T> { }
//}


// DTOs = Data Transfer Objects
// These are simple classes used to send/receive data via API.
// We do NOT expose the full database model to the client.

using System.ComponentModel.DataAnnotations;

namespace AmazeCare.API.DTOS
{
    // ══════════════════════════════════════════════════════
    // AUTH DTOs
    // ══════════════════════════════════════════════════════

    public class RegisterPatientDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Mobile number must be 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*]).{8,}$",
            ErrorMessage = "Password must have uppercase, number and special character.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }


    // ══════════════════════════════════════════════════════
    // PATIENT DTOs
    // ══════════════════════════════════════════════════════

    public class PatientDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdatePatientDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Mobile number must be 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = string.Empty;
    }


    // ══════════════════════════════════════════════════════
    // DOCTOR DTOs
    // ══════════════════════════════════════════════════════

    public class DoctorDto
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

    public class CreateDoctorDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specialty is required.")]
        [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters.")]
        public string Specialty { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public int ExperienceYears { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        [StringLength(100, ErrorMessage = "Qualification cannot exceed 100 characters.")]
        public string Qualification { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(100, ErrorMessage = "Designation cannot exceed 100 characters.")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Mobile number must be 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class UpdateDoctorDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specialty is required.")]
        public string Specialty { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public int ExperienceYears { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        public string Qualification { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Mobile number must be 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;
    }


    // ══════════════════════════════════════════════════════
    // APPOINTMENT DTOs
    // ══════════════════════════════════════════════════════

    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "Doctor is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid doctor.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Time slot is required.")]
        public string TimeSlot { get; set; } = string.Empty;

        [Required(ErrorMessage = "Symptoms are required.")]
        [StringLength(500, ErrorMessage = "Symptoms cannot exceed 500 characters.")]
        public string Symptoms { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nature of visit is required.")]
        [StringLength(200, ErrorMessage = "Nature of visit cannot exceed 200 characters.")]
        public string NatureOfVisit { get; set; } = string.Empty;
    }

    public class AppointmentDto
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

        public string? CancellationReason { get; set; }

        public ConsultationDto? Consultation { get; set; }
    }

    public class RescheduleAppointmentDto
    {
        [Required(ErrorMessage = "New date is required.")]
        public DateTime NewDate { get; set; }

        [Required(ErrorMessage = "New time slot is required.")]
        public string NewTimeSlot { get; set; } = string.Empty;
    }

    public class CancelAppointmentDto
    {
        [Required(ErrorMessage = "Cancellation reason is required.")]
        [StringLength(300, ErrorMessage = "Reason cannot exceed 300 characters.")]
        public string Reason { get; set; } = string.Empty;
    }


    // ══════════════════════════════════════════════════════
    // CONSULTATION DTOs
    // ══════════════════════════════════════════════════════

    public class CreateConsultationDto
    {
        [Required(ErrorMessage = "Current symptoms are required.")]
        [StringLength(1000, ErrorMessage = "Symptoms cannot exceed 1000 characters.")]
        public string CurrentSymptoms { get; set; } = string.Empty;

        [Required(ErrorMessage = "Physical examination details are required.")]
        [StringLength(1000, ErrorMessage = "Cannot exceed 1000 characters.")]
        public string PhysicalExamination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(500, ErrorMessage = "Diagnosis cannot exceed 500 characters.")]
        public string Diagnosis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Treatment plan is required.")]
        [StringLength(1000, ErrorMessage = "Treatment plan cannot exceed 1000 characters.")]
        public string TreatmentPlan { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Recommended tests cannot exceed 500 characters.")]
        public string RecommendedTests { get; set; } = string.Empty;

        //public List<CreatePrescriptionDto> Prescriptions { get; set; }
        //    = new List<CreatePrescriptionDto>();
    }

    public class ConsultationDto
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public string CurrentSymptoms { get; set; } = string.Empty;
        public string PhysicalExamination { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string TreatmentPlan { get; set; } = string.Empty;
        public string RecommendedTests { get; set; } = string.Empty;
        public DateTime ConsultedAt { get; set; }
        //public List<PrescriptionDto> Prescriptions { get; set; }
        //    = new List<PrescriptionDto>();
    }


    // ══════════════════════════════════════════════════════
    // MEDICAL RECORD DTOs
    // ══════════════════════════════════════════════════════

    public class CreateMedicalRecordDto
    {
        [Required(ErrorMessage = "Patient is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid patient.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Recorded by is required.")]
        [StringLength(100, ErrorMessage = "Cannot exceed 100 characters.")]
        public string RecordedBy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Record type is required.")]
        [StringLength(100, ErrorMessage = "Record type cannot exceed 100 characters.")]
        public string RecordType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
        public string? AttachmentFileName { get; set; }

        [Required(ErrorMessage = "Record date is required.")]
        public DateTime RecordDate { get; set; }
    }

    public class MedicalRecordDto
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


    // ══════════════════════════════════════════════════════
    // PRESCRIPTION DTOs
    // ══════════════════════════════════════════════════════

    public class CreatePrescriptionDto
    {
        [Required(ErrorMessage = "Medicine name is required.")]
        [StringLength(200, ErrorMessage = "Medicine name cannot exceed 200 characters.")]
        public string MedicineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dosage is required.")]
        [RegularExpression(@"^\d-\d-\d$",
            ErrorMessage = "Dosage must be in format 0-0-1 (Morning-Afternoon-Night).")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Timing is required.")]
        [RegularExpression(@"^(AF|BF)$",
            ErrorMessage = "Timing must be AF (After Food) or BF (Before Food).")]
        public string Timing { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required.")]
        [StringLength(50, ErrorMessage = "Duration cannot exceed 50 characters.")]
        public string Duration { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Notes cannot exceed 300 characters.")]
        public string? Notes { get; set; }
    }

    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int ConsultationId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Timing { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class QueryParameters
    {
        // ── PAGINATION ────────────────────────────────────
        // Which page to show (starts from 1)
        public int PageNumber { get; set; } = 1;

        // How many records per page (max 50)
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            // If user sends more than 50, we cap it at 50
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        // ── SORTING ───────────────────────────────────────
        // Which field to sort by
        // Doctors:      "name", "experience", "specialty"
        // Patients:     "name", "date"
        // Appointments: "date", "status"
        public string? SortBy { get; set; }

        // asc  = A to Z / smallest to largest
        // desc = Z to A / largest to smallest
        public string SortDirection { get; set; } = "asc";
    }


    // ══════════════════════════════════════════════════════
    // PAGED RESPONSE
    // This class wraps the data with pagination information
    // Returned to client after every paginated request
    // ══════════════════════════════════════════════════════
    public class PagedResponse<T>
    {
        // The actual records for this page
        public List<T> Data { get; set; } = new List<T>();

        // Total records in DB (all pages combined)
        public int TotalCount { get; set; }

        // Current page number
        public int PageNumber { get; set; }

        // Records per page
        public int PageSize { get; set; }

        // Total number of pages
        // e.g. 15 records / 5 per page = 3 pages
        public int TotalPages => (int)Math.Ceiling(
                                     (double)TotalCount / PageSize);

        // Is there a page before this one?
        public bool HasPreviousPage => PageNumber > 1;

        // Is there a page after this one?
        public bool HasNextPage => PageNumber < TotalPages;
    }






    // ══════════════════════════════════════════════════════
    // COMMON RESPONSE WRAPPER
    // All API responses follow this consistent structure.
    // ══════════════════════════════════════════════════════
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        // Helper: create a success response
        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        // Helper: create a failure response
        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T> { Success = false, Message = message };
        }
    }

}



