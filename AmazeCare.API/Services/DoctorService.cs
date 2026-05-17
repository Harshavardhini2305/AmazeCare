using AmazeCare.API.Data;
using AmazeCare.API.DTOS;
using AmazeCare.API.Models;
using AmazeCare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;
        
        public DoctorService(AppDbContext context)
        {
            _context = context;
        }

        // Get all active doctors
        public async Task<PagedResponse<DoctorDto>> GetAllDoctorsAsync(
             QueryParameters query)
        {
            //  Start with all active doctors
            var doctorsQuery = _context.Doctors
                .Where(d => d.IsActive);

           
            // Step 2: Apply sorting BEFORE pagination
            // sortBy tells which field, sortDirection tells order
            doctorsQuery = query.SortBy?.ToLower() switch
            {

                // sort by ID
                "id" => query.SortDirection == "desc"
                    ? doctorsQuery.OrderByDescending(d => d.DoctorId)
                    : doctorsQuery.OrderBy(d => d.DoctorId),

                // Sort by name A→Z or Z→A
                "name" => query.SortDirection == "desc"
                    ? doctorsQuery.OrderByDescending(d => d.FullName.ToLower())
                    : doctorsQuery.OrderBy(d => d.FullName.ToLower()),

                // Sort by experience years
                "experience" => query.SortDirection == "desc"
                    ? doctorsQuery.OrderByDescending(d => d.ExperienceYears)
                    : doctorsQuery.OrderBy(d => d.ExperienceYears),

                // Sort by specialty A→Z or Z→A
                "specialty" => query.SortDirection == "desc"
                    ? doctorsQuery.OrderByDescending(d => d.Specialty.ToLower())
                    : doctorsQuery.OrderBy(d => d.Specialty.ToLower()),

                // Default: sort by name A→Z
                _ => doctorsQuery.OrderBy(d => d.FullName)
            };

            // ── PAGINATION ────────────────────────────────
            // Step 3: Count TOTAL records BEFORE paging
            int totalCount = await doctorsQuery.CountAsync();

            // Step 4: Skip previous pages, Take current page only
            // Example: Page 2, Size 5 → Skip 5, Take 5
            var doctors = await doctorsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(d => new DoctorDto
                {
                    DoctorId = d.DoctorId,
                    FullName = d.FullName,
                    Email = d.Email,
                    Specialty = d.Specialty,
                    ExperienceYears = d.ExperienceYears,
                    Qualification = d.Qualification,
                    Designation = d.Designation,
                    MobileNumber = d.MobileNumber
                })
                .ToListAsync();

            // Step 5: Return paged response with all info
            return new PagedResponse<DoctorDto>
            {
                Data = doctors,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
                // TotalPages, HasPreviousPage, HasNextPage
                // are calculated automatically!
            };
        }



        // Search doctors by specialty or name (for patient appointment booking)
        public async Task<List<DoctorDto>> SearchDoctorsAsync(string? specialty)
        {
            // Start with all active doctors
            var query = _context.Doctors.Where(d => d.IsActive);

            // Filter by specialty if provided
            if (!string.IsNullOrEmpty(specialty))
                query = query.Where(d => d.Specialty.Contains(specialty));

           

            var result = await query
                .Select(d => new DoctorDto
                {
                    DoctorId = d.DoctorId,
                    FullName = d.FullName,
                    Email = d.Email,
                    Specialty = d.Specialty,
                    ExperienceYears = d.ExperienceYears,
                    Qualification = d.Qualification,
                    Designation = d.Designation,
                    MobileNumber = d.MobileNumber
                })
                .ToListAsync();
            //  Return meaningful message if no doctors found
            if (result.Count == 0)
                throw new Exception($"No doctors found for specialty: {specialty}");

            return result;

        }
        // Get a single doctor
        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var d = await _context.Doctors.FindAsync(id);
            if (d == null || !d.IsActive)
                return null;

            return new DoctorDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Email = d.Email,
                Specialty = d.Specialty,
                ExperienceYears = d.ExperienceYears,
                Qualification = d.Qualification,
                Designation = d.Designation,
                MobileNumber = d.MobileNumber
            };
        }

        // Admin: Add new doctor
        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto dto)
        {
            // Check duplicate email
            bool exists = await _context.Doctors.AnyAsync(d => d.Email == dto.Email);
            if (exists)
                throw new Exception("A doctor with this email already exists.");

            var doctor = new Doctor
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Specialty = dto.Specialty,
                ExperienceYears = dto.ExperienceYears,
                Qualification = dto.Qualification,
                Designation = dto.Designation,
                MobileNumber = dto.MobileNumber,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.FullName,
                Email = doctor.Email,
                Specialty = doctor.Specialty,
                ExperienceYears = doctor.ExperienceYears,
                Qualification = doctor.Qualification,
                Designation = doctor.Designation,
                MobileNumber = doctor.MobileNumber
            };
        }


        // Admin: Update doctor
        public async Task<DoctorDto> UpdateDoctorAsync(int id, UpdateDoctorDto dto)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            doctor.FullName = dto.FullName;
            doctor.Specialty = dto.Specialty;
            doctor.ExperienceYears = dto.ExperienceYears;
            doctor.Qualification = dto.Qualification;
            doctor.Designation = dto.Designation;
            doctor.MobileNumber = dto.MobileNumber;

            await _context.SaveChangesAsync();

            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.FullName,
                Email = doctor.Email,
                Specialty = doctor.Specialty,
                ExperienceYears = doctor.ExperienceYears,
                Qualification = doctor.Qualification,
                Designation = doctor.Designation,
                MobileNumber = doctor.MobileNumber
            };
        }



        // Admin: Soft delete doctor
        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return false;

            doctor.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
