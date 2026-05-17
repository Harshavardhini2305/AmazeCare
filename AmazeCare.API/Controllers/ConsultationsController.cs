using AmazeCare.API.DTOS;
using AmazeCare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AmazeCare.API.Controllers
{
    [ApiController]                    
    [Route("api/[controller]")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationsController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        /// <summary>
        /// Add consultation details for an appointment - Doctor only
        /// </summary>

        // POST api/appointments/5/consultation  →  Doctor adds consultation
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Add(int appointmentId, [FromBody] CreateConsultationDto dto)
        {
            var result = await _consultationService.AddConsultationAsync(appointmentId, dto);
            return Ok(ApiResponse<ConsultationDto>.Ok(result, "Consultation saved."));
        }


        /// <summary>
        /// Get consultation details for an appointment
        /// </summary>

        // GET api/appointments/5/consultation  →  View consultation
        [HttpGet]
        public async Task<IActionResult> Get(int appointmentId)
        {
            var result = await _consultationService.GetByAppointmentIdAsync(appointmentId);
            if (result == null)
                return NotFound(ApiResponse<ConsultationDto>.Fail("No consultation found."));

            return Ok(ApiResponse<ConsultationDto>.Ok(result));
        }


        /// <summary>
        /// Update consultation details - Doctor only
        /// </summary>

        // PUT api/appointments/5/consultation/2  →  Doctor updates consultation
        [HttpPut("{consultationId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Update(int appointmentId, int consultationId,
            [FromBody] CreateConsultationDto dto)
        {
            var result = await _consultationService.UpdateConsultationAsync(consultationId, dto);
            return Ok(ApiResponse<ConsultationDto>.Ok(result, "Consultation updated."));
        }


    }
}
