namespace StatusManagementSystem.Models
{
    public class QuestionModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string AnsType { get; set; }
        public long? PageMasterId { get; set; }
        public string StatusFlag { get; set; }
    }
}
