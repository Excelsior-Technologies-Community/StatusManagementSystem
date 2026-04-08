using Microsoft.AspNetCore.Mvc.Rendering;

namespace StatusManagementSystem.Models
{
    public class CurrencyModel
    {
        public int ID { get; set; }
        public long? CountryId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string StatusFlag { get; set; }

        // Extra for display
        public string CountryName { get; set; }
        public List<SelectListItem> CountryList { get; set; }
    }
}
