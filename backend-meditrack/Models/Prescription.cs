using System.ComponentModel.DataAnnotations;

namespace backend_meditrack.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public string MedicineName { get; set; }
        public string Instruction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRecurring { get; set; } // true = daily, false = specific days
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public ICollection<PrescriptionDay> PrescriptionDays { get; set; }
        public ICollection<PrescriptionTime> PrescriptionTimes { get; set; }
        public ICollection<DoseLog> DoseLogs { get; set; }
    }


}
