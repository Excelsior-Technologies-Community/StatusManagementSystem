using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class ExamCenterController : Controller
    {
        private readonly string cs = "Server=.;Database=StatusDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public ActionResult Index()
        {
            List<ExamCenterVM> list = new List<ExamCenterVM>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamCenter_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new ExamCenterVM
                    {
                        Id = Convert.ToInt64(dr["ID"]),
                        ExamCenterName = dr["ExamCenterName"].ToString(),
                        ProviderName = dr["ProviderName"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    });
                }
            }

            return View(list);
        }

        public ActionResult Create()
        {
            var model = new ExamCenterVM
            {
                ProviderList = GetProviders(),
                CountryList = GetCountries()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ExamCenterVM model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamCenter_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ExamTypeId", model.ExamTypeId);
                cmd.Parameters.AddWithValue("@ExamProviderId", model.ExamProviderId);
                cmd.Parameters.AddWithValue("@ExamCenterName", model.ExamCenterName);
                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo ?? "");
                cmd.Parameters.AddWithValue("@Address", model.Address ?? "");
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);
                cmd.Parameters.AddWithValue("@StateId", model.StateId);
                cmd.Parameters.AddWithValue("@CityId", model.CityId);
                cmd.Parameters.AddWithValue("@AreaId", model.AreaId);
                cmd.Parameters.AddWithValue("@Pincode", model.Pincode ?? "");
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(long id)
        {
            ExamCenterVM model = new ExamCenterVM();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamCenter_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.Id = id;
                    model.ExamCenterName = dr["ExamCenterName"].ToString();
                    model.ExamProviderId = Convert.ToInt64(dr["ExamProviderId"]);
                    model.CountryId = Convert.ToInt64(dr["CountryId"]);
                    model.StateId = Convert.ToInt64(dr["StateId"]);
                    model.CityId = Convert.ToInt64(dr["CityId"]);
                    model.AreaId = Convert.ToInt64(dr["AreaId"]);
                    model.Email = dr["Email"].ToString();
                    model.MobileNo = dr["MobileNo"].ToString();
                    model.Address = dr["Address"].ToString();
                    model.Pincode = dr["Pincode"].ToString();
                }
            }

            model.ProviderList = GetProviders();
            model.CountryList = GetCountries();
            model.StateList = GetStatesList(model.CountryId);
            model.CityList = GetCitiesList(model.StateId);
            model.AreaList = GetAreasList(model.CityId);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ExamCenterVM model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamCenter_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@ExamTypeId", model.ExamTypeId);
                cmd.Parameters.AddWithValue("@ExamProviderId", model.ExamProviderId);
                cmd.Parameters.AddWithValue("@ExamCenterName", model.ExamCenterName);
                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo ?? "");
                cmd.Parameters.AddWithValue("@Address", model.Address ?? "");
                cmd.Parameters.AddWithValue("@CountryId", model.CountryId);
                cmd.Parameters.AddWithValue("@StateId", model.StateId);
                cmd.Parameters.AddWithValue("@CityId", model.CityId);
                cmd.Parameters.AddWithValue("@AreaId", model.AreaId);
                cmd.Parameters.AddWithValue("@Pincode", model.Pincode ?? "");
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
                SqlCommand cmd = new SqlCommand("sp_ExamCenter_Delete", con);
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
                SqlCommand cmd = new SqlCommand("sp_ExamCenter_ToggleStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        

        private List<SelectListItem> GetCountries()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT CountryId, CountryName FROM tbl_Country", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr["CountryId"].ToString(),
                        Text = dr["CountryName"].ToString()
                    });
                }
            }
            return list;
        }

        private List<SelectListItem> GetStatesList(long countryId)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT StateId, StateName FROM tbl_State WHERE CountryId=@CountryId", con);
                cmd.Parameters.AddWithValue("@CountryId", countryId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr["StateId"].ToString(),
                        Text = dr["StateName"].ToString()
                    });
                }
            }

            return list;
        }

        private List<SelectListItem> GetCitiesList(long stateId)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT CityId, CityName FROM tbl_City WHERE StateId=@StateId", con);
                cmd.Parameters.AddWithValue("@StateId", stateId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr["CityId"].ToString(),
                        Text = dr["CityName"].ToString()
                    });
                }
            }

            return list;
        }

        private List<SelectListItem> GetAreasList(long cityId)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT AreaId, AreaName FROM tbl_Area WHERE CityId=@CityId", con);
                cmd.Parameters.AddWithValue("@CityId", cityId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr["AreaId"].ToString(),
                        Text = dr["AreaName"].ToString()
                    });
                }
            }

            return list;
        }

        private List<SelectListItem> GetProviders()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT ID, Title FROM tbl_Exam_Provider", con);
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
            var list = GetStatesList(countryId);
            return Json(list);
        }

        public JsonResult GetCities(long stateId)
        {
            var list = GetCitiesList(stateId);
            return Json(list);
        }

        public JsonResult GetAreas(long cityId)
        {
            var list = GetAreasList(cityId);
            return Json(list);
        }

    }
}
