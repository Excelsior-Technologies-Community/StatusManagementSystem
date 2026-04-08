using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatusManagementSystem.Models
{
    public class ExamScheduleVM
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public long? ExamTypeId { get; set; }
        public long? ProviderId { get; set; }
        public long? CenterId { get; set; }

        public DateTime? ExamDate { get; set; }
        public DateTime? ExamFromTime { get; set; }
        public DateTime? ExamToTime { get; set; }

        public DateTime? ResultDate { get; set; }

        public string IsAvailabe { get; set; }
        public string StatusFlag { get; set; }

        // Dropdowns
        public List<SelectListItem> ExamTypeList { get; set; }
        public List<SelectListItem> ProviderList { get; set; }
        public List<SelectListItem> CenterList { get; set; }
        public List<ExamDetailVM> ExamDetails { get; set; } = new List<ExamDetailVM>();
        public string ExamDetailsJson { get; set; }


        // Display
        public string ExamTypeName { get; set; }
        public string ProviderName { get; set; }
        public string CenterName { get; set; }
    }
}
