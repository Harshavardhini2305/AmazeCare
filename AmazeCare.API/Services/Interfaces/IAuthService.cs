using AmazeCare.API.DTOS;

namespace AmazeCare.API.Services.Interfaces
{
   
        public interface IAuthService
        {
            Task<LoginResponseDto> RegisterPatientAsync(RegisterPatientDto dto);
            Task<LoginResponseDto> LoginPatientAsync(LoginDto dto);
            Task<LoginResponseDto> LoginDoctorAsync(LoginDto dto);
            Task<LoginResponseDto> LoginAdminAsync(LoginDto dto);
        }
    
}
