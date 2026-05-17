using AmazeCare.API.DTOS;

namespace AmazeCare.API.Services.Interfaces
{
    public interface IConsultationService
    {
        Task<ConsultationDto> AddConsultationAsync(int appointmentId, CreateConsultationDto dto);
        Task<ConsultationDto?> GetByAppointmentIdAsync(int appointmentId);
        Task<ConsultationDto> UpdateConsultationAsync(int consultationId, CreateConsultationDto dto);
    }
}
