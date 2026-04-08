using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class QuestionMasterController : Controller
    {
        private readonly IConfiguration _config;

        public QuestionMasterController(IConfiguration config)
        {
            _config = config;
        }

        string cs => _config.GetConnectionString("DefaultConnection");

        // ===================== LIST =====================
        public IActionResult Index()
        {
            List<QuestionModel> list = new List<QuestionModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetQuestions", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new QuestionModel
                    {
                        Id = Convert.ToInt64(dr["Id"]),
                        Title = dr["Title"].ToString(),
                        AnsType = dr["AnsType"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    });
                }
            }

            return View(list);
        }

        // ===================== CREATE =====================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(QuestionModel model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@AnsType", model.AnsType);
                cmd.Parameters.AddWithValue("@PageMasterId", model.PageMasterId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ===================== EDIT =====================
        public IActionResult Edit(long id)
        {
            QuestionModel model = new QuestionModel();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetQuestionById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.Id = Convert.ToInt64(dr["Id"]);
                    model.Title = dr["Title"].ToString();
                    model.AnsType = dr["AnsType"].ToString();
                    model.PageMasterId = dr["PageMasterId"] as long?;
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(QuestionModel model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@AnsType", model.AnsType);
                cmd.Parameters.AddWithValue("@PageMasterId", model.PageMasterId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ===================== DELETE =====================
        public IActionResult Delete(long id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteQuestion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ===================== STATUS =====================
        public IActionResult ChangeStatus(long id, string status)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ChangeQuestionStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
