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

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; }
    }

}
