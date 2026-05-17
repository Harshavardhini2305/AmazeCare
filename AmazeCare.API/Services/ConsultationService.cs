using AmazeCare.API.Data;

using AmazeCare.API.DTOS;
using AmazeCare.API.Models;
using AmazeCare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.API.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly AppDbContext _context;

        public ConsultationService(AppDbContext context)
        {
            _context = context;
        }

        // ── ADD CONSULTATION (no prescriptions here) ──────
        public async Task<ConsultationDto> AddConsultationAsync(
            int appointmentId, CreateConsultationDto dto)
        {
            // Check appointment exists
            var appointment = await _context.Appointments
                .FindAsync(appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found.");


            //// ✅ Add this check if missing
            //if (appointment.Status != "Confirmed")
            //    throw new Exception(
            //        "Appointment must be Confirmed before adding consultation.");

            // Check consultation not already exists
            bool alreadyExists = await _context.Consultations
                .AnyAsync(c => c.AppointmentId == appointmentId);
            if (alreadyExists)
                throw new Exception(
                    "Consultation already recorded for this appointment.");

            // Create and Save Consultation
            var consultation = new Consultation
            {
                AppointmentId = appointmentId,
                CurrentSymptoms = dto.CurrentSymptoms,
                PhysicalExamination = dto.PhysicalExamination,
                Diagnosis = dto.Diagnosis,
                TreatmentPlan = dto.TreatmentPlan,
                RecommendedTests = dto.RecommendedTests,
                ConsultedAt = DateTime.Now
            };

            _context.Consultations.Add(consultation);

            // Mark appointment as Completed
            appointment.Status = "Completed";

            await _context.SaveChangesAsync();

            // Return ConsultationDto
            return new ConsultationDto
            {
                ConsultationId = consultation.ConsultationId,
                AppointmentId = consultation.AppointmentId,
                CurrentSymptoms = consultation.CurrentSymptoms,
                PhysicalExamination = consultation.PhysicalExamination,
                Diagnosis = consultation.Diagnosis,
                TreatmentPlan = consultation.TreatmentPlan,
                RecommendedTests = consultation.RecommendedTests,
                ConsultedAt = consultation.ConsultedAt
            };
        }

        // ── GET CONSULTATION BY APPOINTMENT ID ────────────
        public async Task<ConsultationDto?> GetByAppointmentIdAsync(
            int appointmentId)
        {
            var c = await _context.Consultations
                .FirstOrDefaultAsync(c => c.AppointmentId == appointmentId);

            if (c == null) return null;

            return new ConsultationDto
            {
                ConsultationId = c.ConsultationId,
                AppointmentId = c.AppointmentId,
                CurrentSymptoms = c.CurrentSymptoms,
                PhysicalExamination = c.PhysicalExamination,
                Diagnosis = c.Diagnosis,
                TreatmentPlan = c.TreatmentPlan,
                RecommendedTests = c.RecommendedTests,
                ConsultedAt = c.ConsultedAt
            };
        }

        // ── UPDATE CONSULTATION ───────────────────────────
        public async Task<ConsultationDto> UpdateConsultationAsync(
            int consultationId, CreateConsultationDto dto)
        {
            var consultation = await _context.Consultations
                .FindAsync(consultationId);

            if (consultation == null)
                throw new Exception("Consultation not found.");

            consultation.CurrentSymptoms = dto.CurrentSymptoms;
            consultation.PhysicalExamination = dto.PhysicalExamination;
            consultation.Diagnosis = dto.Diagnosis;
            consultation.TreatmentPlan = dto.TreatmentPlan;
            consultation.RecommendedTests = dto.RecommendedTests;

            await _context.SaveChangesAsync();

            return new ConsultationDto
            {
                ConsultationId = consultation.ConsultationId,
                AppointmentId = consultation.AppointmentId,
                CurrentSymptoms = consultation.CurrentSymptoms,
                PhysicalExamination = consultation.PhysicalExamination,
                Diagnosis = consultation.Diagnosis,
                TreatmentPlan = consultation.TreatmentPlan,
                RecommendedTests = consultation.RecommendedTests,
                ConsultedAt = consultation.ConsultedAt
            };
        }
    }
}