using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class InstituteTypeController : Controller
    {
        private readonly IConfiguration _config;

        public InstituteTypeController(IConfiguration config)
        {
            _config = config;
        }

        string GetCon() => _config.GetConnectionString("DefaultConnection"); 
        // LIST
        public ActionResult Index()
        {
            List<InstituteTypeModel> list = new List<InstituteTypeModel>();

            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_InstituteType_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new InstituteTypeModel
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        Title = dr["Title"].ToString(),
                        ShortName = dr["ShortName"].ToString(),
                        Description = dr["Description"].ToString(),
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
        public ActionResult Create(InstituteTypeModel model, string submit)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_InstituteType_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@ShortName", model.ShortName);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            if (submit == "SaveAdd")
                return RedirectToAction("Create");

            return RedirectToAction("Index");
        }

        // EDIT GET
        public ActionResult Edit(int id)
        {
            InstituteTypeModel model = new InstituteTypeModel();

            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_InstituteType_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.ID = Convert.ToInt32(dr["ID"]);
                    model.Title = dr["Title"].ToString();
                    model.ShortName = dr["ShortName"].ToString();
                    model.Description = dr["Description"].ToString();
                }
            }

            return View(model);
        }

        // EDIT POST
        [HttpPost]
        public ActionResult Edit(InstituteTypeModel model)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_InstituteType_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@ShortName", model.ShortName);
                cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_InstituteType_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // STATUS
        public ActionResult ChangeStatus(int id)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_InstituteType_ChangeStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
