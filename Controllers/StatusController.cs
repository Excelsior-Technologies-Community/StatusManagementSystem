using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class StatusController : Controller
    {
        private readonly IConfiguration _config;

        public StatusController(IConfiguration config)
        {
            _config = config;
        }

        string GetConnection() => _config.GetConnectionString("DefaultConnection");

        // LIST
        public IActionResult Index()
        {
            List<StatusModel> list = new List<StatusModel>();

            using (SqlConnection con = new SqlConnection(GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("sp_GetStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new StatusModel
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        StatusCode = dr["StatusCode"].ToString(),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    });
                }
            }

            return View(list);
        }

        // ADD
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StatusModel model)
        {
            using (SqlConnection con = new SqlConnection(GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StatusCode", model.StatusCode ?? "");
                cmd.Parameters.AddWithValue("@Title", model.Title ?? "");
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // EDIT
        public IActionResult Edit(int id)
        {
            StatusModel model = new StatusModel();

            using (SqlConnection con = new SqlConnection(GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("sp_GetStatusById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.ID = Convert.ToInt32(dr["ID"]);
                    model.StatusCode = dr["StatusCode"].ToString();
                    model.Title = dr["Title"].ToString();
                    model.Description = dr["Description"].ToString();
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(StatusModel model)
        {
            using (SqlConnection con = new SqlConnection(GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@StatusCode", model.StatusCode ?? "");
                cmd.Parameters.AddWithValue("@Title", model.Title ?? "");
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // TOGGLE ACTIVE
        public IActionResult Toggle(int id)
        {
            using (SqlConnection con = new SqlConnection(GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("sp_ToggleStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}