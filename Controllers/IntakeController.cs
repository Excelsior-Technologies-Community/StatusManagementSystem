using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class IntakeController : Controller
    {
        private readonly IConfiguration _config;

        public IntakeController(IConfiguration config)
        {
            _config = config;
        }

        string GetCon() => _config.GetConnectionString("DefaultConnection");

        // LIST
        public ActionResult Index()
        {
            List<IntakeModel> list = new List<IntakeModel>();

            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Intake_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new IntakeModel
                    {
                        ID = Convert.ToInt64(dr["ID"]),
                        Year = dr["Year"].ToString(),
                        Month = dr["Month"].ToString(),
                        Remarks = dr["Remarks"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    });
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
        public ActionResult Create(IntakeModel model, string submit)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Intake_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@Month", model.Month);
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            if (submit == "SaveAdd")
                return RedirectToAction("Create");

            return RedirectToAction("Index");
        }

        // EDIT GET
        public ActionResult Edit(long id)
        {
            IntakeModel model = new IntakeModel();

            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Intake_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.ID = Convert.ToInt64(dr["ID"]);
                    model.Year = dr["Year"].ToString();
                    model.Month = dr["Month"].ToString();
                    model.Remarks = dr["Remarks"].ToString();
                }
            }

            return View(model);
        }

        // EDIT POST
        [HttpPost]
        public ActionResult Edit(IntakeModel model)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Intake_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@Month", model.Month);
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public ActionResult Delete(long id)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Intake_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // CHANGE STATUS
        public ActionResult ChangeStatus(long id)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Intake_ChangeStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
