using AmazeCare.API.DTOS;


namespace AmazeCare.API.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> BookAppointmentAsync(int patientId, CreateAppointmentDto dto);
        Task<List<AppointmentDto>> GetPatientAppointmentsAsync(int patientId);
        Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId);
        Task<PagedResponse<AppointmentDto>> GetAllAppointmentsAsync(QueryParameters query);
        Task<AppointmentDto?> GetAppointmentByIdAsync(int id);
        Task<AppointmentDto> RescheduleAsync(int id, RescheduleAppointmentDto dto);
        Task<bool> CancelAsync(int id, CancelAppointmentDto dto);
        Task<bool> ConfirmAsync(int id);

        Task<List<AppointmentDto>> GetPatientUpcomingAppointmentsAsync(int patientId);
        Task<List<AppointmentDto>> GetPatientCompletedAppointmentsAsync(int patientId);
        Task<List<AppointmentDto>> GetDoctorUpcomingAppointmentsAsync(int doctorId);
        Task<List<AppointmentDto>> GetDoctorCompletedAppointmentsAsync(int doctorId);
    }
}
