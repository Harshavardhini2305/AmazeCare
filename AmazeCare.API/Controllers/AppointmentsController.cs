using AmazeCare.API.DTOS;
using AmazeCare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazeCare.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]    // Route: api/appointments
    [Authorize]

    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }




        /// <summary>
        /// Book a new appointment - Patient only
        /// </summary>


        // POST api/appointments  →  Patient books appointment
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Book([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get patient ID from JWT token (not from body - more secure)
            int patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            _logger.LogInformation("Patient {PatientId} booking appointment with Doctor {DoctorId}",
                patientId, dto.DoctorId);

            var result = await _appointmentService.BookAppointmentAsync(patientId, dto);
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Appointment booked successfully."));
        }


        /// <summary>
        /// Get all appointments of logged in patient
        /// </summary>

        // GET api/appointments/my  →  Patient sees their appointments
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyAppointments()
        {
            int patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var list = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            return Ok(ApiResponse<List<AppointmentDto>>.Ok(list));
        }


        /// <summary>
        /// Get all appointments of a specific doctor
        /// </summary>
        // GET api/appointments/doctor/3  →  Doctor sees their appointments
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetByDoctor(int doctorId)
        {
            var list = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
            return Ok(ApiResponse<List<AppointmentDto>>.Ok(list));
        }


        /// <summary>
        /// Get all appointments - Admin only
        /// </summary>

        // GET api/appointments  →  Admin sees all appointments
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _appointmentService.GetAllAppointmentsAsync(query);
            return Ok(ApiResponse<PagedResponse<AppointmentDto>>.Ok(result));
        }



        /// <summary>
        /// Get one appointment with full deatils
        /// </summary>
        // GET api/appointments/5  →  Get one appointment with full details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<AppointmentDto>.Fail("Appointment not found."));

            return Ok(ApiResponse<AppointmentDto>.Ok(result));
        }


        /// <summary>
        /// Reschedule an appointment
        /// </summary>

        // PUT api/appointments/5/reschedule  →  Patient or Admin reschedules
        [HttpPut("{id}/reschedule")]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleAppointmentDto dto)
        {
            var result = await _appointmentService.RescheduleAsync(id, dto);
            return Ok(ApiResponse<AppointmentDto>.Ok(result, "Appointment rescheduled."));
        }


        /// <summary>
        /// Cancel an appointment
        /// </summary>

        // PUT api/appointments/5/cancel  →  Patient, Doctor, or Admin cancels
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelAppointmentDto dto)
        {
            await _appointmentService.CancelAsync(id, dto);
            return Ok(ApiResponse<string>.Ok("Cancelled", "Appointment cancelled."));
        }

        /// <summary>
        /// Confirm an appointment - Doctor only
        /// </summary>

        // PUT api/appointments/5/confirm  →  Doctor confirms appointment
        [HttpPut("{id}/confirm")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _appointmentService.ConfirmAsync(id);
            return Ok(ApiResponse<string>.Ok("Confirmed", "Appointment confirmed."));
        }


        /// <summary>
        /// Get patient upcoming appointments
        /// </summary>
        [HttpGet("my/upcoming")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyUpcoming()
        {
            int patientId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _appointmentService
                .GetPatientUpcomingAppointmentsAsync(patientId);

            return Ok(ApiResponse<List<AppointmentDto>>.Ok(result));
        }

        /// <summary>
        /// Get patient completed appointments with consultation
        /// </summary>
        [HttpGet("my/completed")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyCompleted()
        {
            int patientId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _appointmentService
                .GetPatientCompletedAppointmentsAsync(patientId);

            return Ok(ApiResponse<List<AppointmentDto>>.Ok(result));
        }


        /// <summary>
        /// Get doctor upcoming appointments
        /// </summary>
        [HttpGet("doctor/{doctorId}/upcoming")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetDoctorUpcoming(int doctorId)
        {
            var result = await _appointmentService
                .GetDoctorUpcomingAppointmentsAsync(doctorId);

            return Ok(ApiResponse<List<AppointmentDto>>.Ok(result));
        }

        /// <summary>
        /// Get doctor completed appointments with prescription
        /// </summary>
        [HttpGet("doctor/{doctorId}/completed")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetDoctorCompleted(int doctorId)
        {
            var result = await _appointmentService
                .GetDoctorCompletedAppointmentsAsync(doctorId);

            return Ok(ApiResponse<List<AppointmentDto>>.Ok(result));
        }

    }
}
