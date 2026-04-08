using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class ServicePackageController : Controller
    {
        private readonly IConfiguration _config;

        public ServicePackageController(IConfiguration config)
        {
            _config = config;
        }

        string cs => _config.GetConnectionString("DefaultConnection");

        // INDEX
        public IActionResult Index()
        {
            List<ServiceModel> list = new List<ServiceModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Service_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new ServiceModel
                    {
                        ID = Convert.ToInt64(dr["ID"]),
                        Title = dr["Title"].ToString(),
                        Alias = dr["Alias"].ToString(),
                        TotalFees = dr["TotalFees"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalFees"]),
                        Description = dr["Description"].ToString(),
                        GSTName = dr["GSTName"].ToString(),

                        IsAgreementsSend = dr["IsAgreementsSend"].ToString(),
                        IsReceiptSend = dr["IsReceiptSend"].ToString(),
                        IsCollegeApplicationActivities = dr["IsCollegeApplicationActivities"].ToString(),
                        IsCourseOptions = dr["IsCourseOptions"].ToString(),
                        IsFeesPayment = dr["IsFeesPayment"].ToString(),
                        IsPostDecisionActivities = dr["IsPostDecisionActivities"].ToString(),
                        IsAlliedServices = dr["IsAlliedServices"].ToString(),

                        StatusFlag = dr["StatusFlag"].ToString()
                    });
                }
            }

            return View(list);
        }


        // Get Dropdown
        private List<SelectListItem> GetDropdown(string sp, string valueColumn, string textColumn)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(sp, con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr[valueColumn].ToString(),   
                        Text = dr[textColumn].ToString()
                    });
                }
            }

            return list;
        }


        // CREATE
        public IActionResult Create()
        {
            ServicePackageVM vm = new ServicePackageVM();

            vm.GSTList = GetDropdown("sp_GetGstList", "ID", "Title");
            vm.CountryList = GetDropdown("sp_GetCountryList", "CountryId", "CountryName");
            vm.VisaTypeList = GetDropdown("sp_GetVisaTypes", "ID", "Title");
            vm.ActivityList = GetDropdown("sp_GetActivities", "ID", "Title");
            vm.DocumentTypeList = GetDropdown("sp_GetDocumentTypes", "ID", "Title");
            vm.QuestionList = GetDropdown("sp_GetQuestions", "ID", "Title");
            vm.DocumentList = GetDropdown("sp_GetDocuments", "ID", "Title");

            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(ServicePackageVM vm)
        {
            var model = vm.Service;

            long serviceId = 0;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Service_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Title", model.Title ?? "");
                cmd.Parameters.AddWithValue("@ReportHeader", model.ReportHeader ?? "");
                cmd.Parameters.AddWithValue("@Alias", model.Alias ?? "");
                cmd.Parameters.AddWithValue("@GSTId", model.GSTId);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@VisaId", model.VisaTypeId);
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);
                cmd.Parameters.AddWithValue("@VisaTypeId", model.VisaTypeId);
                cmd.Parameters.AddWithValue("@IsCoaching", model.IsCoaching ?? "No");
                cmd.Parameters.AddWithValue("@IsServiceComboPackage", model.IsServiceComboPackage ?? "No");
                cmd.Parameters.AddWithValue("@IsAgreementsSend", model.IsAgreementsSend ?? "No");
                cmd.Parameters.AddWithValue("@IsReceiptSend", model.IsReceiptSend ?? "No");
                cmd.Parameters.AddWithValue("@InAppShow", model.InAppShow ?? "No");
                cmd.Parameters.AddWithValue("@TermsAndCondition", model.TermsAndCondition ?? "");
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                serviceId = Convert.ToInt64(cmd.ExecuteScalar());
            }

            // SAVE ACTIVITIES
            if (model.Activities != null)
            {
                foreach (var act in model.Activities)
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("sp_ServiceActivities_Insert", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        cmd.Parameters.AddWithValue("@ActivitiesId", act.ActivitiesId);
                        cmd.Parameters.AddWithValue("@MilestoneAlias", act.MilestoneAlias);
                        cmd.Parameters.AddWithValue("@DueDays", act.DueDays);
                        cmd.Parameters.AddWithValue("@ActivitiesAmount", act.ActivitiesAmount);
                        cmd.Parameters.AddWithValue("@CreateUser", 1);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // SAVE QUESTIONS
            if (model.Questions != null)
            {
                foreach (var q in model.Questions)
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("sp_ServiceQuestion_Insert", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        cmd.Parameters.AddWithValue("@QuestionId", q.Id);
                        cmd.Parameters.AddWithValue("@AnswerType", q.AnsType);
                        cmd.Parameters.AddWithValue("@CreateUser", 1);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // SAVE DOCUMENTS
            if (model.Documents != null)
            {
                foreach (var doc in model.Documents)
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("sp_ServiceDocument_Insert", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        cmd.Parameters.AddWithValue("@DocumentTypeId", doc.DocumentTypeId);
                        cmd.Parameters.AddWithValue("@DocumentId", doc.DocumentId);
                        cmd.Parameters.AddWithValue("@CreateUser", 1);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToAction("Index");
        }


        public IActionResult Edit(long id)
        {
            ServicePackageVM vm = new ServicePackageVM();

            vm.Service = new ServiceModel();
            vm.Activities = new List<ServiceActivityModel>();
            vm.Documents = new List<ServiceDocumentModel>();
            vm.Questions = new List<QuestionModel>(); 

            // 🔹 Load dropdowns
            vm.ActivityList = GetDropdown("sp_GetActivities", "ID", "Title");
            vm.DocumentTypeList = GetDropdown("sp_GetDocumentTypes", "ID", "Title");
            vm.DocumentList = GetDropdown("sp_GetDocuments", "ID", "Title");
            vm.QuestionList = GetDropdown("sp_GetQuestions", "ID", "Title");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // 🔹 SERVICE
                SqlCommand cmd = new SqlCommand("sp_Service_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    vm.Service.ID = id;
                    vm.Service.Title = dr["Title"].ToString();
                    vm.Service.Alias = dr["Alias"].ToString();
                    vm.Service.ReportHeader = dr["ReportHeader"].ToString();
                    vm.Service.Description = dr["Description"].ToString();

                    // IMPORTANT FIX
                    vm.Service.GSTId = dr["GSTId"] == DBNull.Value ? 0 : Convert.ToInt64(dr["GSTId"]);
                    vm.Service.CountryId = dr["CountryId"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CountryId"]);
                    vm.Service.VisaTypeId = dr["VisaTypeId"] == DBNull.Value ? 0 : Convert.ToInt64(dr["VisaTypeId"]);
                }
                dr.Close();

                // 🔹 ACTIVITIES
                cmd = new SqlCommand("sp_ServiceActivities_ByService", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceId", id);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    vm.Activities.Add(new ServiceActivityModel
                    {
                        ActivitiesId = Convert.ToInt64(dr["ActivitiesId"]),
                        MilestoneAlias = dr["MilestoneAlias"].ToString(),
                        DueDays = dr["DueDays"].ToString(),
                        ActivitiesAmount = dr["ActivitiesAmount"].ToString()
                    });
                }
                dr.Close();

                // 🔹 DOCUMENTS
                cmd = new SqlCommand("sp_ServiceDocuments_ByService", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceId", id);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    vm.Documents.Add(new ServiceDocumentModel
                    {
                        DocumentTypeId = Convert.ToInt64(dr["DocumentTypeId"]),
                        DocumentId = Convert.ToInt64(dr["DocumentId"])
                    });
                }
                dr.Close();

                // 🔥 QUESTIONS (IMPORTANT FIX)
                cmd = new SqlCommand("sp_ServiceQuestions_GetByServiceId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceId", id);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    vm.Questions.Add(new QuestionModel
                    {
                        Id = Convert.ToInt64(dr["QuestionId"]),
                        AnsType = dr["AnswerType"].ToString()
                    });
                }
                dr.Close();
            }

            return View(vm);
        }


        [HttpPost]
        public IActionResult Edit(ServicePackageVM vm)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // UPDATE MAIN
                SqlCommand cmd = new SqlCommand("sp_Service_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", vm.Service.ID);
                cmd.Parameters.AddWithValue("@Title", vm.Service.Title);
                cmd.Parameters.AddWithValue("@ReportHeader", vm.Service.ReportHeader);
                cmd.Parameters.AddWithValue("@Alias", vm.Service.Alias);
                cmd.Parameters.AddWithValue("@GSTId", vm.Service.GSTId);
                cmd.Parameters.AddWithValue("@Description", vm.Service.Description);
                cmd.Parameters.AddWithValue("@VisaId", vm.Service.VisaId);
                cmd.Parameters.AddWithValue("@CountryId", vm.Service.CountryId);
                cmd.Parameters.AddWithValue("@VisaTypeId", vm.Service.VisaTypeId);
                cmd.Parameters.AddWithValue("@IsCoaching", vm.Service.IsCoaching ?? "No");
                cmd.Parameters.AddWithValue("@IsServiceComboPackage", vm.Service.IsServiceComboPackage ?? "No");
                cmd.Parameters.AddWithValue("@IsAgreementsSend", vm.Service.IsAgreementsSend ?? "No");
                cmd.Parameters.AddWithValue("@IsReceiptSend", vm.Service.IsReceiptSend ?? "No");
                cmd.Parameters.AddWithValue("@InAppShow", vm.Service.InAppShow ?? "No");
                cmd.Parameters.AddWithValue("@TermsAndCondition", vm.Service.TermsAndCondition ?? "");
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                cmd.ExecuteNonQuery();

                // DELETE OLD CHILD
                cmd = new SqlCommand("sp_Service_DeleteChildData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceId", vm.Service.ID);
                cmd.ExecuteNonQuery();

                // INSERT AGAIN (Activities)
                foreach (var act in vm.Activities)
                {
                    cmd = new SqlCommand("sp_ServiceActivities_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ServiceId", vm.Service.ID);
                    cmd.Parameters.AddWithValue("@ActivitiesId", act.ActivitiesId);
                    cmd.Parameters.AddWithValue("@MilestoneAlias", act.MilestoneAlias);
                    cmd.Parameters.AddWithValue("@DueDays", act.DueDays);
                    cmd.Parameters.AddWithValue("@ActivitiesAmount", act.ActivitiesAmount);
                    cmd.Parameters.AddWithValue("@CreateUser", 1);

                    cmd.ExecuteNonQuery();
                }

                // INSERT QUESTIONS
                foreach (var q in vm.Questions)
                {
                    cmd = new SqlCommand("sp_ServiceQuestion_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ServiceId", vm.Service.ID);
                    cmd.Parameters.AddWithValue("@QuestionId", q.Id);
                    cmd.Parameters.AddWithValue("@AnswerType", q.AnsType);
                    cmd.Parameters.AddWithValue("@CreateUser", 1);

                    cmd.ExecuteNonQuery();
                }

                // INSERT AGAIN (Documents)
                foreach (var doc in vm.Documents)
                {
                    cmd = new SqlCommand("sp_ServiceDocument_Insert", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ServiceId", vm.Service.ID);
                    cmd.Parameters.AddWithValue("@DocumentTypeId", doc.DocumentTypeId);
                    cmd.Parameters.AddWithValue("@DocumentId", doc.DocumentId);
                    cmd.Parameters.AddWithValue("@CreateUser", 1);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ToggleStatus(long id, string status)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Service_ToggleStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index"); 
        }

        
        public IActionResult Delete(long id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Service_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

    }
}
