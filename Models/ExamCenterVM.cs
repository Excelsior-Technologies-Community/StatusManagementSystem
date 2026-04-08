using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatusManagementSystem.Models
{
    public class ExamCenterVM
    {
        public long Id { get; set; }
        public string ExamCenterName { get; set; }
        public long ExamTypeId { get; set; }
        public long ExamProviderId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }
        public long AreaId { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public string StatusFlag { get; set; }
        public string ProviderName { get; set; }
        public List<SelectListItem> ProviderList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> CityList { get; set; }
        public List<SelectListItem> AreaList { get; set; }
    }
}
