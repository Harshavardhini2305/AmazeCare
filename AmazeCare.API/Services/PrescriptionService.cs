using AmazeCare.API.Data;
using AmazeCare.API.DTOS;
using AmazeCare.API.Models;
using AmazeCare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace AmazeCare.API.Services
{
    public class PrescriptionService: IPrescriptionService
    {
        private readonly AppDbContext _context;

        public PrescriptionService(AppDbContext context)
        {
            _context = context;
        }

        // ── GET ALL PRESCRIPTIONS FOR A CONSULTATION ──────────────
        // Used in: Doctor Dashboard → "View Completed Appointment Prescription"
        //          Patient Dashboard → "View Prescription from Doctor"
        public async Task<List<PrescriptionDto>> GetByConsultationIdAsync(int consultationId)
        {
            return await _context.Prescriptions
                .Where(p => p.ConsultationId == consultationId)
                .Select(p => new PrescriptionDto
                {
                    PrescriptionId = p.PrescriptionId,
                    ConsultationId = p.ConsultationId,
                    MedicineName = p.MedicineName,
                    Dosage = p.Dosage,
                    Timing = p.Timing,
                    Duration = p.Duration,
                    Notes = p.Notes
                })
                .ToListAsync();
        }

        // ── GET ONE PRESCRIPTION BY ID ────────────────────────────
        public async Task<PrescriptionDto?> GetByIdAsync(int prescriptionId)
        {
            var p = await _context.Prescriptions.FindAsync(prescriptionId);
            if (p == null) return null;

            return new PrescriptionDto
            {
                PrescriptionId = p.PrescriptionId,
                ConsultationId = p.ConsultationId,
                MedicineName = p.MedicineName,
                Dosage = p.Dosage,
                Timing = p.Timing,
                Duration = p.Duration,
                Notes = p.Notes
            };
        }

        // ── ADD ONE MEDICINE TO A CONSULTATION ───────────────────
        // e.g. Add "Paracetamol 500mg | 1-1-1 | AF | 5 days"
        public async Task<PrescriptionDto> AddPrescriptionAsync(
            int consultationId, CreatePrescriptionDto dto)
        {
            // Check consultation exists
            var consultation = await _context.Consultations.FindAsync(consultationId);
            if (consultation == null)
                throw new Exception("Consultation not found.");

            var prescription = new Prescription
            {
                ConsultationId = consultationId,
                MedicineName = dto.MedicineName,
                Dosage = dto.Dosage,
                Timing = dto.Timing,
                Duration = dto.Duration,
                Notes = dto.Notes
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return new PrescriptionDto
            {
                PrescriptionId = prescription.PrescriptionId,
                ConsultationId = prescription.ConsultationId,
                MedicineName = prescription.MedicineName,
                Dosage = prescription.Dosage,
                Timing = prescription.Timing,
                Duration = prescription.Duration,
                Notes = prescription.Notes
            };
        }
        // ── ADD MULTIPLE MEDICINES AT ONCE ───────────────────────
        // Doctor typically prescribes 2-5 medicines at once.
        // This saves all of them in one call.
        public async Task<List<PrescriptionDto>> AddManyPrescriptionsAsync(
            int consultationId, List<CreatePrescriptionDto> prescriptions)
        {
            // Check consultation exists
            var consultation = await _context.Consultations.FindAsync(consultationId);
            if (consultation == null)
                throw new Exception("Consultation not found.");

            if (prescriptions == null || prescriptions.Count == 0)
                throw new Exception("At least one prescription item is required.");

            var entities = new List<Prescription>();

            // Convert each DTO to entity
            foreach (var dto in prescriptions)
            {
                entities.Add(new Prescription
                {
                    ConsultationId = consultationId,
                    MedicineName = dto.MedicineName,
                    Dosage = dto.Dosage,
                    Timing = dto.Timing,
                    Duration = dto.Duration,
                    Notes = dto.Notes
                });
            }

            // Add all at once (efficient - one DB call)
            _context.Prescriptions.AddRange(entities);
            await _context.SaveChangesAsync();

            // Return as DTOs
            return entities.Select(p => new PrescriptionDto
            {
                PrescriptionId = p.PrescriptionId,
                ConsultationId = p.ConsultationId,
                MedicineName = p.MedicineName,
                Dosage = p.Dosage,
                Timing = p.Timing,
                Duration = p.Duration,
                Notes = p.Notes
            }).ToList();
        }

        // ── UPDATE ONE MEDICINE ───────────────────────────────────
        // Doctor changes dosage or duration for a medicine
        public async Task<PrescriptionDto> UpdatePrescriptionAsync(
            int prescriptionId, CreatePrescriptionDto dto)
        {
            var prescription = await _context.Prescriptions.FindAsync(prescriptionId);
            if (prescription == null)
                throw new Exception("Prescription not found.");

            prescription.MedicineName = dto.MedicineName;
            prescription.Dosage = dto.Dosage;
            prescription.Timing = dto.Timing;
            prescription.Duration = dto.Duration;
            prescription.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return new PrescriptionDto
            {
                PrescriptionId = prescription.PrescriptionId,
                ConsultationId = prescription.ConsultationId,
                MedicineName = prescription.MedicineName,
                Dosage = prescription.Dosage,
                Timing = prescription.Timing,
                Duration = prescription.Duration,
                Notes = prescription.Notes
            };
        }


        // ── DELETE ONE MEDICINE FROM PRESCRIPTION ────────────────
        public async Task<bool> DeletePrescriptionAsync(int prescriptionId)
        {
            var prescription = await _context.Prescriptions.FindAsync(prescriptionId);
            if (prescription == null)
                return false;

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
