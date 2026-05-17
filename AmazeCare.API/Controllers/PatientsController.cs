using AmazeCare.API.DTOS;

using AmazeCare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazeCare.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]    // Route: api/patients
    [Authorize]                    // All endpoints require login

    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }


        /// <summary>
        /// Get all patients - Admin only
        /// </summary>

        // GET api/patients  →  Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _patientService.GetAllPatientsAsync(query);
            return Ok(ApiResponse<PagedResponse<PatientDto>>.Ok(result));
        }




        /// <summary>
        /// Get logged in patient own profile
        /// </summary>

        // GET api/patients/me  →  Patient sees their own profile
        [HttpGet("me")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyProfile()
        {
            // Read logged-in patient's ID from JWT token claims
            int patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            return Ok(ApiResponse<PatientDto>.Ok(patient!));
        }



        /// <summary>
        /// Get patient by ID - Admin and Doctor only
        /// </summary>

        // GET api/patients/5  →  Admin or Doctor can view patient
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound(ApiResponse<PatientDto>.Fail("Patient not found."));

            return Ok(ApiResponse<PatientDto>.Ok(patient));
        }



        /// <summary>
        /// Update patient profile
        /// </summary>
        // PUT api/patients/5  →  Patient updates own profile or Admin updates
        [HttpPut("{id}")]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto dto)
        {
            var patient = await _patientService.UpdatePatientAsync(id, dto);
            return Ok(ApiResponse<PatientDto>.Ok(patient, "Profile updated."));
        }



        /// <summary>
        /// Delete patient - Admin only
        /// </summary>

        // DELETE api/patients/5  →  Admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail("Patient not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Patient deleted."));
        }


    }
}
