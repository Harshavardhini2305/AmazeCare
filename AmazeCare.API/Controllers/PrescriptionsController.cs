using AmazeCare.API.DTOS;
using AmazeCare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazeCare.API.Controllers
{
    // ─────────────────────────────────────────────────────────
    // PrescriptionsController
    // Routes:
    //   GET    /api/prescriptions/consultation/{consultationId}  → View all medicines
    //   GET    /api/prescriptions/{id}                           → View one medicine
    //   POST   /api/prescriptions/consultation/{consultationId}  → Add one medicine
    //   POST   /api/prescriptions/consultation/{id}/bulk         → Add all medicines
    //   PUT    /api/prescriptions/{id}                           → Update one medicine
    //   DELETE /api/prescriptions/{id}                           → Remove one medicine
    // ─────────────────────────────────────────────────────────
    [ApiController]
    [Route("api/[controller]")]    // api/prescriptions
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            ILogger<PrescriptionsController> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }


        /// <summary>
        /// Get all prescriptions for a consultation
        /// </summary>


        // GET api/prescriptions/consultation/5
        // View all medicines prescribed in a consultation
        // Patient, Doctor, Admin can view
        [HttpGet("consultation/{consultationId}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> GetByConsultation(int consultationId)
        {
            var prescriptions = await _prescriptionService.GetByConsultationIdAsync(consultationId);
            return Ok(ApiResponse<List<PrescriptionDto>>.Ok(prescriptions));
        }

      

        // GET api/prescriptions/7
        [HttpGet("{id}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound(ApiResponse<PrescriptionDto>.Fail("Prescription not found."));

            return Ok(ApiResponse<PrescriptionDto>.Ok(prescription));
        }


        /// <summary>
        /// Add one medicine to a consultation - Doctor only
        /// </summary>

        // POST api/prescriptions/consultation/5
        // Doctor adds ONE medicine to a consultation
        [HttpPost("consultation/{consultationId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddOne(
            int consultationId, [FromBody] CreatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _prescriptionService.AddPrescriptionAsync(consultationId, dto);
            return Ok(ApiResponse<PrescriptionDto>.Ok(result, "Prescription added."));
        }


        /// <summary>
        /// Add multiple medicines at once - Doctor only
        /// </summary>
        // POST api/prescriptions/consultation/5/bulk
        // Doctor adds ALL medicines at once (most common use case)
        // Body example:
        // [
        //   { "medicineName": "Paracetamol 500mg", "dosage": "1-1-1", "timing": "AF", "duration": "5 days" },
        //   { "medicineName": "Cough Syrup 10ml",  "dosage": "0-0-1", "timing": "BF", "duration": "3 days" }
        // ]
        [HttpPost("consultation/{consultationId}/bulk")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddBulk(
            int consultationId, [FromBody] List<CreatePrescriptionDto> prescriptions)
        {
            _logger.LogInformation(
                "Doctor adding {Count} medicines for Consultation {ConsultationId}",
                prescriptions.Count, consultationId);

            var result = await _prescriptionService.AddManyPrescriptionsAsync(
                consultationId, prescriptions);

            return Ok(ApiResponse<List<PrescriptionDto>>.Ok(
                result, $"{result.Count} medicine(s) added to prescription."));
        }

        /// <summary>
        /// Update a prescription - Doctor only
        /// </summary>

        // PUT api/prescriptions/7
        // Doctor corrects a medicine (change dose / duration)
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePrescriptionDto dto)
        {
            var result = await _prescriptionService.UpdatePrescriptionAsync(id, dto);
            return Ok(ApiResponse<PrescriptionDto>.Ok(result, "Prescription updated."));
        }

        /// <summary>
        /// Delete a prescription - Doctor and Admin only
        /// </summary>

        // DELETE api/prescriptions/7
        // Doctor removes a medicine from the prescription
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _prescriptionService.DeletePrescriptionAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail("Prescription not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Prescription removed."));
        }
    }
}


