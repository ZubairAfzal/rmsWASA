using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            if (!string.IsNullOrEmpty(Session["UserName"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            var db = new rmsWasa01Entities();
            if (userName != null || password != null)
            {
                var user = db.tblUsers.SingleOrDefault(item => item.userLoginName.Equals(userName.Trim()));
                if (user != null)
                {
                    if (user.userPassword.Trim().Equals(password.Trim()))
                    {
                        var userLogged = (from u in db.tblUsers where u.userLoginName == userName select u).FirstOrDefault();
                        Session["UserName"] = user.userFullName;
                        Session["CompanyID"] = user.companyID;
                        Session["UserID"] = user.userID;
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                ViewBag.Login = false;
                ViewBag.message = "The user name or password provided is incorrect.";
                ViewBag.messageType = "error";
                //return Content("false");
                return View();
            }
            return View();
            //DataTable dt = new DataTable();
            ////string query = "select * from tblUser";
            //string query = "select userFullName, userID, companyID from tblUser where userLoginName = '"+userName+"' and userPassword = '"+password+"' ";
            //using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        conn.Open();
            //        SqlCommand cmd = new SqlCommand(query, conn);
            //        SqlDataAdapter da = new SqlDataAdapter(query, conn);
            //        da.Fill(dt);
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    //return new SelectList(theResourceTypes, "Value", "Text", "id");
            //}
            //if (dt.Rows.Count > 0)
            //{
            //    DataRow dr = dt.Rows[0];
            //    Session["UserName"] = dr["userFullName"].ToString();
            //    Session["UserID"] = dr["userID"].ToString();
            //    Session["CompanyID"] = dr["companyID"].ToString();
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    return View();
            //}
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            int u_id = Convert.ToInt32(Session["UserID"]);
            string oldPass = "";
            string query = "select  userPassword  from tblUser where userID = " + u_id + " ";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    oldPass = cmd.ExecuteScalar().ToString();
                    conn.Close();
                }
                catch (Exception ex)
                {

                }
                //return new SelectList(theResourceTypes, "Value", "Text", "id");
            }
            if (currentPassword == oldPass && newPassword == confirmNewPassword)
            {
                string queryUpdate = "update tblUser set userPassword = '" + newPassword + "' where userID = " + u_id + " ";
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(queryUpdate, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                    //return new SelectList(theResourceTypes, "Value", "Text", "id");
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Login", "Account");
        }

    }
}