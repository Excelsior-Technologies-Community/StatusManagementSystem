namespace StatusManagementSystem.Models
{
    public class ActivityModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string InAppShow { get; set; }
        public string StatusFlag { get; set; }

        public List<ActivityDetailModel> Details { get; set; }
    }
}