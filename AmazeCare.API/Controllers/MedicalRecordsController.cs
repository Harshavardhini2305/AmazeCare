using AmazeCare.API.DTOS;
using AmazeCare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazeCare.API.Controllers
{
    // ─────────────────────────────────────────────────────────
    // MedicalRecordsController
    // Routes:
    //   GET    /api/medicalrecords/patient/{patientId}  → View patient history
    //   GET    /api/medicalrecords/{id}                 → View single record
    //   POST   /api/medicalrecords                      → Add new record
    //   PUT    /api/medicalrecords/{id}                 → Update record
    //   DELETE /api/medicalrecords/{id}                 → Delete record
    // ─────────────────────────────────────────────────────────
    [ApiController]
    [Route("api/[controller]")]    // api/medicalrecords
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _recordService;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(
            IMedicalRecordService recordService,
            ILogger<MedicalRecordsController> logger)
        {
            _recordService = recordService;
            _logger = logger;
        }


        /// <summary>
        /// Get all medical records of a patient
        /// </summary>

        // GET api/medicalrecords/patient/3
        // Patient sees their own records
        // Doctor/Admin can see any patient's records
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            // If role is Patient, only allow viewing own records
            if (User.IsInRole("Patient"))
            {
                int loggedInPatientId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (loggedInPatientId != patientId)
                    return Forbid(); // 403 - cannot see another patient's records
            }

            var records = await _recordService.GetByPatientIdAsync(patientId);
            return Ok(ApiResponse<List<MedicalRecordDto>>.Ok(records));
        }


        /// <summary>
        /// Get a single medical record by ID
        /// </summary>

        // GET api/medicalrecords/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _recordService.GetByIdAsync(id);
            if (record == null)
                return NotFound(ApiResponse<MedicalRecordDto>.Fail("Medical record not found."));

            return Ok(ApiResponse<MedicalRecordDto>.Ok(record));
        }


        /// <summary>
        /// Add a new medical record for a patient - Doctor and Admin only
        /// </summary>

        // POST api/medicalrecords
        // Only Doctor or Admin can add records
        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Add([FromBody] CreateMedicalRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation(
                "Adding medical record for Patient {PatientId}, Type: {Type}",
                dto.PatientId, dto.RecordType);

            var record = await _recordService.AddRecordAsync(dto);
            return Ok(ApiResponse<MedicalRecordDto>.Ok(record, "Medical record added."));
        }


        /// <summary>
        /// Update a medical record - Doctor and Admin only
        /// </summary>

        // PUT api/medicalrecords/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMedicalRecordDto dto)
        {
            var record = await _recordService.UpdateRecordAsync(id, dto);
            return Ok(ApiResponse<MedicalRecordDto>.Ok(record, "Medical record updated."));
        }


        /// <summary>
        /// Delete a medical record - Admin only
        /// </summary>

        // DELETE api/medicalrecords/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recordService.DeleteRecordAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail("Medical record not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Medical record deleted."));
        }
    }

}
