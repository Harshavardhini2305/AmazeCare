using AmazeCare.API.DTOS;
namespace AmazeCare.API.Services.Interfaces
{
    public interface IPrescriptionService
    {
        // Get all prescriptions for a consultation
        Task<List<PrescriptionDto>> GetByConsultationIdAsync(int consultationId);

        // Get one prescription by ID
        Task<PrescriptionDto?> GetByIdAsync(int prescriptionId);

        // Doctor adds one medicine to a consultation
        Task<PrescriptionDto> AddPrescriptionAsync(int consultationId, CreatePrescriptionDto dto);

        // Doctor adds MULTIPLE medicines at once (full prescription list)
        Task<List<PrescriptionDto>> AddManyPrescriptionsAsync(
            int consultationId, List<CreatePrescriptionDto> prescriptions);

        // Update one medicine in prescription
        Task<PrescriptionDto> UpdatePrescriptionAsync(int prescriptionId, CreatePrescriptionDto dto);

        // Remove one medicine from prescription
        Task<bool> DeletePrescriptionAsync(int prescriptionId);
    }
}
