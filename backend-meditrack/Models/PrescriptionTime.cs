namespace backend_meditrack.Models
{
    public class PrescriptionTime
    {
        public int PrescriptionTimeId { get; set; }
        public TimeSpan TimeOfDay { get; set; }  // e.g., 08:00, 12:00

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
    }

}
