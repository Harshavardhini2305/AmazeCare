using Microsoft.AspNetCore.Mvc;
using AmazeCare.API.DTOS;
using AmazeCare.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace AmazeCare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active doctors
        /// </summary>

        // GET api/doctors  →  list all doctors (anyone can call this)
        [HttpGet]
        [AllowAnonymous]
        
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        {
            var result = await _doctorService.GetAllDoctorsAsync(query);
            return Ok(ApiResponse<PagedResponse<DoctorDto>>.Ok(result));
        }

        /// <summary>
        /// Search doctors by specialty or name
        /// </summary>

        // GET api/doctors/search?specialty=Cardiology&name=Kumar
        [HttpGet("search")]
       
        public async Task<IActionResult> Search([FromQuery] string? specialty)
        {
            var doctors = await _doctorService.SearchDoctorsAsync(specialty);
            return Ok(ApiResponse<List<DoctorDto>>.Ok(doctors));
        }


        /// <summary>
        /// Get doctor details by ID
        /// </summary>
        // GET api/doctors/5
        [HttpGet("{id}")]
       
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound(ApiResponse<DoctorDto>.Fail("Doctor not found."));  // 404

            return Ok(ApiResponse<DoctorDto>.Ok(doctor));
        }



        /// <summary>
        /// Add a new doctor - Admin only
        /// </summary>

        // POST api/doctors  →  Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Admin adding new doctor: {Email}", dto.Email);
            var doctor = await _doctorService.CreateDoctorAsync(dto);
            return Ok(ApiResponse<DoctorDto>.Ok(doctor, "Doctor added successfully."));
        }


        /// <summary>
        /// Update doctor details - Admin only
        /// </summary>

        // PUT api/doctors/5  →  Admin only
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDto dto)
        {
            var doctor = await _doctorService.UpdateDoctorAsync(id, dto);
            return Ok(ApiResponse<DoctorDto>.Ok(doctor, "Doctor updated successfully."));
        }



        /// <summary>
        /// Delete a doctor - Admin only
        /// </summary>
        // DELETE api/doctors/5  →  Admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail("Doctor not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Doctor deleted."));
        }


        /// <summary>
        /// Get list of all available specialties
        /// </summary>
        [HttpGet("specialties")]
        [AllowAnonymous]
        public IActionResult GetSpecialties()
        {
            var specialties = new List<string>
    {
        "Cardiology",
        "Dermatology",
        "Endocrinology",
        "Gastroenterology",
        "General Medicine",
        "Neurology",
        "Obstetrics and Gynecology",
        "Oncology",
        "Ophthalmology",
        "Orthopedics",
        "Pediatrics",
        "Psychiatry",
        "Pulmonology",
        "Radiology",
        "Urology",
        "Dentist"
    };

            return Ok(ApiResponse<List<string>>.Ok(specialties));
        }


    }
}
