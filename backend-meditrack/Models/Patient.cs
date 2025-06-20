using System.ComponentModel.DataAnnotations;

namespace backend_meditrack.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public Doctor? Doctor { get; set; }  // ← ⚠️ Make this nullable with `?`

        public ICollection<Prescription>? Prescriptions { get; set; }  // ← ⚠️ Also nullable
    }

}
