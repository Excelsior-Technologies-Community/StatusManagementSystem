using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class ExamProviderController : Controller
    {
        private readonly string cs = "Server=.;Database=StatusDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public ActionResult Index()
        {
            List<ExamProviderVM> list = new List<ExamProviderVM>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamProvider_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new ExamProviderVM
                    {
                        Id = Convert.ToInt64(dr["ID"]),
                        Title = dr["Title"].ToString(),
                        Website = dr["Website"].ToString(),
                        Description = dr["Description"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString(),
                        ExamTypeName = dr["ExamTypeName"].ToString()
                    });
                }
            }

            return View(list);
        }

        public ActionResult Create()
        {
            ExamProviderVM model = new ExamProviderVM();
            model.ExamTypeList = GetExamTypes();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ExamProviderVM model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamProvider_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ExamTypeId", model.ExamTypeId);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Website", model.Website ?? "");
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(long id)
        {
            ExamProviderVM model = new ExamProviderVM();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamProvider_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.Id = id;
                    model.ExamTypeId = Convert.ToInt64(dr["ExamTypeID"]);
                    model.Title = dr["Title"].ToString();
                    model.Website = dr["Website"].ToString();
                    model.Description = dr["Description"].ToString();
                }
            }

            model.ExamTypeList = GetExamTypes();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ExamProviderVM model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamProvider_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@ExamTypeId", model.ExamTypeId);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Website", model.Website ?? "");
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(long id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamProvider_Delete", con);
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
                SqlCommand cmd = new SqlCommand("sp_ExamProvider_ToggleStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetExamTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT ID, Title FROM tbl_Exam_Type WHERE StatusFlag='Active'", con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr["ID"].ToString(),
                        Text = dr["Title"].ToString()
                    });
                }
            }

            return list;
        }
    }
}
