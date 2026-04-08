using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly IConfiguration _config;

        public ActivitiesController(IConfiguration config)
        {
            _config = config;
        }

        string GetCon() => _config.GetConnectionString("DefaultConnection");

        // LIST
        public IActionResult Index()
        {
            List<dynamic> list = new List<dynamic>();

            using SqlConnection con = new SqlConnection(GetCon());
            SqlCommand cmd = new SqlCommand("sp_GetActivities", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    ID = dr["ID"],
                    Title = dr["Title"],
                    ActionList = dr["ActionList"],
                    StatusFlag = dr["StatusFlag"]
                });
            }

            return View(list);
        }

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ActivityModel model)
        {
            long activityId = 0;

            using SqlConnection con = new SqlConnection(GetCon());

            // Insert Master
            SqlCommand cmd = new SqlCommand("sp_InsertActivity", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@InAppShow", model.InAppShow ?? "No");
            cmd.Parameters.AddWithValue("@CreateUser", 1);

            con.Open();
            activityId = Convert.ToInt64(cmd.ExecuteScalar());

            // Insert Details
            foreach (var item in model.Details)
            {
                SqlCommand dcmd = new SqlCommand("sp_InsertActivityDetail", con);
                dcmd.CommandType = CommandType.StoredProcedure;

                dcmd.Parameters.AddWithValue("@ActivityId", activityId);
                dcmd.Parameters.AddWithValue("@Title", item.Title);
                dcmd.Parameters.AddWithValue("@ActionTypeId", item.ActionTypeId);
                dcmd.Parameters.AddWithValue("@PageMasterId", item.PageMasterId);
                dcmd.Parameters.AddWithValue("@InAppShow", item.InAppShow ?? "No");
                dcmd.Parameters.AddWithValue("@CreateUser", 1);

                dcmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ActivityModel model = new ActivityModel();
            model.Details = new List<ActivityDetailModel>();

            using SqlConnection con = new SqlConnection(GetCon());

            SqlCommand cmd = new SqlCommand("sp_GetActivityById", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            // MASTER
            if (dr.Read())
            {
                model.ID = Convert.ToInt64(dr["ID"]);
                model.Title = dr["Title"].ToString();
                model.InAppShow = dr["InAppShow"].ToString();
            }

            // DETAILS
            if (dr.NextResult())
            {
                while (dr.Read())
                {
                    model.Details.Add(new ActivityDetailModel
                    {
                        ID = Convert.ToInt64(dr["ID"]),
                        ActivityId = Convert.ToInt64(dr["ActivityId"]),
                        Title = dr["Title"].ToString(),
                        ActionTypeId = Convert.ToInt64(dr["ActionTypeId"]),
                        PageMasterId = Convert.ToInt64(dr["PageMasterId"]),
                        InAppShow = dr["InAppShow"].ToString()
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ActivityModel model)
        {
            using SqlConnection con = new SqlConnection(GetCon());
            con.Open();

            // UPDATE MASTER
            SqlCommand cmd = new SqlCommand("sp_UpdateActivity", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", model.ID);
            cmd.Parameters.AddWithValue("@Title", model.Title ?? "");
            cmd.Parameters.AddWithValue("@InAppShow", model.InAppShow ?? "No");
            cmd.Parameters.AddWithValue("@UpdateUser", 1);

            cmd.ExecuteNonQuery();

            // DELETE OLD DETAILS
            SqlCommand delCmd = new SqlCommand("sp_DeleteActivityDetails", con);
            delCmd.CommandType = CommandType.StoredProcedure;
            delCmd.Parameters.AddWithValue("@ActivityId", model.ID);
            delCmd.ExecuteNonQuery();

            // INSERT NEW DETAILS
            if (model.Details != null)
            {
                foreach (var item in model.Details)
                {
                    SqlCommand dcmd = new SqlCommand("sp_InsertActivityDetail", con);
                    dcmd.CommandType = CommandType.StoredProcedure;

                    dcmd.Parameters.AddWithValue("@ActivityId", model.ID);
                    dcmd.Parameters.AddWithValue("@Title", item.Title ?? "");
                    dcmd.Parameters.AddWithValue("@ActionTypeId", item.ActionTypeId);
                    dcmd.Parameters.AddWithValue("@PageMasterId", item.PageMasterId);
                    dcmd.Parameters.AddWithValue("@InAppShow", item.InAppShow ?? "No");
                    dcmd.Parameters.AddWithValue("@CreateUser", 1);

                    dcmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            using SqlConnection con = new SqlConnection(GetCon());
            SqlCommand cmd = new SqlCommand("sp_DeleteActivity", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", id);
            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        // TOGGLE
        public IActionResult Toggle(int id)
        {
            using SqlConnection con = new SqlConnection(GetCon());
            SqlCommand cmd = new SqlCommand("sp_ToggleActivity", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", id);
            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}