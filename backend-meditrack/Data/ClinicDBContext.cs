using backend_meditrack.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_meditrack.Data
{
    public class ClinicDbContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionDay> PrescriptionDays { get; set; }
        public DbSet<PrescriptionTime> PrescriptionTimes { get; set; }
        public DbSet<DoseLog> DoseLogs { get; set; }

        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Patient-Doctor (Many-to-One)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(p => p.DoctorId);

            // Prescription-Patient (Many-to-One)
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(pat => pat.Prescriptions)
                .HasForeignKey(p => p.PatientId);

            // PrescriptionDay-Prescription (Many-to-One)
            modelBuilder.Entity<PrescriptionDay>()
                .HasOne(pd => pd.Prescription)
                .WithMany(p => p.PrescriptionDays)
                .HasForeignKey(pd => pd.PrescriptionId);

            // PrescriptionTime-Prescription (Many-to-One)
            modelBuilder.Entity<PrescriptionTime>()
                .HasOne(pt => pt.Prescription)
                .WithMany(p => p.PrescriptionTimes)
                .HasForeignKey(pt => pt.PrescriptionId);

            // DoseLog-Prescription (Many-to-One)
            modelBuilder.Entity<DoseLog>()
                .HasOne(dl => dl.Prescription)
                .WithMany(p => p.DoseLogs)
                .HasForeignKey(dl => dl.PrescriptionId);
        }
    }
}
