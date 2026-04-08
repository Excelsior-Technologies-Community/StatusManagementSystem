using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatusManagementSystem.Models
{
    public class ServicePackageVM
    {
        public ServiceModel Service { get; set; }

        public List<ServiceActivityModel> Activities { get; set; }
        public List<QuestionModel> Questions { get; set; }
        public List<ServiceDocumentModel> Documents { get; set; }


        // DROPDOWNS
        public List<SelectListItem> GSTList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> VisaList { get; set; }
        public List<SelectListItem> VisaTypeList { get; set; }
        public List<SelectListItem> ActivityList { get; set; }
        public List<SelectListItem> DocumentTypeList { get; set; }
        public List<SelectListItem> QuestionList { get; set; }
        public List<SelectListItem> DocumentList { get; set; }
    }
}
