
using AmazeCare.API.DTOS;
using AmazeCare.API.Services;
using FluentAssertions;
using NUnit.Framework;

namespace AmazeCare.Tests
{
  
    // CONSULTATION SERVICE TESTS  (Tests 31–35)
    
    [TestFixture]
    public class ConsultationServiceTests : TestBase
    {
        // ── TEST 31 ────────────────────────────────────────────
        [Test]
        [Description("AddConsultation saves consultation and marks appointment as Completed")]
        public async Task AddConsultation_SavesAndMarksAppointmentCompleted()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new ConsultationService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            db.Appointments.Add(CreateTestAppointment(
                id: 1, patientId: 1, doctorId: 1, status: "Confirmed"));
            await db.SaveChangesAsync();

            var dto = new CreateConsultationDto
            {
                CurrentSymptoms = "Chest pain and shortness of breath",
                PhysicalExamination = "BP: 140/90, Pulse: 88 bpm",
                Diagnosis = "Hypertension Stage 1",
                TreatmentPlan = "Medication and lifestyle changes",
                RecommendedTests = "ECG, Blood Test",
            };

            // ACT
            var result = await service.AddConsultationAsync(1, dto);

            // ASSERT
            result.Should().NotBeNull();
            result.AppointmentId.Should().Be(1);
            result.Diagnosis.Should().Be("Hypertension Stage 1");

            // Appointment status should now be Completed
            var appointment = db.Appointments.Find(1);
            appointment!.Status.Should().Be("Completed");
        }

        // ── TEST 32 ────────────────────────────────────────────
        [Test]
        [Description("AddConsultation throws exception when appointment does not exist")]
        public async Task AddConsultation_WithInvalidAppointmentId_ThrowsException()
        {
            // ARRANGE — empty database
            var db = CreateDbContext();
            var service = new ConsultationService(db);

            var dto = new CreateConsultationDto
            {
                CurrentSymptoms = "Headache",
                PhysicalExamination = "Normal",
                Diagnosis = "Migraine",
                TreatmentPlan = "Rest and medication",
                RecommendedTests = "",
            };

            // ACT + ASSERT
            var act = async () => await service.AddConsultationAsync(999, dto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Appointment*");
        }

        // ── TEST 33 ────────────────────────────────────────────
        [Test]
        [Description("AddConsultation throws when consultation already exists for appointment")]
        public async Task AddConsultation_WhenAlreadyExists_ThrowsException()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new ConsultationService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            db.Appointments.Add(CreateTestAppointment(
                id: 1, patientId: 1, doctorId: 1, status: "Confirmed"));
            // Add consultation already
            db.Consultations.Add(CreateTestConsultation(id: 1, appointmentId: 1));
            await db.SaveChangesAsync();

            var dto = new CreateConsultationDto
            {
                CurrentSymptoms = "New symptoms",
                PhysicalExamination = "New exam",
                Diagnosis = "New diagnosis",
                TreatmentPlan = "New plan",
            };

            // ACT + ASSERT — should throw because consultation already exists
            var act = async () => await service.AddConsultationAsync(1, dto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*already*");
        }

        // ── TEST 34 ────────────────────────────────────────────
        [Test]
        [Description("GetByAppointmentId returns consultation for that appointment")]
        public async Task GetByAppointmentId_ReturnsCorrectConsultation()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new ConsultationService(db);

            db.Appointments.Add(CreateTestAppointment(id: 1));
            db.Consultations.Add(CreateTestConsultation(
                id: 1, appointmentId: 1));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.GetByAppointmentIdAsync(1);

            // ASSERT
            result.Should().NotBeNull();
            result!.AppointmentId.Should().Be(1);
            result.Diagnosis.Should().Be("Hypertension Stage 1");
        }

        // ── TEST 35 ────────────────────────────────────────────
        [Test]
        [Description("GetByAppointmentId returns null when no consultation exists")]
        public async Task GetByAppointmentId_WhenNoConsultation_ReturnsNull()
        {
            // ARRANGE — no consultation in DB
            var db = CreateDbContext();
            var service = new ConsultationService(db);

            // ACT
            var result = await service.GetByAppointmentIdAsync(999);

            // ASSERT
            result.Should().BeNull();
        }
    }


    // PRESCRIPTION SERVICE TESTS  (Tests 36–40)
   

    [TestFixture]
    public class PrescriptionServiceTests : TestBase
    {
        // ── TEST 36 ────────────────────────────────────────────
        [Test]
        [Description("GetByConsultationId returns all prescriptions for that consultation")]
        public async Task GetByConsultationId_ReturnsAllPrescriptions()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PrescriptionService(db);

            // Add consultation and 3 prescriptions
            db.Consultations.Add(CreateTestConsultation(id: 1, appointmentId: 1));
            db.Prescriptions.AddRange(
                CreateTestPrescription(id: 1, consultationId: 1),
                CreateTestPrescription(id: 2, consultationId: 1),
                CreateTestPrescription(id: 3, consultationId: 1)
            );
            await db.SaveChangesAsync();

            // ACT
            var result = await service.GetByConsultationIdAsync(1);

            // ASSERT
            result.Should().HaveCount(3);
            result.Should().OnlyContain(p => p.ConsultationId == 1);
        }

        // ── TEST 37 ────────────────────────────────────────────
        [Test]
        [Description("AddPrescription adds one medicine to a consultation")]
        public async Task AddPrescription_AddsOneMedicineToConsultation()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PrescriptionService(db);

            db.Consultations.Add(CreateTestConsultation(id: 1, appointmentId: 1));
            await db.SaveChangesAsync();

            var dto = new CreatePrescriptionDto
            {
                MedicineName = "Paracetamol 500mg",
                Dosage = "1-1-1",
                Timing = "AF",
                Duration = "5 days",
                Notes = "Take with warm water",
            };

            // ACT
            var result = await service.AddPrescriptionAsync(1, dto);

            // ASSERT
            result.Should().NotBeNull();
            result.MedicineName.Should().Be("Paracetamol 500mg");
            result.Dosage.Should().Be("1-1-1");
            result.Timing.Should().Be("AF");
            result.ConsultationId.Should().Be(1);
        }

        // ── TEST 38 ────────────────────────────────────────────
        [Test]
        [Description("AddManyPrescriptions (bulk) adds all medicines at once")]
        public async Task AddManyPrescriptions_AddsAllMedicinesAtOnce()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PrescriptionService(db);

            db.Consultations.Add(CreateTestConsultation(id: 1, appointmentId: 1));
            await db.SaveChangesAsync();

            var prescriptions = new List<CreatePrescriptionDto>
            {
                new() { MedicineName = "Amlodipine 5mg",  Dosage = "0-0-1", Timing = "AF", Duration = "30 days" },
                new() { MedicineName = "Aspirin 75mg",    Dosage = "1-0-0", Timing = "AF", Duration = "30 days" },
                new() { MedicineName = "Atorvastatin 10mg", Dosage = "0-0-1", Timing = "AF", Duration = "30 days" },
            };

            // ACT
            var result = await service.AddManyPrescriptionsAsync(1, prescriptions);

            // ASSERT
            result.Should().HaveCount(3);
            result.Should().Contain(p => p.MedicineName == "Amlodipine 5mg");
            result.Should().Contain(p => p.MedicineName == "Aspirin 75mg");
            result.Should().Contain(p => p.MedicineName == "Atorvastatin 10mg");
        }

        // ── TEST 39 ────────────────────────────────────────────
        [Test]
        [Description("UpdatePrescription changes medicine details correctly")]
        public async Task UpdatePrescription_UpdatesMedicineDetails()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PrescriptionService(db);

            db.Consultations.Add(CreateTestConsultation(id: 1, appointmentId: 1));
            db.Prescriptions.Add(CreateTestPrescription(id: 1, consultationId: 1));
            await db.SaveChangesAsync();

            var updateDto = new CreatePrescriptionDto
            {
                MedicineName = "Updated Medicine",
                Dosage = "1-0-0",
                Timing = "BF",           // changed from AF to BF
                Duration = "10 days",
                Notes = "Updated notes",
            };

            // ACT
            var result = await service.UpdatePrescriptionAsync(1, updateDto);

            // ASSERT
            result.Should().NotBeNull();
            result.MedicineName.Should().Be("Updated Medicine");
            result.Timing.Should().Be("BF");
            result.Duration.Should().Be("10 days");
        }

        // ── TEST 40 ────────────────────────────────────────────
        [Test]
        [Description("DeletePrescription removes medicine from the database")]
        public async Task DeletePrescription_RemovesFromDatabase()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PrescriptionService(db);

            db.Consultations.Add(CreateTestConsultation(id: 1, appointmentId: 1));
            db.Prescriptions.Add(CreateTestPrescription(id: 1, consultationId: 1));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.DeletePrescriptionAsync(1);

            // ASSERT
            result.Should().BeTrue();

            // Prescription should be removed from DB completely
            var prescription = db.Prescriptions.Find(1);
            prescription.Should().BeNull();  // completely deleted (not soft delete)
        }
    }


    
    // MEDICAL RECORD SERVICE TESTS  (Tests 41–45)
    

    [TestFixture]
    public class MedicalRecordServiceTests : TestBase
    {
        // ── TEST 41 ────────────────────────────────────────────
        [Test]
        [Description("GetByPatientId returns all medical records for that patient")]
        public async Task GetByPatientId_ReturnsAllPatientRecords()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new MedicalRecordService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.MedicalRecords.AddRange(
                CreateTestMedicalRecord(id: 1, patientId: 1),
                CreateTestMedicalRecord(id: 2, patientId: 1),
                CreateTestMedicalRecord(id: 3, patientId: 2)  // different patient
            );
            await db.SaveChangesAsync();

            // ACT — get records for patient 1 only
            var result = await service.GetByPatientIdAsync(1);

            // ASSERT
            result.Should().HaveCount(2);         // only patient 1's records
            result.Should().OnlyContain(r => r.PatientId == 1);
        }

        // ── TEST 42 ────────────────────────────────────────────
        [Test]
        [Description("AddRecord saves new medical record to database")]
        public async Task AddRecord_WithValidData_SavesToDatabase()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new MedicalRecordService(db);

            db.Patients.Add(CreateTestPatient(id: 1, name: "Test Patient"));
            await db.SaveChangesAsync();

            var dto = new CreateMedicalRecordDto
            {
                PatientId = 1,
                RecordedBy = "Dr. Arun Kumar",
                RecordType = "Lab Report",
                Title = "Blood Test CBC",
                Description = "All values normal",
                AttachmentFileName = "blood_test.pdf",
                RecordDate = DateTime.Now,
            };

            // ACT
            var result = await service.AddRecordAsync(dto);

            // ASSERT
            result.Should().NotBeNull();
            result.PatientId.Should().Be(1);
            result.RecordType.Should().Be("Lab Report");
            result.Title.Should().Be("Blood Test CBC");
            result.RecordedBy.Should().Be("Dr. Arun Kumar");
        }

        // ── TEST 43 ────────────────────────────────────────────
        [Test]
        [Description("GetById returns the correct medical record")]
        public async Task GetById_WithValidId_ReturnsMedicalRecord()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new MedicalRecordService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.MedicalRecords.Add(CreateTestMedicalRecord(id: 5, patientId: 1));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.GetByIdAsync(5);

            // ASSERT
            result.Should().NotBeNull();
            result!.RecordId.Should().Be(5);
            result.RecordType.Should().Be("Lab Report");
        }

        // ── TEST 44 ────────────────────────────────────────────
        [Test]
        [Description("UpdateRecord changes the medical record details")]
        public async Task UpdateRecord_ChangesDetailsSuccessfully()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new MedicalRecordService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.MedicalRecords.Add(CreateTestMedicalRecord(id: 1, patientId: 1));
            await db.SaveChangesAsync();

            var updateDto = new CreateMedicalRecordDto
            {
                PatientId = 1,
                RecordedBy = "Dr. Updated Doctor",
                RecordType = "X-Ray",                // changed
                Title = "X-Ray Chest PA View",  // changed
                Description = "No abnormality detected",
                AttachmentFileName = "xray.jpg",
                RecordDate = DateTime.Now,
            };

            // ACT
            var result = await service.UpdateRecordAsync(1, updateDto);

            // ASSERT
            result.Should().NotBeNull();
            result.RecordType.Should().Be("X-Ray");
            result.Title.Should().Be("X-Ray Chest PA View");
        }

        // ── TEST 45 ────────────────────────────────────────────
        [Test]
        [Description("DeleteRecord removes medical record from database")]
        public async Task DeleteRecord_RemovesFromDatabase()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new MedicalRecordService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.MedicalRecords.Add(CreateTestMedicalRecord(id: 1, patientId: 1));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.DeleteRecordAsync(1);

            // ASSERT
            result.Should().BeTrue();

            // Record should be removed
            var deletedRecord = db.MedicalRecords.Find(1);
            deletedRecord.Should().BeNull();
        }
    }
}
