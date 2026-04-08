namespace StatusManagementSystem.Models
{
    public class ServiceModel
    {
        public long ID { get; set; }

        public string Title { get; set; }
        public string ReportHeader { get; set; }
        public string Alias { get; set; }

        public long GSTId { get; set; }
        public decimal? TotalFees { get; set; }  
        public string Description { get; set; }

        public long VisaId { get; set; }
        public long CountryId { get; set; }
        public long VisaTypeId { get; set; }

        // BOOLEAN FLAGS (YES/NO)
        public string IsCoaching { get; set; }
        public string IsServiceComboPackage { get; set; }
        public string IsAgreementsSend { get; set; }
        public string IsReceiptSend { get; set; }
        public string InAppShow { get; set; }
        public string GSTName { get; set; }

        public string IsCollegeApplicationActivities { get; set; }
        public string IsCourseOptions { get; set; }                  
        public string IsFeesPayment { get; set; }                    
        public string IsPostDecisionActivities { get; set; }         
        public string IsAlliedServices { get; set; }                 
        public string StatusFlag { get; set; }                      

        public string TermsAndCondition { get; set; }

        public List<ServiceActivityModel> Activities { get; set; }
        public List<QuestionModel> Questions { get; set; }
        public List<ServiceDocumentModel> Documents { get; set; }
    }
}