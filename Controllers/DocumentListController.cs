using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StatusManagementSystem.Models;
using System.Data;

namespace StatusManagementSystem.Controllers
{
    public class DocumentListController : Controller
    {
        private readonly IConfiguration _config;
        public DocumentListController(IConfiguration config)
        {
            _config = config;
        }

        string cs => _config.GetConnectionString("DefaultConnection");

        // ================= LIST =================
        public IActionResult Index()
        {
            List<DocumentModel> list = new List<DocumentModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetDocuments", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new DocumentModel
                    {
                        Id = Convert.ToInt64(dr["Id"]),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        DocumentTypeName = dr["DocumentTypeName"].ToString(),
                        StatusFlag = dr["StatusFlag"].ToString()
                    });
                }
            }

            return View(list);
        }

        // ================= DROPDOWN =================
        public List<SelectListItem> GetDocumentTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT ID, Title FROM tbl_Document_Type_mst WHERE StatusFlag='A'", con);
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

        // ================= CREATE =================
        public IActionResult Create()
        {
            ViewBag.DocTypes = GetDocumentTypes();
            return View();
        }

        [HttpPost]
        public IActionResult Create(DocumentModel model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DocumentTypeId", model.DocumentTypeId);
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
            DocumentModel model = new DocumentModel();
            ViewBag.DocTypes = GetDocumentTypes();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_GetDocumentById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model.Id = Convert.ToInt64(dr["Id"]);
                    model.Title = dr["Title"].ToString();
                    model.Description = dr["Description"].ToString();
                    model.DocumentTypeId = Convert.ToInt64(dr["DocumentTypeId"]);
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(DocumentModel model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@DocumentTypeId", model.DocumentTypeId);
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
                SqlCommand cmd = new SqlCommand("sp_DeleteDocument", con);
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
                SqlCommand cmd = new SqlCommand("sp_ChangeDocumentStatus", con);
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
