using Microsoft.EntityFrameworkCore;
using AmazeCare.API.Models;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer; 



namespace AmazeCare.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ── DbSets = Database Tables ──────────────────────────────
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }      // NEW
        public DbSet<MedicalRecord> MedicalRecords { get; set; }    // NEW



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── PATIENT ───────────────────────────────────────────
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.PatientId);
                entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(p => p.Email).IsUnique();
                entity.Property(p => p.Gender).HasMaxLength(20);
                entity.Property(p => p.MobileNumber).HasMaxLength(15);
            });

            // ── DOCTOR ────────────────────────────────────────────
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.DoctorId);
                entity.Property(d => d.FullName).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(d => d.Email).IsUnique();
                entity.Property(d => d.Specialty).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Qualification).HasMaxLength(100);
                entity.Property(d => d.Designation).HasMaxLength(100);
            });

            // ── ADMIN ─────────────────────────────────────────────
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(a => a.AdminId);
                entity.Property(a => a.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(a => a.Email).IsUnique();
            });

            // ── APPOINTMENT ───────────────────────────────────────
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.AppointmentId);
                entity.Property(a => a.Status).HasDefaultValue("Pending");
                entity.Property(a => a.TimeSlot).HasMaxLength(20);
                entity.Property(a => a.Symptoms).HasMaxLength(500);
                entity.Property(a => a.NatureOfVisit).HasMaxLength(200);

                // One-to-Many: Patient → Appointments
                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One-to-Many: Doctor → Appointments
                entity.HasOne(a => a.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── CONSULTATION ──────────────────────────────────────
            modelBuilder.Entity<Consultation>(entity =>
            {
                entity.HasKey(c => c.ConsultationId);
                entity.Property(c => c.CurrentSymptoms).HasMaxLength(1000);
                entity.Property(c => c.Diagnosis).HasMaxLength(500);
                entity.Property(c => c.TreatmentPlan).HasMaxLength(1000);
                entity.Property(c => c.RecommendedTests).HasMaxLength(500);

                // One-to-One: Appointment ↔ Consultation
                entity.HasOne(c => c.Appointment)
                      .WithOne(a => a.Consultation)
                      .HasForeignKey<Consultation>(c => c.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── PRESCRIPTION (NEW) ────────────────────────────────
            // One Consultation → Many Prescriptions
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(p => p.PrescriptionId);
                entity.Property(p => p.MedicineName).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Dosage).IsRequired().HasMaxLength(20);  // e.g. "1-0-1"
                entity.Property(p => p.Timing).HasMaxLength(10);               // AF or BF
                entity.Property(p => p.Duration).HasMaxLength(50);
                entity.Property(p => p.Notes).HasMaxLength(300);

                // One-to-Many: Consultation → Prescriptions (Fluent API)
                entity.HasOne(p => p.Consultation)
                      .WithMany(c => c.Prescriptions)
                      .HasForeignKey(p => p.ConsultationId)
                      .OnDelete(DeleteBehavior.Cascade); // delete prescriptions if consultation deleted
            });

            // ── MEDICAL RECORD (NEW) ──────────────────────────────
            // One Patient → Many MedicalRecords
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(m => m.RecordId);
                entity.Property(m => m.RecordType).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Description).HasMaxLength(2000);
                entity.Property(m => m.RecordedBy).HasMaxLength(100);
                entity.Property(m => m.AttachmentFileName).HasMaxLength(255);

                // One-to-Many: Patient → MedicalRecords (Fluent API)
                entity.HasOne(m => m.Patient)
                      .WithMany(p => p.MedicalRecords)
                      .HasForeignKey(m => m.PatientId)
                      .OnDelete(DeleteBehavior.Cascade); // delete records if patient deleted
            });

            // ── SEED DEFAULT ADMIN ────────────────────────────────
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                AdminId = 1,
                FullName = "Super Admin",
                Email = "admin@amazecare.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin"
            });
        }

    }
}
