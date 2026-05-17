using AmazeCare.API.DTOS;

namespace AmazeCare.API.Services.Interfaces
{
    
        public interface IDoctorService
        {
        Task<PagedResponse<DoctorDto>> GetAllDoctorsAsync(QueryParameters query);
        Task<List<DoctorDto>> SearchDoctorsAsync(string? specialty);
            Task<DoctorDto?> GetDoctorByIdAsync(int id);
            Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto dto);
            Task<DoctorDto> UpdateDoctorAsync(int id, UpdateDoctorDto dto);
            Task<bool> DeleteDoctorAsync(int id);
        }
    
}
