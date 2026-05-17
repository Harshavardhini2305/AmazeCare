using AmazeCare.API.Data;
using AmazeCare.API.DTOS;
using AmazeCare.API.Models;
using AmazeCare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AmazeCare.API.Services
{

    // AuthService handles:
    // 1. Patient Registration (save to DB with hashed password)
    // 2. Login for Patient, Doctor, Admin (verify password, generate JWT token)
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        // Dependency Injection: AppDbContext and IConfiguration are injected automatically
        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ── REGISTER PATIENT ──────────────────────────────────────────
        public async Task<LoginResponseDto> RegisterPatientAsync(RegisterPatientDto dto)
        {
            // Check if email already exists
            bool emailExists = await _context.Patients.AnyAsync(p => p.Email == dto.Email);
            if (emailExists)
                throw new Exception("Email is already registered.");

            // Create new Patient object
            var patient = new Patient
            {
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                MobileNumber = dto.MobileNumber,
                Email = dto.Email,
                // BCrypt: hash the password before saving to database
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Patient",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Generate JWT token and return
            return GenerateToken(patient.PatientId, patient.FullName, "Patient");
        }
        // ── LOGIN PATIENT ──────────────────────────────────────────────
        public async Task<LoginResponseDto> LoginPatientAsync(LoginDto dto)
        {
            // Find patient by email
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (patient == null)
                throw new Exception("Invalid email or password.");

            // BCrypt.Verify: compare entered password with stored hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, patient.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Invalid email or password.");

            if (!patient.IsActive)
                throw new Exception("Your account is deactivated.");

            return GenerateToken(patient.PatientId, patient.FullName, "Patient");
        }

        // ── LOGIN DOCTOR ───────────────────────────────────────────────
        public async Task<LoginResponseDto> LoginDoctorAsync(LoginDto dto)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Email == dto.Email);

            if (doctor == null)
                throw new Exception("Invalid email or password.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, doctor.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Invalid email or password.");

            return GenerateToken(doctor.DoctorId, doctor.FullName, "Doctor");
        }
        // ── LOGIN ADMIN ────────────────────────────────────────────────
        public async Task<LoginResponseDto> LoginAdminAsync(LoginDto dto)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == dto.Email);

            if (admin == null)
                throw new Exception("Invalid email or password.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, admin.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Invalid email or password.");

            return GenerateToken(admin.AdminId, admin.FullName, "Admin");
        }

        // ── GENERATE JWT TOKEN ─────────────────────────────────────────
        // This private method creates the JWT token.
        // The token contains: UserId, FullName, Role
        // It is signed using a secret key stored in appsettings.json
        private LoginResponseDto GenerateToken(int userId, string fullName, string role)
        {
            // Read JWT settings from appsettings.json
            var secretKey = _config["JwtSettings:SecretKey"]!;
            var issuer = _config["JwtSettings:Issuer"]!;
            var audience = _config["JwtSettings:Audience"]!;
            int expiryHours = int.Parse(_config["JwtSettings:ExpiryHours"] ?? "8");

            // Create signing key from secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims = information stored inside the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

            // Create the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(expiryHours),
                signingCredentials: credentials
            );

            // Convert token to string
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString,
                Role = role,
                UserId = userId,
                FullName = fullName
            };
        }

    }
}
