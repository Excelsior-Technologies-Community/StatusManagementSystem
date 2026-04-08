using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class ExamScheduleController : Controller
    {
        private readonly string cs = "Server=.;Database=StatusDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public ActionResult Index()
        {
            List<ExamScheduleVM> list = new List<ExamScheduleVM>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamSchedule_GetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new ExamScheduleVM
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        Title = dr["Title"].ToString(),
                        ExamTypeName = dr["ExamTypeName"].ToString(),
                        ProviderName = dr["ProviderName"].ToString(),
                        CenterName = dr["CenterName"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    });
                }
            }

            return View(list);
        }


        public ActionResult Create()
        {
            ExamScheduleVM model = new ExamScheduleVM();

            model.ExamTypeList = GetExamTypes();
            model.ProviderList = GetProviders();
            model.CenterList = GetCenters();

            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ExamScheduleVM model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamSchedule_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@ExamTypeId", model.ExamTypeId);
                cmd.Parameters.AddWithValue("@ResultDate", model.ResultDate);
                cmd.Parameters.AddWithValue("@ExamDetailsJson", model.ExamDetailsJson);
                cmd.Parameters.AddWithValue("@CreateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            ExamScheduleVM model = new ExamScheduleVM();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamSchedule_GetById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                // MASTER
                if (dr.Read())
                {
                    model.Id = id;
                    model.Title = dr["Title"].ToString();
                    model.ExamTypeId = Convert.ToInt64(dr["ExamTypeId"]);
                    model.ResultDate = dr["ResultDate"] != DBNull.Value
                        ? Convert.ToDateTime(dr["ResultDate"])
                        : (DateTime?)null;
                }

                // DETAILS
                if (dr.NextResult())
                {
                    while (dr.Read())
                    {
                        model.ExamDetails.Add(new ExamDetailVM
                        {
                            ProviderId = Convert.ToInt64(dr["ProviderId"]),
                            ProviderName = dr["ProviderName"].ToString(),
                            CenterId = Convert.ToInt64(dr["CenterId"]),
                            CenterName = dr["CenterName"].ToString(),
                            ExamDate = Convert.ToDateTime(dr["ExamDate"]),
                            FromTime = TimeSpan.Parse(dr["ExamFromTime"].ToString()),
                            ToTime = TimeSpan.Parse(dr["ExamToTime"].ToString())
                        });
                    }
                }
            }

            model.ExamTypeList = GetExamTypes();

            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(ExamScheduleVM model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamSchedule_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@ExamTypeId", model.ExamTypeId);
                cmd.Parameters.AddWithValue("@ResultDate", model.ResultDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExamDetailsJson", model.ExamDetailsJson);
                cmd.Parameters.AddWithValue("@UpdateUser", 1);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        public ActionResult ToggleStatus(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamSchedule_ToggleStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_ExamSchedule_Delete", con);
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

        private List<SelectListItem> GetProviders()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT ID, Title FROM tbl_Exam_Provider WHERE StatusFlag='Active'", con);
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

        private List<SelectListItem> GetCenters()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT ID, ExamCenterName FROM tbl_Exam_Center WHERE StatusFlag='Active'", con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr["ID"].ToString(),
                        Text = dr["ExamCenterName"].ToString()
                    });
                }
            }

            return list;
        }


        public JsonResult GetProvidersByExamType(long examTypeId)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ID, Title FROM tbl_Exam_Provider WHERE ExamTypeID=@ExamTypeId AND StatusFlag='Active'", con);

                cmd.Parameters.AddWithValue("@ExamTypeId", examTypeId);

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

            return Json(list);
        }

        public JsonResult GetCentersByProvider(long providerId)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT ID, ExamCenterName FROM tbl_Exam_Center WHERE ExamProviderId=@ProviderId AND StatusFlag='Active'", con);

                cmd.Parameters.AddWithValue("@ProviderId", providerId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = dr["ID"].ToString(),
                        Text = dr["ExamCenterName"].ToString()
                    });
                }
            }

            return Json(list);
        }


    }
}
