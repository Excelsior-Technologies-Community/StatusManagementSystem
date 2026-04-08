using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly IConfiguration _config;

        public CurrencyController(IConfiguration config)
        {
            _config = config;
        }

        string GetCon() => _config.GetConnectionString("DefaultConnection");

        public List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Country_Dropdown", con);
                cmd.CommandType = CommandType.StoredProcedure;

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


        

        // INDEX
        public ActionResult Index()
        {
            List<CurrencyModel> list = new List<CurrencyModel>();

            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Currency_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new CurrencyModel
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString(),
                        CountryName = dr["CountryName"].ToString()
                    });
                }
            }

            return View(list);
        }

        // CREATE GET
        public ActionResult Create()
        {
            CurrencyModel model = new CurrencyModel();
            model.CountryList = GetCountryList();
            return View(model);
        }

        // CREATE POST
        [HttpPost]
        public ActionResult Create(CurrencyModel model, string submit)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Currency_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);
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
            CurrencyModel model = new CurrencyModel();
            model.CountryList = GetCountryList();

            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Currency_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.ID = Convert.ToInt32(dr["ID"]);
                    model.Title = dr["Title"].ToString();
                    model.Description = dr["Description"].ToString();
                    model.CountryId = Convert.ToInt64(dr["CountryId"]);
                }
            }

            return View(model);
        }

        // EDIT POST
        [HttpPost]
        public ActionResult Edit(CurrencyModel model)
        {
            using (SqlConnection con = new SqlConnection(GetCon()))
            {
                SqlCommand cmd = new SqlCommand("sp_Currency_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);
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
                SqlCommand cmd = new SqlCommand("sp_Currency_Delete", con);
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
                SqlCommand cmd = new SqlCommand("sp_Currency_ChangeStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

    }
}
