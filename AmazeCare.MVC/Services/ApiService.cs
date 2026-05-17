// ─────────────────────────────────────────────────────────────
// AmazeCare.MVC/Services/ApiService.cs
//
// All HTTP calls to AmazeCare.API backend
// Uses HttpClient with JWT token in header
// ─────────────────────────────────────────────────────────────

using AmazeCare.MVC.Models;
using Newtonsoft.Json;
using System.Text;

namespace AmazeCare.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // ── SET JWT TOKEN IN HEADER ───────────────────────
        // Called before every protected API request
        private void SetAuthHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add(
                "Authorization", $"Bearer {token}");
        }

        // ── GENERIC GET ───────────────────────────────────
        private async Task<T?> GetAsync<T>(string url, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    SetAuthHeader(token);

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
                    return result != null ? result.Data : default;
                }

                _logger.LogWarning("GET {Url} failed: {Content}", url, content);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET {Url} error", url);
                return default;
            }
        }

        // ── GENERIC POST ──────────────────────────────────
        private async Task<(bool Success, T? Data, string Message)>
            PostAsync<T>(string url, object body, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    SetAuthHeader(token);

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);

                if (response.IsSuccessStatusCode && result != null)
                    return (true, result.Data, result.Message);

                var message = result?.Message ?? "Something went wrong.";
                return (false, default, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST {Url} error", url);
                return (false, default, ex.Message);
            }
        }

        // ── GENERIC PUT ───────────────────────────────────
        private async Task<(bool Success, string Message)>
            PutAsync(string url, object? body, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    SetAuthHeader(token);

                var json = body != null
                    ? JsonConvert.SerializeObject(body)
                    : "{}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseContent);

                if (response.IsSuccessStatusCode)
                    return (true, result?.Message ?? "Success");

                return (false, result?.Message ?? "Something went wrong.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PUT {Url} error", url);
                return (false, ex.Message);
            }
        }

        // ── GENERIC PATCH ─────────────────────────────────
        private async Task<(bool Success, string Message)>
            PatchAsync(string url, object body, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    SetAuthHeader(token);

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Patch, url)
                {
                    Content = content
                };
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseContent);

                if (response.IsSuccessStatusCode)
                    return (true, result?.Message ?? "Success");

                return (false, result?.Message ?? "Something went wrong.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PATCH {Url} error", url);
                return (false, ex.Message);
            }
        }

        // ── GENERIC DELETE ────────────────────────────────
        private async Task<(bool Success, string Message)>
            DeleteAsync(string url, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                    SetAuthHeader(token);

                var response = await _httpClient.DeleteAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ApiResponse<object>>(responseContent);

                if (response.IsSuccessStatusCode)
                    return (true, result?.Message ?? "Deleted");

                return (false, result?.Message ?? "Something went wrong.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DELETE {Url} error", url);
                return (false, ex.Message);
            }
        }


        // ════════════════════════════════════════════════════
        // AUTH APIs
        // ════════════════════════════════════════════════════

        public async Task<(bool Success, LoginResponseViewModel? Data, string Message)>
            RegisterPatientAsync(RegisterViewModel model)
        {
            var body = new
            {
                fullName = model.FullName,
                dateOfBirth = model.DateOfBirth,
                gender = model.Gender,
                mobileNumber = model.MobileNumber,
                email = model.Email,
                password = model.Password
            };
            var result = await PostAsync<LoginResponseViewModel>("auth/register", body);
            return result;
        }

        public async Task<(bool Success, LoginResponseViewModel? Data, string Message)>
            LoginAsync(string email, string password, string role)
        {
            var body = new { email, password };
            var endpoint = role.ToLower() switch
            {
                "patient" => "auth/login/patient",
                "doctor" => "auth/login/doctor",
                "admin" => "auth/login/admin",
                _ => "auth/login/patient"
            };
            return await PostAsync<LoginResponseViewModel>(endpoint, body);
        }


        // ════════════════════════════════════════════════════
        // DOCTOR APIs
        // ════════════════════════════════════════════════════

        public async Task<List<DoctorViewModel>> GetAllDoctorsAsync(
            string token, int page = 1, int size = 10,
            string sortBy = "name", string sortDir = "asc")
        {
            var result = await GetAsync<PagedResponse<DoctorViewModel>>(
                $"doctors?pageNumber={page}&pageSize={size}&sortBy={sortBy}&sortDirection={sortDir}",
                token);
            return result?.Data ?? new List<DoctorViewModel>();
        }

        public async Task<List<DoctorViewModel>> SearchDoctorsAsync(
            string? specialty, string token)
        {
            var url = string.IsNullOrEmpty(specialty)
                ? "doctors/search"
                : $"doctors/search?specialty={specialty}";
            return await GetAsync<List<DoctorViewModel>>(url, token)
                ?? new List<DoctorViewModel>();
        }

        public async Task<DoctorViewModel?> GetDoctorByIdAsync(int id, string token) =>
            await GetAsync<DoctorViewModel>($"doctors/{id}", token);

        public async Task<(bool Success, string Message)> CreateDoctorAsync(
            CreateDoctorViewModel model, string token)
        {
            var body = new
            {
                fullName = model.FullName,
                email = model.Email,
                password = model.Password,
                specialty = model.Specialty,
                experienceYears = model.ExperienceYears,
                qualification = model.Qualification,
                designation = model.Designation,
                mobileNumber = model.MobileNumber
            };
            var result = await PostAsync<DoctorViewModel>("doctors", body, token);
            return (result.Success, result.Message);
        }

        public async Task<(bool Success, string Message)> UpdateDoctorAsync(
            int id, UpdateDoctorViewModel model, string token)
        {
            var body = new
            {
                fullName = model.FullName,
                specialty = model.Specialty,
                experienceYears = model.ExperienceYears,
                qualification = model.Qualification,
                designation = model.Designation,
                mobileNumber = model.MobileNumber
            };
            return await PatchAsync($"doctors/{id}", body, token);
        }

        public async Task<(bool Success, string Message)> DeleteDoctorAsync(
            int id, string token) =>
            await DeleteAsync($"doctors/{id}", token);

        public async Task<List<string>> GetSpecialtiesAsync(string token) =>
            await GetAsync<List<string>>("doctors/specialties", token)
            ?? new List<string>();


        // ════════════════════════════════════════════════════
        // PATIENT APIs
        // ════════════════════════════════════════════════════

        public async Task<List<PatientViewModel>> GetAllPatientsAsync(
            string token, int page = 1, int size = 10)
        {
            var result = await GetAsync<PagedResponse<PatientViewModel>>(
                $"patients?pageNumber={page}&pageSize={size}", token);
            return result?.Data ?? new List<PatientViewModel>();
        }

        public async Task<PatientViewModel?> GetPatientByIdAsync(
            int id, string token) =>
            await GetAsync<PatientViewModel>($"patients/{id}", token);

        public async Task<PatientViewModel?> GetMyProfileAsync(string token) =>
            await GetAsync<PatientViewModel>("patients/me", token);

        public async Task<(bool Success, string Message)> UpdatePatientAsync(
            int id, UpdatePatientViewModel model, string token)
        {
            var body = new
            {
                fullName = model.FullName,
                mobileNumber = model.MobileNumber,
                dateOfBirth = model.DateOfBirth,
                gender = model.Gender
            };
            return await PatchAsync($"patients/{id}", body, token);
        }


        // ════════════════════════════════════════════════════
        // APPOINTMENT APIs
        // ════════════════════════════════════════════════════

        public async Task<(bool Success, string Message)> BookAppointmentAsync(
            CreateAppointmentViewModel model, string token)
        {
            var body = new
            {
                doctorId = model.DoctorId,
                appointmentDate = model.AppointmentDate,
                timeSlot = model.TimeSlot,
                symptoms = model.Symptoms,
                natureOfVisit = model.NatureOfVisit
            };
            var result = await PostAsync<AppointmentViewModel>(
                "appointments", body, token);
            return (result.Success, result.Message);
        }

        public async Task<List<AppointmentViewModel>> GetMyAppointmentsAsync(
            string token) =>
            await GetAsync<List<AppointmentViewModel>>(
                "appointments/my", token)
            ?? new List<AppointmentViewModel>();

        public async Task<List<AppointmentViewModel>> GetMyUpcomingAsync(
            string token) =>
            await GetAsync<List<AppointmentViewModel>>(
                "appointments/my/upcoming", token)
            ?? new List<AppointmentViewModel>();

        public async Task<List<AppointmentViewModel>> GetMyCompletedAsync(
            string token) =>
            await GetAsync<List<AppointmentViewModel>>(
                "appointments/my/completed", token)
            ?? new List<AppointmentViewModel>();

        public async Task<List<AppointmentViewModel>> GetDoctorAppointmentsAsync(
            int doctorId, string token) =>
            await GetAsync<List<AppointmentViewModel>>(
                $"appointments/doctor/{doctorId}", token)
            ?? new List<AppointmentViewModel>();

        public async Task<List<AppointmentViewModel>> GetDoctorUpcomingAsync(
            int doctorId, string token) =>
            await GetAsync<List<AppointmentViewModel>>(
                $"appointments/doctor/{doctorId}/upcoming", token)
            ?? new List<AppointmentViewModel>();

        public async Task<List<AppointmentViewModel>> GetDoctorCompletedAsync(
            int doctorId, string token) =>
            await GetAsync<List<AppointmentViewModel>>(
                $"appointments/doctor/{doctorId}/completed", token)
            ?? new List<AppointmentViewModel>();

        public async Task<List<AppointmentViewModel>> GetAllAppointmentsAsync(
            string token, int page = 1, int size = 10)
        {
            var result = await GetAsync<PagedResponse<AppointmentViewModel>>(
                $"appointments?pageNumber={page}&pageSize={size}", token);
            return result?.Data ?? new List<AppointmentViewModel>();
        }

        public async Task<AppointmentViewModel?> GetAppointmentByIdAsync(
            int id, string token) =>
            await GetAsync<AppointmentViewModel>($"appointments/{id}", token);

        public async Task<(bool Success, string Message)> ConfirmAppointmentAsync(
            int id, string token) =>
            await PutAsync($"appointments/{id}/confirm", null, token);

        public async Task<(bool Success, string Message)> CancelAppointmentAsync(
            int id, string reason, string token)
        {
            var body = new { reason };
            return await PutAsync($"appointments/{id}/cancel", body, token);
        }

        public async Task<(bool Success, string Message)> RescheduleAppointmentAsync(
            int id, RescheduleViewModel model, string token)
        {
            var body = new
            {
                newDate = model.NewDate,
                newTimeSlot = model.NewTimeSlot
            };
            return await PutAsync($"appointments/{id}/reschedule", body, token);
        }


        // ════════════════════════════════════════════════════
        // CONSULTATION APIs
        // ════════════════════════════════════════════════════

        public async Task<(bool Success, ConsultationViewModel? Data, string Message)>
            AddConsultationAsync(int appointmentId,
                CreateConsultationViewModel model, string token)
        {
            var body = new
            {
                currentSymptoms = model.CurrentSymptoms,
                physicalExamination = model.PhysicalExamination,
                diagnosis = model.Diagnosis,
                treatmentPlan = model.TreatmentPlan,
                recommendedTests = model.RecommendedTests
            };
            return await PostAsync<ConsultationViewModel>(
                $"appointments/{appointmentId}/consultation", body, token);
        }

        public async Task<ConsultationViewModel?> GetConsultationAsync(
            int appointmentId, string token) =>
            await GetAsync<ConsultationViewModel>(
                $"appointments/{appointmentId}/consultation", token);


        // ════════════════════════════════════════════════════
        // PRESCRIPTION APIs
        // ════════════════════════════════════════════════════

        public async Task<(bool Success, string Message)> AddPrescriptionsAsync(
            int consultationId,
            List<PrescriptionItemViewModel> prescriptions,
            string token)
        {
            var body = prescriptions.Select(p => new
            {
                medicineName = p.MedicineName,
                dosage = p.Dosage,
                timing = p.Timing,
                duration = p.Duration,
                notes = p.Notes
            }).ToList();

            var result = await PostAsync<List<PrescriptionViewModel>>(
                $"prescriptions/consultation/{consultationId}/bulk",
                body, token);
            return (result.Success, result.Message);
        }

        public async Task<List<PrescriptionViewModel>> GetPrescriptionsAsync(
            int consultationId, string token) =>
            await GetAsync<List<PrescriptionViewModel>>(
                $"prescriptions/consultation/{consultationId}", token)
            ?? new List<PrescriptionViewModel>();

        public async Task<(bool Success, string Message)> DeletePrescriptionAsync(
            int prescriptionId, string token) =>
            await DeleteAsync($"prescriptions/{prescriptionId}", token);


        // ════════════════════════════════════════════════════
        // MEDICAL RECORD APIs
        // ════════════════════════════════════════════════════

        public async Task<List<MedicalRecordViewModel>> GetMedicalRecordsAsync(
            int patientId, string token) =>
            await GetAsync<List<MedicalRecordViewModel>>(
                $"medicalrecords/patient/{patientId}", token)
            ?? new List<MedicalRecordViewModel>();

        public async Task<(bool Success, string Message)> AddMedicalRecordAsync(
            CreateMedicalRecordViewModel model, string token)
        {
            var body = new
            {
                patientId = model.PatientId,
                recordedBy = model.RecordedBy,
                recordType = model.RecordType,
                title = model.Title,
                description = model.Description,
                attachmentFileName = model.AttachmentFileName,
                recordDate = model.RecordDate
            };
            var result = await PostAsync<MedicalRecordViewModel>(
                "medicalrecords", body, token);
            return (result.Success, result.Message);
        }

        public async Task<(bool Success, string Message)> DeleteMedicalRecordAsync(
            int id, string token) =>
            await DeleteAsync($"medicalrecords/{id}", token);
    }
}
