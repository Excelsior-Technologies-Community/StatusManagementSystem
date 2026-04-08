using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;
using System.IO;

namespace StatusManagementSystem.Controllers
{
    public class InstituteController : Controller
    {
        private readonly string cs = "Server=.;Database=StatusDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public List<SelectListItem> GetDropdown(string sp)
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
                        Value = dr["ID"].ToString(),
                        Text = dr["Title"].ToString()
                    });
                }
            }

            return list;
        }


        public JsonResult GetStates(long countryId)
        {
            var list = new List<object>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_State_ByCountry", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CountryId", countryId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        id = dr["ID"],
                        text = dr["Title"]
                    });
                }
            }

            return Json(list);
        }


        public JsonResult GetCities(long stateId)
        {
            var list = new List<object>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_City_ByState", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StateId", stateId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        id = dr["ID"],
                        text = dr["Title"]
                    });
                }
            }

            return Json(list);
        }

        public JsonResult GetAreas(long cityId)
        {
            var list = new List<object>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Area_ByCity", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CityId", cityId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        id = dr["ID"],
                        text = dr["Title"]
                    });
                }
            }

            return Json(list);
        }

        public ActionResult Index()
        {
            List<InstituteModel> list = new List<InstituteModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Institute_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new InstituteModel
                    {
                        ID = Convert.ToInt64(dr["ID"]),
                        InstituteTypeName = dr["InstituteTypeName"].ToString(),
                        InstituteName = dr["InstituteName"].ToString(),
                        ContactNumber = dr["ContactNumber"].ToString(),
                        Email = dr["Email"].ToString(),
                        CountryName = dr["CountryName"].ToString(),
                        StateName = dr["StateName"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString(),
                        Institutecode = dr["Institutecode"].ToString()
                    });
                }
            }

            return View(list);
        }

        public ActionResult Create()
        {
            InstituteModel model = new InstituteModel();

            model.InstituteTypeList = GetDropdown("sp_InstituteType_GetAll");
            model.CountryList = GetDropdown("sp_Country_Dropdown");

            return View(model);
        }


        [HttpPost]
        public ActionResult Create(InstituteModel model, IFormFile file, string submit)
        {
            string fileName = "";

            if (file != null)
            {
                fileName = Path.GetFileName(file.FileName);
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string path = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Institute_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@InstituteTypeId", model.InstituteTypeId);
                cmd.Parameters.AddWithValue("@InstituteName", model.InstituteName);
                cmd.Parameters.AddWithValue("@ContactNumber", model.ContactNumber ?? "");
                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@Website", model.Website ?? "");
                cmd.Parameters.AddWithValue("@Institutecode", model.Institutecode ?? "");
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);
                cmd.Parameters.AddWithValue("@StateId", model.StateId);
                cmd.Parameters.AddWithValue("@CityId", model.CityId);
                cmd.Parameters.AddWithValue("@AreaId", model.AreaId);
                cmd.Parameters.AddWithValue("@Address", model.Address ?? "");
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
                cmd.Parameters.AddWithValue("@InstituteLogo", fileName);
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        public ActionResult Edit(long id)
        {
            InstituteModel model = new InstituteModel();

            model.InstituteTypeList = GetDropdown("sp_InstituteType_GetAll");
            model.CountryList = GetDropdown("sp_Country_Dropdown");

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Institute_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.ID = Convert.ToInt64(dr["ID"]);
                    model.InstituteTypeId = Convert.ToInt64(dr["InstituteTypeId"]);
                    model.InstituteName = dr["InstituteName"].ToString();
                    model.ContactNumber = dr["ContactNumber"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.Website = dr["Website"].ToString();
                    model.Institutecode = dr["Institutecode"].ToString();
                    model.CountryId = Convert.ToInt64(dr["CountryId"]);
                    model.StateId = dr["StateId"] == DBNull.Value ? null : (long?)Convert.ToInt64(dr["StateId"]);
                    model.CityId = dr["CityId"] == DBNull.Value ? null : (long?)Convert.ToInt64(dr["CityId"]);
                    model.AreaId = dr["AreaId"] == DBNull.Value ? null : (long?)Convert.ToInt64(dr["AreaId"]);
                    model.Address = dr["Address"].ToString();
                    model.Remarks = dr["Remarks"].ToString();
                    model.InstituteLogo = dr["InstituteLogo"].ToString();
                }
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(InstituteModel model, IFormFile file)
        {
            string fileName = model.InstituteLogo;

            // Upload new file if selected
            if (file != null && file.Length > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string path = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Institute_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@InstituteTypeId", model.InstituteTypeId);
                cmd.Parameters.AddWithValue("@InstituteName", model.InstituteName);
                cmd.Parameters.AddWithValue("@ContactNumber", model.ContactNumber ?? "");
                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@Website", model.Website ?? "");
                cmd.Parameters.AddWithValue("@Institutecode", model.Institutecode ?? "");
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);
                cmd.Parameters.AddWithValue("@StateId", model.StateId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CityId", model.CityId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AreaId", model.AreaId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", model.Address ?? "");
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
                cmd.Parameters.AddWithValue("@InstituteLogo", fileName);
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
                SqlCommand cmd = new SqlCommand("sp_Institute_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        public ActionResult ChangeStatus(long id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_Institute_ChangeStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


    }
}
