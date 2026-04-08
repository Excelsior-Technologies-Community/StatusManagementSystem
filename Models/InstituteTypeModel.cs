namespace StatusManagementSystem.Models
{
    public class InstituteTypeModel
    {
        public int ID { get; set; }

        public string Title { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string StatusFlag { get; set; }

        public long? CreateUser { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
