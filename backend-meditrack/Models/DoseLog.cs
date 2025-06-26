namespace backend_meditrack.Models
{
    public class DoseLog
    {
        public int DoseLogId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? TakenTime { get; set; }

        public DoseStatus Status { get; set; } = DoseStatus.Pending;

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
    }

    public enum DoseStatus
    {
        Pending,  
        Taken,
        Missed
    }
}
