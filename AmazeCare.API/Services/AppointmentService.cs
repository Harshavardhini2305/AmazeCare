using AmazeCare.API.Data;
using AmazeCare.API.DTOS;
using AmazeCare.API.Models;
using AmazeCare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace AmazeCare.API.Services
{
    public class AppointmentService : IAppointmentService

    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {

            _context = context;

        }

        // Patient books a new appointment
        public async Task<AppointmentDto> BookAppointmentAsync(int patientId, CreateAppointmentDto dto)
        {
            // Validate patient exists
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
                throw new Exception("Patient not found.");

            // Validate doctor exists
            var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            // Check if the doctor already has an appointment at that date + time slot
            bool conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.AppointmentDate.Date == dto.AppointmentDate.Date &&
                a.TimeSlot == dto.TimeSlot &&
                a.Status != "Cancelled");

            if (conflict)
                throw new Exception("This time slot is already booked. Please choose another.");

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                TimeSlot = dto.TimeSlot,
                Symptoms = dto.Symptoms,
                NatureOfVisit = dto.NatureOfVisit,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return new AppointmentDto
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                PatientName = patient.FullName ?? "",
                PatientMobile = patient.MobileNumber ?? "",
                DoctorId = appointment.DoctorId,
                DoctorName = doctor.FullName ?? "",
                DoctorSpecialty = doctor.Specialty ?? "",
                AppointmentDate = appointment.AppointmentDate,
                TimeSlot = appointment.TimeSlot,
                Symptoms = appointment.Symptoms,
                NatureOfVisit = appointment.NatureOfVisit,
                Status = appointment.Status,
                CancellationReason = appointment.CancellationReason
            };
        }

        // Get all appointments for a patient
        public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(int patientId)
        {
            // Eager Loading: Include Doctor and Consultation data
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientMobile = a.Patient.MobileNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Symptoms = a.Symptoms,
                    NatureOfVisit = a.NatureOfVisit,
                    Status = a.Status,
                    CancellationReason = a.CancellationReason,
                    Consultation = a.Consultation == null ? null : new ConsultationDto
                    {
                        ConsultationId = a.Consultation.ConsultationId,
                        CurrentSymptoms = a.Consultation.CurrentSymptoms,
                        PhysicalExamination = a.Consultation.PhysicalExamination,
                        Diagnosis = a.Consultation.Diagnosis,
                        TreatmentPlan = a.Consultation.TreatmentPlan,
                        RecommendedTests = a.Consultation.RecommendedTests,
                        //Prescription = a.Consultation.Prescription,
                        ConsultedAt = a.Consultation.ConsultedAt
                    }
                })
                .ToListAsync();
        }

        // Get all appointments for a doctor
        public async Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId)
        {
            // Eager Loading: Include Patient and Consultation
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Consultation)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientMobile = a.Patient.MobileNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Symptoms = a.Symptoms,
                    NatureOfVisit = a.NatureOfVisit,
                    Status = a.Status,
                    CancellationReason = a.CancellationReason,
                    Consultation = a.Consultation == null ? null : new ConsultationDto
                    {
                        ConsultationId = a.Consultation.ConsultationId,
                        CurrentSymptoms = a.Consultation.CurrentSymptoms,
                        PhysicalExamination = a.Consultation.PhysicalExamination,
                        Diagnosis = a.Consultation.Diagnosis,
                        TreatmentPlan = a.Consultation.TreatmentPlan,
                        RecommendedTests = a.Consultation.RecommendedTests,
                        //Prescription = a.Consultation.Prescription,
                        ConsultedAt = a.Consultation.ConsultedAt
                    }
                })
                .ToListAsync();
        }

        // Admin: Get all appointments
        public async Task<PagedResponse<AppointmentDto>> GetAllAppointmentsAsync(
           QueryParameters query)
        {
            // Step 1: Start with ALL appointments
            var appointmentsQuery = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            // ── SORTING ───────────────────────────────────
            // Step 2: Apply sorting BEFORE pagination
            appointmentsQuery = query.SortBy?.ToLower() switch
            {

                "id" => query.SortDirection == "desc"
       ? appointmentsQuery.OrderByDescending(a => a.AppointmentId)
       : appointmentsQuery.OrderBy(a => a.AppointmentId),

                // Sort by appointment date
                "date" => query.SortDirection == "desc"
                    ? appointmentsQuery.OrderByDescending(a => a.AppointmentDate)
                    : appointmentsQuery.OrderBy(a => a.AppointmentDate),

                // Sort by status (Pending, Confirmed, Completed, Cancelled)
                "status" => query.SortDirection == "desc"
                    ? appointmentsQuery.OrderByDescending(a => a.Status.ToLower())
                    : appointmentsQuery.OrderBy(a => a.Status.ToLower()),

                // Sort by patient name
                "patient" => query.SortDirection == "desc"
                    ? appointmentsQuery.OrderByDescending(a => a.Patient.FullName.ToLower())
                    : appointmentsQuery.OrderBy(a => a.Patient.FullName.ToLower()),

                // Sort by doctor name
                "doctor" => query.SortDirection == "desc"
                    ? appointmentsQuery.OrderByDescending(a => a.Doctor.FullName.ToLower())
                    : appointmentsQuery.OrderBy(a => a.Doctor.FullName.ToLower()),

                // Default: sort by date newest first
                _ => appointmentsQuery.OrderByDescending(a => a.AppointmentDate)
            };

            // ── PAGINATION ────────────────────────────────
            // Step 3: Count TOTAL records BEFORE paging
            int totalCount = await appointmentsQuery.CountAsync();

            // Step 4: Skip previous pages, Take current page only
            var appointments = await appointmentsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientMobile = a.Patient.MobileNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Symptoms = a.Symptoms,
                    NatureOfVisit = a.NatureOfVisit,
                    Status = a.Status,
                    CancellationReason = a.CancellationReason,
                })
                .ToListAsync();

            // Step 5: Return paged response
            return new PagedResponse<AppointmentDto>
            {
                Data = appointments,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }


        // Get one appointment with full details
        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            var a = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (a == null) return null;

            return new AppointmentDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                PatientMobile = a.Patient.MobileNumber,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                DoctorSpecialty = a.Doctor.Specialty,
                AppointmentDate = a.AppointmentDate,
                TimeSlot = a.TimeSlot,
                Symptoms = a.Symptoms,
                NatureOfVisit = a.NatureOfVisit,
                Status = a.Status,
                CancellationReason = a.CancellationReason,
                Consultation = a.Consultation == null ? null : new ConsultationDto
                {
                    ConsultationId = a.Consultation.ConsultationId,
                    CurrentSymptoms = a.Consultation.CurrentSymptoms,
                    PhysicalExamination = a.Consultation.PhysicalExamination,
                    Diagnosis = a.Consultation.Diagnosis,
                    TreatmentPlan = a.Consultation.TreatmentPlan,
                    RecommendedTests = a.Consultation.RecommendedTests,
                    //Prescription = a.Consultation.Prescription,
                    ConsultedAt = a.Consultation.ConsultedAt
                }
            };
        }
        // Reschedule an appointment
        public async Task<AppointmentDto> RescheduleAsync(int id, RescheduleAppointmentDto dto)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
                throw new Exception("Cannot reschedule a completed or cancelled appointment.");

            // Check for conflict at new slot (excluding current appointment)
            bool conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appointment.DoctorId &&
                a.AppointmentDate.Date == dto.NewDate.Date &&
                a.TimeSlot == dto.NewTimeSlot &&
                a.Status != "Cancelled" &&
                a.AppointmentId != id);

            if (conflict)
                throw new Exception("New time slot is already booked. Please choose another.");

            appointment.AppointmentDate = dto.NewDate;
            appointment.TimeSlot = dto.NewTimeSlot;
            appointment.Status = "Pending";

            await _context.SaveChangesAsync();

            return new AppointmentDto
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient.FullName,
                DoctorName = appointment.Doctor.FullName,
                AppointmentDate = appointment.AppointmentDate,
                TimeSlot = appointment.TimeSlot,
                Status = appointment.Status
            };
        }
        // Cancel an appointment
        public async Task<bool> CancelAsync(int id, CancelAppointmentDto dto)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.Status == "Completed")
                throw new Exception("Cannot cancel a completed appointment.");

            appointment.Status = "Cancelled";
            appointment.CancellationReason = dto.Reason;

            await _context.SaveChangesAsync();
            return true;
        }

        // Doctor confirms an appointment
        public async Task<bool> ConfirmAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            appointment.Status = "Confirmed";
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Patient Upcoming Appointments
        /// </summary>
      

        // Patient Upcoming Appointments
        public async Task<List<AppointmentDto>> GetPatientUpcomingAppointmentsAsync(
    int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId
                        && (a.Status == "Pending" || a.Status == "Confirmed")
                        && a.AppointmentDate >= DateTime.Today)  // future dates only
                .OrderBy(a => a.AppointmentDate)   // nearest date first
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientMobile = a.Patient.MobileNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Symptoms = a.Symptoms,
                    NatureOfVisit = a.NatureOfVisit,
                    Status = a.Status
                })
                .ToListAsync();
        }

        /// <summary>
        /// Patient Completed Appointments
       ///</summary>
        // Patient Completed Appointments
        public async Task<List<AppointmentDto>> GetPatientCompletedAppointmentsAsync(
            int patientId)
        {
            return await _context.Appointments
       .Include(a => a.Doctor)
       .Include(a => a.Consultation)
           .ThenInclude(c => c.Prescriptions)  // include medicines too
       .Where(a => a.PatientId == patientId
               && a.Status == "Completed")
       .OrderByDescending(a => a.AppointmentDate)  // latest first
       .Select(a => new AppointmentDto
       {
           AppointmentId = a.AppointmentId,
           PatientId = a.PatientId,
           PatientName = a.Patient.FullName,
           PatientMobile = a.Patient.MobileNumber,
           DoctorId = a.DoctorId,
           DoctorName = a.Doctor.FullName,
           DoctorSpecialty = a.Doctor.Specialty,
           AppointmentDate = a.AppointmentDate,
           TimeSlot = a.TimeSlot,
           Symptoms = a.Symptoms,
           NatureOfVisit = a.NatureOfVisit,
           Status = a.Status,
           Consultation = a.Consultation == null ? null : new ConsultationDto
           {
               ConsultationId = a.Consultation.ConsultationId,
               CurrentSymptoms = a.Consultation.CurrentSymptoms,
               PhysicalExamination = a.Consultation.PhysicalExamination,
               Diagnosis = a.Consultation.Diagnosis,
               TreatmentPlan = a.Consultation.TreatmentPlan,
               RecommendedTests = a.Consultation.RecommendedTests,
               ConsultedAt = a.Consultation.ConsultedAt,
               //Prescriptions = a.Consultation.Prescriptions
               //    .Select(p => new PrescriptionDto
               //    {
               //        PrescriptionId = p.PrescriptionId,
               //        ConsultationId = p.ConsultationId,
               //        MedicineName = p.MedicineName,
               //        Dosage = p.Dosage,
               //        Timing = p.Timing,
               //        Duration = p.Duration,
               //        Notes = p.Notes
               //    }).ToList()
           }
       })
       .ToListAsync();
        }

        /// <summary>
        /// Doctor Upcoming Appointments
        /// </summary>
        

        // Doctor Upcoming Appointments
        public async Task<List<AppointmentDto>> GetDoctorUpcomingAppointmentsAsync(
            int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId
                        && (a.Status == "Pending" || a.Status == "Confirmed")
                        && a.AppointmentDate >= DateTime.Today)
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientMobile = a.Patient.MobileNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Symptoms = a.Symptoms,
                    NatureOfVisit = a.NatureOfVisit,
                    Status = a.Status
                })
                .ToListAsync();
        }

        /// <summary>
        /// Doctor Completed Appointments
        /// </summary>


        // Doctor Completed Appointments
        public async Task<List<AppointmentDto>> GetDoctorCompletedAppointmentsAsync(
            int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Consultation)
                    .ThenInclude(c => c.Prescriptions)
                .Where(a => a.DoctorId == doctorId
                        && a.Status == "Completed")
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientMobile = a.Patient.MobileNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialty = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Symptoms = a.Symptoms,
                    NatureOfVisit = a.NatureOfVisit,
                    Status = a.Status,
                    Consultation = a.Consultation == null ? null : new ConsultationDto
                    {
                        ConsultationId = a.Consultation.ConsultationId,
                        CurrentSymptoms = a.Consultation.CurrentSymptoms,
                        PhysicalExamination = a.Consultation.PhysicalExamination,
                        Diagnosis = a.Consultation.Diagnosis,
                        TreatmentPlan = a.Consultation.TreatmentPlan,
                        RecommendedTests = a.Consultation.RecommendedTests,
                        ConsultedAt = a.Consultation.ConsultedAt,
                        //Prescriptions = a.Consultation.Prescriptions
                        //    .Select(p => new PrescriptionDto
                        //    {
                        //        PrescriptionId = p.PrescriptionId,
                        //        ConsultationId = p.ConsultationId,
                        //        MedicineName = p.MedicineName,
                        //        Dosage = p.Dosage,
                        //        Timing = p.Timing,
                        //        Duration = p.Duration,
                        //        Notes = p.Notes
                        //    }).ToList()
                    }
                })
                .ToListAsync();
        }




    }
}
