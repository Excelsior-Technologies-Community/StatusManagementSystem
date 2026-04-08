namespace StatusManagementSystem.Models
{
    public class DocumentModel
    {
        public long Id { get; set; }
        public long DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StatusFlag { get; set; }
    }
}
