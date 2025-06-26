namespace backend_meditrack.Models
{
    public class PrescriptionDTO
    {
        public string MedicineName { get; set; }
        public string Instruction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRecurring { get; set; } // true = daily, false = specific days
        public int PatientId { get; set; }

        public List<int>? Days { get; set; } // only if specific days
        public List<string>? Times { get; set; } // always required
    }
}
