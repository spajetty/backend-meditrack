namespace backend_meditrack.Models
{
    public class PrescriptionDay
    {
        public int PrescriptionDayId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }  // Enum → Sunday, Monday, etc.

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
    }

}
