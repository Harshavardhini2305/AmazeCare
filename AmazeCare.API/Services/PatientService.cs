using AmazeCare.API.Data;
using AmazeCare.API.DTOS;
using AmazeCare.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AmazeCare.API.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        // Get all patients (Admin use)
        public async Task<PagedResponse<PatientDto>> GetAllPatientsAsync(
            QueryParameters query)
        {
            // Step 1: Start with all active patients
            var patientsQuery = _context.Patients
                .Where(p => p.IsActive);

            // ── SORTING ───────────────────────────────────
            // Step 2: Apply sorting BEFORE pagination
            patientsQuery = query.SortBy?.ToLower() switch
            {

                "id" => query.SortDirection == "desc"
                    ? patientsQuery.OrderByDescending(p => p.PatientId)
                    : patientsQuery.OrderBy(p => p.PatientId),
                // Sort by name A→Z or Z→A
                "name" => query.SortDirection == "desc"
                    ? patientsQuery.OrderByDescending(p => p.FullName.ToLower())
                    : patientsQuery.OrderBy(p => p.FullName.ToLower()),

                // Sort by registration date
                "date" => query.SortDirection == "desc"
                    ? patientsQuery.OrderByDescending(p => p.CreatedAt)
                    : patientsQuery.OrderBy(p => p.CreatedAt),

                // Sort by gender
                "gender" => query.SortDirection == "desc"
                    ? patientsQuery.OrderByDescending(p => p.Gender)
                    : patientsQuery.OrderBy(p => p.Gender),

                // Default: sort by name A→Z
                _ => patientsQuery.OrderBy(p => p.FullName)
            };

            // ── PAGINATION ────────────────────────────────
            // Step 3: Count TOTAL records BEFORE paging
            int totalCount = await patientsQuery.CountAsync();

            // Step 4: Skip previous pages, Take current page only
            var patients = await patientsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new PatientDto
                {
                    PatientId = p.PatientId,
                    FullName = p.FullName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    MobileNumber = p.MobileNumber,
                    Email = p.Email
                })
                .ToListAsync();

            // Step 5: Return paged response
            return new PagedResponse<PatientDto>
            {
                Data = patients,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }


        // Get single patient by ID
        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null || !patient.IsActive)
                return null;

            return new PatientDto
            {
                PatientId = patient.PatientId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                MobileNumber = patient.MobileNumber,
                Email = patient.Email
            };
        }

        // Update patient profile
        public async Task<PatientDto> UpdatePatientAsync(int id, UpdatePatientDto dto)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                throw new Exception("Patient not found.");

            // Update only the fields user is allowed to change
            patient.FullName = dto.FullName;
            patient.MobileNumber = dto.MobileNumber;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Gender = dto.Gender;

            await _context.SaveChangesAsync();

            return new PatientDto
            {
                PatientId = patient.PatientId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                MobileNumber = patient.MobileNumber,
                Email = patient.Email
            };
        }

        // Soft delete: set IsActive = false instead of removing from DB
        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return false;

            patient.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
