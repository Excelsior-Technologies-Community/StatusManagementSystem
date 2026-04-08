namespace StatusManagementSystem.Models
{
    public class ExamDetailVM
    {
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }

        public long CenterId { get; set; }
        public string CenterName { get; set; }

        public DateTime? ExamDate { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
    }
}