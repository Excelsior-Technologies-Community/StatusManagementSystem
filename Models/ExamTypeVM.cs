namespace StatusManagementSystem.Models
{
    public class ExamTypeVM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StatusFlag { get; set; }

        public bool IsLead { get; set; }
        public bool IsInquiry { get; set; }
        public bool IsRegistration { get; set; }
        public bool IsCoaching { get; set; }
        public bool IsProcess { get; set; }
        public bool IsMock { get; set; }
        public bool IsProfessional { get; set; }
        public bool IsEnglishTest { get; set; }

        public List<string> GradeList { get; set; } = new List<string>();

        public string GradeInput { get; set; } // for textbox
    }
}
