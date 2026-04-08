using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatusManagementSystem.Models
{
    public class ExamProviderVM
    {
        public long Id { get; set; }
        public long ExamTypeId { get; set; }
        public string ExamTypeName { get; set; }
        public string Title { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string StatusFlag { get; set; }

        public List<SelectListItem> ExamTypeList { get; set; }
    }
}
