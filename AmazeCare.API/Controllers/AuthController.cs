using Microsoft.AspNetCore.Mvc;
using AmazeCare.API.DTOS;
using AmazeCare.API.Services.Interfaces;



// AuthController.cs
// Handles: Register, Login (Patient / Doctor / Admin)
namespace AmazeCare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        // Constructor Injection (Dependency Injection)
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        /// <summary>
        /// Register a new patient account
        /// </summary>
        
        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPatientDto dto)
        {
            // Check model validation (data annotations on DTO)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Register attempt for email: {Email}", dto.Email);

            var result = await _authService.RegisterPatientAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Registration successful."));
        }


        /// <summary>
        /// Login as a patient
        /// </summary>
        // POST api/auth/login/patient
        [HttpPost("login/patient")]
        public async Task<IActionResult> LoginPatient([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginPatientAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Login successful."));
        }


        /// <summary>
        /// Login as a doctor
        /// </summary>

        // POST api/auth/login/doctor
        [HttpPost("login/doctor")]
        public async Task<IActionResult> LoginDoctor([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginDoctorAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Login successful."));
        }


        /// <summary>
        /// Login as an administrator
        /// </summary>
        // POST api/auth/login/admin
        [HttpPost("login/admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAdminAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Login successful."));
        }
    }
}
