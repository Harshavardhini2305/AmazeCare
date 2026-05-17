using AmazeCare.API.DTOS;
namespace AmazeCare.API.Services.Interfaces
{
    
        public interface IPatientService
        {
        Task<PagedResponse<PatientDto>> GetAllPatientsAsync(QueryParameters query);
        Task<PatientDto?> GetPatientByIdAsync(int id);
            Task<PatientDto> UpdatePatientAsync(int id, UpdatePatientDto dto);
            Task<bool> DeletePatientAsync(int id);
        }
    
}
