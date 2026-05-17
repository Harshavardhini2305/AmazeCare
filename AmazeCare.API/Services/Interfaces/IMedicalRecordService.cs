using AmazeCare.API.DTOS;

namespace AmazeCare.API.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        // Get all records for a specific patient
        Task<List<MedicalRecordDto>> GetByPatientIdAsync(int patientId);

        // Get one record by its ID
        Task<MedicalRecordDto?> GetByIdAsync(int recordId);

        // Doctor or Admin adds a new record
        Task<MedicalRecordDto> AddRecordAsync(CreateMedicalRecordDto dto);

        // Update an existing record
        Task<MedicalRecordDto> UpdateRecordAsync(int recordId, CreateMedicalRecordDto dto);

        // Delete a record
        Task<bool> DeleteRecordAsync(int recordId);
    }
}
