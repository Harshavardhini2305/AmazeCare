

//using AmazeCare.API.DTOs;
using AmazeCare.API.DTOS;
using AmazeCare.API.Services;
using FluentAssertions;
using NUnit.Framework;

namespace AmazeCare.Tests
{
    [TestFixture]
    public class PatientServiceTests : TestBase
    {
        // ── TEST 9 ─────────────────────────────────────────────
        [Test]
        [Description("GetAllPatients returns all active patients in the database")]
        public async Task GetAllPatients_ReturnsAllActivePatients()
        {
            // ARRANGE — add 3 active patients
            var db = CreateDbContext();
            var service = new PatientService(db);

            db.Patients.AddRange(
                CreateTestPatient(id: 1, email: "p1@test.com"),
                CreateTestPatient(id: 2, email: "p2@test.com"),
                CreateTestPatient(id: 3, email: "p3@test.com")
            );
            await db.SaveChangesAsync();

            var query = new QueryParameters { PageNumber = 1, PageSize = 10 };

            // ACT
            var result = await service.GetAllPatientsAsync(query);

            // ASSERT
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);   // 3 patients should be returned
            result.TotalCount.Should().Be(3);
        }

        // ── TEST 10 ────────────────────────────────────────────
        [Test]
        [Description("GetAllPatients does NOT return deleted (inactive) patients")]
        public async Task GetAllPatients_DoesNotReturn_InactivePatients()
        {
            // ARRANGE — 2 active, 1 deleted
            var db = CreateDbContext();
            var service = new PatientService(db);

            var active1 = CreateTestPatient(id: 1, email: "active1@test.com");
            var active2 = CreateTestPatient(id: 2, email: "active2@test.com");
            var deleted = CreateTestPatient(id: 3, email: "deleted@test.com");
            deleted.IsActive = false;  // ← soft deleted patient

            db.Patients.AddRange(active1, active2, deleted);
            await db.SaveChangesAsync();

            var query = new QueryParameters { PageNumber = 1, PageSize = 10 };

            // ACT
            var result = await service.GetAllPatientsAsync(query);

            // ASSERT — only 2 active patients returned
            result.Data.Should().HaveCount(2);
            result.Data.Should().NotContain(p => p.Email == "deleted@test.com");
        }

        // ── TEST 11 ────────────────────────────────────────────
        [Test]
        [Description("GetPatientById returns the correct patient by ID")]
        public async Task GetPatientById_WithValidId_ReturnsPatient()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PatientService(db);

            db.Patients.Add(CreateTestPatient(
                id: 5,
                name: "Ramesh Kumar",
                email: "ramesh@test.com"));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.GetPatientByIdAsync(5);

            // ASSERT
            result.Should().NotBeNull();
            result!.PatientId.Should().Be(5);
            result.FullName.Should().Be("Ramesh Kumar");
            result.Email.Should().Be("ramesh@test.com");
        }

        // ── TEST 12 ────────────────────────────────────────────
        [Test]
        [Description("GetPatientById returns null when patient ID does not exist")]
        public async Task GetPatientById_WithInvalidId_ReturnsNull()
        {
            // ARRANGE — empty database
            var db = CreateDbContext();
            var service = new PatientService(db);

            // ACT
            var result = await service.GetPatientByIdAsync(999); // ID that doesn't exist

            // ASSERT
            result.Should().BeNull();
        }

        // ── TEST 13 ────────────────────────────────────────────
        [Test]
        [Description("UpdatePatient changes only the fields that are provided")]
        public async Task UpdatePatient_WithValidData_UpdatesSuccessfully()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PatientService(db);

            db.Patients.Add(CreateTestPatient(
                id: 1,
                name: "Old Name",
                email: "patient@test.com"));
            await db.SaveChangesAsync();

            var updateDto = new UpdatePatientDto
            {
                FullName = "New Name",       // ← changing name
                MobileNumber = "9999999999",     // ← changing mobile
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 1),
            };

            // ACT
            var result = await service.UpdatePatientAsync(1, updateDto);

            // ASSERT
            result.Should().NotBeNull();
            result.FullName.Should().Be("New Name");          // name changed
            result.MobileNumber.Should().Be("9999999999");    // mobile changed
        }

        // ── TEST 14 ────────────────────────────────────────────
        [Test]
        [Description("DeletePatient performs soft delete — sets IsActive to false")]
        public async Task DeletePatient_SetsIsActiveToFalse()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new PatientService(db);

            db.Patients.Add(CreateTestPatient(id: 1, email: "patient@test.com"));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.DeletePatientAsync(1);

            // ASSERT
            result.Should().BeTrue();  // deletion was successful

            // Check in DB — patient should still exist but IsActive = false
            var deletedPatient = db.Patients.Find(1);
            deletedPatient.Should().NotBeNull();         // still in database!
            deletedPatient!.IsActive.Should().BeFalse(); // but marked as deleted
        }

        // ── TEST 15 ────────────────────────────────────────────
        [Test]
        [Description("DeletePatient returns false when patient ID does not exist")]
        public async Task DeletePatient_WithInvalidId_ReturnsFalse()
        {
            // ARRANGE — empty database
            var db = CreateDbContext();
            var service = new PatientService(db);

            // ACT
            var result = await service.DeletePatientAsync(999);

            // ASSERT
            result.Should().BeFalse();
        }
    }
}
