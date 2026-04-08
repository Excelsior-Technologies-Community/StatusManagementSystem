namespace StatusManagementSystem.Models
{
    public class ActivityDetailModel
    {
        public long ID { get; set; }
        public long ActivityId { get; set; }
        public string Title { get; set; }
        public long ActionTypeId { get; set; }
        public long PageMasterId { get; set; }
        public string InAppShow { get; set; }
    }
}