using System.ComponentModel.DataAnnotations;

namespace backend_meditrack.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Specialty { get; set; }

        public ICollection<Patient> Patients { get; set; }
    }

}
