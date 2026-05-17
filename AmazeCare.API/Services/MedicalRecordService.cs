using AmazeCare.API.Data;
using AmazeCare.API.DTOS;
using AmazeCare.API.Models;
using AmazeCare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;




namespace AmazeCare.API.Services
{
    public class MedicalRecordService: IMedicalRecordService
    {
        private readonly AppDbContext _context;

        public MedicalRecordService(AppDbContext context)
        {
            _context = context;
        }


        // ── GET ALL RECORDS FOR A PATIENT ─────────────────────────
        // Used in: Patient Dashboard → "View Medical History"
        //          Doctor Dashboard → "View Patient Medical Records"
        public async Task<List<MedicalRecordDto>> GetByPatientIdAsync(int patientId)
        {
            // Eager Loading: Include Patient name using Include()
            return await _context.MedicalRecords
                .Include(m => m.Patient)
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.RecordDate)   // latest first
                .Select(m => new MedicalRecordDto
                {
                    RecordId = m.RecordId,
                    PatientId = m.PatientId,
                    PatientName = m.Patient.FullName,
                    RecordedBy = m.RecordedBy,
                    RecordType = m.RecordType,
                    Title = m.Title,
                    Description = m.Description,
                    AttachmentFileName = m.AttachmentFileName,
                    RecordDate = m.RecordDate,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }

        // ── GET ONE RECORD BY ID ──────────────────────────────────
        public async Task<MedicalRecordDto?> GetByIdAsync(int recordId)
        {
            var m = await _context.MedicalRecords
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.RecordId == recordId);

            if (m == null) return null;

            return new MedicalRecordDto
            {
                RecordId = m.RecordId,
                PatientId = m.PatientId,
                PatientName = m.Patient.FullName,
                RecordedBy = m.RecordedBy,
                RecordType = m.RecordType,
                Title = m.Title,
                Description = m.Description,
                AttachmentFileName = m.AttachmentFileName,
                RecordDate = m.RecordDate,
                CreatedAt = m.CreatedAt
            };
        }

        // ── ADD NEW MEDICAL RECORD ────────────────────────────────
        // Doctor adds blood report, X-ray result, diagnosis etc.
        public async Task<MedicalRecordDto> AddRecordAsync(CreateMedicalRecordDto dto)
        {
            // Make sure patient exists
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                throw new Exception("Patient not found.");

            var record = new MedicalRecord
            {
                PatientId = dto.PatientId,
                RecordedBy = dto.RecordedBy,
                RecordType = dto.RecordType,
                Title = dto.Title,
                Description = dto.Description,
                AttachmentFileName = dto.AttachmentFileName,
                RecordDate = dto.RecordDate,
                CreatedAt = DateTime.Now
            };

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            return new MedicalRecordDto
            {
                RecordId = record.RecordId,
                PatientId = record.PatientId,
                PatientName = patient.FullName,
                RecordedBy = record.RecordedBy,
                RecordType = record.RecordType,
                Title = record.Title,
                Description = record.Description,
                AttachmentFileName = record.AttachmentFileName,
                RecordDate = record.RecordDate,
                CreatedAt = record.CreatedAt
            };
        }
        // ── UPDATE MEDICAL RECORD ─────────────────────────────────
        public async Task<MedicalRecordDto> UpdateRecordAsync(int recordId, CreateMedicalRecordDto dto)
        {
            var record = await _context.MedicalRecords
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.RecordId == recordId);

            if (record == null)
                throw new Exception("Medical record not found.");

            // Update fields
            record.RecordedBy = dto.RecordedBy;
            record.RecordType = dto.RecordType;
            record.Title = dto.Title;
            record.Description = dto.Description;
            record.AttachmentFileName = dto.AttachmentFileName;
            record.RecordDate = dto.RecordDate;

            await _context.SaveChangesAsync();

            return new MedicalRecordDto
            {
                RecordId = record.RecordId,
                PatientId = record.PatientId,
                PatientName = record.Patient.FullName,
                RecordedBy = record.RecordedBy,
                RecordType = record.RecordType,
                Title = record.Title,
                Description = record.Description,
                AttachmentFileName = record.AttachmentFileName,
                RecordDate = record.RecordDate,
                CreatedAt = record.CreatedAt
            };
        }

        // ── DELETE MEDICAL RECORD ─────────────────────────────────
        public async Task<bool> DeleteRecordAsync(int recordId)
        {
            var record = await _context.MedicalRecords.FindAsync(recordId);
            if (record == null)
                return false;

            _context.MedicalRecords.Remove(record);  // Hard delete (real delete from DB)
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
