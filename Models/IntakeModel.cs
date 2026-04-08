namespace StatusManagementSystem.Models
{
    public class IntakeModel
    {
        public long ID { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Remarks { get; set; }
        public string StatusFlag { get; set; }

        public long? CreateUser { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
