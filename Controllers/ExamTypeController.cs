using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class ExamTypeController : Controller
    {
        private readonly string cs = "Server=.;Database=StatusDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // LIST
        public ActionResult Index()
        {
            List<ExamTypeVM> list = new List<ExamTypeVM>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamType_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    var item = new ExamTypeVM
                    {
                        Id = Convert.ToInt64(dr["ID"]),
                        Title = dr["Title"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    };

                    string json = dr["Description"]?.ToString();

                    if (!string.IsNullOrEmpty(json) && json.Trim().StartsWith("{"))
                    {
                        try
                        {
                            dynamic data = JsonConvert.DeserializeObject(json);
                            item.Description = data.Description;

                            item.IsLead = data.IsLead ?? false;
                            item.IsInquiry = data.IsInquiry ?? false;
                            item.IsRegistration = data.IsRegistration ?? false;
                            item.IsCoaching = data.IsCoaching ?? false;
                            item.IsProcess = data.IsProcess ?? false;
                            item.IsMock = data.IsMock ?? false;
                            item.IsProfessional = data.IsProfessional ?? false;
                            item.IsEnglishTest = data.IsEnglishTest ?? false;
                        }
                        catch
                        {
                            item.Description = json; 
                        }
                    }
                    else
                    {
                        
                        item.Description = json;
                    }

                    list.Add(item);
                }
            }

            return View(list);
        }

        // CREATE GET
        public ActionResult Create()
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        public ActionResult Create(ExamTypeVM model, string GradeTitles)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamType_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                var checkboxData = new
                {
                    model.IsLead,
                    model.IsInquiry,
                    model.IsRegistration,
                    model.IsCoaching,
                    model.IsProcess,
                    model.IsMock,
                    model.IsProfessional,
                    model.IsEnglishTest
                };

                string json = JsonConvert.SerializeObject(checkboxData);

                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@StatusFlag", "Active");
                cmd.Parameters.AddWithValue("@CreateUser", 1);
                cmd.Parameters.AddWithValue("@GradeTitles", GradeTitles);
                cmd.Parameters.AddWithValue("@CheckboxJson", json);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // EDIT GET
        public ActionResult Edit(long id)
        {
            ExamTypeVM model = new ExamTypeVM();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamType_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.Id = id;
                    model.Title = dr["Title"].ToString();
                    model.Description = dr["Description"].ToString();
                    string json = dr["Description"]?.ToString();

                    if (!string.IsNullOrEmpty(json) && json.Trim().StartsWith("{"))
                    {
                        try
                        {
                            dynamic data = JsonConvert.DeserializeObject(json);

                            model.IsLead = data.IsLead ?? false;
                            model.IsInquiry = data.IsInquiry ?? false;
                            model.IsRegistration = data.IsRegistration ?? false;
                            model.IsCoaching = data.IsCoaching ?? false;
                            model.IsProcess = data.IsProcess ?? false;
                            model.IsMock = data.IsMock ?? false;
                            model.IsProfessional = data.IsProfessional ?? false;
                            model.IsEnglishTest = data.IsEnglishTest ?? false;
                        }
                        catch
                        {
                            // ignore invalid JSON
                        }
                    }
                }

                if (dr.NextResult())
                {
                    while (dr.Read())
                    {
                        model.GradeList.Add(dr["Title"].ToString());
                    }
                }
            }

            return View(model);
        }

        // EDIT POST
        [HttpPost]
        public ActionResult Edit(ExamTypeVM model, string GradeTitles)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamType_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@StatusFlag", "Active");
                cmd.Parameters.AddWithValue("@UpdateUser", 1);
                cmd.Parameters.AddWithValue("@GradeTitles", GradeTitles);

                //  JSON
                var checkboxData = new
                {
                    model.Description, 
                    model.IsLead,
                    model.IsInquiry,
                    model.IsRegistration,
                    model.IsCoaching,
                    model.IsProcess,
                    model.IsMock,
                    model.IsProfessional,
                    model.IsEnglishTest
                };

                string json = JsonConvert.SerializeObject(checkboxData);

                cmd.Parameters.AddWithValue("@CheckboxJson", json);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public ActionResult Delete(long id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamType_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        public ActionResult ToggleStatus(long id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamType_ToggleStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


    }
}
