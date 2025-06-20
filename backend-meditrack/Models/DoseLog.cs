namespace backend_meditrack.Models
{
    public class DoseLog
    {
        public int DoseLogId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? TakenTime { get; set; }

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
    }
}
