using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatusManagementSystem.Models
{
    public class InstituteModel
    {
        public long ID { get; set; }

        public long? InstituteTypeId { get; set; }
        public string InstituteName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Institutecode { get; set; }

        public long? CountryId { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public long? AreaId { get; set; }

        public string Address { get; set; }
        public string Remarks { get; set; }
        public string InstituteLogo { get; set; }
        public string StatusFlag { get; set; }

        // Display Fields
        public string InstituteTypeName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }

        // Dropdowns
        public List<SelectListItem> InstituteTypeList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> CityList { get; set; }
        public List<SelectListItem> AreaList { get; set; }
    }
}
