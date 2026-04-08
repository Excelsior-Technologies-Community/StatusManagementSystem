using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class VisaTypeController : Controller
    {
        private readonly IConfiguration _config;
        public VisaTypeController(IConfiguration config)
        {
            _config = config;
        }

        string cs => _config.GetConnectionString("DefaultConnection");

        // ================= INDEX =================
        public IActionResult Index()
        {
            List<VisaTypeModel> list = new List<VisaTypeModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetVisaTypes", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new VisaTypeModel
                    {
                        Id = Convert.ToInt64(dr["Id"]),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    });
                }
            }

            return View(list);
        }

        // ================= CREATE =================
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(VisaTypeModel model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertVisaType", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ================= EDIT =================
        public IActionResult Edit(long id)
        {
            VisaTypeModel model = new VisaTypeModel();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetVisaTypeById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.Id = Convert.ToInt64(dr["Id"]);
                    model.Title = dr["Title"].ToString();
                    model.Description = dr["Description"].ToString();
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(VisaTypeModel model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateVisaType", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ================= DELETE =================
        public IActionResult Delete(long id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteVisaType", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ================= STATUS =================
        public IActionResult Toggle(long id, string status)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ChangeVisaTypeStatus", con);
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
