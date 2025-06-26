namespace backend_meditrack.Models
{
    public class PrescriptionDTO
    {
        public string MedicineName { get; set; }
        public string Instruction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurringIntervalHours { get; set; }
        public int PatientId { get; set; }

        public List<int>? Days { get; set; } // 0=Sunday ... 6=Saturday
        public List<string>? Times { get; set; } // e.g. "08:00", "14:30"
    }
}
