
//
// BASE CLASS for all tests
//  Creates a fresh InMemory Database for each test
//  Seeds common test data (patients, doctors, admin)
//  Provides JWT configuration for AuthService
//  All test classes inherit from this
// ─────────────────────────────────────────────────────────────

using AmazeCare.API.Data;
using AmazeCare.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AmazeCare.Tests
{
    /// <summary>
    /// Base class providing in-memory database and JWT config
    /// for all test classes
    /// </summary>
    public class TestBase
    {
        // ── Create Fresh InMemory Database ─────────────────────
        // Each test gets its OWN database so tests don't interfere
        protected AppDbContext CreateDbContext(string dbName = "")
        {
            // Give each test a unique DB name
            if (string.IsNullOrEmpty(dbName))
                dbName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        // ── Create JWT Configuration ───────────────────────────
        // AuthService needs these settings to generate JWT tokens
        protected IConfiguration CreateJwtConfig()
        {
            var settings = new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey",   "AmazeCare@SuperSecretKey2024!MinLength32Chars" },
                { "JwtSettings:Issuer",      "AmazeCare.API" },
                { "JwtSettings:Audience",    "AmazeCare.Clients" },
                { "JwtSettings:ExpiryHours", "8" },
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        // ── Seed Test Patient ──────────────────────────────────
        protected Patient CreateTestPatient(
            int id = 1,
            string name = "Test Patient",
            string email = "patient@test.com",
            string password = "Test@1234")
        {
            return new Patient
            {
                PatientId = id,
                FullName = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                MobileNumber = "9876543210",
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 1),
                Role = "Patient",
                IsActive = true,
                CreatedAt = DateTime.Now,
            };
        }

        // ── Seed Test Doctor ───────────────────────────────────
        protected Doctor CreateTestDoctor(
            int id = 1,
            string name = "Dr. Test Doctor",
            string email = "doctor@test.com",
            string specialty = "Cardiology",
            string password = "Doctor@123")
        {
            return new Doctor
            {
                DoctorId = id,
                FullName = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Specialty = specialty,
                ExperienceYears = 10,
                Qualification = "MD Cardiology",
                Designation = "Senior Consultant",
                MobileNumber = "9876543211",
                Role = "Doctor",
                IsActive = true,
                CreatedAt = DateTime.Now,
            };
        }

        // ── Seed Test Admin ────────────────────────────────────
        protected Admin CreateTestAdmin(
            int id = 1,
            string email = "admin@amazecare.com",
            string password = "Admin@123")
        {
            return new Admin
            {
                AdminId = id,
                FullName = "Super Admin",
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin",
            };
        }

        // ── Seed Test Appointment ──────────────────────────────
        protected Appointment CreateTestAppointment(
            int id = 1,
            int patientId = 1,
            int doctorId = 1,
            string status = "Pending")
        {
            return new Appointment
            {
                AppointmentId = id,
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = DateTime.Now.AddDays(1),
                TimeSlot = "10:00 AM",
                Symptoms = "Chest pain and headache",
                NatureOfVisit = "General Checkup",
                Status = status,
                CreatedAt = DateTime.Now,
            };
        }

        // ── Seed Test Consultation ─────────────────────────────
        protected Consultation CreateTestConsultation(
            int id = 1,
            int appointmentId = 1)
        {
            return new Consultation
            {
                ConsultationId = id,
                AppointmentId = appointmentId,
                CurrentSymptoms = "Chest pain and shortness of breath",
                PhysicalExamination = "BP: 140/90, Pulse: 88 bpm",
                Diagnosis = "Hypertension Stage 1",
                TreatmentPlan = "Medication and lifestyle changes",
                RecommendedTests = "ECG, Blood Test",
                ConsultedAt = DateTime.Now,
            };
        }

        // ── Seed Test Prescription ─────────────────────────────
        protected Prescription CreateTestPrescription(
            int id = 1,
            int consultationId = 1)
        {
            return new Prescription
            {
                PrescriptionId = id,
                ConsultationId = consultationId,
                MedicineName = "Amlodipine 5mg",
                Dosage = "0-0-1",
                Timing = "AF",
                Duration = "30 days",
                Notes = "Take at night",
            };
        }

        // ── Seed Test Medical Record ───────────────────────────
        protected MedicalRecord CreateTestMedicalRecord(
            int id = 1,
            int patientId = 1)
        {
            return new MedicalRecord
            {
                RecordId = id,
                PatientId = patientId,
                RecordedBy = "Dr. Test Doctor",
                RecordType = "Lab Report",
                Title = "Blood Test CBC Report",
                Description = "Hemoglobin normal, WBC normal",
                AttachmentFileName = "blood_test.pdf",
                RecordDate = DateTime.Now,
                CreatedAt = DateTime.Now,
            };
        }
    }
}
